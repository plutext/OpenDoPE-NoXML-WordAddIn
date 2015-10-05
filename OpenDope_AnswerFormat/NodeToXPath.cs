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
using Office = Microsoft.Office.Core;
using OpenDoPEModel;


namespace OpenDope_AnswerFormat
{
    class NodeToXPath
    {
        /// <summary>
        /// Recursively construct the node's XPath, something like 
        ///   /answers/repeat[@qref='rpt1']/row[1]/answer[@id='qa_1']
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static string getXPath(Office.CustomXMLNode node)
        {
            if (node.BaseName.Equals("answers"))
            {
                return "/oda:answers";
            }
            else if (node.BaseName.Equals("answer"))
            {
                return getXPath(node.ParentNode) + "/oda:answer[@id='"
                    + CustomXMLNodeHelper.getAttribute(node, "id") + "']";
            }
            else if (node.BaseName.Equals("row"))
            {
                return getXPath(node.ParentNode) + "/oda:row[1]";
            }
            else if (node.BaseName.Equals("repeat"))
            {
                return getXPath(node.ParentNode) + "/oda:repeat[@qref='"
                    + CustomXMLNodeHelper.getAttribute(node, "qref") + "']";
            }
            else if (node.NamespaceURI.Equals("http://opendope.org/answers"))
            {
                return getXPath(node.ParentNode) + "/oda:" + node.BaseName + "[1]";
            }
            else 
            {
                return getXPath(node.ParentNode) + "/" + node.BaseName + "[1]";
            }
        }

    }
}
