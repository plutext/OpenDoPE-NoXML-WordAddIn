/*
 *  OpenDoPE authoring Word AddIn
    Copyright (C) Plutext Pty Ltd, 2012
 * 
    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using NLog;
//using OpenDope_AnswerFormat.Helpers;

using Office = Microsoft.Office.Core;
using OpenDoPEModel;
using Word = Microsoft.Office.Interop.Word;
using Microsoft.Office.Tools.Word.Extensions; // for VSTOObject

namespace OpenDope_AnswerFormat.Forms
{
    /// <summary>
    /// The XForm will display the questions in the order in which
    /// their answers appear in the AnswersPart.
    /// 
    /// So this form allows the author to re-order the answers, via
    /// drag and drop in a tree view.
    /// 
    /// A tree view is used so that the author can re-order within
    /// a repeat.
    /// 
    /// Drag and drop is restricted so that the hierarchical structure
    /// is preserved.  ie author can't drag to/from different repeats.
    /// </summary>
    public partial class FormAnswerOrder : Form
    {
        static Logger log = LogManager.GetLogger("FormAnswerOrder");

        Office.CustomXMLPart answersPart;
        private answers answersObj;
        private questionnaire questionnaire;

        TreeNode root = new TreeNode("Questions");

        public FormAnswerOrder(Office.CustomXMLPart answersPart, Office.CustomXMLPart questionsPart)
        {
            InitializeComponent();

            // To populate the tree view, we need to traverse
            // the answers.  We could do this at the DOM level,
            // or using our answers object. 
            // Best to use our answers object.

            this.answersPart = answersPart;
            answersObj = new answers();
            OpenDope_AnswerFormat.answers.Deserialize(answersPart.XML, out answersObj);

            // We want to show the question text in the tree view
            questionnaire = new questionnaire();
            questionnaire.Deserialize(questionsPart.XML, out questionnaire);

            ImageList TreeviewIL = new ImageList();
            TreeviewIL.Images.Add(System.Drawing.Image.FromStream(
                System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("OpenDope_AnswerFormat.folder.png")));
            TreeviewIL.Images.Add(System.Drawing.Image.FromStream(
                System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("OpenDope_AnswerFormat.Icons.LogicTree.variable_chevron.png")));
            TreeviewIL.Images.Add(System.Drawing.Image.FromStream(
                System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("OpenDope_AnswerFormat.Icons.LogicTree.repeat.png")));
            this.treeView1.ImageList = TreeviewIL;


            addNodes(answersObj.Items, root);
            this.treeView1.Nodes.Add(root);
            root.ImageIndex = 0;
            root.SelectedImageIndex = 0;


            treeView1.ExpandAll();
        }


        TreeNode thisNode;

        private void addNodes(List<object> objects, TreeNode attachmentPoint)
        {
            foreach (object o in objects)
            {
                if (o is answer)
                {
                    thisNode = new TreeNode(getQuestionText((answer)o));
                    attachmentPoint.Nodes.Add(thisNode);
                    thisNode.ImageIndex = 1;
                    thisNode.SelectedImageIndex = 1; // what to use when it is selected
                    thisNode.Tag = o;
                }
                else if (o is repeat)
                {
                    repeat r = (repeat)o;

                    thisNode = new TreeNode(getQuestionText(r));
                    attachmentPoint.Nodes.Add(thisNode);
                    thisNode.ImageIndex = 2;
                    thisNode.SelectedImageIndex = 2;
                    thisNode.Tag = o;

                    // Now recurse
                    List<repeatRow> row = r.row;
                    addNodes(row[0].Items, thisNode);
                }
            }
        }


        private string getQuestionText(answer a) {
            return questionnaire.getQuestion(a.id).text;
        }

        private string getQuestionText(repeat a)
        {
            return questionnaire.getQuestion(a.qref).text;
        }

        private void buttonApply_Click(object sender, EventArgs e)
        {

        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {

        }

        private answers answersResult;

        /// <summary>
        /// Save displayed order
        /// </summary>
        public void apply()
        {
            answersResult = new answers();

            saveNodes(root.Nodes, answersResult.Items);

            string result = answersResult.Serialize();
            log.Debug(result);

            CustomXmlUtilities.replaceXmlDoc(answersPart, result);
        }

        private void saveNodes(TreeNodeCollection tnc, List<object> attachmentPoint)
        {
            foreach (TreeNode tn in tnc)
            {
                object o = tn.Tag;
                if (o is answer)
                {
                    attachmentPoint.Add(o);
                }
                else if (o is repeat)
                {
                    repeat rNew = new repeat();
                    attachmentPoint.Add(rNew);
                    rNew.qref = ((repeat)o).qref;

                    repeatRow rr = new repeatRow();
                    rNew.row.Add(rr);

                    // Now recurse
                    saveNodes(tn.Nodes, rr.Items); 
                }
            }
        }

        //------------------------------------------------------
        // Drag and drop stuff; heavily adapted from http://www.codeproject.com/Articles/6184/TreeView-Rearrange
        // (basically just keeping its insertion point graphics)
        // Or, could adapt http://msdn.microsoft.com/en-us/library/system.windows.forms.control.dodragdrop


        void treeView1_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            this.treeView1.SelectedNode = this.treeView1.GetNodeAt(e.X, e.Y);
        }


        private void treeView1_ItemDrag(object sender, System.Windows.Forms.ItemDragEventArgs e)
        {
            DoDragDrop(e.Item, DragDropEffects.Move);
        }

        private void treeView1_DragEnter(object sender, System.Windows.Forms.DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        /// <summary>
        /// Occurs when a drag-and-drop operation is completed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeView1_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
        {
            if (NodeOver == null)
            {
                this.Refresh();
                return;
            }

            TreeNode movingNode = (TreeNode)e.Data.GetData("System.Windows.Forms.TreeNode");

            TreeNodeCollection insertCollection = movingNode.Parent.Nodes;

            int fromPos = insertCollection.IndexOf(movingNode);
            int toPos;
            if (insertBefore)
            {
                toPos = insertCollection.IndexOf(NodeOver);
            }
            else
            {
                toPos = insertCollection.IndexOf(NodeOver) + 1;
            }

            if (fromPos > toPos)
            {
                movingNode.Remove();
                insertCollection.Insert(toPos, movingNode);
            }
            else
            {
                movingNode.Remove();
                insertCollection.Insert(toPos-1, movingNode);
            }

            // DEBUG
            //apply();
        }

        TreeNode NodeOver = null;
        bool insertBefore = true;

        private void treeView1_DragOver(object sender, System.Windows.Forms.DragEventArgs e)
        {

            NodeOver = null;
            TreeNode tmpNodeOver = this.treeView1.GetNodeAt(
                this.treeView1.PointToClient(Cursor.Position));
            if (tmpNodeOver == null) return;
            //log.Debug("entered DragOver, over " + tmpNodeOver.Text);

            TreeNode NodeMoving = (TreeNode)e.Data.GetData("System.Windows.Forms.TreeNode");

            if (tmpNodeOver == NodeMoving)
            {
                this.treeView1.Cursor = Cursors.No;
                return;
            }
            if (tmpNodeOver.Parent != NodeMoving.Parent)
            {
                this.treeView1.Cursor = Cursors.No;
                return;
            }
            // OK, 
            this.treeView1.Cursor = Cursors.Default;
            NodeOver = tmpNodeOver;

            int OffsetY = this.treeView1.PointToClient(Cursor.Position).Y - NodeOver.Bounds.Top;
            int NodeOverImageWidth = this.treeView1.ImageList.Images[NodeOver.ImageIndex].Size.Width + 8;

            if (OffsetY < (NodeOver.Bounds.Height / 2))
            {
                insertBefore = true;

                // Clear placeholders above and below
                this.Refresh();

                // Draw the placeholders
                this.DrawLeafTopPlaceholders(NodeOver);

            }
            else
            {
                insertBefore = false;
                        
                // Clear placeholders above and below
                this.Refresh();
                        
                // Image index of 1 is the answer icon
                if (NodeOver.ImageIndex == 1)
                {
                        // Draw the placeholders
                        DrawLeafBottomPlaceholders(NodeOver, null);
                } else {
                        // Draw the placeholders
                        DrawFolderTopPlaceholders(NodeOver);
                }                        
            }
        }

        #region Helper Methods
        private void DrawLeafTopPlaceholders(TreeNode NodeOver)
        {
            Graphics g = this.treeView1.CreateGraphics();

            int NodeOverImageWidth = this.treeView1.ImageList.Images[NodeOver.ImageIndex].Size.Width + 8;
            int LeftPos = NodeOver.Bounds.Left - NodeOverImageWidth;
            int RightPos = this.treeView1.Width - 4;

            Point[] LeftTriangle = new Point[5]{
												   new Point(LeftPos, NodeOver.Bounds.Top - 4),
												   new Point(LeftPos, NodeOver.Bounds.Top + 4),
												   new Point(LeftPos + 4, NodeOver.Bounds.Y),
												   new Point(LeftPos + 4, NodeOver.Bounds.Top - 1),
												   new Point(LeftPos, NodeOver.Bounds.Top - 5)};

            Point[] RightTriangle = new Point[5]{
													new Point(RightPos, NodeOver.Bounds.Top - 4),
													new Point(RightPos, NodeOver.Bounds.Top + 4),
													new Point(RightPos - 4, NodeOver.Bounds.Y),
													new Point(RightPos - 4, NodeOver.Bounds.Top - 1),
													new Point(RightPos, NodeOver.Bounds.Top - 5)};


            g.FillPolygon(System.Drawing.Brushes.Black, LeftTriangle);
            g.FillPolygon(System.Drawing.Brushes.Black, RightTriangle);
            g.DrawLine(new System.Drawing.Pen(Color.Black, 2), new Point(LeftPos, NodeOver.Bounds.Top), new Point(RightPos, NodeOver.Bounds.Top));

        }

        private void DrawLeafBottomPlaceholders(TreeNode NodeOver, TreeNode ParentDragDrop)
        {
            Graphics g = this.treeView1.CreateGraphics();

            int NodeOverImageWidth = this.treeView1.ImageList.Images[NodeOver.ImageIndex].Size.Width + 8;
            // Once again, we are not dragging to node over, draw the placeholder using the ParentDragDrop bounds
            int LeftPos, RightPos;
            if (ParentDragDrop != null)
                LeftPos = ParentDragDrop.Bounds.Left - (this.treeView1.ImageList.Images[ParentDragDrop.ImageIndex].Size.Width + 8);
            else
                LeftPos = NodeOver.Bounds.Left - NodeOverImageWidth;
            RightPos = this.treeView1.Width - 4;

            Point[] LeftTriangle = new Point[5]{
												   new Point(LeftPos, NodeOver.Bounds.Bottom - 4),
												   new Point(LeftPos, NodeOver.Bounds.Bottom + 4),
												   new Point(LeftPos + 4, NodeOver.Bounds.Bottom),
												   new Point(LeftPos + 4, NodeOver.Bounds.Bottom - 1),
												   new Point(LeftPos, NodeOver.Bounds.Bottom - 5)};

            Point[] RightTriangle = new Point[5]{
													new Point(RightPos, NodeOver.Bounds.Bottom - 4),
													new Point(RightPos, NodeOver.Bounds.Bottom + 4),
													new Point(RightPos - 4, NodeOver.Bounds.Bottom),
													new Point(RightPos - 4, NodeOver.Bounds.Bottom - 1),
													new Point(RightPos, NodeOver.Bounds.Bottom - 5)};


            g.FillPolygon(System.Drawing.Brushes.Black, LeftTriangle);
            g.FillPolygon(System.Drawing.Brushes.Black, RightTriangle);
            g.DrawLine(new System.Drawing.Pen(Color.Black, 2), new Point(LeftPos, NodeOver.Bounds.Bottom), new Point(RightPos, NodeOver.Bounds.Bottom));
        }

        private void DrawFolderTopPlaceholders(TreeNode NodeOver)
        {
            Graphics g = this.treeView1.CreateGraphics();
            int NodeOverImageWidth = this.treeView1.ImageList.Images[NodeOver.ImageIndex].Size.Width + 8;

            int LeftPos, RightPos;
            LeftPos = NodeOver.Bounds.Left - NodeOverImageWidth;
            RightPos = this.treeView1.Width - 4;

            Point[] LeftTriangle = new Point[5]{
												   new Point(LeftPos, NodeOver.Bounds.Top - 4),
												   new Point(LeftPos, NodeOver.Bounds.Top + 4),
												   new Point(LeftPos + 4, NodeOver.Bounds.Y),
												   new Point(LeftPos + 4, NodeOver.Bounds.Top - 1),
												   new Point(LeftPos, NodeOver.Bounds.Top - 5)};

            Point[] RightTriangle = new Point[5]{
													new Point(RightPos, NodeOver.Bounds.Top - 4),
													new Point(RightPos, NodeOver.Bounds.Top + 4),
													new Point(RightPos - 4, NodeOver.Bounds.Y),
													new Point(RightPos - 4, NodeOver.Bounds.Top - 1),
													new Point(RightPos, NodeOver.Bounds.Top - 5)};


            g.FillPolygon(System.Drawing.Brushes.Black, LeftTriangle);
            g.FillPolygon(System.Drawing.Brushes.Black, RightTriangle);
            g.DrawLine(new System.Drawing.Pen(Color.Black, 2), new Point(LeftPos, NodeOver.Bounds.Top), new Point(RightPos, NodeOver.Bounds.Top));

        }


        #endregion


    }
}
