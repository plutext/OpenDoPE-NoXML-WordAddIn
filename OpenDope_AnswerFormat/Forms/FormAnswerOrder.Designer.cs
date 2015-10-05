namespace OpenDope_AnswerFormat.Forms
{
    partial class FormAnswerOrder
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
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.buttonApply = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonDocumentOrder = new System.Windows.Forms.Button();
            this.buttonTopics = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // treeView1
            // 
            this.treeView1.AllowDrop = true;
            this.treeView1.Location = new System.Drawing.Point(28, 28);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(319, 373);
            this.treeView1.TabIndex = 0;
            this.treeView1.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.treeView1_ItemDrag);
            this.treeView1.DragDrop += new System.Windows.Forms.DragEventHandler(this.treeView1_DragDrop);
            this.treeView1.DragEnter += new System.Windows.Forms.DragEventHandler(this.treeView1_DragEnter);
            this.treeView1.DragOver += new System.Windows.Forms.DragEventHandler(this.treeView1_DragOver);
            this.treeView1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.treeView1_MouseDown);
            // 
            // buttonApply
            // 
            this.buttonApply.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonApply.Location = new System.Drawing.Point(181, 451);
            this.buttonApply.Name = "buttonApply";
            this.buttonApply.Size = new System.Drawing.Size(75, 23);
            this.buttonApply.TabIndex = 1;
            this.buttonApply.Text = "Apply";
            this.buttonApply.UseVisualStyleBackColor = true;
            this.buttonApply.Click += new System.EventHandler(this.buttonApply_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(272, 451);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 2;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonDocumentOrder
            // 
            this.buttonDocumentOrder.Enabled = false;
            this.buttonDocumentOrder.Location = new System.Drawing.Point(28, 407);
            this.buttonDocumentOrder.Name = "buttonDocumentOrder";
            this.buttonDocumentOrder.Size = new System.Drawing.Size(150, 23);
            this.buttonDocumentOrder.TabIndex = 3;
            this.buttonDocumentOrder.Text = "Snap to document order...";
            this.buttonDocumentOrder.UseVisualStyleBackColor = true;
            // 
            // buttonTopics
            // 
            this.buttonTopics.Enabled = false;
            this.buttonTopics.Location = new System.Drawing.Point(28, 437);
            this.buttonTopics.Name = "buttonTopics";
            this.buttonTopics.Size = new System.Drawing.Size(75, 23);
            this.buttonTopics.TabIndex = 4;
            this.buttonTopics.Text = "Topics...";
            this.buttonTopics.UseVisualStyleBackColor = true;
            // 
            // FormAnswerOrder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(387, 486);
            this.Controls.Add(this.buttonTopics);
            this.Controls.Add(this.buttonDocumentOrder);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonApply);
            this.Controls.Add(this.treeView1);
            this.Name = "FormAnswerOrder";
            this.Text = "Questions - Ordering";
            this.ResumeLayout(false);

        }


        #endregion

        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.Button buttonApply;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonDocumentOrder;
        private System.Windows.Forms.Button buttonTopics;
    }
}