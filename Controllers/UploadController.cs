using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;

namespace crms2.Controllers
{
    public class UploadController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            // Return the view using the full path
            return View("~/Views/Home/Upload.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                ViewBag.Message = "Please select a valid CSV file.";
                return View("~/Views/Home/Upload.cshtml");
            }

            // Ensure the uploads directory exists
            var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
            if (!Directory.Exists(uploadsDir))
            {
                Directory.CreateDirectory(uploadsDir);
            }

            // Save the uploaded file
            var filePath = Path.Combine(uploadsDir, file.FileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            ViewBag.Message = "File uploaded successfully!";
            return View("~/Views/Home/Upload.cshtml");
        }
    }
}
