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
using System.Linq;
using System.Text;

using NLog;

using Office = Microsoft.Office.Core;
using OpenDoPEModel;
using Word = Microsoft.Office.Interop.Word;
//using Microsoft.Office.Tools.Word.Extensions; // for VSTOObject

using System.Windows.Forms;

namespace OpenDope_AnswerFormat.Helpers
{
    class QuestionListHelper
    {
        static Logger log = LogManager.GetLogger("QuestionListHelper");

        /// <summary>
        /// Magic type for getting the position of this instance in a repeat
        /// </summary>
        public static string REPEAT_POS = "repeated pos";

        /// <summary>
        /// Magic type for getting the number of instances this repeat has
        /// </summary>
        public static string REPEAT_COUNT = "repeat count";

        public ListBox listBoxTypeFilter { get; set; }
        public CheckBox checkBoxScope { get; set; }
        public ListBox listBoxQuestions { get; set; }

        private Model model;
        protected XPathsPartEntry xppe;
        private questionnaire questionnaire;

        protected Word.ContentControl cc;


        public QuestionListHelper(Model model, XPathsPartEntry xppe, questionnaire questionnaire, 
                    Word.ContentControl cc) {

            this.model = model;
            this.xppe = xppe;
            this.questionnaire = questionnaire;

            this.cc = cc;        
        }

        public void listBoxTypeFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            filterAction();
        }


        public void checkBoxScope_CheckedChanged(object sender, EventArgs e)
        {
            filterAction();
        }

        /// <summary>
        /// This type filter is used in 2 places.
        /// 1. FormCondition, to filter questions by type
        /// 2. FormQA, if you want to reuse an existing question.
        /// For this second usage, the magic repeat types aren't
        /// wanted.
        /// </summary>
        /// <param name="includeRptMagicTypes"></param>
        public void populateTypeFilter(bool includeRptMagicTypes)
        {
            listBoxTypeFilter.Items.Add("text");
            listBoxTypeFilter.Items.Add("number");
            listBoxTypeFilter.Items.Add("date");
            listBoxTypeFilter.Items.Add("boolean");
            listBoxTypeFilter.Items.Add("email");
            listBoxTypeFilter.Items.Add("card number");
            //this.listBoxTypeFilter.Items.Add("ALL");
            if (includeRptMagicTypes)
            {
                listBoxTypeFilter.Items.Add(REPEAT_POS);
                listBoxTypeFilter.Items.Add(REPEAT_COUNT);
            }

        }

        public void populateQuestions(string type)
        {
            listBoxQuestions.Items.Clear();

            if (type != null && type.Equals(REPEAT_COUNT))
            {
                populateRepeats();
                return;
            }


            if (type != null && type.Equals(REPEAT_POS))
            {
                // This special type does not relate to any specific repeat question
                return;
            }

            HashSet<question> questions = null;

            bool limitScope = !checkBoxScope.Checked;
            if (limitScope)
            {
                // Find questions which are in scope:
                questions = new HashSet<question>();

                // Add their questions.  Could do this by finding
                // the repeat answer via XPath, then getting
                // variables in it, but in this case its easier
                // just to string match in the XPaths part.
                xpaths xpaths = xppe.xpaths;

                // Get list of repeat ancestors
                List<Word.ContentControl> relevantRepeats = new List<Word.ContentControl>();
                Word.ContentControl currentCC = cc.ParentContentControl;
                while (currentCC != null)
                {
                    if (currentCC.Tag.Contains("od:repeat"))
                    {
                        relevantRepeats.Add(currentCC);
                    }
                    currentCC = currentCC.ParentContentControl;
                }

                foreach (Word.ContentControl rcc in relevantRepeats)
                {
                    TagData td = new TagData(rcc.Tag);
                    string rXPathID = td.getRepeatID();
                    xpathsXpath xp = xppe.getXPathByID(rXPathID);
                    string rXPath = xp.dataBinding.xpath;
                    int rXPathLength = rXPath.Length;

                    // we want xpaths containing this xpath,
                    // and no extra repeat
                    foreach (xpathsXpath xx in xpaths.xpath)
                    {
                        if (xx.questionID != null) // includes repeats. Shouldn't if can add/remove row on XForm? 
                        {
                            string thisXPath = xx.dataBinding.xpath;
                            if (thisXPath.Contains(rXPath))
                            {
                                if (thisXPath.LastIndexOf("oda:repeat") < rXPathLength)
                                {
                                    questions.Add(
                                        questionnaire.getQuestion(xx.questionID));
                                }
                            }
                        }
                    }
                }

                // Finally, add top level questions
                foreach (xpathsXpath xx in xpaths.xpath)
                {
                    if (xx.questionID != null)
                    {
                        string thisXPath = xx.dataBinding.xpath;
                        if (thisXPath.IndexOf("oda:repeat") < 0)
                        {
                            questions.Add(
                                questionnaire.getQuestion(xx.questionID));
                        }
                    }
                }


            }
            else
            {
                questions = questionnaire.questions;
            }

            foreach (question q in questions)
            {
                if (type == null)
                //   || type.Equals("ALL"))
                {
                    // all questions
                    listBoxQuestions.Items.Add(q);
                }
                else
                {
                    xpathsXpath xpath = xppe.getXPathByQuestionID(q.id);

                    if (xpath.type != null)
                    {
                        if (type.Equals("card number"))
                        {
                            type = "card-number"; // the real date type name
                        }

                        if (type.Equals("number"))
                        {
                            // special case
                            if (xpath.type.Equals("decimal")
                                || xpath.type.Contains("Integer"))
                            {
                                listBoxQuestions.Items.Add(q);
                                log.Debug("Added to listbox " + q.id);
                            }
                        }
                        else
                        {
                            if (xpath.type.Equals(type))
                            {
                                listBoxQuestions.Items.Add(q);
                                log.Debug("Added to listbox " + q.id);

                            }
                        }
                    }
                }
            }

        }


        /// <summary>
        /// Specific to FormCondition, and type REPEAT_COUNT.
        /// 
        /// List:
        /// 1. any sibling repeats
        /// 2. if in repeat, self and ancestor repeats
        /// </summary>
        protected void populateRepeats()
        {
            // There are 2 potential ways to work this out.
            // One is to look at content controls on the
            // document surface.
            // The other is to rely on the AF structure.
            // The AF structure is easier (but of course 
            // makes this code less usable in the generic case).

            // For the AF approach, our starting point
            // is to find out which repeat, if any, we are in.
            Word.ContentControl currentCC = cc.ParentContentControl;
            Word.ContentControl repeatCC = null;
            while (currentCC != null)
            {
                if (currentCC.Tag.Contains("od:repeat"))
                {
                    repeatCC = currentCC;
                    break;
                }
                currentCC = currentCC.ParentContentControl;
            }

            answers answers = new answers();
            answers.Deserialize(model.answersPart.XML, out answers);

            if (repeatCC == null)
            {
                // Then we're interested in top-level repeats in the AF
                log.Debug("No repeat ancestor, so we're interested in top-level repeats in the AF");
                foreach (object o in answers.Items)
                {
                    if (o is repeat)
                    {
                        string qid = ((repeat)o).qref;
                        question q = questionnaire.getQuestion(qid);
                        listBoxQuestions.Items.Add(q);
                        log.Debug("Added to listbox " + q.id);
                    }
                }

            }
            else
            {
                // We're interested in this repeat, and its child repeats
                // Find it.
                log.Debug("In repeat, so we're interested in this repeat, and its child repeats");
                String repeatId = (new TagData(repeatCC.Tag)).getRepeatID();
                // To find it, must go via it XPath
                xpathsXpath xp = xppe.getXPathByID(repeatId);
                repeat r = Helpers.AnswersHelper.findRepeat(answers, xp.questionID);

                // Add it
                question q = questionnaire.getQuestion(xp.questionID);
                listBoxQuestions.Items.Add(q);
                log.Debug("Added to listbox " + q.id);

                // Now add its child repeats (if any)
                foreach (object o in r.row[0].Items)
                {
                    if (o is repeat)
                    {
                        string qid = ((repeat)o).qref;
                        question q2 = questionnaire.getQuestion(qid);
                        listBoxQuestions.Items.Add(q2);
                        log.Debug("Added to listbox " + q2.id);
                    }
                }
            }
        }

        public void filterAction()
        {

            // Get the currently selected item in the ListBox.
            string type = null;
            if (listBoxTypeFilter.SelectedItem != null)
            {
                type = listBoxTypeFilter.SelectedItem.ToString();
                //map
                if (type.Equals("text"))
                {
                    type = "string";
                }
            }

            populateQuestions(type);
            if (this.listBoxQuestions.Items.Count > 0)
            {
                question q = (question)this.listBoxQuestions.Items[0];
                this.listBoxQuestions.SelectedIndexChanged -= new System.EventHandler(this.listBoxQuestions_SelectedIndexChanged);
                this.listBoxQuestions.SelectedItem = q;
                this.listBoxQuestions.SelectedIndexChanged += new System.EventHandler(this.listBoxQuestions_SelectedIndexChanged);
                if (q.response.Item is responseFixed)
                {
                    populateValues((responseFixed)q.response.Item, null);
                }
                else
                {
                    clearComboBoxValues();
                }
                populatePredicates(q);
            }
            else
            {
                populatePredicates(null);
            }
        }

        public void listBoxQuestions_SelectedIndexChanged(object sender, EventArgs e)
        {
            question q = (question)listBoxQuestions.SelectedItem;
            populatePredicates(q);
            if (q.response.Item is responseFixed)
            {
                populateValues((responseFixed)q.response.Item, null);
            }
            else
            {
                clearComboBoxValues();
            }

            xpathsXpath xpath = xppe.getXPathByQuestionID(q.id);
            this.listBoxTypeFilter.SelectedIndexChanged -= new System.EventHandler(this.listBoxTypeFilter_SelectedIndexChanged);
            if (xpath.dataBinding.xpath.EndsWith("oda:row"))
            {
                if (listBoxTypeFilter.SelectedItem.Equals(REPEAT_COUNT)
                    || listBoxTypeFilter.SelectedItem.Equals(REPEAT_POS))
                {
                    // Do nothing
                }
                else
                {
                    listBoxTypeFilter.SelectedItem = REPEAT_POS;
                }
            }
            else
            {
                listBoxTypeFilter.SelectedItem = xpath.type;
            }
            this.listBoxTypeFilter.SelectedIndexChanged += new System.EventHandler(this.listBoxTypeFilter_SelectedIndexChanged);


        }

        public virtual void populateValues(responseFixed responses, string matchResponse) { }
        public virtual void clearComboBoxValues() { }
        public virtual void populatePredicates(question q) { }


    }
}
