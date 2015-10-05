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
    class QuestionListHelperForConditionsForm : QuestionListHelper
    {

        public System.Windows.Forms.ComboBox comboBoxValues { get; set; }
        public System.Windows.Forms.ListBox listBoxPredicate { get; set; }

        public QuestionListHelperForConditionsForm(Model model, XPathsPartEntry xppe, questionnaire questionnaire,
                    Word.ContentControl cc) : base( model,  xppe,  questionnaire, cc)
        {
        }

        override public void populateValues(responseFixed responses, string matchResponse)
        {
            this.comboBoxValues.Items.Clear();

            foreach (responseFixedItem item in responses.item)
            {
                int currentIndex = this.comboBoxValues.Items.Add(item);
                if (matchResponse != null && matchResponse.Contains(item.value))
                {
                    // Simple minded matching, will do for now.
                    // Saves us having an XPath parser.
                    this.comboBoxValues.SelectedIndex = currentIndex;
                }
            }

        }


        override public void clearComboBoxValues()
        {
            this.comboBoxValues.Items.Clear();
        }



        override public void populatePredicates(question q)
        {
            string type = null;
            if (listBoxTypeFilter.SelectedItem != null)
            {
                type = listBoxTypeFilter.SelectedItem.ToString();
            }

            if (q != null) // can happen if there are no questions of this type
            {
                xpathsXpath xpath = xppe.getXPathByQuestionID(q.id);
                if (xpath.dataBinding.xpath.EndsWith("oda:row")
                    && type == null)
                {
                    type = REPEAT_POS; // default
                }
                else
                {
                    type = xpath.type;
                }
            }

            this.listBoxPredicate.Items.Clear();

            if (type == null)
            {
            }
            else if (type.Equals(REPEAT_POS))
            {
                this.listBoxPredicate.Items.Add("first");
                this.listBoxPredicate.Items.Add("not first");
                this.listBoxPredicate.Items.Add("second");
                this.listBoxPredicate.Items.Add("second last");
                this.listBoxPredicate.Items.Add("last");
                this.listBoxPredicate.Items.Add("not last");

                listBoxTypeFilter.SelectedItem = REPEAT_POS;
            }
            else if (type.Equals("string"))
            {
                this.listBoxPredicate.Items.Add("equals");
                this.listBoxPredicate.Items.Add("is not");
                this.listBoxPredicate.Items.Add("starts-with");
                this.listBoxPredicate.Items.Add("contains");
                this.listBoxPredicate.Items.Add("not blank.");
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
              || type.Equals(REPEAT_COUNT)
              )
            {
                this.listBoxPredicate.Items.Add("=");
                this.listBoxPredicate.Items.Add(">");
                this.listBoxPredicate.Items.Add(">=");
                this.listBoxPredicate.Items.Add("<");
                this.listBoxPredicate.Items.Add("<=");

            }
            else if (type.Equals("date"))
            {
                this.listBoxPredicate.Items.Add("equals");
                this.listBoxPredicate.Items.Add("is before");
                this.listBoxPredicate.Items.Add("is after");
            }
            // TODO: flesh this out with the full range of allowable datatypes 
            // (card number, email address, custom types)
        }

    }
}
