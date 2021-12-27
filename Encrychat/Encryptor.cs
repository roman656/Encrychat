using System;
using System.Security.Cryptography;
using System.Text;

namespace Encrychat
{
    public static class Encryptor
    {
        public static readonly string PublicKey;
        public static readonly string PrivateKey;

        static Encryptor()
        {
            using var rsa = new RSACryptoServiceProvider(Settings.KeySize);
            PublicKey = GetKeyString(rsa.ExportParameters(false));
            PrivateKey = GetKeyString(rsa.ExportParameters(true));
        }
        
        public static string Encrypt(string text) => Encrypt(text, PublicKey);
        public static string Decrypt(string text) => Decrypt(text, PrivateKey);
        
        public static string GetKeysMessage()
        {
            var separatorLine = new string('=', Settings.SeparatorLinesSize);
            var beginSeparator = $"<{separatorLine}[Ключи-начало]{separatorLine}>\n";
            var endSeparator = $"<{separatorLine}[Ключи--конец]{separatorLine}>\n";
            return $"{beginSeparator}[Открытый ключ]:\n{PublicKey}\n[Закрытый ключ]:\n{PrivateKey}\n{endSeparator}";
        }

        private static string GetKeyString(RSAParameters key)
        {
            var stringWriter = new System.IO.StringWriter();
            var xmlSerializer = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
            xmlSerializer.Serialize(stringWriter, key);
            return stringWriter.ToString();
        }

        public static string Encrypt(string text, string publicKey)
        {
            using var rsa = new RSACryptoServiceProvider(Settings.KeySize);
            rsa.FromXmlString(publicKey);
            return Convert.ToBase64String(rsa.Encrypt(Encoding.UTF8.GetBytes(text), false));
        }

        public static string Decrypt(string text, string privateKey)
        {
            using var rsa = new RSACryptoServiceProvider(Settings.KeySize);
            rsa.FromXmlString(privateKey);
            return Encoding.UTF8.GetString(rsa.Decrypt(Convert.FromBase64String(text), false));
        }
    }
}