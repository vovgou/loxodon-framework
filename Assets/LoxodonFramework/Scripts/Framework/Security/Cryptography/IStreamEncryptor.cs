using System.IO;

namespace Loxodon.Framework.Security.Cryptography
{
    public interface IStreamEncryptor : IEncryptor
    {
        Stream Encrypt(Stream input);

    }
}