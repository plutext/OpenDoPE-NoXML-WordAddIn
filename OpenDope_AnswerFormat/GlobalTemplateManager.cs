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
using System.IO;
using System.Windows.Forms;

using NLog;
using Word = Microsoft.Office.Interop.Word;
using Microsoft.Office.Tools.Word.Extensions; // for VSTOObject

using OpenDope_AnswerFormat.Forms;
using OpenDoPEModel;

namespace OpenDope_AnswerFormat
{
    /// <summary>
    /// GlobalTemplateManager handles access to FabDocx.dotx.
    /// 
    /// FabDocx.dotx is used for storing logic which can be used across 
    /// documents (by copying, not by reference).
    /// 
    /// Typically it is one or more content controls stored as a building block,
    /// plus associated xpath, conditions etc.
    /// 
    /// (FabDocx.dotx can also be used for storing user config data.)
    /// 
    /// To add a building block, I have to access FabDocx.dotx
    /// as a Word.Template (which means it needs to be loaded as 
    /// an addin - see http://msdn.microsoft.com/en-us/library/microsoft.office.interop.word.template )
    /// 
    /// To access the custom xml parts, I have to access FabDocx.dotx
    /// as a Word.Document.
    /// 
    /// It seems it you can open it as a document, and then 
    /// load as an addin, without needing to close the document.
    /// 
    /// </summary>
    class GlobalTemplateManager
    {
        static Logger log = LogManager.GetLogger("GlobalTemplateManager");

        static GlobalTemplateManager globalTemplate;

        Word.Document FabDotxAsDocument;
        Word.Template FabDotxAsTemplate;

        /// <summary>
        /// C:\Users\jharrop\Documents\Visual Studio 2010\Projects\OpenDope_Author_NoXML\trunk\OpenDope_AnswerFormat\OpenDope_AnswerFormat\bin\Debug\FabDocx.dotx
        /// </summary>
        string templatePath;

        public static string FABDOCX_ADDIN = "FabDocx.dotx";

        object missingType = System.Reflection.Missing.Value;

        public static GlobalTemplateManager GetGlobalTemplateManager()
        {
            if (globalTemplate==null) {
                globalTemplate = new GlobalTemplateManager();
            }
            return globalTemplate;
        }

        private GlobalTemplateManager()
        {            
            init();
        }

        /// <summary>
        /// Get our dotx, creating if necessary.
        /// </summary>
        private void init()
        {
            templatePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                  @FABDOCX_ADDIN);

            // Does the file exist?
            if (System.IO.File.Exists(templatePath))
            {
                log.Info("Opening " + templatePath);
                //openTemplateAsDocument(); // assume our custom xml parts are present

                addin();
            }
            else
            {
                log.Info("Creating " + templatePath);
                createTemplate();

                log.Info("Adding custom xml.. ");
                addFabDocxParts();

                log.Info("Saving " + templatePath);
                saveTemplate(FabDotxAsDocument);

                closeDocument(FabDotxAsDocument);

                addin();
            }
        }

        Word.AddIn addinObj;

        private void addin()
        {
            addinObj = Globals.ThisAddIn.Application.AddIns.Add(templatePath);
            log.Debug("addin object: " + addinObj.Name);

            findTemplate();
        }

        private void findTemplate() {

            foreach (Word.Template t in Globals.ThisAddIn.Application.Templates)
            {
                //C:\Users\jharrop\Documents\Visual Studio 2010\Projects\OpenDope_Author_NoXML\trunk\OpenDope_AnswerFormat\OpenDope_AnswerFormat\bin\Debug\FabDocx.dotx
                //C:\Users\jharrop\AppData\Roaming\Microsoft\Templates\Normal.dotm

                if (t.FullName.Contains(templatePath))
                {
                    FabDotxAsTemplate = t;
                    log.Debug("Found template " + t.FullName);
                    break;
                }
            }
            
        }

        private Word.Document openDocument(string path)
        {
            log.Debug("opening" + path);

            object FileName = path;
            object ConfirmConversions = System.Reflection.Missing.Value;
            object ReadOnly = System.Reflection.Missing.Value;
            object AddToRecentFiles = false;
            object PasswordDocument = System.Reflection.Missing.Value;
            object PasswordTemplate = System.Reflection.Missing.Value;
            object Revert = false; // activate document, if open
            object WritePasswordDocument = System.Reflection.Missing.Value;
            object WritePasswordTemplate = System.Reflection.Missing.Value;
            object Format = System.Reflection.Missing.Value;
            object Encoding = System.Reflection.Missing.Value;
            object Visible = false;
            object OpenAndRepair = System.Reflection.Missing.Value;
            object DocumentDirection = System.Reflection.Missing.Value;
            object NoEncodingDialog = System.Reflection.Missing.Value;
            object XMLTransform = System.Reflection.Missing.Value;



            return Globals.ThisAddIn.Application.Documents.Open(ref  FileName,
                                                            ref  ConfirmConversions,
                                                            ref  ReadOnly,
                                                            ref  AddToRecentFiles,
                                                            ref  PasswordDocument,
                                                            ref  PasswordTemplate,
                                                            ref  Revert,
                                                            ref  WritePasswordDocument,
                                                            ref  WritePasswordTemplate,
                                                            ref  Format,
                                                            ref  Encoding,
                                                            ref  Visible,
                                                            ref  OpenAndRepair,
                                                            ref  DocumentDirection,
                                                            ref  NoEncodingDialog,
                                                            ref  XMLTransform
                                                        );
        }

        private void createTemplate()
        {
            object isTemplate = true;
            object isVisible = false;
            FabDotxAsDocument = Globals.ThisAddIn.Application.Documents.Add(ref missingType, ref isTemplate, ref missingType, ref isVisible);
        }

        private void addFabDocxParts()
        {
            PartCreator pc = new PartCreator();
            pc.process(FabDotxAsDocument, true);
            pc.createAnswersPart(FabDotxAsDocument);
        }

        private void saveTemplate(Word.Document doc)
        {
	        object FileName = templatePath;
	        object FileFormat = Word.WdSaveFormat.wdFormatXMLTemplate;
	        object LockComments = false;
	        object Password = System.Reflection.Missing.Value;
	        object AddToRecentFiles = false;
	        object WritePassword = System.Reflection.Missing.Value;
	        object ReadOnlyRecommended = false;
	        object EmbedTrueTypeFonts = false;
	        object SaveNativePictureFormat = false;
	        object SaveFormsData = false;
	        object SaveAsAOCELetter = false;
	        object Encoding = System.Reflection.Missing.Value;
	        object InsertLineBreaks = System.Reflection.Missing.Value;
	        object AllowSubstitutions = System.Reflection.Missing.Value;
	        object LineEnding = System.Reflection.Missing.Value;
	        object AddBiDiMarks = System.Reflection.Missing.Value;
	        //object CompatibilityMode = Word.WdCompatibilityMode.wdWord2007;

            doc.SaveAs( ref  FileName,
	                                    ref  FileFormat,
	                                    ref  LockComments,
	                                    ref  Password,
	                                    ref  AddToRecentFiles,
	                                    ref  WritePassword,
	                                    ref  ReadOnlyRecommended,
	                                    ref  EmbedTrueTypeFonts,
	                                    ref  SaveNativePictureFormat,
	                                    ref  SaveFormsData,
	                                    ref  SaveAsAOCELetter,
	                                    ref  Encoding,
	                                    ref  InsertLineBreaks,
	                                    ref  AllowSubstitutions,
	                                    ref  LineEnding,
	                                    ref  AddBiDiMarks);
	                                    //ref  CompatibilityMode);

        }

        private void closeDocument(Word.Document doc)
        {
            object SaveChanges = Word.WdSaveOptions.wdSaveChanges;
            doc.Close(ref SaveChanges);
        }

        public void createBuildingBlock(Word.Range range)
        {
            // Identify logic
            // If repeated content is being reused, it must be reused 
            // from the shallowest on down.
            FabDocxState fabDocxState = (FabDocxState)Globals.ThisAddIn.Application.ActiveDocument.GetVstoObject(Globals.Factory).Tag;
            Helpers.LibraryHelper libHelp = new Helpers.LibraryHelper(fabDocxState.model);
            try
            {
                libHelp.identifyLogic(range);
            }
            catch (Helpers.BuildingBlockLogicException bble)
            {
                MessageBox.Show("Since your selection includes repeating content, to make a building block you need to include the outermost repeat.");
                return;
            }

            // For each bit of logic in the selection,
            // copy it to FabDocx.dotx
            // (if the logic is there already, from this docx,
            //  overwrite it, since its been updated)
            Word.Document fabdocx = null;
            try
            {
                fabdocx = FabDotxAsTemplate.OpenAsDocument();
                // TODO hide it
                Model targetModel = Model.ModelFactory(fabdocx);
                libHelp.injectLogic(targetModel, true, false, true);
            }
            catch (Helpers.BuildingBlockLogicException bble)
            {
                // ID collisions
                // (these are OK only if same source document)
                // TODO - alter IDs...
                MessageBox.Show(bble.Message);
                return;
            }
            finally
            {
                saveTemplate(fabdocx);
                closeDocument(fabdocx);
            }

            libHelp.TagsSourceAdd(range);            

            // Save selection as building block in FabDocx.dotx
            FormBuildingBlockNew bbn = new FormBuildingBlockNew();
            bbn.ShowDialog();

            object description = bbn.textBoxDescription.Text;
            //object insertOptions = Word.WdDocPartInsertOptions.wdInsertParagraph;
            FabDotxAsTemplate.BuildingBlockEntries.Add(bbn.textBoxName.Text,
                Word.WdBuildingBlockTypes.wdTypeCustom1,
                bbn.textBoxCategory.Text, range, ref description); //, Word.WdDocPartInsertOptions.wdInsertParagraph);
                // NB, how to adjust any bindings so they point to FabDocx's custom xml parts?
                // as we can't easily change anything in the glossary document.
                // I guess it doesn't really matter; we'll change them when we load.
                // If it did, could always do it via OpenXML.

            // OK, now the building block is done, remove od:source from the cc's
            // in our document
            libHelp.TagsSourceRemove(range);            

            bbn.Dispose();

            // Word prompts to save changes to the dotx when you exit Word, 
            // not when you close the docx.
            FabDotxAsTemplate.Save();

            // Hack to force our gallery to refresh
            addinObj.Installed = false;
            addinObj.Installed = true;

            findTemplate(); //avoid object has been deleted


        }

        public void copyBuildingBlockLogic(Word.Document target, Word.Range range)
        {
            // FabDotxAsTemplate.OpenAsDocument() throws:
            //     This object model command is not available while in the current event.
            // Documents.Open(the dotx) is not allowed either
            // so make a temporary copy.

            //string tmp = System.IO.Path.GetTempFileName();
            //System.IO.File.Copy(templatePath, tmp, true);
            // .. but that fails the second time
            Word.Document fabdocx;
            try
            {
                //fabdocx = openDocument(tmp);
                fabdocx = FabDotxAsTemplate.OpenAsDocument();
            }
            catch (Exception e)
            {
                log.Error(e);
                throw e;
            }

            Model srcModel = Model.ModelFactory(fabdocx);

            Helpers.LibraryHelper libHelp = new Helpers.LibraryHelper(srcModel);
            try
            {
                libHelp.identifyLogic(range);
            }
            catch (Helpers.BuildingBlockLogicException bble)
            {
                log.Error(bble);
                MessageBox.Show("You can't reuse repeating content in a repeat"); // TODO, relax this restriction
                range.Delete();
                return;
            }

            Model targetModel = Model.ModelFactory(target);

            try
            {
                libHelp.injectLogic(targetModel, false, true, false);
                libHelp.updateBindings(range, targetModel.answersPart);
                // OK, remove od:source from the cc's
                // in our document
                libHelp.TagsSourceRemove(range);
            }
            catch (Helpers.BuildingBlockLogicException bble)
            {
                log.Error(bble);
                MessageBox.Show("ID collision"); // TODO, relax this restriction
                range.Delete();
                return;
            }
            finally
            {
                // OK to close the template now
                closeDocument(fabdocx);
                //System.IO.File.Delete(tmp);
                //log.Debug("deleted " + tmp);
            }

        }


    }
}
