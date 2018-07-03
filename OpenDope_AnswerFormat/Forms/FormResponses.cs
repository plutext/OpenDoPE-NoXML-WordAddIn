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

using OpenDoPEModel;

namespace OpenDope_AnswerFormat
{
    public partial class FormResponses : Form
    {

        public FormResponses(responseFixed responseFixed)
        {
            InitializeComponent();
            this.controlQuestionResponsesFixed1.init(responseFixed);
            this.controlQuestionResponsesFixed1.radioTypeBoolean.CheckedChanged += new System.EventHandler(this.radioTypeBoolean_CheckedChanged);

        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (!controlQuestionResponsesFixed1.isValid())
            {
                DialogResult = DialogResult.None;
                return;
            }

            controlQuestionResponsesFixed1.injectItems();

            this.Close();

        }


        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Mbox.ShowSimpleMsgBoxError("Not implemented yet!");
        }

        private void radioTypeBoolean_CheckedChanged(object sender, EventArgs e)
        {
            if (this.controlQuestionResponsesFixed1.radioTypeBoolean.Checked)
            {
                this.checkBoxContentControl.Enabled = true;
            } else
            {
                this.checkBoxContentControl.Enabled = false;
            }
        }

        private void checkBoxContentControl_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBoxContentControl.Checked)
            {
                // if they want to insert a checkbox control,
                // then of course they want to insert a control!
                this.checkBoxInsertControl.Checked = true;
            }
            // This UI will need to be rethought if/when we 
            // support adding ComboBox, Dropdownlist content controls.

        }
    }
}
