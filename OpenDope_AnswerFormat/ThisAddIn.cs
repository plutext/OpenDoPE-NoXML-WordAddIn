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
using System.Text;
using System.Xml.Linq;
using Word = Microsoft.Office.Interop.Word;
using Office = Microsoft.Office.Core;
using Microsoft.Office.Tools.Word;
using NLog;
using NLog.Config;
using OpenDoPEModel;

using System.Windows.Forms;

namespace OpenDope_AnswerFormat
{
    //[System.Diagnostics.DebuggerNonUserCodeAttribute()]
    public partial class ThisAddIn
    {
        /*
         * Created as a Word 2007 Add-In project
         * (cf 2010), so that:
         * 
         * (i)  this.Application exists
         * (ii) can override override Microsoft.Office.Core.IRibbonExtensibility CreateRibbonExtensibilityObject()
         * 
         * If you are running on a Word 2010 machine, 
         * and you get message saying "can't run or debug",
         * Right click in the Add-in project properties; 
         * in the Debug tab, Start external program => Browse for the installed version of Word.
         */

        public Word.Application app;

        static Logger log;

        static ThisAddIn()
        {
            NLog.Config.LoggingConfiguration config = new NLog.Config.LoggingConfiguration();
            NLog.Targets.Target t;
            //System.Diagnostics.Trace.WriteLine(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
            // eg file:///C:/Users/jharrop/Documents/Visual Studio 2010/Projects/com.plutext.search/com.plutext.search.main/bin/Debug/com.plutext.search.main.DLL
            if (System.Reflection.Assembly.GetExecutingAssembly().CodeBase.Contains("Debug"))
            {
                t = new NLog.Targets.DebuggerTarget();
                ((NLog.Targets.DebuggerTarget)t).Layout = "${callsite} ${message}";
            }
            else
            {
                t = new NLog.Targets.FileTarget();
                ((NLog.Targets.FileTarget)t).FileName = System.IO.Path.GetTempPath() + "plutext.txt";
                //// Win 7:  C:\Users\jharrop\AppData\Local\Temp\
                //System.Diagnostics.Trace.WriteLine("TEMP: " + System.IO.Path.GetTempPath());
                ((NLog.Targets.FileTarget)t).AutoFlush = true;
            }
            //ILayout layout = new NLog.Layout("${longdate} ${callsite} ${level} ${message}");
            //NLog.LayoutCollection lc = new NLog.LayoutCollection();
            //lc.Add(layout);
            ////t.GetLayouts().Add(layout);
            //t.PopulateLayouts(lc);

            config.AddTarget("ds", t);
            config.LoggingRules.Add(new NLog.Config.LoggingRule("*", LogLevel.Trace, t));
            LogManager.Configuration = config;
            log = LogManager.GetLogger("OpenDoPE_Wed");
            log.Info("Logging operational.");
        }

        /// <summary>
        /// In Word 2010, when you open a saved document by double clicking on it, the DocumentOpen and WindowActivate events are not fired. 
        /// However, when you start up word first and open the saved document through the File menu, all events fire as expected.
        /// 
        /// The reason is that in Word 2010 the word startup behavior is changed, VSTO runtime waits for Word to be ready before firing the ThisAddIn_Startup event. 
        /// And In this scenario by that time the DocumentOpen and WindowActivate events are already fired.
        /// 
        /// See further http://social.msdn.microsoft.com/Forums/vstudio/en-US/3027424c-add3-4935-a822-b517147dbdef/documentopen-and-windowactivate-events-do-not-fire-on-word-2010?forum=vsto
        /// 
        /// </summary>
        bool initialized = false;

        private void InitializeCustom()
        {
            initialized = true;
            Globals.ThisAddIn.Application.DocumentOpen += new Word.ApplicationEvents4_DocumentOpenEventHandler(App_DocumentOpen);
        }

        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {

            app = this.Application;

            // Event handler for NewDocument
            ((Word.ApplicationEvents4_Event)app).NewDocument += new Word.ApplicationEvents4_NewDocumentEventHandler(ThisAddIn_NewDocument);
                

            // (Application level) Event hander for DocumentOpen
            /// In Word 2010 the word startup behavior is changed, VSTO runtime waits for Word to be ready before firing the ThisAddIn_Startup event. 
            /// And by that time, a doc opened via double click in Windows Explorer has already opened.  InitializeCustom() is a workaround for that,
            /// when invoked from ThisAddIn.Designer Initialize(), it happens early enough.
            if (!initialized)
            {
                log.Error("Call to InitializeCustom() seems to be missing from end of ThisAddIn.Designer Initialize(), so double click to launch Word 2010 won't work properly!");
                // Ensure things still work in 
                app.DocumentOpen +=
                    new Word.ApplicationEvents4_DocumentOpenEventHandler(App_DocumentOpen);

            }

            app.DocumentBeforeClose +=
                new Word.ApplicationEvents4_DocumentBeforeCloseEventHandler(app_DocumentBeforeClose);


            /*
             * Better for ribbon invalidate purposes to use WindowActive, since
             * in common situations, what fires is:
             * 
             *   WA (then sometimes DC)
             * 
             * or 
             * 
             *   DC then WA then DC again
             *   
             * 2013 12 10: TODO???
             * 
             */
            //((Word.ApplicationEvents4_Event)app).DocumentChange += new Word.ApplicationEvents4_DocumentChangeEventHandler(ThisAddIn_DocumentChange);
            //((Word.ApplicationEvents4_Event)app).WindowActivate += new Word.ApplicationEvents4_WindowActivateEventHandler(ThisAddIn_WindowActivate);

            ((Word.ApplicationEvents4_Event)app).WindowSelectionChange += new Word.ApplicationEvents4_WindowSelectionChangeEventHandler(ThisAddIn_WindowSelectionChange);

            //GlobalErrorHandler.Ensure();

            log.Info("Word version: " + Globals.ThisAddIn.Application.Version);
            // eg 14.0
            
        }

        void ThisAddIn_NewDocument(Word.Document Doc)
        {
            log.Debug("new document event handler fired..");
            Ribbon.myInvalidate();
        }

        

        /// <summary>
        /// In docx before close, note the associated ctp
        /// </summary>
        private Microsoft.Office.Tools.CustomTaskPane pendingCloseCTP = null;
        void app_DocumentBeforeClose(Word.Document Doc, ref bool Cancel)
        {

            // NET 4 way; see http://msdn.microsoft.com/en-us/library/microsoft.office.tools.word.extensions.aspx
            Document extendedDocument = Globals.Factory.GetVstoObject(Globals.ThisAddIn.Application.ActiveDocument);

            // NET 3.5 way, which requires using Microsoft.Office.Tools.Word.Extensions
            //Document extendedDocument = Application.ActiveDocument.GetVstoObject(Globals.Factory);
            try {
                log.Debug(" = " + extendedDocument.Name);
            } catch (Microsoft.VisualStudio.Tools.Applications.Runtime.ControlNotFoundException) {
                // Too late, its gone...
              /*
                Message=This document might not function as expected because the following control is missing: ThisDocument. 
               * Data that relies on this control will not be automatically displayed or updated, and other custom functionality 
               * will not be available. Contact your administrator or the author of this document for further assistance.
                  Source=Microsoft.Office.Tools.Word.Implementation
                  StackTrace:
                       at Microsoft.Office.Tools.Word.DocumentImpl.GetObjects()
                       at Microsoft.Office.Tools.Word.DocumentImpl.GetPrimaryControl()
                       at Microsoft.Office.Tools.Word.DocumentImpl.get_Name()
                       at OpenDope_AnswerFormat.ThisAddIn.app_DocumentBeforeClose(Document Doc, Boolean& Cancel) in C:\Users\jharrop\Documents\Visual Studio 2010\Projects\OpenDope_Author_NoXML\trunk\OpenDope_AnswerFormat\OpenDope_AnswerFormat\ThisAddIn.cs:line 158
                  InnerException: System.NullReferenceException
                       HResult=-2147467261
                       Message=Object reference not set to an instance of an object.
                       Source=Microsoft.VisualStudio.Tools.Office.Runtime
                       StackTrace:
                            at Microsoft.VisualStudio.Tools.Office.Runtime.DomainCreator.ExecuteCustomization.Microsoft.Office.Tools.IHostItemProvider.GetHostObject(Type primaryType, String primaryCookie)
                            at Microsoft.Office.Tools.Word.DocumentImpl.GetObjects()
               */
                
            }


            if (extendedDocument.Tag != null
                && extendedDocument.Tag is FabDocxState) {
                
                pendingCloseCTP = ((FabDocxState)extendedDocument.Tag).TaskPane;
                log.Debug(".. taskpane identified for closing");
            }
            else
            {
                log.Debug(".. no associated taskpane to close");
            }

            Ribbon.myInvalidate();
        }

        /*
         * Bug: if you cancel the close dialog on a docx which has a ctp,
         * and then close some other docx (which also has a ctp, since event
         * only fires in that case); the ctp belonging to the first 
         * docx will be closed!
         * 
         * May be able to prevent this by storing a reference to the docx
         * in the ctp, then doing something like doc.Name.  
         * If this throws exception "Object has been deleted", then
         * we know we're good to close the CTP.  If it doesn't, 
         * then it is probably the wrong document.
         * 
         * Implemented this.  But in hindsight, it shouldn't be
         * a problem, because extendedDocument_Shutdown should
         * only fire for docx which has a wedtaskpane.  
         * 
         * But maybe there are exotic cases, so keep the code...
         */

        /// <summary>
        /// Close the Custom Task Pane when the docx closes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void extendedDocument_Shutdown(object sender, EventArgs e)
        {
            System.Diagnostics.Trace.WriteLine("in document shutdown");

            // sender is Microsoft.Office.Tools.Word.DocumentImpl

            //Document documentClosing = (Document)sender;
            //log.Debug("Document closing: " + documentClosing.Name); // Object has been deleted            

            if (pendingCloseCTP != null)
            {
                try
                {
                    removeTaskPane(pendingCloseCTP);
                    pendingCloseCTP = null;
                    //WedTaskPane wedTaskPane = (WedTaskPane)pendingCloseCTP.Control;
                    //string foo = wedTaskPane.associatedDocument.Name;
                    //log.Debug(".. cowardly refusing to close taskpane!");
                }
                catch (Exception ex)
                {
                    log.Error(ex.Message);
                    //removeTaskPane(pendingCloseCTP);
                    //pendingCloseCTP = null;
                    //log.Debug(".. taskpane also closed");
                }
            }
            else
            {
                log.Debug(".. no associated taskpane to close");
            }



        }

        Ribbon myRibbon = null;

        protected override Microsoft.Office.Core.IRibbonExtensibility CreateRibbonExtensibilityObject()
        {
            // For Ribbon XML
            // See http://msdn.microsoft.com/en-us/library/aa942866(v=vs.100).aspx

            log.Info("Ribbon created.");
            myRibbon = new Ribbon();
            return myRibbon;
        }

        /*
                void App_NewDocument(Word.Document document)
                {
                    //System.Windows.Forms.MessageBox.Show("new doc");
                    log.Debug("New doc event fired.");
                    // new Microsoft.Office.Interop.Word.Document() doesn't make this fire!!!


                    //document.Activate();

                }
         */

        /// <summary>
        /// Create task pane if this docx contains
        /// XPaths & Conditions parts.
        /// </summary>
        /// <param name="document"></param>
        void App_DocumentOpen(Word.Document document)
        {
            // AutoExec macro (if present) and this event will both run,
            // but where there is an auto exec macro, 
            // only if the user runs the AutoExec macro.
            // The AutoExec macro runs first, then this.

            log.Debug("App_DocumentOpen event fired.");
            //MessageBox.Show("App_DocumentOpen event fired.");


            if (OpenDoPEDetection.configIfPresent(document))
            {

                log.Debug("OpenDoPE detected");
                //MessageBox.Show("OpenDoPE detected");


                //Ribbon theRibbon = (Ribbon)Ribbon.getRibbon(); // doesn't work .. can't cast COM object
                myRibbon.enable(false);

                //app.WindowBeforeRightClick += new Word.ApplicationEvents4_WindowBeforeRightClickEventHandler(app_WindowBeforeRightClick);

            }
            else
            {
                //MessageBox.Show("OpenDoPE NOT detected!!");
            }

            document.Activate();

            // Set state to match this docx
            Ribbon.myInvalidate();


            log.Debug("Open event handler finished");

        }


        void taskPane_VisibleChanged(object sender, EventArgs e)
        {
            log.Debug("ctp visible event fired");
            Ribbon.myInvalidate();
        }

/*
        public WedTaskPane createCTP(Word.Document document, List<Office.CustomXMLPart> cxp,
            Office.CustomXMLPart xpathsPart,
            Office.CustomXMLPart conditionsPart,
            Office.CustomXMLPart questionsPart,
            Office.CustomXMLPart componentsPart
            )
        {
            string ptcName = "OpenDoPE " + Globals.ThisAddIn.Application.ActiveDocument.Name;

            WedTaskPane plutextTabbedControl = new WedTaskPane(cxp, xpathsPart, conditionsPart, questionsPart, componentsPart);
            //AvalonForm plutextTabbedControl = new AvalonForm();

            return plutextTabbedControl;
        }


        void app_DocumentChange()
        {
            // RemoveOrphanedTaskPanes();
        }
    */

        void ThisAddIn_WindowSelectionChange(Word.Selection selection)
        {
            // Is there a custom task pane associated with this document?
            Word.Window wn = this.Application.ActiveWindow;
            log.Debug("In WSC, with active window: " + wn.Caption);

            //Microsoft.Office.Tools.CustomTaskPane ctp = getTaskPane(true);

            // NET 4 way; see http://msdn.microsoft.com/en-us/library/microsoft.office.tools.word.extensions.aspx
            Document extendedDocument = Globals.Factory.GetVstoObject(Globals.ThisAddIn.Application.ActiveDocument);

            // NET 3.5 way, which requires using Microsoft.Office.Tools.Word.Extensions
            //Document extendedDocument = Application.ActiveDocument.GetVstoObject(Globals.Factory);

            // if (extendedDocument.Tag != null

        }

        /*
                public Microsoft.Office.Tools.CustomTaskPane getTaskPane(bool deleteOthers)
                {
                    Word.Window wn = this.Application.ActiveWindow;
                    string ptcName = "OpenDoPE " + Globals.ThisAddIn.Application.ActiveDocument.Name;

                    log.Debug("active window: " + wn.Caption);

                    Microsoft.Office.Tools.CustomTaskPane result = null;
                    WedTaskPane wedTaskPane = null;
                    List<Microsoft.Office.Tools.CustomTaskPane> ctpDeletes = new List<Microsoft.Office.Tools.CustomTaskPane>();

                    foreach (Microsoft.Office.Tools.CustomTaskPane ctp in CustomTaskPanes)
                    {

                        if (ctp.Window == null)
                        {
                            log.Debug("encountered obsolete ctp. removing...");
                            ctpDeletes.Add(ctp);
                            continue;
                        }

                        log.Debug("ctp window: " + ((Word.Window)ctp.Window).Caption);


                        if (((Word.Window)ctp.Window) == wn
                            && ctp.Control is WedTaskPane)
                        {
                            log.Debug("found ctp");

                            result = ctp;
                            wedTaskPane = (WedTaskPane)ctp.Control;

                            // Verify, since if a doc with a ctp attached is closed,
                            // the ctp is moved to another doc!


                            //if (wedTaskPane.windowCaption.Equals("OpenDoPE " + wn.Caption))
                            // Don't use that test, since the caption could be 
                            // "docname without extension (Read-Only)"

                            if (wedTaskPane.windowCaption.Equals(ptcName))
                            {
                                // All good
                            } else {

                                log.Debug("but its obsolete. removing...");
                                ctpDeletes.Add(ctp);
                                continue;
                            }

                            // ok, we know its the right one
                            //muc.handleSelection(selection);

                            break;
                        }
                        else
                        {
                            log.Debug("..not ctp window");
                        }
                    }

                    // Clean up orphans
                    if (deleteOthers)
                    {
                        foreach (Microsoft.Office.Tools.CustomTaskPane ctp in ctpDeletes)
                        {
                            removeTaskPane(ctp);
                        }
                    }

                    return result;

                }
         */

        public void removeTaskPane(Microsoft.Office.Tools.CustomTaskPane ctp)
        {
            if (ctp == null) return;
            CustomTaskPanes.Remove(ctp);
        }

        //public bool isaPlutextDocOpen()
        //{

        //    foreach (Microsoft.Office.Tools.CustomTaskPane _ctp in
        //        Globals.ThisAddIn.CustomTaskPanes)
        //    {
        //        if (_ctp.Title.Contains("Plutext"))
        //        {
        //            return true;
        //        }
        //    }
        //    return false;
        //}


        //private void ThisAddIn_DocumentBeforeClose(Word.Document document, ref bool cancel)
        //{
        //    /*
        //     * Unfortunately this fires before the save dialog, so if the
        //     * user hits cancel, the timer etc is still shutdown.
        //     * 
        //     * See Carter & Lippert p258.
        //     * 
        //     * Have posted to MSDN VSTO forum
        //     * http://forums.microsoft.com/MSDN/ShowPost.aspx?PostID=3465964&SiteID=1
        //     */

        //    // Avoid save prompt
        //    // this.InnerObject.Saved = true;

        //    WedTaskPane myControl = null;
        //    //Boolean foundNewCtp = false;
        //    foreach (Microsoft.Office.Tools.CustomTaskPane _ctp in
        //        Globals.ThisAddIn.CustomTaskPanes)
        //    {

        //        // FIXME: somehow there seem to be 2 of these?

        //        Word.Window ctpWindow = (Word.Window)_ctp.Window;
        //        if (_ctp.Title.Contains("OpenDoPE") &&
        //            ctpWindow == Globals.ThisAddIn.Application.ActiveWindow)
        //        {
        //            myControl = (WedTaskPane)_ctp.Control;

        //            // ok, now save the document
        //            document.Save();


        //            this.CustomTaskPanes.Remove(_ctp);
        //        }
        //    }

        //}

        //private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        //{
        //    System.Diagnostics.Trace.WriteLine("shutdown event");
        //    log.Info("Document closed");        
        //}

        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
        }

        #region VSTO generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(ThisAddIn_Startup);
            this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
        }

        #endregion
    }
}
