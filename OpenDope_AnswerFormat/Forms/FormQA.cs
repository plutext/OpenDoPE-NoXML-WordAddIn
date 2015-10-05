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
//using Microsoft.Office.Tools.Word.Extensions; // for VSTOObject

namespace OpenDope_AnswerFormat
{
    public partial class FormQA : Form
    {
        static Logger log = LogManager.GetLogger("FormQA");

        private question q;

        private Office.CustomXMLPart answersPart;

        private List<string> answerID = new List<string>();

        bool bindToControl = false;

        protected Word.ContentControl cc;
        protected Model model;

        protected Office.CustomXMLPart questionsPart;
        //private question q;
        protected questionnaire questionnaire;

        protected XPathsPartEntry xppe;

        private Helpers.QuestionListHelper questionListHelper;

        public FormQA(Word.ContentControl cc) 
        {
            bindToControl = true;
            init(cc);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cc"></param>
        /// <param name="bindToControl">This form can be invoked from
        /// the add condition form.  In that case, we don't want to
        /// actually bind to content control, nor do we want them
        /// to be able to re-use.</param>
        public FormQA(Word.ContentControl cc, bool bindToControl) 
        {
            this.bindToControl = bindToControl;
            init(cc);

            if (!bindToControl)
            {
                // If we're creating a new question from FormCondition,
                // don't let them try to re-use!
                this.tabPageReuseQ.Hide();
            }
        }

        private void init(Word.ContentControl cc)
        {

            InitializeComponent();

            //base.listBoxTypeFilter = this.listBoxTypeFilter;
            //base.listBoxQuestions = this.listBoxQuestions;
            //base.checkBoxScope = this.checkBoxScope;

            // NET 4 way; see http://msdn.microsoft.com/en-us/library/microsoft.office.tools.word.extensions.aspx
            FabDocxState fabDocxState = (FabDocxState)Globals.Factory.GetVstoObject(Globals.ThisAddIn.Application.ActiveDocument).Tag;

            // NET 3.5 way, which requires using Microsoft.Office.Tools.Word.Extensions
            //FabDocxState fabDocxState = (FabDocxState)Globals.ThisAddIn.Application.ActiveDocument.GetVstoObject(Globals.Factory).Tag;
            this.model = fabDocxState.model;
            xppe = new XPathsPartEntry(model);

            this.cc = cc;


            this.questionsPart = model.questionsPart;
            questionnaire = new questionnaire();
            questionnaire.Deserialize(questionsPart.XML, out questionnaire);

            answersPart = model.answersPart; //.userParts[0]; // TODO: make this better
            Office.CustomXMLNodes answers = answersPart.SelectNodes("//oda:answer");
            foreach (Office.CustomXMLNode answer in answers)
            {
                this.answerID.Add(CustomXMLNodeHelper.getAttribute(answer, "id"));
                    //answer.Attributes[1].NodeValue); // ID
            }

            // 
            // controlQuestionVaryWhichRepeat1
            // 
            controlQuestionVaryWhichRepeat1.init(cc, questionnaire, xppe);
            groupBoxRepeat.Visible = controlQuestionVaryWhichRepeat1.shouldShow();


            // Set the focus on the text box
            this.ActiveControl=this.controlQuestionCommon1.textBoxQuestionText;


            if (bindToControl)
            {
                // Show the re-use tab

                questionListHelper = new Helpers.QuestionListHelper(model, xppe, questionnaire, cc);
                questionListHelper.listBoxTypeFilter = listBoxTypeFilter;
                questionListHelper.listBoxQuestions = listBoxQuestions;
                questionListHelper.checkBoxScope = checkBoxScope;

                questionListHelper.populateTypeFilter(false);

                // for init, populate with all questions
                questionListHelper.populateQuestions(null);

                this.listBoxTypeFilter.SelectedIndexChanged += new System.EventHandler(questionListHelper.listBoxTypeFilter_SelectedIndexChanged);

            }

        }

        //void answerID_TextChanged(object sender, System.EventArgs e)
        //{
        //    // this fires on every keystroke.
        //    // what we want is, is as soon as the value
        //    // matches a pre-existing ID, to grey out the other fields

        //    string currentVal = this.answerID.Text;
        //    q = isExistingID(currentVal);
        //    if (q==null)
        //    {
        //        // Do nothing .. leave other fields as is, 
        //        // so user can edit to suit
        //    }
        //    else
        //    {
        //        // Populate with correct value
        //        // .. so first find answer
        //        //textBoxMetaDesc.Enabled = false;
        //        textBoxQuestionText.Text = q.text.Value;

        //        if (q.response != null
        //            && q.response.Item != null
        //            && q.response.Item is responseFixed)
        //        {
        //            // check ...
        //            this.radioButtonMCYes.Checked = true;
        //        }
        //        else
        //        {
        //            this.radioButtonMCYes.Checked = false;
        //        }

        //    }
        //}

        //private question isExistingID(string val)
        //{
        //    foreach (question q in questionnaire.questions)
        //    {
        //        if ( val.Equals(
        //                q.id )) return q;
        //    }
        //    return null;
        //}

        private void buttonNext_Click(object sender, EventArgs e)
        {
            //log.Info(".Text:" + answerID.Text);
            //log.Info(".SelectedText:" + answerID.SelectedText);

            //log.Info(".textBoxQuestionText:" + textBoxQuestionText.Text);

            if (!this.controlQuestionCommon1.isValid() )
            {
                Mbox.ShowSimpleMsgBoxError("You need to enter the text of the question!");
                DialogResult = DialogResult.None; // or use on closing event; see http://stackoverflow.com/questions/2499644/preventing-a-dialog-from-closing-in-the-buttons-click-event-handler
                return;
            }

            q = new question();
            //q.id = ID;
            this.controlQuestionCommon1.populateQuestion(q);

            // Generate a nice ID
            // .. first we need a list of existing IDs
            List<string> reserved = new List<string>();
            foreach (question qx in questionnaire.questions)
            {
                reserved.Add(qx.id);
            }
            q.id = IdHelper.SuggestID(q.text, reserved);

            questionnaire.questions.Add(q);

            if (this.radioButtonMCYes.Checked)
            {
                mcq();
            }
            else
            {
                notMcq();
            }

        }

        private void notMcq()
        {

            // Responses
            response responses = q.response;
            if (responses == null)
            {
                responses = new response();
                q.response = responses;
            }

            FormDataType formDataType = new FormDataType();
            //formDataType.controlDataTypeMAIN1.controlDataType1.initializeOperators();
            formDataType.ShowDialog();


            string sampleAnswer = formDataType.controlDataTypeMAIN1.controlDataType1.getSampleAnswerProcessed();

            bool required = formDataType.controlDataTypeMAIN1.controlDataType1.getRequired();

            q.hint = formDataType.controlDataTypeMAIN1.controlDataType1.getHint();

            responseFree responseFree = new responseFree();
            responses.Item = responseFree;

            // Reduce chance of ID collision if saved to library
            q.id = q.id + "_" + IdHelper.GenerateShortID(2);

            TagData td;

            // Work out where to put this answer
            // It will just go in /answers, unless 
            // user has indicated in treeview that it
            // varies in repeat.
            Word.ContentControl repeatAncestor = controlQuestionVaryWhichRepeat1.getRepeatAncestor(cc);


            // We want something in the content control,
            // which helps the user understand their document.
            // The title is the question, so a sensible thing to
            // have here is an answer.
            // We can do this via placeholder text:
            // this.cc.SetPlaceholderText(null, null, q.text);
            // but better is to take advantage of databinding!

            //string xml = "<answer id=\"" + ID + "\" xmlns=\"http://opendope.org/answers\"/>";
            string xml = "<oda:answer id=\"" + q.id + "\" xmlns:oda=\"http://opendope.org/answers\">" + sampleAnswer + "</oda:answer>";
            // Must include ns dec in xml string, but using
            // default ns seems to confuse JAXB (or more likely,
            // whatever XML parser CustomXmlDataStoragePart is using)

            string xpath;
            if (repeatAncestor == null)
            {
                // create it in /answers
                Office.CustomXMLNode node = answersPart.SelectSingleNode("/oda:answers");

                node.AppendChildSubtree(xml);
                xpath = "/oda:answers/oda:answer[@id='" + q.id + "']";

            }
            else
            {
                td = new TagData(repeatAncestor.Tag);
                string ancestorRepeatXPathID = td.getRepeatID();

                // Get the XPath, to find the question ID,
                // which is what we use to find the repeat answer.
                xpathsXpath xp = xppe.getXPathByID(ancestorRepeatXPathID);

                Office.CustomXMLNode node = answersPart.SelectSingleNode("//oda:repeat[@qref='" + xp.questionID + "']/oda:row");

                string parentXPath = NodeToXPath.getXPath(node);

                node.AppendChildSubtree(xml);

                xpath = parentXPath + "/oda:answer[@id='" + q.id + "']";
            }

            log.Info(answersPart.XML);


            // Bind to answer XPath
            xpathsXpath xpathEntry = xppe.setup("", answersPart.Id, xpath, null, false);
            xpathEntry.questionID = q.id;
            xpathEntry.dataBinding.prefixMappings = "xmlns:oda='http://opendope.org/answers'";

            formDataType.controlDataTypeMAIN1.controlDataType1.populateXPath(xpathEntry);


            // save is below

            if (bindToControl)
            {

                if (xpathEntry.type.Equals("date"))
                {
                    cc.Type = Word.WdContentControlType.wdContentControlDate;
                    cc.DateStorageFormat = Word.WdContentControlDateStorageFormat.wdContentControlDateStorageDate;
                    // Default is DateTime, but to be valid on our XForm, we need just date
                }

                td = new TagData("");
                td.set("od:xpath", xppe.xpathId);
                cc.Tag = td.asQueryString();

                // TODO - allow ID to be specified


                // At this point, answer should be present.  Sanity check!

                // TODO

                //cc.Title = "Data Value [" + this.answerID.Text + "]"; // // This used if they later click edit
                cc.Title = q.text;
                cc.XMLMapping.SetMapping(xpath, "xmlns:oda='http://opendope.org/answers'", model.answersPart); //model.userParts[0]);
            }

            formDataType.Dispose();

            // Save XPaths part
            xppe.save();

            // Finally, add to part
            updateQuestionsPart();

            this.Close();


        }

        private void mcq()
        {
            // Responses
            response responses = q.response;
            if (responses == null)
            {
                responses = new response();
                q.response = responses;
            }

            string dataType = null;
            string sampleAnswer = null;
            bool required = false;

            // MCQ: display response form
            responseFixed responseFixed = new responseFixed();
            responses.Item = responseFixed;
            FormResponses formResponses = new FormResponses(responseFixed);
            formResponses.ShowDialog();

            // set data type
            dataType = formResponses.controlQuestionResponsesFixed1.getDataType();

            appearanceType apt = formResponses.controlQuestionResponsesFixed1.getAppearanceType();
            q.appearance = apt;
            q.appearanceSpecified = true;

            sampleAnswer = formResponses.controlQuestionResponsesFixed1.getDefault();

            if (string.IsNullOrWhiteSpace(sampleAnswer))
            {
                sampleAnswer = "«multiple choice»";
            }

            formResponses.Dispose();
            // TODO - handle cancel

            // Reduce chance of ID collision if saved to library
            q.id = q.id + "_" + IdHelper.GenerateShortID(2);

            TagData td;

            // Work out where to put this answer
            // It will just go in /answers, unless 
            // user has indicated in treeview that it
            // varies in repeat.
            Word.ContentControl repeatAncestor = controlQuestionVaryWhichRepeat1.getRepeatAncestor(cc);


            // We want something in the content control,
            // which helps the user understand their document.
            // The title is the question, so a sensible thing to
            // have here is an answer.
            // We can do this via placeholder text:
            // this.cc.SetPlaceholderText(null, null, q.text);
            // but better is to take advantage of databinding!

            //string xml = "<answer id=\"" + ID + "\" xmlns=\"http://opendope.org/answers\"/>";
            string xml = "<oda:answer id=\"" + q.id + "\" xmlns:oda=\"http://opendope.org/answers\">" + sampleAnswer + "</oda:answer>";
            // Must include ns dec in xml string, but using
            // default ns seems to confuse JAXB (or more likely,
            // whatever XML parser CustomXmlDataStoragePart is using)

            string xpath;
            if (repeatAncestor == null)
            {
                // create it in /answers
                Office.CustomXMLNode node = answersPart.SelectSingleNode("/oda:answers");

                node.AppendChildSubtree(xml);
                xpath = "/oda:answers/oda:answer[@id='" + q.id + "']";

            }
            else
            {
                td = new TagData(repeatAncestor.Tag);
                string ancestorRepeatXPathID = td.getRepeatID();

                // Get the XPath, to find the question ID,
                // which is what we use to find the repeat answer.
                xpathsXpath xp = xppe.getXPathByID(ancestorRepeatXPathID);

                Office.CustomXMLNode node = answersPart.SelectSingleNode("//oda:repeat[@qref='" + xp.questionID + "']/oda:row");

                string parentXPath = NodeToXPath.getXPath(node);

                node.AppendChildSubtree(xml);

                xpath = parentXPath + "/oda:answer[@id='" + q.id + "']";
            }

            log.Info(answersPart.XML);


            // Bind to answer XPath
            xpathsXpath xpathEntry = xppe.setup("", answersPart.Id, xpath, null, false);
            xpathEntry.questionID = q.id;
            xpathEntry.dataBinding.prefixMappings = "xmlns:oda='http://opendope.org/answers'";
            xpathEntry.type = dataType;
            xpathEntry.required = required;
            xpathEntry.requiredSpecified = true;

            // save is below

            if (bindToControl)
            {
                td = new TagData("");
                td.set("od:xpath", xppe.xpathId);
                cc.Tag = td.asQueryString();

                // TODO - allow ID to be specified


                // At this point, answer should be present.  Sanity check!

                // TODO

                //cc.Title = "Data Value [" + this.answerID.Text + "]"; // // This used if they later click edit
                cc.Title = q.text;
                cc.XMLMapping.SetMapping(xpath, "xmlns:oda='http://opendope.org/answers'", model.answersPart); //model.userParts[0]);
            }



            // Save XPaths part
            xppe.save();

            // Finally, add to part
            updateQuestionsPart();

            this.Close();

        }

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

        /// <summary>
        /// filter questions by data type
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void listBoxTypeFilter_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            throw new System.NotImplementedException();
        }


        private void buttonReuseOK_Click(object sender, EventArgs e)
        {
            if (this.listBoxQuestions.SelectedItem == null)
            {
                MessageBox.Show("You must select a question!");
                DialogResult = DialogResult.None; // or use on closing event; see http://stackoverflow.com/questions/2499644/preventing-a-dialog-from-closing-in-the-buttons-click-event-handler
                return;
            }

            question q = this.listBoxQuestions.SelectedItem as question;

            // Get its XPath
            xpathsXpath xpathObj = xppe.getXPathByQuestionID(q.id);

            TagData td = new TagData("");
            td.set("od:xpath", xpathObj.id);
            cc.Tag = td.asQueryString();
            cc.Title = q.text;
            cc.XMLMapping.SetMapping(xpathObj.dataBinding.xpath, 
                "xmlns:oda='http://opendope.org/answers'", model.answersPart);

            if (xpathObj.type!=null
                && xpathObj.type.Equals("date"))
            {
                cc.Type = Word.WdContentControlType.wdContentControlDate;
                cc.DateStorageFormat = Word.WdContentControlDateStorageFormat.wdContentControlDateStorageDate;
                // Default is DateTime, but to be valid on our XForm, we need just date

            }


            // They could be re-using this question somewhere
            // outside its repeat scope.  This should handle that.
            ContentControlCopyHandler handler = new ContentControlCopyHandler();
            handler.handle(cc);

        }



        private void buttonClone_Click(object sender, EventArgs e)
        {

        }

    }
}
