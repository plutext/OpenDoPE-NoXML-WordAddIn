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
    /// Invoked when a condition CC is added.
    /// We need to check that the relevant variable is OK here.
    /// </summary>
    class ContentControlNewConditionCheck : ContentControlHandlerAbstract
    {
        //static Logger log = LogManager.GetLogger("OpenDoPE_Wed");

        public ContentControlNewConditionCheck()//Word.ContentControl copiedCC)
            : base()//copiedCC)
        {
        }

        public void checkAnswerAncestry(string xpathID) {
            handleXPath(xpathID, true);
        }

    }
}
