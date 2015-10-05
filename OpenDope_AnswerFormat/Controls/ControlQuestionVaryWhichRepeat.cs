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
using OpenDope_AnswerFormat.Helpers;

using Office = Microsoft.Office.Core;
using OpenDoPEModel;
using Word = Microsoft.Office.Interop.Word;
using Microsoft.Office.Tools.Word.Extensions; // for VSTOObject

namespace OpenDope_AnswerFormat.Controls
{
    public partial class ControlQuestionVaryWhichRepeat : UserControl
    {
        static Logger log = LogManager.GetLogger("OpenDoPE_Wed");

        TreeNode root = null;

        public ControlQuestionVaryWhichRepeat()
        {
            InitializeComponent();
        }

        /// <summary>
        /// When question is first being added, it is being added to a particular CC,
        /// so we only need to consider that CC's ancestors.
        /// 
        /// When a question is being edited, it could be in several content controls,
        /// so that case is handled by the more generic method.
        /// </summary>
        /// <param name="cc"></param>
        /// <param name="questionnaire"></param>
        /// <param name="xppe"></param>
        public void init(Word.ContentControl cc, questionnaire questionnaire, XPathsPartEntry xppe)
        {
            root = new TreeNode("Ask only once");

            Word.ContentControl currentCC = cc.ParentContentControl;

            TreeNode thisNode = null;
            TreeNode previousNode = null;
            treeViewRepeat.HideSelection = false; // keep selection when focus is lost

            while (currentCC != null)
            {
                if (currentCC.Tag.Contains("od:repeat"))
                {
                    TagData td = new TagData(currentCC.Tag);
                    string ancestorRepeatXPathID = td.getRepeatID();

                    // Find associated question
                    xpathsXpath xp = xppe.getXPathByID(ancestorRepeatXPathID);
                    question q = questionnaire.getQuestion(xp.questionID);


                    thisNode = new TreeNode(q.text);
                    thisNode.Tag = ancestorRepeatXPathID;

                    if (previousNode == null)
                    {
                        // Check the innermost
                        treeViewRepeat.SelectedNode = thisNode;
                    }
                    else
                    {
                        thisNode.Nodes.Add(previousNode);
                    }
                    previousNode = thisNode;
                }
                currentCC = currentCC.ParentContentControl;
            }

            if (thisNode == null)
            {
                // Hide the control
//                this.groupBoxRepeat.Visible = false;
                root = null;
            }
            else
            {
                root.Nodes.Add(thisNode);
                this.treeViewRepeat.Nodes.Add(root);
                treeViewRepeat.ExpandAll();
            }
        }

        public bool shouldShow()
        {
            return (root != null);
        }

        public Word.ContentControl getRepeatAncestor(Word.ContentControl cc)
        {
            Word.ContentControl repeatAncestor = null;

            if (root != null
                && !treeViewRepeat.SelectedNode.Equals(root)) // this varies with some repeat
            {
                // Which one is checked?
                string variesInRepeatId = (string)treeViewRepeat.SelectedNode.Tag;

                Word.ContentControl currentCC = cc.ParentContentControl;
                while (repeatAncestor == null && currentCC != null)
                {
                    if (currentCC.Tag.Contains("od:repeat"))
                    {
                        TagData td = new TagData(currentCC.Tag);
                        string variesXPathID = td.getRepeatID();

                        if (variesXPathID.Equals(variesInRepeatId))
                        {
                            return currentCC;
                        }
                    }
                    currentCC = currentCC.ParentContentControl;
                }
            }

            return null;

        }

        //-------------------------------------------------------------------------------------------------
        List<Word.ContentControl> thisQuestionControls = null;

        public void init(
            Office.CustomXMLPart answersPart,
            questionnaire questionnaire,
            question q,
            XPathsPartEntry xppe,
            ConditionsPartEntry cpe)
        {

            QuestionHelper qh = new QuestionHelper(xppe, cpe);
            thisQuestionControls = qh.getControlsUsingQuestion(q);

            List<Word.ContentControl> relevantRepeats = new List<Word.ContentControl>();
            foreach (Word.ContentControl ccx in thisQuestionControls)
            {
                Word.ContentControl rpt = RepeatHelper.getYoungestRepeatAncestor(ccx);
                if (rpt == null)
                {
                    // will have to make the answer top level and we're done.
                    break;
                }
                else
                {
                    relevantRepeats.Add(rpt);
                }
            }

            init(
                answersPart,
                relevantRepeats,
                 questionnaire,
                 q.id,
                 xppe);

        }

        public void init(
            Office.CustomXMLPart answersPart,
            List<Word.ContentControl> relevantRepeats,
            questionnaire questionnaire, 
            string questionID,
            XPathsPartEntry xppe)
        {
            List<Office.CustomXMLNode> commonAncestors =null;
            foreach (Word.ContentControl repeat in relevantRepeats)
            {
                log.Info("considering relevantRepeat cc " + repeat.ID + " " + repeat.Tag);
                TagData repeatTD = new TagData(repeat.Tag);
                string repeatXPathID = repeatTD.getRepeatID();
                xpathsXpath repeatXP = xppe.getXPathByID(repeatXPathID);
                Office.CustomXMLNode repeatNode = answersPart.SelectSingleNode(repeatXP.dataBinding.xpath).ParentNode;

                if (commonAncestors == null)
                {
                    // First entry, so init
                    // Make a list of the ancestors of the 
                    // first repeat. 
                    commonAncestors = new List<Microsoft.Office.Core.CustomXMLNode>();
                    commonAncestors.Add(repeatNode);
                    log.Info("Added to common ancestors " + CustomXMLNodeHelper.getAttribute(repeatNode, "qref"));
                    addAncestors(commonAncestors, repeatNode);
                }
                else
                {
                    // cross off that list anything
                    // which isn't an ancestor of the other repeats.
                    List<Microsoft.Office.Core.CustomXMLNode> whitelist = new List<Microsoft.Office.Core.CustomXMLNode>();
                    whitelist.Add(repeatNode);
                    addAncestors(whitelist, repeatNode);
                    removeNonCommonAncestor(commonAncestors, whitelist);
                }

                if (commonAncestors.Count == 0) break;
            }
            if (commonAncestors == null)
            {
                    commonAncestors = new List<Microsoft.Office.Core.CustomXMLNode>();
            }

            // Is it OK where it is?
            // Yes - if it is top level
            log.Debug(questionID + " --> " + xppe.getXPathByQuestionID(questionID).dataBinding.xpath);
            // eg /oda:answers/oda:repeat[@qref='rpt1']/oda:row[1]/oda:answer[@id='qa_2']
            OkAsis = (xppe.getXPathByQuestionID(questionID).dataBinding.xpath.IndexOf("oda:repeat") < 0);
            Microsoft.Office.Core.CustomXMLNode currentPos = null; // so we can highlight existing choice
            // Yes - if it is a child of common ancestors
            if (OkAsis)
            {
                log.Debug("its top level");
            }
            else
            {
                foreach (Microsoft.Office.Core.CustomXMLNode currentNode in commonAncestors)
                {
                    Microsoft.Office.Core.CustomXMLNode selection =
                        currentNode.SelectSingleNode("oda:row[1]/oda:answer[@id='" + questionID + "']");
                    if (selection != null)
                    {
                        log.Debug("found it");
                        OkAsis = true;
                        currentPos = currentNode;
                        break;
                    }
                }
            }

            // Now make the tree from what is left in commonAncestors

            root = new TreeNode("Ask only once");
            this.treeViewRepeat.Nodes.Add(root);

            TreeNode thisNode = null;
            TreeNode previousNode = null;
            treeViewRepeat.HideSelection = false; // keep selection when focus is lost

            TreeNode nodeToSelect = null;

            foreach (Microsoft.Office.Core.CustomXMLNode currentNode in commonAncestors)
            {
                // Find the question associated with this repeat
                string rptQRef = CustomXMLNodeHelper.getAttribute(currentNode, "qref");
                    //currentNode.Attributes[1].NodeValue;
                question q = questionnaire.getQuestion(rptQRef);
                if (q == null)
                {
                    log.Error("no question with id " + rptQRef);
                }
                thisNode = new TreeNode(q.text);
                thisNode.Tag = rptQRef;

                if (currentNode == currentPos)
                {
                    nodeToSelect = thisNode;
                }

                if (previousNode == null)
                {
                    // Check the innermost (may be overridden below by what level user already had, if possible)
                    this.treeViewRepeat.SelectedNode = thisNode;
                }
                else
                {
                    thisNode.Nodes.Add(previousNode);
                }
                previousNode = thisNode;

            }

            if (thisNode != null)
            {
                root.Nodes.Add(thisNode);
            }
            treeViewRepeat.ExpandAll();

            if (nodeToSelect != null)
            {
                this.treeViewRepeat.SelectedNode = nodeToSelect;
                originalValue = thisNode;
            }
            else if (OkAsis)
            {
                originalValue = root;
                this.treeViewRepeat.SelectedNode = root;
            }
        }

        private TreeNode originalValue = null;

        private void addAncestors(List<Microsoft.Office.Core.CustomXMLNode> ancestors, Office.CustomXMLNode currentNode)
        {
            while (currentNode != null)
            {
                currentNode = currentNode.ParentNode;
                if (currentNode.BaseName.Equals("row")) currentNode = currentNode.ParentNode;
                if (currentNode.BaseName.Equals("answers")) return;

                ancestors.Add(currentNode);
                log.Info("Added to common ancestors " + CustomXMLNodeHelper.getAttribute(currentNode, "qref"));

            }
            return; // shouldn't get here!        
        }

        private void removeNonCommonAncestor(List<Microsoft.Office.Core.CustomXMLNode> commonAncestors, List<Microsoft.Office.Core.CustomXMLNode> whitelist)
        {
            List<Microsoft.Office.Core.CustomXMLNode> thingsToRemove = new List<Office.CustomXMLNode>();

            foreach (Microsoft.Office.Core.CustomXMLNode node in commonAncestors)
            {
                if (!myContains(whitelist,node))
                {
                    thingsToRemove.Add(node);
                }
            }

            foreach (Microsoft.Office.Core.CustomXMLNode node in thingsToRemove)
            {
                log.Info("Removed from common ancestors " + CustomXMLNodeHelper.getAttribute(node, "qref"));

                commonAncestors.Remove(node);
            }
        }

        private bool myContains(List<Microsoft.Office.Core.CustomXMLNode> whitelist, Microsoft.Office.Core.CustomXMLNode node)
        {
            string qref = CustomXMLNodeHelper.getAttribute(node, "qref");
            foreach (Microsoft.Office.Core.CustomXMLNode n2 in whitelist)
            {
                if (CustomXMLNodeHelper.getAttribute(n2, "qref").Equals(qref))
                {
                    return true;
                }
            }
            return false;
        }

        public bool OkAsis { get; set; }

        private string getVaryingRepeat()
        {
            if (treeViewRepeat.SelectedNode.Equals(root)) return null; // move to top level
                
            return (string)treeViewRepeat.SelectedNode.Tag;
        }

        public bool changed()
        {
            return !(treeViewRepeat.SelectedNode.Equals(originalValue));
        }

        /// <summary>
        /// When an answer's vary in repeat changes:
        /// - move the node
        /// - change its xpath
        /// - change the databinding in all relevant controls
        /// </summary>
        /// <param name="questionID"></param>
        /// <param name="xp"></param>
        /// <param name="answersPart"></param>
        public void moveIfNecessary(string questionID, xpathsXpath xp, 
            Office.CustomXMLPart answersPart)
        {
            string varyInRepeat = getVaryingRepeat( );
            if (varyInRepeat == null)
            {
                // make the answer top level and we're done.
                NodeMover nm = new NodeMover();
                nm.Move(xp.dataBinding.xpath, "/oda:answers");
                nm.adjustBinding(thisQuestionControls, "/oda:answers", questionID);
            }
            else
            {
                // Move it to the selected repeat
                // get the node corresponding to the repeat's row
                Office.CustomXMLNode node = answersPart.SelectSingleNode("//oda:repeat[@qref='" + varyInRepeat + "']/oda:row[1]");
                if (node == null)
                {
                    log.Error("no node for nested repeat " + varyInRepeat);
                }
                string toRepeat = NodeToXPath.getXPath(node);
                NodeMover nm = new NodeMover();
                nm.Move(xp.dataBinding.xpath, toRepeat);
                nm.adjustBinding(thisQuestionControls, toRepeat, questionID);

            }
        }


    }
}
