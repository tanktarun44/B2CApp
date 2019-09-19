using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;


namespace B2CApp.Controllers
{
    public class SourceFile
    {
        public string Name { get; set; }
        public string Extension { get; set; }
        public Byte[] FileBytes { get; set; }
    }
    
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        public IActionResult Get()
        {
            List<string> fileName = new List<string>();
            fileName.Add("00937dc9a02547cd95e17bf7f992f957.txt");
            fileName.Add("025ce8e69d3640c498a653a43a0428dc.jpg");

            List<SourceFile> sourceFiles = new List<SourceFile>();

            foreach (var fname in fileName)
            {
                string[] fdata = fname.Split(".");
                CloudBlockBlob blockBlob = GetBlockBlobDetail(fname);
                Stream blobStream = blockBlob.OpenReadAsync().GetAwaiter().GetResult();
                sourceFiles.Add(new SourceFile() { Name = fdata[0], Extension = fdata[1], FileBytes = ReadFully(blobStream) });
            }
            // get the source files

            // ...

            // the output bytes of the zip
            byte[] fileBytes = null;

            // create a working memory stream
            using (System.IO.MemoryStream memoryStream = new System.IO.MemoryStream())
            {
                // create a zip
                using (System.IO.Compression.ZipArchive zip = new System.IO.Compression.ZipArchive(memoryStream, System.IO.Compression.ZipArchiveMode.Create, true))
                {
                    // interate through the source files
                    foreach (SourceFile f in sourceFiles)
                    {
                        // add the item name to the zip
                        System.IO.Compression.ZipArchiveEntry zipItem = zip.CreateEntry(f.Name + "." + f.Extension);
                        // add the item bytes to the zip entry by opening the original file and copying the bytes 
                        using (System.IO.MemoryStream originalFileMemoryStream = new System.IO.MemoryStream(f.FileBytes))
                        {
                            using (System.IO.Stream entryStream = zipItem.Open())
                            {
                                originalFileMemoryStream.CopyTo(entryStream);
                            }
                        }
                    }
                }
                fileBytes = memoryStream.ToArray();
            }

            // download the constructed zip
            Response.Headers.Add("Content-Disposition", "attachment; filename=download.zip");
            return File(fileBytes, "application/zip");
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        private CloudBlockBlob GetBlockBlobDetail(string blobFileName)
        {
            try
            {
                CloudBlockBlob blockblob = null;
                var ConnectionString = "DefaultEndpointsProtocol=https;AccountName=ddopcoprogram;AccountKey=fTyjkmCt2hlcCn6lA+dx6bJsgeqP1YWbWS6QOxnB6Iotjs8X7xbmEKqyJzkjzdoUD0wQcfbj4dLkP57/e+SQCw==;EndpointSuffix=core.windows.net";
                if (CloudStorageAccount.TryParse(ConnectionString, out CloudStorageAccount storageAccount))
                {
                    var blobClient = storageAccount.CreateCloudBlobClient();
                    CloudBlobContainer cloudBlobContainer = blobClient.GetContainerReference("blob-api");
                    cloudBlobContainer.CreateIfNotExistsAsync();

                    blockblob = cloudBlobContainer.GetBlockBlobReference(blobFileName);

                    return blockblob;
                }
                else
                {
                    return blockblob;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private byte[] ReadFully(Stream input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }
    }
}
