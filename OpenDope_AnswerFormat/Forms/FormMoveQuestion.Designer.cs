namespace OpenDope_AnswerFormat
{
    partial class FormMoveQuestion
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
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxQuestion = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.controlQuestionVaryWhichRepeat1 = new OpenDope_AnswerFormat.Controls.ControlQuestionVaryWhichRepeat();
            this.SuspendLayout();
            // 
            // buttonOK
            // 
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Location = new System.Drawing.Point(41, 242);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 1;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(191, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "With which repeat should the question:";
            // 
            // textBoxQuestion
            // 
            this.textBoxQuestion.Location = new System.Drawing.Point(41, 37);
            this.textBoxQuestion.Multiline = true;
            this.textBoxQuestion.Name = "textBoxQuestion";
            this.textBoxQuestion.Size = new System.Drawing.Size(223, 34);
            this.textBoxQuestion.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(25, 89);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(33, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "vary?";
            // 
            // controlQuestionVaryWhichRepeat1
            // 
            this.controlQuestionVaryWhichRepeat1.Location = new System.Drawing.Point(41, 125);
            this.controlQuestionVaryWhichRepeat1.Name = "controlQuestionVaryWhichRepeat1";
            this.controlQuestionVaryWhichRepeat1.Size = new System.Drawing.Size(335, 90);
            this.controlQuestionVaryWhichRepeat1.TabIndex = 5;
            // 
            // FormMoveQuestion
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(392, 289);
            this.Controls.Add(this.controlQuestionVaryWhichRepeat1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxQuestion);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonOK);
            this.Name = "FormMoveQuestion";
            this.Text = "FormMoveQuestion";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxQuestion;
        private System.Windows.Forms.Label label2;
        private Controls.ControlQuestionVaryWhichRepeat controlQuestionVaryWhichRepeat1;
    }
}