using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Text.RegularExpressions;

namespace Duckify {
    public class Auth {

        public static Dictionary<string, Auth> ValidUsers { get; set; } = new Dictionary<string, Auth>();
        public static Dictionary<string, Auth> TempStore { get; set; } = new Dictionary<string, Auth>();

        private static Random rnd = new Random(DateTime.Now.Millisecond);

        public string Key { get; set; }
        public string Vector { get; set; }
        public string Token { get; set; }
        public string DecryptedToken { get; set; }
        private bool _isLocal;
        private string _ipAddress;


        public Auth() {
            Key = GetRandomString();
            Vector = GetRandomString();
        }

        public static bool IsAuthorized(string auth) {
            if (string.IsNullOrEmpty(auth)) {
                return false;
            }
            if (!Auth.ValidUsers.ContainsKey(auth)) {
                return false;
            }
            //TODO: Add checks that trigger themselfs at random
            return true;
        }

        public (bool validated, string token) ValidateToken(string token, string ip) {
            token = token.Replace(' ', '+');
            var decrypted = DecryptString(token);
            var contains = ValidUsers.Values.FirstOrDefault(x => x.DecryptedToken == decrypted);
            //This means that user with the same local and public IP address already accessed the app and will recieve exiting token.
            if (contains != default) {
                return (true, contains.Token);
            }
            //First validity check - make sure the format is valid
        
            var parts = decrypted.Split(':').Where(s => !string.IsNullOrWhiteSpace(s)).Distinct();
            if (parts.Count() != 2) {
                return (false, null);
            }
            //TODO: Second check - regex the shit out of the token
            //Third check - make sure the IP is not faked.
            //if ((parts[1] != ip || parts[0] != ip) && ip != "::1" && ip != "127.0.0.1") {
            //    return (false, null);
            //}
            //Checks have passed, create new entery in ValidUsers dictionary.
            _ipAddress = ip;
            Token = token;
            DecryptedToken = decrypted;
            ValidUsers.Add(token, this);
            return (true, token);
        }

        private string GetRandomString() {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, 16).Select(s => s[rnd.Next(s.Length)]).ToArray());
        }

        public string DecryptString(string token) {
            var cipherText = Convert.FromBase64String(token);
            var key = Encoding.UTF8.GetBytes(Key);
            var iv = Encoding.UTF8.GetBytes(Vector);

            string plaintext = null;
            using (var rijndael = new RijndaelManaged()) {
                rijndael.Mode = CipherMode.CBC;
                rijndael.Padding = PaddingMode.PKCS7;
                rijndael.FeedbackSize = 128;
                rijndael.Key = key;
                rijndael.IV = iv;
                var decryptor = rijndael.CreateDecryptor(rijndael.Key, rijndael.IV);
                using (var msDecrypt = new MemoryStream(cipherText)) {
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read)) {
                        using (var srDecrypt = new StreamReader(csDecrypt)) {
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
            return plaintext;
        }

    }
}
