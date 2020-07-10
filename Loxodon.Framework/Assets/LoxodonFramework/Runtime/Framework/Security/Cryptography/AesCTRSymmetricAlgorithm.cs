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

#if !NETFX_CORE && !UNITY_WSA && !UNITY_WSA_10_0
using System;
using System.Security.Cryptography;

namespace Loxodon.Framework.Security.Cryptography
{
    public class AesCTRSymmetricAlgorithm : SymmetricAlgorithm
    {
        private RijndaelManaged rijndael;

        public AesCTRSymmetricAlgorithm(byte[] key, byte[] iv)
        {
            int blockSize = 128;
            this.BlockSizeValue = blockSize;
            this.ModeValue = CipherMode.ECB;
            this.PaddingValue = PaddingMode.None;

            this.KeyValue = key;
            this.IVValue = iv;

            this.rijndael = new RijndaelManaged()
            {
                Mode = CipherMode.ECB,
                Padding = PaddingMode.None,
                KeySize = 128,
                BlockSize = blockSize
            };
        }

        public override ICryptoTransform CreateDecryptor(byte[] rgbKey, byte[] rgbIV)
        {
            return new AesCTRCryptoTransform(rijndael, rgbKey, rgbIV);
        }

        public override ICryptoTransform CreateEncryptor(byte[] rgbKey, byte[] rgbIV)
        {
            return new AesCTRCryptoTransform(rijndael, rgbKey, rgbIV);
        }

        public override void GenerateIV()
        {
            rijndael.GenerateIV();
        }

        public override void GenerateKey()
        {
            rijndael.GenerateKey();
        }
    }

    public class AesCTRCryptoTransform : ICryptoTransform
    {
        private readonly byte[] key;
        private readonly byte[] iv;
        private readonly ICryptoTransform transform;
        private readonly int blockSize;

        private long position;
        private uint counter;
        private int index;
        private byte[] masks;
        public AesCTRCryptoTransform(SymmetricAlgorithm algorithm, byte[] key, byte[] iv)
        {
            this.key = key;
            this.iv = iv;
            this.blockSize = algorithm.BlockSize / 8;
            this.transform = algorithm.CreateEncryptor(this.key, new byte[blockSize]);

            this.masks = new byte[blockSize];
            this.counter = 0;
            this.index = 0;
            CalculateMask(counter);
        }

        public bool CanTransformMultipleBlocks { get { return true; } }
        public bool CanReuseTransform { get { return false; } }
        public int InputBlockSize { get { return blockSize; } }
        public int OutputBlockSize { get { return blockSize; } }

        protected uint Counter
        {
            get { return this.counter; }
            set
            {
                if (this.counter == value)
                    return;

                this.counter = value;
                CalculateMask(this.counter);
            }
        }

        public long Position
        {
            get { return position; }
            set
            {
                if (this.position == value)
                    return;

                this.position = value;
                this.Counter = (uint)(this.position / blockSize);
                this.index = (int)(this.position % blockSize);
            }
        }

        public int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
        {
            for (var i = 0; i < inputCount; i++)
            {
                byte mask = masks[index];
                outputBuffer[outputOffset + i] = (byte)(inputBuffer[inputOffset + i] ^ mask);

                this.position++;
                this.index++;
                if (index == blockSize)
                {
                    this.Counter++;
                    this.index = 0;
                }
            }
            return inputCount;
        }

        public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
        {
            var outputBuffer = new byte[inputCount];
            TransformBlock(inputBuffer, inputOffset, inputCount, outputBuffer, 0);
            return outputBuffer;
        }

        private void CalculateMask(uint counter)
        {
            byte[] data = BitConverter.GetBytes(counter);
            Array.Copy(data, 0, iv, 12, 4);
            transform.TransformBlock(iv, 0, iv.Length, masks, 0);
        }

        public void Dispose()
        {
        }
    }
}
#endif