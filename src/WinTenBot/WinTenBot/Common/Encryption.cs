using System;
using System.Security.Cryptography;
using EasyEncrypt2;
using Serilog;

namespace WinTenBot.Common
{
    public static class EncryptionHelper
    {
        public static string Password { get; set; } = "1234";
        public static string Salt { get; set; } = "12345678";

        public static string AesEncrypt(this string input)
        {
            try
            {
                return new EasyEncrypt(Password, Salt, Aes.Create()).Encrypt(input);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error AES Encrypt");
                return null;
            }
        }

        public static string AesDecrypt(this string encryptedInput)
        {
            try
            {
                return new EasyEncrypt(Password, Salt, Aes.Create()).Decrypt(encryptedInput);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error AES Decrypt");
                return null;
            }
        }
    }
}