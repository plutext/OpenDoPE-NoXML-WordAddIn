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
    partial class FormQA
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
            this.groupBoxA = new System.Windows.Forms.GroupBox();
            this.radioButtonMCNo = new System.Windows.Forms.RadioButton();
            this.radioButtonMCYes = new System.Windows.Forms.RadioButton();
            this.labelAType = new System.Windows.Forms.Label();
            this.buttonNext = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPageNewQ = new System.Windows.Forms.TabPage();
            this.controlQuestionCommon1 = new OpenDope_AnswerFormat.Controls.ControlQuestionCommon();
            this.groupBoxRepeat = new System.Windows.Forms.GroupBox();
            this.tabPageReuseQ = new System.Windows.Forms.TabPage();
            //this.buttonClone = new System.Windows.Forms.Button();
            this.buttonReuseOK = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.listBoxTypeFilter = new System.Windows.Forms.ListBox();
            this.checkBoxScope = new System.Windows.Forms.CheckBox();
            this.listBoxQuestions = new System.Windows.Forms.ListBox();
            this.controlQuestionVaryWhichRepeat1 = new OpenDope_AnswerFormat.Controls.ControlQuestionVaryWhichRepeat();
            this.groupBoxA.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPageNewQ.SuspendLayout();
            this.groupBoxRepeat.SuspendLayout();
            this.tabPageReuseQ.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxA
            // 
            this.groupBoxA.Controls.Add(this.radioButtonMCNo);
            this.groupBoxA.Controls.Add(this.radioButtonMCYes);
            this.groupBoxA.Controls.Add(this.labelAType);
            this.groupBoxA.Location = new System.Drawing.Point(20, 220);
            this.groupBoxA.Name = "groupBoxA";
            this.groupBoxA.Size = new System.Drawing.Size(367, 78);
            this.groupBoxA.TabIndex = 1;
            this.groupBoxA.TabStop = false;
            this.groupBoxA.Text = "Answer type";
            // 
            // radioButtonMCNo
            // 
            this.radioButtonMCNo.AutoSize = true;
            this.radioButtonMCNo.Checked = true;
            this.radioButtonMCNo.Location = new System.Drawing.Point(108, 46);
            this.radioButtonMCNo.Name = "radioButtonMCNo";
            this.radioButtonMCNo.Size = new System.Drawing.Size(39, 17);
            this.radioButtonMCNo.TabIndex = 2;
            this.radioButtonMCNo.TabStop = true;
            this.radioButtonMCNo.Text = "No";
            this.radioButtonMCNo.UseVisualStyleBackColor = true;
            // 
            // radioButtonMCYes
            // 
            this.radioButtonMCYes.AutoSize = true;
            this.radioButtonMCYes.Location = new System.Drawing.Point(108, 22);
            this.radioButtonMCYes.Name = "radioButtonMCYes";
            this.radioButtonMCYes.Size = new System.Drawing.Size(43, 17);
            this.radioButtonMCYes.TabIndex = 1;
            this.radioButtonMCYes.TabStop = true;
            this.radioButtonMCYes.Text = "Yes";
            this.radioButtonMCYes.UseVisualStyleBackColor = true;
            // 
            // labelAType
            // 
            this.labelAType.AutoSize = true;
            this.labelAType.Location = new System.Drawing.Point(22, 22);
            this.labelAType.Name = "labelAType";
            this.labelAType.Size = new System.Drawing.Size(85, 13);
            this.labelAType.TabIndex = 0;
            this.labelAType.Text = "Multiple Choice?";
            // 
            // buttonNext
            // 
            this.buttonNext.Location = new System.Drawing.Point(35, 459);
            this.buttonNext.Name = "buttonNext";
            this.buttonNext.Size = new System.Drawing.Size(75, 23);
            this.buttonNext.TabIndex = 2;
            this.buttonNext.Text = "Next ..";
            this.buttonNext.UseVisualStyleBackColor = true;
            this.buttonNext.Click += new System.EventHandler(this.buttonNext_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(155, 459);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 3;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPageNewQ);
            this.tabControl1.Controls.Add(this.tabPageReuseQ);
            this.tabControl1.Location = new System.Drawing.Point(12, 22);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(610, 528);
            this.tabControl1.TabIndex = 5;
            // 
            // tabPageNewQ
            // 
            this.tabPageNewQ.Controls.Add(this.controlQuestionCommon1);
            this.tabPageNewQ.Controls.Add(this.groupBoxRepeat);
            this.tabPageNewQ.Controls.Add(this.buttonNext);
            this.tabPageNewQ.Controls.Add(this.buttonCancel);
            this.tabPageNewQ.Controls.Add(this.groupBoxA);
            this.tabPageNewQ.Location = new System.Drawing.Point(4, 22);
            this.tabPageNewQ.Name = "tabPageNewQ";
            this.tabPageNewQ.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageNewQ.Size = new System.Drawing.Size(602, 502);
            this.tabPageNewQ.TabIndex = 0;
            this.tabPageNewQ.Text = "New";
            this.tabPageNewQ.UseVisualStyleBackColor = true;
            // 
            // controlQuestionCommon1
            // 
            this.controlQuestionCommon1.Location = new System.Drawing.Point(20, 5);
            this.controlQuestionCommon1.Name = "controlQuestionCommon1";
            this.controlQuestionCommon1.Size = new System.Drawing.Size(375, 208);
            this.controlQuestionCommon1.TabIndex = 8;
            // 
            // groupBoxRepeat
            // 
            this.groupBoxRepeat.Controls.Add(this.controlQuestionVaryWhichRepeat1);
            this.groupBoxRepeat.Location = new System.Drawing.Point(20, 317);
            this.groupBoxRepeat.Name = "groupBoxRepeat";
            this.groupBoxRepeat.Size = new System.Drawing.Size(367, 123);
            this.groupBoxRepeat.TabIndex = 7;
            this.groupBoxRepeat.TabStop = false;
            this.groupBoxRepeat.Text = "Ask for each Repeat?";
            // 
            // tabPageReuseQ
            // 
            //this.tabPageReuseQ.Controls.Add(this.buttonClone);
            this.tabPageReuseQ.Controls.Add(this.buttonReuseOK);
            this.tabPageReuseQ.Controls.Add(this.button2);
            this.tabPageReuseQ.Controls.Add(this.groupBox1);
            this.tabPageReuseQ.Location = new System.Drawing.Point(4, 22);
            this.tabPageReuseQ.Name = "tabPageReuseQ";
            this.tabPageReuseQ.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageReuseQ.Size = new System.Drawing.Size(602, 502);
            this.tabPageReuseQ.TabIndex = 1;
            this.tabPageReuseQ.Text = "Re-use";
            this.tabPageReuseQ.UseVisualStyleBackColor = true;
            // 
            // buttonClone
            // 
            //this.buttonClone.Location = new System.Drawing.Point(118, 292);
            //this.buttonClone.Name = "buttonClone";
            //this.buttonClone.Size = new System.Drawing.Size(75, 23);
            //this.buttonClone.TabIndex = 7;
            //this.buttonClone.Text = "Copy ..";
            //this.buttonClone.UseVisualStyleBackColor = true;
            //this.buttonClone.Click += new System.EventHandler(this.buttonClone_Click);
            // 
            // buttonReuseOK
            // 
            this.buttonReuseOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonReuseOK.Location = new System.Drawing.Point(21, 292);
            this.buttonReuseOK.Name = "buttonReuseOK";
            this.buttonReuseOK.Size = new System.Drawing.Size(75, 23);
            this.buttonReuseOK.TabIndex = 4;
            this.buttonReuseOK.Text = "OK";
            this.buttonReuseOK.UseVisualStyleBackColor = true;
            this.buttonReuseOK.Click += new System.EventHandler(this.buttonReuseOK_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(313, 292);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 5;
            this.button2.Text = "Cancel";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // groupBox1 (reuse tab)
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.listBoxTypeFilter);
            this.groupBox1.Controls.Add(this.checkBoxScope);
            this.groupBox1.Controls.Add(this.listBoxQuestions);
            this.groupBox1.Location = new System.Drawing.Point(16, 29);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(548, 238);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Select question";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(23, 35);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 13);
            this.label1.TabIndex = 16;
            this.label1.Text = "Filter";
            // 
            // listBoxTypeFilter
            // 
            this.listBoxTypeFilter.FormattingEnabled = true;
            this.listBoxTypeFilter.Location = new System.Drawing.Point(23, 51);
            this.listBoxTypeFilter.Name = "listBoxTypeFilter";
            this.listBoxTypeFilter.Size = new System.Drawing.Size(95, 82);
            this.listBoxTypeFilter.TabIndex = 15;
            // 
            // checkBoxScope
            // 
            this.checkBoxScope.AutoSize = true;
            this.checkBoxScope.Location = new System.Drawing.Point(138, 204);
            this.checkBoxScope.Name = "checkBoxScope";
            this.checkBoxScope.Size = new System.Drawing.Size(97, 17);
            this.checkBoxScope.TabIndex = 14;
            this.checkBoxScope.Text = "Expand scope ";
            this.checkBoxScope.UseVisualStyleBackColor = true;
            // 
            // listBoxQuestions
            // 
            this.listBoxQuestions.FormattingEnabled = true;
            this.listBoxQuestions.Location = new System.Drawing.Point(138, 19);
            this.listBoxQuestions.Name = "listBoxQuestions";
            this.listBoxQuestions.Size = new System.Drawing.Size(381, 160);
            this.listBoxQuestions.TabIndex = 0;
            // 
            // controlQuestionVaryWhichRepeat1
            // 
            this.controlQuestionVaryWhichRepeat1.Location = new System.Drawing.Point(14, 21);
            this.controlQuestionVaryWhichRepeat1.Name = "controlQuestionVaryWhichRepeat1";
            this.controlQuestionVaryWhichRepeat1.Size = new System.Drawing.Size(335, 90);
            this.controlQuestionVaryWhichRepeat1.TabIndex = 0;
            // 
            // FormQA
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(664, 580);
            this.Controls.Add(this.tabControl1);
            this.Name = "FormQA";
            this.Text = "Q/A Setup";
            this.groupBoxA.ResumeLayout(false);
            this.groupBoxA.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPageNewQ.ResumeLayout(false);
            this.groupBoxRepeat.ResumeLayout(false);
            this.tabPageReuseQ.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }


        #endregion

        private System.Windows.Forms.GroupBox groupBoxA;
        private System.Windows.Forms.Button buttonNext;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.RadioButton radioButtonMCNo;
        private System.Windows.Forms.RadioButton radioButtonMCYes;
        private System.Windows.Forms.Label labelAType;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPageNewQ;
        private System.Windows.Forms.TabPage tabPageReuseQ;
        //private System.Windows.Forms.Button buttonClone;
        private System.Windows.Forms.Button buttonReuseOK;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBoxRepeat;
        private System.Windows.Forms.Label label1;

        private System.Windows.Forms.CheckBox checkBoxScope;
        private System.Windows.Forms.ListBox listBoxQuestions;
        private System.Windows.Forms.ListBox listBoxTypeFilter;

        private Controls.ControlQuestionCommon controlQuestionCommon1;
        public Controls.ControlQuestionVaryWhichRepeat controlQuestionVaryWhichRepeat1;
    }
}