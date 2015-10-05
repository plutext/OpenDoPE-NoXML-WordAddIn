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

using OpenDoPEModel;

using NLog;
using Word = Microsoft.Office.Interop.Word;
using Office = Microsoft.Office.Core;


namespace OpenDope_AnswerFormat
{
    public partial class FormMoveQuestion : Form
    {
        static Logger log = LogManager.GetLogger("OpenDoPE_Wed");


        public FormMoveQuestion(Office.CustomXMLPart answersPart,
            List<Word.ContentControl> relevantRepeats,
            questionnaire questionnaire, 
            string questionID,
            XPathsPartEntry xppe)
        {
            InitializeComponent();

            this.textBoxQuestion.Text = questionnaire.getQuestion(questionID).text;

            this.controlQuestionVaryWhichRepeat1.init(answersPart,
                relevantRepeats,
                questionnaire, questionID,
                xppe);


        }

        public void moveIfNecessary(string questionID, xpathsXpath xp,
            Office.CustomXMLPart answersPart)
        {
            controlQuestionVaryWhichRepeat1.moveIfNecessary(questionID, xp, answersPart);
        }

        //public string getVaryingRepeat()
        //{
        //    return controlQuestionVaryWhichRepeat1.getVaryingRepeat();
        //}

        public bool OkAsis()
        {
            return controlQuestionVaryWhichRepeat1.OkAsis;
        }
    }
}
