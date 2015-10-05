namespace OpenDope_AnswerFormat
{
    partial class FormCondition
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPageNew = new System.Windows.Forms.TabPage();
            this.buttonQuestionAdd = new System.Windows.Forms.Button();
            this.checkBoxScope = new System.Windows.Forms.CheckBox();
            this.comboBoxValues = new System.Windows.Forms.ComboBox();
            this.listBoxQuestions = new System.Windows.Forms.ListBox();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBoxID = new System.Windows.Forms.TextBox();
            this.buttonAdvanced = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.listBoxPredicate = new System.Windows.Forms.ListBox();
            this.listBoxTypeFilter = new System.Windows.Forms.ListBox();
            this.tabReuse = new System.Windows.Forms.TabPage();
            this.tabControl1.SuspendLayout();
            this.tabPageNew.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPageNew);
            this.tabControl1.Controls.Add(this.tabReuse);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(815, 330);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPageNew
            // 
            this.tabPageNew.Controls.Add(this.buttonQuestionAdd);
            this.tabPageNew.Controls.Add(this.checkBoxScope);
            this.tabPageNew.Controls.Add(this.comboBoxValues);
            this.tabPageNew.Controls.Add(this.listBoxQuestions);
            this.tabPageNew.Controls.Add(this.buttonCancel);
            this.tabPageNew.Controls.Add(this.buttonOK);
            this.tabPageNew.Controls.Add(this.groupBox1);
            this.tabPageNew.Controls.Add(this.buttonAdvanced);
            this.tabPageNew.Controls.Add(this.label3);
            this.tabPageNew.Controls.Add(this.label2);
            this.tabPageNew.Controls.Add(this.label1);
            this.tabPageNew.Controls.Add(this.listBoxPredicate);
            this.tabPageNew.Controls.Add(this.listBoxTypeFilter);
            this.tabPageNew.Location = new System.Drawing.Point(4, 22);
            this.tabPageNew.Name = "tabPageNew";
            this.tabPageNew.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageNew.Size = new System.Drawing.Size(807, 304);
            this.tabPageNew.TabIndex = 0;
            this.tabPageNew.Text = "New";
            this.tabPageNew.UseVisualStyleBackColor = true;
            // 
            // buttonQuestionAdd
            // 
            this.buttonQuestionAdd.Location = new System.Drawing.Point(305, 221);
            this.buttonQuestionAdd.Name = "buttonQuestionAdd";
            this.buttonQuestionAdd.Size = new System.Drawing.Size(96, 23);
            this.buttonQuestionAdd.TabIndex = 14;
            this.buttonQuestionAdd.Text = "New Question...";
            this.buttonQuestionAdd.UseVisualStyleBackColor = true;
            this.buttonQuestionAdd.Click += new System.EventHandler(this.buttonQuestionAdd_Click);
            // 
            // checkBoxScope
            // 
            this.checkBoxScope.AutoSize = true;
            this.checkBoxScope.Location = new System.Drawing.Point(124, 222);
            this.checkBoxScope.Name = "checkBoxScope";
            this.checkBoxScope.Size = new System.Drawing.Size(97, 17);
            this.checkBoxScope.TabIndex = 13;
            this.checkBoxScope.Text = "Expand scope ";
            this.checkBoxScope.UseVisualStyleBackColor = true;
            //this.checkBoxScope.CheckedChanged += new System.EventHandler(this.checkBoxScope_CheckedChanged);
            // 
            // comboBoxValues
            // 
            this.comboBoxValues.FormattingEnabled = true;
            this.comboBoxValues.Location = new System.Drawing.Point(523, 93);
            this.comboBoxValues.Name = "comboBoxValues";
            this.comboBoxValues.Size = new System.Drawing.Size(206, 21);
            this.comboBoxValues.TabIndex = 12;
            // 
            // listBoxQuestions
            // 
            this.listBoxQuestions.FormattingEnabled = true;
            this.listBoxQuestions.Location = new System.Drawing.Point(124, 94);
            this.listBoxQuestions.Name = "listBoxQuestions";
            this.listBoxQuestions.Size = new System.Drawing.Size(277, 121);
            this.listBoxQuestions.TabIndex = 11;
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(113, 262);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 10;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // buttonOK
            // 
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Location = new System.Drawing.Point(22, 262);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 9;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textBoxID);
            this.groupBox1.Location = new System.Drawing.Point(22, 19);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(200, 50);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Short name (optional)";
            // 
            // textBoxID
            // 
            this.textBoxID.Location = new System.Drawing.Point(19, 19);
            this.textBoxID.MaxLength = 20;
            this.textBoxID.Name = "textBoxID";
            this.textBoxID.Size = new System.Drawing.Size(166, 20);
            this.textBoxID.TabIndex = 0;
            // 
            // buttonAdvanced
            // 
            this.buttonAdvanced.Location = new System.Drawing.Point(636, 240);
            this.buttonAdvanced.Name = "buttonAdvanced";
            this.buttonAdvanced.Size = new System.Drawing.Size(75, 23);
            this.buttonAdvanced.TabIndex = 7;
            this.buttonAdvanced.Text = "Advanced...";
            this.buttonAdvanced.UseVisualStyleBackColor = true;
            this.buttonAdvanced.Click += new System.EventHandler(buttonAdvanced_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(519, 76);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(34, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Value";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(120, 77);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(49, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Question";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 77);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Filter";
            // 
            // listBoxPredicate
            // 
            this.listBoxPredicate.FormattingEnabled = true;
            this.listBoxPredicate.Location = new System.Drawing.Point(407, 93);
            this.listBoxPredicate.Name = "listBoxPredicate";
            this.listBoxPredicate.Size = new System.Drawing.Size(110, 82);
            this.listBoxPredicate.TabIndex = 2;
            // 
            // listBoxTypeFilter
            // 
            this.listBoxTypeFilter.FormattingEnabled = true;
            this.listBoxTypeFilter.Location = new System.Drawing.Point(22, 93);
            this.listBoxTypeFilter.Name = "listBoxTypeFilter";
            this.listBoxTypeFilter.Size = new System.Drawing.Size(95, 82);
            this.listBoxTypeFilter.TabIndex = 1;
            // 
            // tabReuse
            // 
            this.tabReuse.Location = new System.Drawing.Point(4, 22);
            this.tabReuse.Name = "tabReuse";
            this.tabReuse.Padding = new System.Windows.Forms.Padding(3);
            this.tabReuse.Size = new System.Drawing.Size(807, 304);
            this.tabReuse.TabIndex = 1;
            this.tabReuse.Text = "Reuse existing";
            this.tabReuse.UseVisualStyleBackColor = true;
            // 
            // FormCondition
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(852, 390);
            this.Controls.Add(this.tabControl1);
            this.Name = "FormCondition";
            this.Text = "Specify Condition";
            this.tabControl1.ResumeLayout(false);
            this.tabPageNew.ResumeLayout(false);
            this.tabPageNew.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPageNew;
        private System.Windows.Forms.TabPage tabReuse;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox textBoxID;
        private System.Windows.Forms.Button buttonAdvanced;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox listBoxPredicate;
        private System.Windows.Forms.ListBox listBoxTypeFilter;
        private System.Windows.Forms.ComboBox comboBoxValues;
        private System.Windows.Forms.ListBox listBoxQuestions;
        private System.Windows.Forms.CheckBox checkBoxScope;
        private System.Windows.Forms.Button buttonQuestionAdd;
    }
}