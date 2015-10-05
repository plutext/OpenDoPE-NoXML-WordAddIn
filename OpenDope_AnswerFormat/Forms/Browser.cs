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

namespace OpenDope_AnswerFormat {

    public partial class Browser : Form
    {
        public Browser()
        {
            InitializeComponent();

            //this.webBrowser1.Navigate(
            this.webBrowser1.DocumentText = "<html><body>" 
                    + "<form id=\"fileForm\" action=\"http://www.fabdocx.com/library/upload\" method=\"POST\" enctype=\"multipart/form-data\">	"
                    + "	Choose file to upload: <input type=\"file\" id=\"docxfile\"  name=\"docxfile\"  "
                     + "onchange=\"var thisform = document.getElementById('fileForm'); var filename = thisform.docxfile.value;  "
	                    + "	im = new Array();  "
                    + "		im['docx'] = 'Text'; "
                    + "		im['docm'] = 'Text';  "
                    + "		im['dotx'] = 'Text'; "
                    + "		im['dotm'] = 'Text'; "
                    + "		im['xml'] = 'Text'; "
                    + "	    var dot = filename.lastIndexOf('.');   "
                    + "	    if (dot != -1) {  "
                    + "	       var ext = filename.substr(dot + 1).toLowerCase();  "
                    + "			var family = im[ext];  "
                    + "			if (family == undefined) {  "
                    + "		    	alert('Sorry, but importing of '+ ext + ' is not supported');  "
                    + "				thisform.reset(); "
                    + "			} else { "
                    + "				thisform.submit();  "
                    + "			}  "
                    + "		} else {  "
                    + "		    	alert('What sort of document is this?');  "
                    + "				thisform.reset();  "
                    + "		} \"  "								
                    + "	 /> "
                    + "</form>  "
                    + "<p>Note: this form will be replaced by an automated upload, so you don't have to manually navigate to the document.</p>"
                    + "</body></html>";

        }
    }
}
