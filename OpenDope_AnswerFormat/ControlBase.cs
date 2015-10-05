//Copyright (c) Microsoft Corporation.  All rights reserved.
using System.Globalization;
using System.Windows.Forms;
using Office = Microsoft.Office.Core;
using Word = Microsoft.Office.Interop.Word;

namespace OpenDope_AnswerFormat
{
    public partial class ControlBase {

        #region Dialog box methods

        /// <summary>
        /// Show a standard error dialog box, parented to the current control.
        /// </summary>
        /// <param name="Message">A string specifying the text to be shown in the dialog box .</param>
        public static void ShowErrorMessage(string Message)
        {
            GenericMessageBox.Show( Message, Properties.Resources.DialogTitle, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, (MessageBoxOptions)0);
        }

        /// <summary>
        /// Show a standard "yes or no" dialog box, parented to the current control.
        /// </summary>
        /// <param name="Message">A string specifying the text to be shown in the dialog box .</param>
        /// <returns>A DialogResult specifying the button selected by the user.</returns>
        public static DialogResult ShowYesNoMessage(string Message)
        {
            return GenericMessageBox.Show( Message, Properties.Resources.DialogTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, (MessageBoxOptions)0);
        }

        #endregion
    }

    /// <summary>
    /// Specifies a message box that flips based on the reading order of the UI.
    /// </summary>
    public static class GenericMessageBox
    {
        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, MessageBoxOptions options)
        {
            return MessageBox.Show(text, caption, buttons, icon, defaultButton, options);
        }
        
    }
}
