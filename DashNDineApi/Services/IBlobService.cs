namespace DashNDineApi.Services
{
    public interface IBlobService
    {
        public Task<string> GetBlob(string blobName, string containerName);

        public Task<bool> DeleteBlob(string blobName, string containerName);

        public Task<string> UploadBlob(string blobName, string containerName, IFormFile file);
    }
}
