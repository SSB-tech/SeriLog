using Microsoft.AspNetCore.Mvc;
using Serilog;
using SeriLog.DB;
using SeriLog.Models;
using System.Diagnostics;

namespace SeriLog.Controllers
{
    public class HomeController : Controller
    {

        //Intentionally kept errors to see the logs also go to url/serilog-ui to view logs on UI
        //private readonly ApplicationDbContext _context;
        private readonly ILogger<HomeController> _logger;
        ApplicationDbContext _context;
      
    public HomeController(ILogger<HomeController> logger/*, ApplicationDbContext context*/)
        {
            _logger = logger;
          // _context = context;

        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(User user)
        {
                try
                {
                    int a=Convert.ToInt32 (user.Name);
                    _context.User.Add(user);
                    _context.SaveChanges();
                    return RedirectToAction("Index", "Home");
                }
                catch
                {
                    throw;
                }
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
    }
}
