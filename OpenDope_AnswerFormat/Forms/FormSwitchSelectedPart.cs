/*
 * (c) Copyright Plutext Pty Ltd, 2012
 * 
 * All rights reserved.
 * 
 * This source code is the proprietary information of Plutext
 * Pty Ltd, and must be kept confidential.
 * 
 * You may use, modify and distribute this source code only
 * as provided in your license agreement with Plutext.
 * 
 * If you do not have a license agreement with Plutext:
 * 
 * (i) you must return all copies of this source code to Plutext, 
 * or destroy it.  
 * 
 * (ii) under no circumstances may you use, modify or distribute 
 * this source code.
 * 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Office.Tools;

namespace XmlMappingTaskPane.Forms
{
    public partial class FormSwitchSelectedPart : Form
    {
        public FormSwitchSelectedPart()
        {
            InitializeComponent();

            //CustomTaskPane ctpPaneForThisWindow = Utilities.FindTaskPaneForCurrentWindow();
            //Controls.ControlMain ccm = (Controls.ControlMain)ctpPaneForThisWindow.Control;

            this.controlPartList.controlMain = ccm;
        }

        private Controls.ControlMain _ccm;
        public Controls.ControlMain ccm
        {
            get { return _ccm; }
            set { _ccm = value; }
        }

        /// <summary>
        /// Just want to hide the form.  
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormSwitchSelectedPart_FormClosing(object sender, FormClosingEventArgs e)
        {
            //If the user is simply hitting the X in the window the form hides, 
            // if any thing else such as task manager, application.exit, 
            // or windows shutdown the form is properly closed, since the 
            // return statement would be executed.
            // From http://stackoverflow.com/questions/2021681/c-sharp-hide-form-instead-of-close
            if (e.CloseReason != CloseReason.UserClosing) return;
            e.Cancel = true; // this cancels the close event.
            Hide(); 
        }

        private void buttonHide_Click(object sender, System.EventArgs e)
        {
            Hide();
        }
    }
}

