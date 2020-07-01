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

namespace Loxodon.Framework.Net
{
    public enum UNIT
    {
        BYTE,
        KB,
        MB,
        GB
    }

    public class ProgressInfo
    {
        private long totalSize = 0;
        private long completedSize = 0;

        private int totalCount = 0;
        private int completedCount = 0;

        private float speed = 0f;
        private long lastTime = -1;
        private long lastValue = -1;
        private long lastTime2 = -1;
        private long lastValue2 = -1;

        public ProgressInfo() : this(0, 0)
        {
        }

        public ProgressInfo(long totalSize, long completedSize)
        {
            this.totalSize = totalSize;
            this.completedSize = completedSize;

            lastTime = DateTime.UtcNow.Ticks / 10000;
            lastValue = this.completedSize;

            lastTime2 = lastTime;
            lastValue2 = lastValue;
        }

        public long TotalSize
        {
            get { return this.totalSize; }
            set { this.totalSize = value; }
        }
        public long CompletedSize
        {
            get { return this.completedSize; }
            set
            {
                this.completedSize = value;
                this.OnUpdate();
            }
        }

        public int TotalCount
        {
            get { return this.totalCount; }
            set { this.totalCount = value; }
        }
        public int CompletedCount
        {
            get { return this.completedCount; }
            set { this.completedCount = value; }
        }

        private void OnUpdate()
        {
            long now = DateTime.UtcNow.Ticks / 10000;

            if ((now - lastTime) >= 1000)
            {
                lastTime2 = lastTime;
                lastValue2 = lastValue;

                this.lastTime = now;
                this.lastValue = this.completedSize;
            }

            float dt = (now - lastTime2) / 1000f;
            speed = (this.completedSize - this.lastValue2) / dt;
        }

        public virtual float Value
        {
            get
            {
                if (this.totalSize <= 0)
                    return 0f;

                return this.completedSize / (float)this.totalSize;
            }
        }

        public virtual float GetTotalSize(UNIT unit = UNIT.BYTE)
        {
            switch (unit)
            {
                case UNIT.KB:
                    return this.totalSize / 1024f;
                case UNIT.MB:
                    return this.totalSize / 1048576f;
                case UNIT.GB:
                    return this.totalSize / 1073741824f;
                default:
                    return (float)this.totalSize;
            }
        }

        public virtual float GetCompletedSize(UNIT unit = UNIT.BYTE)
        {
            switch (unit)
            {
                case UNIT.KB:
                    return this.completedSize / 1024f;
                case UNIT.MB:
                    return this.completedSize / 1048576f;
                case UNIT.GB:
                    return this.completedSize / 1073741824f;
                default:
                    return (float)this.completedSize;
            }
        }

        public virtual float GetSpeed(UNIT unit = UNIT.BYTE)
        {
            switch (unit)
            {
                case UNIT.KB:
                    return speed / 1024f;
                case UNIT.MB:
                    return speed / 1048576f;
                case UNIT.GB:
                    return speed / 1073741824f;
                default:
                    return speed;
            }
        }
    }
}