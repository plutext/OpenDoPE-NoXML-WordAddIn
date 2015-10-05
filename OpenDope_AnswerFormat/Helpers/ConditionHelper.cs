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

using OpenDoPEModel;

namespace OpenDope_AnswerFormat.Helpers
{
    public class ConditionHelper
    {
        public static bool doesConditionUseQuestion(XPathsPartEntry xppe, conditions conditions, condition c, string questionID)
        {
            //List<xpathsXpath> xpaths = ConditionsPartEntry.getXPathsUsedInCondition(c, xppe);
            List<xpathsXpath> xpaths = new List<xpathsXpath>();
            c.listXPaths(xpaths, conditions, xppe.getXPaths());


            foreach (xpathsXpath xpathObj in xpaths)
            {

                String xpathVal = xpathObj.dataBinding.xpath;

                if (xpathVal.StartsWith("/"))
                {
                    // simple
                    //System.out.println("question " + xpathObj.getQuestionID() 
                    //        + " is in use via boolean condition " + conditionId);
                    if (xpathObj.questionID.Equals(questionID))
                    {
                        return true;
                    }

                }
                else if (xpathVal.Contains("position"))
                {
                    continue;
                }
                else
                {
                    //System.out.println(xpathVal);

                    String qid = xpathVal.Substring(
                        xpathVal.LastIndexOf("@id") + 5);
                    //						System.out.println("Got qid: " + qid);
                    qid = qid.Substring(0, qid.IndexOf("'"));
                    //						System.out.println("Got qid: " + qid);

                    //System.out.println("question " + qid 
                    //        + " is in use via condition " + conditionId);

                    if (qid.Equals(questionID))
                    {
                        return true;
                    }

                }
            }
            return false;

        }

        //private bool doesConditionUseQuestion(condition c, string questionID)
        //{
        //    if (c.Item is xpathref)
        //    {
        //        xpathref xref = (xpathref)c.Item;
        //        xpathsXpath xp = xppe.getXPathByID(xref.id);
        //        if (xp.questionID != null
        //            && xp.questionID.Equals(questionID))
        //        {
        //            return true;
        //        }
        //    }
        //    else if (c.Item is not)
        //    {
        //        not notObj = (not)c.Item;
        //        object notContents = notObj.Item;
        //        if (notContents is xpathref)
        //        {
        //            xpathref xref = (xpathref)notContents;
        //            xpathsXpath xp = xppe.getXPathByID(xref.id);
        //            if (xp.questionID != null
        //                && xp.questionID.Equals(questionID))
        //            {
        //                return true;
        //            }
        //        }
        //        else
        //        {
        //            log.Warn("TODO CCHandler - add support for condition not contents " + c.Item.GetType().Name);
        //            return false;
        //        }
        //    }
        //    else
        //    {
        //        log.Warn("TODO CCHandler - add support for condition " + c.Item.GetType().Name);
        //        return false;
        //    }
        //    return false;
        //}


    }
}
