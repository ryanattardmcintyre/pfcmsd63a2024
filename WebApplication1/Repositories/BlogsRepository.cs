using Google.Cloud.Firestore;
using WebApplication1.Models;

namespace WebApplication1.Repositories
{

    //The repository classes will manage the data around (fetching data from the database, writing data to the
    //database, deleting data from the database, updating data to the database)

    //Implement CRUD operations
    //Create, Read, Update, Delete
    public class BlogsRepository
    {

        private FirestoreDb db;
        public BlogsRepository(string project) {
           db = FirestoreDb.Create(project); //the initialization of the database
        }


        /// <summary>
        /// This method will insert a new blog instance in the No-Sql database 
        /// </summary>
        /// <param name="b"></param>
        public async void AddBlog(Blog b)
        {
            b.Id = Guid.NewGuid().ToString();
            DocumentReference docRef = db.Collection("blogs").Document(b.Id);
            await docRef.SetAsync(b);

        }

        public async Task<List<Blog>> GetBlogs()
        {
            List<Blog> blogs = new List<Blog>(); //creating an empty list where i will be holding my returned blog instances

            Query blogsQuery = db.Collection("blogs"); //creating a query object to query the collection called blogs
            QuerySnapshot blogsQuerySnapshot = await blogsQuery.GetSnapshotAsync();
            //looping with the snapshot because there might me more than 1 blog
            foreach (DocumentSnapshot documentSnapshot in blogsQuerySnapshot.Documents) 
            {
                    Blog b = documentSnapshot.ConvertTo<Blog>(); //converts from json data to a custom object
                    b.Id = documentSnapshot.Id; //assign the Id used for the blog within the no-sql database 
                    blogs.Add(b); //adding the instance into the prepared list on line34
            }

            return blogs; //returning the prepared list
        }


    }
}
