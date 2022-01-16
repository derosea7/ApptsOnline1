using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Appts.Models.Document;
using Appts.Models.View;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Auth;
using Microsoft.Azure.Storage.Blob;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Appts.Web.Ui.Scheduler.Controllers
{
  [Authorize]
  public class UserController : Controller
  {
    // GET: /<controller>/
    public IActionResult Settings()
    {
      return View();
    }

    public IActionResult Dashboard()
    {
      return View();
    }
    //public IActionResult UploadImage()
    //{
    //  return View();
    //}
    //[HttpPost("[controller]/[action]")]
    //[HttpPost]
    //public IActionResult UploadImage(IFormFile image)
    //{
    //  if (image == null)
    //  {
    //    // show message or do nothing?
    //    //image.Length?
    //  }
    //  string storageAccountName = "saappts01";
    //  string containerName = "thumbnail";
    //  var creds = new StorageCredentials(storageAccountName, "0QcjJUc676SXx8ypCNcMjUDsAbkT2MCYmQYi7/HHEbyjh/mUH3FlJdjd3FTHF5WiCrp/DOpp4cAO+toDwDiHYg==");
    //  CloudStorageAccount sa = new CloudStorageAccount(creds, true);
    //  CloudBlobClient blobClient = sa.CreateCloudBlobClient();
    //  CloudBlobContainer blobContainer = blobClient.GetContainerReference(containerName);
    //  //TODO: generate guid for file name here
    //  CloudBlockBlob blockBlob = blobContainer.GetBlockBlobReference(Path.GetFileName(image.FileName));
    //  using (Stream stream = image.OpenReadStream())
    //  {
    //    blockBlob.UploadFromStreamAsync(stream).GetAwaiter().GetResult();
    //  }

    //  return RedirectToAction("ImageGallery");
    //}
    //public IActionResult ImageGallery()
    //{
    //  var model = new UserImageGalleryViewModel()
    //  {
    //    Images = new List<UserImage>()
    //    {
    //      new UserImage() { Url = "https://images.pexels.com/photos/760710/pexels-photo-760710.jpeg" },
    //      new UserImage() { Url = "https://images.pexels.com/photos/273141/pexels-photo-273141.jpeg" },
    //      new UserImage() { Url = "https://images.pexels.com/photos/179911/pexels-photo-179911.jpeg" },
    //      new UserImage() { Url = "https://images.pexels.com/photos/1440901/pexels-photo-1440901.jpeg" },
    //      new UserImage() { Url = "https://images.pexels.com/photos/1459504/pexels-photo-1459504.jpeg" }
    //    }
    //  };
    //  return View(model);
    //}
  }
}
