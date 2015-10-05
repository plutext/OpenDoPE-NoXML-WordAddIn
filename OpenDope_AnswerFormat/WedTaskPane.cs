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

using Word = Microsoft.Office.Interop.Word;
using Office = Microsoft.Office.Core;
//using DocumentFormat.OpenXml.Wordprocessing;


namespace OpenDope_AnswerFormat
{
    /// <summary>
    /// This is the user control for my custom task pane.
    /// 
    /// The idea is that it will show 4 different views:
    /// 
    /// 1. not in a content control (tell them their not, point to dev tab to add)
    /// 2. in a virgin control
    /// 3. in a bind or a repeat
    /// 4. in a condition
    /// 
    /// We have a separate user control for each of these.
    /// 
    /// This contents will be clear() and the relevant control added
    /// as the user moves around the document.
    /// 
    /// </summary>
    public class WedTaskPane
    {
        static Logger log = LogManager.GetLogger("OpenDoPE_Wed");

        public Word.ContentControl currentCC { get; set; }

        public Microsoft.Office.Tools.CustomTaskPane ctp { get; set; }

        /// <summary>
        /// The document being edited.  TODO: use this more
        /// (ie instead of active document)
        /// </summary>
        public Word.Document associatedDocument { get; set; }

        public bool questions { get; set; }

        //OpenDoPE_Wed.XP xp;
        public List<Office.CustomXMLPart> cxp { get; set; }
        public Office.CustomXMLPart activeCxp { get; set; }


        public Office.CustomXMLPart xpathsPart { get; set; }
        public Office.CustomXMLPart conditionsPart { get; set; }
        public Office.CustomXMLPart questionsPart { get; set; }
        public Office.CustomXMLPart componentsPart { get; set; }

        public Office.CustomXMLPart partBeingEdited; // could be cxp or conditions

        public WedTaskPane(
            List<Office.CustomXMLPart> cxp,
            Office.CustomXMLPart xpathsPart,
            Office.CustomXMLPart conditionsPart,
            Office.CustomXMLPart questionsPart,
            Office.CustomXMLPart componentsPart
            )
        {


            if (questionsPart == null)
            {
                questions = false;
            }
            else
            {
                questions = true;
            }

            // TODO - remove this
            //XmlEditorControl af = new XmlEditorControl(cxp[0], ".xml", true);
            //af.Size = new System.Drawing.Size(350, 600);
            //this.Controls.Add(af);

            this.cxp = cxp;
            activeCxp = cxp[0];
            this.xpathsPart = xpathsPart;
            this.conditionsPart = conditionsPart;
            this.questionsPart = questionsPart;
            this.componentsPart = componentsPart;

            //Office.CustomXMLNode node = cxp.SelectSingleNode("/node()");
            //xp = new OpenDoPE_Wed.XP(node.XML);

            partBeingEdited = activeCxp;

            // setupCcEvents(Globals.ThisAddIn.Application.ActiveDocument);

            //timer = new System.Threading.Timer(timerCallback, this, SEC_1, SEC_1);


        }


    }
}
