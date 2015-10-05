namespace OpenDope_AnswerFormat.Controls
{
    partial class ControlDataType
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
            this.listBoxDataTypes = new System.Windows.Forms.ListBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.listBoxDataTypes);
            this.groupBox1.Location = new System.Drawing.Point(2, 9);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(105, 133);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Data type";
            // 
            // listBoxDataTypes
            // 
            this.listBoxDataTypes.FormattingEnabled = true;
            this.listBoxDataTypes.Location = new System.Drawing.Point(5, 18);
            this.listBoxDataTypes.Name = "listBoxDataTypes";
            this.listBoxDataTypes.Size = new System.Drawing.Size(96, 95);
            this.listBoxDataTypes.TabIndex = 0;
            // 
            // ControlDataType
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Name = "ControlDataType";
            this.Size = new System.Drawing.Size(109, 301);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        //private System.Windows.Forms.CheckBox checkBoxRequired;
        //private System.Windows.Forms.GroupBox groupBox2;
        //public System.Windows.Forms.TextBox textBoxSampleAnswer;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ListBox listBoxDataTypes;
    }
}
