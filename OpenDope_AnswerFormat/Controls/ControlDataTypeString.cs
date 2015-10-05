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

namespace OpenDope_AnswerFormat.Controls
{
    public partial class ControlDataTypeString : UserControl
    {

        static Logger log = LogManager.GetLogger("ControlDataTypeString");

        public ControlDataTypeString()
        {
            InitializeComponent();
        }

        public void populateControl(xpathsXpath xpathObj, string sampleAnswer, bool isInteger, string hint)
        {
            this.textBoxSampleAnswer.Text = sampleAnswer;

            //<xs:attribute name="required" type="xs:boolean" />
            if (xpathObj.requiredSpecified && xpathObj.required)
            {
                this.checkBoxRequired.Checked = true;
            }
            else
            {
                this.checkBoxRequired.Checked = false;
            }
            //<xs:attribute name="prepopulate" type="xs:boolean" />
            if (xpathObj.prepopulateSpecified && xpathObj.prepopulate)
            {
                this.checkBoxPopulateForm.Checked = true;
            }
            else
            {
                this.checkBoxPopulateForm.Checked = false;
            }
            //<xs:attribute name="fieldWidth" type="xs:positiveInteger" />
            this.textBoxFieldWidth.Text = xpathObj.fieldWidth;

            // Min length for string
            //<xs:attribute name="lower" type="xs:string" />
            this.textBoxLengthMin.Text = xpathObj.lower;

            //<xs:attribute name="lowerOperator" type="xs:string" />
            // Not relevant

            // Max length for string
            //<xs:attribute name="upper" type="xs:string" />
            this.textBoxLengthMax.Text = xpathObj.upper;

            //<xs:attribute name="upperOperator" type="xs:string" />
            // Not relevant

            this.textBoxHint.Text = hint;

        }

        public void populateXPath(xpathsXpath xpathObj)
        {
            xpathObj.type = "string";

            //<xs:attribute name="required" type="xs:boolean" />
            xpathObj.requiredSpecified = true;
            xpathObj.required = this.checkBoxRequired.Checked;

            //<xs:attribute name="prepopulate" type="xs:boolean" />
            xpathObj.prepopulateSpecified = true;
            xpathObj.prepopulate = this.checkBoxPopulateForm.Checked;

            //<xs:attribute name="fieldWidth" type="xs:positiveInteger" />
            int result;
            if (!string.IsNullOrWhiteSpace(this.textBoxFieldWidth.Text)
                && int.TryParse(this.textBoxFieldWidth.Text, out result))
            {
                xpathObj.fieldWidth = this.textBoxFieldWidth.Text;
            }

            //<xs:attribute name="lower" type="xs:string" />
            if (!string.IsNullOrWhiteSpace(this.textBoxLengthMin.Text)
                && int.TryParse(this.textBoxLengthMin.Text, out result))
            {
                xpathObj.lower = this.textBoxLengthMin.Text;
            }

            //<xs:attribute name="lowerOperator" type="xs:string" />

            //<xs:attribute name="upper" type="xs:string" />
            if (!string.IsNullOrWhiteSpace(this.textBoxLengthMax.Text)
                && int.TryParse(this.textBoxLengthMax.Text, out result))
            {
                xpathObj.upper = this.textBoxLengthMax.Text;
            }

            //<xs:attribute name="upperOperator" type="xs:string" />

        }

        public string getSampleAnswer()
        {
            return this.textBoxSampleAnswer.Text;
        }

    }
}
