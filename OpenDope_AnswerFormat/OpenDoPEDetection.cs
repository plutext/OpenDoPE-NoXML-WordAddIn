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
using Office = Microsoft.Office.Core;
using Microsoft.Office.Tools.Word;
using Microsoft.Office.Tools.Word.Extensions; // for VSTOObject
using NLog;

namespace OpenDope_AnswerFormat
{
    class OpenDoPEDetection
    {
        static Logger log = LogManager.GetLogger("OpenDoPE_Wed");

        public static bool configIfPresent(Word.Document document)
        {

            log.Debug("Looking for OpenDoPE parts...");

            // Does this document contains the custom xml parts?
            // If so, add the task pane.
            Office.CustomXMLPart xpathsPart = null;
            Office.CustomXMLPart conditionsPart = null;
            Office.CustomXMLPart questionsPart = null;
            Office.CustomXMLPart componentsPart = null;
            List<Office.CustomXMLPart> cxp = new List<Office.CustomXMLPart>();

            foreach (Office.CustomXMLPart cp in document.CustomXMLParts)
            {
                log.Info("cxp: " + cp.DocumentElement + ", " + cp.NamespaceURI
                    + ", " + cp.Id);

                if (cp.NamespaceURI.Equals(Namespaces.XPATHS))
                {
                    xpathsPart = cp;
                }
                else if (cp.NamespaceURI.Equals(Namespaces.CONDITIONS))
                {
                    conditionsPart = cp;
                }
                else if (cp.NamespaceURI.Equals(Namespaces.COMPONENTS))
                {
                    componentsPart = cp;
                }
                else if (cp.NamespaceURI.Equals(Namespaces.QUESTIONS))
                {
                    questionsPart = cp;
                }
                else if (cp.BuiltIn)
                {
                    log.Info("--> built-in");
                }
                else
                {
                    log.Info("--> cxp");
                    cxp.Add(cp);
                }
            }

            return (xpathsPart != null && conditionsPart != null && questionsPart != null);

        }

    }
}
