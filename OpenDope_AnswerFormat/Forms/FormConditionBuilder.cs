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

namespace OpenDope_AnswerFormat.Forms
{
    /// <summary>
    /// Used for adding a condition, or editing an existing one.
    /// </summary>
    public partial class FormConditionBuilder : Form
    {
        static Logger log = LogManager.GetLogger("FormCondition");

        ConditionsPartEntry cpe; 
        condition existingCondition;

        protected Word.ContentControl cc;
        protected Model model;

        protected Office.CustomXMLPart questionsPart;
        //private question q;

        // need to be able to update this from rowHelper if user adds a question
        public XPathsPartEntry xppe { get; set; }
        public questionnaire questionnaire { get; set; }

        public conditions conditions { get; set; }

        Helpers.ConditionsFormRowHelper rowHelper;

        public FormConditionBuilder(Word.ContentControl cc, ConditionsPartEntry cpe, condition existingCondition)
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
            questionnaire qtmp = new questionnaire();
            questionnaire.Deserialize(questionsPart.XML, out qtmp);
            questionnaire = qtmp;

            conditions ctmp = new conditions();
            conditions.Deserialize(model.conditionsPart.XML, out ctmp);
            conditions = ctmp;

            log.Debug("conditions: " + conditions.Serialize());


            this.listBoxGovernor.Items.Add("all");
            this.listBoxGovernor.Items.Add("any");
            this.listBoxGovernor.Items.Add("none");
            this.listBoxGovernor.SelectedItem = "all";

            rowHelper = new Helpers.ConditionsFormRowHelper(model, xppe, questionnaire, cc, this);

            rowHelper.init(this.dataGridView);

            DataGridViewRow row = this.dataGridView.Rows[0];
            rowHelper.populateRow(row, null, null);
           
        }

        /// <summary>
        /// returns null if the row is valid; otherwise, a message
        /// </summary>
        /// <param name="row"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        private string isRowValid(DataGridViewRow row, int i)
        {

            DataGridViewCell c = row.Cells["Questions"];
            if (c.Value==null
                || c.Value is string)
            {
                return "Question missing (row " + i + ")";
            }

            // that's all that's required for condition re-use
            if (c.Value is condition)
            {
                return null;
            }

             c = row.Cells["Predicate"];
            if (string.IsNullOrWhiteSpace((string)c.Value))
            {
                return "Predicate missing (row " + i + ")";
            }

            // unless it is a repeat position,
            // there must be a value as well
            if (c.Value is RepeatPosition)
            {
                return null;
            }

            c = row.Cells["Value"];
            if (c.Value==null)
            {
                return "Value missing (row " + i + ")";
            }


            return null;
        }

        public string isValid()
        {
            if (this.dataGridView.Rows.Count < 2) // auto last row
            {
                return "You must provide at least 1 row";
            }

            int last = this.dataGridView.Rows.Count - 1;
            int i = 1;
            foreach (DataGridViewRow row in this.dataGridView.Rows)
            {
                // Last row is added automatically
                if (row == this.dataGridView.Rows[last]) continue;

                string result = isRowValid(row, i);
                if(result!=null)
                {
                    return result;
                }
                i++;

            }
            return null;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            string msg = isValid();
            if (msg!=null)
            {
                MessageBox.Show(msg);
                DialogResult = DialogResult.None; // or use on closing event; see http://stackoverflow.com/questions/2499644/preventing-a-dialog-from-closing-in-the-buttons-click-event-handler
                return;
            }

            buildCondition();

        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {

        }

        void textBoxDescription_GotFocus(object sender, System.EventArgs e)
        {
            textBoxEnglish.Text = "";
            string msg = isValid();
            if (msg != null)
            {
                textBoxEnglish.Text = msg;
            }
        }

        void textBoxName_GotFocus(object sender, System.EventArgs e)
        {
            textBoxEnglish.Text = "";
            string msg = isValid();
            if (msg != null)
            {
                textBoxEnglish.Text = msg;
            }
        }

        private void setTag(Word.ContentControl cc, condition result)
        {
            TagData td = new TagData("");
            td.set("od:condition", result.id);
            cc.Tag = td.asQueryString();

            cc.SetPlaceholderText(null, null, "Type the text for when this condition is satisfied.");
            // that'll only be displayed if the cc is not being wrapped around existing content :-)


            log.Info("Created condition " + result.Serialize());
        }

        private string restrict64chars(string input)
        {
            if (input.Length > 64)
            {
                return input.Substring(0, 62) + "..";
            }
            else
            {
                return input;
            }

        }

        private void buildCondition()
        {
            ConditionsPartEntry cpe = new ConditionsPartEntry(model);
            TagData td;

            if (this.dataGridView.Rows.Count == 2 // auto last row
                &&  !this.listBoxGovernor.SelectedItem.ToString().Equals("none") ) // none handled separately
            {
                // a simple condition

                if (this.dataGridView.Rows[0].Cells["Questions"].Value is condition)
                {
                    // this is just condition re-use!
                    condition cReused = (condition)this.dataGridView.Rows[0].Cells["Questions"].Value;
                    setTag(cc, cReused);
                    cc.Title = cReused.description; // that'll do for now
                    return;
                }

                // Usual case
                Pairing pair = buildXPathRef(this.dataGridView.Rows[0]);
                cc.Title = restrict64chars(pair.titleText);

                if (pair.xpathEntry.dataBinding.xpath.Contains("position()"))
                {
                    // special case.  TODO: make this a normal condition!
                    // since this approach won't work if it is in complex condition
                    td = new TagData("");
                    td.set("od:RptPosCon", pair.xpathEntry.id);
                    cc.Tag = td.asQueryString();

                    cc.SetPlaceholderText(null, null, "Type the text that'll appear between repeated items.");
                    // that'll only be displayed if the cc is not being wrapped around existing content :-)
                    return;
                } 

                condition result = cpe.setup(pair.xpathEntry);
                result.name = this.textBoxName.Text;
                if (string.IsNullOrWhiteSpace(this.textBoxDescription.Text))
                {
                    result.description = pair.titleText;
                }
                else
                {
                    result.description = this.textBoxDescription.Text;
                }
                cpe.save();

                setTag(cc, result);

                return;
            }

            // multi-row
            int last = this.dataGridView.Rows.Count - 1;
            condition outer = new condition();
            cc.Title = null;
            if (this.listBoxGovernor.SelectedItem.ToString().Equals("all"))
            {
                // = and
                and and = new and();
                outer.Item = and;

                foreach (DataGridViewRow row in this.dataGridView.Rows)
                {
                    // Last row is added automatically
                    if (row == this.dataGridView.Rows[last]) continue;

                    if (row.Cells["Questions"].Value is condition)
                    {
                        // this is just condition re-use!
                        condition cReused = (condition)row.Cells["Questions"].Value;

                        if (cc.Title == null)
                        {
                            cc.Title = this.restrict64chars(cReused.description); // that'll do for now
                        }
                        else
                        {
                            cc.Title = this.restrict64chars(cc.Title + " and " + cReused.description); // that'll do for now
                        }


                        conditionref conditionref = new conditionref();
                        conditionref.id = cReused.id;
                        and.Items.Add(conditionref);

                    }
                    else
                    {
                        // xpathref
                        Pairing pair = buildXPathRef(row);

                        if (cc.Title == null)
                        {
                            cc.Title = this.restrict64chars(pair.titleText);
                        }
                        else
                        {
                            cc.Title = this.restrict64chars(cc.Title + " and " + pair.titleText);
                        }

                        xpathref xpathref = new xpathref();
                        xpathref.id = pair.xpathEntry.id;

                        and.Items.Add(xpathref);
                    }
                }

                outer.name = this.textBoxName.Text;
                if (string.IsNullOrWhiteSpace(this.textBoxDescription.Text))
                {
                    outer.description = cc.Title;
                }
                else
                {
                    outer.description = this.textBoxDescription.Text;
                }

                cpe.add(outer, null);
                cpe.save();

                setTag(cc, outer);

                return;
            } 
            
            if (this.listBoxGovernor.SelectedItem.ToString().Equals("any") ) {
                // = or
                or or = new or();
                outer.Item = or;

                foreach (DataGridViewRow row in this.dataGridView.Rows)
                {
                    // Last row is added automatically
                    if (row == this.dataGridView.Rows[last]) continue;

                    if (row.Cells["Questions"].Value is condition)
                    {
                        // this is just condition re-use!
                        condition cReused = (condition)row.Cells["Questions"].Value;

                        if (cc.Title == null)
                        {
                            cc.Title = this.restrict64chars(cReused.description); // that'll do for now
                        }
                        else
                        {
                            cc.Title = this.restrict64chars(cc.Title + " and " + cReused.description); // that'll do for now
                        }


                        conditionref conditionref = new conditionref();
                        conditionref.id = cReused.id;
                        or.Items.Add(conditionref);

                    }
                    else
                    {

                        Pairing pair = buildXPathRef(row);

                        if (cc.Title == null)
                        {
                            cc.Title = this.restrict64chars(pair.titleText);
                        }
                        else
                        {
                            cc.Title = this.restrict64chars(cc.Title + " or " + pair.titleText);
                        }

                        xpathref xpathref = new xpathref();
                        xpathref.id = pair.xpathEntry.id;

                        or.Items.Add(xpathref);
                    }
                }

                outer.name = this.textBoxName.Text;
                if (string.IsNullOrWhiteSpace(this.textBoxDescription.Text))
                {
                    outer.description = cc.Title;
                }
                else
                {
                    outer.description = this.textBoxDescription.Text;
                }

                cpe.add(outer, null);
                cpe.save();

                setTag(cc, outer);

                return;
            }
            
            if (this.listBoxGovernor.SelectedItem.ToString().Equals("none"))
            {
                // none:  not(A || B) = !A && !B
                not not = new not();
                outer.Item = not;

                or or = new or();
                not.Item = or;

                cc.Title = "NONE OF ";
                foreach (DataGridViewRow row in this.dataGridView.Rows)
                {
                    // Last row is added automatically
                    if (row == this.dataGridView.Rows[last]) continue;
                    if (row.Cells["Questions"].Value is condition)
                    {
                        // this is just condition re-use!
                        condition cReused = (condition)row.Cells["Questions"].Value;

                        if (cc.Title == null)
                        {
                            cc.Title = this.restrict64chars(cReused.description); // that'll do for now
                        }
                        else
                        {
                            cc.Title = this.restrict64chars(cc.Title + " and " + cReused.description); // that'll do for now
                        }


                        conditionref conditionref = new conditionref();
                        conditionref.id = cReused.id;
                        or.Items.Add(conditionref);

                    }
                    else
                    {

                        Pairing pair = buildXPathRef(row);

                        if (cc.Title.Equals("NONE OF "))
                        {
                            cc.Title = this.restrict64chars(cc.Title + pair.titleText);
                        }
                        else
                        {
                            cc.Title = this.restrict64chars(cc.Title + " or " + pair.titleText);
                        }

                        xpathref xpathref = new xpathref();
                        xpathref.id = pair.xpathEntry.id;

                        or.Items.Add(xpathref);
                    }
                }

                outer.name = this.textBoxName.Text;
                if (string.IsNullOrWhiteSpace(this.textBoxDescription.Text))
                {
                    outer.description = cc.Title;
                }
                else
                {
                    outer.description = this.textBoxDescription.Text;
                }

                cpe.add(outer, null);
                cpe.save();

                setTag(cc, outer);

                return;
            }





            //// Make sure this question is allowed here
            //// ie the it is top level or in a common repeat ancestor.
            //// We do this last, so this cc has od:condition on it,
            //// in which case we can re-use existing code to do the check
            //// TODO: when we support and/or, will need to do this
            //// for each variable.
            //ContentControlNewConditionCheck variableRelocator = new ContentControlNewConditionCheck();
            //variableRelocator.checkAnswerAncestry(xpathExisting.id);

        }

        private Pairing buildXPathRef(DataGridViewRow row)
        {
            Pairing pairing = new Pairing(); 
            string pred = (string)row.Cells["Predicate"].Value;
            TagData td;
            string newXPath;

            if (row.Cells["Questions"].Value is RepeatPosition)
            {
                if (pred.Equals("first"))
                {
                    newXPath = "position()=1";
                    pairing.titleText = "If first entry in repeat";
                }
                else if (pred.Equals("not first"))
                {
                    newXPath = "position()&gt;1";
                    pairing.titleText = "If not the first entry in repeat";
                }
                else if (pred.Equals("second"))
                {
                    newXPath = "position()=2";
                    pairing.titleText = "If second entry in repeat";
                }
                else if (pred.Equals("second last"))
                {
                    newXPath = "position()=last()-1";
                    pairing.titleText = "If second last entry in repeat";

                }
                else if (pred.Equals("last"))
                {
                    newXPath = "position()=last()";
                    pairing.titleText = "If last entry in repeat";
                }
                else if (pred.Equals("not last"))
                {
                    newXPath = "position()!=last()";
                    pairing.titleText = "If not the last entry in repeat";
                }
                else
                {
                    log.Error("unexpected predicate " + pred);
                    return null;
                }
                // No point making this a condition

                //condition result = conditionsHelper.setup(xpathExisting.dataBinding.storeItemID,
                //    newXPath, xpathExisting.dataBinding.prefixMappings, false);

                xpathsXpath xpathEntry = xppe.setup("", model.answersPart.Id, newXPath, null, false);
                //xpathEntry.questionID = q.id;
                xpathEntry.dataBinding.prefixMappings = "xmlns:oda='http://opendope.org/answers'";
                //xpathEntry.type = dataType;
                xppe.save();

                //td = new TagData("");
                //td.set("od:RptPosCon", xpathEntry.id);
                //cc.Tag = td.asQueryString();

                //cc.Title = titleText;
                //cc.SetPlaceholderText(null, null, "Type the text that'll appear between repeated items.");
                //// that'll only be displayed if the cc is not being wrapped around existing content :-)

                //// Don't :
                //// ContentControlNewConditionCheck variableRelocator = new ContentControlNewConditionCheck();
                //// variableRelocator.checkAnswerAncestry(xpathExisting.id);

                //postconditionsMet = true;

                pairing.xpathEntry = xpathEntry;

                return pairing;

            }

            string val = null;
            object o = row.Cells["Value"].Value;
            if (o is string)
            {
                val = (string)o;
            }
            else
            {
                //responseFixed
                val = ((responseFixedItem)o).value;
            }

            xpathsXpath xpathExisting;

            question q;
            if (row.Cells["Questions"].Value is RepeatCount)
            {

                q = ((RepeatCount)row.Cells["Questions"].Value).Repeat;

                xpathExisting = xppe.getXPathByQuestionID(q.id);

                if (pred.Equals("="))
                {
                    newXPath = "count(" + xpathExisting.dataBinding.xpath + ")=" + val;
                    pairing.titleText = "If Repeat " + q.text + " has " + val;
                }
                else if (pred.Equals(">"))
                {
                    newXPath = "count(" + xpathExisting.dataBinding.xpath + ")>" + val;
                    pairing.titleText = "If Repeat " + q.text + " > " + val;
                }
                else if (pred.Equals(">="))
                {
                    newXPath = "count(" + xpathExisting.dataBinding.xpath + ")>=" + val;
                    pairing.titleText = "If Repeat " + q.text + " >= " + val;
                }
                else if (pred.Equals("<"))
                {
                    newXPath = "count(" + xpathExisting.dataBinding.xpath + ")<" + val;
                    pairing.titleText = "If Repeat " + q.text + " < " + val;

                }
                else if (pred.Equals("<="))
                {
                    newXPath = "count(" + xpathExisting.dataBinding.xpath + ")<=" + val;
                    pairing.titleText = "If Repeat " + q.text + " <= " + val;
                }
                else
                {
                    log.Error("unexpected predicate " + pred);
                    return null;
                }

            }
            else
            {

                q = ((question)row.Cells["Questions"].Value);
                xpathExisting = xppe.getXPathByQuestionID(q.id);

                if (xpathExisting.type.Equals("boolean"))
                {

                    // done this way, since XPath spec says the boolean value of a string is true,
                    // if it is not empty!

                    newXPath = "string(" + xpathExisting.dataBinding.xpath + ")='" + val + "'";
                    pairing.titleText = "If '" + val + "' for Q: " + q.text;

                }
                else if (xpathExisting.type.Equals("string"))
                {
                    if (pred.Equals("equals"))
                    {
                        newXPath = "string(" + xpathExisting.dataBinding.xpath + ")='" + val + "'";
                        pairing.titleText = "If '" + val + "' for Q: " + q.text;

                    }
                    else if (pred.Equals("is not"))
                    {
                        newXPath = "string(" + xpathExisting.dataBinding.xpath + ")!='" + val + "'";
                        pairing.titleText = "If NOT '" + val + "' for Q: " + q.text;
                    }
                    else if (pred.Equals("starts-with"))
                    {
                        newXPath = "starts-with(string(" + xpathExisting.dataBinding.xpath + "), '" + val + "')";
                        pairing.titleText = "If starts-with '" + val + "' for Q: " + q.text;

                    }
                    else if (pred.Equals("contains"))
                    {
                        newXPath = "contains(string(" + xpathExisting.dataBinding.xpath + "), '" + val + "')";
                        pairing.titleText = "If contains '" + val + "' for Q: " + q.text;
                    }
                    else
                    {
                        log.Error("unexpected predicate " + pred);
                        return null;
                    }
                }
                else if (xpathExisting.type.Equals("decimal")
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
                        pairing.titleText = "If '" + val + "' for Q: " + q.text;
                    }
                    else if (pred.Equals(">"))
                    {
                        newXPath = "number(" + xpathExisting.dataBinding.xpath + ")>" + val;
                        pairing.titleText = "If >" + val + " for Q: " + q.text;
                    }
                    else if (pred.Equals(">="))
                    {
                        newXPath = "number(" + xpathExisting.dataBinding.xpath + ")>=" + val;
                        pairing.titleText = "If >=" + val + " for Q: " + q.text;
                    }
                    else if (pred.Equals("<"))
                    {
                        newXPath = "number(" + xpathExisting.dataBinding.xpath + ")<" + val;
                        pairing.titleText = "If <" + val + " for Q: " + q.text;

                    }
                    else if (pred.Equals("<="))
                    {
                        newXPath = "number(" + xpathExisting.dataBinding.xpath + ")<=" + val;
                        pairing.titleText = "If <=" + val + " for Q: " + q.text;
                    }
                    else
                    {
                        log.Error("unexpected predicate " + pred);
                        return null;
                    }

                }
                else if (xpathExisting.type.Equals("date"))
                {
                    // Requires XPath 2.0

                    if (pred.Equals("equals"))
                    {
                        newXPath = "xs:date(" + xpathExisting.dataBinding.xpath + ") = xs:date('" + val + "')";
                        pairing.titleText = "If '" + val + "' for Q: " + q.text;
                    }
                    else if (pred.Equals("is before"))
                    {
                        newXPath = "xs:date(" + xpathExisting.dataBinding.xpath + ") < xs:date('" + val + "')";
                        pairing.titleText = "If before '" + val + "' for Q: " + q.text;
                    }
                    else if (pred.Equals("is after"))
                    {
                        newXPath = "xs:date(" + xpathExisting.dataBinding.xpath + ") > xs:date('" + val + "')";
                        pairing.titleText = "If after '" + val + "' for Q: " + q.text;
                    }
                    else
                    {
                        log.Error("unexpected predicate " + pred);
                        return null;
                    }

                }
                else
                {
                    log.Error("Unexpected data type " + xpathExisting.type);
                    return null;
                }
            }


            // Drop any trailing "/" from a Condition XPath
            if (newXPath.EndsWith("/"))
            {
                newXPath = newXPath.Substring(0, newXPath.Length - 1);
            }
            log.Debug("Creating condition using XPath:" + newXPath);

            xpathsXpath xpath = xppe.setup("cond", xpathExisting.dataBinding.storeItemID,
                newXPath, xpathExisting.dataBinding.prefixMappings, false);
            xppe.save();

            pairing.xpathEntry = xpath;

            return pairing;

        }

        public class Pairing
        {
            public xpathsXpath xpathEntry { get; set; }

            public string titleText { get; set; }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }



    }
}
