using System;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace TourmalineCore.AspNetCore.JwtAuthentication.Core.Signing
{
    public static class SigningHelper
    {
        public static RsaSecurityKey GetPublicKey(string key)
        {
            var publicKey = Convert.FromBase64String(key);

            var rsa = RSA.Create();
            rsa.ImportSubjectPublicKeyInfo(publicKey, out _);

            return new RsaSecurityKey(rsa);
        }

        public static RsaSecurityKey GetPrivateKey(string key)
        {
            var privateKey = Convert.FromBase64String(key);

            var rsa = RSA.Create();
            rsa.ImportRSAPrivateKey(privateKey, out _);

            return new RsaSecurityKey(rsa);
        }
    }
}