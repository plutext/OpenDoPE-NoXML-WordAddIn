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
//using System.ComponentModel;
//using System.Data;
//using System.Drawing;
//using System.Linq;
using System.Text;
//using System.Windows.Forms;

using Office = Microsoft.Office.Core;
using Word = Microsoft.Office.Interop.Word;

using NLog;
using OpenDoPEModel;

namespace OpenDope_AnswerFormat
{
    class PartCreator
    {
        static Logger log = LogManager.GetLogger("OpenDoPE_Wed");

        List<Office.CustomXMLPart> cxp = new List<Office.CustomXMLPart>();


        /// <summary>
        /// Create OpenDoPE parts, including optionally, question part.
        /// </summary>
        /// <param name="needQuestionsPart"></param>
        public void process(bool needQuestionsPart)
        {
            Microsoft.Office.Interop.Word.Document document = null;
            try
            {
                document = Globals.ThisAddIn.Application.ActiveDocument;
            }
            catch (Exception)
            {
                Mbox.ShowSimpleMsgBoxError("No document is open/active. Create or open a docx first.");
                return;
            }
            process(document, needQuestionsPart);
        }

        /// <summary>
        /// Create OpenDoPE parts, including optionally, question part.
        /// </summary>
        /// <param name="needQuestionsPart"></param>
        public void process(Microsoft.Office.Interop.Word.Document document, bool needQuestionsPart)
        {


            Model model = Model.ModelFactory(document);

            // Button shouldn't be available if this exists,
            // but ..
            if (model.conditionsPart == null)
            {
                conditions conditions = new conditions();
                string conditionsXml = conditions.Serialize();
                model.conditionsPart = addCustomXmlPart(document, conditionsXml);
            }

            if (model.componentsPart == null)
            {
                components components = new components();
                string componentsXml = components.Serialize();
                model.componentsPart = addCustomXmlPart(document, componentsXml);
            }

            // TODO 2013 12 if there is an existing questions part or answers part
            // I suspect we just want to keep it, rather than overwriting its contents!
            // Unless it is to do with supporting multiple Q, A parts?

            questionnaire q = null;
            if (model.questionsPart == null)
            {
                if (needQuestionsPart)
                {
                    q = new questionnaire();
                }
            }
            else
            {
                //needQuestionsPart = false;

                // Button shouldn't be available if this exists,
                // but ..
                q = new questionnaire();
                questionnaire.Deserialize(model.questionsPart.XML, out q);
            }

            // Add XPath
            xpaths xpaths = new xpaths();
            // Button shouldn't be available if this exists,
            // but ..
            if (model.xpathsPart != null)
            {
                xpaths.Deserialize(model.xpathsPart.XML, out xpaths);
            }

            if (needQuestionsPart)
            {
                // Are there content controls which need questions
                bool missingQuestions = false;
                foreach (Word.ContentControl cc in Globals.ThisAddIn.Application.ActiveDocument.ContentControls)
                {
                    if (cc.XMLMapping.IsMapped)
                    {
                        missingQuestions = true;
                        break;
                    }
                }
                if (missingQuestions)
                {
                    log.Warn("Document contains pre-existing bound content controls; without Questions.");
                    System.Windows.Forms.MessageBox.Show("This docx already contains bound content controls!");
                }
            }

            string xpathsXml = xpaths.Serialize();
            if (model.xpathsPart == null)
            {
                model.xpathsPart = addCustomXmlPart(document, xpathsXml);
            }
            else
            {
                CustomXmlUtilities.replaceXmlDoc(model.xpathsPart, xpathsXml);
            }


            if (model.questionsPart == null && needQuestionsPart)
            {
                string qxml = q.Serialize();
                model.questionsPart = addCustomXmlPart(document, qxml);
            }

        }


        //private String getCandidatePartsNames(List<Office.CustomXMLPart> cxp)
        //{
        //    StringBuilder sb = new StringBuilder();

        //    bool first = true;
        //    foreach (Office.CustomXMLPart cp in cxp)
        //    {
        //        Office.CustomXMLNode node = cp.SelectSingleNode("/node()");
        //        if (first)
        //        {
        //            sb.Append(node.BaseName);
        //            first = false;
        //        }
        //        else
        //        {
        //            sb.Append(", " + node.BaseName);
        //        }
        //    }

        //    return sb.ToString();
        //}

        Office.CustomXMLPart addCustomXmlPart(Word.Document document, string xml)
        {
            object missing = System.Reflection.Missing.Value;

            Office.CustomXMLPart cxp = document.CustomXMLParts.Add(xml, missing);

            log.Debug("part added");

            //bool result = cxp.LoadXML("<mynewpart><blagh/></mynewpart>");
            /* 
            * Can't do this .. causes System.Runtime.InteropServices.COMException
            * "This custom XML part has already been loaded"
            * 
            * Why?  What is the method for if it can't be used?
            * 
            * So our options are:
            * 
            * 1. replace from root node
            * 2. Delete the part, and re-add
            * 
            * Will Word remove the bindings if we do this?
            * 
            */

            //replaceXmlDoc(cxp, "<mynewpart><blagh/></mynewpart>");

            log.Debug("done");

            return cxp;
        }

        public Office.CustomXMLPart createAnswersPart(Microsoft.Office.Interop.Word.Document document)
        {
            // Particular to this Add-In
            // The XML part is hard wired to our answers format.
            answers answers = new answers();
            string answersXml = answers.Serialize();
            object missing = System.Reflection.Missing.Value;

            Office.CustomXMLPart answerPart = document.CustomXMLParts.Add(answersXml, missing);
            answerPart.NamespaceManager.AddNamespace("oda", "http://opendope.org/answers");

            return answerPart;
        }

    }
}
