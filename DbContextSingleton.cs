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
