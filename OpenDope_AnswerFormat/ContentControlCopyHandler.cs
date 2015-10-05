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
    /// Invoked when a CC is copied in this docx.
    /// We detect a copied when add event fires,
    /// with an ID unknown.
    /// 
    /// Note that this CC could also have been
    /// copied from another document, so we need
    /// to handle that.  If it has an od tag which
    /// works in this docx, that's OK.  If it doesn't
    /// then complete the paste but strip out the CC.
    /// </summary>
    class ContentControlCopyHandler : ContentControlHandlerAbstract
    {
        //static Logger log = LogManager.GetLogger("OpenDoPE_Wed");


        public ContentControlCopyHandler()//Word.ContentControl copiedCC)
            : base()//copiedCC)
        {
        }

        public void handle(Word.ContentControl copiedCC)
        {
            if (copiedCC.Tag == null)
            {
                // Its just some content control we don't care about
                fabDocxState.registerKnownSdt(copiedCC);
                return; // this is OK, since if it contains a repeat or something, that event will fire
            }
            log.Info("copy handler invoked on CC: " + copiedCC.Tag);
            
            if (copiedCC.Tag.Contains("od:xpath"))
            {
                td = new TagData(copiedCC.Tag);
                string xpathID = td.getXPathID();

                // Check it is known to this docx
                if (xppe.getXPathByID(xpathID) == null)
                {
                    removeButKeepContents(copiedCC);
                    return;
                }
                // RULE: A variable cc can copied wherever.  
                // If its repeat ancestors change, its "vary in repeat" 
                // will need to change (to lowest common denominator).
                handleXPath(td.getXPathID(), true);
            }
            else if (copiedCC.Tag.Contains("od:repeat"))
            {
                td = new TagData(copiedCC.Tag);
                string xpathID = td.getRepeatID();

                // Check it is known to this docx
                if (xppe.getXPathByID(xpathID) == null)
                {
                    removeButKeepContents(copiedCC);
                    return;
                }
                // RULE: A repeat can only be copied if destination 
                // has same repeat ancestors (in which case no change 
                // to answer file is required).
                handleRepeat(copiedCC);
            }
            else if (copiedCC.Tag.Contains("od:condition"))
            {
                // Find child CC
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
            else
            {
                // Its just some content control we don't care about
                fabDocxState.registerKnownSdt(copiedCC);
                return;
            }

        }

        /// <summary>
        /// A repeat content control can only be copied if destination has same 
        /// repeat ancestors (in which case no change to answer file is required).
        /// 
        /// If it doesn't, remove the content control but keep contents. 
        /// </summary>
        private void handleRepeat(Word.ContentControl copiedCC)
        {            
            string repeatID = td.getRepeatID();

            suppressDescendantEvents(copiedCC);

            // Find one of the other content controls using the same od:repeat
            List<Word.ContentControl> controlsThisRepeat = getRepeatCCsUsingRepeatID(copiedCC, repeatID);

            if (controlsThisRepeat.Count == 0)
            {
                log.Warn("Couldn't find the content control from which this repeat cc " + repeatID + "copied!");
                return;
            }

            Word.ContentControl copiedCcRepeatAncestor = RepeatHelper.getYoungestRepeatAncestor(copiedCC);

            if (copiedCcRepeatAncestor == null)
            {
                log.Warn("copiedCcRepeatAncestor is null. Check others are as well.");
                // Then must be null for the others as well.
                // Check this.
                foreach (Word.ContentControl ccx in controlsThisRepeat)
                {
                    if (RepeatHelper.getYoungestRepeatAncestor(ccx) != null)
                    {
                        // This is a problem
                        MessageBox.Show("Your paste includes a repeat content control which can't go here. Removing that repeat.");
                        removeButKeepContents(copiedCC);
                        // and enable descendant events
                        fabDocxState.suppressEventsForSdtID.Clear();
                        return;
                    }
                }
            }
            else
            {
                string copiedCcAncestorID = (new TagData(copiedCcRepeatAncestor.Tag)).getRepeatID();
                log.Warn("copiedCcRepeatAncestor is " + copiedCcAncestorID + ". Check others are as well.");
                foreach (Word.ContentControl ccx in controlsThisRepeat)
                {
                    Word.ContentControl ancestor = RepeatHelper.getYoungestRepeatAncestor(ccx);
                    if (ancestor == null)
                    {
                        MessageBox.Show("Your paste includes a repeat content control which can't go here. Removing that repeat.");
                        removeButKeepContents(copiedCC);
                        // and enable descendant events
                        fabDocxState.suppressEventsForSdtID.Clear();
                        return;
                    }
                    else {
                        string ancestorID = (new TagData(ancestor.Tag)).getRepeatID();
                        if (!ancestorID.Equals(copiedCcAncestorID))
                        {
                            MessageBox.Show("Your paste includes a repeat content control which can't go here. Removing that repeat.");
                            removeButKeepContents(copiedCC);
                            // and enable descendant events
                            fabDocxState.suppressEventsForSdtID.Clear();
                            return;
                        }
                    }
                }
            }

        }


        //public static bool isDescendant(Office.CustomXMLNode ancestor, Office.CustomXMLNode possibleDesc)
        //{
        //    return possibleDesc.XPath.StartsWith(ancestor.XPath);
        //}
    }
}
