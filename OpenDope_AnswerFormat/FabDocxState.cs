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
using Word = Microsoft.Office.Interop.Word;

using OpenDoPEModel;

namespace OpenDope_AnswerFormat
{
    class FabDocxState
    {
        public FabDocxState()
        {
        }

        List<string> knownSdtByID = new List<string>();

        public bool inPlutextAdd { get; set; }

        public Model model { get; set; }

        public Microsoft.Office.Tools.CustomTaskPane TaskPane { 
            get; set; }

        public void initTaskPane(Word.Document document)
        {
            TaskPane = Globals.ThisAddIn.CustomTaskPanes.Add(
//                new Controls.LogicTaskPaneUserControl(model, Globals.ThisAddIn.Application.ActiveDocument ), 
                new Controls.LogicTaskPaneUserControl(model, document), 
                "FabDocx Logical Structure");
            TaskPane.Width = 420;

        }

        /// <summary>
        /// List of CCs encountered in this docx
        /// (whether FabDocx ones or not)
        /// </summary>
        /// <param name="NewContentControl"></param>
        public void registerKnownSdt(Word.ContentControl NewContentControl)
        {
            knownSdtByID.Add(NewContentControl.ID);
        }

        public bool isKnownSdt(Word.ContentControl NewContentControl)
        {
            return knownSdtByID.Contains(NewContentControl.ID);
        }

        public List<string> suppressEventsForSdtID { get;  set; }

        public bool areEventsSuppressed(Word.ContentControl cc)
        {
            if (suppressEventsForSdtID == null) return false;
            return suppressEventsForSdtID.Contains(cc.ID);
        }

    }
}
