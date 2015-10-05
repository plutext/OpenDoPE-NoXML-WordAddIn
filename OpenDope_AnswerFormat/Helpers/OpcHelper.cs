using System;
using System.IO;
using System.IO.Packaging; // Need to add reference to WindowsBase!
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Office.Interop.Word;
using NLog;

namespace OpenDope_AnswerFormat.Helpers
{

    /*
     * This class FAILS for large files!

        Google: IsolatedStorageException MS.Internal.IO.Packaging
        One of the results http://social.msdn.microsoft.com/Forums/en/oslo/thread/4d0c258e-9cd4-405b-a598-cc0c92b3607e says: 

        wrapped all of my code inside of a new class that gets executed in a new appdomain

        See generally http://www.kevinrohrbaugh.com/blog/2008/4/4/server-side-office-document-generation-bug.html

        Per http://rekiwi.blogspot.com/2008/12/unable-to-determine-identity-of-domain.html

         if the package part is too big (more than 1.3Mb compressed), the framework decides to unzip the entire package part to Isolated Storage

        Under COM, we are running in a DefaultDomain that doesn't have any evidence.

        You can't set the evidence for an AppDomain once it has been started and it's not possible to specify that the COM DLL should run with certain evidence or in a special AppDomain. Running the "Microsoft .NET Framework 2.0 Configuration" tool and granting Full Trust to our assembly doesn't solve the problem because there is still no "evidence" for the Framework code to examine. So there are two options:

        1) In the Win32 code, host the CLR, create an AppDomain with the appropriate evidence and load the assembly. Use reflection to get at the methods.

        2) (What I did in the interests of expediency). In the COM component, create a new AppDomain with the appropriate evidence. and execute the code in that. This works fine. There is a performance hit because we are now marshaling across AppDomains as well as marshaling across COM. We will see if the performance is acceptable. If not we will have to go with (1).

        Doing (2) is similar to what you have to do for Office add-ins. For Office add-ins, the recommended strategy to satisfy the security model is to have an unmanaged shim. You sign the shim to make Office happy. Office talks to the shim, the shim acts as a proxy passing everything to your managed code.

        In our case, the COM interface now loads up the AppDomain and proxies calls to an instance of our class running in that AppDomain.

        The crucial (necessary and sufficient) piece of evidence is that we require the code to be running in the MyComputer zone.

        First we need a simple AppDomainSetup:

        AppDomainSetup setup = new AppDomainSetup();
        setup.ApplicationBase = AppDomain.CurrentDomain.BaseDirectory.ToString();


        Then we need our evidence

        Evidence evidence = new Evidence();
        evidence.AddHost(new Zone(SecurityZone.MyComputer));


        Now we can fire up an AppDomain running with that evidence.

        AppDomain hostedAppDomain = AppDomain.CreateDomain("Demo", evidence, setup);


        Now we get a handle on an instance of our class running in that AppDomain

        ObjectHandle handle = hostedAppDomain.CreateInstance("MyStuff.Demo, Version=1.0.0.0, Culture=neutral, PublicKeyToken=d0e8b069449d61a1", "MyStuff.Demo.DemoComponent");


        To pull this off, DemoComponent has to inherit from MarshalByRefObject so now we have a little .Net remoting magic to do. We have to get a lease on the object and extend its lease if we are not done with it. A trivial class LicenseRenewer that implements ISponsor does the trick

        lease = (ILease)handle.GetLifetimeService();
        lease.Register(leaseRenewer);


        Finally we can get a usable instance of the class. We access this locally and it transparently proxies calls to the other AppDomain.

        demoComponent = (IDemoComponent)handle.Unwrap();


        Now calls to our COM interface can just explicitly proxy to demoComponent e.g.

        public bool Demo()
        {
        return demoComponent.Demo();
        }


        Then our COM interface just proxies things to demoComponent. Any types you want to marshall across AppDomains have to marked [Serializable()] of course.


        A first chance exception of type 'System.IO.IsolatedStorage.IsolatedStorageException' occurred in mscorlib.dll
        A first chance exception of type 'System.IO.IsolatedStorage.IsolatedStorageException' occurred in WindowsBase.dll
        com.plutext.search.main.ResultsOverview.bookmark_BeforeDoubleClick Unable to determine the identity of domain.
        com.plutext.search.main.ResultsOverview.bookmark_BeforeDoubleClick Invoking handler
        com.plutext.search.main.GlobalErrorHandler.HandleException 
        System.IO.IsolatedStorage.IsolatedStorageException: Unable to determine the identity of domain.
           at System.IO.IsolatedStorage.IsolatedStorage._GetAccountingInfo(Evidence evidence, Type evidenceType, IsolatedStorageScope fAssmDomApp, Object& oNormalized)
           at System.IO.IsolatedStorage.IsolatedStorage.GetAccountingInfo(Evidence evidence, Type evidenceType, IsolatedStorageScope fAssmDomApp, String& typeName, String& instanceName)
           at System.IO.IsolatedStorage.IsolatedStorage._InitStore(IsolatedStorageScope scope, Evidence domainEv, Type domainEvidenceType, Evidence assemEv, Type assemblyEvidenceType, Evidence appEv, Type appEvidenceType)
           at System.IO.IsolatedStorage.IsolatedStorage.InitStore(IsolatedStorageScope scope, Type domainEvidenceType, Type assemblyEvidenceType)
           at System.IO.IsolatedStorage.IsolatedStorageFile.GetUserStoreForDomain()

           at MS.Internal.IO.Packaging.PackagingUtilities.ReliableIsolatedStorageFileFolder.GetCurrentStore()
           at MS.Internal.IO.Packaging.PackagingUtilities.ReliableIsolatedStorageFileFolder..ctor()
           at MS.Internal.IO.Packaging.PackagingUtilities.GetDefaultIsolatedStorageFile()
           at MS.Internal.IO.Packaging.PackagingUtilities.CreateUserScopedIsolatedStorageFileStreamWithRandomName(Int32 retryCount, String& fileName)
           at MS.Internal.IO.Packaging.SparseMemoryStream.EnsureIsolatedStoreStream()
           at MS.Internal.IO.Packaging.SparseMemoryStream.SwitchModeIfNecessary()
           at MS.Internal.IO.Packaging.SparseMemoryStream.Write(Byte[] buffer, Int32 offset, Int32 count)
           at MS.Internal.IO.Packaging.DeflateEmulationTransform.Decompress(Stream source, Stream sink)
           at MS.Internal.IO.Packaging.CompressEmulationStream..ctor(Stream baseStream, Stream tempStream, Int64 position, IDeflateTransform transformer)
           at MS.Internal.IO.Packaging.CompressStream.ChangeMode(Mode newMode)
           at MS.Internal.IO.Packaging.CompressStream.Seek(Int64 offset, SeekOrigin origin)
           at MS.Internal.IO.Zip.ProgressiveCrcCalculatingStream.Seek(Int64 offset, SeekOrigin origin)
           at MS.Internal.IO.Zip.ZipIOModeEnforcingStream.Read(Byte[] buffer, Int32 offset, Int32 count)
           at System.IO.BinaryReader.ReadBytes(Int32 count)

           at com.plutext.search.main.OpcHelper.GetContentsAsXml(PackagePart part) in C:\Users\jharrop\Documents\Visual Studio 2010\Projects\com.plutext.search-TRUNK\com.plutext.search.main\OpcHelper.cs:line 46
           at com.plutext.search.main.OpcHelper.<OpcToFlatOpc>b__c(PackagePart part) in C:\Users\jharrop\Documents\Visual Studio 2010\Projects\com.plutext.search-TRUNK\com.plutext.search.main\OpcHelper.cs:line 98
           at System.Linq.Enumerable.WhereSelectEnumerableIterator`2.MoveNext()
           at System.Xml.Linq.XContainer.AddContentSkipNotify(Object content)
           at System.Xml.Linq.XContainer.AddContentSkipNotify(Object content)
           at System.Xml.Linq.XElement..ctor(XName name, Object content)
           at System.Xml.Linq.XElement..ctor(XName name, Object[] content)
           at com.plutext.search.main.OpcHelper.OpcToFlatOpc(Package package) in C:\Users\jharrop\Documents\Visual Studio 2010\Projects\com.plutext.search-TRUNK\com.plutext.search.main\OpcHelper.cs:line 93
           at com.plutext.search.main.DocumentMagnifierXml.OpenXmlAction(Document doc, List`1 tokens, Boolean hiderAction, Boolean highlighterAction) in C:\Users\jharrop\Documents\Visual Studio 2010\Projects\com.plutext.search-TRUNK\com.plutext.search.main\DocumentMagnifierXml.cs:line 235
           at com.plutext.search.main.DocumentMagnifierXml.displayResultDocument(String queryString, List`1 tokens, List`1 results, StateNavCurrentIndex index) in C:\Users\jharrop\Documents\Visual Studio 2010\Projects\com.plutext.search-TRUNK\com.plutext.search.main\DocumentMagnifierXml.cs:line 114
           at com.plutext.search.main.ResultsOverview.bookmark_BeforeDoubleClick(Object sender, ClickEventArgs e) in C:\Users\jharrop\Documents\Visual Studio 2010\Projects\com.plutext.search-TRUNK\com.plutext.search.main\ResultsOverview.cs:line 449
        com.plutext.search.main.GlobalErrorHandler.HandleException Unable to determine the identity of domain.
     * 
     */

    public static class OpcHelper
    {

        static Logger log = LogManager.GetLogger("com.plutext.search");

        // See http://blogs.msdn.com/b/ericwhite/archive/2010/02/02/increasing-performance-of-word-automation-for-large-amount-of-data-using-open-xml-sdk.aspx

    /// <summary>
    /// Returns the part contents in xml
    /// </summary>
    /// <param name="part">System.IO.Packaging.Packagepart</param>
    /// <returns></returns>
    static XElement GetContentsAsXml(PackagePart part)
    {
        XNamespace pkg = 
           "http://schemas.microsoft.com/office/2006/xmlPackage";
        if (part.ContentType.EndsWith("xml"))
        {
            using (Stream partstream = part.GetStream())
            using (StreamReader streamReader = new StreamReader(partstream))
            {
                string streamString = streamReader.ReadToEnd();
                XElement newXElement = 
                    new XElement(pkg + "part", new XAttribute(pkg + "name", part.Uri), 
                        new XAttribute(pkg + "contentType", part.ContentType), 
                        new XElement(pkg + "xmlData", XElement.Parse(streamString)));
                return newXElement;
            }
         }
        else
        {
            using (Stream str = part.GetStream())
            using (BinaryReader binaryReader = new BinaryReader(str))
            {
                int len = (int)binaryReader.BaseStream.Length;
                byte[] byteArray = binaryReader.ReadBytes(len);
                // the following expression creates the base64String, then chunks
                // it to lines of 76 characters long
                string base64String = (System.Convert.ToBase64String(byteArray))
                    .Select
                    (
                        (c, i) => new
                        {
                            Character = c,
                            Chunk = i / 76
                        }
                    )
                    .GroupBy(c => c.Chunk)
                    .Aggregate(
                        new StringBuilder(),
                        (s, i) =>
                            s.Append(
                                i.Aggregate(
                                    new StringBuilder(),
                                    (seed, it) => seed.Append(it.Character),
                                    sb => sb.ToString()
                                )
                            )
                            .Append(Environment.NewLine),
                        s => s.ToString()
                    );

                return new XElement(pkg + "part",
                    new XAttribute(pkg + "name", part.Uri),
                    new XAttribute(pkg + "contentType", part.ContentType),
                    new XAttribute(pkg + "compression", "store"),
                    new XElement(pkg + "binaryData", base64String)
                );
            }
        }
    }
    /// <summary>
    /// Returns an XDocument
    /// </summary>
    /// <param name="package">System.IO.Packaging.Package</param>
    /// <returns></returns>
    public static XDocument OpcToFlatOpc(Package package)
    {
        XNamespace 
            pkg = "http://schemas.microsoft.com/office/2006/xmlPackage";
        XDeclaration 
            declaration = new XDeclaration("1.0", "UTF-8", "yes");
        XDocument doc = new XDocument(
            declaration,
            new XProcessingInstruction("mso-application", "progid=\"Word.Document\""),
            new XElement(pkg + "package",
                new XAttribute(XNamespace.Xmlns + "pkg", pkg.ToString()),
                package.GetParts().Select(part => GetContentsAsXml(part))
            )
        );
        return doc;
    }
    /// <summary>
    /// Returns a System.IO.Packaging.Package stream for the given range.
    /// </summary>
    /// <param name="range">Range in word document</param>
    /// <returns></returns>
    public static Stream GetPackageStreamFromRange(this Range range)
    {
        //string s = range.WordOpenXML;
        //XDocument doc = XDocument.Parse(s);
        XDocument doc = XDocument.Parse(range.WordOpenXML);
        XNamespace pkg =
           "http://schemas.microsoft.com/office/2006/xmlPackage";
        XNamespace rel =
            "http://schemas.openxmlformats.org/package/2006/relationships";
        Package InmemoryPackage = null;
        MemoryStream memStream = new MemoryStream();
        using (InmemoryPackage = Package.Open(memStream, FileMode.Create))
        {
            // add all parts (but not relationships)
            foreach (var xmlPart in doc.Root
                .Elements()
                .Where(p =>
                    (string)p.Attribute(pkg + "contentType") !=
                    "application/vnd.openxmlformats-package.relationships+xml"))
            {
                string name = (string)xmlPart.Attribute(pkg + "name");
                string contentType = (string)xmlPart.Attribute(pkg + "contentType");
                if (contentType.EndsWith("xml"))
                {
                    Uri u = new Uri(name, UriKind.Relative);
                    PackagePart part = InmemoryPackage.CreatePart(u, contentType,
                        CompressionOption.SuperFast);
                    using (Stream str = part.GetStream(FileMode.Create))
                    using (XmlWriter xmlWriter = XmlWriter.Create(str))
                        xmlPart.Element(pkg + "xmlData")
                            .Elements()
                            .First()
                            .WriteTo(xmlWriter);
                }
                else
                {
                    Uri u = new Uri(name, UriKind.Relative);
                    PackagePart part = InmemoryPackage.CreatePart(u, contentType,
                        CompressionOption.SuperFast);
                    using (Stream str = part.GetStream(FileMode.Create))
                    using (BinaryWriter binaryWriter = new BinaryWriter(str))
                    {
                        string base64StringInChunks =
                       (string)xmlPart.Element(pkg + "binaryData");
                        char[] base64CharArray = base64StringInChunks
                            .Where(c => c != '\r' && c != '\n').ToArray();
                        byte[] byteArray =
                            System.Convert.FromBase64CharArray(base64CharArray,
                            0, base64CharArray.Length);
                        binaryWriter.Write(byteArray);
                    }
                }
            }
            foreach (var xmlPart in doc.Root.Elements())
            {
                string name = (string)xmlPart.Attribute(pkg + "name");
                string contentType = (string)xmlPart.Attribute(pkg + "contentType");
                if (contentType ==
                    "application/vnd.openxmlformats-package.relationships+xml")
                {
                    // add the package level relationships
                    if (name == "/_rels/.rels")
                    {
                        foreach (XElement xmlRel in
                            xmlPart.Descendants(rel + "Relationship"))
                        {
                            string id = (string)xmlRel.Attribute("Id");
                            string type = (string)xmlRel.Attribute("Type");
                            string target = (string)xmlRel.Attribute("Target");
                            string targetMode =
                                (string)xmlRel.Attribute("TargetMode");
                            if (targetMode == "External")
                                InmemoryPackage.CreateRelationship(
                                    new Uri(target, UriKind.Absolute),
                                    TargetMode.External, type, id);
                            else
                                InmemoryPackage.CreateRelationship(
                                    new Uri(target, UriKind.Relative),
                                    TargetMode.Internal, type, id);
                        }
                    }
                    else
                    // add part level relationships
                    {
                        string directory = name.Substring(0, name.IndexOf("/_rels"));
                        string relsFilename = name.Substring(name.LastIndexOf('/'));
                        string filename =
                            relsFilename.Substring(0, relsFilename.IndexOf(".rels"));
                        PackagePart fromPart = InmemoryPackage.GetPart(
                            new Uri(directory + filename, UriKind.Relative));
                        foreach (XElement xmlRel in
                            xmlPart.Descendants(rel + "Relationship"))
                        {
                            string id = (string)xmlRel.Attribute("Id");
                            string type = (string)xmlRel.Attribute("Type");
                            string target = (string)xmlRel.Attribute("Target");
                            string targetMode =
                                (string)xmlRel.Attribute("TargetMode");
                            if (targetMode == "External")
                            {
                                // new Uri chokes if target is www.artslaw.com.au
                                // which Word 2007 does sometimes
                                // as opposed to http://
                                // NB, in Word 2007, you can make a hyperlink to
                                // foo://bar ie there is no validation
                                Uri uri;
                                try
                                {
                                    uri = new Uri(target, UriKind.Absolute);
                                }
                                catch (System.UriFormatException ufe)
                                {
                                    log.Error("broken uri target: " + target);
                                    try
                                    {
                                        uri = new Uri(target, UriKind.Relative);
                                        log.Error(".. but relative seems to work");
                                    }
                                    catch (System.UriFormatException ufe2)
                                    {
                                        // Make up something
                                        uri = new Uri("http://www.microsoft.com", UriKind.Absolute);
                                    }
                                }
                                fromPart.CreateRelationship(
                                    uri,
                                    TargetMode.External, type, id);

                                //if (target.StartsWith("www")) {
                                //    target = "http://" + target;
                                //}

                                //if (target.StartsWith("http") || target.StartsWith("file")
                                //    || target.StartsWith("smb") || target.StartsWith("cifs")  // does Word use these?
                                //    || target.StartsWith("mailto"))
                                //{

                                //    fromPart.CreateRelationship(
                                //        new Uri(target, UriKind.Absolute),
                                //        TargetMode.External, type, id);
                                //}
                                //else
                                //{
                                //    // TODO: handle relative uri better
                                //    fromPart.CreateRelationship(
                                //        new Uri(target, UriKind.Relative),
                                //        TargetMode.External, type, id);
                                //}
                            }
                            else
                                fromPart.CreateRelationship(
                                    new Uri(target, UriKind.Relative),
                                    TargetMode.Internal, type, id);
                        }
                    }
                }
            }
            InmemoryPackage.Flush();
        }
        return memStream;
    }
}
}
