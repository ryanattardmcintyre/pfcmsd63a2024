using PdfSharp.Drawing;
using PdfSharp.Fonts;
using PdfSharp.Pdf;
using System;
using System.IO;
using Google.Cloud.Storage.V1;
 
 namespace MyPubsubFunction{
 public class FileFontResolver : IFontResolver // FontResolverBase
    {
        public string DefaultFontName => throw new NotImplementedException();

        public byte[] GetFont(string faceName)
        {
            //this snippet of code is going to open a file called Verdana.ttf from a folder
            //called bin which will be generated at runtime thus erasing whatever is already inside it
           /* using (var ms = new MemoryStream())
            {
                using (var fs = File.Open(faceName, FileMode.Open))
                {
                    fs.CopyTo(ms);
                    ms.Position = 0;
                    return ms.ToArray();
                }
            }*/

            //solution: we can place the Verdana.ttf in a bucket
            //          we can read the Verdana.ttf from the online bucket
            //https://storage.cloud.google.com/msd63apfcra_fg/Verdana.ttf

                var storage = StorageClient.Create();
                MemoryStream stream = new MemoryStream();
                storage.DownloadObject("msd63apfcra_fg", "Verdana.ttf", stream);
                stream.Position=0;
                return stream.ToArray();

        }

 
        public FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
        {
            if (familyName.Equals("Verdana", StringComparison.CurrentCultureIgnoreCase))
            {
                if (isBold && isItalic)
                {
                    return new FontResolverInfo("Verdana-BoldItalic.ttf");
                }
                else if (isBold)
                {
                    return new FontResolverInfo("Verdana-Bold.ttf");
                }
                else if (isItalic)
                {
                    return new FontResolverInfo("Verdana-Italic.ttf");
                }
                else
                {
                    return new FontResolverInfo("Verdana.ttf");
                }
            }
            return null;
        }
    }
 }