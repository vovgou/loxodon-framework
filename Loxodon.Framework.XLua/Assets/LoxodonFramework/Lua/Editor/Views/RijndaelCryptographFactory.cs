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
using Loxodon.Framework.Security.Cryptography;
using UnityEngine;

namespace Loxodon.Framework.XLua.Editors
{
    [Serializable]
    public class RijndaelCryptographFactory : EncryptorFactory
    {
        [SerializeField]
        private Algorithm algorithm = Algorithm.AES128_CBC_PKCS7;

        [Multiline(2)]
        [SerializeField]
        private string iv = "5Hh2390dQlVh0AqC";

        [Multiline(5)]
        [SerializeField]
        private string key = "E4YZgiGQ0aqe5LEJ";

        public Algorithm Algorithm
        {
            get { return this.algorithm; }
            set { this.algorithm = value; }
        }

        public string IV
        {
            get { return this.iv; }
            set { this.iv = value; }
        }

        public string Key
        {
            get { return this.key; }
            set { this.key = value; }
        }

        public override IEncryptor Create()
        {
            int keySize = 128;
            switch (this.Algorithm)
            {
                case Algorithm.AES128_CBC_PKCS7:
                    keySize = 128;
                    break;
                case Algorithm.AES192_CBC_PKCS7:
                    keySize = 192;
                    break;
                case Algorithm.AES256_CBC_PKCS7:
                    keySize = 256;
                    break;
            }
            return new RijndaelCryptograph(keySize, Encoding.ASCII.GetBytes(this.Key), Encoding.ASCII.GetBytes(this.IV));
        }
    }

    public enum Algorithm
    {
        AES128_CBC_PKCS7,
        AES192_CBC_PKCS7,
        AES256_CBC_PKCS7
    }
}
