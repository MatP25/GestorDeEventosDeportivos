using System.Security.Cryptography;

// Usado para encriptar y desencriptar IDs de usuarios que son expuestos en la UI
public class EncriptacionIds
{
    private class KeyIvHolder
    {
        public byte[] Key = new byte[32]; // AES-256 key size
        public byte[] IV = new byte[16];  // AES block size
    }

    private static KeyIvHolder GetKeyIvHolder(IConfiguration configuration)
    {
        var keyBase64 = configuration["EncriptacionIds:Key"];
        var ivBase64 = configuration["EncriptacionIds:IV"];

        if (string.IsNullOrWhiteSpace(keyBase64) || string.IsNullOrWhiteSpace(ivBase64))
            throw new InvalidOperationException("No se encontraron las claves de encriptación (EncriptacionIds:Key, EncriptacionIds:IV).");

        return new KeyIvHolder
        {
            Key = Convert.FromBase64String(keyBase64),
            IV = Convert.FromBase64String(ivBase64)
        };
    }

    private static string ToUrlSafeBase64(byte[] data)
    {
        return Convert.ToBase64String(data)
            .Replace('+', '-')
            .Replace('/', '_')
            .TrimEnd('=');
    }

    private static byte[] FromUrlSafeBase64(string urlSafeBase64)
    {
        string base64 = urlSafeBase64
            .Replace('-', '+')
            .Replace('_', '/');
        
        // Add padding if needed
        switch (base64.Length % 4)
        {
            case 2: base64 += "=="; break;
            case 3: base64 += "="; break;
        }
        
        return Convert.FromBase64String(base64);
    }

    public static string EncriptarString(string plainText)
    {
        KeyIvHolder keyIvHolder = GetKeyIvHolder(new ConfigurationBuilder().AddJsonFile("appsettings.json").Build());

        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = keyIvHolder.Key;
            aesAlg.IV = keyIvHolder.IV;

            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(plainText);
                    }
                    return ToUrlSafeBase64(msEncrypt.ToArray());
                }
            }
        }
    }

    public static string DesencriptarString(string cipherText)
    {
        KeyIvHolder keyIvHolder = GetKeyIvHolder(new ConfigurationBuilder().AddJsonFile("appsettings.json").Build());

        byte[] cipherBytes = FromUrlSafeBase64(cipherText);

        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = keyIvHolder.Key;
            aesAlg.IV = keyIvHolder.IV;

            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            using (MemoryStream msDecrypt = new MemoryStream(cipherBytes))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {
                        return srDecrypt.ReadToEnd();
                    }
                }
            }
        }
    }
}