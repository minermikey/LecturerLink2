using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LecturerLink.Controllers
{
    public class InformationController : Controller
    {
        private readonly IWebHostEnvironment _environment;

        public InformationController(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(IFormFile fileUpload)
        {
            if (fileUpload != null && fileUpload.Length > 0)
            {
                // Define the path where you want to save the uploaded file
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");

                // Ensure the folder exists
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Create the full file path
                var filePath = Path.Combine(uploadsFolder, fileUpload.FileName);

                // Save the file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await fileUpload.CopyToAsync(stream);
                }

                // Redirect to the Index action to view the files
                return RedirectToAction("Index");
            }

            // If no file was uploaded, return the Create view with an error message
            ModelState.AddModelError("File", "Please upload a valid file.");
            return View();
        }

        public IActionResult Index()
        {
            // Get the list of files in the uploads folder
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
            var files = Directory.GetFiles(uploadsFolder);

            // Extract just the file names from the full paths
            var fileNames = files.Select(f => Path.GetFileName(f)).ToList();

            return View(fileNames);
        }
    }
}
