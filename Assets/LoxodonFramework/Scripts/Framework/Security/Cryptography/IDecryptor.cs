namespace Loxodon.Framework.Security.Cryptography
{
    public interface IDecryptor
    {
        byte[] Decrypt(byte[] buffer);

    }
}