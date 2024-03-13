using Google.Apis.Storage.v1.Data;
using Google.Cloud.Storage.V1;
using System.Security.AccessControl;
using System.Text;

namespace WebApplication1.Repositories
{
    public class BucketsRepository
    {

        private string _projectId;
        private string _bucketName;
        public BucketsRepository(string projectId, string bucketName) { 
            _projectId = projectId;
            _bucketName = bucketName;
        }

        public async Task<Google.Apis.Storage.v1.Data.Object> UploadFile(string filename, MemoryStream ms)
        {
            var storage = StorageClient.Create();
            //if you dont await for this line to complete, that it stars the upload but it does not wait for it to finish
            //meaning that you might never see the file in the bucket

            //returning the result of this line meaning a process which keeps track of the file being uploaded
            return await storage.UploadObjectAsync(_bucketName, filename, "application/octet-stream", ms);
        }


        public async Task<Google.Apis.Storage.v1.Data.Object> GrantAccess(string filename, string recipient)
        {
            var storage = StorageClient.Create();
            var storageObject = storage.GetObject(_bucketName, filename, new GetObjectOptions
            {
                Projection = Projection.Full
            });

            storageObject.Acl.Add(new ObjectAccessControl
            {
                Bucket = _bucketName,
                Entity = $"user-{recipient}",
                Role = "READER",
            });
            return await storage.UpdateObjectAsync(storageObject);
        }

    }
}
