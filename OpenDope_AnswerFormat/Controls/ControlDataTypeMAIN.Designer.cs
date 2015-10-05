namespace OpenDope_AnswerFormat.Controls
{
    partial class ControlDataTypeMAIN
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
            //this.buttonOK = new System.Windows.Forms.Button();
            this.controlDataType1 = new OpenDope_AnswerFormat.Controls.ControlDataType();
            this.controlDataType1.ControlDataTypeMAIN = this;
            this.controlDataTypeNumber = new OpenDope_AnswerFormat.Controls.ControlDataTypeNumber();
            this.controlDataTypeString = new OpenDope_AnswerFormat.Controls.ControlDataTypeString();
            this.controlDataTypeDate = new OpenDope_AnswerFormat.Controls.ControlDataTypeDate();
            this.controlDataTypeCreditCard = new OpenDope_AnswerFormat.Controls.ControlDataTypeCreditCard();
            this.controlDataTypeEmail = new OpenDope_AnswerFormat.Controls.ControlDataTypeEmail();
            this.SuspendLayout();
            // 
            // buttonOK
            // 
            //this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            //this.buttonOK.Location = new System.Drawing.Point(19, 264);
            //this.buttonOK.Name = "buttonOK";
            //this.buttonOK.Size = new System.Drawing.Size(75, 23);
            //this.buttonOK.TabIndex = 5;
            //this.buttonOK.Text = "OK";
            //this.buttonOK.UseVisualStyleBackColor = true;
            // 
            // controlDataType1
            // 
            this.controlDataType1.Location = new System.Drawing.Point(1, 13);
            this.controlDataType1.Name = "controlDataType1";
            this.controlDataType1.Size = new System.Drawing.Size(109, 301);
            this.controlDataType1.TabIndex = 6;

            this.controlDataTypeNumber.Location = new System.Drawing.Point(110, 13);
            this.controlDataTypeNumber.Name = "controlDataTypeNumber";
            this.controlDataTypeNumber.Size = new System.Drawing.Size(321, 296);
            this.controlDataTypeNumber.TabIndex = 7;
            this.controlDataTypeNumber.Visible = false;

            this.controlDataTypeString.Location = new System.Drawing.Point(110, 13);
            this.controlDataTypeString.Name = "controlDataTypeString";
            this.controlDataTypeString.Size = new System.Drawing.Size(321, 296);
            this.controlDataTypeString.TabIndex = 7;
            this.controlDataTypeString.Visible = false;

            this.controlDataTypeDate.Location = new System.Drawing.Point(110, 13);
            this.controlDataTypeDate.Name = "controlDataTypeDate";
            this.controlDataTypeDate.Size = new System.Drawing.Size(321, 296);
            this.controlDataTypeDate.TabIndex = 7;
            this.controlDataTypeDate.Visible = false;

            this.controlDataTypeCreditCard.Location = new System.Drawing.Point(110, 13);
            this.controlDataTypeCreditCard.Name = "controlDataTypeCreditCard";
            this.controlDataTypeCreditCard.Size = new System.Drawing.Size(321, 296);
            this.controlDataTypeCreditCard.TabIndex = 7;
            this.controlDataTypeCreditCard.Visible = false;

            this.controlDataTypeEmail.Location = new System.Drawing.Point(110, 13);
            this.controlDataTypeEmail.Name = "controlDataTypeEmail";
            this.controlDataTypeEmail.Size = new System.Drawing.Size(321, 296);
            this.controlDataTypeEmail.TabIndex = 7;
            this.controlDataTypeEmail.Visible = false;

            // 
            // ControlDataTypeMAIN
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.controlDataType1);
            this.Controls.Add(this.controlDataTypeNumber);
            this.Controls.Add(this.controlDataTypeString);
            this.Controls.Add(this.controlDataTypeDate);
            this.Controls.Add(this.controlDataTypeCreditCard);
            this.Controls.Add(this.controlDataTypeEmail);
            //this.Controls.Add(this.buttonOK);
            this.Name = "ControlDataTypeMAIN";
            this.Size = new System.Drawing.Size(520, 312);
            this.ResumeLayout(false);

        }

        #endregion

        ////private System.Windows.Forms.Button buttonOK;
        public Controls.ControlDataType controlDataType1 { get; set; }

        // Only one of these will be relevant
        public Controls.ControlDataTypeNumber controlDataTypeNumber { get; set; }
        public Controls.ControlDataTypeString controlDataTypeString { get; set; }
        public Controls.ControlDataTypeDate controlDataTypeDate { get; set; }
        public Controls.ControlDataTypeCreditCard controlDataTypeCreditCard { get; set; }
        public Controls.ControlDataTypeEmail controlDataTypeEmail { get; set; }


        //private Controls.ControlDataType controlDataType1;
        //private Controls.ControlDataTypeNumber controlDataTypeNumber;
        //private Controls.ControlDataTypeString controlDataTypeString;
        //private Controls.ControlDataTypeDate controlDataTypeDate;
        //private Controls.ControlDataTypeCreditCard controlDataTypeCreditCard;
        //private Controls.ControlDataTypeEmail controlDataTypeEmail;

    }
}
