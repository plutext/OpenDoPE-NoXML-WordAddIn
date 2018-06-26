using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OpenDope_AnswerFormat.Forms
{
    public partial class FormComponent : Form
    {
        public FormComponent()
        {
            InitializeComponent();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            this.userControlComponentIRI1.publish();
        }

        private void buttonRemove_Click(object sender, EventArgs e)
        {
            this.userControlComponentIRI1.cancel();
        }
    }
}
