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
using UnityEngine;

namespace Loxodon.Framework.Examples.Repositories
{
    public class JsonUserInfoRepository : IUserInfoRepository
    {
        private Dictionary<int, UserInfo> idAndUserInfoMapping = new Dictionary<int, UserInfo>();
        private Dictionary<string, UserInfo> usernameAndUserInfoMapping = new Dictionary<string, UserInfo>();
        private bool loaded = false;
        private void LoadAll()
        {
            var text = Resources.Load<TextAsset>("Json/userinfo");
            if (text == null || text.text.Length <= 0)
                return;

            using (StringReader reader = new StringReader(text.text))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    UserInfo userInfo = JsonConvert.DeserializeObject<UserInfo>(line);
                    if (userInfo == null)
                        continue;

                    idAndUserInfoMapping[userInfo.Id] = userInfo;
                    usernameAndUserInfoMapping[userInfo.Username] = userInfo;
                }
            }
            this.loaded = true;
        }

        public virtual UserInfo GetById(int id)
        {
            if (!loaded)
                this.LoadAll();

            UserInfo userInfo = null;
            idAndUserInfoMapping.TryGetValue(id, out userInfo);
            return userInfo;
        }

        public virtual UserInfo GetByUsername(string username)
        {
            if (!loaded)
                this.LoadAll();

            UserInfo userInfo = null;
            usernameAndUserInfoMapping.TryGetValue(username, out userInfo);
            return userInfo;
        }
    }
}
#endif