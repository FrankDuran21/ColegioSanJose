using ColegioSanJose.Data;
using ColegioSanJose.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ColegioSanJose.Controllers
{
    public class CarreraController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CarreraController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string buscar)
        {
            ViewData["FiltroActual"] = buscar;

            var carreras = _context.Carreras.AsQueryable();

            if (!string.IsNullOrWhiteSpace(buscar))
            {
                buscar = buscar.Trim();

                carreras = carreras.Where(c =>
                    c.NombreCarrera.Contains(buscar)
                );
            }

            carreras = carreras.OrderBy(c => c.NombreCarrera);

            return View(await carreras.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carrera = await _context.Carreras
                .FirstOrDefaultAsync(c => c.CarreraId == id);

            if (carrera == null)
            {
                return NotFound();
            }

            return View(carrera);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CarreraId,NombreCarrera")] Carrera carrera)
        {
            if (ModelState.IsValid)
            {
                carrera.NombreCarrera = carrera.NombreCarrera.Trim();

                var existe = await _context.Carreras
                    .AnyAsync(c => c.NombreCarrera == carrera.NombreCarrera);

                if (existe)
                {
                    ModelState.AddModelError("NombreCarrera", "Esta carrera ya está registrada.");
                    return View(carrera);
                }

                _context.Add(carrera);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(carrera);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carrera = await _context.Carreras.FindAsync(id);

            if (carrera == null)
            {
                return NotFound();
            }

            return View(carrera);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CarreraId,NombreCarrera")] Carrera carrera)
        {
            if (id != carrera.CarreraId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                carrera.NombreCarrera = carrera.NombreCarrera.Trim();

                var existe = await _context.Carreras
                    .AnyAsync(c => c.CarreraId != carrera.CarreraId &&
                                   c.NombreCarrera == carrera.NombreCarrera);

                if (existe)
                {
                    ModelState.AddModelError("NombreCarrera", "Esta carrera ya está registrada.");
                    return View(carrera);
                }

                try
                {
                    _context.Update(carrera);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CarreraExists(carrera.CarreraId))
                    {
                        return NotFound();
                    }

                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(carrera);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carrera = await _context.Carreras
                .FirstOrDefaultAsync(c => c.CarreraId == id);

            if (carrera == null)
            {
                return NotFound();
            }

            var usadaEnAlumnos = await _context.Alumnos
                .AnyAsync(a => a.Grado == carrera.NombreCarrera);

            var usadaEnMaterias = await _context.Materias
                .AnyAsync(m => m.Grado == carrera.NombreCarrera);

            ViewBag.EstaEnUso = usadaEnAlumnos || usadaEnMaterias;

            return View(carrera);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var carrera = await _context.Carreras.FindAsync(id);

            if (carrera == null)
            {
                return NotFound();
            }

            var usadaEnAlumnos = await _context.Alumnos
                .AnyAsync(a => a.Grado == carrera.NombreCarrera);

            var usadaEnMaterias = await _context.Materias
                .AnyAsync(m => m.Grado == carrera.NombreCarrera);

            if (usadaEnAlumnos || usadaEnMaterias)
            {
                ModelState.AddModelError("", "No se puede eliminar esta carrera porque está asignada a alumnos o materias.");
                ViewBag.EstaEnUso = true;
                return View(carrera);
            }

            _context.Carreras.Remove(carrera);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool CarreraExists(int id)
        {
            return _context.Carreras.Any(e => e.CarreraId == id);
        }
    }
}