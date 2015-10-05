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
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using OpenDoPEModel;
using NLog;
using Office = Microsoft.Office.Core;

namespace OpenDope_AnswerFormat.Controls
{
    public partial class ControlQuestionResponsesFixed : UserControl
    {
        static Logger log = LogManager.GetLogger("ControlQuestionResponsesFixed");

        private responseFixed responseFixed;

        public ControlQuestionResponsesFixed()
        {
            InitializeComponent();

            // in order that you don't have to click twice
            dataGridView1.EditMode = DataGridViewEditMode.EditOnEnter;

        }

        public void init(responseFixed responseFixed) {

            this.responseFixed = responseFixed;

            // Pre-populate with true/false
            this.dataGridView1.Rows.Add(2);
            this.dataGridView1.Rows[0].Cells[0].Value = "true"; // val
            this.dataGridView1.Rows[0].Cells[1].Value = "yes";  // label

            this.dataGridView1.Rows[1].Cells[0].Value = "false";
            this.dataGridView1.Rows[1].Cells[1].Value = "no";
        }

        public void populateControl(xpathsXpath xpathObj, question q, string defaultAnswer)
        {
            responseFixed =  q.response.Item as responseFixed;

            if (xpathObj.type.Equals("string"))
            {
                this.radioTypeText.Checked = true;

            }
            else if (xpathObj.type.Equals("decimal"))
            {
                this.radioTypeNumber.Checked = true;

            }
            else if (xpathObj.type.Equals("date"))
            {
                this.radioTypeDate.Checked = true;

            }
            else if (xpathObj.type.Equals("boolean"))
            {
                this.radioTypeBoolean.Checked = true;

            }
            //else if (xpathObj.type.Equals("duration"))
            //{
            //    this.radioTypeText.Checked = true;

            //}
            //else if (xpathObj.type.Equals("email"))
            //{
            //    this.radioTypeText.Checked = true;

            //}
            //else if (xpathObj.type.Equals("cardnumber"))
            //{
            //    this.radioTypeText.Checked = true;
            //}
            else
            {
                log.Error("XPath " + xpathObj.id + " has unknown value for datatype: '" + xpathObj.type);
            }

            // The possible responses
            int i = 0;
            this.dataGridView1.Rows.Add(responseFixed.item.Count);
            foreach (responseFixedItem rFI in responseFixed.item)
            {
                this.dataGridView1.Rows[i].Cells[0].Value = rFI.value;
                this.dataGridView1.Rows[i].Cells[1].Value = rFI.label;
                i++;
            }


            // How many responses can be checked?
            if (responseFixed.canSelectMany)
            {
                this.radioButtonYes.Checked = true;
            }
            else {
                this.radioButtonNo.Checked = true;
            }

            // Interview appearance
            if (q.appearance.Equals(appearanceType.compact)) {
                this.radioButtonAppearanceCompact.Checked = true;
            }
            else if (q.appearance.Equals(appearanceType.full)) {
                this.radioButtonAppearanceFull.Checked = true;
            }
            else if (q.appearance.Equals(appearanceType.minimal) ){
                this.radioButtonAppearanceMinimal.Checked = true;
            }

            // Default
            this.textBoxDefault.Text = defaultAnswer;

        }

        public bool isValid()
        {
            if (this.dataGridView1.Rows.Count < 3) // auto last row
            {
                Mbox.ShowSimpleMsgBoxError("You must provide at least 2 choices!");
                return false;
            }

            int last = this.dataGridView1.Rows.Count -1;
            foreach (DataGridViewRow row in this.dataGridView1.Rows)
            {
                // Last row is added automatically
                if (row == this.dataGridView1.Rows[last]) continue;

                DataGridViewCell c = row.Cells[0];
                if (string.IsNullOrWhiteSpace((string)c.Value))
                {
                    Mbox.ShowSimpleMsgBoxError("You must enter data in each cell!");
                    return false;
                }
                c = row.Cells[1];
                if (string.IsNullOrWhiteSpace((string)c.Value))
                {
                    Mbox.ShowSimpleMsgBoxError("You must enter data in each cell!");
                    return false;
                }
            }
            return true;
        }

        // TODO, check values match data type!

        public void injectItems() {

            int last = this.dataGridView1.Rows.Count - 1;
            foreach (DataGridViewRow row in this.dataGridView1.Rows)
            {
                if (row == this.dataGridView1.Rows[last]) continue;

                responseFixedItem item = new OpenDoPEModel.responseFixedItem();

                item.value = (string)row.Cells[0].Value;
                item.label = (string)row.Cells[1].Value;

                responseFixed.item.Add(item);
            }

            if (this.radioButtonYes.Checked)
            {
                responseFixed.canSelectMany = true;
            }
            else
            {
                responseFixed.canSelectMany = false;
            }
            responseFixed.canSelectManySpecified = true; // have to set this!

        }

        public appearanceType getAppearanceType()
        {
            // AppearanceType
            if (this.radioButtonAppearanceFull.Checked)
            {
                return appearanceType.full;
            }
            else if (this.radioButtonAppearanceCompact.Checked)
            {
                return appearanceType.compact;
            }
            else if (this.radioButtonAppearanceMinimal.Checked)
            {
                return appearanceType.minimal;
            }
            return appearanceType.full;
        }

        public string getDataType()
        {
            // TODO: values to be validated against selected data type
            if (this.radioTypeBoolean.Checked)
            {
                return "boolean";
            }
            else if (this.radioTypeDate.Checked)
            {
                return "date";
            }
            else if (this.radioTypeNumber.Checked)
            {
                return "decimal"; // allows integer
            }
            else if (this.radioTypeText.Checked)
            {
                return "string";
            }
            else
            {   // default
                return "string";
            }
        }

        public void updateQuestionFromForm(xpathsXpath xpathObj, question q, Office.CustomXMLNode node)
        {
            // data type
            xpathObj.type = getDataType();

            // The items, and canSelectMany
            responseFixed.item.Clear();
            injectItems();

            // interview appearance
            q.appearance = getAppearanceType();
            q.appearanceSpecified = true;

            if (string.IsNullOrWhiteSpace(getDefault()))
            {
                node.Text = "«multiple choice»";
            }
            else
            {
                node.Text = getDefault();
            }


        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Mbox.ShowSimpleMsgBoxError("Not implemented yet!");
        }

        private void buttonSetDefault_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null)
            {
                this.textBoxDefault.Text = (string)dataGridView1.CurrentRow.Cells[0].Value;
            }
        }

        public string getDefault()
        {
            return this.textBoxDefault.Text;
        }
    }
}
