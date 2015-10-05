namespace OpenDope_AnswerFormat
{
    partial class FormDataType
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
            this.controlDataTypeMAIN1 = new OpenDope_AnswerFormat.Controls.ControlDataTypeMAIN();
            this.SuspendLayout();
            // 
            // buttonOK
            // 
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Location = new System.Drawing.Point(19, 320);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 5;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(buttonOK_Click);
            // 
            // controlDataTypeMAIN1
            // 
            this.controlDataTypeMAIN1.Location = new System.Drawing.Point(0, 2);
            this.controlDataTypeMAIN1.Name = "controlDataTypeMAIN1";
            this.controlDataTypeMAIN1.Size = new System.Drawing.Size(680, 312);
            this.controlDataTypeMAIN1.TabIndex = 0;
            // 
            // FormDataType
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(540, 350); // was 312
            this.Controls.Add(this.controlDataTypeMAIN1);
            this.Controls.Add(this.buttonOK);
            this.Name = "FormDataType";
            this.Text = "Answer type";
            this.ResumeLayout(false);

        }


        #endregion

        private System.Windows.Forms.Button buttonOK;
        public Controls.ControlDataTypeMAIN controlDataTypeMAIN1 { get; set; }
    
      

    }
}