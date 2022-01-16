using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Appts.Web.Ui.Scheduler.Repositories
{
  public interface IBlobAvatarRepository
  {
    Task<bool> UploadFileToStorageAsync(Stream fileStream, string fileName);
    Task<MemoryStream> DownloadBlobFromUriAsync(string uri);
    bool IsImage(IFormFile file);
    //Task<List<string>> GetThumbNailUrls();
    bool CreateDefaultAvatar(string userId);
  }
}
