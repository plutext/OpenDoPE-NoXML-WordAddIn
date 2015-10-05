namespace OpenDope_AnswerFormat.Controls
{
    partial class ControlQuestionCommon
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
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.textBoxHelp = new System.Windows.Forms.TextBox();
            this.groupBoxQ = new System.Windows.Forms.GroupBox();
            this.textBoxQuestionText = new System.Windows.Forms.TextBox();
            this.groupBox2.SuspendLayout();
            this.groupBoxQ.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.textBoxHelp);
            this.groupBox2.Location = new System.Drawing.Point(4, 106);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(365, 95);
            this.groupBox2.TabIndex = 8;
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
            // groupBoxQ
            // 
            this.groupBoxQ.Controls.Add(this.textBoxQuestionText);
            this.groupBoxQ.Location = new System.Drawing.Point(3, 18);
            this.groupBoxQ.Name = "groupBoxQ";
            this.groupBoxQ.Size = new System.Drawing.Size(367, 82);
            this.groupBoxQ.TabIndex = 7;
            this.groupBoxQ.TabStop = false;
            this.groupBoxQ.Text = "Question";
            // 
            // textBoxQuestionText
            // 
            this.textBoxQuestionText.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxQuestionText.Location = new System.Drawing.Point(7, 20);
            this.textBoxQuestionText.Multiline = true;
            this.textBoxQuestionText.Name = "textBoxQuestionText";
            this.textBoxQuestionText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxQuestionText.Size = new System.Drawing.Size(346, 56);
            this.textBoxQuestionText.TabIndex = 0;
            // 
            // ControlQuestionCommon
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBoxQ);
            this.Name = "ControlQuestionCommon";
            this.Size = new System.Drawing.Size(375, 208);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBoxQ.ResumeLayout(false);
            this.groupBoxQ.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox textBoxHelp;
        private System.Windows.Forms.GroupBox groupBoxQ;
        public System.Windows.Forms.TextBox textBoxQuestionText;
    }
}
