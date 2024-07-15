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
