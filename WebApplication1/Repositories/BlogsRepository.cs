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


        public BlogsRepository() {
          //dependency injection
          //
          //FirestoreDb db = FirestoreDb.Create(project);

        }


        /// <summary>
        /// This method will insert a new blog instance in the No-Sql database 
        /// </summary>
        /// <param name="b"></param>
        public async void AddBlog(Blog b)
        {
            DocumentReference docRef = db.Collection("blogs").Document(b.Id);
            await docRef.SetAsync(b);

        }


    }
}
