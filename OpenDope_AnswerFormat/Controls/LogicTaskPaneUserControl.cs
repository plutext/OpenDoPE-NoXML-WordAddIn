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
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Word = Microsoft.Office.Interop.Word;
using Microsoft.Office.Tools.Word.Extensions; // for VSTOObject

using NLog;

using OpenDoPEModel;
using OpenDope_AnswerFormat.Helpers;

using System.IO;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace OpenDope_AnswerFormat.Controls
{
    public partial class LogicTaskPaneUserControl : UserControl
    {
        static Logger log = LogManager.GetLogger("LogicTaskPaneUserControl");

        private Model model;
        private Word.Document document;

        const int TEXT_LENGTH = 40;

        TreeNode root;

        ContextMenuStrip rightClickMenu;

        /// <summary>
        /// Where a user deletes a content control on the document surface,
        /// this flag will be set, and acted upon on their first left click
        /// on the tree view.
        /// 
        /// It is not ideal, but it is a simple way to refresh, without
        /// dealing with background worker, or invoke.  
        /// See further http://msdn.microsoft.com/en-us/library/ms171728
        /// </summary>
        public bool Dirty { get; set; }

        public LogicTaskPaneUserControl(Model model, Word.Document docx)
        {
            InitializeComponent();

            this.model = model;
            this.document = docx;

            //this.listBoxFilter.Items.Add("show all logic");
            //this.listBoxFilter.Items.Add("conditions");
            //this.listBoxFilter.Items.Add("repeats");
            //this.listBoxFilter.Items.Add("variables");

            string[] images = { 
                                  "outline", //0
                                  "p_small_circle", 
                                  "table",   //2
                                  "table_row", 
                                  "table_cell", 
                                  "variable_chevron", //5
                                  "condition_if_32", 
                                  "repeat", 
                                  "narrative", //8
                                  "narrative_topic",
                                  "sdt_anon"};

            ImageList TreeviewIL = new ImageList();
            TreeviewIL.ImageSize = new Size(24,24);
            for (int i = 0; i < images.Length; i++)
            {
                TreeviewIL.Images.Add(System.Drawing.Image.FromStream(
                    System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(
                        "OpenDope_AnswerFormat.Icons.LogicTree." + images[i] + ".png")));
            }
            this.treeViewLogicUsed.ImageList = TreeviewIL;

            populateLogicInUse();

            //Setup some menu items.
            locateLabel = new ToolStripMenuItem();
            locateLabel.Text = MENU_LOCATE_IN_DOC;
            locateLabel.Click += new EventHandler(menuItem_Click);

            editLabel = new ToolStripMenuItem();
            editLabel.Text = MENU_EDIT_LOGIC;
            editLabel.Click += new EventHandler(menuItem_Click);

            deleteLabel = new ToolStripMenuItem();
            deleteLabel.Text = MENU_DELETE;

            ToolStripMenuItem deleteControlAndContentsLabel = new ToolStripMenuItem();
            deleteControlAndContentsLabel.Text = MENU_DELETE_CONTROL_AND_CONTENTS;
            deleteControlAndContentsLabel.Click += new EventHandler(menuItem_Click);

            ToolStripMenuItem deleteButKeepContentsLabel = new ToolStripMenuItem();
            deleteButKeepContentsLabel.Text = MENU_DELETE_CONTROL;
            deleteButKeepContentsLabel.Click += new EventHandler(menuItem_Click);

            ToolStripMenuItem deleteContentsOnlyLabel = new ToolStripMenuItem();
            deleteContentsOnlyLabel.Text = MENU_DELETE_CONTENTS;
            deleteContentsOnlyLabel.Click += new EventHandler(menuItem_Click);

            deleteLabel.DropDownItems.AddRange(new ToolStripMenuItem[] { deleteControlAndContentsLabel, deleteButKeepContentsLabel, deleteContentsOnlyLabel });
        }

        /// <summary>
        /// Left click action is to select the content control in the document
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void treeViewLogicUsed_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // Select the clicked node
                treeViewLogicUsed.SelectedNode = treeViewLogicUsed.GetNodeAt(e.X, e.Y);

                if (treeViewLogicUsed.SelectedNode != null
                    && treeViewLogicUsed.SelectedNode.Tag != null
                    && treeViewLogicUsed.SelectedNode.Tag is SdtElement)
                {
                    SdtElement sdtEl = (SdtElement)treeViewLogicUsed.SelectedNode.Tag;

                    DocumentFormat.OpenXml.Wordprocessing.SdtId id = sdtEl.SdtProperties.GetFirstChild<DocumentFormat.OpenXml.Wordprocessing.SdtId>();
                    executeLocateInDoc(id, false);

                }
            }

            if (Dirty)
            {
                this.populateLogicInUse();
            }
        }

        /// <summary>
        /// Right click action is to show the context menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void treeViewLogicUsed_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Right)
                {
                    // Select the clicked node
                    treeViewLogicUsed.SelectedNode = treeViewLogicUsed.GetNodeAt(e.X, e.Y);

                    if(treeViewLogicUsed.SelectedNode != null)
                    {
                        //log.Debug("selected: " + treeViewLogicUsed.SelectedNode.Text);

                        if (treeViewLogicUsed.SelectedNode.Tag != null)
                        {
                            if (treeViewLogicUsed.SelectedNode.Tag is SdtElement)
                            {
                                SdtElement sdtEl = (SdtElement)treeViewLogicUsed.SelectedNode.Tag;
                                DocumentFormat.OpenXml.Wordprocessing.Tag tag = sdtEl.SdtProperties.GetFirstChild<DocumentFormat.OpenXml.Wordprocessing.Tag>();

                                if (tag == null)
                                {
                                    showRightClickMenu(e.Location, false);
                                }
                                else if (tag.Val.Value.Contains("od:xpath"))
                                {
                                    showRightClickMenu(e.Location, true);
                                }
                                else if (tag.Val.Value.Contains("od:condition")
                                    || tag.Val.Value.Contains("od:RptPosCon"))
                                {
                                    //showRightClickMenu(e.Location, true);
                                    showRightClickMenu(e.Location, false);  // TODO condition editor
                                }
                                else if (tag.Val.Value.Contains("od:repeat"))
                                {
                                    showRightClickMenu(e.Location, true);
                                }
                                else if (tag.Val.Value.Contains("od:narrative"))
                                {
                                    showRightClickMenu(e.Location, false);
                                }

                            }
                            //else
                            //{
                            //    log.Debug(treeViewLogicUsed.SelectedNode.Tag.GetType().FullName);
                            //}
                        }

                    }
                }
        }

         const string MENU_LOCATE_IN_DOC = "Shift Focus to document";
         const string MENU_EDIT_LOGIC = "Edit logic";
         const string MENU_DELETE = "Delete";
         const string MENU_DELETE_ALL = "Everything! (Control, Contents, and logic)"; //TODO
         const string MENU_DELETE_CONTROL_AND_CONTENTS = "Keep logic (but del both Control and its Contents)";
         const string MENU_DELETE_CONTENTS = "Contents (!)";
         const string MENU_DELETE_CONTROL = "Control, but keep Contents and logic";

        ToolStripMenuItem locateLabel;
        ToolStripMenuItem editLabel;
        ToolStripMenuItem deleteLabel;

        void showRightClickMenu(Point point, bool editable)
        {
            // Create the ContextMenuStrip.
            rightClickMenu = new ContextMenuStrip();

            //Add the menu items to the menu.
            if (editable)
            {
                rightClickMenu.Items.AddRange(new ToolStripMenuItem[] { locateLabel, editLabel, deleteLabel });
            }
            else
            {
                rightClickMenu.Items.AddRange(new ToolStripMenuItem[] { locateLabel, deleteLabel });
            }

            rightClickMenu.Show(treeViewLogicUsed, point);
        }

        void menuItem_Click(object sender, EventArgs e)
        {
            if (treeViewLogicUsed.SelectedNode != null)
            {
                //log.Debug("selected: " + treeViewLogicUsed.SelectedNode.Text);

                if (treeViewLogicUsed.SelectedNode.Tag != null)
                {
                    if (treeViewLogicUsed.SelectedNode.Tag is SdtElement)
                    {
                        SdtElement sdtEl = (SdtElement)treeViewLogicUsed.SelectedNode.Tag;
                        DocumentFormat.OpenXml.Wordprocessing.Tag tag = sdtEl.SdtProperties.GetFirstChild<DocumentFormat.OpenXml.Wordprocessing.Tag>();

                        DocumentFormat.OpenXml.Wordprocessing.SdtId id = sdtEl.SdtProperties.GetFirstChild<DocumentFormat.OpenXml.Wordprocessing.SdtId>();

                        string command = ((ToolStripMenuItem)sender).Text;
                        log.Debug(command + " clicked! for node " + tag.Val.Value);

                        if (command.Equals(MENU_LOCATE_IN_DOC)) {
                            executeLocateInDoc(id, true);
                        }
                        else if (command.Equals(MENU_EDIT_LOGIC))
                        {
                            executeEditLogic( id);

                            // Refresh
                            populateLogicInUse();
                        }
                        else if (command.Equals(MENU_DELETE_CONTROL_AND_CONTENTS))
                        {
                            executeDeleteSdt(id, true, true, false);
                        }
                        else if (command.Equals(MENU_DELETE_CONTROL))
                        {
                            executeDeleteSdt(id, true, false, false);
                        }
                        else if (command.Equals(MENU_DELETE_CONTENTS))
                        {
                            executeDeleteSdt(id, false, true, false);

                            // Refresh
                            populateLogicInUse();
                        }


                    }
                }
            }
            else
            {
                log.Debug(((ToolStripMenuItem)sender).Text + " clicked! BUT NO NODE SELECTED! " );

            }
        }

        private Word.ContentControl getControlById(SdtId id)
        {

            /* The spec says SdtId is type xsd:int, being an integer between -2147483648 and 2147483647
             * (2^31),
             * and indeed, that is what we see in the XML.  But Word internally uses positive 
             * numbers, hence the conversion value here (+ 2^32). */

            string val1 = id.Val.Value.ToString();
            string val2 = (id.Val.Value + 4294967296).ToString();
            foreach (Word.ContentControl cc in document.ContentControls)
            {
                //log.Debug("checking " + cc.ID + " contents " + cc.Tag);
                if (cc.ID.Equals(val1 )
                    || cc.ID.Equals(val2))
                {
                    return cc;
                }
            }
            log.Error("Couldn't find cc by id " + id.Val.Value.ToString() );
            return null;
        }

        public void executeDeleteSdt(SdtId id, bool deleteControl, bool deleteContents, bool deleteLogic)
        {
            Word.ContentControl cc = getControlById(id);
            if (cc != null)
            {
                if (deleteControl)
                {
                    cc.Delete(deleteContents);
                }
                else if (deleteContents)
                {
                    cc.Range.Delete();
                }
            }
        }

        /// <summary>
        /// Select this content control in the document.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="focus"></param>
        public void executeLocateInDoc(SdtId id, bool focus)
        {
            Word.ContentControl cc = getControlById(id);
            if (cc != null)
            {
                cc.Range.Select();

                if (focus)
                {
                    focusOnDocument();
                }
            }
        }

        private void focusOnDocument()
        {
            document.Activate();

            //Application.DoEvents(); // supposedly works in VB

            // doesn't work
            //document.ActiveWindow.ActivePane.View.SeekView = Word.WdSeekView.wdSeekMainDocument;

            // Activate Ribbon = deactivate CustomTaskPane (is there other way to do that?)
            System.Windows.Forms.SendKeys.Send("{F10}");
            // Deactivate Ribbon - document get the focus
            System.Windows.Forms.SendKeys.Send("{Esc}");
        }

        public void executeEditLogic(SdtId id)
        {
            Word.ContentControl cc = getControlById(id);
            if (cc != null
                && cc.Tag!=null)
            {
                cc.Range.Select();

                TagData td = new TagData(cc.Tag);
                if (td.getXPathID() != null)
                {
                    XPathsPartEntry xppe = new XPathsPartEntry(model);
                    xpathsXpath xpo = xppe.getXPathByID(td.getXPathID());

                    if (xpo.questionID != null)
                    {
                        Forms.FormQuestionEdit fqe = new Forms.FormQuestionEdit(xpo.questionID);
                        fqe.ShowDialog();
                        fqe.Dispose();
                    }
                    else
                    {
                        log.Warn("No question associated with xpath " + xpo.dataBinding.xpath);
                    }

                }
                else if (td.getConditionID() != null)
                {
                    ConditionsPartEntry cpe = new ConditionsPartEntry(model);

                    condition c = cpe.getConditionByID(td.getConditionID());

                    FormCondition formCondition = new FormCondition(cc, cpe, c);
                    formCondition.ShowDialog();
                    formCondition.Dispose();
                }
                else if (td.getRepeatID() != null)
                {
                    // TODO
                }

            }
        }

        public void populateLogicInUse()
        {

            // To facilitate refresh
            this.treeViewLogicUsed.Nodes.Clear();
            root = new TreeNode("DocumentStructure");

              Word.Range r = document.Range();

              //Get stream for the range. This is the System.IO.Packaging.Package stream
              Stream packageStream;
              try
              {
                  packageStream = r.GetPackageStreamFromRange();
                  // or OpcHelper.GetPackageStreamFromRange(r);
              }
              catch (Exception e)
              {
                  // There are 2 possible problems:
                  // (1) .WordOpenXML fails when result would be say 150 MB
                  // (2) If .WordOpenXML succeeds, open packaging stuff can still fail in 
                  //     IsolatedStorage (see OpcHelper for details)

                  //// More re (1):
                  //   .WordOpenXML fails with a COMException (and similar prob testing in VBA, but save as XML works) 
                  //// Problem is too many pictures .. so delete them and try again
                  //// Leaving this until an exception has occurred is too slow, hence
                  //// proactive pruning above.
                  //// Test case: IGA_ScheduleF_national_education_agreement.doc
                  //// (search for student employment)
                  ////  http://www.coag.gov.au/intergov_agreements/federal_financial_relations/docs/IGA_ScheduleF_national_education_agreement.doc
                  //log.Warn(e.Message);

                  // More re (2):
                  // NB If the package part is too big (more than 1.3Mb compressed), 
                  // .NET decides to unzip the entire package part to Isolated Storage,
                  // which fails with IsolatedStorageException since under COM,
                  // we are running in a DefaultDomain that doesn't have any evidence.
                  // See OpcHelper for more comment.

                  log.Error(e);
                  System.Windows.Forms.MessageBox.Show("Can't display logic.");
                  return;
              }

              //Stream packageStream = this.Paragraphs[1].Range.GetPackageStreamFromRange();
              //Use Open Xml SDK to process it.
              using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(packageStream, true))
              {
                  addNodes(wordDoc.MainDocumentPart.Document.Body.Elements(), 
                      root);

                  this.treeViewLogicUsed.Nodes.Add(root);
                  root.ImageIndex = 0;
                  root.SelectedImageIndex = 0;

                  treeViewLogicUsed.ExpandAll();
                  root.EnsureVisible(); // scroll to top

                  //                      log.Debug(el.LocalName + ": " + el.InnerText);
              }
        }

        TreeNode thisNode;


        // Join runs together
        StringBuilder runTexts; 
        TreeNode pTreeNode;
        bool pTreeNodePopulated;

        private string showWhitespace(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return text.Replace(" ", "~");
            }
            else
            {
                return text;
            }
        }

        private void addNodes(IEnumerable<OpenXmlElement> elements, TreeNode attachmentPoint)
        {

            foreach (OpenXmlElement el in elements)
            {
                if (el.LocalName.Equals("p"))
                {
                    //log.Debug(el.LocalName + ": " + el.GetType().FullName);

                    string text = el.InnerText;

                    // Ignore empty paragraphs 
                    if (string.IsNullOrWhiteSpace(text)
                        && !el.InnerXml.Contains("w:sdt") )
                    {
                        continue;
                    }

                    if (text.Length > TEXT_LENGTH)
                    {
                        text = text.Substring(0, TEXT_LENGTH);
                    }

                    thisNode = new TreeNode(text); // provisional; we'll change this if there is logic
                    attachmentPoint.Nodes.Add(thisNode);

                    pTreeNode = thisNode;
                    pTreeNodePopulated = false;
                    runTexts = new StringBuilder();

                    // Outline .. very simple minded ..
                    // use this icon if it has a heading style
                    Paragraph p = (Paragraph)el;
                    if (p.ParagraphProperties!=null
                        && p.ParagraphProperties.ParagraphStyleId!=null
                            && p.ParagraphProperties.ParagraphStyleId.Val.Value.Contains("eading")) {

                        thisNode.ImageIndex = 0;
                        thisNode.SelectedImageIndex = 0;

                    } else {

                        thisNode.ImageIndex = 1;
                        thisNode.SelectedImageIndex = 1;
                        //thisNode.Tag = o;
                    }

                    // Now recurse                    
                    addNodes(el.Elements(), thisNode);

                    // handle any residual run content
                    if (runTexts.Length > 0)
                    {
                        if (pTreeNodePopulated)
                        {
                            TreeNode runNode = new TreeNode(showWhitespace(runTexts.ToString()));
                            runNode.ImageIndex = 1;
                            runNode.SelectedImageIndex = 1;
                            pTreeNode.Nodes.Add(runNode);
                        }
                        else
                        {
                            // we already provisionally populated this
                        }
                    }
                }
                else if (el is Run)
                {
                    runTexts.Append(el.InnerText);
                } 
                else if (el is ParagraphProperties
                   || el is RunProperties
                   || el is SdtProperties
                   || el is SectionProperties
                   || el is TableCellProperties)
                {
                    continue;
                }
                else if (el is Table)
                {
                    thisNode = new TreeNode("table");
                    attachmentPoint.Nodes.Add(thisNode);
                    thisNode.ImageIndex = 2;
                    thisNode.SelectedImageIndex = 2;
                    //thisNode.Tag = o;

                    // Now recurse                    
                    addNodes(el.Elements(), thisNode);
                }
                else if (el is SdtElement)
                {
                    // first, end the run stuff
                    if (runTexts==null) {
                        // Hit a block level cc, before hitting a w:p
                        runTexts = new StringBuilder();
                    }
                    else if (runTexts.Length > 0)
                    //if (el is SdtRun)
                    {
                        if (pTreeNodePopulated)
                        {
                            // add a run node
                            TreeNode runNode = new TreeNode(showWhitespace(runTexts.ToString()));
                            runNode.ImageIndex = 1;
                            runNode.SelectedImageIndex = 1;
                            attachmentPoint.Nodes.Add(runNode);
                        }
                        else
                        {
                            // put it at p level
                            pTreeNode.Text = showWhitespace(runTexts.ToString());
                            pTreeNodePopulated = true;
                        }
                        runTexts = new StringBuilder();
                    }

                    SdtElement sdtEl = (SdtElement)el;
                    TreeNode sdtTreeNode = null;

                    DocumentFormat.OpenXml.Wordprocessing.Tag tag = sdtEl.SdtProperties.GetFirstChild<DocumentFormat.OpenXml.Wordprocessing.Tag>();

                    SdtAlias alias = sdtEl.SdtProperties.GetFirstChild<SdtAlias>();
                    if (alias == null)
                    {
                        if (tag == null)
                        {
                            sdtTreeNode = new TreeNode("content control");
                        }
                        else
                        {
                            sdtTreeNode = new TreeNode(tag.Val.Value);
                        }
                    }
                    else
                    {
                        sdtTreeNode = new TreeNode(alias.Val.Value);
                    }
                    sdtTreeNode.Tag = sdtEl;
                    attachmentPoint.Nodes.Add(sdtTreeNode);


                    if (tag == null)
                    {
                        sdtTreeNode.ImageIndex = 10;
                        sdtTreeNode.SelectedImageIndex = 10;
                    }
                    else if (tag.Val.Value.Contains("od:xpath"))
                    {
                        sdtTreeNode.ImageIndex = 5;
                        sdtTreeNode.SelectedImageIndex = 5;
                    }
                    else if (tag.Val.Value.Contains("od:condition")
                        || tag.Val.Value.Contains("od:RptPosCon"))
                    {
                        sdtTreeNode.ImageIndex = 6;
                        sdtTreeNode.SelectedImageIndex = 6;
                    }
                    else if (tag.Val.Value.Contains("od:repeat"))
                    {
                        sdtTreeNode.ImageIndex = 7;
                        sdtTreeNode.SelectedImageIndex = 7;
                    }
                    else if (tag.Val.Value.Contains("od:narrative"))
                    {
                        sdtTreeNode.ImageIndex = 8;
                        sdtTreeNode.SelectedImageIndex = 8;
                    }
                    else
                    {
                        log.Debug(tag.Val.Value);
                        sdtTreeNode.ImageIndex = 10;
                        sdtTreeNode.SelectedImageIndex = 10;
                    }


                    // Now recurse if necessary                   
                    if (tag!=null && (tag.Val.Value.Contains("od:xpath")
                        || tag.Val.Value.Contains("od:narrative")))
                    {
                        // do nothing
                    }
                    else
                    {
                        // yep, recurse
                        thisNode = sdtTreeNode;
                        addNodes(el.Elements(), sdtTreeNode);

                        // handle any residual run content
                        if (runTexts.Length > 0)
                        {
                            // add a run node
                            TreeNode runNode = new TreeNode(showWhitespace(runTexts.ToString()));
                            runNode.ImageIndex = 1;
                            runNode.SelectedImageIndex = 1;
                            sdtTreeNode.Nodes.Add(runNode);
                            runTexts = new StringBuilder();
                        }
                    }
                }
                else if (el.LocalName.Equals("sdtContent"))
                {
                    // just recurse
                    addNodes(el.Elements(), thisNode);
                }
                else if (el is TableRow)
                {
                    thisNode = new TreeNode("table row");
                    attachmentPoint.Nodes.Add(thisNode);
                    thisNode.ImageIndex = 3;
                    thisNode.SelectedImageIndex = 3;
                    //thisNode.Tag = o;

                    // Now recurse                    
                    addNodes(el.Elements(), thisNode);
                }
                else if (el is TableCell)
                {
                    thisNode = new TreeNode("table cell");
                    attachmentPoint.Nodes.Add(thisNode);
                    thisNode.ImageIndex = 4;
                    thisNode.SelectedImageIndex = 4;
                    //thisNode.Tag = o;

                    // Now recurse                    
                    addNodes(el.Elements(), thisNode);
                }
                else
                {

                    // pPr: DocumentFormat.OpenXml.Wordprocessing.ParagraphProperties
                    //r: DocumentFormat.OpenXml.Wordprocessing.Run
                    // sdt: DocumentFormat.OpenXml.Wordprocessing.SdtRun
                    //tbl: DocumentFormat.OpenXml.Wordprocessing.Table


                    log.Debug(el.LocalName + ": " + el.GetType().FullName);
                }

            }
        }
    }
}
