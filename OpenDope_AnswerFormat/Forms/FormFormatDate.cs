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

using Word = Microsoft.Office.Interop.Word;

namespace OpenDope_AnswerFormat.Forms
{
    public partial class FormFormatDate : Form
    {
        string[] formatValues = { "d/MM/yyyy", "dddd, d MMMM yyyy", 
                              "d MMMM yyyy", "d/MM/yy", "yyyy-MM-dd", 
                              "d-MMM-yy", "d.MM.yyyy", "d MMM. yy", 
                              "MMMM yy", "MMM-yy", "dd", "MMMM", "yyyy"
                              , " 'day of' MMMM, yyyy" };

        Word.ContentControl currentCC = null;
        DateTime thisDate1;

        public FormFormatDate(Word.ContentControl currentCC)
        {
            InitializeComponent();

            this.currentCC = currentCC;

            if (currentCC.XMLMapping.IsMapped)
            {
                String dateValue = currentCC.XMLMapping.CustomXMLNode.Text;
                if (!DateTime.TryParse(dateValue, out thisDate1)) {
                    thisDate1 = DateTime.Now;
                    }
            }
            else
            {
                thisDate1 = DateTime.Now;
            }

            for (int i = 0; i < formatValues.Length; i++)
            {
                FormatChoice fc = new FormatChoice();
                fc.format = formatValues[i];
                fc.display = thisDate1.ToString(fc.format);
                this.listBoxFormat.Items.Add(fc);
            }

            if (currentCC.DateDisplayFormat != null)
            {
                this.textBoxFormat.Text = currentCC.DateDisplayFormat;
            }

        }

        void listBoxFormat_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            this.textBoxFormat.Text = ((FormatChoice)this.listBoxFormat.SelectedItem).format;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            // is it ok?
            if (string.IsNullOrWhiteSpace(this.textBoxFormat.Text) ) {
                MessageBox.Show("You must choose a format.");
                DialogResult = DialogResult.None;
                return;
             }
            
            String result = thisDate1.ToString(this.textBoxFormat.Text);
            if (string.IsNullOrWhiteSpace(result) ) {
                MessageBox.Show("Broken format string?");
                DialogResult = DialogResult.None;
                return;
             }

            currentCC.DateDisplayFormat = this.textBoxFormat.Text;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            // do nothing
        }
    }



    public class FormatChoice
    {
        public string display { get; set; }

        public string format { get; set; }

        public override string ToString()
        {
            return display;
        }

    }
}
