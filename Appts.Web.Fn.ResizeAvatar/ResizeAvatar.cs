using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Azure.Storage.Blobs;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Newtonsoft.Json.Linq;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace Appts.Web.Fn.ResizeAvatar
{
  public static class ResizeAvatar
  {
    private static readonly string BLOB_STORAGE_CONNECTION_STRING = Environment.GetEnvironmentVariable("blobcdnstorage");
    private static bool _HasImageEncoder = false;
    private static string GetBlobNameFromUrl(string bloblUrl)
    {
      var uri = new Uri(bloblUrl);
      var blobClient = new BlobClient(uri);
      return blobClient.Name;
    }

    private static IImageEncoder GetEncoder(string extension)
    {
      IImageEncoder encoder = null;

      extension = extension.Replace(".", "");

      var isSupported = Regex.IsMatch(extension, "gif|png|jpe?g", RegexOptions.IgnoreCase);

      if (isSupported)
      {
        switch (extension.ToLower())
        {
          case "png":
            encoder = new PngEncoder();
            break;
          case "jpg":
            encoder = new JpegEncoder();
            break;
          case "jpeg":
            encoder = new JpegEncoder();
            break;
          case "gif":
            encoder = new GifEncoder();
            break;
          default:
            break;
        }
      }

      return encoder;
    }

    private static IImageEncoder IsImage(Stream stream)
    {
      stream.Seek(0, SeekOrigin.Begin);

      List<string> jpg = new List<string> { "FF", "D8" };
      List<string> bmp = new List<string> { "42", "4D" };
      List<string> gif = new List<string> { "47", "49", "46" };
      List<string> png = new List<string> { "89", "50", "4E", "47", "0D", "0A", "1A", "0A" };
      List<List<string>> imgTypes = new List<List<string>> { jpg, bmp, gif, png };

      List<string> bytesIterated = new List<string>();

      for (int i = 0; i < 8; i++)
      {
        string bit = stream.ReadByte().ToString("X2");
        bytesIterated.Add(bit);

        bool isImage = imgTypes.Any(img => !img.Except(bytesIterated).Any());
        if (isImage)
        {
          _HasImageEncoder = true;
          break;
        }
      }

      var set = new HashSet<string>(bytesIterated);
      if (set.SetEquals(jpg))
      {
        return new JpegEncoder();
      }
      if (set.SetEquals(gif))
      {
        return new GifEncoder();
      }
      if (set.SetEquals(png))
      {
        return new PngEncoder();
      }
      return new PngEncoder();
    }

    [FunctionName("ResizeAvatar")]
    public static async void Run(
        [EventGridTrigger] EventGridEvent eventGridEvent,
        [Blob("{data.url}", FileAccess.Read, Connection = "blobcdnstorage")] Stream input,
        ILogger log)
    {
      log.LogInformation($"Starting avatar resize: {eventGridEvent}");

      try
      {
        if (input != null)
        {
          var createdEvent = ((JObject)eventGridEvent.Data).ToObject<StorageBlobCreatedEventData>();
          var extension = Path.GetExtension(createdEvent.Url);
          var encoder = GetEncoder(extension);
          if (encoder == null)
          {

          }
          //if (encoder != null)
          //{
          var thumbnailWidth = Convert.ToInt32(Environment.GetEnvironmentVariable("THUMBNAIL_WIDTH"));
          var thumbContainerName = Environment.GetEnvironmentVariable("THUMBNAIL_CONTAINER_NAME");
          var blobServiceClient = new BlobServiceClient(BLOB_STORAGE_CONNECTION_STRING);
          var blobContainerClient = blobServiceClient.GetBlobContainerClient(thumbContainerName);
          //var blobName = GetBlobNameFromUrl(createdEvent.Url);

          var uri = new Uri(createdEvent.Url);
          var blobClient = new BlobClient(uri);
          var blobName = blobClient.Name;
          //log.LogError($"blobName: {blobName}, uri: {createdEvent.Url}");
          //var blobclient2 = blobContainerClient.GetBlobClient(blobName);
          var blobclient2 = blobContainerClient.GetBlobClient(blobName);
          IImageFormat iform;
          using (var output = new MemoryStream())
          using (Image<Rgba32> image = (Image<Rgba32>)Image.Load(input, out iform))
          {
            var divisor = image.Width / thumbnailWidth;
            var height = Convert.ToInt32(Math.Round((decimal)(image.Height / divisor)));

            image.Mutate(x => x.Resize(thumbnailWidth, height));

            //image.Save(output, encoder);
            image.Save(output, iform);
            output.Position = 0;
            try
            {
              //await blobContainerClient.UploadBlobAsync(blobName, output);
              await blobclient2.UploadAsync(output, true);
            }
            catch (Exception ex)
            {
              log.LogError("FAILED TO UPLOAD");
              log.LogError(ex.Message);
              //throw;
            }

          }
          //}
          //else
          //{
          //  log.LogInformation($"No encoder support for: {createdEvent.Url}");
          //}
        }
        else
        {
          log.LogError("input is null");
        }
      }
      catch (Exception ex)
      {
        log.LogInformation(ex.Message);
        throw;
      }


    }
  }
}
