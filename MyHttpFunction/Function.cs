using Google.Cloud.Functions.Framework;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Google.Cloud.Firestore;
using System;
using Google.Apis.Logging;

namespace MyHttpFunction;

public class Function : IHttpFunction
{
    public Function()
    {
          string pathToKeyFile = "msd63a2024-d05b0e7fc641.json";
        System.Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", pathToKeyFile);
    }

    /// <summary>
    /// Logic for your function goes here.
    /// </summary>
    /// <param name="context">The HTTP context, containing the request and the response.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task HandleAsync(HttpContext context)
    {
        string blogId = context.Request.Query["blogid"].ToString();
        int count = (await GetPosts(blogId));
        //Console.WriteLine("in this blog you have " + count + " posts");
        await context.Response.WriteAsync("in this blog you have " + count + " posts");
    }
      public async Task<int> GetPosts(string blogId)
        {  
            var db = FirestoreDb.Create("msd63a2024"); //your-project-id
            Query postsQuery = db.Collection($"blogs/{blogId}/posts"); //creating a query object to query the collection called blogs
            QuerySnapshot postsQuerySnapshot = await postsQuery.GetSnapshotAsync();
            //looping with the snapshot because there might me more than 1 blog
           return  postsQuerySnapshot.Documents.Count;
        }
}
