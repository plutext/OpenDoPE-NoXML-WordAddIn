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
    partial class ConditionOrRepeat
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
            this.label1 = new System.Windows.Forms.Label();
            this.radioButtonCondition = new System.Windows.Forms.RadioButton();
            this.radioButtonRepeat = new System.Windows.Forms.RadioButton();
            this.buttonNext = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(40, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(188, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "What sort of control do you want?";
            // 
            // radioButtonCondition
            // 
            this.radioButtonCondition.AutoSize = true;
            this.radioButtonCondition.Checked = true;
            this.radioButtonCondition.Location = new System.Drawing.Point(84, 57);
            this.radioButtonCondition.Name = "radioButtonCondition";
            this.radioButtonCondition.Size = new System.Drawing.Size(69, 17);
            this.radioButtonCondition.TabIndex = 1;
            this.radioButtonCondition.TabStop = true;
            this.radioButtonCondition.Text = "Condition";
            this.radioButtonCondition.UseVisualStyleBackColor = true;
            // 
            // radioButtonRepeat
            // 
            this.radioButtonRepeat.AutoSize = true;
            this.radioButtonRepeat.Location = new System.Drawing.Point(84, 84);
            this.radioButtonRepeat.Name = "radioButtonRepeat";
            this.radioButtonRepeat.Size = new System.Drawing.Size(60, 17);
            this.radioButtonRepeat.TabIndex = 2;
            this.radioButtonRepeat.TabStop = true;
            this.radioButtonRepeat.Text = "Repeat";
            this.radioButtonRepeat.UseVisualStyleBackColor = true;
            // 
            // radioButtonBind
            // 
            this.radioButtonRepeat.AutoSize = true;
            this.radioButtonRepeat.Location = new System.Drawing.Point(84, 111);
            this.radioButtonRepeat.Name = "radioButtonBind";
            this.radioButtonRepeat.Size = new System.Drawing.Size(60, 17);
            this.radioButtonRepeat.TabIndex = 2;
            this.radioButtonRepeat.TabStop = true;
            this.radioButtonRepeat.Text = "XPath data value";
            this.radioButtonRepeat.UseVisualStyleBackColor = true;
            // 
            // buttonNext
            // 
            this.buttonNext.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonNext.Location = new System.Drawing.Point(78, 148);
            this.buttonNext.Name = "buttonNext";
            this.buttonNext.Size = new System.Drawing.Size(75, 23);
            this.buttonNext.TabIndex = 3;
            this.buttonNext.Text = "Next";
            this.buttonNext.UseVisualStyleBackColor = true;
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(188, 148);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 4;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // ConditionOrRepeat
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(390, 186);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonNext);
            this.Controls.Add(this.radioButtonRepeat);
            this.Controls.Add(this.radioButtonCondition);
            this.Controls.Add(this.radioButtonBind);
            this.Controls.Add(this.label1);
            this.Name = "ConditionOrRepeat";
            this.Text = "Select content control type ";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.RadioButton radioButtonCondition { get; set; }
        public System.Windows.Forms.RadioButton radioButtonRepeat { get; set; }
        public System.Windows.Forms.RadioButton radioButtonBind { get; set; }
        private System.Windows.Forms.Button buttonNext;
        private System.Windows.Forms.Button buttonCancel;
    }
}