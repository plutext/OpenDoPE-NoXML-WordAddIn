using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using NLog;

using Word = Microsoft.Office.Interop.Word;
using Office = Microsoft.Office.Core;

using OpenDoPEModel;


namespace OpenDope_AnswerFormat.Controls
{
    public partial class UserControlComponentIRI : UserControl
    {

        static Logger log = LogManager.GetLogger("UserControlComponentIRI");

        //WedTaskPane taskPane;

        components odComponents;
        componentsComponent existingComponent;

        Word.ContentControl currentCC;
        Model model;

        public UserControlComponentIRI()
            //Word.ContentControl currentCC, XmlEditorControl xec)
        {
            //this.taskPane = taskPane;
            InitializeComponent();
        }

        public void initFields(Model model, Word.ContentControl theCC)
        {
            this.model = model;
            currentCC = theCC;

            odComponents = new components();
            OpenDoPEModel.components.Deserialize(model.componentsPart.XML, out odComponents);

            TagData td = new TagData(currentCC.Tag);
            if (td.get("od:component") != null)
            {
                string id = td.get("od:component");

                existingComponent = getComponentById(id);
                if (existingComponent!=null) {
                    this.textBox1.Text = existingComponent.iri;
                }
            }
        }

        private string generateId()
        {
            int i = 0;
            componentsComponent fetchedComponent;
            do
            {
                i++;
                fetchedComponent = getComponentById("comp" + i);

            } while (fetchedComponent != null);

            return "comp" + i;

        }

        private componentsComponent getComponentById(string id)
        {
            foreach(componentsComponent comp in odComponents.component) {
                if (comp.id.Equals(id)) {
                    return comp;
                }
            }
            return null;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {

            if (existingComponent == null)
            {
                existingComponent = new componentsComponent();
                existingComponent.id = generateId();
                odComponents.component.Add(existingComponent);
            }
            existingComponent.iri = this.textBox1.Text;

            // Write tag
            TagData td = new TagData(currentCC.Tag);
            td.remove("od:repeat");
            td.remove("od:condition");
            td.remove("od:component");
            td.set("od:component", existingComponent.id);
            currentCC.Tag = td.asQueryString();

            // Save it in docx
            string result = odComponents.Serialize();
            log.Info(result);
            CustomXmlUtilities.replaceXmlDoc(model.componentsPart, result);

            log.Debug("Component written!");
            //Mbox.ShowSimpleMsgBoxInfo("Component written!");

        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {


            if (currentCC == null)
            {
                log.Error("currentCC not set?!");
                return;
            }

            // Write tag
            TagData td = new TagData(currentCC.Tag);

            try
            {
                if (td.get("od:component") == null)
                {
                    Mbox.ShowSimpleMsgBoxInfo("This content control isn't a component.");
                    return;
                }
            }
            catch (Exception ex)
            {
            }

            td.remove("od:component");
            currentCC.Tag = td.asQueryString();

            Mbox.ShowSimpleMsgBoxInfo("component removed from this content control");



        }

        public void publish()
        {

            if (existingComponent == null)
            {
                existingComponent = new componentsComponent();
                existingComponent.id = generateId();
                odComponents.component.Add(existingComponent);
            }
            existingComponent.iri = this.textBox1.Text;

            // Write tag
            TagData td = new TagData(currentCC.Tag);
            td.remove("od:repeat");
            td.remove("od:condition");
            td.remove("od:component");
            td.set("od:component", existingComponent.id);
            currentCC.Tag = td.asQueryString();

            // Save it in docx
            string result = odComponents.Serialize();
            log.Info(result);
            CustomXmlUtilities.replaceXmlDoc(model.componentsPart, result);

            log.Debug("Component written!");
            //Mbox.ShowSimpleMsgBoxInfo("Component written!");

        }

        public void cancel()
        {


            if (currentCC == null)
            {
                log.Error("currentCC not set?!");
                return;
            }

            // Write tag
            TagData td = new TagData(currentCC.Tag);

            try
            {
                if (td.get("od:component") == null)
                {
                    Mbox.ShowSimpleMsgBoxInfo("This content control isn't a component.");
                    return;
                }
            }
            catch (Exception ex)
            {
            }

            td.remove("od:component");
            currentCC.Tag = td.asQueryString();

            Mbox.ShowSimpleMsgBoxInfo("component removed from this content control");



        }
    }
}
