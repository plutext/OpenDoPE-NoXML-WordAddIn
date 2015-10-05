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
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Office = Microsoft.Office.Core;

using System.Data;
using Word = Microsoft.Office.Interop.Word;
using Microsoft.Office.Tools.Word.Extensions; // for VSTOObject

using NLog;

using OpenDoPEModel;
using System.Windows.Forms;

namespace OpenDope_AnswerFormat
{
    [ComVisible(true)]
    public class Ribbon : Office.IRibbonExtensibility
    {
        /* This ribbon is (largely) stateless.
        // The active document contains the state.
         * 
         * The intent is that a result overview document,
         * and its group of instance documents,
         * behave as a connected group (ie with
         * consistent next|previous behaviour).
         * 
         * This should be true for each overview|instance
         * group.
         * 
         */

        /*
         * See http://blogs.msdn.com/b/vsto/archive/2010/06/04/creating-an-add-in-for-office-2007-and-office-2010-that-quot-lights-up-quot-on-office-2010-mclean-schofield.aspx
         * re pattern for the use of 2010 features (eg UndoRecord), which 
         *  is possible by virtue of the new embedded interop types feature 
         *  in Visual Studio 2010 (also sometimes referred to as no-PIA or 
         *  by the related /link compiler option). When you compile an add-in 
         *  project that targets the .NET Framework 4, by default* the type 
         *  information for all the PIA types referenced in the add-in code 
         *  is embedded in the add-in assembly. At run time, this type 
         *  information is used to resolve calls to the underlying COM type, 
         *  rather than relying on type information in the PIAs.
         */
        int majorVersion;  // Word version

        #region Infrastructure


        Word.Application wdApp;

        static private Office.IRibbonUI ribbon;
        public static Office.IRibbonUI getRibbon()
        {


            return ribbon;
        }

        //private static Microsoft.Office.Tools.CustomTaskPane ctp;

        public static void myInvalidate()
        {
            //ctp = findCustomTaskPane();
            Ribbon.ribbon.Invalidate();
        }


        /*
         * IRibbonUI behaves like a singleton (though nowhere does
         * the MSDN doc actually say this?)
         * 
         * Observation:  Activating a Window calls get on text box 
         * (if Word determines that is necessary), but stops short 
         * of invalidating the ribbon, (since that would set new values in
         * the menus and by observation, the menu in each window is different).
         * 
         * Clicked on a result in results overview; Observed to change 
         * text box in another window. (But it will change back when 
         * i activate that window)
         */

        static Logger log = LogManager.GetLogger("OpenDope_AnswerFormat");

        public Ribbon()
        {
            log.Debug("Ribbon constructed.");
        }

/*
        public System.Drawing.Bitmap LoadImage(string imageName)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            Stream stream = assembly.GetManifestResourceStream("com.plutext.search.main.google_desktop_facicon.bmp");

            //string[] names = assembly.GetManifestResourceNames();
            //foreach (string name in names)
            //    log.Debug(name);

            return new System.Drawing.Bitmap(stream);
        }
        */




        public string GetCustomUI(string ribbonID)
        {
            return GetResourceText("OpenDope_AnswerFormat.Ribbon.xml");
        }




        //Create callback methods here. For more information about adding callback methods, select the Ribbon XML item in Solution Explorer and then press F1

        // See http://msdn.microsoft.com/en-us/library/aa722523.aspx for signatures

        public void Ribbon_Load(Office.IRibbonUI ribbonUI)
        {
            Ribbon.ribbon = ribbonUI;
        }

        private FabDocxState getState()
        {
            if (Globals.ThisAddIn.Application.Documents.Count == 0) return null;

            Microsoft.Office.Tools.Word.Document extendedDocument = null;
            try
            {
                extendedDocument = Globals.ThisAddIn.Application.ActiveDocument.GetVstoObject(Globals.Factory);
            }
            catch (COMException e)
            {
                log.Error(e);
                return null;
            }

            if (extendedDocument.Tag == null)
            {
                // Things might not have got set up correctly
                if (OpenDoPEDetection.configIfPresent(Globals.ThisAddIn.Application.ActiveDocument))
                {
                    log.Debug("OpenDoPE detected");
                    enable(false);
                }
            }

            if (extendedDocument.Tag != null
                && extendedDocument.Tag is FabDocxState)
            {
                log.Debug("found FabDocxState");
                return (FabDocxState)extendedDocument.Tag;
            }
            else
            {
                log.Debug("no FabDocxState attached to this docx");
                return null;
            }


        }

        #endregion

        public Boolean IsAddDoPEEnabled(Office.IRibbonControl control)
        {
            // Can't enable if no documents are open
            if (Globals.ThisAddIn.Application.Documents.Count == 0) return false;

            // When the user opens an existing document, we create the
            // FabDocxState iff our parts are present.  

            // So if there is a FabDocxState for this docx, disable the button;
            // otherwise, enable it
            if (getState() == null) return true;
            else return false;
        }

        /// <summary>
        /// Call this in order to add custom xml parts to a document.
        /// Once they have been added, it should be greyed out.
        /// </summary>
        /// <param name="control"></param>
        public void buttonAddDoPE_Click(Office.IRibbonControl control)
        {
            myInvalidate(); 


            enable(true);

        }

        public void enable(bool newDocument)
        {

            string majorVersionString = Globals.ThisAddIn.Application.Version.Split(new char[] { '.' })[0];
            majorVersion = Convert.ToInt32(majorVersionString);

            /* String xml = System.IO.File.ReadAllText(@"C:\Users\jharrop\workspace\docx4j_1\sample-docs\aaa Document.xml");
            object missing = System.Reflection.Missing.Value;
            Globals.ThisAddIn.Application.Selection.InsertXML(xml, ref missing);
             */
            Word.Document document = Globals.ThisAddIn.Application.ActiveDocument;

            Microsoft.Office.Tools.Word.Document extendedDocument
                = Globals.ThisAddIn.Application.ActiveDocument.GetVstoObject(Globals.Factory);

            extendedDocument.ContentControlBeforeDelete += new Word.DocumentEvents2_ContentControlBeforeDeleteEventHandler(extendedDocument_ContentControlBeforeDelete);
            extendedDocument.ContentControlAfterAdd += new Word.DocumentEvents2_ContentControlAfterAddEventHandler(extendedDocument_ContentControlAfterAdd);

            extendedDocument.ContentControlOnEnter +=new Word.DocumentEvents2_ContentControlOnEnterEventHandler(extendedDocument_ContentControlOnEnter);
            extendedDocument.ContentControlOnExit += new Word.DocumentEvents2_ContentControlOnExitEventHandler(extendedDocument_ContentControlOnExit);
            extendedDocument.ContentControlBeforeStoreUpdate += new Word.DocumentEvents2_ContentControlBeforeStoreUpdateEventHandler(extendedDocument_ContentControlBeforeStoreUpdate);

            extendedDocument.BeforeRightClick += new Microsoft.Office.Tools.Word.ClickEventHandler(extendedDocument_BeforeRightClick);

            extendedDocument.BuildingBlockInsert += new Word.DocumentEvents2_BuildingBlockInsertEventHandler(extendedDocument_BuildingBlockInsert);

            extendedDocument.Shutdown += new EventHandler(Globals.ThisAddIn.extendedDocument_Shutdown);

            FabDocxState fabDocxState = new FabDocxState();
            extendedDocument.Tag = fabDocxState;

            PartCreator pc = null;

            if (newDocument)
            {
                log.Info("OpenDoPE parts not detected; adding now.");
                // Questions - I expect most users will want these,
                // since this particular add-in is targeted at the 
                // interactive case,
                // but some won't .. some people doing non-interactive
                // processing won't have their own xml formats.
                // Design decision: always have questions part,
                // as a way of documenting what the answers mean.
                // (Better than to put the description in the answer part)
                // TODO: add meta description to questions.xsd
                pc = new PartCreator();
                pc.process(true);
            }
            else
            {
                // Register content controls
                foreach (Word.ContentControl ccx in Globals.ThisAddIn.Application.ActiveDocument.ContentControls)
                {
                    fabDocxState.registerKnownSdt(ccx);
                }
            }

            fabDocxState.model = Model.ModelFactory(document);
            if (fabDocxState.model.answersPart == null)
            {
                Office.CustomXMLPart answerPart = pc.createAnswersPart(document);

                fabDocxState.model = Model.ModelFactory(document);
                //fabDocxState.model.userParts.Add(answerPart);
                fabDocxState.model.answersPart = answerPart;
                //log.Info(answersXml);
            }
            else
            {
                log.Info("Using existing answers: " + fabDocxState.model.answersPart.XML);
                fabDocxState.model.answersPart.NamespaceManager.AddNamespace("oda", "http://opendope.org/answers");
            }


            // Now we have model, we can:
            try
            {
                fabDocxState.initTaskPane(document);
            }
            catch (Exception e)
            {
                log.Error(e);
                /*
                 * The Stack Trace is:
                      at System.Runtime.InteropServices.Marshal.IsComObject(Object o)
                      at System.Runtime.InteropServices.ComAwareEventInfo.AddEventHandler(Object target, Delegate handler)
                      at Microsoft.Office.Tools.CustomTaskPaneImpl..ctor(_CustomTaskPane customTaskPane, UserControl control)
                      at Microsoft.Office.Tools.CustomTaskPaneCollectionImpl.AddHelper(UserControl control, String title, Object window)
                      at Microsoft.Office.Tools.CustomTaskPaneCollectionImpl.Add(UserControl control, String title, Object window
                 */ 
                // No matter; we do it later
            }
            //MessageBox.Show("task pane inited");

            // Load FabDocx.dotx
            GlobalTemplateManager.GetGlobalTemplateManager();

        }

        void extendedDocument_BeforeRightClick(object sender, Microsoft.Office.Tools.Word.ClickEventArgs e)
        {
            // compare app_WindowBeforeRightClick(Word.Selection Sel, ref bool Cancel)

            log.Debug("right-clicked.");
            //e.Cancel = true;

            //e.Selection

            // If they are in a content control,

            // .. and its databound, offer them "edit question" menu only

            // .. and its a repeat, add "edit repeat"; add repeat; logic explorer

            // .. and its a condition, add edit condition; logic explorer

        }

        /// <summary>
        /// This event fires when the user exits a bound content control, before the onExit event.
        /// ie it fires once, not for each keystroke.
        /// 
        /// Warn the user if they type new lines, since chances are they are in the wrong place in the document.
        /// </summary>
        void extendedDocument_ContentControlBeforeStoreUpdate(Word.ContentControl cc, ref string Content)
        {
            log.Debug("Updating store with string: '" + Content + "', for cc " + cc.Title);
            if (Content.Contains("\n"))
            {
                // carriage return is /n; there is no /r
                MessageBox.Show("You typed inside an Answer control, which you probably didn't want to do.  Ctrl Z to Undo. \n\n Press 'Show Tags' if you need to see exactly where your content controls start and finish. ");
                // NB Ctrl Z causes the content control to be deleted, then added again! ie those events fire.
            }
        }


        Word.Application getWordApp()
        {
            if (wdApp == null)
            {
                //wdApp = new Word.Application();
                wdApp = Globals.ThisAddIn.Application;
            }
            return wdApp;
        }

        bool enableQChangeButtons = false;

        Word.ContentControl currentCC = null;

        public void extendedDocument_ContentControlOnEnter(Word.ContentControl cc)
        {
            /* The on enter / on exit events seem to do what you'd expect,
             * at least in Office 2010:
             * 
             * - only the event for the current (ie most deeply nested) cc fires
             * - the exit event for one fires before the enter event for another
             */ 

            log.Info("Entered cc: " + cc.Title + " of type " + cc.Type);
            currentCC = cc;
            if ( cc.Tag!=null && ((new TagData(cc.Tag)).getXPathID()!=null) ) {
                enableQChangeButtons = true;
            }
            myInvalidate();
        }

        void extendedDocument_ContentControlOnExit(Word.ContentControl cc, ref bool Cancel)
        {
            log.Info(".. left cc: " + cc.Title);
            currentCC = null;
            enableQChangeButtons = false;
            myInvalidate();
        }

        public void extendedDocument_ContentControlAfterAdd(Word.ContentControl NewContentControl, bool InUndoRedo)
        {

            log.Info("add fired for " + NewContentControl.ID + " with tag " + NewContentControl.Tag);


            if (NewContentControl.Tag != null
                && NewContentControl.Tag.Contains("od:source=library"))
            {
                // If it is a building block insert, note that this event fires first.
                // We don't want this event detecting it as a copy, and deleting 
                // unrecognised logic, when the logic we need is about to be added
                // (via building block insert event).
                log.Debug("ContentControlAfterAdd, ignoring for building block insertion");
                return;
            }

            FabDocxState fabDocxState = getState();

            // Can't refresh taskpane here;
            // it results in:
            //OpenDope_AnswerFormat.Ribbon.extendedDocument_ContentControlAfterAdd add fired for 3909469227 with tag 
            //A first chance exception of type 'System.Runtime.InteropServices.COMException' occurred in OpenDope_AnswerFormat.DLL
            //OpenDope_AnswerFormat.Controls.LogicTaskPaneUserControl.populateLogicInUse System.Runtime.InteropServices.COMException (0x800A172A): The object is not valid.
            //   at Microsoft.Office.Interop.Word.Range.get_WordOpenXML()
            //   at OpenDope_AnswerFormat.Helpers.OpcHelper.GetPackageStreamFromRange(Range range)

            if (InUndoRedo)
            {
                log.Debug("InUndoRedo, ignoring cc add event");
            }
            else if (fabDocxState.inPlutextAdd)
            {
                // OK, my code added it.
                fabDocxState.registerKnownSdt(NewContentControl);
                log.Debug("ignoring cc add event");
                fabDocxState.inPlutextAdd = false;
            }
            else if (fabDocxState.areEventsSuppressed(NewContentControl))
            {
                log.Info("Ignored event for descendant CC " + NewContentControl.ID
                    + " " + NewContentControl.Tag + " " + NewContentControl.Title);
            }
            else if (fabDocxState.isKnownSdt(NewContentControl))
            {
                log.Debug("ContentControlAfterAdd, handling as Move");
                if (majorVersion >= 14)
                {
                    getWordApp().UndoRecord.StartCustomRecord("FabDocx Move content control");
                }

                // its a move
                ContentControlMoveHandler handler = new ContentControlMoveHandler();
                handler.handle(NewContentControl);
                if (majorVersion >= 14)
                {
                    getWordApp().UndoRecord.EndCustomRecord();
                }

            }
            else
            {
                log.Debug("ContentControlAfterAdd, handling as Copy");
                if (majorVersion >= 14)
                {
                    getWordApp().UndoRecord.StartCustomRecord("FabDocx Copy content control");
                }

                // its a copy
                ContentControlCopyHandler handler = new ContentControlCopyHandler();
                handler.handle(NewContentControl);
                if (majorVersion >= 14)
                {
                    getWordApp().UndoRecord.EndCustomRecord();
                }
            }


        }

        public void extendedDocument_ContentControlBeforeDelete(Word.ContentControl OldContentControl, bool InUndoRedo)
        {
            log.Info("delete fired for " + OldContentControl.ID + " with tag " + OldContentControl.Tag);

            // Refresh task pane in a new thread
            // results in System.InvalidOperationException was unhandled
            // Message=Cross-thread operation not valid: Control 'treeViewLogicUsed' accessed from a thread other than the thread it was created on.
            // See further http://msdn.microsoft.com/en-us/library/ms171728

            FabDocxState fabDocxState = getState();
            if (fabDocxState.TaskPane.Visible)
            {
                Controls.LogicTaskPaneUserControl ltp = (Controls.LogicTaskPaneUserControl)fabDocxState.TaskPane.Control;
                ltp.Dirty = true;
            }
        }

        //static void refreshTaskpane(FabDocxState fabDocxState)
        //{
        //    System.Threading.Thread.Sleep(300);

        //    Controls.LogicTaskPaneUserControl ltp = (Controls.LogicTaskPaneUserControl)fabDocxState.TaskPane.Control;
        //    ltp.populateLogicInUse();
        //}

    public void buttonNarrativeAdd_Click(Office.IRibbonControl control)
        {
            Word.Document document = Globals.ThisAddIn.Application.ActiveDocument;
            //Model model = Model.ModelFactory(document);

            FabDocxState fabDocxState = getState();
            fabDocxState.inPlutextAdd = true;

            Word.UndoRecord ur = getWordApp().UndoRecord;
            if (majorVersion >= 14)
            {
                getWordApp().UndoRecord.StartCustomRecord("FabDocx Add Narrative");
            }


            // Create control
            Word.ContentControl cc = null;
            object missing = System.Type.Missing;
            try
            {
                cc = document.ContentControls.Add(Word.WdContentControlType.wdContentControlRichText, ref missing);
                cc.Title = "Narrative only";
                cc.Tag = "od:narrative";
                cc.SetPlaceholderText(null, null, 
                    "Narrative text. Text entered here will appear in the interview, but not the resulting document."); 
            }
            catch (System.Exception)
            {
                if (majorVersion >= 14)
                {
                    getWordApp().UndoRecord.EndCustomRecord();
                }
                MessageBox.Show("Selection must be either part of a single paragraph, or one or more whole paragraphs");
                fabDocxState.inPlutextAdd = false;
                return;
            }

            if (majorVersion >= 14)
            {
                getWordApp().UndoRecord.EndCustomRecord();
            }

            if (fabDocxState.TaskPane.Visible)
            {
                Controls.LogicTaskPaneUserControl ltp = (Controls.LogicTaskPaneUserControl)fabDocxState.TaskPane.Control;
                ltp.populateLogicInUse();
            }

        }
        public void buttonBind_Click(Office.IRibbonControl control)
        {
            try
            {
                Word.Document document = Globals.ThisAddIn.Application.ActiveDocument;


                //Model model = Model.ModelFactory(document);

                FabDocxState fabDocxState = getState();
                fabDocxState.inPlutextAdd = true;

                if (majorVersion>=14)
                {
                    getWordApp().UndoRecord.StartCustomRecord("FabDocx Insert Question");
                }

                // Create control
                Word.ContentControl cc = null;
                object missing = System.Type.Missing;
                try
                {
                    cc = document.ContentControls.Add(Word.WdContentControlType.wdContentControlText, ref missing);
                    cc.MultiLine = true;
                }
                catch (System.Exception e)
                {
                    if (majorVersion>=14)
                    {
                        getWordApp().UndoRecord.EndCustomRecord();
                    }
                    log.Error(e);
                    MessageBox.Show("Selection must be either part of a single paragraph, or one or more whole paragraphs");
                    fabDocxState.inPlutextAdd = false;
                    return;
                }



                // Ask question - form to have button which says "use existing Q/A"
                // or a combobox presenting
                // a default id, or choice from drop down?
                // Since we only want to create a new answer if it is a new question,
                // present the question form first.
                FormQA formQA = new FormQA(cc);
                formQA.ShowDialog();
                formQA.Dispose();

                if (majorVersion >= 14)
                {
                    getWordApp().UndoRecord.EndCustomRecord();
                }

                // It also makes the answer format more friendly if the id we give
                // an answer happens to be the same as the relevant question
                // (though not mandatory, since it is the xpath entry which ties
                //  the 2 together)

                if (fabDocxState.TaskPane.Visible)
                {
                    Controls.LogicTaskPaneUserControl ltp = (Controls.LogicTaskPaneUserControl)fabDocxState.TaskPane.Control;
                    ltp.populateLogicInUse();
                }
            }
            catch (System.Exception e2)
            {
                log.Error(e2);
                MessageBox.Show("Something went wrong.  Check the logs.");
            }
        }

        public void buttonEditQuestion_Click(Office.IRibbonControl control)
        {

            FabDocxState fabDocxState = getState();
            if (currentCC.Tag != null) 
            {
                String XPathID = (new TagData(currentCC.Tag)).getXPathID();
                if (XPathID != null)
                {
                    // Get the xpath obj
                    XPathsPartEntry xppe = new XPathsPartEntry(fabDocxState.model);

                    xpathsXpath xpo = xppe.getXPathByID(XPathID);

                    if (xpo.questionID != null)
                    {
                        Forms.FormQuestionEdit fqe = new Forms.FormQuestionEdit(xpo.questionID);
                        fqe.ShowDialog();
                        fqe.Dispose();
                    }
                    else
                    {
                        log.Debug("No question associated with xpath " + xpo.dataBinding.xpath);
                    }
                }

            }

            if (fabDocxState.TaskPane.Visible)
            {
                Controls.LogicTaskPaneUserControl ltp = (Controls.LogicTaskPaneUserControl)fabDocxState.TaskPane.Control;
                ltp.populateLogicInUse();
            }

        }


        public void buttonCondition_Click(Office.IRibbonControl control)
        {
            // conditions .. 
            // (a) simple answer
            // (b) xpath?? Advanced mode?
            // (c) condition editor .. c# gui for boolean and/or/not of existing answers

            Word.Document document = Globals.ThisAddIn.Application.ActiveDocument;
            //Model model = Model.ModelFactory(document);

            FabDocxState fabDocxState = getState();
            fabDocxState.inPlutextAdd = true;

            if (majorVersion >= 14)
            {
                getWordApp().UndoRecord.StartCustomRecord("FabDocx Add Condition");
            }


            // Create control
            Word.ContentControl cc = null;
            object missing = System.Type.Missing;
            try
            {
                cc = document.ContentControls.Add(Word.WdContentControlType.wdContentControlRichText, ref missing);               
            }
            catch (System.Exception e)
            {
                if (majorVersion >= 14)
                {
                    getWordApp().UndoRecord.EndCustomRecord();
                }
                log.Error(e);
                MessageBox.Show("Selection must be either part of a single paragraph, or one or more whole paragraphs");
                fabDocxState.inPlutextAdd = false;
                return;
            }

            Forms.FormConditionBuilder formCondition = new Forms.FormConditionBuilder(cc, null, null);
            DialogResult result = formCondition.ShowDialog();
            if (result == DialogResult.Cancel)
            {
                cc.Delete();
            }

            //FormCondition formCondition = new FormCondition(cc, null, null);
            //if (formCondition.preconditionsMet())
            //{
            //    formCondition.ShowDialog();
            //    if (!formCondition.postconditionsMet)
            //    {
            //        cc.Delete();
            //    }
            //}
            //else
            //{
            //    cc.Delete();
            //}
            formCondition.Dispose();

            if (majorVersion >= 14)
            {
                getWordApp().UndoRecord.EndCustomRecord();
            }

            if (fabDocxState.TaskPane.Visible)
            {
                Controls.LogicTaskPaneUserControl ltp = (Controls.LogicTaskPaneUserControl)fabDocxState.TaskPane.Control;
                ltp.populateLogicInUse();
            }

        }

        public void buttonConditionEdit_Click(Office.IRibbonControl control)
        {
            FabDocxState fabDocxState = getState();
            if (currentCC.Tag != null)
            {
                String conditionID = (new TagData(currentCC.Tag)).getConditionID();
                if (conditionID != null)
                {
                    ConditionsPartEntry cpe = new ConditionsPartEntry(fabDocxState.model);

                    condition c = cpe.getConditionByID(conditionID);

                    FormCondition formCondition = new FormCondition(currentCC, cpe, c);
                    formCondition.ShowDialog();
                    formCondition.Dispose();
                }
            }

            if (fabDocxState.TaskPane.Visible)
            {
                Controls.LogicTaskPaneUserControl ltp = (Controls.LogicTaskPaneUserControl)fabDocxState.TaskPane.Control;
                ltp.populateLogicInUse();
            }

        }

        public void buttonRepeat_Click(Office.IRibbonControl control)
        {
            if (majorVersion >= 14)
            {
                getWordApp().UndoRecord.StartCustomRecord("FabDocx Repeat");
            }


            RepeatButtonAction rba = new RepeatButtonAction();
            rba.buttonRepeat_Click(control);

            if (majorVersion >= 14)
            {
                getWordApp().UndoRecord.EndCustomRecord();
            }

            FabDocxState fabDocxState = getState();
            if (fabDocxState.TaskPane.Visible)
            {
                Controls.LogicTaskPaneUserControl ltp = (Controls.LogicTaskPaneUserControl)fabDocxState.TaskPane.Control;
                ltp.populateLogicInUse();
            }

        }

        public void buttonFormat_Click(Office.IRibbonControl control)
        {
            /* You can only format a date or a number here.
             * 
             * For a date, we use cc date stuff:
             * 
                <w:sdt>
                  <w:sdtPr>
                    <w:date w:fullDate="2012-07-13T00:00:00Z">
                      <w:dateFormat w:val="dddd, d MMMM yyyy"/>
                      <w:lid w:val="en-AU"/>
                      <w:storeMappedDataAs w:val="date"/>
                      <w:calendar w:val="gregorian"/>
                    </w:date>
                  </w:sdtPr>
             * 
             * For a number, ...
             */

            if (currentCC.Tag != null)
            {
                String XPathID = (new TagData(currentCC.Tag)).getXPathID();
                if (XPathID != null)
                {
                    // Get the xpath obj
                    FabDocxState fabDocxState = getState();
                    XPathsPartEntry xppe = new XPathsPartEntry(fabDocxState.model);

                    xpathsXpath xpo = xppe.getXPathByID(XPathID);

                    if (xpo.type != null)
                    {
                        if (xpo.type.Equals("date"))
                        {
                            Forms.FormFormatDate formDate = new Forms.FormFormatDate(currentCC);
                            formDate.ShowDialog();
                            formDate.Dispose();
                        }

                        //else if (xpo.type.Equals("decimal")
                        //    || xpo.type.Equals("integer")
                        //    || xpo.type.Equals("positiveInteger")
                        //    || xpo.type.Equals("nonPositiveInteger")
                        //    || xpo.type.Equals("negativeInteger")
                        //    || xpo.type.Equals("nonNegativeInteger")
                        //    )
                        //{
                        //    return true;
                        //}
                    }
                }
            }


        }

        public void buttonBuildingBlockSave_Click(Office.IRibbonControl control)
        {
            Word.Document document = Globals.ThisAddIn.Application.ActiveDocument;

            GlobalTemplateManager.GetGlobalTemplateManager().createBuildingBlock(
                Globals.ThisAddIn.Application.Selection.Range);

            document.Activate();

            // myInvalidate();

        }

        void extendedDocument_BuildingBlockInsert(Word.Range Range, string Name, 
            string Category, string BlockType, string Template)
        {
            log.Debug("building block inserted:" + Name + " from " + Template);

            // NB extendedDocument_ContentControlAfterAdd fires first (tested in Word 2010).
            
            // if its one of ours ..
            if (Template.EndsWith(GlobalTemplateManager.FABDOCX_ADDIN))
            {
                log.Debug("Range contains: " + Range.Text);

                log.Debug("Range contains cc's: " + Range.ContentControls.Count);

                Word.Document document = Globals.ThisAddIn.Application.ActiveDocument;
                //log.Debug(document.Name);

                // It has been inserted
                // Do this in a separate thread, after this event has finished, in order to avoid
                //   This object model command is not available while in the current event.
                // when opening the template.
                System.Threading.Thread t = new System.Threading.Thread( delegate()
                { CopyBuildingBlockLogic(document, Range, (FabDocxState)document.GetVstoObject(Globals.Factory).Tag  ); }
                    // GetVstoObject is called here, rather than via getState(), since that results in:
                    //     System.InvalidCastException was unhandled Message=Unable to cast COM object of type 'System.__ComObject' 
                    //     to interface type 'Microsoft.VisualStudio.Tools.Office.Runtime.Interop.IHostItemFactoryNoMAF'. This 
                    //     operation failed because the QueryInterface call on the COM component for the interface with IID 
                    //     '{A0885C0A-33F2-4890-8F29-25C8DE7808F1}' failed due to the following error: No such interface 
                    //     supported (Exception from HRESULT: 0x80004002 (E_NOINTERFACE))
                    );
                t.Start();
            }
        }

        static void CopyBuildingBlockLogic(Word.Document document, Word.Range Range, FabDocxState fabDocxState)
        {
            System.Threading.Thread.Sleep(300);

            // We need to copy the logic over
            GlobalTemplateManager.GetGlobalTemplateManager().copyBuildingBlockLogic(document, Range);

            // If its being copied into a repeat, either:
            // 1. tell them sorry, can't be done; or
            // 2. move the repeated variables (and xpaths..) into that repeat!

            document.Activate();

            if (fabDocxState.TaskPane.Visible)
            {
                Controls.LogicTaskPaneUserControl ltp = (Controls.LogicTaskPaneUserControl)fabDocxState.TaskPane.Control;
                ltp.populateLogicInUse();
            }

        }

        public void buttonQuestionOrder_Click(Office.IRibbonControl control)
        {

            FabDocxState fabDocxState = getState();
            Forms.FormAnswerOrder fao = new Forms.FormAnswerOrder(fabDocxState.model.answersPart, fabDocxState.model.questionsPart);
            if (fao.ShowDialog().Equals(DialogResult.OK))
            {
                fao.apply();
            }
            fao.Dispose();

        }


        public void buttonLaunchBrowser_Click(Office.IRibbonControl control)
        {
            // Save the document locally

            // Launch IE .. see http://www.motobit.com/tips/detpg_uploadvbaie/
            //Browser browser = new Browser();
            //browser.ShowDialog();
            //browser.Dispose();

            string target = "http://www.fabdocx.com/library/upload.html";

            try
            {
                System.Diagnostics.Process.Start(target);
            }
            catch
                (
                 System.ComponentModel.Win32Exception noBrowser)
            {
                if (noBrowser.ErrorCode == -2147467259)
                    MessageBox.Show(noBrowser.Message);
            }
            catch (System.Exception other)
            {
                MessageBox.Show(other.Message);
            }

        }

        public void buttonSingleParagraph_Click(Office.IRibbonControl control)
        {

            Word.Selection sel = Globals.ThisAddIn.Application.Selection;
            Word.Find findObj = sel.Find;

            object missing = System.Reflection.Missing.Value;

            object findtext = "^p";
            object findreplacement = " ";

            //object f = false;

            object findforward = false;
            object findformat = true;
            object findwrap = Microsoft.Office.Interop.Word.WdFindWrap.wdFindStop;
            object findmatchcase = false;
            object findmatchwholeword = false;
            object findmatchwildcards = false;
            object findmatchsoundslike = false;
            object findmatchallwordforms = false;
            object findreplace = Microsoft.Office.Interop.Word.WdReplace.wdReplaceAll;

            sel.Find.Execute(ref findtext, ref findmatchcase, ref findmatchwholeword,
                ref findmatchwildcards,
                ref findmatchsoundslike, ref findmatchallwordforms, ref findforward,
                ref findwrap, ref findformat, ref findreplacement, ref findreplace, ref missing, ref missing,
                ref missing, ref missing);

        }

        /// <summary>
        /// An inline rich text control can't contain carriage returns.
        /// 
        /// For a condition, its useful to be able to split it across paragraphs.
        /// 
        /// Not so for a repeat, since the result won't be what you want.
        /// So TODO greyout if there is a repeat ancestor.
        /// 
        /// (Also, sometimes, Word converts a rich text content control to plain text???)
        /// </summary>
        /// <param name="control"></param>
        public void buttonParagraphAdd_Click(Office.IRibbonControl control)
        {
            if (currentCC.XMLMapping.IsMapped) return;

            Word.Selection sel = Globals.ThisAddIn.Application.Selection;
            if (sel.Range.End - sel.Range.Start > 0)
            {
                MessageBox.Show("Position your cursor without selecting anything, and try again.");
                return;
            }


            if (currentCC.Type.Equals(Word.WdContentControlType.wdContentControlText))
            {
                currentCC.Type = Word.WdContentControlType.wdContentControlRichText;

                //Word.Document document = Globals.ThisAddIn.Application.ActiveDocument;
                log.Warn("Converted a plain text control " + currentCC.Tag + " back to rich text!");
            }

            if (majorVersion >= 14)
            {
                getWordApp().UndoRecord.StartCustomRecord("FabDocx Split paragraph");
            }


            object unitParagraph = Word.WdUnits.wdParagraph;
            object back1 = -1;
            object forward1 = 1;

            Word.Document document = Globals.ThisAddIn.Application.ActiveDocument;

            // An inline rich text control can't take a paragraph mark.
            // So we need an algorithm for converting it into 2.
            // Can't just cut first half of para and paste it; the pasted bit
            // won't contain any cc's which spanned the point.

            // Approach:
            //  .. Get the paragraph
            // .. Make a copy
            // .. in the copy, delete up to our position
            // .. in the original, delete after our position

            Word.Range splittingPoint = sel.Range;

            object para1DeleteStartPoint = splittingPoint.Start;

            Word.Range paraOrig = Globals.ThisAddIn.Application.ActiveDocument.Range(ref para1DeleteStartPoint, ref para1DeleteStartPoint);
            paraOrig.MoveStart(ref unitParagraph, ref back1);

            int lengthStartSegment = paraOrig.End - paraOrig.Start;

            object startPoint = paraOrig.Start;

            paraOrig.MoveEnd(ref unitParagraph, ref forward1);

            object endPoint = paraOrig.End;
            object endPointPlusOne = paraOrig.End + 1;

            // copy it
            Word.Range insertPoint = Globals.ThisAddIn.Application.ActiveDocument.Range(ref endPoint, ref endPoint);
            paraOrig.Copy();
            insertPoint.Paste();

            // In the copy, delete the first half
            // (do this operation first, to preserve our original position calculations)
            object para2DeleteEndpoint = (int)endPoint + lengthStartSegment;
            Word.Range para2Deletion = Globals.ThisAddIn.Application.ActiveDocument.Range(ref endPoint, ref para2DeleteEndpoint);
            para2Deletion.Delete();

            // In the original, delete the second half
            Word.Range para1Deletion = Globals.ThisAddIn.Application.ActiveDocument.Range(ref para1DeleteStartPoint, ref endPoint);
            para1Deletion.Delete();

            if (majorVersion >= 14)
            {
                getWordApp().UndoRecord.EndCustomRecord();
            }

        }


        ///// <summary>
        ///// If user clicks close [x] on a task pane, that sets its visible=false.
        ///// This sets visible=true again.
        ///// </summary>
        ///// <param name="control"></param>
        //public void buttonShowCTP_Click(Office.IRibbonControl control)
        //{
        //    // TODO - only enable this if the custom xml parts
        //    // are present (and the ctp is not visible already).
        //    // Could use visibility changed event to update button

        //    Microsoft.Office.Tools.CustomTaskPane ctp = findCustomTaskPane();

        //    // This shouldn't happen.  If it does, we'd need to recreate
        //    if (ctp == null)
        //    {
        //        //Mbox.ShowSimpleMsgBoxInfo("Task pane not found");
        //        return;
        //    }

        //    if (ctp.Visible)
        //    {
        //        //Mbox.ShowSimpleMsgBoxInfo("I reckon its visible already");
        //        return;
        //    }

        //    ctp.Visible = true;
        //}

        //public Boolean IsShowCTPEnabled(Office.IRibbonControl control)
        //{
        //    Microsoft.Office.Tools.CustomTaskPane ctp = findCustomTaskPane();
        //    if (ctp == null) 
        //    {
        //        // Parts haven't been added to this docx, so
        //        // can't display the CTP
        //        return false;
        //    }

        //    if (ctp.Visible)
        //    {
        //        return false;
        //    }
        //    else { return true; }
        //}

        //private static Microsoft.Office.Tools.CustomTaskPane findCustomTaskPane() {

        //    Microsoft.Office.Tools.Word.Document extendedDocument
        //        = Globals.ThisAddIn.Application.ActiveDocument.GetVstoObject(Globals.Factory);

        //    if (extendedDocument.Tag != null
        //        && extendedDocument.Tag is Microsoft.Office.Tools.CustomTaskPane)
        //    {
        //        log.Debug("found ctp");
        //        return (Microsoft.Office.Tools.CustomTaskPane)extendedDocument.Tag;
        //    }
        //    else
        //    {
        //        log.Debug("no ctp attached to this docx");
        //        return null;
        //    }
        //}

        //private static Microsoft.Office.Tools.CustomTaskPane findCustomTaskPane()
        //{
        //    Word.Window wn = Globals.ThisAddIn.Application.ActiveWindow;

        //    WedTaskPane wedTaskPane = null;


        //    foreach (Microsoft.Office.Tools.CustomTaskPane ctpx in Globals.ThisAddIn.CustomTaskPanes)
        //    {

        //        if (((Word.Window)ctpx.Window) == wn
        //            && ctpx.Control is WedTaskPane)
        //        {
        //            log.Debug("found ctp");

        //            wedTaskPane = (WedTaskPane)ctpx.Control;

        //            // Verify, since if a doc with a ctp attached is closed,
        //            // the ctp is moved to another doc!
        //            if (!wedTaskPane.windowCaption.Equals("OpenDoPE " + wn.Caption))
        //            {
        //                log.Debug("but its obsolete...");
        //                continue;
        //            }

        //            return ctpx;
        //        }
        //        else
        //        {
        //            //log.Debug("..not ctp window");
        //        }
        //    }
        //    return null;
        //}

        //public void buttonInjectMacro_Click(Office.IRibbonControl control)
        //{
        //    //MacroManager.injectMacro();
        //}

        //public Boolean isInjectMacroEnabled(Office.IRibbonControl control)
        //{
        //    // TODO, grey out once macro has been added
        //    if (findCustomTaskPane() == null) return false;
        //    else return true;
        //}

        public void buttonOptions_Click(Office.IRibbonControl control)
        {
            //MacroManager.injectMacro();
        }

        public void buttonLogicExplorer_Click(Office.IRibbonControl control, bool visible)
        {
            FabDocxState fabDocxState = getState();

            if (fabDocxState.TaskPane == null)
            {
                //MessageBox.Show("fabDocxState.TaskPane was null!!!");
                // This happens when Word is launched by double clicking on a docx.
                //Where initTaskPane failed earlier (for unknown reasons) we do it here...
                fabDocxState.initTaskPane(Globals.ThisAddIn.Application.ActiveDocument);
            }


            if (visible)
            {
                Controls.LogicTaskPaneUserControl ltp = (Controls.LogicTaskPaneUserControl)fabDocxState.TaskPane.Control;
                //if (ltp == null)
                //{
                //    MessageBox.Show("ltp was null!!!");
                //}
                ltp.populateLogicInUse();
            }
            fabDocxState.TaskPane.Visible = visible;

        }



// -------------------------

        //public Boolean isToggleAllInParagraphEnabled(Office.IRibbonControl control)
        //{
        //}


        /**********************************************************
         *     Help, Feedback etc
         **********************************************************/

/*
        public void buttonHelp_Click(Office.IRibbonControl control)
        {
            string dir = Assembly.GetExecutingAssembly().CodeBase;
            log.Debug("Locating help file relative to " + dir);
            dir = dir.Replace(@"file:///", string.Empty);
            int lastPost = dir.LastIndexOf("/");
            dir = dir.Substring(0, lastPost) + "/help/";
            dir = dir.Replace("/", "\\");

            object fileName = dir + "Results Springboard Help.docx";

            try
            {
                object omissing = System.Reflection.Missing.Value;
                object addToRecentFilesObj = false;

                Word.Document resultDoc = Globals.ThisAddIn.Application.Documents.Open(ref fileName, ref omissing, ref omissing, ref addToRecentFilesObj, ref omissing,
                                                 ref omissing, ref omissing, ref omissing, ref omissing, ref omissing, ref omissing,
                                                 ref omissing, ref omissing, ref omissing, ref omissing, ref omissing);
                resultDoc.Activate();
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show("Unable to open local Help file.  Please visit our help online at www.plutext.com");
                log.Error("Unable to open '" + fileName);
                log.Error(e.Message);
                return;
            }


        }

        public void buttonFeedback_Click(Office.IRibbonControl control)
        {
            if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                System.Windows.Forms.MessageBox.Show("You don't appear to have an internet connection right now.  Please try again later.");
                return;
            }

            FormFeedback ff = new FormFeedback();
            ff.ShowDialog();
        }

*/

        /* To have just a simple next button, which turns into a menu
         * if held for long enough, see http://www.keyongtech.com/569303-how-to-tell-how-long
         * 
         * Essentially, you use System.Windows.Forms.MouseEventHandler
         * 
         * Would need to position the menu properly.
         */

        #region Enabled

        public Boolean IsNarrativeEnabled(Office.IRibbonControl control)
        {
            if (getState() == null) return false;
            else return true;
        }

        public Boolean IsBindEnabled(Office.IRibbonControl control)
        {
            if (getState() == null) return false;
            else return true;
        }
        public Boolean IsEditQuestionEnabled(Office.IRibbonControl control)
        {
            if (getState() == null) return false;
            return this.enableQChangeButtons;
        }
        public Boolean IsFormatEnabled(Office.IRibbonControl control)
        {
            if (getState() == null) return false;

            // Enable for date and (TODO) numbers
            if (currentCC!=null && currentCC.Tag != null)
            {
                String XPathID = (new TagData(currentCC.Tag)).getXPathID();
                if (XPathID != null)
                {
                    // Get the xpath obj
                    FabDocxState fabDocxState = getState();
                    XPathsPartEntry xppe = new XPathsPartEntry(fabDocxState.model);

                    xpathsXpath xpo = xppe.getXPathByID(XPathID);

                    if (xpo.type != null)
                    {
                        if (xpo.type.Equals("date"))
                        {
                            return true;
                        }

                        //else if (xpo.type.Equals("decimal")
                        //    || xpo.type.Equals("integer")
                        //    || xpo.type.Equals("positiveInteger")
                        //    || xpo.type.Equals("nonPositiveInteger")
                        //    || xpo.type.Equals("negativeInteger")
                        //    || xpo.type.Equals("nonNegativeInteger")
                        //    )
                        //{
                        //    return true;
                        //}
                    }
                }
            }
            return false;
        }

        public Boolean IsSingleParagraphEnabled(Office.IRibbonControl control)
        {
            return true;

            //if (Globals.ThisAddIn.Application.Selection.Paragraphs.Count > 0)
            //{

            //}
        }

        public Boolean IsParagraphAddEnabled(Office.IRibbonControl control)
        {
            if (getState() == null) return false;

            if (currentCC != null && currentCC.Tag != null)
            {
                TagData td = (new TagData(currentCC.Tag));
                if (td!= null
                    && (td.getConditionID()!=null ))
                {
                    return true;
                }
            }
            return false;
        }


        public Boolean IsConditionEnabled(Office.IRibbonControl control)
        {
            if (getState() == null) return false;
            else return true;
        }
        public Boolean IsConditionEditEnabled(Office.IRibbonControl control)
        {
            return false;

            //if (getState() == null) return false;

            //if (currentCC==null) return false;

            //if (currentCC.Tag == null)
            //{
            //    return false;
            //} else {
            //    String conditionID = (new TagData(currentCC.Tag)).getConditionID();
            //    return (conditionID != null);
            //}
        }

        public Boolean IsRepeatEnabled(Office.IRibbonControl control)
        {
            if (getState() == null) return false;
            else return true;
        }

        public Boolean IsBuildingBlockSaveEnabled(Office.IRibbonControl control)
        {
            return true;
        }

        public Boolean IsQuestionOrderEnabled(Office.IRibbonControl control)
        {
            if (getState() == null) return false;
            else return true;
        }

        public Boolean IsLogicExplorerEnabled(Office.IRibbonControl control)
        {
            if (getState() == null) return false;
            else return true;
        }
        public Boolean IsOptionsEnabled(Office.IRibbonControl control)
        {
            return false;
        }

        #endregion

        #region Helpers

        private static string GetResourceText(string resourceName)
        {
            log.Debug("Fetching resource: " + resourceName);
            Assembly asm = Assembly.GetExecutingAssembly();
            string[] resourceNames = asm.GetManifestResourceNames();
            for (int i = 0; i < resourceNames.Length; ++i)
            {
                if (string.Compare(resourceName, resourceNames[i], StringComparison.OrdinalIgnoreCase) == 0)
                {
                    using (System.IO.StreamReader resourceReader = new System.IO.StreamReader(asm.GetManifestResourceStream(resourceNames[i])))
                    {
                        if (resourceReader != null)
                        {
                            return resourceReader.ReadToEnd();
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Need this if SearchProviders.isGoogleDesktopSearchAvailable
        ///        || SearchProviders.AvailableSearchProviders.Count > 1
        /// </summary>
        /// <returns></returns>
        //public string buttonSearchSplit_GetContent()
        //{
        //    log.Debug("in buttonSearchSplit_GetContent!");

        //    if (SearchProviders.AvailableSearchProviders.Count == 1
        //         && !SearchProviders.isGoogleDesktopSearchAvailable)
        //    {
        //        log.Debug(".. nothing to do");
        //        return null;
        //    }

        //    try
        //    {

        //        StringBuilder MyStringBuilder = new StringBuilder();


        //        for (int i = docState.index.Index + 1;
        //            ((i < dt.Rows.Count) && (i <= (docState.index.Index + DROPDOWN_MAX_ENTRIES))); i++)
        //        {

        //            MyStringBuilder.Append(@"<button id=""" + NEXT_PREFIX + i + @""" tag=""uri1"" label="""
        //                + dt.Rows[i].ItemArray[0] + @""" onAction=""buttonNextDoc_Click""  imageMso=""SignatureLineInsert"" />");
        //        }
        //        MyStringBuilder.Append(@"</menu>");
        //        return MyStringBuilder.ToString();
        //    }
        //    catch (Exception e)
        //    {
        //        log.Error(e.StackTrace);
        //        GlobalErrorHandler.HandleException(e);
        //        return null;
        //    }

        //}


        #endregion
    }
}
