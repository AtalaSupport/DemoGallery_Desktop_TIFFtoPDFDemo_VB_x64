# TIFFtoPDFDemo
A very simple console app that converts a TIFF file into a PDF by
using in a memory-efficient way using FileSystemImageSource.

Who says you always need a viewer in an imaging application?

This console app uses our FileSystemImageSource to read each frame
of the target file directly into a PDF Encoder. Each page read will
trigger a Compression Selector event so you can choose the best compression
to use for that specific page's Pixel Format

This approach can easily be adapted to services or plumbed in to
batch-based processing.

By setting a handler for PdfEnc.SetEncoderCompression, we are 
able to dynamically select the most appropriate form of image 
compression to apply, based on the PixelFormat (color depth) of each page.

NOTE that tecnically, the input file is not limited to TIfF, any supported 
input file type may be used. Some require additional references and to have 
additional decoders added to RegisteredDecoders.Decoders. Please feel free 
to reach out to support if you have questions.

This is a VB.NET solution. We also offer a [C# version](https://github.com/AtalaSupport/DemoGallery_Desktop_TIFFtoPDFDemo_CS_x64).


## Licensing
This application requires a license for DotImage Document Imaging. You may also request a 30 day evaulation if youre evaluating if DotImage / our OCR is right for you.


## SDK Dependencies
This app was built based on 2026.2.0.0. It targets .NET Framework 4.6.2 and was created in Visual Studio 2022. You must have our SDK installed (and licesed per above)

[Download DotImage](https://www.atalasoft.com/BeginDownload/DotImageDownloadPage)


### Using NuGet for SDK Dependencies
We do publish our SDK components to NuGet. We have chosen to base the demo on local installed SDK because this leads to much smaller applications (NuGet packages add a lot of overhead due to the way they're packaged and deployed, and many of our demos -- including this one -- are often used to reproduce issues that need to be submitted to support. Apps that use NuGet are often significantly larger and run up against our maximum support case upload size)

Still, if you wish to use NuGet for the dependencies instead of relying on locally installed SDK, you can.

- Take note of each of the references we've included:
    - Atalasoft.DotImage.dll
    - Atalasoft.DotImage.AdvancedDocClean.dll
    - Atalasoft.DotImage.Lib.dll
    - Atalasoft.DotImage.WinControls.dll
    - Atalasoft.PdfDoc.dll
    - Atalasoft.Shared.dll
- Remove those referneces
- Open the NuGet Package Manger from `Tools -> NuGet Package Manager -> Manage NuGet Packages for this Solution`
- Browse for and install  Atalasoft.DotImage.WinControls.x64 - It will pull in DotImage Document Imaging (the base SDK) and our windows controls and shared dll


## Downloading source
The sources can be downloaded for [c#](https://github.com/AtalaSupport/DemoGallery_Desktop_TiffManipulatorDemo_CS_x64/archive/refs/heads/main.zip) and [VB.NET](https://github.com/AtalaSupport/DemoGallery_Desktop_TiffManipulatorDemo_VB_x64/archive/refs/heads/main.zip)


## Cloning
If you wish to clone the repo, we recommend:

Example: git for windows
```bash
git clone https://github.com/AtalaSupport/DemoGallery_Desktop_TiffManipulatorDemo_CS_x64.git TiffManipulatorDemo
```


## Related documentation
In addition to this README, the Atalasoft documentation set includes the following:  
- [AtalaSupport Github](https://github.com/AtalaSupport/) For an extensive set of sample apps.  
- [Atalasoft's APIs & Developer Guides page](https://www.atalasoft.com/Support/APIs-Dev-Guides) for our Developers guide and API references.  
- [Atalasoft Support](http://www.atalasoft.com/support/) for our main support portal.
- [Atalasoft Knowledgebase](http://www.atalasoft.com/kb2) where you can find answers to common questions / issues.  


## Getting Help for Atalasoft products
Atalasoft regularly updates our support [Knowledgebase](http://www.atalasoft.com/kb2) with the latest information about our products. To access some resources, you must have a valid Support Agreement with an authorized Atalasoft Reseller/Partner or with Atalasoft directly. Use the tools that Atalasoft provides for researching and identifying issues. 

Customers with an active evaluation, or those with active support / maintenance may [create a support case](https://www.atalasoft.com/Support/my-portal/Cases/Create-Case) 24/7, or call in to support ([+1 949 236-6510](tel:19492366510) ) during our normal support hours (Monday - Friday 8:00am to 5:00PM Eastern (New York) time).  

Customers who are unable to create a case or call in may [email our Sales Team](email:sales@atalasoft.com).  

