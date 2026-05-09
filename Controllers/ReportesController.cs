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

        public async Task<IActionResult> Graficas(string grado)
        {
            ViewData["GradoActual"] = grado;

            ViewBag.Grados = await _context.Alumnos
                .Select(a => a.Grado)
                .Distinct()
                .OrderBy(g => g)
                .ToListAsync();

            var alumnosConsulta = _context.Alumnos.AsQueryable();
            var materiasConsulta = _context.Materias.AsQueryable();
            var expedientesConsulta = _context.Expedientes
                .Include(e => e.Alumno)
                .Include(e => e.Materia)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(grado))
            {
                alumnosConsulta = alumnosConsulta.Where(a => a.Grado == grado);
                materiasConsulta = materiasConsulta.Where(m => m.Grado == grado);
                expedientesConsulta = expedientesConsulta.Where(e => e.Alumno!.Grado == grado);
            }

            ViewBag.TotalAlumnos = await alumnosConsulta.CountAsync();
            ViewBag.TotalMaterias = await materiasConsulta.CountAsync();
            ViewBag.TotalExpedientes = await expedientesConsulta.CountAsync();

            ViewBag.PromedioGeneral = await expedientesConsulta.AnyAsync()
                ? await expedientesConsulta.AverageAsync(e => e.NotaFinal)
                : 0;

            var promedioPorCarrera = await expedientesConsulta
                .GroupBy(e => e.Alumno!.Grado)
                .Select(g => new
                {
                    Carrera = g.Key,
                    Promedio = g.Average(e => e.NotaFinal)
                })
                .OrderBy(x => x.Carrera)
                .ToListAsync();

            var promedioPorMateria = await expedientesConsulta
                .GroupBy(e => e.Materia!.NombreMateria)
                .Select(g => new
                {
                    Materia = g.Key,
                    Promedio = g.Average(e => e.NotaFinal)
                })
                .OrderByDescending(x => x.Promedio)
                .ToListAsync();

            var aprobados = await expedientesConsulta.CountAsync(e => e.NotaFinal >= 6);
            var reprobados = await expedientesConsulta.CountAsync(e => e.NotaFinal < 6);

            var cantidadPorCarrera = await expedientesConsulta
                .GroupBy(e => e.Alumno!.Grado)
                .Select(g => new
                {
                    Carrera = g.Key,
                    Cantidad = g.Count()
                })
                .OrderBy(x => x.Carrera)
                .ToListAsync();

            ViewBag.CarrerasLabels = promedioPorCarrera.Select(x => x.Carrera).ToList();
            ViewBag.CarrerasPromedios = promedioPorCarrera.Select(x => x.Promedio).ToList();

            ViewBag.MateriasLabels = promedioPorMateria.Select(x => x.Materia).ToList();
            ViewBag.MateriasPromedios = promedioPorMateria.Select(x => x.Promedio).ToList();

            ViewBag.EstadoLabels = new List<string> { "Aprobados", "Reprobados" };
            ViewBag.EstadoCantidad = new List<int> { aprobados, reprobados };

            ViewBag.CantidadCarreraLabels = cantidadPorCarrera.Select(x => x.Carrera).ToList();
            ViewBag.CantidadCarreraDatos = cantidadPorCarrera.Select(x => x.Cantidad).ToList();

            return View();
        }
    }
}