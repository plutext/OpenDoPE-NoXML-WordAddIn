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
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using NLog;

using Office = Microsoft.Office.Core;
using OpenDoPEModel;
using Word = Microsoft.Office.Interop.Word;
using Microsoft.Office.Tools.Word.Extensions; // for VSTOObject

namespace OpenDope_AnswerFormat
{
    /// <summary>
    /// Used for adding a condition, or editing an existing one.
    /// </summary>
    public partial class FormCondition : Form
    {
        static Logger log = LogManager.GetLogger("FormCondition");

        ConditionsPartEntry cpe; 
        condition existingCondition;

        protected Word.ContentControl cc;
        protected Model model;

        protected Office.CustomXMLPart questionsPart;
        //private question q;
        protected questionnaire questionnaire;

        protected XPathsPartEntry xppe;

        Helpers.QuestionListHelperForConditionsForm questionListHelper;

        public FormCondition(Word.ContentControl cc, ConditionsPartEntry cpe, condition existingCondition)
        {
            InitializeComponent();

            // NET 4 way; see http://msdn.microsoft.com/en-us/library/microsoft.office.tools.word.extensions.aspx
            FabDocxState fabDocxState = (FabDocxState)Globals.Factory.GetVstoObject(Globals.ThisAddIn.Application.ActiveDocument).Tag;

            // NET 3.5 way, which requires using Microsoft.Office.Tools.Word.Extensions
            //FabDocxState fabDocxState = (FabDocxState)Globals.ThisAddIn.Application.ActiveDocument.GetVstoObject(Globals.Factory).Tag;
            this.model = fabDocxState.model;
            xppe = new XPathsPartEntry(model);

            this.cc = cc;

            this.cpe = cpe;
            this.existingCondition = existingCondition;

            this.questionsPart = model.questionsPart;
            questionnaire = new questionnaire();
            questionnaire.Deserialize(questionsPart.XML, out questionnaire);

            questionListHelper = new Helpers.QuestionListHelperForConditionsForm(model, xppe, questionnaire, cc);
            questionListHelper.listBoxTypeFilter = listBoxTypeFilter;
            questionListHelper.listBoxQuestions = listBoxQuestions;
            questionListHelper.checkBoxScope = checkBoxScope;

            questionListHelper.comboBoxValues = comboBoxValues;
            questionListHelper.listBoxPredicate = listBoxPredicate;

            this.listBoxQuestions.SelectedIndexChanged += new System.EventHandler(questionListHelper.listBoxQuestions_SelectedIndexChanged);
            this.listBoxTypeFilter.SelectedIndexChanged += new System.EventHandler(questionListHelper.listBoxTypeFilter_SelectedIndexChanged);

            question existingQuestion = null;
            string matchResponse = null;
            if (existingCondition != null)
            {
                // Use the question associated with it, to pre-select
                // the correct entries in the dialog.

                // Re-label the window, so user can see what the condition was about
                this.Text = "Editing Condition:   " + cc.Title;

                //List<xpathsXpath> xpaths = ConditionsPartEntry.getXPathsUsedInCondition(existingCondition, xppe);
                List<xpathsXpath> xpaths = new List<xpathsXpath>();
                existingCondition.listXPaths(xpaths, cpe.conditions, xppe.getXPaths());

                if (xpaths.Count > 1)
                {
                    // TODO: use complex conditions editor
                }
                xpathsXpath xpathObj = xpaths[0];

                String xpathVal = xpathObj.dataBinding.xpath;

                if (xpathVal.StartsWith("/"))
                {
                    // simple
                    //System.out.println("question " + xpathObj.getQuestionID() 
                    //        + " is in use via boolean condition " + conditionId);

                    existingQuestion = this.questionnaire.getQuestion(xpathObj.questionID);
                    matchResponse = xpathVal;
                }
                else if (xpathVal.Contains("position"))
                {
                    // TODO
                }
                else
                {
                    //System.out.println(xpathVal);

                    String qid = xpathVal.Substring(
                        xpathVal.LastIndexOf("@id") + 5);
                    //						System.out.println("Got qid: " + qid);
                    qid = qid.Substring(0, qid.IndexOf("'"));
                    //						System.out.println("Got qid: " + qid);

                    //System.out.println("question " + qid 
                    //        + " is in use via condition " + conditionId);

                    existingQuestion = this.questionnaire.getQuestion(qid);
                    matchResponse = xpathVal;

                }

            }

            questionListHelper.populateTypeFilter(true);

            if (existingQuestion == null)
            {
                // for init, populate with all questions
                questionListHelper.populateQuestions(null);
            }
            else
            {
                // Just show the existing question
                listBoxQuestions.Items.Add(existingQuestion);
            }

            if (this.listBoxQuestions.Items.Count == 0) // Never happens if in a repeat, and nor do we want it to, since user might just want to use "repeat pos" stuff 
            {
                // Try including out of scope
                this.checkBoxScope.Checked = true;
                questionListHelper.populateQuestions(null);
                if (this.listBoxQuestions.Items.Count == 0)
                {
                    MessageBox.Show("You can't define a condition until you have set up at least one question. Let's do that now. ");

                    FormQA formQA = new FormQA(cc, false);
                    formQA.ShowDialog();
                    formQA.Dispose();

                    // Refresh these
                    xppe = new XPathsPartEntry(model);
                    questionnaire.Deserialize(questionsPart.XML, out questionnaire);

                    questionListHelper.filterAction();

                    return;
                }
            }
            // value 
            question q;
            if (existingQuestion == null)
            {
                // for init, populate with all questions
                q = (question)this.listBoxQuestions.Items[0];
            }
            else
            {
                q = existingQuestion;
            }
            
            this.listBoxQuestions.SelectedItem = q;
            if (q.response.Item is responseFixed)
            {
                questionListHelper.populateValues((responseFixed)q.response.Item, matchResponse);
            }

            // predicate =
            questionListHelper.populatePredicates(q);  // TODO: set this correctly in editing mode
        }

        void buttonAdvanced_Click(object sender, System.EventArgs e)
        {
        }


        public bool preconditionsMet()
        {
            //return (this.listBoxQuestions.Items.Count > 0);
            return true;
        }

        public bool postconditionsMet { get; set; }






        private void buttonOK_Click(object sender, EventArgs e)
        {

            string titleText = "";
            String newXPath = null;
            string pred;
            TagData td;

            if (listBoxTypeFilter.SelectedItem != null
                && listBoxTypeFilter.SelectedItem.ToString().Equals(Helpers.QuestionListHelper.REPEAT_POS))
            {
                // Special case
                newXPath = null;
                pred = (string)this.listBoxPredicate.SelectedItem;

                if (pred.Equals("first"))
                {
                    newXPath = "position()=1";
                    titleText = "If first entry in repeat";
                }
                else if (pred.Equals("not first"))
                {
                    newXPath = "position()&gt;1";
                    titleText = "If not the first entry in repeat";
                }
                else if (pred.Equals("second"))
                {
                    newXPath = "position()=2";
                    titleText = "If second entry in repeat";
                }
                else if (pred.Equals("second last"))
                {
                    newXPath = "position()=last()-1";
                    titleText = "If second last entry in repeat";

                }
                else if (pred.Equals("last"))
                {
                    newXPath = "position()=last()";
                    titleText = "If last entry in repeat";
                }
                else if (pred.Equals("not last"))
                {
                    newXPath = "position()!=last()";
                    titleText = "If not the last entry in repeat";
                }
                else
                {
                    log.Error("unexpected predicate " + pred);
                }
                // No point making this a condition

                //condition result = conditionsHelper.setup(xpathExisting.dataBinding.storeItemID,
                //    newXPath, xpathExisting.dataBinding.prefixMappings, false);

                xpathsXpath xpathEntry = xppe.setup("", model.answersPart.Id, newXPath, null, false);
                //xpathEntry.questionID = q.id;
                xpathEntry.dataBinding.prefixMappings = "xmlns:oda='http://opendope.org/answers'";
                //xpathEntry.type = dataType;
                xppe.save();

                td = new TagData("");
                td.set("od:RptPosCon", xpathEntry.id);
                cc.Tag = td.asQueryString();

                cc.Title = titleText;
                cc.SetPlaceholderText(null, null, "Type the text that'll appear between repeated items.");
                // that'll only be displayed if the cc is not being wrapped around existing content :-)

                // Don't :
                // ContentControlNewConditionCheck variableRelocator = new ContentControlNewConditionCheck();
                // variableRelocator.checkAnswerAncestry(xpathExisting.id);

                postconditionsMet = true;
                return;

            }


            xpathsXpath xpathExisting = null;
            string val = null;


            // Validation
            question q = (question)listBoxQuestions.SelectedItem;
            if (q == null)
            {
                MessageBox.Show("You must select a question!");
                DialogResult = DialogResult.None; // or use on closing event; see http://stackoverflow.com/questions/2499644/preventing-a-dialog-from-closing-in-the-buttons-click-event-handler
                return;
            }
            else
            {
                // Get the XPath for the selected question
                xpathExisting = xppe.getXPathByQuestionID(q.id);
            }

            //if (listBoxTypeFilter.SelectedItem != null 
            //    && listBoxTypeFilter.SelectedItem.ToString().Equals("repeat")) {
            //        // Special case
            //    }
            //else
            //{
                //More validation

                object o = this.comboBoxValues.SelectedItem;
                if (o==null)
                {
                    if (comboBoxValues.Text == null)
                    {
                        MessageBox.Show("You must specify a value!");
                        DialogResult = DialogResult.None;
                        return;
                    }
                    else
                    {
                        o = comboBoxValues.Text;
                    }
                } 
                if (o is string)
                {
                    val = (string)o;
                }
                else
                {
                    //responseFixed
                    val = ((responseFixedItem)o).value;
                }
            //}
            ConditionsPartEntry conditionsHelper = new ConditionsPartEntry(model);


            //if (xpathExisting!=null && xpathExisting.type.Equals("boolean")
            //    && (val.ToLower().Equals("true")
            //    || val.ToLower().Equals("false"))) {
            //    // if its boolean true, all we need to do is create a condition pointing to that
            //    // if its boolean false, all we need to do is create a condition not pointing to that

            //    TagData td = new TagData("");

            //    if (val.ToLower().Equals("true") )
            //    {
            //        // if its boolean true, all we need to do is create a condition pointing to that
            //        log.Info("boolean true - just need a condition");

            //        condition c = conditionsHelper.setup(xpathExisting);
            //        td.set("od:condition", c.id);
            //        cc.Tag = td.asQueryString();

            //        titleText = "If '" + val + "' for Q: " + q.text;
            //    }
            //    else if (val.ToLower().Equals("false") )
            //    {
            //        // if its boolean false, all we need to do is create a condition not pointing to that
            //        log.Info("boolean true - just need a NOT condition");
            //        condition c = new condition();

            //        xpathref xpathref = new xpathref();
            //        xpathref.id = xpathExisting.id;

            //        not n = new not();
            //        n.Item = xpathref;

            //        c.Item = n;

            //        conditionsHelper.add(c, "not" + xpathref.id);
            //        td.set("od:condition", c.id);
            //        cc.Tag = td.asQueryString();

            //        titleText = "If '" + val + "' for Q: " + q.text;
            //    }
            //    else
            //    {
            //        MessageBox.Show("Only true/yes or false/no are allowed for this question");
            //        return;
            //    }

            //} else {


                // otherwise, create a new xpath object, and a condition pointing to it
                pred = (string)this.listBoxPredicate.SelectedItem;

                if (pred == null)
                {
                    MessageBox.Show("For " + xpathExisting.type + ", you must select a relation!");
                    DialogResult = DialogResult.None;
                    return;
                }

                log.Info("create a new xpath object, and a condition pointing to it.  Predicate is " + pred);

                newXPath = null;

                if (listBoxTypeFilter.SelectedItem != null
                    && listBoxTypeFilter.SelectedItem.ToString().Equals(Helpers.QuestionListHelper.REPEAT_COUNT))
                {
                    if (pred.Equals("="))
                    {
                        newXPath = "count(" + xpathExisting.dataBinding.xpath + ")=" + val;
                        titleText = "If Repeat " + q.text + " has " + val;
                    }
                    else if (pred.Equals(">"))
                    {
                        newXPath = "count(" + xpathExisting.dataBinding.xpath + ")>" + val;
                        titleText = "If Repeat " + q.text + " > " + val;
                    }
                    else if (pred.Equals(">="))
                    {
                        newXPath = "count(" + xpathExisting.dataBinding.xpath + ")>=" + val;
                        titleText = "If Repeat " + q.text + " >= " + val;
                    }
                    else if (pred.Equals("<"))
                    {
                        newXPath = "count(" + xpathExisting.dataBinding.xpath + ")<" + val;
                        titleText = "If Repeat " + q.text + " < " + val;

                    }
                    else if (pred.Equals("<="))
                    {
                        newXPath = "count(" + xpathExisting.dataBinding.xpath + ")<=" + val;
                        titleText = "If Repeat " + q.text + " <= " + val;
                    }
                    else
                    {
                        log.Error("unexpected predicate " + pred);
                    }

                } else if (xpathExisting.type.Equals("boolean")) {

                    // done this way, since XPath spec says the boolean value of a string is true,
                    // if it is not empty!

                    newXPath = "string(" + xpathExisting.dataBinding.xpath + ")='" + val + "'";
                    titleText = "If '" + val + "' for Q: " + q.text;

                } else if (xpathExisting.type.Equals("string"))
                {
                    if (pred.Equals("equals"))
                    {
                        newXPath = "string(" + xpathExisting.dataBinding.xpath + ")='" + val + "'";
                        titleText = "If '" + val + "' for Q: " + q.text;

                    }
                    else if (pred.Equals("is not"))
                    {
                        newXPath = "string(" + xpathExisting.dataBinding.xpath + ")!='" + val + "'";
                        titleText = "If NOT '" + val + "' for Q: " + q.text;
                    }
                    else if (pred.Equals("starts-with"))
                    {
                        newXPath = "starts-with(string(" + xpathExisting.dataBinding.xpath + "), '" + val + "')";
                        titleText = "If starts-with '" + val + "' for Q: " + q.text;

                    }
                    else if (pred.Equals("contains"))
                    {
                        newXPath = "contains(string(" + xpathExisting.dataBinding.xpath + "), '" + val + "')";
                        titleText = "If contains '" + val + "' for Q: " + q.text;
                    }
                    else
                    {
                        log.Error("unexpected predicate " + pred);
                    }
                } else if (xpathExisting.type.Equals("decimal")
                    || xpathExisting.type.Equals("integer") 
                    || xpathExisting.type.Equals("positiveInteger")
                    || xpathExisting.type.Equals("nonPositiveInteger")
                    || xpathExisting.type.Equals("negativeInteger")
                    || xpathExisting.type.Equals("nonNegativeInteger")
                    )
                {
                    if (pred.Equals("="))
                    {
                        newXPath = "number(" + xpathExisting.dataBinding.xpath + ")=" + val;
                        titleText = "If '" + val + "' for Q: " + q.text;
                    }
                    else if (pred.Equals(">"))
                    {
                        newXPath = "number(" + xpathExisting.dataBinding.xpath + ")>" + val;
                        titleText = "If >" + val + " for Q: " + q.text;
                    }
                    else if (pred.Equals(">="))
                    {
                        newXPath = "number(" + xpathExisting.dataBinding.xpath + ")>=" + val;
                        titleText = "If >=" + val + " for Q: " + q.text;
                    }
                    else if (pred.Equals("<"))
                    {
                        newXPath = "number(" + xpathExisting.dataBinding.xpath + ")<" + val;
                        titleText = "If <" + val + " for Q: " + q.text;

                    }
                    else if (pred.Equals("<="))
                    {
                        newXPath = "number(" + xpathExisting.dataBinding.xpath + ")<=" + val;
                        titleText = "If <=" + val + " for Q: " + q.text;
                    }
                    else
                    {
                        log.Error("unexpected predicate " + pred);
                    }

                }
                else if (xpathExisting.type.Equals("date"))
                {
                    // Requires XPath 2.0

                    if (pred.Equals("equals"))
                    {
                        newXPath = "xs:date(" + xpathExisting.dataBinding.xpath + ") = xs:date('" + val + "')";
                        titleText = "If '" + val + "' for Q: " + q.text;
                    }
                    else if (pred.Equals("is before"))
                    {
                        newXPath = "xs:date(" + xpathExisting.dataBinding.xpath + ") < xs:date('" + val + "')";
                        titleText = "If before '" + val + "' for Q: " + q.text;
                    }
                    else if (pred.Equals("is after"))
                    {
                        newXPath = "xs:date(" + xpathExisting.dataBinding.xpath + ") > xs:date('" + val + "')";
                        titleText = "If after '" + val + "' for Q: " + q.text;
                    }
                    else
                    {
                        log.Error("unexpected predicate " + pred);
                    }

                } else
                {
                    log.Error("Unexpected data type " + xpathExisting.type);
                }

                if (existingCondition == null)
                {
                    // Create new condition

                    condition result = conditionsHelper.setup(xpathExisting.dataBinding.storeItemID,
                        newXPath, xpathExisting.dataBinding.prefixMappings, false);
                    td = new TagData("");
                    td.set("od:condition", result.id);
                    cc.Tag = td.asQueryString();

                    //}

                    cc.SetPlaceholderText(null, null, "Type the text for when this condition is satisfied.");
                    // that'll only be displayed if the cc is not being wrapped around existing content :-)
                }
                else
                {
                    // Update existing condition

                    // Drop any trailing "/" from a Condition XPath
                    if (newXPath.EndsWith("/"))
                    {
                        newXPath = newXPath.Substring(0, newXPath.Length - 1);
                    }
                    log.Debug("Creating condition using XPath:" + newXPath);

                    XPathsPartEntry xppe = new XPathsPartEntry(model);
                    xpathsXpath xpath = xppe.setup("cond", xpathExisting.dataBinding.storeItemID, 
                        newXPath, xpathExisting.dataBinding.prefixMappings, false);
                    xppe.save();

                    xpathref xpathref = new xpathref();
                    xpathref.id = xpath.id;

                    // NB no attempt is made here to delete the old xpathref
                    // TODO

                    existingCondition.Item = xpathref;

                    // Save the conditions in docx
                    cpe.save();

                }
                cc.Title = titleText;

                if (listBoxTypeFilter.SelectedItem != null
                    && listBoxTypeFilter.SelectedItem.ToString().Equals(Helpers.QuestionListHelper.REPEAT_COUNT))
                {
                    // Skip ContentControlNewConditionCheck
                }
                else
                {
                    // Make sure this question is allowed here
                    // ie the it is top level or in a common repeat ancestor.
                    // We do this last, so this cc has od:condition on it,
                    // in which case we can re-use existing code to do the check
                    // TODO: when we support and/or, will need to do this
                    // for each variable.
                    ContentControlNewConditionCheck variableRelocator = new ContentControlNewConditionCheck();
                    variableRelocator.checkAnswerAncestry(xpathExisting.id);
                }
            postconditionsMet = true;

        }

        private void buttonQuestionAdd_Click(object sender, EventArgs e)
        {
            FormQA formQA = new FormQA(cc, false);
            formQA.ShowDialog();
            formQA.Dispose();

            // Refresh these
            xppe = new XPathsPartEntry(model);
            questionnaire.Deserialize(questionsPart.XML, out questionnaire);

            questionListHelper.filterAction();
        }

        
    }
}
