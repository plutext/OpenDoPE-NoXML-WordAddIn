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
    public partial class ControlDataTypeDate : UserControl
    {

        static Logger log = LogManager.GetLogger("ControlDataTypeDate");

        public ControlDataTypeDate()
        {
            InitializeComponent();

            this.listBoxLowerOperator.Items.Add(">");
            this.listBoxLowerOperator.Items.Add(">=");
            this.listBoxLowerOperator.Items.Add("n/a");
            this.listBoxLowerOperator.SelectedItem = "n/a"; 

            this.listBoxUpperOperator.Items.Add("<");
            this.listBoxUpperOperator.Items.Add("<=");
            this.listBoxUpperOperator.Items.Add("n/a");
            this.listBoxUpperOperator.SelectedItem = "n/a";

        }

        public void populateControl(xpathsXpath xpathObj, string sampleAnswer, bool isInteger, string hint)
        {
            this.datePickerSample.Text = sampleAnswer;

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
            // Not relevant

            //<xs:attribute name="lower" type="xs:string" />
            if (string.IsNullOrEmpty(xpathObj.lower))
            {
                this.datePickerLower.Value = DateTime.Now;
            }
            else
            {
                this.datePickerLower.Value = formatDate(xpathObj.lower);
            }

            //<xs:attribute name="lowerOperator" type="xs:string" />
            if (string.IsNullOrEmpty(xpathObj.lowerOperator))
            {
                this.listBoxLowerOperator.SelectedItem = "n/a";
            }
            else
            {
                this.listBoxLowerOperator.SelectedItem = xpathObj.lowerOperator;
            }

            //<xs:attribute name="upper" type="xs:string" />
            if (string.IsNullOrEmpty(xpathObj.upper))
            {
                this.datePickerUpper.Value = DateTime.Now;
            }
            else
            {
                this.datePickerUpper.Value = formatDate(xpathObj.upper);
            }

            //<xs:attribute name="upperOperator" type="xs:string" />
            if (string.IsNullOrEmpty(xpathObj.upperOperator))
            {
                this.listBoxUpperOperator.SelectedItem = "n/a";
            }
            else
            {
                this.listBoxUpperOperator.SelectedItem = xpathObj.upperOperator;
            }

            this.textBoxHint.Text = hint;
        }


        public void populateXPath(xpathsXpath xpathObj)
        {
            xpathObj.type = "date";

            //<xs:attribute name="required" type="xs:boolean" />
            xpathObj.requiredSpecified = true;
            xpathObj.required = this.checkBoxRequired.Checked;

            //<xs:attribute name="prepopulate" type="xs:boolean" />
            xpathObj.prepopulateSpecified = true;
            xpathObj.prepopulate = this.checkBoxPopulateForm.Checked;

            //<xs:attribute name="fieldWidth" type="xs:positiveInteger" />

            //<xs:attribute name="lower" type="xs:string" />
            if (!this.listBoxLowerOperator.SelectedItem.Equals("n/a"))
            {
                if (this.datePickerLower.Value!=null)
                {
                    xpathObj.lower = formatDate(this.datePickerLower.Value);
                }
                //<xs:attribute name="lowerOperator" type="xs:string" />
                xpathObj.lowerOperator = (string)this.listBoxLowerOperator.SelectedItem;
            }

            //<xs:attribute name="upper" type="xs:string" />
            if (!this.listBoxUpperOperator.SelectedItem.Equals("n/a"))
            {
                if (this.datePickerUpper.Value != null)
                {
                    xpathObj.upper = formatDate(this.datePickerUpper.Value);
                }

                //<xs:attribute name="upperOperator" type="xs:string" />
                xpathObj.upperOperator = (string)this.listBoxUpperOperator.SelectedItem;
            }
        }

        /// <summary>
        /// Convert 'Friday, 14 December 2012' to yyyy-MM-dd
        /// </summary>
        /// <param name="dateIn"></param>
        /// <returns></returns>
        private string formatDate(DateTime dateIn)
        {
            // Need to store this as date yyyy-mm-dd
            // log.Info("storing: " + thisDate1.ToString("yyyy-MM-dd") );
            return dateIn.ToString("yyyy-MM-dd");
        }

        private DateTime formatDate(string dateIn)
        {
            DateTime thisDate1;
            if (DateTime.TryParse(dateIn, out thisDate1))
            {
                return thisDate1;
            }
            else
            {
                return DateTime.Now;
            }

        }

        public string getSampleAnswer()
        {
            return formatDate(this.datePickerSample.Value);
        }

        //void datePickerUpper_Enter(object sender, System.EventArgs e)
        //{
        //}
        //void datePickerLower_Enter(object sender, System.EventArgs e)
        //{
        //    throw new System.NotImplementedException();
        //}

    }
}
