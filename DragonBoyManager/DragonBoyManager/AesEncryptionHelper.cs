using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Win32;

namespace DragonBoyManager
{

    public static class AesEncryptionHelper
        {
        // Lấy mã thiết bị (Machine GUID) từ registry
        private static string GetMachineGuid()
        {
            try
            {
                using (var localMachineX64View = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
                using (var rk = localMachineX64View.OpenSubKey(@"SOFTWARE\Microsoft\Cryptography"))
                {
                    return rk?.GetValue("MachineGuid")?.ToString();
                }
            }
            catch
            {
                return null;
            }
        }


        private static byte[] GetKeyBytes()
        {
            string guid = GetMachineGuid();

            if (string.IsNullOrWhiteSpace(guid))
                throw new Exception("Không thể lấy MachineGuid. Khóa mã hóa không hợp lệ.");

            // Hash để đảm bảo đủ 256-bit (32 bytes) cho AES-256
            using (SHA256 sha = SHA256.Create())
            {
                return sha.ComputeHash(Encoding.UTF8.GetBytes(guid));
            }
        }


        public static string Encrypt(string plainText)
            {
                byte[] keyBytes = GetKeyBytes();
                using (Aes aes = Aes.Create())
                {
                    aes.Key = keyBytes;
                    aes.GenerateIV();
                    byte[] iv = aes.IV;

                    using (MemoryStream ms = new MemoryStream())
                    {
                        ms.Write(iv, 0, iv.Length); // ghi IV vào đầu

                        using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
                            cs.Write(plainBytes, 0, plainBytes.Length);
                            cs.FlushFinalBlock();
                            return Convert.ToBase64String(ms.ToArray());
                        }
                    }
                }
            }

            public static string Decrypt(string encryptedText)
            {
                byte[] fullCipher = Convert.FromBase64String(encryptedText);
                byte[] keyBytes = GetKeyBytes();

                using (Aes aes = Aes.Create())
                {
                    aes.Key = keyBytes;

                    byte[] iv = new byte[16];
                    Array.Copy(fullCipher, 0, iv, 0, iv.Length);
                    aes.IV = iv;

                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(fullCipher, iv.Length, fullCipher.Length - iv.Length);
                            cs.FlushFinalBlock();
                            return Encoding.UTF8.GetString(ms.ToArray());
                        }
                    }
                }
            }
        }
}
