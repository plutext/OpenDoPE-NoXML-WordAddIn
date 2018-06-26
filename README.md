# OpenDoPE-NoXML-WordAddIn
A Word AddIn for custom xml databinding, where the XML format is fixed in advance.

Because the XML format is fixed, there is no need for the author to be exposed to it.  
This makes this tool suitable for non-technical authors (as compared to the other authoring
tool OpenDoPE-Mapping-WordAddIn).

If you want to try it out, there's an installer at https://github.com/plutext/OpenDoPE-NoXML-WordAddIn/blob/master/BinaryReleases/OpenDoPE-NoXML-WordAddIn-1_01?raw=true

The fixed XML format is very simple:

- The root element is <answers>. 
- It contains answer and repeat children.
- An answer has @id
- A repeat contains row elements, which in turn contains answer and repeat children

This AddIn is currently targeted at the interactive use case.  That is, it assumes
a user will answer questions (eg in a web browser) in order to provide the data
to be used to generate the document at run time, and so contains forms to gather the
necessary info.  

The non-interactive use case is actually simpler.  You don't need questions, and
you might not need defined data types. To address that use case,
part of the user interface could be hidden.  A boolean flag in app.config could
cater for this.  But still, it works fine as is.

Generating the web form(s) based on the docx authored via this AddIn is outside the scope
of this project. Generating an instance document based on the authored docx plus runtime 
data, is handled out of the box by docx4j.

Developed in Visual Studio 2015; you should be able to use that or a later version.

Targets Word 2007 or later.

This solution uses project https://github.com/plutext/OpenDoPE-Model
so you should get that, then adjust your project references to use it.

# License

This project is licensed to you under the GPL v3.
