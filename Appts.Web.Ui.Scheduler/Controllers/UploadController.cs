using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Appts.Web.Ui.Scheduler.Repositories;
using System.IO;
using Microsoft.AspNetCore.Http;
using Appts.Models.View;
using Appts.Common.Constants;
using Microsoft.ApplicationInsights;
namespace Appts.Web.Ui.Scheduler.Controllers
{
  public class UploadController : Controller
  {
    private readonly IBlobAvatarRepository _blobAvatarRepository;
    private TelemetryClient _telemetry;
    public UploadController(IBlobAvatarRepository blobAvatarRepository, TelemetryClient telemetry)
    {
      _blobAvatarRepository = blobAvatarRepository;
      _telemetry = telemetry;
    }
    public IActionResult Avatar()
    {
      _telemetry.TrackPageView("UploadAvatar");
      string userId = User.Claims.First(c => c.Type == IdentityK.NameIdentifier).Value;
      var vm = new UploadAvatarViewModel() { UserId = userId };
      return View(vm);
    }
    // POST /api/images/upload
    [HttpPost("[controller]/[action]")]
    public IActionResult UploadAvatar(ICollection<IFormFile> files)
    {
      _telemetry.TrackEvent("UploadAvatarRequested");
      bool isUploaded = false;
      try
      {
        if (files.Count == 0)
          return BadRequest("No files received from the upload");
        string userId = User.Claims.First(c => c.Type == IdentityK.NameIdentifier).Value;
        foreach (var formFile in files)
        {
          if (_blobAvatarRepository.IsImage(formFile))
          {
            if (formFile.Length > 0)
            {
              using (Stream stream = formFile.OpenReadStream())
              {
                //isUploaded = await _blobAvatarRepository.UploadFileToStorageAsync(stream, userId);
                isUploaded = _blobAvatarRepository.UploadFileToStorageAsync(stream, userId).GetAwaiter().GetResult();
              }
            }
          }
          else
          {
            return new UnsupportedMediaTypeResult();
          }
        }
        if (isUploaded)
        {
          return Json(new { result = "Redirect", url = Url.Action("Avatar", "Upload") });
        }
        else
          return BadRequest("Look like the image couldnt upload to the storage");
      }
      catch (Exception ex)
      {
        return BadRequest(ex.Message);
      }
    }
  }
}