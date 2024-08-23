using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace YourNamespace.Controllers
{
    public class UploadController : Controller
    {
        [HttpPost]
        public async Task<ActionResult> UploadFile(HttpPostedFileBase file)
        {
            if (Request.Files.Count > 0)
            {
                var file_ = Request.Files[0];
                if (file_ != null && file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var filePath = Path.Combine(Server.MapPath("~/UploadFiles/"), fileName);
                    file.SaveAs(filePath);

                    var result = await UploadFileToApi(filePath);
                    ViewBag.Message = result ? "File uploaded successfully." : "File upload failed.";
                }
            }
            return View();
        }

        private async Task<bool> UploadFileToApi(string filePath)
        {

            //File.

            // Read the Excel file into a byte array
            // byte[] fileBytes = File.ReadAllBytes(filePath);

            var readFile = System.IO.File.ReadAllBytes(filePath);

            using (var client = new HttpClient())
            {
                using (var form = new MultipartFormDataContent())
                {




                    var fileContent = new ByteArrayContent(System.IO.File.ReadAllBytes(filePath));

                    // Set the content type to the appropriate MIME type for Excel files
                    fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                    fileContent.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("form-data")
                    {
                        Name = "file",
                        FileName = Path.GetFileName(filePath)
                    };
                    form.Add(fileContent);

                    var response = await client.PostAsync("http://192.168.0.208:8000/api/generate_report", form);
                    if (response.IsSuccessStatusCode)
                    {
                        // Read and print the response content if needed
                        var content = await response.Content.ReadAsStringAsync();                        
                    }


                    return response.IsSuccessStatusCode;
                }
            }
        }

        [HttpPost]
        // [Route("api/upload_excel")]
        public ActionResult UploadExcel(HttpPostedFileBase file)
        {
            if (file == null || file.ContentLength == 0)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest, "No file uploaded.");
            }

            try
            {
                var fileName = Path.GetFileName(file.FileName);
                var filePath = Path.Combine(Server.MapPath("~/UploadFiles/"), fileName);
                file.SaveAs(filePath);

                // Process the file as needed
                // ...

                //DownloadExcel(fileName);


                return Json(new { message = "File uploaded successfully." });
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.InternalServerError, $"Error: {ex.Message}");
            }
        }

        //[HttpPost]
        ////[Route("api/upload_excel")]
        //public ActionResult UploadExcel(HttpPostedFileBase file)
        //{
        //    if (file == null || file.ContentLength == 0)
        //    {
        //        return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest, "No file uploaded.");
        //    }

        //    try
        //    {
        //        var fileName = Path.GetFileName(file.FileName);
        //        var filePath = Path.Combine(Server.MapPath("~/Uploads/"), fileName);
        //        file.SaveAs(filePath);

        //        ViewBag.Message = "File uploaded successfully.";
        //        ViewBag.FileName = fileName;

        //        return View("ApiUploadExcelFile");
        //    }
        //    catch (Exception ex)
        //    {
        //        return new HttpStatusCodeResult(System.Net.HttpStatusCode.InternalServerError, $"Error: {ex.Message}");
        //    }
        //}

        [HttpGet]
       // [Route("api/download_excel")]
        public ActionResult DownloadExcel(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest, "No file name provided.");
            }

            var filePath = Path.Combine(Server.MapPath("~/UploadFiles/"), fileName);
            if (!System.IO.File.Exists(filePath))
            {
                return HttpNotFound("File not found.");
            }

            return File(filePath, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        [HttpGet]
        public JsonResult Test() 
        {
            
            return Json(new { status = 1, message = "Ali Sample" });
        
        }

    }
}