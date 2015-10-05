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
using System.Windows.Forms;

using NLog;

using Office = Microsoft.Office.Core;
using OpenDoPEModel;
using Word = Microsoft.Office.Interop.Word;
using Microsoft.Office.Tools.Word.Extensions; // for VSTOObject

namespace OpenDope_AnswerFormat
{
    /// <summary>
    /// Invoked when a CC is moved in this docx.
    /// We detect a move when add event fires,
    /// with an ID already known.
    /// </summary>
    class ContentControlMoveHandler : ContentControlHandlerAbstract
    {
        //static Logger log = LogManager.GetLogger("OpenDoPE_Wed");

        public ContentControlMoveHandler()//Word.ContentControl copiedCC)
            : base()//copiedCC)
        {
        }

        public void handle(Word.ContentControl copiedCC)
        {
            // In the move case, we know that if it is a FabDocx content control, it is valid
            // in this docx
            if (copiedCC.Tag == null)
            {
                // Its just some content control we don't care about
                fabDocxState.registerKnownSdt(copiedCC);
                return; // this is OK, since if it contains a repeat or something, that event will fire
            }
            else if (copiedCC.Tag.Contains("od:xpath"))
            {
                td = new TagData(copiedCC.Tag);

                // RULE: A variable cc can copied wherever.  
                // If its repeat ancestors change, its "vary in repeat" 
                // will need to change (to lowest common denominator).
                handleXPath(td.getXPathID(), true);
            }
            else if (copiedCC.Tag.Contains("od:repeat"))
            {
                // RULE: A repeat can be moved wherever under
                // the same repeat ancestor cc.
                
                // It can be moved elsewhere, provided it is 
                // the only cc using that repeat.  (change 
                // AF and XPaths accordingly).

                handleRepeat(copiedCC);

            }
            else if (copiedCC.Tag.Contains("od:condition"))
            {
                // Identify child content controls
                Microsoft.Office.Interop.Word.Range rng = copiedCC.Range;
                Word.ContentControls ccs = rng.ContentControls;
                foreach (Word.ContentControl desc in ccs)
                {
                    if (desc.ParentContentControl.ID.Equals(copiedCC.ID))
                    {
                        handle(desc);
                    }
                }

            }

        }


        /// <summary>
        // RULE: A repeat can be moved wherever under
        // the same repeat ancestor cc.

        // It can be moved elsewhere, provided it is 
        // the only cc using that repeat.  (change 
        // AF and XPaths accordingly).

        // In principle, all n x cc using that repeat
        // could legally be moved to the same destination,
        // but we have no way to facilitate this! 
        /// </summary>
        private void handleRepeat(Word.ContentControl copiedCC)
        {

            td = new TagData(copiedCC.Tag);
            string repeatID = td.getRepeatID();

            suppressDescendantEvents(copiedCC);

            // Look in the answer file to find this repeats
            // closest ancestor repeat (if any).
            // It is enough to look at the XPath, so get that.
            // (Actually, either way you get a qref, which is
            //  NOT a repeat id. ) 
            string xpathCurrent = xppe.getXPathByID(repeatID).dataBinding.xpath;
            // something like /oda:answers/oda:repeat[@qref='rpt1']/oda:row[1]/oda:repeat[@qref='rpt2']"
            int index = xpathCurrent.LastIndexOf("oda:repeat");
            if (index < 0)
            {
                log.Error("Couldn't find repeat in " + xpathCurrent);
            }
            string xpathSubstring = xpathCurrent.Substring(0, index);
            log.Debug("xpath substring: " + xpathSubstring); //eg /oda:answers/oda:repeat[@qref='rpt1']/oda:row[1]/
            // OK, find last remaining qref
            int qrefIndex = xpathSubstring.LastIndexOf("qref");

            Word.ContentControl rptAncestor = RepeatHelper.getYoungestRepeatAncestor(copiedCC);
            if (qrefIndex < 0)
            {
                // No repeat ancestor, so this repeat cc can be moved anywhere
                log.Debug("No repeat ancestor, so this repeat cc can be moved anywhere");

                // Has it been moved into a repeat?
                if (rptAncestor == null)
                {
                    // Destination has no repeat ancestor, 
                    // so nothing to do
                    return;
                }

                // Now change AF and XPath structure.
                string destRptID = (new TagData(rptAncestor.Tag)).getRepeatID();
                string destRptXPath = xppe.getXPathByID(destRptID).dataBinding.xpath;

                //Office.CustomXMLPart answersPart = model.userParts[0]; // TODO: make this better
                //Office.CustomXMLNode node = answersPart.SelectSingleNode(destRptXPath);
                //if (node == null)
                //{
                //    log.Error("no answer for repeat " + destRptXPath);
                //}
                //string toRepeat = NodeToXPath.getXPath(node);
                NodeMover nm = new NodeMover();
                nm.Move(xpathCurrent, destRptXPath);

                return;
                    // TODO: Need to figure out whether to do this just for
                    // the repeat (and let descendants take care of themselves),
                    // or do it for all (and suppress action on descendant event)
                    // NodeMover does it for all XPaths, so unless that is changed, we're looking at suppression!
                    // - suppression is difficult?   
                    // - easy to do just a single xpath, but what about answers? we move these as a tree!! leave desc behind?
                // BIG QUESTION: are there things we need to do to the descendants in response to their events,
                // or does this repeat logic take care of everything?
                // What about in the copy case?
            }
            else
            {
                // Had a repeat ancestor, with ancestorRepeatID

                string ancestorQref = xpathSubstring.Substring(qrefIndex + 6);
                //log.Debug("ancestorQref: " + ancestorQref); //  rpt1']/oda:row[1]/
                ancestorQref = ancestorQref.Substring(0, ancestorQref.IndexOf("'"));
                log.Debug("ancestorQref: " + ancestorQref);

                string ancestorRepeatID = xppe.getXPathByQuestionID(ancestorQref).id;
                log.Debug("Had a repeat ancestor, with ancestorRepeatID: " + ancestorRepeatID);

                List<Word.ContentControl> controlsThisRepeat = getRepeatCCsUsingRepeatID(copiedCC, repeatID);

                // Find the new repeat ancestor, if any
                if (rptAncestor == null)
                {
                    // It can be moved elsewhere, provided it is 
                    // the only cc using that repeat

                    // Is it the only repeat cc using this repeat id?
                    if (controlsThisRepeat.Count == 0) // since this cc excluded from count
                    {
                        // Yes
                        NodeMover nm = new NodeMover();
                        nm.Move(xpathCurrent, "/oda:answers");
                    }
                    else
                    {
                        // This is a problem
                        MessageBox.Show("Your move includes a repeat content control which can't go here. Removing that repeat.");
                        removeButKeepContents(copiedCC);
                        // and enable descendant events
                        fabDocxState.suppressEventsForSdtID.Clear();
                        return;
                        // ie there were >1 cc using this repeat ID, and we just removed one of them!
                    }
                }
                else
                {
                    string destRptID = (new TagData(rptAncestor.Tag)).getRepeatID();
                    string destRptXPath = xppe.getXPathByID(destRptID).dataBinding.xpath;

                    if (destRptID.Equals(ancestorRepeatID))
                    {
                        // OK, no change in ancestor.
                        // Nothing to do.
                        return;
                    }
                    else if (controlsThisRepeat.Count == 1)
                    {
                        // OK, only used once
                        NodeMover nm = new NodeMover();
                        nm.Move(xpathCurrent, destRptXPath);
                    }
                    else
                    {
                        // Not allowed
                        MessageBox.Show("Your move includes a repeat content control which can't go here. Removing that repeat.");
                        removeButKeepContents(copiedCC);
                        // and enable descendant events
                        fabDocxState.suppressEventsForSdtID.Clear();
                        return;
                        // ie there were >1 cc using this repeat ID, and we just removed one of them!
                    }

                }

            }


        }



    }
}
