using AIApplication.Data;
using AIApplication.Models;
using AIApplication.Service;
using Microsoft.AspNetCore.Mvc;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;

namespace AIApplication.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly OllamaService _ollama;
        private readonly SqlService _sqlservice;


        public HomeController(ILogger<HomeController> logger, OllamaService ollama, SqlService sqlService)
        {
            _logger = logger;
            _ollama = ollama;
            _sqlservice = sqlService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        [Obsolete]
        public async Task<IActionResult> Generate(string text, string mode)
        {
            try
            {
                if (mode == "embedding")
                {
                    var vector = await _ollama.GetEmbedding(text);

                    if (vector != null)
                    {
                        await _sqlservice.Save(text, vector);
                        ViewBag.Result = "Stored!\n\n" + string.Join(",", vector);
                    }
                    else
                    {
                        ViewBag.Result = "Error generating embedding";
                    }
                }
                else if (mode == "input")
                {
                    var response = await _ollama.GenerateResponse(text);
                    ViewBag.Title = "Result";
                    ViewBag.Result = response;
                }

                return View("Index");
            }
            catch (Exception ex) {
                ViewBag.Title = "Error Happened";
                Debug.WriteLine(ex.Message);
                return View("Index");
            }
        }
    }
}
