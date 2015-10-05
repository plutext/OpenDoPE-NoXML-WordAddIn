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
using Microsoft.Office.Tools.Ribbon;

using NLog;

using OpenDoPEModel;
using Word = Microsoft.Office.Interop.Word;

namespace OpenDope_AnswerFormat.Helpers
{
    public class QuestionHelper
    {
        protected static Logger log = LogManager.GetLogger("QuestionHelper");

        protected XPathsPartEntry xppe;
        protected ConditionsPartEntry cpe;

        public QuestionHelper(XPathsPartEntry xppe, ConditionsPartEntry cpe)
        {
            this.xppe = xppe;
            this.cpe = cpe;
        }

        public List<Word.ContentControl> getControlsUsingQuestion(question q)
        {
            string xpathID = xppe.getXPathByQuestionID(q.id).id;
            return getControlsUsingQuestion(q.id, xpathID);
        }

        public List<Word.ContentControl> getControlsUsingQuestion(string questionID, string xpathID)
        {
            List<Word.ContentControl> thisQuestionControls = new List<Word.ContentControl>();
            foreach (Word.ContentControl ccx in Globals.ThisAddIn.Application.ActiveDocument.ContentControls)
            {
                if (ccx.Tag == null)
                {
                    log.Debug("Encountered cc without tag. Skipping");
                }
                else if (ccx.Tag.Contains("od:xpath"))
                {
                    string thisID = (new TagData(ccx.Tag)).getXPathID();
                    if (thisID.Equals(xpathID))
                    {
                        thisQuestionControls.Add(ccx);
                    }
                }
                else if (ccx.Tag.Contains("od:condition"))
                {
                    string thisID = (new TagData(ccx.Tag)).getConditionID();
                    condition c = cpe.getConditionByID(thisID);
                    if (ConditionHelper.doesConditionUseQuestion(xppe, cpe.conditions, c, questionID))
                    {
                        log.Info("condition uses question " + questionID);
                        thisQuestionControls.Add(ccx);
                    }
                }
            }

            return thisQuestionControls;
        }


    }
}
