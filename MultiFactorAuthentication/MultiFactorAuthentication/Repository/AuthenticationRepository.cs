#region

using System;
using System.Data.Entity;
using System.Runtime.CompilerServices;
using MultiFactorAuthentication.EntityFramework;
using MultiFactorAuthentication.Interfaces;
using MultiFactorAuthentication.Models;

#endregion

#if DEBUG
[assembly: InternalsVisibleTo("MultiFactorAuthenticationTests")]
#endif

namespace MultiFactorAuthentication.Repository
{
    public class AuthenticationRepository : IAuthenticationRepository
    {
        private DatabaseContext _context;

        private AuthenticationRepository(DatabaseContext context)
        {
            _context = context;
        }

        public Authentication Find(Guid userid)
        {
            return _context.Authentications.Find(userid);
        }

        public void Add(Authentication authentication)
        {
            _context.Authentications.Add(authentication);
            _context.SaveChanges();
        }

        public void Remove(Guid userId)
        {
            var authentication = new Authentication {UserId = userId};
            _context.Entry(authentication).State = EntityState.Deleted;
            _context.SaveChanges();
        }

        public void Dispose()
        {
            if (_context == null) return;
            _context.Dispose();
            _context = null;
        }

        public void Save(Authentication user)
        {
            _context.Entry(user).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public static AuthenticationRepository Create(string nameOrConnectionString)
        {
            var context = new DatabaseContext(nameOrConnectionString);

            return new AuthenticationRepository(context);
        }
    }
}