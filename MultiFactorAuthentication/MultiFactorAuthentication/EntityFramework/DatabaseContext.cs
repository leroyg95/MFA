#region

using System.Data.Entity;
using MultiFactorAuthentication.Models;

#endregion

namespace MultiFactorAuthentication.EntityFramework
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext()
        {
        }

        public DatabaseContext(string nameOrConnectionString) : base(nameOrConnectionString)
        {
        }

        public DbSet<Authentication> Authentications { get; set; }
    }
}