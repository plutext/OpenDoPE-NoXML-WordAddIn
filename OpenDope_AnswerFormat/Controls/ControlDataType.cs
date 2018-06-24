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

using NLog;

using OpenDoPEModel;
using Office = Microsoft.Office.Core;


namespace OpenDope_AnswerFormat.Controls
{
    public partial class ControlDataType : UserControl
    {
        static Logger log = LogManager.GetLogger("ControlDataType");

        public ControlDataTypeMAIN ControlDataTypeMAIN { get; set; }

        public ControlDataType()
        {
            InitializeComponent();

            this.listBoxDataTypes.Items.Add("text");

            this.listBoxDataTypes.Items.Add("number");

            //this.listBoxDataTypes.Items.Add("decimal number");
            //this.listBoxDataTypes.Items.Add("integer: any ");
            //this.listBoxDataTypes.Items.Add("integer: positive");
            //this.listBoxDataTypes.Items.Add("integer: positive or zero");
            //this.listBoxDataTypes.Items.Add("integer: negative");
            //this.listBoxDataTypes.Items.Add("integer: negative or zero");

            this.listBoxDataTypes.Items.Add("date");

            //this.listBoxDataTypes.Items.Add("duration");
            this.listBoxDataTypes.Items.Add("email");

            this.listBoxDataTypes.Items.Add("card number");

            this.listBoxDataTypes.Items.Add("Word rich text (docx Flat OPC XML)");

            this.listBoxDataTypes.Items.Add("XHTML (non-interactive)");

            // Default
            listBoxDataTypes.SelectedItem = "text";


            listBoxDataTypes.SelectedValueChanged += new EventHandler(listBoxDataTypes_SelectedValueChanged);

            //this.checkBoxRequired.Checked = false;

        }


        void listBoxDataTypes_SelectedValueChanged(object sender, EventArgs e)
        {
            if (listBoxDataTypes.SelectedItem.ToString().Equals("date"))
            {
                this.ControlDataTypeMAIN.controlDataTypeString.Visible = false;
                this.ControlDataTypeMAIN.controlDataTypeFlatOpcXml.Visible = false;
                this.ControlDataTypeMAIN.controlDataTypeXHTML.Visible = false;
                this.ControlDataTypeMAIN.controlDataTypeNumber.Visible = false;
                this.ControlDataTypeMAIN.controlDataTypeDate.Visible = true;
                this.ControlDataTypeMAIN.controlDataTypeCreditCard.Visible = false;
                this.ControlDataTypeMAIN.controlDataTypeEmail.Visible = false;
                //this.textBoxSampleAnswer.Text = DateTime.Now.ToString("yyyy-MM-dd");
                //this.textBoxSampleAnswer.ReadOnly = true;
                // Can't trust them to type something valid here.
                // They can use the date picker if they want another date.

            }
            else if (listBoxDataTypes.SelectedItem.ToString().Equals("text") )
            {
                this.ControlDataTypeMAIN.controlDataTypeString.Visible = true;
                this.ControlDataTypeMAIN.controlDataTypeFlatOpcXml.Visible = false;
                this.ControlDataTypeMAIN.controlDataTypeXHTML.Visible = false;
                this.ControlDataTypeMAIN.controlDataTypeNumber.Visible = false;
                this.ControlDataTypeMAIN.controlDataTypeDate.Visible = false;
                this.ControlDataTypeMAIN.controlDataTypeCreditCard.Visible = false;
                this.ControlDataTypeMAIN.controlDataTypeEmail.Visible = false;
            }
            else if (listBoxDataTypes.SelectedItem.ToString().StartsWith("Word"))
            {
                this.ControlDataTypeMAIN.controlDataTypeString.Visible = false;
                this.ControlDataTypeMAIN.controlDataTypeFlatOpcXml.Visible = true;
                this.ControlDataTypeMAIN.controlDataTypeXHTML.Visible = false;
                this.ControlDataTypeMAIN.controlDataTypeNumber.Visible = false;
                this.ControlDataTypeMAIN.controlDataTypeDate.Visible = false;
                this.ControlDataTypeMAIN.controlDataTypeCreditCard.Visible = false;
                this.ControlDataTypeMAIN.controlDataTypeEmail.Visible = false;
            }
            else if (listBoxDataTypes.SelectedItem.ToString().StartsWith("XHTML"))
            {
                this.ControlDataTypeMAIN.controlDataTypeString.Visible = false;
                this.ControlDataTypeMAIN.controlDataTypeFlatOpcXml.Visible = false;
                this.ControlDataTypeMAIN.controlDataTypeXHTML.Visible = true;
                this.ControlDataTypeMAIN.controlDataTypeNumber.Visible = false;
                this.ControlDataTypeMAIN.controlDataTypeDate.Visible = false;
                this.ControlDataTypeMAIN.controlDataTypeCreditCard.Visible = false;
                this.ControlDataTypeMAIN.controlDataTypeEmail.Visible = false;
            }
            else if (listBoxDataTypes.SelectedItem.ToString().Equals("number"))
            {
                this.ControlDataTypeMAIN.controlDataTypeString.Visible = false;
                this.ControlDataTypeMAIN.controlDataTypeFlatOpcXml.Visible = false;
                this.ControlDataTypeMAIN.controlDataTypeXHTML.Visible = false;
                this.ControlDataTypeMAIN.controlDataTypeNumber.Visible = true;
                this.ControlDataTypeMAIN.controlDataTypeDate.Visible = false;
                this.ControlDataTypeMAIN.controlDataTypeCreditCard.Visible = false;
                this.ControlDataTypeMAIN.controlDataTypeEmail.Visible = false;
            }
            else if (listBoxDataTypes.SelectedItem.ToString().Equals("email"))
            {
                this.ControlDataTypeMAIN.controlDataTypeString.Visible = false;
                this.ControlDataTypeMAIN.controlDataTypeFlatOpcXml.Visible = false;
                this.ControlDataTypeMAIN.controlDataTypeXHTML.Visible = false;
                this.ControlDataTypeMAIN.controlDataTypeNumber.Visible = false;
                this.ControlDataTypeMAIN.controlDataTypeDate.Visible = false;
                this.ControlDataTypeMAIN.controlDataTypeCreditCard.Visible = false;
                this.ControlDataTypeMAIN.controlDataTypeEmail.Visible = true;
            }
            else if (listBoxDataTypes.SelectedItem.ToString().Equals("card number"))
            {
                this.ControlDataTypeMAIN.controlDataTypeString.Visible = false;
                this.ControlDataTypeMAIN.controlDataTypeFlatOpcXml.Visible = false;
                this.ControlDataTypeMAIN.controlDataTypeXHTML.Visible = false;
                this.ControlDataTypeMAIN.controlDataTypeNumber.Visible = false;
                this.ControlDataTypeMAIN.controlDataTypeDate.Visible = false;
                this.ControlDataTypeMAIN.controlDataTypeCreditCard.Visible = true;
                this.ControlDataTypeMAIN.controlDataTypeEmail.Visible = false;
            }
            else
            {
                // Shouldn't happen!!
                log.Error("Unrecognised selection: " + listBoxDataTypes.SelectedItem);
            }
        }

        public bool isFlatOpc()
        {
            return (listBoxDataTypes.SelectedItem.ToString().StartsWith("Word"));
        }
        public bool isXHTML()
        {
            return (listBoxDataTypes.SelectedItem.ToString().StartsWith("XHTML"));
        }

        public void populateControl(xpathsXpath xpathObj, responseFree rF, string sampleAnswer, string hint)
        {
            // responseFree object currently has empty content model, but pass it anyway

            sampleAnswer = this.trimSampleAnswerDelims(sampleAnswer);

            if (xpathObj.type.Equals("string"))
            {
                listBoxDataTypes.SelectedItem = "text";
                // TODO: visible stuff?
                this.ControlDataTypeMAIN.controlDataTypeString.populateControl(xpathObj, sampleAnswer, false, hint);
            }
            else if (xpathObj.type.Equals("FlatOpcXml"))
            {
                listBoxDataTypes.SelectedItem = "Word rich text (docx Flat OPC XML)";
                this.ControlDataTypeMAIN.controlDataTypeFlatOpcXml.populateControl(xpathObj, sampleAnswer, false, hint);

            }
            else if (xpathObj.type.Equals("XHTML"))
            {
                listBoxDataTypes.SelectedItem = "XHTML (non-interactive)";
                this.ControlDataTypeMAIN.controlDataTypeXHTML.populateControl(xpathObj, sampleAnswer, false, hint);

            }
            else if (xpathObj.type.Equals("date"))
            {
                listBoxDataTypes.SelectedItem = "date";
                this.ControlDataTypeMAIN.controlDataTypeDate.populateControl(xpathObj, sampleAnswer, false, hint);

            } else if (xpathObj.type.Equals("email")) {

                listBoxDataTypes.SelectedItem = "email";
                this.ControlDataTypeMAIN.controlDataTypeEmail.populateControl(xpathObj, sampleAnswer, false, hint);
            }
            else if (xpathObj.type.Equals("decimal"))
            {
                listBoxDataTypes.SelectedItem = "number";
                this.ControlDataTypeMAIN.controlDataTypeNumber.populateControl(xpathObj, sampleAnswer, false, hint);
            } 
            else if (xpathObj.type.Equals("integer"))
            {
                listBoxDataTypes.SelectedItem = "number";
                this.ControlDataTypeMAIN.controlDataTypeNumber.populateControl(xpathObj, sampleAnswer, true, hint);
            }
            else if (xpathObj.type.Equals("positiveInteger"))
            {
                listBoxDataTypes.SelectedItem = "number";
                this.ControlDataTypeMAIN.controlDataTypeNumber.populateControl(xpathObj, sampleAnswer, true, hint);
            }
            else if (xpathObj.type.Equals("nonNegativeInteger"))
            {
                listBoxDataTypes.SelectedItem = "number";
                this.ControlDataTypeMAIN.controlDataTypeNumber.populateControl(xpathObj, sampleAnswer, true, hint);
            }
            else if (xpathObj.type.Equals("negativeInteger"))
            {
                listBoxDataTypes.SelectedItem = "number";
                this.ControlDataTypeMAIN.controlDataTypeNumber.populateControl(xpathObj, sampleAnswer, true, hint);
            }
            else if (xpathObj.type.Equals("nonPositiveInteger"))
            {
                listBoxDataTypes.SelectedItem = "number";
                this.ControlDataTypeMAIN.controlDataTypeNumber.populateControl(xpathObj, sampleAnswer, true, hint);
            }
            else if (xpathObj.type.Equals("card-number"))
            {
                listBoxDataTypes.SelectedItem = "card number";
                this.ControlDataTypeMAIN.controlDataTypeCreditCard.populateControl(xpathObj, sampleAnswer, false, hint);
            }
            else
            {
                // TODO add their custom type
                log.Error("XPath " + xpathObj.id + " has unknown value for datatype: '" + xpathObj.type);
            }

            //this.textBoxSampleAnswer.Text = trimSampleAnswerDelims(sampleAnswer);

            //this.checkBoxRequired.Checked = xpathObj.required;

        }

        public void populateXPath(xpathsXpath xpathObj)
        {
            log.Info("in pX " + listBoxDataTypes.SelectedItem);

            if (listBoxDataTypes.SelectedItem.ToString().Equals("date"))
            {
                this.ControlDataTypeMAIN.controlDataTypeDate.populateXPath(xpathObj);
            }
            else if (listBoxDataTypes.SelectedItem.ToString().Equals("text"))
            {
                this.ControlDataTypeMAIN.controlDataTypeString.populateXPath(xpathObj);
            }
            else if (listBoxDataTypes.SelectedItem.ToString().StartsWith("Word"))
            {
                this.ControlDataTypeMAIN.controlDataTypeFlatOpcXml.populateXPath(xpathObj);
            }
            else if (listBoxDataTypes.SelectedItem.ToString().StartsWith("XHTML"))
            {
                this.ControlDataTypeMAIN.controlDataTypeXHTML.populateXPath(xpathObj);
            }
            else if (listBoxDataTypes.SelectedItem.ToString().Equals("number"))
            {
                this.ControlDataTypeMAIN.controlDataTypeNumber.populateXPath(xpathObj);
            }
            else if (listBoxDataTypes.SelectedItem.ToString().Equals("email"))
            {
                this.ControlDataTypeMAIN.controlDataTypeEmail.populateXPath(xpathObj);
            }
            else if (listBoxDataTypes.SelectedItem.ToString().Equals("card number"))
            {
                this.ControlDataTypeMAIN.controlDataTypeCreditCard.populateXPath(xpathObj);
            }
            else
            {
                // Shouldn't happen!!
                log.Error("Unrecognised selection: " + listBoxDataTypes.SelectedItem);
            }
        }


        public bool isValid()
        { 
            // TODO
            return true;
        }


        public void updateQuestionFromForm(xpathsXpath xpathObj, question q, Office.CustomXMLNode node)
        {
            // Currently used only by QuestionEdit. Why not also initial populate?

            // sampleAnswer
            node.Text = getSampleAnswerProcessed();

            //if (!string.IsNullOrWhiteSpace(getHint()))
            //{
                q.hint = getHint();
            //}

            populateXPath(xpathObj);
        }

        public string getHint()
        {

            if (listBoxDataTypes.SelectedItem.ToString().Equals("date"))
            {
                return this.ControlDataTypeMAIN.controlDataTypeDate.textBoxHint.Text;
            }
            else if (listBoxDataTypes.SelectedItem.ToString().Equals("text"))
            {
                return this.ControlDataTypeMAIN.controlDataTypeString.textBoxHint.Text;
            }
            else if (listBoxDataTypes.SelectedItem.ToString().StartsWith("Word"))
            {
                return null;
                //return this.ControlDataTypeMAIN.controlDataTypeFlatOpcXml.textBoxHint.Text;
            }
            else if (listBoxDataTypes.SelectedItem.ToString().StartsWith("XHTML"))
            {
                return null;
                //return this.ControlDataTypeMAIN.controlDataTypeXHTML.textBoxHint.Text;
            }
            else if (listBoxDataTypes.SelectedItem.ToString().Equals("number"))
            {
                return this.ControlDataTypeMAIN.controlDataTypeNumber.textBoxHint.Text;
            }
            else if (listBoxDataTypes.SelectedItem.ToString().Equals("email"))
            {
                return this.ControlDataTypeMAIN.controlDataTypeEmail.textBoxHint.Text;
            }
            else if (listBoxDataTypes.SelectedItem.ToString().Equals("card number"))
            {
                return this.ControlDataTypeMAIN.controlDataTypeCreditCard.textBoxHint.Text;
            }
            else
            {
                // Shouldn't happen!!
                log.Error("Unrecognised selection: " + listBoxDataTypes.SelectedItem);
                return null;
            }

        }

        public bool getRequired()
        {

            if (listBoxDataTypes.SelectedItem.ToString().Equals("date"))
            {
                // should this always be true??
                return this.ControlDataTypeMAIN.controlDataTypeDate.checkBoxRequired.Checked;
            }
            else if (listBoxDataTypes.SelectedItem.ToString().Equals("text"))
            {
                return this.ControlDataTypeMAIN.controlDataTypeString.checkBoxRequired.Checked;
            }
            else if (listBoxDataTypes.SelectedItem.ToString().StartsWith("Word"))
            {
                return false;
            }
            else if (listBoxDataTypes.SelectedItem.ToString().StartsWith("XHTML"))
            {
                return false;
            }
            else if (listBoxDataTypes.SelectedItem.ToString().Equals("number"))
            {
                return this.ControlDataTypeMAIN.controlDataTypeNumber.checkBoxRequired.Checked;
            }
            else if (listBoxDataTypes.SelectedItem.ToString().Equals("email"))
            {
                return this.ControlDataTypeMAIN.controlDataTypeEmail.checkBoxRequired.Checked;
            }
            else if (listBoxDataTypes.SelectedItem.ToString().Equals("card number"))
            {
                return this.ControlDataTypeMAIN.controlDataTypeCreditCard.checkBoxRequired.Checked;
            }
            else
            {
                // Shouldn't happen!!
                log.Error("Unrecognised selection: " + listBoxDataTypes.SelectedItem);
                return false;
            }

        }

        public string getSampleAnswerProcessed()
        {
            string sampleAnswer = getSampleAnswer();
            if (string.IsNullOrWhiteSpace(sampleAnswer))
            {
                sampleAnswer = "«" + listBoxDataTypes.SelectedItem.ToString() + " answer»";
            }
            else if (listBoxDataTypes.SelectedItem.ToString().Equals("date"))
            {
                // don't add the chevrons!
            }
            else
            {
                sampleAnswer = "«" + sampleAnswer + "»";
                // Unicode Character 'LEFT-POINTING DOUBLE ANGLE QUOTATION MARK' (U+00AB)
            }
            return sampleAnswer.Trim();
        }

        private string getSampleAnswer()
        {
            if (listBoxDataTypes.SelectedItem.ToString().Equals("date"))
            {
                // should this always be true??
                return this.ControlDataTypeMAIN.controlDataTypeDate.getSampleAnswer();
            }
            else if (listBoxDataTypes.SelectedItem.ToString().Equals("text"))
            {
                return this.ControlDataTypeMAIN.controlDataTypeString.getSampleAnswer();
            }
            else if (listBoxDataTypes.SelectedItem.ToString().StartsWith("Word"))
            {
                return this.ControlDataTypeMAIN.controlDataTypeFlatOpcXml.getSampleAnswer();
            }
            else if (listBoxDataTypes.SelectedItem.ToString().StartsWith("XHTML"))
            {
                return this.ControlDataTypeMAIN.controlDataTypeXHTML.getSampleAnswer();
            }
            else if (listBoxDataTypes.SelectedItem.ToString().Equals("number"))
            {
                return this.ControlDataTypeMAIN.controlDataTypeNumber.getSampleAnswer();
            }
            else if (listBoxDataTypes.SelectedItem.ToString().Equals("email"))
            {
                return this.ControlDataTypeMAIN.controlDataTypeEmail.getSampleAnswer();
            }
            else if (listBoxDataTypes.SelectedItem.ToString().Equals("card number"))
            {
                return this.ControlDataTypeMAIN.controlDataTypeCreditCard.getSampleAnswer();
            }
            else
            {
                // Shouldn't happen!!
                log.Error("Unrecognised selection: " + listBoxDataTypes.SelectedItem);
                return null;
            }

        }

        /// <summary>
        /// Is the sample answer to be injected into the XForm?
        /// </summary>
        /// <returns></returns>
        public bool getPrepopulate()
        {

            if (listBoxDataTypes.SelectedItem.ToString().Equals("date"))
            {
                // should this always be true??
                return this.ControlDataTypeMAIN.controlDataTypeDate.checkBoxPopulateForm.Checked;
            }
            else if (listBoxDataTypes.SelectedItem.ToString().Equals("text"))
            {
                return this.ControlDataTypeMAIN.controlDataTypeString.checkBoxPopulateForm.Checked;
            }
            else if (listBoxDataTypes.SelectedItem.ToString().StartsWith("Word"))
            {
                return false; 
            }
            else if (listBoxDataTypes.SelectedItem.ToString().StartsWith("XHTML"))
            {
                return false;
            }
            else if (listBoxDataTypes.SelectedItem.ToString().Equals("number"))
            {
                return this.ControlDataTypeMAIN.controlDataTypeNumber.checkBoxPopulateForm.Checked;
            }
            else if (listBoxDataTypes.SelectedItem.ToString().Equals("email"))
            {
                return this.ControlDataTypeMAIN.controlDataTypeEmail.checkBoxPopulateForm.Checked;
            }
            else if (listBoxDataTypes.SelectedItem.ToString().Equals("card number"))
            {
                return this.ControlDataTypeMAIN.controlDataTypeCreditCard.checkBoxPopulateForm.Checked;
            }
            else
            {
                // Shouldn't happen!!
                log.Error("Unrecognised selection: " + listBoxDataTypes.SelectedItem);
                return false;
            }

        }

        //public string getSampleAnswer()
        //{
        //    return this.textBoxSampleAnswer.Text;
        //}

        public string trimSampleAnswerDelims(string tmp)
        {
            if (tmp.StartsWith("«"))
            {
                tmp = tmp.Substring(1);
            }
            if (tmp.EndsWith("»"))
            {
                tmp = tmp.Substring(0, tmp.Length - 1);
            }
            return tmp;
        }

        private void buttonCustom_Click(object sender, EventArgs e)
        {

        }

    }
}
