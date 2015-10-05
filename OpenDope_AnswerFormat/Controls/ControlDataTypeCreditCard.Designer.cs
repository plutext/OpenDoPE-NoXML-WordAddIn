namespace OpenDope_AnswerFormat.Controls
{
    partial class ControlDataTypeCreditCard
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBoxHint = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.checkBoxPopulateForm = new System.Windows.Forms.CheckBox();
            this.checkBoxRequired = new System.Windows.Forms.CheckBox();
            this.textBoxSampleAnswer = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textBoxHint);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.checkBoxPopulateForm);
            this.groupBox1.Controls.Add(this.checkBoxRequired);
            this.groupBox1.Controls.Add(this.textBoxSampleAnswer);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(7, 7);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(303, 277);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Credit Card";
            // 
            // textBoxHint
            // 
            this.textBoxHint.Location = new System.Drawing.Point(60, 238);
            this.textBoxHint.Name = "textBoxHint";
            this.textBoxHint.Size = new System.Drawing.Size(205, 20);
            this.textBoxHint.TabIndex = 18;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(21, 241);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(26, 13);
            this.label1.TabIndex = 17;
            this.label1.Text = "Hint";
            // 
            // checkBoxPopulateForm
            // 
            this.checkBoxPopulateForm.AutoSize = true;
            this.checkBoxPopulateForm.Location = new System.Drawing.Point(116, 117);
            this.checkBoxPopulateForm.Name = "checkBoxPopulateForm";
            this.checkBoxPopulateForm.Size = new System.Drawing.Size(149, 17);
            this.checkBoxPopulateForm.TabIndex = 15;
            this.checkBoxPopulateForm.Text = "use as default in web form";
            this.checkBoxPopulateForm.UseVisualStyleBackColor = true;
            // 
            // checkBoxRequired
            // 
            this.checkBoxRequired.AutoSize = true;
            this.checkBoxRequired.Location = new System.Drawing.Point(22, 36);
            this.checkBoxRequired.Name = "checkBoxRequired";
            this.checkBoxRequired.Size = new System.Drawing.Size(123, 17);
            this.checkBoxRequired.TabIndex = 14;
            this.checkBoxRequired.Text = "Required/mandatory";
            this.checkBoxRequired.UseVisualStyleBackColor = true;
            // 
            // textBoxSampleAnswer
            // 
            this.textBoxSampleAnswer.Location = new System.Drawing.Point(116, 81);
            this.textBoxSampleAnswer.Name = "textBoxSampleAnswer";
            this.textBoxSampleAnswer.Size = new System.Drawing.Size(152, 20);
            this.textBoxSampleAnswer.TabIndex = 6;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(19, 81);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(82, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Sample answer:";
            // 
            // ControlDataTypeCreditCard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Name = "ControlDataTypeCreditCard";
            this.Size = new System.Drawing.Size(321, 296);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox textBoxSampleAnswer;
        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.CheckBox checkBoxRequired { get; set; }
        public System.Windows.Forms.CheckBox checkBoxPopulateForm { get; set; }
        public System.Windows.Forms.TextBox textBoxHint { get; set; }
        private System.Windows.Forms.Label label1 { get; set; }

        //private System.Windows.Forms.GroupBox groupBox1;
        //private System.Windows.Forms.TextBox textBoxSampleAnswer;
        //private System.Windows.Forms.Label label2;
        //private System.Windows.Forms.CheckBox checkBoxRequired;
        //private System.Windows.Forms.CheckBox checkBoxPopulateForm;
        //private System.Windows.Forms.TextBox textBoxHint;
        //private System.Windows.Forms.Label label1;

    }
}
