using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace DoorRequest.API
{
    public class OpenLDAPPasswordHelpers
    {
        public static string GenerateSaltedSHA1(string plainTextString)
        {
            HashAlgorithm algorithm = SHA1.Create();
            var saltBytes = GenerateSalt(4);
            var plainTextBytes = Encoding.ASCII.GetBytes(plainTextString);

            var plainTextWithSaltBytes = AppendByteArray(plainTextBytes, saltBytes);
            var saltedSHA1Bytes = algorithm.ComputeHash(plainTextWithSaltBytes);
            var saltedSHA1WithAppendedSaltBytes = AppendByteArray(saltedSHA1Bytes, saltBytes);

            return "{SSHA}" + Convert.ToBase64String(saltedSHA1WithAppendedSaltBytes);
        }

        public static void Compare(string plainTextString, string password)
        {
            var saltBytes = GetSaltBytes(password);
            var saltedSHA = GenerateSaltedSHA1(plainTextString, saltBytes);
            if (saltedSHA != password)
            {
                throw new Exception("Invalid credentials");
            }
        }

        public static string GenerateSaltedSHA1(string plainTextString, byte[] saltBytes)
        {
            HashAlgorithm algorithm = SHA1.Create();
            var plainTextBytes = Encoding.UTF8.GetBytes(plainTextString);

            var plainTextWithSaltBytes = AppendByteArray(plainTextBytes, saltBytes);
            var saltedSHA1Bytes = algorithm.ComputeHash(plainTextWithSaltBytes);
            var saltedSHA1WithAppendedSaltBytes = AppendByteArray(saltedSHA1Bytes, saltBytes);

            var base64Prefixed = "{SSHA}" + Convert.ToBase64String(saltedSHA1WithAppendedSaltBytes);
            return base64Prefixed;
        }

        private static byte[] GenerateSalt(int saltSize)
        {
            var rng = RandomNumberGenerator.Create();
            var buff = new byte[saltSize];
            rng.GetBytes(buff);
            return buff;
        }

        private static byte[] AppendByteArray(byte[] byteArray1, byte[] byteArray2)
        {
            var byteArrayResult =
                new byte[byteArray1.Length + byteArray2.Length];

            for (var i = 0; i < byteArray1.Length; i++)
                byteArrayResult[i] = byteArray1[i];
            for (var i = 0; i < byteArray2.Length; i++)
                byteArrayResult[byteArray1.Length + i] = byteArray2[i];

            return byteArrayResult;
        }

        public static byte[] GetSaltBytes(string password)
        {
            var rawPassword = string.Empty;
            if (!password.StartsWith("{SSHA}"))
            {
                var base64 = Convert.FromBase64String(password);
                rawPassword = Encoding.UTF8.GetString(base64);
            }
            else
            {
                rawPassword = password;
            }
            var strippedBase64String = rawPassword.Replace("{SSHA}", string.Empty);
            var strippedBase64 = Convert.FromBase64String(strippedBase64String);
            byte[] saltBytes = strippedBase64.Skip(20).ToArray();
            return saltBytes.ToArray();
        }
    }
}