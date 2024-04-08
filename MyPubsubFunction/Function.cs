using CloudNative.CloudEvents;
using Google.Cloud.Functions.Framework;
using Google.Events.Protobuf.Cloud.PubSub.V1;
using System;
using System.Threading;
using System.Threading.Tasks;
using PdfSharp.Drawing;
using PdfSharp.Fonts;
using PdfSharp.Pdf;
using System.IO;
using Google.Apis.Storage.v1.Data;
using Google.Cloud.Storage.V1;
using System.Collections.Generic;
using Google.Cloud.Firestore;

namespace MyPubsubFunction
{
    public class Function : ICloudEventFunction<MessagePublishedData>
    {
        public async Task HandleAsync(CloudEvent cloudEvent, 
        MessagePublishedData data,
        CancellationToken cancellationToken)
        {
            var blogId = data.Message?.TextData;
            if(string.IsNullOrEmpty(blogId))
            {
                //problem
                 Console.WriteLine($"Blog id is empty");
                // return Task.CompletedTask; //will research how to return an error code

            }
            else {
                //code to convert to pdf goes here
                Console.WriteLine($"Blog id: {blogId}");
                 //we have blogid with data
                //putting everything inside a pdf.
                PdfDocument document = new PdfDocument();
                Console.WriteLine("Pdfdocument created");
                // Add a new page to the document
                PdfPage page = document.AddPage();
                Console.WriteLine("page created");
                var myPosts = await GetPosts(blogId); //will get a list of posts pertaining to a blog
                Console.WriteLine("Read the following number of posts: " + myPosts.Count + " ");
                int yPosition = 10;
                Console.WriteLine("Initializing FontResolver");
                GlobalFontSettings.FontResolver = new FileFontResolver();
                Console.WriteLine("FontResolver successfully initialized");

                XFont font = new XFont("Verdana", 12, XFontStyleEx.Regular);
                Console.WriteLine("XFont initialized");
                // Get an XGraphics object for drawing
                using (XGraphics gfx = XGraphics.FromPdfPage(page))
                {
                    Console.WriteLine("XGraphics initialized");
                    foreach (var post in myPosts)
                    {
                        Console.WriteLine("Loop...Post: " + post.Title);
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

                        Console.WriteLine("Finished writing post in the pdfpage");

                    }
                }
                
                Console.WriteLine("Saving the doc...");
                string filenamePDF = blogId + ".pdf";
                document.Save(filenamePDF);
                Console.WriteLine("It saved the doc successfully");
                //code to open back the file and upload it.

               Console.WriteLine("Reading into memorystream the saved doc");
                MemoryStream msIn = new MemoryStream(System.IO.File.ReadAllBytes(filenamePDF));
                msIn.Position = 0;
                Console.WriteLine("Uploading inside the bucket");
                await UploadFile(filenamePDF, msIn);
                Console.WriteLine("Uploaded successfully");
                System.IO.File.Delete(filenamePDF);
                Console.WriteLine("Clearing the function from the uploaded file");

            }

            //return Task.CompletedTask;
        }

        public async Task<Google.Apis.Storage.v1.Data.Object> UploadFile(string filename, MemoryStream ms)
        {
            var storage = StorageClient.Create();
            //if you dont await for this line to complete, that it stars the upload but it does not wait for it to finish
            //meaning that you might never see the file in the bucket

            //returning the result of this line meaning a process which keeps track of the file being uploaded
            return await storage.UploadObjectAsync("msd63apfcra_fg", filename, "application/octet-stream", ms);
        }


        public async Task<List<Post>> GetPosts(string blogId)
        {
            var db = FirestoreDb.Create("msd63a2024"); 

            List<Post> posts = new List<Post>(); //creating an empty list where i will be holding my returned blog instances

            Query postsQuery = db.Collection($"blogs/{blogId}/posts"); //creating a query object to query the collection called blogs
            QuerySnapshot postsQuerySnapshot = await postsQuery.GetSnapshotAsync();
            //looping with the snapshot because there might me more than 1 blog
            foreach (DocumentSnapshot documentSnapshot in postsQuerySnapshot.Documents)
            {
                Post p = documentSnapshot.ConvertTo<Post>(); //converts from json data to a custom object
                p.Id = documentSnapshot.Id; //assign the Id used for the blog within the no-sql database 
                p.BlogId = blogId;

                posts.Add(p); //adding the instance into the prepared list on line34
            }

            return posts; //returning the prepared list
        }



    }
}