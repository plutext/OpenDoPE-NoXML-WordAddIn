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
using OpenDoPEModel;

namespace OpenDope_AnswerFormat
{
    public partial class FormRepeatWhichVariables : Form
    {
        public FormRepeatWhichVariables(List<question> questions)
        {
            InitializeComponent();

            foreach (question xx in questions)
            {
                this.checkedListBox1.Items.Add(xx);
            }

            // Set the focus 
            this.ActiveControl = checkedListBox1;

        }

        public List<question> getVars()
        {
            List<question> toMove = new List<question>();
            foreach (object o in checkedListBox1.CheckedItems) {
                toMove.Add( (question)o);
            }
            return toMove;
        }

    }
}
