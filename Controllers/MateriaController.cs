using ColegioSanJose.Data;
using ColegioSanJose.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ColegioSanJose.Controllers
{
    public class MateriaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MateriaController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Materia
        public async Task<IActionResult> Index(string buscar, string grado, string docente, string orden = "materia_az")
        {
            ViewData["FiltroActual"] = buscar;
            ViewData["GradoActual"] = grado;
            ViewData["DocenteActual"] = docente;
            ViewData["OrdenActual"] = orden;

            var carrerasCatalogo = await _context.Carreras
                .Select(c => c.NombreCarrera)
                .ToListAsync();

            var gradosMaterias = await _context.Materias
                .Select(m => m.Grado)
                .ToListAsync();

            ViewBag.Grados = carrerasCatalogo
                .Union(gradosMaterias)
                .Where(g => !string.IsNullOrWhiteSpace(g))
                .Distinct()
                .OrderBy(g => g)
                .ToList();

            ViewBag.Docentes = await _context.Materias
                .Select(m => m.Docente)
                .Where(d => !string.IsNullOrWhiteSpace(d))
                .Distinct()
                .OrderBy(d => d)
                .ToListAsync();

            var materias = _context.Materias.AsQueryable();

            if (!string.IsNullOrWhiteSpace(buscar))
            {
                buscar = buscar.Trim();

                materias = materias.Where(m =>
                    m.NombreMateria.Contains(buscar) ||
                    m.Docente.Contains(buscar) ||
                    m.Grado.Contains(buscar)
                );
            }

            if (!string.IsNullOrWhiteSpace(grado))
            {
                materias = materias.Where(m => m.Grado == grado);
            }

            if (!string.IsNullOrWhiteSpace(docente))
            {
                materias = materias.Where(m => m.Docente == docente);
            }

            materias = orden switch
            {
                "materia_za" => materias.OrderByDescending(m => m.NombreMateria),
                "grado" => materias.OrderBy(m => m.Grado).ThenBy(m => m.NombreMateria),
                "docente_az" => materias.OrderBy(m => m.Docente).ThenBy(m => m.NombreMateria),
                "docente_za" => materias.OrderByDescending(m => m.Docente).ThenBy(m => m.NombreMateria),
                _ => materias.OrderBy(m => m.NombreMateria)
            };

            return View(await materias.ToListAsync());
        }

        // GET: Materia/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var materia = await _context.Materias
                .FirstOrDefaultAsync(m => m.MateriaId == id);

            if (materia == null)
            {
                return NotFound();
            }

            return View(materia);
        }

        // GET: Materia/Create
        public async Task<IActionResult> Create()
        {
            await CargarCarreras();
            return View();
        }

        // POST: Materia/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MateriaId,NombreMateria,Docente,Grado")] Materia materia)
        {
            if (ModelState.IsValid)
            {
                _context.Add(materia);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            await CargarCarreras(materia.Grado);
            return View(materia);
        }

        // GET: Materia/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var materia = await _context.Materias.FindAsync(id);

            if (materia == null)
            {
                return NotFound();
            }

            await CargarCarreras(materia.Grado);
            return View(materia);
        }

        // POST: Materia/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MateriaId,NombreMateria,Docente,Grado")] Materia materia)
        {
            if (id != materia.MateriaId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(materia);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MateriaExists(materia.MateriaId))
                    {
                        return NotFound();
                    }

                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            await CargarCarreras(materia.Grado);
            return View(materia);
        }

        // GET: Materia/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var materia = await _context.Materias
                .FirstOrDefaultAsync(m => m.MateriaId == id);

            if (materia == null)
            {
                return NotFound();
            }

            return View(materia);
        }

        // POST: Materia/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var materia = await _context.Materias.FindAsync(id);

            if (materia != null)
            {
                _context.Materias.Remove(materia);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MateriaExists(int id)
        {
            return _context.Materias.Any(e => e.MateriaId == id);
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