using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Duckify {
    public static class IdentityUserExtension {

        //I know this is not ideal, but it is what it is..
        private static readonly ConditionalWeakTable<IdentityUser, string> _tokens = new ConditionalWeakTable<IdentityUser, string>();

        public static void SetSpotifyToken(this IdentityUser user, string token) {
            var existing = _tokens.FirstOrDefault(x => x.Key.UserName == user.UserName);
            if(existing.Key != null) {
                _tokens.Remove(existing.Key);
            }
            _tokens.Add(user, token);
        }

        public static string GetSpotifyToken(this IdentityUser user) {
            string token;
            var existing = _tokens.FirstOrDefault(x => x.Key.UserName == user.UserName);
            if(existing.Key != null){
                return existing.Value;
            }
            return default;
        }
    }
}
