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
namespace OpenDope_AnswerFormat
{
    partial class FormResponses
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
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPageManual = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkBoxInsertControl = new System.Windows.Forms.CheckBox();
            this.checkBoxContentControl = new System.Windows.Forms.CheckBox();
            this.controlQuestionResponsesFixed1 = new OpenDope_AnswerFormat.Controls.ControlQuestionResponsesFixed();
            this.tabControl1.SuspendLayout();
            this.tabPageManual.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(29, 444);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 1;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(130, 444);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 2;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPageManual);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(468, 516);
            this.tabControl1.TabIndex = 4;
            // 
            // tabPageManual
            // 
            this.tabPageManual.Controls.Add(this.groupBox1);
            this.tabPageManual.Controls.Add(this.controlQuestionResponsesFixed1);
            this.tabPageManual.Controls.Add(this.buttonOK);
            this.tabPageManual.Controls.Add(this.buttonCancel);
            this.tabPageManual.Location = new System.Drawing.Point(4, 22);
            this.tabPageManual.Name = "tabPageManual";
            this.tabPageManual.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageManual.Size = new System.Drawing.Size(460, 490);
            this.tabPageManual.TabIndex = 0;
            this.tabPageManual.Text = "Manual";
            this.tabPageManual.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.checkBoxContentControl);
            this.groupBox1.Controls.Add(this.checkBoxInsertControl);
            this.groupBox1.Location = new System.Drawing.Point(19, 373);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(395, 54);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Content Control";
            // 
            // checkBoxInsertControl
            // 
            this.checkBoxInsertControl.AutoSize = true;
            this.checkBoxInsertControl.Location = new System.Drawing.Point(15, 24);
            this.checkBoxInsertControl.Name = "checkBoxInsertControl";
            this.checkBoxInsertControl.Size = new System.Drawing.Size(251, 17);
            this.checkBoxInsertControl.TabIndex = 0;
            this.checkBoxInsertControl.Text = "Add content control (show answer in document)";
            this.checkBoxInsertControl.UseVisualStyleBackColor = true;
            // 
            // checkBoxContentControl
            // 
            this.checkBoxContentControl.AutoSize = true;
            this.checkBoxContentControl.Enabled = false;
            this.checkBoxContentControl.Location = new System.Drawing.Point(272, 24);
            this.checkBoxContentControl.Name = "checkBoxContentControl";
            this.checkBoxContentControl.Size = new System.Drawing.Size(109, 17);
            this.checkBoxContentControl.TabIndex = 1;
            this.checkBoxContentControl.Text = "Add as checkbox";
            this.checkBoxContentControl.UseVisualStyleBackColor = true;
            this.checkBoxContentControl.CheckedChanged += new System.EventHandler(this.checkBoxContentControl_CheckedChanged);
            // 
            // controlQuestionResponsesFixed1
            // 
            this.controlQuestionResponsesFixed1.Location = new System.Drawing.Point(8, 9);
            this.controlQuestionResponsesFixed1.Name = "controlQuestionResponsesFixed1";
            this.controlQuestionResponsesFixed1.Size = new System.Drawing.Size(450, 429);
            this.controlQuestionResponsesFixed1.TabIndex = 3;
            // 
            // FormResponses
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(492, 540);
            this.Controls.Add(this.tabControl1);
            this.Name = "FormResponses";
            this.Text = "Add Multiple Choice Responses";
            this.tabControl1.ResumeLayout(false);
            this.tabPageManual.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPageManual;
        public Controls.ControlQuestionResponsesFixed controlQuestionResponsesFixed1;
        private System.Windows.Forms.GroupBox groupBox1;
        public System.Windows.Forms.CheckBox checkBoxInsertControl;
        public System.Windows.Forms.CheckBox checkBoxContentControl;
    }
}