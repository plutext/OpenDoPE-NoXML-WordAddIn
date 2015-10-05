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
    partial class FormRepeat
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
            this.buttonOK1 = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.groupBoxQ = new System.Windows.Forms.GroupBox();
            this.textBoxQuestionText = new System.Windows.Forms.TextBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPageNewQ = new System.Windows.Forms.TabPage();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.radioButton3 = new System.Windows.Forms.RadioButton();
            this.radioButton4 = new System.Windows.Forms.RadioButton();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.textBoxRangeStep = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.textBoxMax = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.textBoxDefault = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxMin = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.textBoxHelp = new System.Windows.Forms.TextBox();
            this.textBoxHint = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tabPageReuseQ = new System.Windows.Forms.TabPage();
            this.buttonReuseOK = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.comboBoxRepeatNames = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tabPageCalc = new System.Windows.Forms.TabPage();
            this.checkBoxAppearanceCompact = new System.Windows.Forms.CheckBox();
            this.groupBoxQ.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPageNewQ.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tabPageReuseQ.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonOK1
            // 
            this.buttonOK1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK1.Location = new System.Drawing.Point(50, 425);
            this.buttonOK1.Name = "buttonOK1";
            this.buttonOK1.Size = new System.Drawing.Size(75, 23);
            this.buttonOK1.TabIndex = 2;
            this.buttonOK1.Text = "OK";
            this.buttonOK1.UseVisualStyleBackColor = true;
            this.buttonOK1.Click += new System.EventHandler(this.buttonNext_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(170, 425);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 3;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // groupBoxQ
            // 
            this.groupBoxQ.Controls.Add(this.textBoxQuestionText);
            this.groupBoxQ.Location = new System.Drawing.Point(20, 14);
            this.groupBoxQ.Name = "groupBoxQ";
            this.groupBoxQ.Size = new System.Drawing.Size(367, 59);
            this.groupBoxQ.TabIndex = 4;
            this.groupBoxQ.TabStop = false;
            this.groupBoxQ.Text = "Repeat name";
            // 
            // textBoxQuestionText
            // 
            this.textBoxQuestionText.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxQuestionText.Location = new System.Drawing.Point(7, 20);
            this.textBoxQuestionText.Name = "textBoxQuestionText";
            this.textBoxQuestionText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxQuestionText.Size = new System.Drawing.Size(171, 21);
            this.textBoxQuestionText.TabIndex = 0;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPageNewQ);
            this.tabControl1.Controls.Add(this.tabPageReuseQ);
            this.tabControl1.Controls.Add(this.tabPageCalc);
            this.tabControl1.Location = new System.Drawing.Point(12, 22);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(464, 498);
            this.tabControl1.TabIndex = 5;
            // 
            // tabPageNewQ
            // 
            this.tabPageNewQ.Controls.Add(this.groupBox4);
            this.tabPageNewQ.Controls.Add(this.groupBox3);
            this.tabPageNewQ.Controls.Add(this.groupBox2);
            this.tabPageNewQ.Controls.Add(this.buttonOK1);
            this.tabPageNewQ.Controls.Add(this.groupBoxQ);
            this.tabPageNewQ.Controls.Add(this.buttonCancel);
            this.tabPageNewQ.Location = new System.Drawing.Point(4, 22);
            this.tabPageNewQ.Name = "tabPageNewQ";
            this.tabPageNewQ.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageNewQ.Size = new System.Drawing.Size(456, 472);
            this.tabPageNewQ.TabIndex = 0;
            this.tabPageNewQ.Text = "New";
            this.tabPageNewQ.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.checkBoxAppearanceCompact);
            this.groupBox4.Controls.Add(this.groupBox5);
            this.groupBox4.Location = new System.Drawing.Point(21, 338);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(366, 50);
            this.groupBox4.TabIndex = 8;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Interview appearance";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.radioButton3);
            this.groupBox5.Controls.Add(this.radioButton4);
            this.groupBox5.Location = new System.Drawing.Point(0, 63);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(396, 50);
            this.groupBox5.TabIndex = 4;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Allow user to check more than one response";
            // 
            // radioButton3
            // 
            this.radioButton3.AutoSize = true;
            this.radioButton3.Checked = true;
            this.radioButton3.Location = new System.Drawing.Point(177, 20);
            this.radioButton3.Name = "radioButton3";
            this.radioButton3.Size = new System.Drawing.Size(39, 17);
            this.radioButton3.TabIndex = 1;
            this.radioButton3.TabStop = true;
            this.radioButton3.Text = "No";
            this.radioButton3.UseVisualStyleBackColor = true;
            // 
            // radioButton4
            // 
            this.radioButton4.AutoSize = true;
            this.radioButton4.Location = new System.Drawing.Point(54, 20);
            this.radioButton4.Name = "radioButton4";
            this.radioButton4.Size = new System.Drawing.Size(43, 17);
            this.radioButton4.TabIndex = 0;
            this.radioButton4.Text = "Yes";
            this.radioButton4.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.textBoxRangeStep);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Controls.Add(this.textBoxMax);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.textBoxDefault);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.textBoxMin);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Location = new System.Drawing.Point(21, 217);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(365, 100);
            this.groupBox3.TabIndex = 7;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Range";
            // 
            // textBoxRangeStep
            // 
            this.textBoxRangeStep.Enabled = false;
            this.textBoxRangeStep.Location = new System.Drawing.Point(316, 42);
            this.textBoxRangeStep.Name = "textBoxRangeStep";
            this.textBoxRangeStep.Size = new System.Drawing.Size(36, 20);
            this.textBoxRangeStep.TabIndex = 7;
            this.textBoxRangeStep.Text = "1";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(313, 27);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(29, 13);
            this.label7.TabIndex = 6;
            this.label7.Text = "Step";
            // 
            // textBoxMax
            // 
            this.textBoxMax.Location = new System.Drawing.Point(186, 42);
            this.textBoxMax.Name = "textBoxMax";
            this.textBoxMax.Size = new System.Drawing.Size(82, 20);
            this.textBoxMax.TabIndex = 5;
            this.textBoxMax.Text = "4";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(183, 27);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(51, 13);
            this.label6.TabIndex = 4;
            this.label6.Text = "Maximum";
            // 
            // textBoxDefault
            // 
            this.textBoxDefault.Location = new System.Drawing.Point(103, 43);
            this.textBoxDefault.Name = "textBoxDefault";
            this.textBoxDefault.Size = new System.Drawing.Size(60, 20);
            this.textBoxDefault.TabIndex = 3;
            this.textBoxDefault.Text = "2";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(100, 27);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(41, 13);
            this.label5.TabIndex = 2;
            this.label5.Text = "Default";
            // 
            // textBoxMin
            // 
            this.textBoxMin.Location = new System.Drawing.Point(12, 43);
            this.textBoxMin.Name = "textBoxMin";
            this.textBoxMin.Size = new System.Drawing.Size(67, 20);
            this.textBoxMin.TabIndex = 1;
            this.textBoxMin.Text = "1";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Minimum";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.textBoxHelp);
            this.groupBox2.Controls.Add(this.textBoxHint);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Location = new System.Drawing.Point(21, 102);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(365, 95);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Help";
            // 
            // textBoxHelp
            // 
            this.textBoxHelp.Location = new System.Drawing.Point(12, 20);
            this.textBoxHelp.Multiline = true;
            this.textBoxHelp.Name = "textBoxHelp";
            this.textBoxHelp.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxHelp.Size = new System.Drawing.Size(330, 43);
            this.textBoxHelp.TabIndex = 2;
            // 
            // textBoxHint
            // 
            this.textBoxHint.Location = new System.Drawing.Point(41, 69);
            this.textBoxHint.Name = "textBoxHint";
            this.textBoxHint.Size = new System.Drawing.Size(301, 20);
            this.textBoxHint.TabIndex = 1;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 72);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(26, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Hint";
            // 
            // tabPageReuseQ
            // 
            this.tabPageReuseQ.Controls.Add(this.buttonReuseOK);
            this.tabPageReuseQ.Controls.Add(this.button2);
            this.tabPageReuseQ.Controls.Add(this.groupBox1);
            this.tabPageReuseQ.Location = new System.Drawing.Point(4, 22);
            this.tabPageReuseQ.Name = "tabPageReuseQ";
            this.tabPageReuseQ.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageReuseQ.Size = new System.Drawing.Size(456, 472);
            this.tabPageReuseQ.TabIndex = 1;
            this.tabPageReuseQ.Text = "Re-use";
            this.tabPageReuseQ.UseVisualStyleBackColor = true;
            // 
            // buttonReuseOK
            // 
            this.buttonReuseOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonReuseOK.Location = new System.Drawing.Point(16, 153);
            this.buttonReuseOK.Name = "buttonReuseOK";
            this.buttonReuseOK.Size = new System.Drawing.Size(75, 23);
            this.buttonReuseOK.TabIndex = 4;
            this.buttonReuseOK.Text = "OK";
            this.buttonReuseOK.UseVisualStyleBackColor = true;
            this.buttonReuseOK.Click += new System.EventHandler(this.buttonReuseOK_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(308, 153);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 5;
            this.button2.Text = "Cancel";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.comboBoxRepeatNames);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Location = new System.Drawing.Point(16, 29);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(367, 67);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Select the repeat";
            // 
            // comboBoxRepeatNames
            // 
            this.comboBoxRepeatNames.FormattingEnabled = true;
            this.comboBoxRepeatNames.Location = new System.Drawing.Point(51, 23);
            this.comboBoxRepeatNames.Name = "comboBoxRepeatNames";
            this.comboBoxRepeatNames.Size = new System.Drawing.Size(153, 21);
            this.comboBoxRepeatNames.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 26);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(38, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Name:";
            // 
            // tabPageCalc
            // 
            this.tabPageCalc.Location = new System.Drawing.Point(4, 22);
            this.tabPageCalc.Name = "tabPageCalc";
            this.tabPageCalc.Size = new System.Drawing.Size(456, 472);
            this.tabPageCalc.TabIndex = 2;
            this.tabPageCalc.Text = "Calculate";
            this.tabPageCalc.UseVisualStyleBackColor = true;
            // 
            // checkBoxAppearanceCompact
            // 
            this.checkBoxAppearanceCompact.AutoSize = true;
            this.checkBoxAppearanceCompact.Checked = true;
            this.checkBoxAppearanceCompact.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxAppearanceCompact.Location = new System.Drawing.Point(41, 20);
            this.checkBoxAppearanceCompact.Name = "checkBoxAppearanceCompact";
            this.checkBoxAppearanceCompact.Size = new System.Drawing.Size(134, 17);
            this.checkBoxAppearanceCompact.TabIndex = 5;
            this.checkBoxAppearanceCompact.Text = "Table row per instance";
            this.checkBoxAppearanceCompact.UseVisualStyleBackColor = true;
            // 
            // FormRepeat
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(502, 561);
            this.Controls.Add(this.tabControl1);
            this.Name = "FormRepeat";
            this.Text = "Repeat Setup";
            this.groupBoxQ.ResumeLayout(false);
            this.groupBoxQ.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPageNewQ.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.tabPageReuseQ.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }


        #endregion

        private System.Windows.Forms.Button buttonOK1;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.GroupBox groupBoxQ;
        private System.Windows.Forms.TextBox textBoxQuestionText;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPageNewQ;
        private System.Windows.Forms.TabPage tabPageReuseQ;
        private System.Windows.Forms.Button buttonReuseOK;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.GroupBox groupBox1;
        public System.Windows.Forms.ComboBox comboBoxRepeatNames;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox textBoxHelp;
        private System.Windows.Forms.TextBox textBoxHint;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox textBoxRangeStep;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textBoxMax;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBoxDefault;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBoxMin;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.RadioButton radioButton3;
        private System.Windows.Forms.RadioButton radioButton4;
        private System.Windows.Forms.TabPage tabPageCalc;
        private System.Windows.Forms.CheckBox checkBoxAppearanceCompact;
    }
}