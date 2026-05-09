using ColegioSanJose.Data;
using ColegioSanJose.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace ColegioSanJose.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.TotalAlumnos = await _context.Alumnos.CountAsync();
            ViewBag.TotalMaterias = await _context.Materias.CountAsync();
            ViewBag.TotalExpedientes = await _context.Expedientes.CountAsync();

            decimal promedioGeneral = 0;

            if (await _context.Expedientes.AnyAsync())
            {
                promedioGeneral = await _context.Expedientes.AverageAsync(e => e.NotaFinal);
            }

            ViewBag.PromedioGeneral = promedioGeneral;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}