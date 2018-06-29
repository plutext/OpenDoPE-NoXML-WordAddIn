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

using System.Data;
using Word = Microsoft.Office.Interop.Word;
using Microsoft.Office.Tools.Word.Extensions; // for VSTOObject

using NLog;

using OpenDoPEModel;
using System.Windows.Forms;

namespace OpenDope_AnswerFormat
{
    class RepeatButtonAction
    {

        static Logger log = LogManager.GetLogger("OpenDope_AnswerFormat");

        public void buttonRepeat_Click(Office.IRibbonControl control)
        {
            Word.Document document = Globals.ThisAddIn.Application.ActiveDocument;

        FabDocxState fabDocxState = (FabDocxState)Globals.ThisAddIn.Application.ActiveDocument.GetVstoObject(Globals.Factory).Tag;

            Model model = fabDocxState.model;
            Office.CustomXMLPart answersPart = model.answersPart; //.userParts[0]; // TODO: make this better
            XPathsPartEntry xppe = new XPathsPartEntry(model); // used to get entries

            questionnaire questionnaire = new questionnaire();
            questionnaire.Deserialize(model.questionsPart.XML, out questionnaire);

            Microsoft.Office.Interop.Word.Range rng = document.ActiveWindow.Selection.Range;

            // Are there any content controls in the selection?
            Word.ContentControls ccs = rng.ContentControls;

            // First identify nested repeats
            List<Word.ContentControl> nestedRepeats = new List<Word.ContentControl>();
            foreach (Word.ContentControl desc in ccs)
            {
                if (desc.Tag.Contains("od:repeat"))
                {
                    nestedRepeats.Add(desc);
                }
            }

            // questions contains questions wrapped by repeat,
            // but not nested inside another repeat
            List<question> questions = new List<question>();

            foreach (Word.ContentControl desc in ccs)
            {
                if (desc.Tag.Contains("od:repeat"))
                {
                    continue; // will handle repeats later
                }

                // exclude if in nested repeat, since
                // this question will have previously been dealt with
                // (ie it already varies with that repeat, or the
                // use has said they don't want it to)
                if (isInside(nestedRepeats, desc))
                {
                    continue;
                }

                //log.Warn("got a desc with tag " + desc.Tag);
                // Get the tag
                if (desc.Tag.Contains("od:xpath"))
                {
                    TagData td = new TagData(desc.Tag);
                    string xpathID = td.getXPathID();
                    //log.Warn("xpath is  " + xpathID);
                    xpathsXpath xp = xppe.getXPathByID(xpathID);
                    log.Warn("qid is  " + xp.questionID);
                    question q = questionnaire.getQuestion(xp.questionID);
                    if (q == null)
                    {
                        log.Error("Consistency issue: couldn't find question {0} used in xpath {1}", xp.questionID, xpathID);
                    }
                    else if (!questions.Contains(q)) 
                    {
                        questions.Add(q);
                    }
                }
                else if (desc.Tag.Contains("od:condition"))
                {
                    // TODO: find questions inside conditions
                }
            }

            if (questions.Count > 0)
            {
                // Rule: Only questions which aren't used elsewhere can vary in a repeat.
                // Check that none of the questions that will be
                // inside the repeat are also used outside of it.
                List<question> questionsUsedOutside = new List<question>();
                foreach (Word.ContentControl ccx in document.ContentControls)
                {
                    if (isListed(ccs, ccx))
                    {
                        // this control is inside the repeat
                    }
                    else
                    {
                        // its outside, so look at its question
                        // TODO: conditions, repeats
                        if (ccx.Tag.Contains("od:xpath"))
                        {
                            TagData td = new TagData(ccx.Tag);
                            string xpathID = td.getXPathID();
                            //log.Warn("xpath is  " + xpathID);
                            xpathsXpath xp = xppe.getXPathByID(xpathID);
                            //log.Warn("qid is  " + xp.questionID);
                            question q = questionnaire.getQuestion(xp.questionID);
                            if (q == null)
                            {
                                log.Error("Consistency issue: couldn't find question {0} used in xpath {1}", xp.questionID, xpathID);
                            }
                            else
                            {
                                if (questions.Contains(q))
                                {
                                    questionsUsedOutside.Add(q);
                                }
                            }
                        }
                    }
                } // foreach

                // If they are, they can't vary in repeat.  Get the user to OK this.
                if (questionsUsedOutside.Count == 0)
                {
                    log.Info("None of the questions in wrapping repeat are used elsewhere");
                }
                else
                {
                    log.Info(questionsUsedOutside.Count + " of the questions in wrapping repeat are used elsewhere");
                    DialogResult dresult = MessageBox.Show(
                        questionsUsedOutside.Count + " of the questions here are also used elsewhere. If you continue, these won't vary in each repeat.",
                        "Questions used elsewhere", MessageBoxButtons.OKCancel);
                    if (dresult == DialogResult.OK)
                    {
                        // Just remove them from the list
                        foreach (question qx in questionsUsedOutside)
                        {
                            questions.Remove(qx);
                        }
                    }
                    else
                    {
                        log.Info("User cancelled wrapping repeat coz questions used elsewhere");
                        return;
                    }
                }
            }

            // Create control
            Word.ContentControl wrappingRepeatCC = null;
            object oRng = rng;
            try
            {
                fabDocxState.inPlutextAdd = true;

                // 2016 UI doesn't allow you wrap a content control around multiple table rows; it silently restricts it to 1 row.
                // To get around this:
                Word.Selection selection = Globals.ThisAddIn.Application.Selection;
                if (selection.Tables.Count==1)
                {
                    //MessageBox.Show("detected a table");
                    //foreach (Word.Table tbl in selection.Tables)
                    //{
                    //    MessageBox.Show("table with " + tbl.Rows.Count);
                    //}
                    Word.Table table = selection.Tables[1]; // 1-based index, and TopLevelTables only contains entire tables!

                    // TODO if (wholeTableInside)
                    // else

                    Word.Table newTable = null;
                    Word.Row rowFirstInTable = null;
                    Word.Row rowInSelectionFirst=null;
                    Word.Row rowAfterSelectionFirst = null;
                    int rowsInSelection = 0;
                    foreach (Word.Row row in table.Rows)
                    {
                        if (rowFirstInTable==null)
                        {
                            rowFirstInTable = row;
                        }

                        // MessageBox.Show("considering a row ... ");
                        if (row.Range.Start >= selection.Start 
                            && row.Range.Start <= selection.End
                            && row.Range.End <= selection.End
                            )
                        {
                            if (rowInSelectionFirst==null) {
                                //MessageBox.Show("found first selected row");
                                rowInSelectionFirst = row;
                            }
                        } else if (row.Range.Start >= selection.End)
                        {
                            if (rowAfterSelectionFirst==null)
                            {
                                //MessageBox.Show("found first excluded row");
                                rowAfterSelectionFirst = row;
                            }
                        }
                        if (rowInSelectionFirst!=null && rowAfterSelectionFirst==null)
                        {
                            rowsInSelection++;
                        }
                    }

                    if (rowsInSelection > 1)
                    {
                        // now split; didn't do this before, so we don't upset selection
                        // second split: do this first
                        Word.Table lastTbl = null;
                        if (rowAfterSelectionFirst != null)
                        {
                            lastTbl = table.Split(rowAfterSelectionFirst);
                        }

                        // first split
                        bool firstSplit = false;
                        if (rowInSelectionFirst == rowFirstInTable) {
                            // no need to split, since no rows above
                            newTable = table;
                        } else
                        {
                            newTable = table.Split(rowInSelectionFirst);
                            firstSplit = true;
                        }

                        object tblRng = newTable.Range;
                        wrappingRepeatCC = document.ContentControls.Add(Word.WdContentControlType.wdContentControlRichText, ref tblRng);

                        // now remove the unwanted inserted paragraphs
                        if (lastTbl!=null)
                        {
                            Word.Range lastRng = lastTbl.Range;
                            lastRng.Collapse(Word.WdCollapseDirection.wdCollapseStart);
                            //lastRng.InsertBefore("Z");
                            lastRng.Move(Word.WdUnits.wdCharacter, -1);
                            lastRng.Delete(Word.WdUnits.wdCharacter, 1);
                            //lastRng.InsertBefore("Z");
                        }

                        if (firstSplit) {
                            Word.Range fsRng = newTable.Range;
                            fsRng.Collapse(Word.WdCollapseDirection.wdCollapseStart);
                            //fsRng.InsertBefore("A");
                            fsRng.Move(Word.WdUnits.wdCharacter, -1);
                            fsRng.Delete(Word.WdUnits.wdCharacter, 1);
                            //fsRng.InsertBefore("A");

                        }

                    } else
                    {
                        // its just a single row, so do it the normal way
                        wrappingRepeatCC = document.ContentControls.Add(Word.WdContentControlType.wdContentControlRichText, ref oRng);

                    }
                } else
                {
                    // usual case
                    wrappingRepeatCC = document.ContentControls.Add(Word.WdContentControlType.wdContentControlRichText, ref oRng);
                }





                //wrappingRepeatCC.Range.Expand(Word.WdUnits.wdRow);


                //cc.MultiLine = true; // Causes error for RichText
            }
            catch (System.Exception ex)
            {
                log.Warn(ex);
                MessageBox.Show(ex.Message);
                MessageBox.Show("Selection must be either part of a single paragraph, or one or more whole paragraphs");
                fabDocxState.inPlutextAdd = false;
                return;
            }


            FormRepeat formRepeat = new FormRepeat(model.questionsPart, answersPart, model, wrappingRepeatCC);
            formRepeat.ShowDialog();
            string repeatId = formRepeat.ID;
            formRepeat.Dispose();

            // necessary here? shouldn't be..
            //answersPart.NamespaceManager.AddNamespace("answers", "http://opendope.org/answers");

            // Destination for moves
            Office.CustomXMLNode destination = answersPart.SelectSingleNode("//oda:repeat[@qref='" + repeatId + "']/oda:row");
            if (destination == null)
            {
                log.Error("no rpt node " + repeatId);
            }

            Dictionary<string, string> xpathChanges = new Dictionary<string, string>();

            // Questions, Conditions
            // ^^^^^^^^^^^^^^^^^^^^^
            // If so, the associated questions may need to be moved into the repeat in the answers XML.
            // Present a table of questions, where the user can say yes/no to each,
            // then move.. table excludes:
            // 1. any that are used outside the repeat, since these can't be made to vary (see above)
            // 2. any that are in a nested repeat
            if (questions.Count > 0)
            {
                FormRepeatWhichVariables formRepeatWhichVariables = new FormRepeatWhichVariables(questions);
                formRepeatWhichVariables.ShowDialog();

                List<question> questionsWhichRepeat = formRepeatWhichVariables.getVars();

                formRepeatWhichVariables.Dispose();

                log.Info(answersPart.XML);

                foreach (question q in questionsWhichRepeat)
                {
                    // Find the relevant answer (by ID)
                    // (easiest to do using XPath on XML document
                    Office.CustomXMLNode node = answersPart.SelectSingleNode("//oda:answer[@id='" + q.id + "']");
                    if (node == null)
                    {
                        log.Error("no node " + q.id);
                    }
                    string fromXPath = NodeToXPath.getXPath(node);
                    log.Info("from: " + fromXPath);

                    // Move it
                    String nodeXML = node.XML; // No API to add a node!
                    node.ParentNode.RemoveChild(node);
                    destination.AppendChildSubtree(nodeXML);

                    // So we'll have to change its xpath in XPaths part
                    // eg from:
                    //   "/oda:answers/oda:answer[@id='qa_2']"
                    // to:
                    //   "/oda:answers/oda:repeat[@qref='rpt1"]/oda:row[1]/oda:answer[@id='qa_2']"
                    //
                    // CustomXMLNode's Xpath produces something like: /ns2:answers[1]/ns2:answer[1]
                    // which we don't want

                    string toXPath = NodeToXPath.getXPath(destination.LastChild);
                    log.Info("to: " + toXPath);

                    xpathChanges.Add(fromXPath, toXPath);
                }

            }
            // nested repeats
            // ^^^^^^^^^^^^^^
            // 1. move the nested repeat answer structure
            // 2. change the xpath for all questions re anything in the nested repeat

            // Note: if wrapping repeat r0 around r1 which in turn contains r2,
            // must avoid explicitly processing  r2, since doing stuff to r1 here is
            // enough to take care of r2.

            // So, step 0. Find top level nested repeats
            // Already have nestedRepeats list, so just remove from it
            // those which aren't top level.
            foreach (Word.ContentControl desc in nestedRepeats)
            {
                if (!desc.ParentContentControl.ID.Equals(wrappingRepeatCC.ID) )
                {
                    // not top level, so remove
                    nestedRepeats.Remove(desc);
                }
            }

            if (nestedRepeats.Count > 0)
            {
                foreach (Word.ContentControl desc in nestedRepeats)
                {
                    TagData td = new TagData(desc.Tag);
                    string nestedRepeatXPathID = td.getRepeatID();

                    // Get the XPath, to find the question ID,
                    // which is what we use to find the repeat answer.
                    xpathsXpath xp = xppe.getXPathByID(nestedRepeatXPathID);

                    // 1. move the nested repeat answer structure
                    Office.CustomXMLNode node = answersPart.SelectSingleNode("//oda:repeat[@qref='" + xp.questionID + "']");
                    if (node == null)
                    {
                        log.Error("no node for nested repeat " + xp.questionID);
                    }
                    string fromXPath = NodeToXPath.getXPath(node);
                    log.Info("from: " + fromXPath);

                    // Move it
                    String nodeXML = node.XML; // No API to add a node!
                    node.ParentNode.RemoveChild(node);
                    destination.AppendChildSubtree(nodeXML);

                    // 2. change the xpath for all questions re anything in the nested repeat
                    // With a bit of luck, this will just work!
                    string toXPath = NodeToXPath.getXPath(destination.LastChild);
                    log.Info("to: " + toXPath);

                    xpathChanges.Add(fromXPath, toXPath);
                }
            }

            // Now do the substitutions in the XPaths part - for all
            string xpaths = model.xpathsPart.XML;
            foreach (KeyValuePair<string, string> entry in xpathChanges)
            {
                xpaths = xpaths.Replace(entry.Key, entry.Value);
            }
            CustomXmlUtilities.replaceXmlDoc(model.xpathsPart, xpaths);
            //log.Info(model.xpathsPart.XML);
            //log.Info(answersPart.XML);

            // Now do the substitutions in the content control databindings
            // (Added 2012 12 16, since docx4j relies on the databinding element to do its bit)
            foreach (Word.ContentControl cc in wrappingRepeatCC.Range.ContentControls)
            //foreach (Word.ContentControl cc in Globals.ThisAddIn.Application.ActiveDocument.ContentControls)
            {
                // XMLMapping.IsMapped returns false here,
                // in cases where it is mapped! So avoid using that.
                // (Could be a defect elsewhere .. check for usage)
                if (cc.XMLMapping!=null
                    && cc.XMLMapping.XPath!=null
                    && cc.XMLMapping.PrefixMappings!=null)
                {
                    foreach (KeyValuePair<string, string> entry in xpathChanges)
                    {
                        //log.Info("Comparing " + cc.XMLMapping.XPath + " with " + entry.Key);

                        if (cc.XMLMapping.XPath.Equals(entry.Key))
                        {
                            // matched, so replace
                            cc.XMLMapping.SetMapping(entry.Value, cc.XMLMapping.PrefixMappings, answersPart);
                            break;
                        }
                    }
                }
            }

        }


        bool isListed(Word.ContentControls ccs, Word.ContentControl ccx)
        {
            foreach (Word.ContentControl cca in ccs)
            {
                if (cca.ID.Equals(ccx.ID)) return true;
            }
            return false;
        }

        /// <summary>
        /// Is the content control inside any of the listed content controls?
        /// </summary>
        /// <param name="ccs"></param>
        /// <param name="ccx"></param>
        /// <returns></returns>
        bool isInside(List<Word.ContentControl> ccs, Word.ContentControl ccx)
        {
            Word.Range controlRange = ccx.Range;
            foreach (Word.ContentControl cca in ccs)
            {
                if (controlRange.InRange(cca.Range)) return true;
            }
            return false;
        }

    }
}
