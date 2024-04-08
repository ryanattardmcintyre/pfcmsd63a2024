using Microsoft.AspNetCore.Mvc;
using PdfSharp.Drawing;
using PdfSharp.Fonts;
using PdfSharp.Pdf;
using PdfSharpCore.Utils;
using WebApplication1.Repositories;

namespace WebApplication1.Controllers
{
    public class ReportsController : Controller
    {
        private PubsubRepository _pubsubRepository;
        private PostsRepository _postsRepository;
        private BucketsRepository _bucketsRepository;
        public ReportsController(PubsubRepository pubsubRepository,
            PostsRepository postsRepository, 
            BucketsRepository bucketsRepository) {
        _pubsubRepository= pubsubRepository;
            _postsRepository = postsRepository;

            _bucketsRepository= bucketsRepository;
        }

        public async Task<IActionResult> Generate()
        {
            string blogId =  _pubsubRepository.PullMessagesSync("msd63a2024ra-sub", true);

            if (string.IsNullOrEmpty(blogId) == false)
            {
                //we have blogid with data

                //putting everything inside a pdf.
                PdfDocument document = new PdfDocument();

                // Add a new page to the document
                PdfPage page = document.AddPage();

                var myPosts = await _postsRepository.GetPosts(blogId); //will get a list of posts pertaining to a blog

                int yPosition = 10;

                GlobalFontSettings.FontResolver = new FileFontResolver();
                XFont font = new XFont("Verdana", 12, XFontStyleEx.Regular);
                // Get an XGraphics object for drawing
                using (XGraphics gfx = XGraphics.FromPdfPage(page))
                {
                    foreach (var post in myPosts)
                    {
                        // Draw the text on the page
                        gfx.DrawString(post.Title, font, XBrushes.Black, new XRect(10, yPosition, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                        // Move to the next line (increase Y-coordinate position)
                        yPosition += font.Height;
                        gfx.DrawString(post.Content, font, XBrushes.Black, new XRect(10, yPosition, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                        // Move to the next line (increase Y-coordinate position)
                        yPosition += font.Height;
                        gfx.DrawString("-----------------------------------------------", font, XBrushes.Black, new XRect(10, yPosition, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
                        // Move to the next line (increase Y-coordinate position)
                        yPosition += (font.Height * 3);

                    }
                }
                string filenamePDF = blogId + ".pdf";
                document.Save(filenamePDF);
                //code to open back the file and upload it.

                MemoryStream msIn = new MemoryStream(System.IO.File.ReadAllBytes(filenamePDF));
                msIn.Position = 0;
                await _bucketsRepository.UploadFile(filenamePDF, msIn);
                System.IO.File.Delete(filenamePDF);

                //update a status field in the firestore to signal that the conversion took place
                //status = 0, >>> status =1


                return Content("pdf generated - done");
            }
            else return Content("error occurred");


          
        }
    }


    public class FileFontResolver : IFontResolver // FontResolverBase
    {
        public string DefaultFontName => throw new NotImplementedException();

        public byte[] GetFont(string faceName)
        {
            using (var ms = new MemoryStream())
            {
                using (var fs = File.Open(faceName, FileMode.Open))
                {
                    fs.CopyTo(ms);
                    ms.Position = 0;
                    return ms.ToArray();
                }
            }
        }

        public FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
        {
            if (familyName.Equals("Verdana", StringComparison.CurrentCultureIgnoreCase))
            {
                if (isBold && isItalic)
                {
                    return new FontResolverInfo("Fonts/Verdana-BoldItalic.ttf");
                }
                else if (isBold)
                {
                    return new FontResolverInfo("Fonts/Verdana-Bold.ttf");
                }
                else if (isItalic)
                {
                    return new FontResolverInfo("Fonts/Verdana-Italic.ttf");
                }
                else
                {
                    return new FontResolverInfo("Fonts/Verdana.ttf");
                }
            }
            return null;
        }
    }
}
