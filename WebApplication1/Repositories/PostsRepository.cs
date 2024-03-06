using Google.Cloud.Firestore;
using WebApplication1.Models;

namespace WebApplication1.Repositories
{
    public class PostsRepository
    {
        private FirestoreDb db;
        public PostsRepository(string project)
        {
            db = FirestoreDb.Create(project); //the initialization of the database
        }

        /// <summary>
        /// This method will insert a new post instance in the No-Sql database as a nested collection inside a blog
        /// </summary>
        /// <param name="b"></param>
        public async void AddPost(Post p)
        {
            ///blogs/9a09be33-70df-4a13-b286-7a17105272cb/posts/H6eYU8cfuIBt56g01lL4
            DocumentReference docRef = db.Collection($"blogs/{p.BlogId}/posts").Document(p.Id);
            await docRef.SetAsync(p);
        }

        /// <summary>
        /// This method will return all the posts/articles that are associated with a particular blog
        /// </summary>
        /// <param name="blogId"></param>
        /// <returns></returns>
        public async Task<List<Post>> GetPosts(string blogId)
        {
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


        public async Task<Post> GetPost(string blogId, string postId)
        {
           
            var docRef  = await db.Collection($"blogs/{blogId}/posts").Document(postId).GetSnapshotAsync(); //creating a query object to query the collection called blogs

            //looping with the snapshot because there might me more than 1 blog

            if (docRef == null)
            {
                return null;
            }
            
            Post p = docRef.ConvertTo<Post>(); //converts from json data to a custom object
            p.Id = docRef.Id; //assign the Id used for the blog within the no-sql database 
            p.BlogId = blogId;

            return p; //returning the only insance returned

        }
    }
}
