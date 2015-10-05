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

using OpenDope_AnswerFormat.Helpers;

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
    abstract class ContentControlHandlerAbstract
    {
        protected static Logger log = LogManager.GetLogger("OpenDoPE_Wed");

        protected FabDocxState fabDocxState;
        protected Model model;

        protected XPathsPartEntry xppe;
        protected ConditionsPartEntry cpe;

        //protected Word.ContentControl copiedCC;

        protected Office.CustomXMLPart questionsPart;
        //protected question q;
        protected questionnaire questionnaire;

        protected TagData td;

        public ContentControlHandlerAbstract()//Word.ContentControl copiedCC)
        {
            fabDocxState = (FabDocxState)Globals.ThisAddIn.Application.ActiveDocument.GetVstoObject(Globals.Factory).Tag; 
            //this.copiedCC = copiedCC;

            model = fabDocxState.model;
            xppe = new XPathsPartEntry(model); // used to get entries
            cpe = new ConditionsPartEntry(model);

        }

        /// <summary>
        /// A repeat CC can have descendant CCs (obviously).
        /// The handler for the repeat CC does everything we need,
        /// so ensure the event handlers for descendant CCs do nothing.
        /// 
        /// Exception: a descendant CC containing a question which
        /// does not vary in this repeat. We could be moving it to
        /// somewhere which requires its varying to be changed.
        /// </summary>
        protected void suppressDescendantEvents(Word.ContentControl copiedCC)
        {
            string repeatID = td.getRepeatID();
            string repeatXPath = xppe.getXPathByID(repeatID).dataBinding.xpath;

            // Find descendant CC
            Microsoft.Office.Interop.Word.Range rng = copiedCC.Range;
            Word.ContentControls ccs = rng.ContentControls;
            List<string> suppressEventsForSdtID = new List<string>();
            foreach (Word.ContentControl desc in ccs)
            {
                // Is this the exception?
                if (desc.Tag.Contains("od:xpath"))
                {
                    string xpathID = (new TagData(desc.Tag)).getXPathID();
                    string xpath = xppe.getXPathByID(xpathID).dataBinding.xpath;

                    if (xpath.StartsWith(repeatXPath))
                    {
                        suppressEventsForSdtID.Add(desc.ID);
                        log.Info("Suppressing events for CC " + desc.ID + " " + desc.Tag + " " + desc.Title);
                    }
                    else
                    {
                        log.Info("Retaining events for CC " + desc.ID + " " + desc.Tag + " " + desc.Title);
                    }
                }
                else
                {
                    suppressEventsForSdtID.Add(desc.ID);
                    log.Info("Suppressing events for CC " + desc.ID + " " + desc.Tag + " " + desc.Title);
                }
            }
            fabDocxState.suppressEventsForSdtID = suppressEventsForSdtID;

            // Repeat cc will be stripped if
            // an attempt is made to:
            // - COPY it somewhere with different ancestors
            // - MOVE it somewhere with different ancestors (if it is used elsewhere)
            // If we strip this repeat CC, we also need to process descendant CC's:
            // - repeat (as per above rules)
            // - variable insert (strip if varies in this repeat OR could enable its event, in which case user will be forced to choose where it varies)
            // Recursive strip, or enable descendant events?
            // If we always stripped descendant CC.
            // - in case of a move, you're left with no CCs (so UNDO is likely)
            // If we process descendant cc, move may partially work.

            // Either way, undo should just work! Except vary in repeat setting, which user will need to answer.
            // So slightly better to do descendant processing
            // Hmmm, undo doesn't work at all!

            // TODO: can you copy a CC into itself? I guess it is given a new ID
        }


        /// <summary>
        // RULE: A variable cc can copied wherever.  
        // If its repeat ancestors change, its "vary in repeat" 
        // will need to change (to lowest common denominator). 

        // If this cc is not in any repeat,
        // make the answer top level and we're done

        // Otherwise, could assume its position is OK wrt
        // existing repeats.  
        // So if anything, we just need to move it
        // up the tree until
        // we reach a node which contains this additional repeat.

        // But we'd like this code to
        // be used for both moves and copy. (Move case
        // needs this constraint relaxed)
        // So our algorithm finds viable positions 
        // (ie ancestors common to all
        // repeats). 

        // That node and higher are candidates for new position
        // Ask user to choose.
        /// </summary>
        protected void handleXPath(string xpathID, bool dontAskIfOkAsis)
        {
            xpathsXpath xp = xppe.getXPathByID(xpathID);

            string questionID = xp.questionID;

            // If this cc is not in any repeat,
            // make the answer top level and we're done

            // Otherwise, we can assume its position is OK wrt
            // existing repeats.  

            // So if anything, we just need to move it
            // up the tree until
            // we reach a node which contains this additional repeat.

            // But for the variable move case (handled in MoveHandler)
            // an existing repeat will no longer be a constraint.
            // So better to just have a single algorithm which
            // finds viable positions (ie ancestors common to all
            // repeats). 

            // 2 algorithms for doing this.
            // The first:  Make a list of the ancestors of the 
            // first repeat.  Then cross off that list anything
            // which isn't an ancestor of the other repeats.

            // The second: Get XPath for each repeat.
            // Find the shortest XPath.
            // Then find the shortest substring common to each 
            // repeat. Then make a list out of that.

            // I like the first better.

            // Find all cc's in which this question is used.
            QuestionHelper qh = new QuestionHelper(xppe, cpe);
            List<Word.ContentControl> thisQuestionControls = qh.getControlsUsingQuestion(questionID, xpathID);

            // For each such cc, find closest repeat ancestor cc (if any).
            // With each such repeat, do the above algorithm.

            // Find all cc's in which this question is used.
            List<Word.ContentControl> relevantRepeats = new List<Word.ContentControl>();
            foreach (Word.ContentControl ccx in thisQuestionControls)
            {
                Word.ContentControl rpt = RepeatHelper.getYoungestRepeatAncestor(ccx);
                if (rpt == null)
                {
                    // make the answer top level and we're done.
                    // That means moving it in AF, and XPaths part.
                    log.Info("question " + questionID + " used at top level, so moving it there.");
                    NodeMover nm = new NodeMover();
                    nm.Move(xp.dataBinding.xpath, "/oda:answers");
                    nm.adjustBinding(thisQuestionControls, "/oda:answers", questionID);
                    return;

                }
                else
                {
                    relevantRepeats.Add(rpt);
                }
            }

            Office.CustomXMLPart answersPart = model.answersPart; // userParts[0]; // TODO: make this better

            // That node and higher are candidates for new position
            // Ask user to choose, or cancel (and remove this cc,
            // but what about any impending child add events??  Maybe
            // need a cancel state)

            this.questionsPart = model.questionsPart;
            questionnaire = new questionnaire();
            questionnaire.Deserialize(questionsPart.XML, out questionnaire);

            FormMoveQuestion formMoveQuestion = new FormMoveQuestion(answersPart, relevantRepeats, questionnaire, questionID, xppe);
            if (dontAskIfOkAsis
                && formMoveQuestion.OkAsis() )
            {
                // Do nothing
            }
            else
            {
                formMoveQuestion.ShowDialog();
                formMoveQuestion.moveIfNecessary(questionID, xp, answersPart);

                //string varyInRepeat = formMoveQuestion.getVaryingRepeat();
                //if (varyInRepeat == null)
                //{
                //    // make the answer top level and we're done.
                //    NodeMover nm = new NodeMover();
                //    nm.Move(xp.dataBinding.xpath, "/oda:answers");
                //    nm.adjustBinding(thisQuestionControls, "/oda:answers", questionID);
                //}
                //else
                //{
                //    // Move it to the selected repeat
                //    // get the node corresponding to the repeat's row
                //    Office.CustomXMLNode node = answersPart.SelectSingleNode("//oda:repeat[@qref='" + varyInRepeat + "']/oda:row[1]");
                //    if (node == null)
                //    {
                //        log.Error("no node for nested repeat " + varyInRepeat);
                //    }
                //    string toRepeat = NodeToXPath.getXPath(node);
                //    NodeMover nm = new NodeMover();
                //    nm.Move(xp.dataBinding.xpath, toRepeat);
                //    nm.adjustBinding(thisQuestionControls, toRepeat, questionID);

                //}
            }
            formMoveQuestion.Dispose();
        }

        protected List<Word.ContentControl> getRepeatCCsUsingRepeatID(Word.ContentControl copiedCC, string repeatID)
        {
            List<Word.ContentControl> controlsThisRepeat = new List<Word.ContentControl>();
            foreach (Word.ContentControl ccx in Globals.ThisAddIn.Application.ActiveDocument.ContentControls)
            {
                if (ccx.Tag.Contains("od:repeat")
                    && !ccx.ID.Equals(copiedCC.ID))
                {
                    string thisID = (new TagData(ccx.Tag)).getRepeatID();
                    if (thisID.Equals(repeatID))
                    {
                        controlsThisRepeat.Add(ccx);
                    }
                }
            }
            return controlsThisRepeat;
        }

        protected void removeButKeepContents(Word.ContentControl copiedCC)
        {
            log.Warn("Deleting cc " + copiedCC.Tag);
            copiedCC.Delete(); // keeps contents
        }

    }
}
