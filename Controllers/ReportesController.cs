using ColegioSanJose.Data;
using ColegioSanJose.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ColegioSanJose.Controllers
{
    public class ReportesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Promedios()
        {
            var promedios = await _context.Expedientes
                .Include(e => e.Alumno)
                .GroupBy(e => new
                {
                    e.AlumnoId,
                    e.Alumno!.Nombre,
                    e.Alumno.Apellido,
                    e.Alumno.Grado
                })
                .Select(g => new PromedioAlumnoViewModel
                {
                    AlumnoId = g.Key.AlumnoId,
                    NombreCompleto = g.Key.Nombre + " " + g.Key.Apellido,
                    Grado = g.Key.Grado,
                    PromedioNotas = g.Average(e => e.NotaFinal),
                    CantidadMaterias = g.Count()
                })
                .OrderBy(p => p.NombreCompleto)
                .ToListAsync();

            return View(promedios);
        }
    }
}