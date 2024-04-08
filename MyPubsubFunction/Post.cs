using Google.Cloud.Firestore;

namespace MyPubsubFunction
{
    [FirestoreData]
    public class Post
    {
      
        public string Id { get; set; }
        
        [FirestoreProperty]
        public string Title { get; set; }

        [FirestoreProperty]
        public string Content { get; set; }

        [FirestoreProperty]
        public Timestamp DateCreated { get; set; }
        [FirestoreProperty]
        public Timestamp DateUpdated { get; set; }

        [FirestoreProperty]
        public string Photo { get; set; }

        public string BlogId { get; set; }
    }
}
