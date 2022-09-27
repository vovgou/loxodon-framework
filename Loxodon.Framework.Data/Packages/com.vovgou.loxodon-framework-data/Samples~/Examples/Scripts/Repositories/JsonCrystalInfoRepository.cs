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

#if NEWTONSOFT
using Loxodon.Framework.Examples.Domains;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Loxodon.Framework.Examples.Repositories
{
    public class JsonCrystalInfoRepository : ICrystalInfoRepository
    {
        private Dictionary<int, CrystalInfo> crystals = new Dictionary<int, CrystalInfo>();
        private bool loaded = false;

        private void LoadAll()
        {
            var text = Resources.Load<TextAsset>("Json/crystalinfo");
            if (text == null || text.text.Length <= 0)
                return;

            using (StringReader reader = new StringReader(text.text))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    CrystalInfo info = JsonConvert.DeserializeObject<CrystalInfo>(line);
                    if (info == null)
                        continue;

                    crystals[info.Id] = info;
                }
            }
            this.loaded = true;
        }

        public CrystalInfo GetById(int id)
        {
            if (!loaded)
                this.LoadAll();

            CrystalInfo info = null;
            crystals.TryGetValue(id, out info);
            return info;
        }

        public CrystalInfo GetBySign(string sign, int level)
        {
            if (!loaded)
                this.LoadAll();
            return crystals.Values.Where(e => e.Sign.Equals(sign) && e.Level == level).First();
        }
    }
}
#endif