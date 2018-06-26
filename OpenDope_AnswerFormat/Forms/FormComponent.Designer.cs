namespace OpenDope_AnswerFormat.Forms
{
    partial class FormComponent
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
            this.userControlComponentIRI1 = new OpenDope_AnswerFormat.Controls.UserControlComponentIRI();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonRemove = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // userControlComponentIRI1
            // 
            this.userControlComponentIRI1.Location = new System.Drawing.Point(25, 21);
            this.userControlComponentIRI1.Name = "userControlComponentIRI1";
            this.userControlComponentIRI1.Size = new System.Drawing.Size(232, 72);
            this.userControlComponentIRI1.TabIndex = 0;
            // 
            // buttonOK
            // 
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Location = new System.Drawing.Point(35, 110);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 1;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonRemove
            // 
            this.buttonRemove.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonRemove.Location = new System.Drawing.Point(158, 110);
            this.buttonRemove.Name = "buttonRemove";
            this.buttonRemove.Size = new System.Drawing.Size(75, 23);
            this.buttonRemove.TabIndex = 2;
            this.buttonRemove.Text = "Remove";
            this.buttonRemove.UseVisualStyleBackColor = true;
            this.buttonRemove.Click += new System.EventHandler(this.buttonRemove_Click);
            // 
            // FormComponent
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 145);
            this.Controls.Add(this.buttonRemove);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.userControlComponentIRI1);
            this.Name = "FormComponent";
            this.Text = "FormComponent";
            this.ResumeLayout(false);

        }

        #endregion

        public Controls.UserControlComponentIRI userControlComponentIRI1;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonRemove;
    }
}