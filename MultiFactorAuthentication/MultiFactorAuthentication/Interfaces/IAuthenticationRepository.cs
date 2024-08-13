#region

using System;
using MultiFactorAuthentication.Models;

#endregion

namespace MultiFactorAuthentication.Interfaces
{
    public interface IAuthenticationRepository : IDisposable
    {
        Authentication Find(Guid userId);

        void Add(Authentication authentication);

        void Remove(Guid userId);

        void Save(Authentication autentication);
    }
}