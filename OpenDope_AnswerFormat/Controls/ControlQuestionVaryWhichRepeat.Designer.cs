namespace OpenDope_AnswerFormat.Controls
{
    partial class ControlQuestionVaryWhichRepeat
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
            this.treeViewRepeat = new System.Windows.Forms.TreeView();
            this.SuspendLayout();
            // 
            // treeViewRepeat
            // 
            this.treeViewRepeat.Location = new System.Drawing.Point(3, 3);
            this.treeViewRepeat.Name = "treeViewRepeat";
            this.treeViewRepeat.Size = new System.Drawing.Size(330, 85);
            this.treeViewRepeat.TabIndex = 0;
            // 
            // ControlQuestionVaryWhichRepeat
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.treeViewRepeat);
            this.Name = "ControlQuestionVaryWhichRepeat";
            this.Size = new System.Drawing.Size(335, 90);
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.TreeView treeViewRepeat;
    }
}
