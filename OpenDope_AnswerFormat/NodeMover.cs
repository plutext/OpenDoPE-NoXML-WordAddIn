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
using Microsoft.Office.Tools.Word.Extensions; // for VSTOObject

namespace OpenDope_AnswerFormat
{
    /// <summary>
    /// Utility class to move an answer or repeat to a new
    /// spot in the answer file, and change XPaths part to suit.
    /// </summary>
    class NodeMover
    {
        static Logger log = LogManager.GetLogger("OpenDoPE_Wed");

        private FabDocxState fabDocxState;

        public NodeMover()
        {
            fabDocxState = (FabDocxState)Globals.ThisAddIn.Application.ActiveDocument.GetVstoObject(Globals.Factory).Tag; 

        }

        /// <summary>
        /// Must use NodeToXPath.getXPath form of XPath expression
        /// </summary>
        /// <param name="fromXPath"></param>
        /// <param name="toXPath"></param>
        public void Move(string fromXPath, string destParentXPath)
        {
            Model model = fabDocxState.model;
            Office.CustomXMLPart answersPart = model.answersPart; //.userParts[0]; // TODO: make this better

            // Destination for move
            Office.CustomXMLNode destination = answersPart.SelectSingleNode(destParentXPath);
            if (destination == null)
            {
                throw new Exception("Answers part doesn't contain destParent " + destParentXPath);
            }

            Office.CustomXMLNode node = answersPart.SelectSingleNode(fromXPath);
            if (node == null)
            {
                throw new Exception("Answers part doesn't contain source node " + fromXPath);
            }

            // Move it
            String nodeXML = node.XML; // No API to add a node!
            node.ParentNode.RemoveChild(node);
            destination.AppendChildSubtree(nodeXML);

            // So we'll have to change its xpath in XPaths part
            // eg from:
            //   "/answers/answer[@id='qa_2']"
            // to:
            //   "/answers/repeat[@qref='rpt1"]/row[1]/answer[@id='qa_2']"
            //
            // CustomXMLNode's Xpath produces something like: /ns2:answers[1]/ns2:answer[1]
            // which we don't want

            string toXPath = NodeToXPath.getXPath(destination.LastChild);

            // Now do the substitutions in the XPaths part - for all
            string xpaths = model.xpathsPart.XML;
            xpaths = xpaths.Replace(fromXPath, toXPath);
            CustomXmlUtilities.replaceXmlDoc(model.xpathsPart, xpaths);

            log.Info(model.xpathsPart.XML);
            log.Info(answersPart.XML);

        }

        public void adjustBinding(List<Word.ContentControl> thisQuestionControls,
            string destParentXPath, string qid)
        {
            foreach (Word.ContentControl ccx in thisQuestionControls)
            {
                if (ccx.Tag == null)
                {
                    log.Debug("Encountered cc without tag. Skipping");
                }
                else if (ccx.Tag.Contains("od:xpath"))
                {
                    if (ccx.XMLMapping.IsMapped)
                    {
                        if (destParentXPath.Equals("/oda:answers"))
                        {
                            ccx.XMLMapping.SetMapping("/oda:answers/oda:answer[@id='" + qid + "']",
                                ccx.XMLMapping.PrefixMappings, ccx.XMLMapping.CustomXMLPart);
                        }
                        else
                        {
                            ccx.XMLMapping.SetMapping(destParentXPath + "/oda:answer[@id='" + qid + "']",
                                ccx.XMLMapping.PrefixMappings, ccx.XMLMapping.CustomXMLPart);
                            log.Info("Mapping set to " + ccx.XMLMapping.XPath);
                        }
                    }
                }
                else if (ccx.Tag.Contains("od:condition"))
                {
                    // Nothing to do
                }

            }            

        }
    }
}
