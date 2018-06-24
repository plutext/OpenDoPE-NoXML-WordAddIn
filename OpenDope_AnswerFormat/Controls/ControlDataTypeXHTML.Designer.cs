namespace OpenDope_AnswerFormat.Controls
{
    partial class ControlDataTypeXHTML
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
            this.textBoxSampleAnswer = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textBox1);
            this.groupBox1.Controls.Add(this.textBoxSampleAnswer);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(7, 7);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(303, 277);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "XHTML";
            this.groupBox1.Enter += new System.EventHandler(this.groupBox1_Enter);
            // 
            // textBoxSampleAnswer
            // 
            this.textBoxSampleAnswer.Location = new System.Drawing.Point(116, 38);
            this.textBoxSampleAnswer.Name = "textBoxSampleAnswer";
            this.textBoxSampleAnswer.Size = new System.Drawing.Size(152, 20);
            this.textBoxSampleAnswer.TabIndex = 6;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(19, 38);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(82, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Sample answer:";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(22, 94);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(246, 71);
            this.textBox1.TabIndex = 8;
            this.textBox1.Text = "Note: This is typically populated from XML data file (ie the non-interactive case" +
    ").  Unless you have an XHTML editing component, this won\'t be useful in the " +
    "interactive case.";
            // 
            // ControlDataTypeFlatOpcXml
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Name = "ControlDataTypeXHTML";
            this.Size = new System.Drawing.Size(321, 296);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        public System.Windows.Forms.CheckBox checkBoxRequired { get; set; }
        private System.Windows.Forms.TextBox textBoxSampleAnswer;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox1;

        public System.Windows.Forms.TextBox textBoxHint { get; set; }
        private System.Windows.Forms.Label label1 { get; set; }

    }
}
