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
    public partial class ControlQuestionCommon : UserControl
    {
        public ControlQuestionCommon()
        {
            InitializeComponent();
        }

        public bool isValid()
        {
            return !string.IsNullOrWhiteSpace(this.textBoxQuestionText.Text);
        }

        public void populateQuestion(question q ) {

            q.text = this.textBoxQuestionText.Text;

            if (!string.IsNullOrWhiteSpace(this.textBoxHelp.Text))
            {
                q.help = this.textBoxHelp.Text;
            }
        }

        public void populateControl(question q)
        {
            this.textBoxQuestionText.Text = q.text;

            if (q.help!=null)
            {
                 this.textBoxHelp.Text = q.help;
            }

        }
    }
}
