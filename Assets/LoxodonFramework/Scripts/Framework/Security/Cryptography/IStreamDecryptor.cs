using System.IO;

namespace Loxodon.Framework.Security.Cryptography
{
    public interface IStreamDecryptor : IDecryptor
    {
        Stream Decrypt(Stream input);

    }
}