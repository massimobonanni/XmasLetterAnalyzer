using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IO;
using XmasLetterAnalyzer.Core.Interfaces;
using XmasLetterAnalyzer.Web.Models.UploadController;

namespace XmasLetterAnalyzer.Web.Controllers
{
    public class AnalyzeLetterController : Controller
    {
        private readonly ILetterAnalyzer letterAnalyzer;
        public AnalyzeLetterController(ILetterAnalyzer letterAnalyzer)
        {
            this.letterAnalyzer = letterAnalyzer;
        }

        [HttpGet]
        public IActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            try
            {
                if (file.Length > 0)
                {
                    UploadViewModel model;
                    using (var stream = new MemoryStream())
                    {
                        await file.CopyToAsync(stream);
                        stream.Position = 0;
                        var serviceResponse = await letterAnalyzer.AnalyzeAsync(stream);
                        model = new UploadViewModel(serviceResponse);
                        model.ImageData = stream.ToArray();
                    }
                    return View("Analyzed",model);
                }
            }
            catch
            {
                ViewBag.Message = "File upload failed!!";
            }
            return View();
        }
    }
}
