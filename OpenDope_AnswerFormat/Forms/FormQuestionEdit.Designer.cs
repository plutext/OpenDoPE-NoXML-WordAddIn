namespace OpenDope_AnswerFormat.Forms
{
    partial class FormQuestionEdit
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
            this.controlQuestionCommon1 = new OpenDope_AnswerFormat.Controls.ControlQuestionCommon();
            this.controlQuestionVaryWhichRepeat1 = new OpenDope_AnswerFormat.Controls.ControlQuestionVaryWhichRepeat();
            this.groupBoxRepeat = new System.Windows.Forms.GroupBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPageQuestion = new System.Windows.Forms.TabPage();
            this.tabPageResponse = new System.Windows.Forms.TabPage();
            this.groupBoxResponseFixed = new System.Windows.Forms.GroupBox();
            //this.buttonSwitchFromMCQ = new System.Windows.Forms.Button();
            this.controlQuestionResponsesFixed1 = new OpenDope_AnswerFormat.Controls.ControlQuestionResponsesFixed();
            this.groupBoxResponseFree = new System.Windows.Forms.GroupBox();
            //this.buttonSwitch = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.Cancel = new System.Windows.Forms.Button();
            this.controlDataTypeMAIN1 = new OpenDope_AnswerFormat.Controls.ControlDataTypeMAIN();
            this.groupBoxRepeat.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPageQuestion.SuspendLayout();
            this.tabPageResponse.SuspendLayout();
            this.groupBoxResponseFixed.SuspendLayout();
            this.groupBoxResponseFree.SuspendLayout();
            this.SuspendLayout();
            // 
            // controlQuestionCommon1
            // 
            this.controlQuestionCommon1.Location = new System.Drawing.Point(24, 19);
            this.controlQuestionCommon1.Name = "controlQuestionCommon1";
            this.controlQuestionCommon1.Size = new System.Drawing.Size(375, 208);
            this.controlQuestionCommon1.TabIndex = 0;
            // 
            // controlQuestionVaryWhichRepeat1
            // 
            this.controlQuestionVaryWhichRepeat1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.controlQuestionVaryWhichRepeat1.Location = new System.Drawing.Point(6, 19);
            this.controlQuestionVaryWhichRepeat1.Name = "controlQuestionVaryWhichRepeat1";
            this.controlQuestionVaryWhichRepeat1.OkAsis = false;
            this.controlQuestionVaryWhichRepeat1.Size = new System.Drawing.Size(335, 90);
            this.controlQuestionVaryWhichRepeat1.TabIndex = 1;
            // 
            // groupBoxRepeat
            // 
            this.groupBoxRepeat.Controls.Add(this.controlQuestionVaryWhichRepeat1);
            this.groupBoxRepeat.Location = new System.Drawing.Point(25, 234);
            this.groupBoxRepeat.Name = "groupBoxRepeat";
            this.groupBoxRepeat.Size = new System.Drawing.Size(367, 124);
            this.groupBoxRepeat.TabIndex = 2;
            this.groupBoxRepeat.TabStop = false;
            this.groupBoxRepeat.Text = "Behaviour in Repeats";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPageQuestion);
            this.tabControl1.Controls.Add(this.tabPageResponse);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(918, 479);
            this.tabControl1.TabIndex = 4;
            // 
            // tabPageQuestion
            // 
            this.tabPageQuestion.Controls.Add(this.controlQuestionCommon1);
            this.tabPageQuestion.Controls.Add(this.groupBoxRepeat);
            this.tabPageQuestion.Location = new System.Drawing.Point(4, 22);
            this.tabPageQuestion.Name = "tabPageQuestion";
            this.tabPageQuestion.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageQuestion.Size = new System.Drawing.Size(910, 453);
            this.tabPageQuestion.TabIndex = 0;
            this.tabPageQuestion.Text = "Question";
            this.tabPageQuestion.UseVisualStyleBackColor = true;
            // 
            // tabPageResponse
            // 
            this.tabPageResponse.Controls.Add(this.groupBoxResponseFixed);
            this.tabPageResponse.Controls.Add(this.groupBoxResponseFree);
            this.tabPageResponse.Location = new System.Drawing.Point(4, 22);
            this.tabPageResponse.Name = "tabPageResponse";
            this.tabPageResponse.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageResponse.Size = new System.Drawing.Size(910, 453);
            this.tabPageResponse.TabIndex = 1;
            this.tabPageResponse.Text = "Answer";
            this.tabPageResponse.UseVisualStyleBackColor = true;
            // 
            // groupBoxResponseFixed
            // 
            //this.groupBoxResponseFixed.Controls.Add(this.buttonSwitchFromMCQ);
            this.groupBoxResponseFixed.Controls.Add(this.controlQuestionResponsesFixed1);
            this.groupBoxResponseFixed.Location = new System.Drawing.Point(455, 22);
            this.groupBoxResponseFixed.Name = "groupBoxResponseFixed";
            this.groupBoxResponseFixed.Size = new System.Drawing.Size(436, 413);
            this.groupBoxResponseFixed.TabIndex = 5;
            this.groupBoxResponseFixed.TabStop = false;
            this.groupBoxResponseFixed.Text = "Multiple Choice response";
            // 
            // buttonSwitchFromMCQ
            // 
            //this.buttonSwitchFromMCQ.Location = new System.Drawing.Point(173, 19);
            //this.buttonSwitchFromMCQ.Name = "buttonSwith";
            //this.buttonSwitchFromMCQ.Size = new System.Drawing.Size(247, 23);
            //this.buttonSwitchFromMCQ.TabIndex = 1;
            //this.buttonSwitchFromMCQ.Text = "Change from Multiple Choice to  open ended..";
            //this.buttonSwitchFromMCQ.UseVisualStyleBackColor = true;
            // 
            // controlQuestionResponsesFixed1
            // 
            this.controlQuestionResponsesFixed1.Location = new System.Drawing.Point(7, 47);
            this.controlQuestionResponsesFixed1.Name = "controlQuestionResponsesFixed1";
            this.controlQuestionResponsesFixed1.Size = new System.Drawing.Size(413, 360);
            this.controlQuestionResponsesFixed1.TabIndex = 0;
            // 
            // groupBoxResponseFree
            // 
            this.groupBoxResponseFree.Controls.Add(this.controlDataTypeMAIN1);
            this.controlDataTypeMAIN1.Location = new System.Drawing.Point(2, 10); //new
            //this.groupBoxResponseFree.Controls.Add(this.buttonSwitch);
            this.groupBoxResponseFree.Location = new System.Drawing.Point(5, 22); //was 15
            this.groupBoxResponseFree.Name = "groupBoxResponseFree";
            this.groupBoxResponseFree.Size = new System.Drawing.Size(450, 413); //was 436
            this.groupBoxResponseFree.TabIndex = 4;
            this.groupBoxResponseFree.TabStop = false;
            this.groupBoxResponseFree.Text = "Open ended response";
            // 
            // buttonSwitch
            // 
            //this.buttonSwitch.Location = new System.Drawing.Point(197, 19);
            //this.buttonSwitch.Name = "buttonSwitch";
            //this.buttonSwitch.Size = new System.Drawing.Size(174, 23);
            //this.buttonSwitch.TabIndex = 4;
            //this.buttonSwitch.Text = "Change to Multiple Choice...";
            //this.buttonSwitch.UseVisualStyleBackColor = true;
            // 
            // buttonOK
            // 
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Location = new System.Drawing.Point(12, 512);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 5;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // Cancel
            // 
            this.Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Cancel.Location = new System.Drawing.Point(117, 511);
            this.Cancel.Name = "Cancel";
            this.Cancel.Size = new System.Drawing.Size(75, 23);
            this.Cancel.TabIndex = 6;
            this.Cancel.Text = "Cancel";
            this.Cancel.UseVisualStyleBackColor = true;
            this.Cancel.Click += new System.EventHandler(this.Cancel_Click);
            // 
            // controlDataTypeMAIN1
            // 
            this.controlDataTypeMAIN1.Location = new System.Drawing.Point(6, 57);
            this.controlDataTypeMAIN1.Name = "controlDataTypeMAIN1";
            this.controlDataTypeMAIN1.Size = new System.Drawing.Size(680, 312);
            this.controlDataTypeMAIN1.TabIndex = 5;
            // 
            // FormQuestionEdit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(948, 560);
            this.Controls.Add(this.Cancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.tabControl1);
            this.Name = "FormQuestionEdit";
            this.Text = "Edit Question";
            this.groupBoxRepeat.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPageQuestion.ResumeLayout(false);
            this.tabPageResponse.ResumeLayout(false);
            this.groupBoxResponseFixed.ResumeLayout(false);
            this.groupBoxResponseFree.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Controls.ControlQuestionCommon controlQuestionCommon1;
        private Controls.ControlQuestionVaryWhichRepeat controlQuestionVaryWhichRepeat1;
        private System.Windows.Forms.GroupBox groupBoxRepeat;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPageQuestion;
        private System.Windows.Forms.TabPage tabPageResponse;
        private System.Windows.Forms.GroupBox groupBoxResponseFixed;
        private System.Windows.Forms.GroupBox groupBoxResponseFree;
        //private System.Windows.Forms.Button buttonSwitch;
        private Controls.ControlQuestionResponsesFixed controlQuestionResponsesFixed1;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button Cancel;
        private Controls.ControlDataTypeMAIN controlDataTypeMAIN1;
    }
}