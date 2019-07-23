namespace Loxodon.Framework.Security.Cryptography
{
    public interface IEncryptor
    {
        byte[] Encrypt(byte[] buffer);

    }
}
