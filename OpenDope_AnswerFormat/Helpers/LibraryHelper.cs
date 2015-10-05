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

//using OpenDope_AnswerFormat.Helpers;

using Office = Microsoft.Office.Core;
using OpenDoPEModel;
using Word = Microsoft.Office.Interop.Word;
using Microsoft.Office.Tools.Word.Extensions; // for VSTOObject

namespace OpenDope_AnswerFormat.Helpers
{
    /// <summary>
    /// Used for handling logic when both saving to, and copying from,
    /// the library.
    /// </summary>
    class LibraryHelper
    {
        protected static Logger log = LogManager.GetLogger("LibraryHelper");

        private XPathsPartEntry srcXppe;
        private Office.CustomXMLPart srcXPathsPart;

        private ConditionsPartEntry srcCpe;
        private Office.CustomXMLPart srcConditionsPart;

        private Office.CustomXMLPart srcQuestionsPart;
        //protected question q;
        private questionnaire srcQuestionnaire;

        private Office.CustomXMLPart srcAnswersPart;
        private answers srcAnswers;

        public LibraryHelper(Model srcModel)
        {
            srcXppe = new XPathsPartEntry(srcModel); // used to get entries
            this.srcXPathsPart = srcModel.xpathsPart;

            srcCpe = new ConditionsPartEntry(srcModel);
            this.srcConditionsPart = srcModel.conditionsPart;

            this.srcQuestionsPart = srcModel.questionsPart;
            srcQuestionnaire = new questionnaire();
            questionnaire.Deserialize(srcQuestionsPart.XML, out srcQuestionnaire);

            srcAnswersPart = srcModel.answersPart;
            srcAnswers = new answers();
            answers.Deserialize(srcAnswersPart.XML, out srcAnswers);
        }

        // These are HashSets, in order to avoid duplicates, without using a Map/Dictionary
        HashSet<xpathsXpath> BBxpaths = new HashSet<xpathsXpath>();
        HashSet<question> BBquestions = new HashSet<question>();
        HashSet<answer> BBanswers = new HashSet<answer>();
        HashSet<repeat> BBrepeats = new HashSet<repeat>();
        HashSet<condition> BBconditions = new HashSet<condition>();

        /// <summary>
        /// Add od:source=library, so later, we'll be able
        /// to use this to suppress any action in content control
        /// after add event, which fires before building block 
        /// insert.
        /// </summary>
        /// <param name="range"></param>
        public void TagsSourceAdd(Word.Range range)
        {
            foreach (Word.ContentControl cc in range.ContentControls)
            {
                if (cc.Tag == null) continue;

                TagData td = new TagData(cc.Tag);
                td.set("od:source", "library");
                cc.Tag = td.asQueryString();
            }
        }

        public void TagsSourceRemove(Word.Range range)
        {
            foreach (Word.ContentControl cc in range.ContentControls)
            {
                if (cc.Tag == null) continue;

                TagData td = new TagData(cc.Tag);
                td.remove("od:source");
                cc.Tag = td.asQueryString();
            }
        }

        public void identifyLogic(Word.Range range)
        {
            foreach (Word.ContentControl cc in range.ContentControls)
            {
                if (cc.Tag == null)
                {
                    // Its just some content control we don't care about
                    continue;
                }

                TagData td = new TagData(cc.Tag);
                if (cc.Tag.Contains("od:xpath"))
                {
                    xpathsXpath xp = srcXppe.getXPathByID( td.getXPathID() );
                    identifyLogicInXPath(xp, range, cc);
                }
                else if (cc.Tag.Contains("od:repeat"))
                {
                    testOldestRepeatIncluded(range, cc);

                    xpathsXpath xp = srcXppe.getXPathByID(td.getRepeatID());

                    // TODO: Clone, so we can write source to just the clone
                    //xp.Serialize
                    //xpathsXpath.Deserialize
                    // TODO consider cloning issue further.  I've implemented it
                    // below for xpaths, but since we're throwing the objects
                    // away, it doesn't matter that we're altering them?

                    BBxpaths.Add(xp);

                    string questionID = xp.questionID;
                    question q = srcQuestionnaire.getQuestion(questionID);
                    BBquestions.Add(q);

                    // Answer - just need to do this for the outermost repeat
                    Word.ContentControl oldestRepeat = RepeatHelper.getOldestRepeatAncestor(cc);
                    if (oldestRepeat.ID.Equals(cc.ID))
                    {
                        foreach (object o in srcAnswers.Items)
                        {
                            if (o is repeat)
                            {
                                repeat a = (repeat)o;
                                if (a.qref.Equals(questionID)) // question ID == answer ID
                                {
                                    BBrepeats.Add(a);
                                    log.Debug("Added outermost repeat!");
                                    break;
                                }
                            }
                        }
                    }
                }
                else if (cc.Tag.Contains("od:condition"))
                {
                    // Add the condition
                    condition c = srcCpe.getConditionByID(td.getConditionID());
                    BBconditions.Add(c);

                    // Find and add questions and xpaths used within it
                    //List<xpathsXpath> xpaths = ConditionsPartEntry.getXPathsUsedInCondition(c, srcXppe);
                    List<xpathsXpath> xpaths = new List<xpathsXpath>();
                    c.listXPaths(xpaths, srcCpe.conditions, srcXppe.getXPaths());

                    foreach (xpathsXpath xp in xpaths)
                    {
                        identifyLogicInXPath(xp, range, cc);
                    }
                }
            }
        }

        private void identifyLogicInXPath(xpathsXpath xp, Word.Range range, Word.ContentControl cc)
        {
            BBxpaths.Add(xp.Clone() );

            // From Java QuestionsInUse
			String xpathVal = xp.dataBinding.xpath;

            string questionID=null;
			if (xpathVal.StartsWith("/")) {
				// simple
                questionID = xp.questionID;
                question q = srcQuestionnaire.getQuestion(questionID);
                BBquestions.Add(q);
			} else if (xpathVal.Contains("position()")) {
                return;
			} else {
				log.Debug(xpathVal);
                questionID = xpathVal.Substring(xpathVal.LastIndexOf("@id") + 5);
//						System.out.println("Got qid: " + qid);
                questionID = questionID.Substring(0, questionID.IndexOf("'"));
//						System.out.println("Got qid: " + qid);

                log.Debug("question " + questionID 
						+ " is in use via condition " );

                question q = srcQuestionnaire.getQuestion(questionID);
                BBquestions.Add(q);
	
			}


            // Answer
            // If its not top level, we'll have it already when we add repeat
            // Therefore here, we only look at the top level ones.
            //Office.CustomXMLNode destination = answersPart.SelectSingleNode(xp.dataBinding.xpath);
            answer a = null;
            foreach (object o in srcAnswers.Items)
            {
                if (o is answer)
                {
                    a = (answer)o;
                    if (a.id.Equals(questionID)) // question ID == answer ID
                    {
                        BBanswers.Add(a);
                        break;
                    }
                }
            }
            if (a == null)
            {
                // Not found at top level, so must be repeated
                testOldestRepeatIncluded(range, cc);
            }

        }

        private void testOldestRepeatIncluded(Word.Range range, Word.ContentControl cc)
        {
            Word.ContentControl oldestRepeat = RepeatHelper.getOldestRepeatAncestor(cc);
            if (oldestRepeat == null)
            {
                log.Debug("Not in repeat.");
                return;
            }

            if (oldestRepeat.Range.InRange(range))
            {
                log.Debug("oldest repeat is included.");
            }
            else
            {
                throw new BuildingBlockLogicException("Outermost repeat must be included in selection.");
            }
        }

        /// <summary>
        /// We've already written the correct binding part id, in our xpaths.  We need to do it in the cc'sas well.
        /// </summary>
        /// <param name="range"></param>
        /// <param name="answersPart"></param>
        public void updateBindings(Word.Range range, Office.CustomXMLPart answersPart)
        {
            foreach (Word.ContentControl cc in range.ContentControls)
            {
                if (cc.Tag == null)
                {
                    // Its just some content control we don't care about
                    continue;
                }

                TagData td = new TagData(cc.Tag);
                if (cc.Tag.Contains("od:xpath"))
                {
                    xpathsXpath xp = srcXppe.getXPathByID(td.getXPathID());

                    cc.XMLMapping.SetMapping(xp.dataBinding.xpath, "xmlns:oda='http://opendope.org/answers'", 
                        answersPart);
                }
            }
        }

        private void injectLogicXPath(XPathsPartEntry targetXppe, xpathsXpath xp, string sourceAttr, string answersPartStoreID)
        {
            if (sourceAttr != null) xp.source = sourceAttr;

            if (answersPartStoreID != null)
            {
                xp.dataBinding.storeItemID = answersPartStoreID;
            }

            targetXppe.getXPaths().xpath.Add(xp);  // this is a HashSet, so we're overrwriting, not adding :-)
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="setSourceAttr">When saving a building block, we want to write the ID of the source part</param>
        /// <param name="setBindingStore">When re-using a building block, storeItemID to write to the xpath; otherwise null</param>
        /// <param name="overwriteExisting">If re-using a building block back into original source, we want to skip silently.
        /// When going the other way, we want to overwrite the logic in any existing building block (since
        /// it may have been updated).</param>
        public void injectLogic(Model targetModel, bool setSourceAttr, bool setBindingStore, bool overwriteExisting)
        {
            //Model targetModel = Model.ModelFactory(target);

            // XPaths
            XPathsPartEntry targetXppe = new XPathsPartEntry(targetModel);
            string sourceAttr = null;
            if (setSourceAttr)
            {
                sourceAttr = srcXPathsPart.Id;
            }
            string answersPartStoreID = null;
            if (setBindingStore)
            {
                answersPartStoreID = targetModel.answersPart.Id;
            }

            // .. add em
            foreach (xpathsXpath xp in BBxpaths)
            {
                xpathsXpath existing = targetXppe.getXPathByID(xp.id);
                if (existing == null)
                {
                    injectLogicXPath(targetXppe, xp, sourceAttr, answersPartStoreID);
                } else
                {
                    // Does it come from this doc?
                    //log.Debug("xp.source: " + xp.source);
                    //log.Debug("existing.source: " + existing.source);
                    //log.Debug("targetModel.xpathsPart.Id: " + targetModel.xpathsPart.Id);
                    if (xp.source != null && xp.source.Equals(targetModel.xpathsPart.Id))
                    {
                        // yes ..
                        if (overwriteExisting)
                        {
                            injectLogicXPath(targetXppe, xp, sourceAttr, answersPartStoreID);
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else if (xp.source != null && 
                        existing.source != null &&
                        xp.source.Equals(existing.source))
                    {
                        // It has already been copied in.
                        // so don't do it again, whether we're copying to template
                        // (could go either way, but for now, only update from original source),
                        // or into docx
                        continue;

                    } else
                    {
                        // Yikes! ID collision
                        throw new BuildingBlockLogicException("XPath with ID " + xp.id + " is already present.");
                    }
                }
            }

            // Questions
            questionnaire targetQuestionnaire = new questionnaire();
            questionnaire.Deserialize(targetModel.questionsPart.XML, out targetQuestionnaire);
            // .. add em
            foreach (question q in BBquestions)
            {
                question existing = targetQuestionnaire.getQuestion(q.id);
                if (existing == null)
                {
                    targetQuestionnaire.questions.Add(q);
                    if (setSourceAttr)
                    {
                        q.source = srcQuestionsPart.Id;
                    }                    
                }
                else
                {
                    // Does it come from this doc?
                    //log.Debug("q.source: " + q.source);
                    //log.Debug("existing.source: " + existing.source);
                    //log.Debug("targetModel.questionsPart.Id: " + targetModel.questionsPart.Id);
                    if (q.source != null && q.source.Equals(targetModel.questionsPart.Id))
                    {
                        // yes ..
                        if (overwriteExisting)
                        {
                            targetQuestionnaire.questions.Add(q); // this is a HashSet, so we're overrwriting, not adding :-)
                            if (setSourceAttr)
                            {
                                q.source = targetModel.questionsPart.Id;
                            }
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else if (q.source != null &&
                        existing.source != null &&
                        q.source.Equals(existing.source))
                    {
                        // It has already been copied in.
                        continue;
                    }
                    else
                    {
                        // Yikes! ID collision
                        throw new BuildingBlockLogicException("Question with ID " + q.id + " is already present.");
                    }
                }
            }

            // Answers
            answers targetAnswers = new answers();
            answers.Deserialize(targetModel.answersPart.XML, out targetAnswers);
            foreach (answer a in BBanswers)
            {
                answer existing = getAnswer(targetAnswers, a.id);
                if (existing == null)
                {
                    targetAnswers.Items.Add(a);
                    if (setSourceAttr)
                    {
                        a.source = srcAnswersPart.Id;
                    }                    
                }
                else
                {
                    // Does it come from this doc?
                    if (a.source != null && a.source.Equals(targetModel.answersPart.Id))
                    {
                        log.Debug("source is this part");
                        // yes ..
                        if (overwriteExisting)
                        {
                            log.Debug(".. and overwriting..");
                            targetAnswers.Items.Add(a); // this is a HashSet, so we're overrwriting, not adding :-)
                            if (setSourceAttr)
                            {
                                a.source = srcAnswersPart.Id;
                            }                    
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else if (a.source != null &&
                        existing.source != null &&
                        a.source.Equals(existing.source))
                    {
                        // It has already been copied in.
                        log.Debug("this logic already present");
                        continue;
                    }
                    else
                    {
                        // Yikes! ID collision
                        throw new BuildingBlockLogicException("Answer with ID " + a.id + " from different source is already present.");
                    }
                }
            }
            foreach (repeat r in BBrepeats)
            {
                repeat existing = getRepeat(targetAnswers, r.qref);
                if (existing == null)
                {
                    targetAnswers.Items.Add(r);
                    if (setSourceAttr)
                    {
                        r.source = srcAnswersPart.Id;
                    }
                }
                else
                {
                    // Does it come from this doc?
                    if (r.source != null && r.source.Equals(targetModel.answersPart.Id))
                    {
                        // yes ..
                        if (overwriteExisting)
                        {
                            targetAnswers.Items.Add(r); // this is a HashSet, so we're overrwriting, not adding :-)
                            if (setSourceAttr)
                            {
                                r.source = srcAnswersPart.Id;
                            }
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else if (r.source != null &&
                        existing.source != null &&
                        r.source.Equals(existing.source))
                    {
                        // It has already been copied in.
                        continue;
                    }
                    else
                    {
                        // Yikes! ID collision
                        throw new BuildingBlockLogicException("Answer with ID " + r.qref + " is already present.");
                    }
                }
            }

            // Conditions
            conditions targetConditions = new conditions();
            conditions.Deserialize(targetModel.conditionsPart.XML, out targetConditions);
            foreach (condition c in BBconditions)
            {
                condition existing = getCondition(targetConditions, c.id);
                if (existing == null)
                {
                    targetConditions.condition.Add(c);
                    if (setSourceAttr)
                    {
                        c.source = srcConditionsPart.Id;
                    }
                }
                else
                {
                    // Does it come from this doc?
                    if (c.source != null && c.source.Equals(targetModel.conditionsPart.Id))
                    {
                        // yes ..
                        if (overwriteExisting)
                        {
                            targetConditions.condition.Add(c);  // this is a HashSet, so we're overrwriting, not adding :-)
                            if (setSourceAttr)
                            {
                                c.source = targetModel.conditionsPart.Id;
                            }
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else if (c.source != null &&
                        existing.source != null &&
                        c.source.Equals(existing.source))
                    {
                        // It has already been copied in.
                        continue;
                    }
                    else
                    {
                        // Yikes! ID collision
                        throw new BuildingBlockLogicException("Condition with ID " + c.id + " is already present.");
                    }
                }
            }


            // .. save: we only save if there have been no ID collisions.
            // Otherwise, we will have aborted with a BuildingBlockLogicException
            targetXppe.save();
            CustomXmlUtilities.replaceXmlDoc(targetModel.questionsPart, targetQuestionnaire.Serialize());
            CustomXmlUtilities.replaceXmlDoc(targetModel.conditionsPart, targetConditions.Serialize());
            CustomXmlUtilities.replaceXmlDoc(targetModel.answersPart, targetAnswers.Serialize());


        }


        public answer getAnswer(answers targetAnswers, String id)
        {
            log.Debug("looking for existing answer: " + id);
            foreach (object o in targetAnswers.Items)
            {
                if (o is answer)
                {
                    answer a = (answer)o;
                    log.Debug("answer " + a.id);
                    if (a.id.Equals(id)) // question ID == answer ID
                    {
                        return a;
                    }
                }

            }
            return null;
        }
        public repeat getRepeat(answers targetAnswers, String id)
        {
            foreach (object o in targetAnswers.Items)
            {
                if (o is repeat)
                {
                    repeat r = (repeat)o;
                    if (r.qref.Equals(id)) // question ID == answer ID
                    {
                        return r;
                    }
                }

            }
            return null;
        }

        public condition getCondition(conditions targetConditions, String id)
        {

            foreach (condition xx in targetConditions.condition)
            {
                if (xx.id.Equals(id))
                {
                    return xx;
                }
            }
            return null;
        }

    }



    public class BuildingBlockLogicException : Exception {

        public BuildingBlockLogicException(string message) : base(message) { }

    }

}
