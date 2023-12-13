/*
 * MIT License
 *
 * Copyright (c) 2018 Clark Yang
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of 
 * this software and associated documentation files (the "Software"), to deal in 
 * the Software without restriction, including without limitation the rights to 
 * use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies 
 * of the Software, and to permit persons to whom the Software is furnished to do so, 
 * subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all 
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE 
 * SOFTWARE.
 */

using System;
using System.Text;

#if NETFX_CORE
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;
#else
using System.Security.Cryptography;
#endif

namespace Loxodon.Framework.Prefs
{
    /// <summary>
    /// 
    /// </summary>
    public class DefaultEncryptor : IEncryptor
    {
        private const int IV_SIZE = 16;
        private static readonly byte[] DEFAULT_IV;
        private static readonly byte[] DEFAULT_KEY;
#if NETFX_CORE
        private CryptographicKey cryptographicKey;
#else
        private RijndaelManaged cipher;
#endif
        private byte[] iv = null;
        private byte[] key = null;

        static DefaultEncryptor()
        {
            DEFAULT_IV = Encoding.ASCII.GetBytes("5CyM5tcL3yDFiWlN");
            DEFAULT_KEY = Encoding.ASCII.GetBytes("W8fnmqMynlTJXPM1");
        }

        /// <summary>
        /// 
        /// </summary>
        public DefaultEncryptor() : this(null, null)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        public DefaultEncryptor(byte[] key, byte[] iv)
        {
            this.iv = iv != null ? iv : DEFAULT_IV;
            this.key = key != null ? key : DEFAULT_KEY;

            CheckIV(this.iv);
            CheckKey(this.key);

#if NETFX_CORE
            SymmetricKeyAlgorithmProvider provider = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.AesCbcPkcs7);
            cryptographicKey = provider.CreateSymmetricKey(this.key.AsBuffer());
#else
            cipher = new RijndaelManaged()
            {
                Mode = CipherMode.CBC,//use CBC
                Padding = PaddingMode.PKCS7,//default PKCS7
                KeySize = 128,//default 256
                BlockSize = 128,//default 128
                FeedbackSize = 128      //default 128
            };
#endif
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        protected bool CheckKey(byte[] bytes)
        {
            if (bytes == null || (bytes.Length != 16 && bytes.Length != 24 && bytes.Length != 32))
                throw new ArgumentException("The 'Key' must be 16byte 24byte or 32byte!");
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        protected bool CheckIV(byte[] bytes)
        {
            if (bytes == null || bytes.Length != IV_SIZE)
                throw new ArgumentException("The 'IV' must be 16byte!");
            return true;
        }

        /// <summary>
        /// encryption
        /// </summary>
        /// <param name="plainData"></param>
        /// <returns></returns>
        public byte[] Encode(byte[] plainData)
        {
#if NETFX_CORE
            IBuffer bufferEncrypt = CryptographicEngine.Encrypt(cryptographicKey, plainData.AsBuffer(), iv.AsBuffer());
            return bufferEncrypt.ToArray();
#else
            ICryptoTransform encryptor = cipher.CreateEncryptor(key, iv);
            return encryptor.TransformFinalBlock(plainData, 0, plainData.Length);
#endif
        }

        /// <summary>
        /// decryption
        /// </summary>
        /// <param name="cipherData"></param>
        /// <returns></returns>
        public byte[] Decode(byte[] cipherData)
        {
#if NETFX_CORE
            IBuffer bufferDecrypt = CryptographicEngine.Decrypt(cryptographicKey, cipherData.AsBuffer(), iv.AsBuffer());
            return bufferDecrypt.ToArray();
#else
            ICryptoTransform decryptor = cipher.CreateDecryptor(key, iv);
            return decryptor.TransformFinalBlock(cipherData, 0, cipherData.Length);
#endif
        }
    }
}