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
    class ConditionsFormRowHelper
    {
        static Logger log = LogManager.GetLogger("ConditionsFormRowHelper");

        /// <summary>
        /// Magic type for getting the position of this instance in a repeat
        /// </summary>
        //public static string REPEAT_POS = "repeated pos";

        /// <summary>
        /// Magic type for getting the number of instances this repeat has
        /// </summary>
        //public static string REPEAT_COUNT = "repeat count";

        public static string NEW_QUESTION = "New Question..";

        private DataGridView dataGridView;

        private DataGridViewComboBoxCell listBoxTypeFilter;
        private DataGridViewComboBoxCell listBoxQuestions;
        private DataGridViewComboBoxCell listBoxPredicate;
        private DataGridViewComboBoxCell comboBoxValues;

        private Model model;
        protected XPathsPartEntry xppe;
        private questionnaire questionnaire;

        protected Word.ContentControl cc;

        Forms.FormConditionBuilder fcb;

        /// <summary>
        /// For each question, I need to keep track of known values.
        /// (even for MCQ, since the user can add a value).
        /// 
        /// Use this same set of possible values in all rows.
        ///
        /// When a question is first selected, this object
        /// is populated for it.
        /// 
        /// Since it is static, it SHOULD be reused each time a
        /// Condition is set up (might need to loosen question equality?),
        /// and also in condition editor :-),
        /// but not across different Word sessions.
        /// 
        /// </summary>
        static Dictionary<question, List<object>> augmentedValues = new Dictionary<question, List<object>>();

        private List<object> augmentResponses(question q)
        {
            List<object> stuff = null;

            try
            {
                stuff = augmentedValues[q];
            }
            catch (KeyNotFoundException)
            {
                // Set it up
                stuff = new List<object>();

                if (q.response.Item is responseFixed)
                {
                    stuff.AddRange(((responseFixed)q.response.Item).item);
                }

                augmentedValues.Add(q, stuff);

            }
            return stuff;
        }

         /* 
         * For each question, I need to keep track of the last value
         * it had on a given row.  This I can store in DataGridViewBand.Tag
         * so a Dictionary<Q, object> is OK for this.
          * 
          * Actually, the last value seems to be retained. Without looking
          * to see why/how, I don't need to do anything.
         * 
         */

        public ConditionsFormRowHelper(Model model, XPathsPartEntry xppe, questionnaire questionnaire, 
                    Word.ContentControl cc, Forms.FormConditionBuilder fcb) {

            this.model = model;
            this.xppe = xppe;
            this.questionnaire = questionnaire;
            this.fcb = fcb;

            this.cc = cc;        
        }

        public void init(DataGridView dataGridView)
        {
            this.dataGridView = dataGridView;

            // I wanted to programmatically manage when
            // a new row is added, but it doesn't seem to
            // work (my new row has index -1).
            // So let the control manage this itself
            // dataGridView.AllowUserToAddRows = false; 

            // in order that you don't have to click twice
            dataGridView.EditMode = DataGridViewEditMode.EditOnEnter;

            dataGridView.CellEndEdit += new DataGridViewCellEventHandler(dataGridView_CellEndEdit);

            // These 2 events facilitate user entering text.
            dataGridView.EditingControlShowing += new DataGridViewEditingControlShowingEventHandler(dataGridView_EditingControlShowing);
            dataGridView.CellValidating += new DataGridViewCellValidatingEventHandler(dataGridView_CellValidating);

//            dataGridView.RowsAdded += new DataGridViewRowsAddedEventHandler(dataGridView_RowsAdded);

            dataGridView.RowEnter += new DataGridViewCellEventHandler(dataGridView_RowEnter);

            dataGridView.DefaultValuesNeeded += new DataGridViewRowEventHandler(dataGridView_DefaultValuesNeeded);

            dataGridView.DataError += new DataGridViewDataErrorEventHandler(dataGridView_DataError);

            dataGridView.CurrentCellChanged += new EventHandler(dataGridView_CurrentCellChanged);
            dataGridView.CellEnter+=new DataGridViewCellEventHandler(dataGridView_CellEnter);

           // dataGridView.CellMouseDoubleClick += new DataGridViewCellMouseEventHandler(dataGridView_CellMouseDoubleClick);
        }

        //void dataGridView_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        //{
        //    log.Error("CellMouseDoubleClick: " + sender.GetType().FullName);
        //    DataGridView dgv = sender as DataGridView;

        //    if (dgv.Columns[dgv.CurrentCell.ColumnIndex].Name.Equals("Questions")
        //        && myCachedComboBox!=null)
        //    {
        //        object o = myCachedComboBox.SelectedItem;
        //        if (o is string
        //            &&((string)o).Equals(NEW_QUESTION)) {
        //            MessageBox.Show("You doubleclicked new question!");
        //        }

        //    }            
        //}



        void dataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            
            log.Error("DataError: " + e.Exception + " column " + dataGridView.Columns[e.ColumnIndex].Name );  // Can't get stack trace :-(

            DataGridViewComboBoxCell cell = (DataGridViewComboBoxCell)dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];

            object val = cell.Value;

            log.Error(val.GetType().FullName);
            log.Error(val);
            
        }

        void dataGridView_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            log.Debug("DefaultValuesNeeded fired " );
            DataGridViewRow row = e.Row;
            listBoxTypeFilter = (DataGridViewComboBoxCell)row.Cells["Filter"];
            listBoxQuestions = (DataGridViewComboBoxCell)row.Cells["Questions"];
            listBoxPredicate = (DataGridViewComboBoxCell)row.Cells["Predicate"];
            comboBoxValues = (DataGridViewComboBoxCell)row.Cells["Value"];
            populateRow(row, null, null);

            fcb.textBoxEnglish.Text = "";
            string msg = fcb.isValid();
            if (msg != null)
            {
                fcb.textBoxEnglish.Text = msg;
            }

        }

        void dataGridView_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            log.Debug("RowEnter fired " + e.RowIndex);
            DataGridViewRow row = this.dataGridView.Rows[e.RowIndex];
            listBoxTypeFilter = (DataGridViewComboBoxCell)row.Cells["Filter"];
            listBoxPredicate = (DataGridViewComboBoxCell)row.Cells["Predicate"];
            comboBoxValues = (DataGridViewComboBoxCell)row.Cells["Value"];

            listBoxQuestions = (DataGridViewComboBoxCell)row.Cells["Questions"];

            // Wanted to do the below, to refresh the question list
            // after new question.  But populateQuestions also calls
            // clearComboBoxValues().
            // The problem is that RowEnter seems to be invoked
            // when the user adds to the Value cell
            // (what happens is that DefaultValuesNeeded fires, then
            //  after that, the existing row is RowEntered again!)
            // So for now, we don't repopulate here :-(

            //object val = listBoxQuestions.Value;
            //if (val is RepeatCount
            //    || val is RepeatPosition)
            //{
            //    populateQuestions("repeats");
            //}
            //else
            //{
            //    populateQuestions("ALL");
            //}
            
        }

        //void dataGridView_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        //{
        //    log.Debug("RowsAdded fired " + e.RowIndex);
        //    DataGridViewRow row = this.dataGridView.Rows[e.RowIndex];
        //    populateRow(row, null, null);

        //    // The Item property of class DataGridViewRowCollection creates a clone of the GridViewRow and returns that
        //}


        void dataGridView_CurrentCellChanged(object sender, EventArgs e)
        {
            log.Debug("\n\r CHANGED CELL \n\r");
            fcb.textBoxEnglish.Text = "";
            string msg = fcb.isValid();
            if (msg != null)
            {
                fcb.textBoxEnglish.Text = msg;
            }
        }

        void dataGridView_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            log.Debug("CellEnter " + sender.GetType().FullName);
            // e gives col, row
        } 

        /// <summary>
        /// must be a drop down if the user is to be able to type...
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            log.Debug("dataGridView_EditingControlShowing.. " + sender.GetType().FullName);
            this.myCachedComboBox = (ComboBox)e.Control; // the only way to get the selected object

            

            //dataGridView.EditingControl.TextChanged += new EventHandler(EditingControl_TextChanged);

            //  the DropDownStyle property of the ComboBox editing control needs to be set to DropDown to enable typing in the combo box
            ComboBox c = e.Control as ComboBox;
            if (c != null) c.DropDownStyle = ComboBoxStyle.DropDown;

            // Set the ComboBox name, so we can use it ...
            DataGridView dgv = sender as DataGridView;
            //myCachedRowIndex = dgv.CurrentCell.RowIndex;

            c.Name = dgv.Columns[dgv.CurrentCell.ColumnIndex].Name;
            log.Debug(".. combo box name: " + c.Name);

            if (c.Name.Equals("Filter") 
                || c.Name.Equals("Questions")) {

                    if (comboBoxEvents.Contains(c))
                    {
                        // We've already set up our event, so do nothing
                    }
                    else
                    {
                        c.SelectedValueChanged += new EventHandler(c_SelectedValueChanged);
                        comboBoxEvents.Add(c);
                    }
            }

            /* Sequence of events when adding a value in the value column:
             * 
                OpenDope_AnswerFormat.Helpers.ConditionsFormRowHelper.dataGridView_EditingControlShowing .. combo box name: Value
             * 
             * .. when you click on another cell, or press enter
             * 
                OpenDope_AnswerFormat.Helpers.ConditionsFormRowHelper.dataGridView_CellValidating CellValidating.. 
                OpenDope_AnswerFormat.Helpers.ConditionsFormRowHelper.dataGridView_CellValidating .. set value to kkk
                OpenDope_AnswerFormat.Helpers.ConditionsFormRowHelper.dataGridView_CellEndEdit CellEndEdit .. 
             * 
             * .. only if you click on another cell
             * 
                OpenDope_AnswerFormat.Helpers.ConditionsFormRowHelper.dataGridView_CurrentCellChanged 
             * 
             * .. only if you press enter (creating another row)
             * 
                OpenDope_AnswerFormat.Helpers.ConditionsFormRowHelper.dataGridView_DefaultValuesNeeded DefaultValuesNeeded fired 
                OpenDope_AnswerFormat.Helpers.ConditionsFormRowHelper.populateRow populateRow: 
             */
        }


        List<ComboBox> comboBoxEvents = new List<ComboBox>();


        void c_SelectedValueChanged(object sender, EventArgs e)
        {
            // this event seems buggy .. sometimes sender is the
            // control in the cell we've already left!
            // myCachedComboBox is made null in CellEndEdit,
            // so a work around is to test for that
            if (myCachedComboBox != null)
            {
                DataGridViewComboBoxEditingControl comboBoxEditingControl = sender as DataGridViewComboBoxEditingControl;
                log.Debug("c_SelectedValueChanged.. " + comboBoxEditingControl.Name + " --> " + comboBoxEditingControl.Text);


                //log.Debug("obj.SelectedIndex " + comboBoxEditingControl.SelectedIndex);
                //log.Debug("obj.Text " + comboBoxEditingControl.Text);
                //log.Debug("obj.SelectedItem " + comboBoxEditingControl.SelectedItem);
                // no good log.Debug("obj.SelectedValue " + obj.SelectedValue);
                // no good log.Debug("obj.SelectedText " + obj.SelectedText); 

                // Value changes as user cursors up/down.
                // Maybe we want DataGridView.CellExit or CellLeave?

                if (comboBoxEditingControl.Name != null && comboBoxEditingControl.Name.Equals("Filter"))
                {
                    filterAction(comboBoxEditingControl.Text);
                }
                else if (comboBoxEditingControl.Name != null && comboBoxEditingControl.Name.Equals("Questions"))
                {
                    object o = myCachedComboBox.SelectedItem;
                    if (o is string)
                    {
                        if (((string)o).Equals(NEW_QUESTION))
                        {
                            QuestionCreateNew();
                        }
                        // otherwise, that'd be the empty message
                        // do nothing
                    }
                    else
                    {
                        questionSelectedAction(myCachedComboBox.SelectedItem);
                    }
                }
            }
        }

        private void QuestionCreateNew()
        {
            FormQA formQA = new FormQA(cc, false);
            formQA.ShowDialog();
            question q = formQA.getQuestion();
            formQA.Dispose();

            // Refresh these
            xppe = new XPathsPartEntry(model);
            fcb.xppe = xppe;

            questionnaire.Deserialize(model.questionsPart.XML, out questionnaire);
            fcb.questionnaire = questionnaire;

            // How do we know what the new question was? Just refresh..
            //listBoxTypeFilter.Value = "ALL";
            //populateQuestions("ALL");
            //dataGridView.RefreshEdit(); //critical

            listBoxQuestions.Items.Add(q);
            listBoxQuestions.Value = q;
            //myCachedComboBox.SelectedItem = q;
            dataGridView.RefreshEdit(); //critical
            log.Debug("set question " + q);

            //log.Debug("Repopulating row " + rowIndex);
            //DataGridViewRow row = dataGridView.Rows[rowIndex];
            //populateRow(row, null, null);
            //dataGridView.RefreshEdit();
            
        }


        /// <summary>
        /// Add value typed by user in Value column only
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            log.Debug("CellValidating.. ");
            if (dataGridView.Columns[e.ColumnIndex] == dataGridView.Columns["Value"])
            {
                DataGridViewComboBoxCell cell = (DataGridViewComboBoxCell)dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];
                object eFV = e.FormattedValue;

                object selectedItem = null;
                foreach (object o in cell.Items)
                {
                    if (o.ToString().Equals(eFV))
                    {
                        selectedItem = o;
                        log.Info("Identified item " + selectedItem);
                        break;
                    }
                }

                if (selectedItem == null)
                {
                    cell.Items.Add(eFV);
                    log.Info("added item: " + eFV);

                    cell.Value = eFV; // needs UpdateCellValue (which I have in CellEndEdit) in order for the selection to show!
                    //see http://social.msdn.microsoft.com/Forums/en-US/winformsdatacontrols/thread/be964ef4-daf9-4c5b-8e5a-08bd9d5ad3f9/

                    dataGridView.UpdateCellValue(e.ColumnIndex, e.RowIndex); // must have this

                    mustSetSelectedItemInCellEndEdit = false;

                    // now add this
                    if (currentQuestion!=null) {
                        List<object> values = augmentResponses(currentQuestion);
                        values.Add(eFV);
                    }
                }
                else
                {
                    // they just selected an existing choice
                    // cell.Value = myCachedComboBox.SelectedItem;
                    // that's what I want, but it gives "data error" here;
                    // must do it in dataGridView_CellEndEdit - God knows why.

                    mustSetSelectedItemInCellEndEdit = true;

                }

            }
        }



        bool mustSetSelectedItemInCellEndEdit;

        private ComboBox myCachedComboBox;
        //private int myCachedRowIndex;

        /// <summary>
        /// This event only fires if they type something; it doesn't fire when
        /// selected value changes. That's why we need that event as well.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void dataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            //dataGridView.EditingControl.TextChanged -= new EventHandler(EditingControl_TextChanged);
            DataGridViewComboBoxCell cell = (DataGridViewComboBoxCell)dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];
            log.Debug("CellEndEdit .. ");
            if (dataGridView.Columns[e.ColumnIndex] == dataGridView.Columns["Filter"])
            {
                // Do nothing .. we did it in c_SelectedValueChanged
                //filterAction((string)myCachedComboBox.SelectedItem );

            } else if (dataGridView.Columns[e.ColumnIndex] == dataGridView.Columns["Questions"])
            {
                object o = myCachedComboBox.SelectedItem;
                cell.Value = o; // Why is this necessary??? Without it, the cell value is set as a string, which is wrong!
                if (o is string)
                {
                    // that's be the empty message
                    // do nothing
                }
                else
                {
                    questionSelectedAction(myCachedComboBox.SelectedItem);
                }
            }
            else if (dataGridView.Columns[e.ColumnIndex] == dataGridView.Columns["Value"])
            {
                if (mustSetSelectedItemInCellEndEdit)
                {
                    cell.Value = myCachedComboBox.SelectedItem; // see comments in CellValidating above
                }
            }
            myCachedComboBox = null;

        }

        //public void createRow()
        //{
        //    DataGridViewRow dr = (DataGridViewRow)dataGridView.Rows[0].Clone();

        //    //dr.Cells.Add(new DataGridViewComboBoxCell());
        //    //dr.Cells.Add(new DataGridViewComboBoxCell());
        //    //dr.Cells.Add(new DataGridViewComboBoxCell());
        //    //dr.Cells.Add(new DataGridViewComboBoxCell());
        //    //dr.Cells.Add(new DataGridViewComboBoxCell());

        //    dataGridView.Rows.Add(dr);
        //    dataGridView.Refresh();
        //    populateRow(dr, null, null);
        //}

        //void EditingControl_TextChanged(object sender, EventArgs e)
        //{
        //    DataGridViewComboBoxEditingControl control = (DataGridViewComboBoxEditingControl)sender;
        //    log.Debug("TextChanged .. Value is: " + control.Text);
        //}

        public void populateRow(DataGridViewRow row, question existingQuestion, string matchResponse)
        {
            log.Debug("populateRow: " );

            listBoxTypeFilter = (DataGridViewComboBoxCell)row.Cells["Filter"];
            listBoxQuestions = (DataGridViewComboBoxCell)row.Cells["Questions"];
            listBoxPredicate = (DataGridViewComboBoxCell)row.Cells["Predicate"];
            comboBoxValues = (DataGridViewComboBoxCell)row.Cells["Value"];

            populateTypeFilter(null);
            

            if (existingQuestion == null)
            {
                // for init, populate with all questions
                populateQuestions(null);
            }
            else
            {
                // Just show the existing question
                listBoxQuestions.Items.Add(existingQuestion);
            }

            ////if (this.listBoxQuestions.Items.Count == 0) // Never happens if in a repeat, and nor do we want it to, since user might just want to use "repeat pos" stuff 
            ////{
            ////    populateQuestions(null);
            ////    if (this.listBoxQuestions.Items.Count == 0)
            ////    {
            ////        MessageBox.Show("You can't define a condition until you have set up at least one question. Let's do that now. ");

            ////        FormQA formQA = new FormQA(cc, false);
            ////        formQA.ShowDialog();
            ////        formQA.Dispose();

            ////        // Refresh these
            ////        xppe = new XPathsPartEntry(model);
            ////        questionnaire.Deserialize(questionsPart.XML, out questionnaire);

            ////        filterAction();

            ////        return;
            ////    }
            ////}
            //// value 
            //object o;
            //if (existingQuestion == null)
            //{
            //    // for init, populate with all questions
            //    o = this.listBoxQuestions.Items[0];
            //}
            //else
            //{
            //    o = existingQuestion;
            //}

            //questionSelectedAction(o);
            //this.listBoxQuestions.Value = o;

            ////if (q.response.Item is responseFixed)
            ////{
            ////    populateValues((responseFixed)q.response.Item, matchResponse);
            ////}

            ////// predicate =
            ////populatePredicates(q);  // TODO: set this correctly in editing mode

        }

        string ALL_QUESTION_TYPES="ALL QUESTIONS";
        /// <summary>
        /// There will ALWAYS be something selected here.
        /// </summary>
        /// <param name="includeRptMagicTypes"></param>
        public void populateTypeFilter(string selected)
        {
            log.Debug("populateTypeFilter: " + selected);

            listBoxTypeFilter.Items.Clear();

            listBoxTypeFilter.Items.Add("text");
            listBoxTypeFilter.Items.Add("number");
            listBoxTypeFilter.Items.Add("date");
            listBoxTypeFilter.Items.Add("boolean");
            listBoxTypeFilter.Items.Add(ALL_QUESTION_TYPES); // is currently all except repeats!
            listBoxTypeFilter.Items.Add("repeats");
            listBoxTypeFilter.Items.Add("conditions");


            if (selected == null)
            {
                listBoxTypeFilter.Value = ALL_QUESTION_TYPES;
            //}
            //else if (selected.Equals("string"))
            //{
            //    listBoxTypeFilter.Value = "text";
            } else 
            {
                listBoxTypeFilter.Value = selected;
            }
        }

        public void populateQuestions(string type)
        {
            log.Debug("populateQuestions of type " + type);
            listBoxQuestions.Value = null; 
            listBoxQuestions.Items.Clear();

            if (type != null && type.Equals("repeats"))
            {
                populateRepeats();
                return;
            }

            if (type != null && type.Equals("conditions"))
            {
                populateConditions();
                return;
            }

            HashSet<question> questions = null;

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

            if (questions.Count == 0)
            {
                // None of any type
                string message = "No questions in scope!";
                this.listBoxQuestions.Items.Add(message);
                this.listBoxQuestions.Value = message;

                freezeAsNoQuestions();
            }
            else
            {
                // filter and add
                foreach (question q in questions)
                {

                    xpathsXpath xpath = xppe.getXPathByQuestionID(q.id);
                    if (xpath.dataBinding.xpath.EndsWith("oda:row"))
                    {
                        // Ignore raw repeats

                    } else if (type == null
                       || type.Equals(ALL_QUESTION_TYPES))
                    {
                        // all questions
                        listBoxQuestions.Items.Add(q);
                    }
                    else
                    {
                        if (xpath.type != null
                            && xpath.type.Equals(type))
                        {
                            listBoxQuestions.Items.Add(q);
                            log.Debug("Added to listbox " + q.id);

                        }
                    }
                }

                if (this.listBoxQuestions.Items.Count > 0)
                {
                    object o = this.listBoxQuestions.Items[0];
                    this.listBoxQuestions.Value = o;

                    if (o is question)
                    {
                        currentQuestion = (question)this.listBoxQuestions.Items[0];
                        populateValues(augmentResponses(currentQuestion), null);
                    }
                    else
                    {
                        currentQuestion = null;
                    }
                    populatePredicates(o);
                }
                else
                {
                    // None of this type
                    string message = "No questions of type " + type;
                    this.listBoxQuestions.Items.Add(message);
                    this.listBoxQuestions.Value = message;

                    freezeAsNoQuestions();
                }
            }

            this.listBoxQuestions.Items.Add(NEW_QUESTION);
            //if (this.listBoxQuestions.Items.Count == 2)
            //{
            //    this.listBoxQuestions.Value = NEW_QUESTION;
            //}


        }

        public void clearComboBoxValues()
        {
            this.comboBoxValues.Value = null;
            this.comboBoxValues.Items.Clear();
        }

        private void freezeAsNoQuestions()
        {
            log.Debug("emptying predicates");

            this.listBoxPredicate.Value = null;
            this.listBoxPredicate.Items.Clear();
            this.listBoxPredicate.ReadOnly = true;

            this.comboBoxValues.Value = null;
            this.comboBoxValues.Items.Clear();
            this.comboBoxValues.ReadOnly = true;

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
                        listBoxQuestions.Items.Add(new RepeatCount(q));
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
                listBoxQuestions.Items.Add( new RepeatPosition(q) );
                listBoxQuestions.Items.Add(new RepeatCount(q));
                log.Debug("Added to listbox " + q.id);

                // Now add its child repeats (if any)
                foreach (object o in r.row[0].Items)
                {
                    if (o is repeat)
                    {
                        string qid = ((repeat)o).qref;
                        question q2 = questionnaire.getQuestion(qid);
                        listBoxQuestions.Items.Add(new RepeatCount(q2));
                        log.Debug("Added to listbox " + q2.id);
                    }
                }
            }

            // Set value to whatever is listed first
            if (this.listBoxQuestions.Items.Count > 0)
            {
                object o = this.listBoxQuestions.Items[0];
                this.listBoxQuestions.Value = o;
                clearComboBoxValues();
                populatePredicates(o);
            }
            else
            {
                // None of this type
                string message = "No repeat conditions available here";
                this.listBoxQuestions.Items.Add(message);
                this.listBoxQuestions.Value = message;

                freezeAsNoQuestions();
            }


        }

        public bool isConditionInScope(List<String> repeatXPaths, List<xpathsXpath> xpathsUsedInCondition)
        {

            foreach (xpathsXpath xp in xpathsUsedInCondition)
            {
                //log.Info("condition " + c.id + " uses " + xp.dataBinding.xpath);

                if (xp.dataBinding.xpath.IndexOf("oda:repeat") < 0)
                {
                    continue;
                }

                // this xpath is in some repeat.
                // is it ok
                string cXPath = xp.dataBinding.xpath; // eg  string(/oda:answers/oda:repeat[@qref='items_oc']/....
                cXPath = cXPath.Substring(cXPath.IndexOf("/oda:answers"));
                cXPath = cXPath.Substring(0, cXPath.LastIndexOf("oda:row"));

                // we want xpaths containing this xpath,
                // and no extra repeat
                bool isInScope = false; 
                foreach (string repeatXPath in repeatXPaths)
                {
                    if (repeatXPath.Contains(cXPath)
                         )
                    {
                        log.Debug(repeatXPath + " contains " + cXPath);
                        if (repeatXPath.LastIndexOf("oda:repeat") < cXPath.Length)
                        {
                            log.Debug("..and not more");
                            isInScope = true;
                            break;
                        }
                        else
                        {
                            log.Debug("..and more");
                        }
                    }
                    else
                    {
                        log.Debug(repeatXPath + " doesn't contain " + cXPath);
                    }
                }
                if (!isInScope)
                {
                    return false;
                }

            }
            return true;

        }

        protected void populateConditions()
        {
            // Get list of repeat ancestors
            List<String> repeatXPaths = new List<String>();
            Word.ContentControl currentCC = cc.ParentContentControl;
            while (currentCC != null)
            {
                if (currentCC.Tag.Contains("od:repeat"))
                {
                    TagData td = new TagData(currentCC.Tag);
                    string rXPathID = td.getRepeatID();
                    xpathsXpath xp = xppe.getXPathByID(rXPathID);
                    repeatXPaths.Add(xp.dataBinding.xpath);
                }
                currentCC = currentCC.ParentContentControl;
            }


            foreach (condition c in fcb.conditions.condition)
            {
                log.Debug(c.id);
                
                // Limit to what is in scope. A condition is 
                // in scope of it does not use a question which varies 
                // in some repeat which is out of scope.
                List<xpathsXpath> xpathsUsedInCondition = new List<xpathsXpath>();
                c.listXPaths(xpathsUsedInCondition, fcb.conditions, xppe.xpaths);

                //foreach(xpathsXpath xp in theList) {
                //    log.Info("condition " + c.id + " uses " + xp.dataBinding.xpath);
                //}
                if (isConditionInScope(repeatXPaths, xpathsUsedInCondition))
                {
                    listBoxQuestions.Items.Add(c);
                }
                else
                {
                    log.Warn("Condition " + c.id + " is out of scope");
                }

            }

            if (this.listBoxQuestions.Items.Count > 0)
            {
                object o = this.listBoxQuestions.Items[0];
                this.listBoxQuestions.Value = o;

                // Freeze predicate and value columns since these aren't relevant
                freezeAsNoQuestions();

            }
            else
            {
                // None of this type
                string message = "No conditions available for re-use";
                this.listBoxQuestions.Items.Add(message);
                this.listBoxQuestions.Value = message;

                freezeAsNoQuestions();
            }


        }

/*
 * 
 *             xpaths xpaths = xppe.xpaths;

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

 */ 

        public void filterAction(string selected)
        {
            // Get the currently selected item in the ListBox.
            string type = null;
            if (selected== null)
            {
                log.Error("null selection in filter!");  // should never happen
            } else {
                type = selected;
                //map
                if (type.Equals("text"))
                {
                    type = "string";
                }
            }

            populateQuestions(type);

        }

        question currentQuestion;

        /// <summary>
        /// When user clicks a question, we need to 
        /// (1) populate predicates for that type
        /// (2) if its MCQ, populate values
        /// </summary>
        /// <param name="q"></param>
        public void questionSelectedAction(object q)
        {
            // input is a question, or RepeatPosition or RepeatCount
            // or
            if (q is condition)
            {
                currentQuestion = null;
                return;
            }
            log.Debug("questionSelectedAction " + q);

            // (1) populate predicates
            populatePredicates(q);

            // (2) populate values
            if (q is question)
            {
                currentQuestion = (question)q;
                populateValues(augmentResponses(currentQuestion), null);
            }
            else
            {
                currentQuestion = null;
                clearComboBoxValues();
            }

            // Choice of question should NEVER update filter.
            // Changes only flow from Filter -> Qs -> selected Q -> predicate & value

            //xpathsXpath xpath = xppe.getXPathByQuestionID(q.id);
            ////this.listBoxTypeFilter.SelectedIndexChanged -= new System.EventHandler(this.listBoxTypeFilter_SelectedIndexChanged);
            //if (xpath.dataBinding.xpath.EndsWith("oda:row"))
            //{
            //    if (listBoxTypeFilter.Value.Equals(REPEAT_COUNT)
            //        || listBoxTypeFilter.Value.Equals(REPEAT_POS))
            //    {
            //        // Do nothing
            //    }
            //    else
            //    {
            //        populateTypeFilter(true, REPEAT_POS);
            //    }
            //}
            //else
            //{
            //    populateTypeFilter(false, xpath.type);
            //}
            ////this.listBoxTypeFilter.SelectedIndexChanged += new System.EventHandler(this.listBoxTypeFilter_SelectedIndexChanged);


        }

        

        //public void populateValues(responseFixed responses, string matchResponse)
        public void populateValues(List<object> responses, string matchResponse)
        {
            this.comboBoxValues.ReadOnly = false;
            this.comboBoxValues.Items.Clear();

            //foreach (responseFixedItem item in responses)
            foreach (object item in responses)
            {
                int currentIndex = this.comboBoxValues.Items.Add(item);
                if (matchResponse != null && matchResponse.Contains(item.ToString()))  // item.value
                {
                    // Simple minded matching, will do for now.
                    // Saves us having an XPath parser.
                    this.comboBoxValues.Value = item;
                }
            }

        }




        public void populatePredicates(object o)
        {
            this.listBoxPredicate.ReadOnly = false;
            this.listBoxPredicate.Items.Clear();

            if (o is RepeatCount)
            {
                this.listBoxPredicate.Items.Add("=");
                this.listBoxPredicate.Items.Add(">");
                this.listBoxPredicate.Items.Add(">=");
                this.listBoxPredicate.Items.Add("<");
                this.listBoxPredicate.Items.Add("<=");

                this.listBoxPredicate.Value = "=";
                return;
            }
            else if (o is RepeatPosition)
            {
                this.listBoxPredicate.Items.Add("first");
                this.listBoxPredicate.Items.Add("not first");
                this.listBoxPredicate.Items.Add("second");
                this.listBoxPredicate.Items.Add("second last");
                this.listBoxPredicate.Items.Add("last");
                this.listBoxPredicate.Items.Add("not last");

                this.listBoxPredicate.Value = "last";
                return;

            }
            else if (o is question)
            {
                // good - handle below
            }
            else
            {
                log.Error("Unexpected type " + o.GetType().FullName);
            }

            string type = null;
            question q = (question)o;
            xpathsXpath xpath = xppe.getXPathByQuestionID(q.id);
            type = xpath.type;
            
            //if (xpath.dataBinding.xpath.EndsWith("oda:row")
            //        && type == null)
            //    {
            //        type = REPEAT_POS; // default
            //    }
            //    else
            //    {
            //        type = xpath.type;
            //    }


            log.Debug("populatePredicates for type:" + type);

            if (type == null)
            {
                log.Error("type missing for xpath " + xpath.Serialize());
            }
            else if (type.Equals("string"))
            {
                this.listBoxPredicate.Items.Add("equals");
                this.listBoxPredicate.Items.Add("is not");
                this.listBoxPredicate.Items.Add("starts-with");
                this.listBoxPredicate.Items.Add("contains");
                this.listBoxPredicate.Items.Add("not blank.");

                this.listBoxPredicate.Value = "equals";
            }
            else if (type.Equals("boolean"))
            {
                this.listBoxPredicate.Items.Add("equals");
            }
            else if (type.Equals("decimal")
              || type.Equals("integer")
              || type.Equals("positiveInteger")
              || type.Equals("nonPositiveInteger")
              || type.Equals("negativeInteger")
              || type.Equals("nonNegativeInteger")  // repeat
              )
            {
                this.listBoxPredicate.Items.Add("=");
                this.listBoxPredicate.Items.Add(">");
                this.listBoxPredicate.Items.Add(">=");
                this.listBoxPredicate.Items.Add("<");
                this.listBoxPredicate.Items.Add("<=");

                this.listBoxPredicate.Value = "=";
            }
            else if (type.Equals("date"))
            {
                this.listBoxPredicate.Items.Add("equals");
                this.listBoxPredicate.Items.Add("is before");
                this.listBoxPredicate.Items.Add("is after");

                this.listBoxPredicate.Value = "equals";
            }
            // TODO: flesh this out with the full range of allowable datatypes 
            // (card number, email address, custom types)

        }



    }
}
