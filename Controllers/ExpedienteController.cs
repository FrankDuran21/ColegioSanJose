using ColegioSanJose.Data;
using ColegioSanJose.Models;
using ColegioSanJose.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ColegioSanJose.Controllers
{
    public class ExpedienteController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ExpedienteController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Expediente
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

            var alumnosConsulta = _context.Alumnos
                .Include(a => a.Expedientes)
                    .ThenInclude(e => e.Materia)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(buscar))
            {
                buscar = buscar.Trim();

                alumnosConsulta = alumnosConsulta.Where(a =>
                    a.Nombre.Contains(buscar) ||
                    a.Apellido.Contains(buscar) ||
                    a.Grado.Contains(buscar) ||
                    a.Expedientes.Any(e =>
                        e.Materia!.NombreMateria.Contains(buscar) ||
                        e.Materia.Docente.Contains(buscar) ||
                        (e.Observaciones != null && e.Observaciones.Contains(buscar))
                    )
                );
            }

            if (!string.IsNullOrWhiteSpace(grado))
            {
                alumnosConsulta = alumnosConsulta.Where(a => a.Grado == grado);
            }

            var alumnos = await alumnosConsulta.ToListAsync();

            var listado = alumnos.Select(a => new ExpedienteAlumnoViewModel
            {
                AlumnoId = a.AlumnoId,
                NombreCompleto = a.Nombre + " " + a.Apellido,
                Grado = a.Grado,
                CantidadMaterias = a.Expedientes.Count,
                Promedio = a.Expedientes.Any() ? a.Expedientes.Average(e => e.NotaFinal) : 0,

                Expedientes = a.Expedientes
                    .OrderBy(e => e.Materia!.NombreMateria)
                    .Select(e => new ExpedienteDetalleViewModel
                    {
                        ExpedienteId = e.ExpedienteId,
                        Materia = e.Materia != null ? e.Materia.NombreMateria : "",
                        Docente = e.Materia != null ? e.Materia.Docente : "",
                        NotaFinal = e.NotaFinal,
                        Observaciones = e.Observaciones
                    })
                    .ToList()
            }).ToList();

            listado = orden switch
            {
                "za" => listado.OrderByDescending(a => a.NombreCompleto).ToList(),
                "grado" => listado.OrderBy(a => a.Grado).ThenBy(a => a.NombreCompleto).ToList(),
                "promedio_desc" => listado.OrderByDescending(a => a.Promedio).ToList(),
                "promedio_asc" => listado.OrderBy(a => a.Promedio).ToList(),
                _ => listado.OrderBy(a => a.NombreCompleto).ToList()
            };

            return View(listado);
        }

        // GET: Expediente/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var expediente = await _context.Expedientes
                .Include(e => e.Alumno)
                .Include(e => e.Materia)
                .FirstOrDefaultAsync(m => m.ExpedienteId == id);

            if (expediente == null)
            {
                return NotFound();
            }

            return View(expediente);
        }

        // GET: Expediente/Create
        public async Task<IActionResult> Create()
        {
            await CargarListasExpediente();
            return View();
        }

        // POST: Expediente/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ExpedienteId,AlumnoId,MateriaId,NotaFinal,Observaciones")] Expediente expediente)
        {
            var alumno = await _context.Alumnos.FindAsync(expediente.AlumnoId);
            var materia = await _context.Materias.FindAsync(expediente.MateriaId);

            if (alumno == null)
            {
                ModelState.AddModelError("AlumnoId", "Debe seleccionar un alumno válido.");
            }

            if (materia == null)
            {
                ModelState.AddModelError("MateriaId", "Debe seleccionar una materia válida.");
            }

            if (alumno != null && materia != null && alumno.Grado != materia.Grado)
            {
                ModelState.AddModelError("MateriaId", "La materia seleccionada no corresponde al grado del alumno.");
            }

            var yaExiste = await _context.Expedientes.AnyAsync(e =>
                e.AlumnoId == expediente.AlumnoId &&
                e.MateriaId == expediente.MateriaId
            );

            if (yaExiste)
            {
                ModelState.AddModelError("MateriaId", "Este alumno ya tiene registrada esta materia en su expediente.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(expediente);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            await CargarListasExpediente(expediente.AlumnoId, expediente.MateriaId);
            return View(expediente);
        }

        // GET: Expediente/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var expediente = await _context.Expedientes.FindAsync(id);

            if (expediente == null)
            {
                return NotFound();
            }

            await CargarListasExpediente(expediente.AlumnoId, expediente.MateriaId);
            return View(expediente);
        }

        // POST: Expediente/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ExpedienteId,AlumnoId,MateriaId,NotaFinal,Observaciones")] Expediente expediente)
        {
            if (id != expediente.ExpedienteId)
            {
                return NotFound();
            }

            var alumno = await _context.Alumnos.FindAsync(expediente.AlumnoId);
            var materia = await _context.Materias.FindAsync(expediente.MateriaId);

            if (alumno == null)
            {
                ModelState.AddModelError("AlumnoId", "Debe seleccionar un alumno válido.");
            }

            if (materia == null)
            {
                ModelState.AddModelError("MateriaId", "Debe seleccionar una materia válida.");
            }

            if (alumno != null && materia != null && alumno.Grado != materia.Grado)
            {
                ModelState.AddModelError("MateriaId", "La materia seleccionada no corresponde al grado del alumno.");
            }

            var yaExiste = await _context.Expedientes.AnyAsync(e =>
                e.ExpedienteId != expediente.ExpedienteId &&
                e.AlumnoId == expediente.AlumnoId &&
                e.MateriaId == expediente.MateriaId
            );

            if (yaExiste)
            {
                ModelState.AddModelError("MateriaId", "Este alumno ya tiene registrada esta materia en su expediente.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(expediente);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExpedienteExists(expediente.ExpedienteId))
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

            await CargarListasExpediente(expediente.AlumnoId, expediente.MateriaId);
            return View(expediente);
        }

        // GET: Expediente/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var expediente = await _context.Expedientes
                .Include(e => e.Alumno)
                .Include(e => e.Materia)
                .FirstOrDefaultAsync(m => m.ExpedienteId == id);

            if (expediente == null)
            {
                return NotFound();
            }

            return View(expediente);
        }

        // POST: Expediente/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var expediente = await _context.Expedientes.FindAsync(id);

            if (expediente != null)
            {
                _context.Expedientes.Remove(expediente);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ExpedienteExists(int id)
        {
            return _context.Expedientes.Any(e => e.ExpedienteId == id);
        }

        private async Task CargarListasExpediente(int? alumnoSeleccionado = null, int? materiaSeleccionada = null)
        {
            var alumnos = await _context.Alumnos
                .OrderBy(a => a.Nombre)
                .ThenBy(a => a.Apellido)
                .ToListAsync();

            var materias = await _context.Materias
                .OrderBy(m => m.Grado)
                .ThenBy(m => m.NombreMateria)
                .ToListAsync();

            ViewBag.Alumnos = alumnos;
            ViewBag.Materias = materias;

            ViewData["AlumnoId"] = new SelectList(
                alumnos.Select(a => new
                {
                    a.AlumnoId,
                    Texto = a.Nombre + " " + a.Apellido + " — " + a.Grado
                }),
                "AlumnoId",
                "Texto",
                alumnoSeleccionado
            );

            ViewData["MateriaId"] = new SelectList(
                materias.Select(m => new
                {
                    m.MateriaId,
                    Texto = m.NombreMateria + " — " + m.Docente + " — " + m.Grado
                }),
                "MateriaId",
                "Texto",
                materiaSeleccionada
            );
        }
    }
}