Imports System
Imports System.IO
Imports Atalasoft.Imaging
Imports Atalasoft.Imaging.Codec
Imports Atalasoft.Imaging.Codec.Pdf
Imports System.Windows.Forms

Namespace TIFFtoPDF
    ''' <summary>
    ''' Summary description for Class1.
    ''' </summary>
    Friend Class Module1
        ''' <summary>
        ''' Converts any multipage PDF file into a multipage TIFF file
        ''' </summary>
        <STAThread()> _
        Public Shared Sub Main(ByVal args As String())
            DoAboutSplash()

            Console.WriteLine("TIFFtoPDF starting")

            Dim imgPath As String = GetWorkingDir()
            Dim inFile As String = Nothing

            Console.WriteLine("Please select the TIFF (or other supported file type) to convert to PDF:")

            Using dlg As New OpenFileDialog()
                dlg.InitialDirectory = imgPath
                dlg.FileName = Path.GetFileName(inFile)
                dlg.Filter = "Tagged Image File Format (TIFF)|*.tif;*.tiff;|All Files (*.*)|*.*"
                If dlg.ShowDialog() = DialogResult.OK Then
                    inFile = dlg.FileName
                Else
                    inFile = Nothing
                End If
            End Using

            If inFile Is Nothing Then
                Console.WriteLine("No input file SelectedOperation canceled")
            Else
                Dim outFile As String = System.IO.Path.ChangeExtension(inFile, ".pdf")

                Console.WriteLine(Convert.ToString("  inFile: ") & inFile)

                Console.WriteLine("Please select a location to save the outgoing PDF to:")
                Using dlg As New SaveFileDialog()
                    dlg.Title = "Select Location to Save output PDF to"
                    dlg.Filter = "Portable Document Format (.pdf)|*.pdf"
                    dlg.DefaultExt = ".pdf"
                    dlg.InitialDirectory = Path.GetDirectoryName(inFile)
                    dlg.FileName = Path.GetFileName(outFile)

                    If dlg.ShowDialog() = DialogResult.OK Then
                        outFile = dlg.FileName
                    Else
                        ' force an error to stop the app
                        outFile = Nothing
                    End If
                End Using

                Console.WriteLine(Convert.ToString("  outFile: ") & outFile)

                If outFile Is Nothing Then
                    Console.WriteLine("No Output selcted: Operation canceled")
                Else

                    Console.WriteLine("Converting file...")

                    ' start timer
                    Dim tick1 As Integer = System.Environment.TickCount

                    ' Do the conversion
                    If File.Exists(inFile) Then
                        ConvertTiffToPdf(inFile, outFile)
                    Else
                        Console.WriteLine("file not found... doing nothing")
                    End If

                    ' finish timer
                    Dim tick4 As Integer = System.Environment.TickCount

                    Console.WriteLine("Conversion complete: Total time " + (tick4 - tick1).ToString() + " ms")
                End If
            End If

            Console.WriteLine("TIFFtoPDF finished... press ENTER to quit")
            Console.ReadLine()
        End Sub


        ''' <summary>
        ''' Given the filename of a tiff file as input and a pdf file as output, convert the
        ''' incoming tiff to a SingleImageOnly PDF
        ''' 
        ''' This demonstrates the most memory-efficient way to do the conversion.
        ''' </summary>
        ''' <param name="inFile">A tiff file to use as the input</param>
        ''' <param name="outFile">A filename to call the outgoing PDF (note that it will delete any existing file by that name first.</param>
        Private Shared Sub ConvertTiffToPdf(ByVal inFile As String, ByVal outFile As String)
            ' Make sure we get rid of the output file if it already exists
            If File.Exists(outFile) Then
                File.Delete(outFile)
            End If

            ' Create your encoder
            Dim pdfEnc As New PdfEncoder()

            ' Forces the image to fit to page and not page to fit to image
            pdfEnc.SizeMode = PdfPageSizeMode.FitToPage

            ' For Jpeg compression (not Jpeg2000), you can use this value to request 
            ' higher compression or quality... the higher the number, the better the 
            ' quality, but the less effective the compression. 80 is the default value/
            pdfEnc.JpegQuality = 80

            ' If you wish to use Jbig2 or Jpeg2000 compression, uncomment the following line to 
            ' instruct the encoder that they're available... NOTE that you will need to add 
            ' the correct references to add such support
            'pdfEnc.UseAdvancedImageCompression = true;

            ' set up an event handler that will pop for each image to let you determine the compression
            AddHandler pdfEnc.SetEncoderCompression, AddressOf pdfEnc_SetEncoderCompression

            ' Reading this direct from a FileSystemImageSource will be much more efficient than using an ImageCollection
            Using imgSrc As FileSystemImageSource = New FileSystemImageSource(inFile, True)
                Using fs As FileStream = New FileStream(outFile, FileMode.Create)
                    ' The magic of using an ImageSource and Stream with the pdfEncoder is 
                    ' that only the active portion of a given image is in memory at one time
                    ' so it's very memory efficient.
                    '
                    ' This effectively does   while(imgSrc.HasMoreImages()) { AtalaImage img = imgSrc.AcquireNext(); ... }
                    '
                    ' Each page willl trigger a new pdfEnc_SetEncoderCompression event 
                    ' where you can choose the type of compression you want to apply on a page-by-page basis
                    pdfEnc.Save(fs, imgSrc, Nothing)
                End Using
            End Using

            pdfEnc = Nothing
        End Sub


        ''' <summary>
        ''' Intelligently selects the appropriate form of compression to apply to the current page
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Shared Sub pdfEnc_SetEncoderCompression(ByVal sender As Object, ByVal e As EncoderCompressionEventArgs)
            If e.Image.PixelFormat = PixelFormat.Pixel1bppIndexed Then
                ' Here, we've chosen CcittGroup4 as our B&W conversion.
                '
                ' Your viable Advanced compression type would be Jbig2.
                '
                ' To enable Jbig2, you need to add a reference to 
                ' Atalasoft.dotImage.Jbig2.dll and make sure that 
                ' pdfEnc.UseAdvancedImageCompression = true;   is uncommented above
                '
                ' NOTE: You must have either a paid or eval license for Jbig2 to use Jbig2 compression
                'e.Compression = new PdfCodecCompression(PdfCompressionType.Jbig2);
                e.Compression = New PdfCodecCompression(PdfCompressionType.CcittGroup4)
            ElseIf e.Image.PixelFormat = PixelFormat.Pixel8bppGrayscale OrElse e.Image.PixelFormat = PixelFormat.Pixel24bppBgr Then
                ' Here, we've chosen Jpeg2000 advanced color compression 
                ' ... Jpeg2000 may be able to handle other types of PixelFormat 
                '          - simply add them in the else/if statement
                ' your other viable compression tipes would be Jpeg, 

                ' Here, we've chosen Jpeg as our Color and Grayscale conversion.
                '
                ' Your viable Advanced compression type would be Jpeg2000.
                '
                ' To enable Jpeg2000, you need to add a reference to 
                ' Atalasoft.dotImage.Jpeg2000.dll and make sure that 
                ' pdfEnc.UseAdvancedImageCompression = true;   is uncommented above 
                '
                ' NOTE: You must have either a paid or eval license for Jpeg2000 to use Jpeg2000 compression
                ' e.Compression = new PdfCodecCompression(PdfCompressionType.Jpeg2000);

                e.Compression = New PdfCodecCompression(PdfCompressionType.Jpeg)
            Else
                ' Fallback method in case the pixelFormat isn't one of the ones defined above
                e.Compression = New PdfCodecCompression(PdfCompressionType.Deflate)
            End If
        End Sub

        ''' <summary>
        ''' Outputs the "About" spash info
        ''' </summary>
        Private Shared Sub DoAboutSplash()
            Console.WriteLine("TIFFtoPDF Demo")
            Console.WriteLine()
            Console.WriteLine("***************************************************************************")
            Console.WriteLine("A very simple console app that converts a TIFF file into a PDF by")
            Console.WriteLine("using in a memory-efficient way using FileSystemImageSource.")
            Console.WriteLine()
            Console.WriteLine("Who says you always need a viewer in an imaging application?")
            Console.WriteLine()
            Console.WriteLine("This console app uses our FileSystemImageSource to read each frame")
            Console.WriteLine("of the target file directly into a PDF Encoder. Each page read will")
            Console.WriteLine("trigger a Compression Selector event so you can choose the best compression")
            Console.WriteLine("to use for that specific page's Pixel Format")
            Console.WriteLine()
            Console.WriteLine("This approach can easily be adapted to services or plumbed in to")
            Console.WriteLine("batch-based processing.")
            Console.WriteLine()
            Console.WriteLine("By setting a handler for PdfEnc.SetEncoderCompression, we are ")
            Console.WriteLine("able to dynamically select the most appropriate form of image ")
            Console.WriteLine("compression to apply, based on the PixelFormat (color depth) of each page")
            Console.WriteLine()
            Console.WriteLine()
            Console.WriteLine("Download the DotImage SDK at:")
            Console.WriteLine("     http://www.atalasoft.com/products/download/dotimage")
            Console.WriteLine()
            Console.WriteLine("Download the DotImage API Help Installers at:")
            Console.WriteLine("     http://www.atalasoft.com/support/dotimage/help/install")
            Console.WriteLine()
            Console.WriteLine("Download the full sources for this demo at:")
            Console.WriteLine("     http://www.atalasoft.com/KB/article.aspx?id=10412")
            Console.WriteLine("***************************************************************************")
            Console.WriteLine()
        End Sub


        ''' <summary>
        ''' Convenience method to get the root directory of the project - really only useful for debugging
        ''' </summary>
        ''' <returns></returns>
        Private Shared Function GetWorkingDir() As String
            Dim cwd As String = System.IO.Directory.GetCurrentDirectory()
            'Console.WriteLine("cwd is '{0}'", cwd);

            If cwd.EndsWith("\bin\Debug") Then
                'Console.WriteLine("updated cwd is '{0}'", cwd);
                cwd = cwd.Replace("\bin\Debug", "\..\..\")
            ElseIf cwd.EndsWith("\bin") Then
                cwd = cwd.Replace("\bin", "\..\")
            End If
            Return cwd
        End Function
    End Class
End Namespace
