namespace OpenDope_AnswerFormat.Controls
{
    partial class LogicTaskPaneUserControl
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
            this.treeViewLogicUsed = new System.Windows.Forms.TreeView();
            this.SuspendLayout();
            // 
            // treeViewLogicUsed
            // 
            this.treeViewLogicUsed.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewLogicUsed.Location = new System.Drawing.Point(0, 0);
            this.treeViewLogicUsed.Name = "treeViewLogicUsed";
            this.treeViewLogicUsed.Size = new System.Drawing.Size(419, 705);
            this.treeViewLogicUsed.TabIndex = 1;
            this.treeViewLogicUsed.MouseDown += new System.Windows.Forms.MouseEventHandler(this.treeViewLogicUsed_MouseDown);
            this.treeViewLogicUsed.MouseUp += new System.Windows.Forms.MouseEventHandler(this.treeViewLogicUsed_MouseUp);
            // 
            // LogicTaskPaneUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.treeViewLogicUsed);
            this.Name = "LogicTaskPaneUserControl";
            this.Size = new System.Drawing.Size(419, 705);
            this.ResumeLayout(false);

        }

        #endregion

        //private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TreeView treeViewLogicUsed;
        //private System.Windows.Forms.GroupBox groupBox1;
        //private System.Windows.Forms.CheckBox checkBoxParagraphs;
        //private System.Windows.Forms.CheckBox checkBoxHeadings;
        //private System.Windows.Forms.ListBox listBoxFilter;
        //private System.Windows.Forms.Label label2;
        //private System.Windows.Forms.Label label3;
        //private System.Windows.Forms.TreeView treeViewLogicUnused;
    }
}
