namespace OpenDope_AnswerFormat.Controls
{
    partial class ControlDataTypeDate
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
            this.datePickerUpper = new System.Windows.Forms.DateTimePicker();
            this.datePickerLower = new System.Windows.Forms.DateTimePicker();
            this.datePickerSample = new System.Windows.Forms.DateTimePicker();
            this.checkBoxRequired = new System.Windows.Forms.CheckBox();
            this.listBoxUpperOperator = new System.Windows.Forms.ListBox();
            this.listBoxLowerOperator = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textBoxHint);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.checkBoxPopulateForm);
            this.groupBox1.Controls.Add(this.datePickerUpper);
            this.groupBox1.Controls.Add(this.datePickerLower);
            this.groupBox1.Controls.Add(this.datePickerSample);
            this.groupBox1.Controls.Add(this.checkBoxRequired);
            this.groupBox1.Controls.Add(this.listBoxUpperOperator);
            this.groupBox1.Controls.Add(this.listBoxLowerOperator);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(7, 7);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(303, 277);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Date";
            // 
            // textBoxHint
            // 
            this.textBoxHint.Location = new System.Drawing.Point(40, 238);
            this.textBoxHint.Name = "textBoxHint";
            this.textBoxHint.Size = new System.Drawing.Size(258, 20);
            this.textBoxHint.TabIndex = 18;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 241);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(26, 13);
            this.label1.TabIndex = 17;
            this.label1.Text = "Hint";
            // 
            // checkBoxPopulateForm
            // 
            this.checkBoxPopulateForm.AutoSize = true;
            this.checkBoxPopulateForm.Location = new System.Drawing.Point(98, 86);
            this.checkBoxPopulateForm.Name = "checkBoxPopulateForm";
            this.checkBoxPopulateForm.Size = new System.Drawing.Size(149, 17);
            this.checkBoxPopulateForm.TabIndex = 19;
            this.checkBoxPopulateForm.Text = "use as default in web form";
            this.checkBoxPopulateForm.UseVisualStyleBackColor = true;
            // 
            // datePickerUpper
            // 
            this.datePickerUpper.Location = new System.Drawing.Point(98, 161);
            this.datePickerUpper.Name = "datePickerUpper";
            this.datePickerUpper.Size = new System.Drawing.Size(200, 20);
            this.datePickerUpper.TabIndex = 18;
            //this.datePickerUpper.Enter += new System.EventHandler(datePickerUpper_Enter);
            // 
            // datePickerLower
            // 
            this.datePickerLower.Location = new System.Drawing.Point(98, 125);
            this.datePickerLower.Name = "datePickerLower";
            this.datePickerLower.Size = new System.Drawing.Size(200, 20);
            this.datePickerLower.TabIndex = 17;
            //this.datePickerLower.Enter += new System.EventHandler(datePickerLower_Enter);
            // 
            // datePickerSample
            // 
            this.datePickerSample.Location = new System.Drawing.Point(98, 60);
            this.datePickerSample.Name = "datePickerSample";
            this.datePickerSample.Size = new System.Drawing.Size(200, 20);
            this.datePickerSample.TabIndex = 16;
            // 
            // checkBoxRequired
            // 
            this.checkBoxRequired.AutoSize = true;
            this.checkBoxRequired.Location = new System.Drawing.Point(11, 25);
            this.checkBoxRequired.Name = "checkBoxRequired";
            this.checkBoxRequired.Size = new System.Drawing.Size(123, 17);
            this.checkBoxRequired.TabIndex = 14;
            this.checkBoxRequired.Text = "Required/mandatory";
            this.checkBoxRequired.UseVisualStyleBackColor = true;
            // 
            // listBoxUpperOperator
            // 
            this.listBoxUpperOperator.FormattingEnabled = true;
            this.listBoxUpperOperator.Location = new System.Drawing.Point(31, 161);
            this.listBoxUpperOperator.Name = "listBoxUpperOperator";
            this.listBoxUpperOperator.Size = new System.Drawing.Size(57, 30);
            this.listBoxUpperOperator.TabIndex = 13;
            // 
            // listBoxLowerOperator
            // 
            this.listBoxLowerOperator.FormattingEnabled = true;
            this.listBoxLowerOperator.Location = new System.Drawing.Point(31, 125);
            this.listBoxLowerOperator.Name = "listBoxLowerOperator";
            this.listBoxLowerOperator.Size = new System.Drawing.Size(57, 30);
            this.listBoxLowerOperator.TabIndex = 12;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 60);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(82, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Sample answer:";
            // 
            // ControlDataTypeDate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Name = "ControlDataTypeDate";
            this.Size = new System.Drawing.Size(321, 296);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }


        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.ListBox listBoxUpperOperator { get; set; }
        public System.Windows.Forms.ListBox listBoxLowerOperator { get; set; }
        private System.Windows.Forms.DateTimePicker datePickerUpper;
        private System.Windows.Forms.DateTimePicker datePickerLower;
        private System.Windows.Forms.DateTimePicker datePickerSample;
        public System.Windows.Forms.CheckBox checkBoxRequired { get; set; }
        public System.Windows.Forms.CheckBox checkBoxPopulateForm { get; set; }
        public System.Windows.Forms.TextBox textBoxHint { get; set; }
        private System.Windows.Forms.Label label1 { get; set; }

        //private System.Windows.Forms.GroupBox groupBox1;
        //private System.Windows.Forms.Label label2;
        //private System.Windows.Forms.ListBox listBoxUpperOperator;
        //private System.Windows.Forms.ListBox listBoxLowerOperator;
        //private System.Windows.Forms.DateTimePicker datePickerUpper;
        //private System.Windows.Forms.DateTimePicker datePickerLower;
        //private System.Windows.Forms.DateTimePicker datePickerSample;
        //private System.Windows.Forms.CheckBox checkBoxRequired;
        //private System.Windows.Forms.CheckBox checkBoxPopulateForm;
        //private System.Windows.Forms.TextBox textBoxHint;
        //private System.Windows.Forms.Label label1;

    }
}
