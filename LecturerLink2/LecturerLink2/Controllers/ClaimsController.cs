using LecturerLink2.Data;
using LecturerLink2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LecturerLink2.Controllers
{
    public class ClaimsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;


        // Injecting the database context into the controller
        public ClaimsController(IWebHostEnvironment environment, ApplicationDbContext context)
        {
            _context = context;
            _environment = environment;

        }

        // GET: Claims/Create
        [Authorize(Roles = "LECTURER,Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Claims/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Claims claim, IFormFile fileUpload)
        {
            if (ModelState.IsValid)
            {
                // Handle file upload
                if (fileUpload != null && fileUpload.Length > 0)
                {
                    // Define the path where you want to save the uploaded file
                    var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");

                    // Ensure the "uploads" folder exists
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    // Create a unique file name to avoid overwriting
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + fileUpload.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    // Save the file
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await fileUpload.CopyToAsync(stream);
                    }

                    // Optional: Save the file path to the database
                    claim.FilePath = "/uploads/" + uniqueFileName;
                }

                // Calculate TotalAmount based on HoursWorked and RatePerHour
                claim.TotalAmount = (claim.HouresWorked ?? 0) * claim.RatePerHour;

                // Set the PaymentStatus to default, e.g., Pending
                claim.PaymentStatus = "Pending";

                // Add the new claim to the database
                _context.Claims.Add(claim);
                await _context.SaveChangesAsync();

                // Redirect to a different page (e.g., the list of claims)
                return RedirectToAction(nameof(Index));
            }

            // If model validation fails, return the same view with the validation errors
            return View(claim);
        }



        // GET: Claims
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            // Return all claims from the database
            return View(await _context.Claims.ToListAsync());
        }

        // GET: Claims/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var claim = await _context.Claims.FindAsync(id);
            if (claim == null)
            {
                return NotFound();
            }

            return View(claim);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, Claims claim, IFormFile fileUpload)
        {
            if (id != claim.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Handle file upload
                    if (fileUpload != null && fileUpload.Length > 0)
                    {
                        // Define the path where the uploaded file will be saved
                        var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");

                        // Ensure the "uploads" folder exists
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        // Create a unique file name to avoid overwriting
                        var uniqueFileName = Guid.NewGuid().ToString() + "_" + fileUpload.FileName;
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        // Save the file
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await fileUpload.CopyToAsync(stream);
                        }

                        // Optional: If you want to save the file path in the database
                        claim.FilePath = "/uploads/" + uniqueFileName;
                    }

                    // Calculate TotalAmount based on HoursWorked and RatePerHour
                    claim.TotalAmount = (claim.HouresWorked ?? 0) * claim.RatePerHour;

                    // Update the claim in the database
                    _context.Update(claim);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClaimExists(claim.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            return View(claim);
        }

        private bool ClaimExists(int id)
        {
            return _context.Claims.Any(e => e.ID == id);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var claim = await _context.Claims.FindAsync(id);
            if (claim != null)
            {
                // Optionally, delete the file from the server if necessary
                if (!string.IsNullOrEmpty(claim.FilePath))
                {
                    var filePath = Path.Combine(_environment.WebRootPath, claim.FilePath);
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                _context.Claims.Remove(claim);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ConfirmPayment(int id)
        {
            var information = await _context.Claims.FindAsync(id);
            if (information != null)
            {
                information.PaymentStatus = "Confirmed";
                _context.Update(information);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: Deny Payment
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DenyPayment(int id)
        {
            var information = await _context.Claims.FindAsync(id);
            if (information != null)
            {
                information.PaymentStatus = "Denied";
                _context.Update(information);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
