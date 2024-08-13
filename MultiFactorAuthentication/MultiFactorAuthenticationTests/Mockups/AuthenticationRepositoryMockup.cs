using System;
using System.Collections.Generic;
using MultiFactorAuthentication.Interfaces;
using MultiFactorAuthentication.Models;

namespace MultiFactorAuthenticationTests.Mockups
{
    public class AuthenticationRepositoryMockup : IAuthenticationRepository
    {
        readonly Dictionary<Guid, Authentication> _dictionary = new Dictionary<Guid, Authentication>();

        public void Add(Authentication authentication)
        {
            _dictionary.Add(authentication.UserId, authentication);
        }

        public Authentication Find(Guid userId)
        {
            if (_dictionary.ContainsKey(userId))
            {
                return _dictionary[userId];
            }

            return null;
        }

        public void Remove(Guid userId)
        {
            if (_dictionary.ContainsKey(userId))
            {
                _dictionary.Remove(userId);
            }
        }

        public void Save(Authentication authentication)
        {
            _dictionary[authentication.UserId] = authentication;
        }

        public void Dispose()
        {
        }
    }
}
