using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Auth;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
namespace Appts.Web.Ui.Scheduler.Repositories
{
  public class BlobAvatarRepository : IBlobAvatarRepository
  {
    private readonly string _accountName;
    private readonly string _accountKey;
    private readonly string _containerName;
    private readonly ILogger<BlobAvatarRepository> _logger;
    private readonly string _avatarContainerBase = "https://saapptscdn.blob.core.windows.net/thumbnail-avatars/default-user-avatar.jpg";
    public BlobAvatarRepository(IConfiguration config)
    {
      _accountName = config["AzureStorage:AccountName"];
      _accountKey = config["AzureStorage:AccountKey"];
      _containerName = config["AzureStorage:ImageContainer"];
    }
    public bool IsImage(IFormFile file)
    {
      if (file.ContentType.Contains("image"))
      {
        return true;
      }
      string[] formats = new string[] { ".jpg", ".png", ".gif", ".jpeg" };
      return formats.Any(item => file.FileName.EndsWith(item, StringComparison.OrdinalIgnoreCase));
    }
    public async Task<MemoryStream> DownloadBlobFromUriAsync(string uri)
    {
      StorageCredentials storageCredentials = new StorageCredentials(_accountName, _accountKey);
      CloudStorageAccount storageAccount = new CloudStorageAccount(storageCredentials, true);
      MemoryStream stream = new MemoryStream();
      try
      {
        CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
        CloudBlobContainer container = blobClient.GetContainerReference(_containerName);
        var blobName = new CloudBlockBlob(new Uri(uri)).Name;
        CloudBlockBlob blockBlob = container.GetBlockBlobReference(blobName);
        await blockBlob.DownloadToStreamAsync(stream).ConfigureAwait(false);
      }
      catch (Exception ex)
      {
        _logger.LogError("Failed to download blob from Uri", ex.Message, ex.StackTrace);
        throw; // bubble up
      }
      return stream;
    }
    public  async Task<bool> UploadFileToStorageAsync(Stream fileStream, string fileName)
    {
      StorageCredentials storageCredentials = new StorageCredentials(_accountName, _accountKey);
      CloudStorageAccount storageAccount = new CloudStorageAccount(storageCredentials, true);
      try
      {
        CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
        CloudBlobContainer container = blobClient.GetContainerReference(_containerName);
        CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);
        await blockBlob.UploadFromStreamAsync(fileStream);
      }
      catch (Exception ex)
      {
        _logger.LogError("Failed to upload blob from file stream", ex.Message, ex.StackTrace);
        throw;
      }
      return await Task.FromResult(true);
    }
    public bool CreateDefaultAvatar(string userId)
    {
      string uri = _avatarContainerBase;
      bool isUploaded;
      try
      {
        using (Stream stream = DownloadBlobFromUriAsync(uri).GetAwaiter().GetResult())
        {
          stream.Position = 0;
          isUploaded = UploadFileToStorageAsync(stream, userId).GetAwaiter().GetResult();
        }
      }
      catch (Exception ex)
      {
        _logger.LogError("Failed to create default blob", ex.Message, ex.StackTrace);
        throw;
      }
      return isUploaded;
    }
  }
}
