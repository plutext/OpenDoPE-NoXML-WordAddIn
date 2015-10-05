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
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using NLog;

using Office = Microsoft.Office.Core;
using OpenDoPEModel;
using OpenDope_AnswerFormat.Helpers;
using Word = Microsoft.Office.Interop.Word;
using Microsoft.Office.Tools.Word.Extensions; // for VSTOObject

namespace OpenDope_AnswerFormat.Forms
{
    public partial class FormQuestionEdit : Form
    {

        static Logger log = LogManager.GetLogger("OpenDoPE_Wed");
        protected Word.ContentControl cc;
        protected Model model;

        protected Office.CustomXMLPart questionsPart;
        protected Office.CustomXMLPart answersPart;  // for sample answer
        private question q;
        protected questionnaire questionnaire;

        protected XPathsPartEntry xppe;
        private xpathsXpath xpathObj;

        public FormQuestionEdit(string questionId)
        {
            InitializeComponent();
            init1();

            // Get the question!
            q = questionnaire.getQuestion(questionId);

            populateQuestionTab();

            // TODO Behaviour in repeats

            initResponsesTab();
        }

        public FormQuestionEdit(question q)
        {
            InitializeComponent();
            init1();

            this.q = q;

            populateQuestionTab();

            // TODO Behaviour in repeats

            initResponsesTab();
        }

        private void init1()
        {
            FabDocxState fabDocxState = (FabDocxState)Globals.ThisAddIn.Application.ActiveDocument.GetVstoObject(Globals.Factory).Tag;
            this.model = fabDocxState.model;
            xppe = new XPathsPartEntry(model);

            this.questionsPart = model.questionsPart;
            questionnaire = new questionnaire();
            questionnaire.Deserialize(questionsPart.XML, out questionnaire);

            this.answersPart = model.answersPart;

        }

        private void populateQuestionTab()
        {

            xpathObj = xppe.getXPathByQuestionID(q.id);

            this.controlQuestionCommon1.populateControl(q);

            this.controlQuestionVaryWhichRepeat1.init(answersPart,
                 questionnaire,
                 q,
                 xppe,
                 new ConditionsPartEntry(model) );

            //this.controlQuestionVaryWhichRepeat1.treeViewRepeat.Visible = true;
            //this.controlQuestionVaryWhichRepeat1.treeViewRepeat.Update();
        }

        private void initResponsesTab() {

            if (q.response.Item is responseFixed) {
                groupBoxResponseFree.Hide();
                this.groupBoxResponseFixed.Location = new System.Drawing.Point(15, 22);
            }
            else
            {
                groupBoxResponseFixed.Hide();
            }

            this.tabPageQuestion.Size = new System.Drawing.Size(468, 453);
            this.tabPageResponse.Size = new System.Drawing.Size(468, 453);
            this.tabControl1.Size = new System.Drawing.Size(476, 479);
            this.ClientSize = new System.Drawing.Size(517, 560);

            // Get the sample answer from the answers part
            Office.CustomXMLNode node = answersPart.SelectSingleNode(xpathObj.dataBinding.xpath);

            if (q.response.Item is responseFixed)
            {
                this.controlQuestionResponsesFixed1.populateControl(xpathObj, q, node.Text);
            }
            else
            {
                //this.controlDataType1.FormDataType = this;
                //this.controlDataType1.populateControl(xpathObj, q.response.Item as responseFree, node.Text, q.hint);
                this.controlDataTypeMAIN1.controlDataType1.populateControl(xpathObj, q.response.Item as responseFree, node.Text, q.hint);
            }

        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            // First, check validity
            // .. controlQuestionCommon1
            if (!controlQuestionCommon1.isValid())
            {
                if (!this.controlQuestionCommon1.isValid())
                {
                    Mbox.ShowSimpleMsgBoxError("You need to enter the text of the question!");
                    DialogResult = DialogResult.None; // or use on closing event; see http://stackoverflow.com/questions/2499644/preventing-a-dialog-from-closing-in-the-buttons-click-event-handler
                    return;
                }
            }
            // .. controlQuestionVaryWhichRepeat1 
            // TODO?

            // .. responses
            if (q.response.Item is responseFixed)
            {
                if (!this.controlQuestionResponsesFixed1.isValid())
                {
                    DialogResult = DialogResult.None; 
                    return;
                }
            }
            else
            {
                if (!this.controlDataTypeMAIN1.controlDataType1.isValid())  // TODO implement
                {
                    Mbox.ShowSimpleMsgBoxError("Data invalid");
                    DialogResult = DialogResult.None;
                    return;
                }
            }

            // OK, write changes
            string questionTextOriginal = q.text;
            controlQuestionCommon1.populateQuestion(q);
            Office.CustomXMLNode node = answersPart.SelectSingleNode(xpathObj.dataBinding.xpath);
            bool dataTypeDateChange = false;
            if (q.response.Item is responseFixed)
            {
                this.controlQuestionResponsesFixed1.updateQuestionFromForm(xpathObj, q, node);

                // If a response value was changed, need to check condition integrity

                    // TODO
            }
            else
            {
                string typeExisting = xpathObj.type;

                this.controlDataTypeMAIN1.controlDataType1.updateQuestionFromForm(xpathObj, q, node);

                dataTypeDateChange = !(typeExisting.Equals(xpathObj.type));
            }

            // Save changes
            // .. questionsPart
            string result = questionnaire.Serialize();
            log.Info(result);
            CustomXmlUtilities.replaceXmlDoc(questionsPart, result);
            // .. xpaths
            xppe.save();


            if (!q.text.Equals(questionTextOriginal)
                || dataTypeDateChange)
            {
                // If we changed the question text, need to update this in CC titles

                ConditionsPartEntry cpe = new ConditionsPartEntry(model);
                foreach (Word.ContentControl ccx in Globals.ThisAddIn.Application.ActiveDocument.ContentControls)
                {
                    if (ccx.Tag.Contains("od:xpath"))
                    {
                        string thisID = (new TagData(ccx.Tag)).getXPathID();
                        if (thisID.Equals(xpathObj.id))
                        {
                            // Update CC title
                            ccx.Title = q.text;

                            if (dataTypeDateChange)
                            {
                                if (xpathObj.type.Equals("date"))
                                {
                                    // it is now a date
                                    ccx.Type = Word.WdContentControlType.wdContentControlDate;
                                    log.Info("converted plain text cc to date");
                                }
                                else
                                {
                                    // no longer a date
                                    ccx.Type = Word.WdContentControlType.wdContentControlText;
                                    log.Info("converted date cc to plain text");
                                }
                            }
                        }
                    }
                    else if (ccx.Tag.Contains("od:condition"))
                    {
                        string thisID = (new TagData(ccx.Tag)).getConditionID();
                        condition c = cpe.getConditionByID(thisID);
                        if (ConditionHelper.doesConditionUseQuestion(xppe, cpe.conditions, c, q.id))
                        {
                            log.Info("condition uses question " + q.id);

                            // Update CC title
                            // TODO

                        }
                    }
                }


            }



            // Vary with which?
            if (this.controlQuestionVaryWhichRepeat1.changed())
            {
                this.controlQuestionVaryWhichRepeat1.moveIfNecessary(q.id, xpathObj, answersPart);
            }


        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            // No worries, nothing to do
            // (as long as we didn't update anything along the way!)

        }
    }
}
