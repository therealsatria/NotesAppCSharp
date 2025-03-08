using System.Security.Cryptography;

namespace NotesAppCSharp.Services;

public class EncryptionService
{
    private readonly byte[] _key;
    private const int TagSize = 16; // Ukuran tag 16 byte (128 bit)

    public EncryptionService(byte[] key)
    {
        _key = key;
    }

    public byte[] Encrypt(string data)
    {
        using var aes = new AesGcm(_key, TagSize); // Tentukan ukuran tag
        byte[] nonce = RandomNumberGenerator.GetBytes(AesGcm.NonceByteSizes.MaxSize);
        byte[] plaintext = System.Text.Encoding.UTF8.GetBytes(data);
        byte[] ciphertext = new byte[plaintext.Length];
        byte[] tag = new byte[TagSize];

        aes.Encrypt(nonce, plaintext, ciphertext, tag);

        byte[] result = new byte[nonce.Length + ciphertext.Length + tag.Length];
        Array.Copy(nonce, 0, result, 0, nonce.Length);
        Array.Copy(ciphertext, 0, result, nonce.Length, ciphertext.Length);
        Array.Copy(tag, 0, result, nonce.Length + ciphertext.Length, tag.Length);

        return result;
    }

    public string Decrypt(byte[] encryptedData)
    {
        using var aes = new AesGcm(_key, TagSize); // Tentukan ukuran tag
        byte[] nonce = new byte[AesGcm.NonceByteSizes.MaxSize];
        byte[] tag = new byte[TagSize];
        byte[] ciphertext = new byte[encryptedData.Length - nonce.Length - tag.Length];

        Array.Copy(encryptedData, 0, nonce, 0, nonce.Length);
        Array.Copy(encryptedData, nonce.Length, ciphertext, 0, ciphertext.Length);
        Array.Copy(encryptedData, nonce.Length + ciphertext.Length, tag, 0, tag.Length);

        byte[] plaintext = new byte[ciphertext.Length];
        aes.Decrypt(nonce, ciphertext, tag, plaintext);

        return System.Text.Encoding.UTF8.GetString(plaintext);
    }
}