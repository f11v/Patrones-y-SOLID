namespace C.Models
{
    // UsuarioFactory.cs
    public static class UsuarioFactory
    {
        public static Usuario CrearUsuario(string correo, string contraseña, int rolId)
        {
            return new Usuario
            {
                Correo = correo,
                Contraseña = contraseña,
                RolId = rolId
            };
        }
    }

}
