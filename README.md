# Proyecto de Notas
**Integrantes**:
- Mateo Encalada
- Fabiana Vásconez


**Descripcion**

El sistema propuesto es una avanzada plataforma de gestión académica diseñada específicamente para instituciones educativas que operan con múltiples semestres académicos. Esta herramienta integral facilitará la administración eficiente de profesores, estudiantes y materias, permitiendo la organización por semestres y la asignación detallada de profesores a cursos específicos. Con un enfoque en la accesibilidad y la simplicidad, la plataforma promete mejorar la comunicación entre el cuerpo docente y los estudiantes, asegurando que todos los involucrados estén informados y comprometidos con el proceso educativo.
La funcionalidad central del sistema permite el registro de notas para los estudiantes en tres evaluaciones progresivas durante el semestre, y el cálculo automático de las notas finales. Esta característica es esencial para mantener una evaluación continua del rendimiento estudiantil, permitiendo a los profesores y administradores identificar y actuar sobre las necesidades académicas de los estudiantes en tiempo real. Además, el sistema está diseñado para ser intuitivo y fácil de usar, garantizando que el personal docente pueda centrarse más en la enseñanza y menos en la gestión administrativa.
Un aspecto destacado del sistema es su capacidad para generar reportes detallados que identifican a los estudiantes en riesgo de reprobar. Estos reportes no solo muestran el rendimiento actual del estudiante, sino que también calculan las notas necesarias para alcanzar la nota mínima de aprobación establecida en 7 sobre 10. Esta funcionalidad es crucial para intervenir de manera efectiva y ayudar a los estudiantes a mejorar su rendimiento académico antes de que finalice el semestre, promoviendo una mayor tasa de éxito académico y reduciendo la tasa de repetición de cursos.
Para proporcionar un entendimiento más profundo del sistema MVC para la gestión académica, profundizaremos en cada componente del patrón Modelo-Vista-Controlador (MVC), explicando sus funciones y cómo interactúan dentro del sistema.


# Proyecto de Patrones de Diseño y Principios SOLID

## Patrones de Diseño

### Factory (Patrón de Fábrica)
**Descripción**: El patrón de fábrica es un patrón creacional que utiliza métodos de fábrica para crear objetos en lugar de instanciarlos directamente. La idea es que una clase delegue la responsabilidad de crear instancias de otras clases a una fábrica. Esto permite que la fábrica decida qué clase instanciar.

**Beneficio**: Promueve el desacoplamiento del código y hace que el sistema sea más flexible y escalable al permitir cambios en el proceso de creación de objetos sin modificar el código cliente.

**UsuariosFactory.cs**
```
using C.Models;

namespace C
{
    public class UsuariosFactory
    {
        public static Usuario CrearrUsuario(string nombre, string apellido, string cedula, string correo, string contraseña, int edad, int? rolId, decimal? promedio)
        {
            return new Usuario
            {
                Nombre = nombre,
                Apellido = apellido,
                Cedula = cedula,
                Correo = correo,
                Contraseña = contraseña,
                Edad = edad,
                RolId = rolId,
                Promedio = promedio
            };
        }
    }
}
```

**UsuariosController.cs**
```
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
```

### Singleton (Patrón de Singleton)
**Descripción**: El patrón singleton es un patrón creacional que garantiza que una clase solo tenga una instancia y proporciona un punto global de acceso a ella. Esto es útil cuando se necesita un control estricto sobre cómo y cuándo se crea una instancia de una clase.

**Beneficio**: Asegura que solo haya una instancia de una clase específica, reduciendo la cantidad de recursos utilizados y evitando posibles problemas de inconsistencia.

**DbContextSingleton.cs**
```
using C.Models;
using Microsoft.EntityFrameworkCore;

namespace C
{
    // DbContextSingleton.cs
    public class DbContextSingleton
    {
        private static CContext _instance;
        private static readonly object _lock = new object();

        private DbContextSingleton() { }

        public static CContext Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        var optionsBuilder = new DbContextOptionsBuilder<CContext>();
                        optionsBuilder.UseSqlServer("Server=PROOS10;Database=C;Trusted_Connection=True;TrustServerCertificate=True;");
                        _instance = new CContext(optionsBuilder.Options);
                    }
                    return _instance;
                }
            }
        }
    }

}

```

**Program.cs**
```
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<UsuarioService>();

// Usar el Singleton para el DbContext
builder.Services.AddSingleton<CContext>(provider => DbContextSingleton.Instance);

```
## Principio SOLID

### SRP (Single Responsibility Principle, Principio de Responsabilidad Única)
**Descripción**: Este principio establece que una clase debe tener una, y solo una, razón para cambiar. En otras palabras, una clase debe tener una única responsabilidad o propósito.

**Beneficio**: Hace que el código sea más fácil de mantener y entender. Si cada clase tiene una única responsabilidad, los cambios en esa clase no afectarán a otras partes del sistema.

**UsuarioService.cs**
```
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using C.Models;

namespace C.Services;

public class UsuarioService
{
    private readonly CContext _context;

    public UsuarioService(CContext context)
    {
        _context = context;
    }

    public async Task<List<Usuario>> GetAllUsuariosAsync()
    {
        return await _context.Usuarios.ToListAsync();
    }
}
```

**UsuariosController.cs**
```
public class UsuariosController : Controller
{
    private readonly UsuarioService _usuarioService;
    private readonly CContext _context;

    public UsuariosController(CContext context, UsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
        _context = context;
    }
}
```
# Conclusion
La adopción de patrones de diseño y principios SOLID no solo mejora la calidad y mantenibilidad del código, sino que también contribuye significativamente al éxito a largo plazo de los proyectos de software. Estas prácticas proporcionan una base sólida para el desarrollo de software escalable, flexible y robusto. Al integrar estos principios en el proceso de desarrollo, los equipos pueden abordar de manera más efectiva la complejidad inherente a los sistemas modernos, mejorar la colaboración y la comunicación, y asegurar que el software cumpla con los requisitos actuales y futuros del negocio.

# Herramientas
**Link Video Explicacion:** https://youtu.be/gViDWMJs6t0

**Link Proyecto Anterior:** https://github.com/f11v/C.git
