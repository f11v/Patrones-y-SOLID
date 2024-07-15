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
