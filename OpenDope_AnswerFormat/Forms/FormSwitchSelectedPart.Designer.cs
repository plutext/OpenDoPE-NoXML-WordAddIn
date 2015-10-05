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
namespace XmlMappingTaskPane.Forms
{
    partial class FormSwitchSelectedPart
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.controlPartList = new XmlMappingTaskPane.Controls.ControlPartList();
            this.buttonHide = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // controlPartList1
            // 
            this.controlPartList.Location = new System.Drawing.Point(12, 21);
            this.controlPartList.Name = "controlPartList";
            this.controlPartList.Size = new System.Drawing.Size(243, 43);
            this.controlPartList.TabIndex = 0;
            // 
            // buttonHide
            // 
            this.buttonHide.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonHide.Location = new System.Drawing.Point(180, 88);
            this.buttonHide.Name = "buttonHide";
            this.buttonHide.Size = new System.Drawing.Size(75, 23);
            this.buttonHide.TabIndex = 2;
            this.buttonHide.Text = "Hide";
            this.buttonHide.UseVisualStyleBackColor = true;
            this.buttonHide.Click +=new System.EventHandler(buttonHide_Click);
            // 
            // FormSwitchSelectedPart
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(268, 125);
            this.Controls.Add(this.buttonHide);
            this.Controls.Add(this.controlPartList);
            this.Name = "FormSwitchSelectedPart";
            this.Text = "Select XML part";
            this.ResumeLayout(false);
            this.FormClosing +=new System.Windows.Forms.FormClosingEventHandler(FormSwitchSelectedPart_FormClosing);

        }

        #endregion

        public Controls.ControlPartList controlPartList { get; set; }
        private System.Windows.Forms.Button buttonHide;

    }
}