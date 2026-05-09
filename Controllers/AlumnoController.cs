using ColegioSanJose.Data;
using ColegioSanJose.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ColegioSanJose.Controllers
{
    public class AlumnoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AlumnoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Alumno
        public async Task<IActionResult> Index(string buscar, string grado, string orden = "az")
        {
            ViewData["FiltroActual"] = buscar;
            ViewData["GradoActual"] = grado;
            ViewData["OrdenActual"] = orden;

            ViewBag.Grados = await _context.Alumnos
                .Select(a => a.Grado)
                .Distinct()
                .OrderBy(g => g)
                .ToListAsync();

            var alumnos = _context.Alumnos.AsQueryable();

            if (!string.IsNullOrWhiteSpace(buscar))
            {
                buscar = buscar.Trim();

                alumnos = alumnos.Where(a =>
                    a.Nombre.Contains(buscar) ||
                    a.Apellido.Contains(buscar)
                );
            }

            if (!string.IsNullOrWhiteSpace(grado))
            {
                alumnos = alumnos.Where(a => a.Grado == grado);
            }

            alumnos = orden switch
            {
                "za" => alumnos.OrderByDescending(a => a.Nombre).ThenByDescending(a => a.Apellido),
                "grado" => alumnos.OrderBy(a => a.Grado).ThenBy(a => a.Nombre).ThenBy(a => a.Apellido),
                _ => alumnos.OrderBy(a => a.Nombre).ThenBy(a => a.Apellido)
            };

            return View(await alumnos.ToListAsync());
        }

        // GET: Alumno/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var alumno = await _context.Alumnos
                .FirstOrDefaultAsync(m => m.AlumnoId == id);

            if (alumno == null)
            {
                return NotFound();
            }

            return View(alumno);
        }

        // GET: Alumno/Create
        public async Task<IActionResult> Create()
        {
            await CargarCarreras();
            return View();
        }

        // POST: Alumno/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AlumnoId,Nombre,Apellido,FechaNacimiento,Grado")] Alumno alumno)
        {
            if (ModelState.IsValid)
            {
                _context.Add(alumno);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            await CargarCarreras(alumno.Grado);
            return View(alumno);
        }

        // GET: Alumno/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var alumno = await _context.Alumnos.FindAsync(id);

            if (alumno == null)
            {
                return NotFound();
            }

            await CargarCarreras(alumno.Grado);
            return View(alumno);
        }

        // POST: Alumno/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AlumnoId,Nombre,Apellido,FechaNacimiento,Grado")] Alumno alumno)
        {
            if (id != alumno.AlumnoId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(alumno);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AlumnoExists(alumno.AlumnoId))
                    {
                        return NotFound();
                    }

                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            await CargarCarreras(alumno.Grado);
            return View(alumno);
        }

        // GET: Alumno/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var alumno = await _context.Alumnos
                .FirstOrDefaultAsync(m => m.AlumnoId == id);

            if (alumno == null)
            {
                return NotFound();
            }

            return View(alumno);
        }

        // POST: Alumno/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var alumno = await _context.Alumnos.FindAsync(id);

            if (alumno != null)
            {
                _context.Alumnos.Remove(alumno);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AlumnoExists(int id)
        {
            return _context.Alumnos.Any(e => e.AlumnoId == id);
        }

        private async Task CargarCarreras(string? carreraSeleccionada = null)
        {
            var carreras = await _context.Carreras
                .Select(c => c.NombreCarrera)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();

            if (!string.IsNullOrWhiteSpace(carreraSeleccionada) && !carreras.Contains(carreraSeleccionada))
            {
                carreras.Insert(0, carreraSeleccionada);
            }

            ViewBag.Carreras = new SelectList(carreras, carreraSeleccionada);
        }
    }
}