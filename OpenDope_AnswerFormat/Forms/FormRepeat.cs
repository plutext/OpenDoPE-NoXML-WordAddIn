/*
 *  OpenDoPE authoring Word AddIn
    Copyright (C) Plutext Pty Ltd, 2012
 * 
    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
//using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using NLog;

using Office = Microsoft.Office.Core;
using OpenDoPEModel;
using Word = Microsoft.Office.Interop.Word;

namespace OpenDope_AnswerFormat
{
    /// <summary>
    /// This form has a tab for creating a new repeat,
    /// and another for re-using an existing one.
    /// 
    /// We don't actually have to handle clicking
    /// between tabs.
    /// 
    /// The repeat question text should be
    /// unique, since that's what is shown in
    /// the XForm, and if the author seeks to re-use
    /// a repeat.
    /// 
    /// </summary>
    public partial class FormRepeat : Form
    {
        static Logger log = LogManager.GetLogger("OpenDoPE_Wed");

        private Word.ContentControl cc;

        private Model model;
        private XPathsPartEntry xppe;

        private Office.CustomXMLPart questionsPart;
        private question q;
        private questionnaire questionnaire;
        public questionnaire getQuestionnaire()
        {
            return questionnaire;
        }

        private Office.CustomXMLPart answersPart;

        private List<string> answerID = new List<string>();

        public string ID { get; set; }

        public FormRepeat(Office.CustomXMLPart questionsPart,
            Office.CustomXMLPart answersPart,
            Model model,
            Word.ContentControl cc)
        {
            InitializeComponent();

            this.model = model;
            this.cc = cc;

            this.questionsPart = questionsPart;
            questionnaire = new questionnaire();
            questionnaire.Deserialize(questionsPart.XML, out questionnaire);

            this.answersPart = answersPart;

            Office.CustomXMLNodes answers = answersPart.SelectNodes("//oda:repeat");
            foreach (Office.CustomXMLNode answer in answers)
            {
                this.answerID.Add(CustomXMLNodeHelper.getAttribute(answer, "qref")); // ID
            }

            // Suggest ID .. the idea is that
            // the question ID = answer ID.
           // this.ID = generateId();

            xppe = new XPathsPartEntry(model);
            // List of repeat names, for re-use purposes
            // (we need a map from repeat name (shown in the UI) to repeat id,
            //  which is xppe.xpathId).
            // What is it that distinguishes a repeat from any other question?
            // Answer: The fact that the XPath pointing to it ends with oda:row

            // Only show repeats which are in scope (since this repeat is not allowed elsewhere)
            // - if no ancestor repeat, then top level repeats.
            // - if there is an ancestor repeat, then just those which are direct children of it.
            Word.ContentControl rptAncestor = RepeatHelper.getYoungestRepeatAncestor(cc);
            String scope = "/oda:answers";
            if (rptAncestor != null)
            {
                // find its XPath
                scope = xppe.getXPathByID(  (new TagData(rptAncestor.Tag)).getRepeatID() ).dataBinding.xpath;

            }

            repeatNameToIdMap = new Dictionary<string, string>();
            foreach (xpathsXpath xpath in xppe.getXPaths().xpath)
            {
                if (xpath.dataBinding.xpath.EndsWith("oda:row"))
                {
                    if (isRepeatInScope(scope, xpath.dataBinding.xpath)) {
                        // the repeat "name" is its question text.
                        // Get that.
                        question q = questionnaire.getQuestion(xpath.questionID);
                        repeatNameToIdMap.Add(q.text, xpath.id);
                    }
                }
            }
            // Populate the comboBox
            foreach(KeyValuePair<String,String> entry in repeatNameToIdMap)
            {
                this.comboBoxRepeatNames.Items.Add(entry.Key);
            }
        }

        private bool isRepeatInScope(string scope, string repeatXPath)
        {
            // require just one instance of oda:row after scope
            int pos = repeatXPath.IndexOf(scope);

            if (pos <0) return false;

            string relative = repeatXPath.Substring(pos + scope.Length);

            int row = relative.IndexOf("oda:row");
            if (row <0) return false; // can't add a repeat to itself

            string rptSegment = relative.Substring(row + "oda:row".Length);

            int nextRepeat = rptSegment.IndexOf("oda:row");
            if (nextRepeat < 0) return true; // good! it's a child repeat

            return false; // it must be nested
        }

        Dictionary<String, String> repeatNameToIdMap;


        ///// <summary>
        ///// Generate a simple ID
        ///// </summary>
        ///// <returns></returns>
        //private string generateId()
        //{
        //    int i = this.answerID.Count;
        //    do
        //    {
        //        i++;
        //    } while (this.answerID.Contains("rpt" + i));

        //    return "rpt" + i;
        //}


        private void buttonNext_Click(object sender, EventArgs e)
        {
            int min = 1;
            int defautlVal = 2;
            int max = 4;
            int step = 1;

            //log.Info(".Text:" + answerID.Text);
            //log.Info(".SelectedText:" + answerID.SelectedText);

            //log.Info(".textBoxQuestionText:" + textBoxQuestionText.Text);

            if (//string.IsNullOrWhiteSpace(this.answerID.Text) ||
                 string.IsNullOrWhiteSpace(this.textBoxQuestionText.Text))
            {
                Mbox.ShowSimpleMsgBoxError("Required data missing!");
                DialogResult = DialogResult.None; // or use on closing event; see http://stackoverflow.com/questions/2499644/preventing-a-dialog-from-closing-in-the-buttons-click-event-handler
                return;
            }
            String repeatName = this.textBoxQuestionText.Text.Trim();

            try
            {
                string foo = repeatNameToIdMap[repeatName];

                Mbox.ShowSimpleMsgBoxError("You already have a repeat named '" + repeatName 
                    + "' . Click the 're-use' tab to re-use it, or choose another name.");
                DialogResult = DialogResult.None;
                return;
            }
            catch (KeyNotFoundException knfe)
            {
                // Good
            }

            // Basic data validation
            try
            {
                min = int.Parse(this.textBoxMin.Text);
                defautlVal = int.Parse(this.textBoxDefault.Text);
                max = int.Parse(this.textBoxMax.Text);
                step = int.Parse(this.textBoxRangeStep.Text);
            } catch (Exception) {
                log.Warn("Repeat range val didn't parse to int properly");
            }
            if (min < 0) min = 0;
            if (min > 20) min = 20;
            if (max < min) max = min + 4;
            if (max > 30) max = 30;
            int av = (int)Math.Round((min + max) / 2.0); 
            if (defautlVal < min || defautlVal > max)
                defautlVal = av;
            if (step > av) step = 1;

            TagData td;

            // Work out where to put this repeat
            // It will just go in /answers, unless it
            // has a repeat ancestor.
            // So look for one...
            Word.ContentControl repeatAncestor = null;
            Word.ContentControl currentCC = cc.ParentContentControl;
            while (repeatAncestor == null && currentCC != null)
            {
                if (currentCC.Tag.Contains("od:repeat"))
                {
                    repeatAncestor = currentCC;
                    break;
                }
                currentCC = currentCC.ParentContentControl;
            }

            // Generate a nice ID
            // .. first we need a list of existing IDs
            List<string> reserved = new List<string>();
            foreach (question qx in questionnaire.questions)
            {
                reserved.Add(qx.id);
            }
            ID = IdHelper.SuggestID(repeatName, reserved) + "_" + IdHelper.GenerateShortID(2);


            string xpath;
            if (repeatAncestor == null)
            {
                // create it in /answers
                Office.CustomXMLNode node = answersPart.SelectSingleNode("/oda:answers");
                //string xml = "<repeat qref=\"" + ID + "\" xmlns=\"http://opendope.org/answers\"><row/></repeat>";
                string xml = "<oda:repeat qref=\"" + ID + "\" xmlns:oda=\"http://opendope.org/answers\" ><oda:row/></oda:repeat>";

                node.AppendChildSubtree(xml);
                xpath = "/oda:answers/oda:repeat[@qref='" + ID + "']/oda:row"; // avoid trailing [1]

            } else
            {
                td = new TagData(repeatAncestor.Tag);
                string ancestorRepeatXPathID = td.getRepeatID();

                // Get the XPath, to find the question ID,
                // which is what we use to find the repeat answer.
                xpathsXpath xp = xppe.getXPathByID(ancestorRepeatXPathID);

                Office.CustomXMLNode node = answersPart.SelectSingleNode("//oda:repeat[@qref='" + xp.questionID + "']/oda:row");

                string parentXPath = NodeToXPath.getXPath(node);

                //string xml = "<repeat qref=\"" + ID + "\" xmlns=\"http://opendope.org/answers\"><row/></repeat>";
                string xml = "<oda:repeat qref=\"" + ID + "\"  xmlns:oda=\"http://opendope.org/answers\" ><oda:row/></oda:repeat>";

                node.AppendChildSubtree(xml);

                xpath = parentXPath + "/oda:repeat[@qref='" + ID + "']/oda:row"; // avoid trailing [1]
            }

            log.Info(answersPart.XML);

            // Question
            q = new question();
            //q.id = this.answerID.Text; // not SelectedText
            q.id = ID;
            q.text = repeatName;

            if (!string.IsNullOrWhiteSpace(this.textBoxHelp.Text))
            {
                q.help = this.textBoxHelp.Text;
            }
            if (!string.IsNullOrWhiteSpace(this.textBoxHint.Text))
            {
                q.hint = this.textBoxHint.Text;
            }

            if (this.isAppearanceCompact())
            {
                q.appearance = appearanceType.compact;
                q.appearanceSpecified = true;
            }
            else
            {
                q.appearanceSpecified = false;
            }

            questionnaire.questions.Add(q);

            // Bind to answer XPath
            xpathsXpath xpathEntry = xppe.setup("", answersPart.Id, xpath, null, false);
            xpathEntry.questionID = q.id;
            xpathEntry.dataBinding.prefixMappings = "xmlns:oda='http://opendope.org/answers'";
            xpathEntry.type = "nonNegativeInteger";
            xppe.save();

            // Repeat
            td = new TagData("");
            td.set("od:repeat", xppe.xpathId);
            cc.Tag = td.asQueryString();


            // At this point, answer should be present.  Sanity check!

            cc.Title = "REPEAT " + repeatName;
            cc.SetPlaceholderText(null, null, "Repeating content goes here.");

            //Not for wdContentControlRichText!
            //cc.XMLMapping.SetMapping(xpath, null, model.userParts[0]);



            // Responses
            response responses = q.response;
            if (responses == null)
            {
                responses = new response();
                q.response = responses;
            }

            // MCQ: display response form
            responseFixed responseFixed = new responseFixed();
            responses.Item = responseFixed;

            //for (int i = min; i<=max; i=i+step)
            //{
            //    responseFixedItem item = new OpenDoPEModel.responseFixedItem();

            //    item.value = ""+i;
            //    item.label = "" + i;

            //    responseFixed.item.Add(item);
            //}

            responseFixed.canSelectMany = false;

            // Finally, add to part
            updateQuestionsPart();

            this.Close();

        }

        /// <summary>
        /// betterForm will show a repeat with appearance compact
        /// as a table row.  All others (inc no appearance) are 
        /// rendered vertically down page.
        /// 
        /// So we'll only set this value if it is compact.
        /// </summary>
        /// <returns></returns>
        private bool isAppearanceCompact() {

            return this.checkBoxAppearanceCompact.Checked;
        }

        //private appearanceType getAppearanceType()
        //{
        //    // AppearanceType
        //    if (this.radioButtonAppearanceFull.Checked)
        //    {
        //        return appearanceType.full;
        //    }
        //    else if (this.radioButtonAppearanceCompact.Checked)
        //    {
        //        return appearanceType.compact;
        //    }
        //    else if (this.radioButtonAppearanceMinimal.Checked)
        //    {
        //        return appearanceType.minimal;
        //    }
        //    return appearanceType.full;
        //}


        public question getQuestion()
        {
            return q;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Mbox.ShowSimpleMsgBoxError("Not implemented yet!");
        }

        public void updateQuestionsPart()
        {
            // Save it in docx
            string result = questionnaire.Serialize();
            log.Info(result);
            CustomXmlUtilities.replaceXmlDoc(questionsPart, result);

        }

        private void buttonReuseOK_Click(object sender, EventArgs e)
        {
            if (this.comboBoxRepeatNames.SelectedItem == null)
            {
                // do nothing for now.
                // TODO: grey out the OK button unless there is a selected item
                DialogResult = DialogResult.None;
                return;
            }
            string repeatName = (string)this.comboBoxRepeatNames.SelectedItem;

            // Get the ID we need to use
            String repeatId = repeatNameToIdMap[repeatName];

            TagData td = new TagData("");
            td.set("od:repeat", repeatId);
            cc.Tag = td.asQueryString();

            cc.Title = "REPEAT " + repeatName;
            cc.SetPlaceholderText(null, null, "Repeating content goes here.");

            // They could be trying to re-usw this repeat somewhere
            // outside its ancestral repeat scope.  This should handle that.
            ContentControlCopyHandler handler = new ContentControlCopyHandler();
            handler.handle(cc);

        }

    }
}
