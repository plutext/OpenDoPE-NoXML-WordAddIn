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
    public partial class ControlDataTypeXHTML : UserControl
    {

        static Logger log = LogManager.GetLogger("ControlDataTypeXHTML");

        public ControlDataTypeXHTML()
        {
            InitializeComponent();
        }

        public void populateControl(xpathsXpath xpathObj, string sampleAnswer, bool isInteger, string hint)
        {
            this.textBoxSampleAnswer.Text = sampleAnswer;

            this.textBoxHint.Text = hint;

        }

        public void populateXPath(xpathsXpath xpathObj)
        {
            xpathObj.type = "XHTML";

            //<xs:attribute name="prepopulate" type="xs:boolean" />
            xpathObj.prepopulateSpecified = false;
            xpathObj.prepopulate = false;

        }

        public string getSampleAnswer()
        {
            return this.textBoxSampleAnswer.Text;
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }
    }
}
