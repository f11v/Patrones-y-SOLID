using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using C.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;
using C.Services;

namespace C.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly UsuarioService _usuarioService;
        private readonly CContext _context;

        public UsuariosController(CContext context, UsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _usuarioService.GetAllUsuariosAsync());
        }

        // Acción de inicio de sesión
        public async Task<IActionResult> Login(string Correo, string Contraseña, int RolId)
        {
            var usuario = UsuarioFactory.CrearUsuario(Correo, Contraseña, RolId);

            var usuarioExistente = await _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.Correo == usuario.Correo && u.Contraseña == usuario.Contraseña && u.RolId == usuario.RolId);

            if (usuarioExistente == null)
            {
                ViewBag.ErrorMessage = "Correo, contraseña o rol incorrectos.";
                ViewBag.Roles = new SelectList(_context.Roles, "RolId", "NombreRol");
                return View();
            }

            HttpContext.Session.SetString("IsLoggedIn", "true");
            HttpContext.Session.SetString("UserRole", usuarioExistente.Rol.NombreRol); // Guardar el rol en la sesión

            // Redireccionar según el rol
            switch (usuarioExistente.Rol.NombreRol)
            {
                case "Administrador":
                    return RedirectToAction("Index", "Calificaciones"); // Asegúrate de que el nombre del controlador y la acción sean correctos
                case "Maestro":
                    return RedirectToAction("Index", "Usuarios");
                case "Estudiante":
                    return RedirectToAction("Index", "Usuarios");
                default:
                    return RedirectToAction("Index", "Home");
            }
        }

        // GET: Usuarios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(m => m.UsuarioId == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // GET: Usuarios/Create
        public IActionResult Create()
        {
            ViewData["RolId"] = new SelectList(_context.Roles, "RolId", "NombreRol");
            return View();
        }

        // POST: Usuarios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nombre,Apellido,Cedula,Correo,Contraseña,Edad,RolId,Promedio")] Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                var nuevoUsuario = UsuariosFactory.CrearrUsuario(usuario.Nombre, usuario.Apellido, usuario.Cedula, usuario.Correo, usuario.Contraseña, usuario.Edad, usuario.RolId, usuario.Promedio);
                _context.Add(nuevoUsuario);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["RolId"] = new SelectList(_context.Roles, "RolId", "NombreRol", usuario.RolId);
            return View(usuario);
        }


        // GET: Usuarios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            ViewData["RolId"] = new SelectList(_context.Roles, "RolId", "NombreRol", usuario.RolId);
            return View(usuario);
        }

        // POST: Usuarios/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UsuarioId,Nombre,Apellido,Cedula,Correo,Contraseña,Edad,RolId,Promedio")] Usuario usuario)
        {
            if (id != usuario.UsuarioId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(usuario);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsuarioExists(usuario.UsuarioId))
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
            ViewData["RolId"] = new SelectList(_context.Roles, "RolId", "NombreRol", usuario.RolId);
            return View(usuario);
        }

        // GET: Usuarios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(m => m.UsuarioId == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // POST: Usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.UsuarioId == id);
        }
    }
}
