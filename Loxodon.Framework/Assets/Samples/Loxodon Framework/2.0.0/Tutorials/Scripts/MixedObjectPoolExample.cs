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

using Loxodon.Framework.ObjectPool;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Loxodon.Framework.Examples
{

    public class MixedObjectPoolExample : MonoBehaviour
    {
        public GameObject template;

        private IMixedObjectPool<GameObject> pool;
        private Dictionary<string, List<GameObject>> dict;

        private void Start()
        {
            CubeMixedObjectFactory factory = new CubeMixedObjectFactory(this.template, this.transform);
            this.pool = new MixedObjectPool<GameObject>(factory, 5);
            this.dict = new Dictionary<string, List<GameObject>>();
        }

        private void OnDestroy()
        {
            if (this.pool != null)
            {
                this.pool.Dispose();
                this.pool = null;
            }
        }

        void OnGUI()
        {
            int x = 50;
            int y = 50;
            int width = 100;
            int height = 60;
            int i = 0;
            int padding = 10;

            if (GUI.Button(new Rect(x, y + i++ * (height + padding), width, height), "Add(red)"))
            {
                Add("red", 1);
            }
            if (GUI.Button(new Rect(x, y + i++ * (height + padding), width, height), "Add(green)"))
            {
                Add("green", 1);
            }
            if (GUI.Button(new Rect(x, y + i++ * (height + padding), width, height), "Add(blue)"))
            {
                Add("blue", 1);
            }

            if (GUI.Button(new Rect(x, y + i++ * (height + padding), width, height), "Delete(red)"))
            {
                Delete("red", 1);
            }
            if (GUI.Button(new Rect(x, y + i++ * (height + padding), width, height), "Delete(green)"))
            {
                Delete("green", 1);
            }
            if (GUI.Button(new Rect(x, y + i++ * (height + padding), width, height), "Delete(blue)"))
            {
                Delete("blue", 1);
            }
        }

        protected void Add(string typeName, int count)
        {
            List<GameObject> list;
            if (!this.dict.TryGetValue(typeName, out list))
            {
                list = new List<GameObject>();
                this.dict.Add(typeName, list);
            }

            for (int i = 0; i < count; i++)
            {
                GameObject go = this.pool.Allocate(typeName);
                go.transform.position = GetPosition();
                go.name = string.Format("Cube {0}-{1}", typeName, list.Count);
                go.SetActive(true);
                list.Add(go);
            }
        }

        protected void Delete(string typeName, int count)
        {
            List<GameObject> list;
            if (!this.dict.TryGetValue(typeName, out list) || list.Count <= 0)
                return;

            for (int i = 0; i < count; i++)
            {
                if (list.Count <= 0)
                    return;

                int index = list.Count - 1;
                GameObject go = list[index];
                list.RemoveAt(index);

                //this.pool.Free(go);
                //or
                IPooledObject freeable = go.GetComponent<IPooledObject>();
                freeable.Free();
            }
        }

        protected Vector3 GetPosition()
        {
            float x = UnityEngine.Random.Range(-10, 10);
            float y = UnityEngine.Random.Range(-5, 5);
            float z = UnityEngine.Random.Range(-10, 10);
            return new Vector3(x, y, z);
        }

        public class CubeMixedObjectFactory : UnityMixedGameObjectFactoryBase
        {
            private GameObject template;
            private Transform parent;
            public CubeMixedObjectFactory(GameObject template, Transform parent)
            {
                this.template = template;
                this.parent = parent;
            }

            protected override GameObject Create(string typeName)
            {
                Debug.LogFormat("Create a cube.");
                GameObject go = GameObject.Instantiate(this.template, parent);
                go.GetComponent<MeshRenderer>().material.color = GetColor(typeName);
                return go;
            }

            protected Color GetColor(string typeName)
            {
                if (typeName.Equals("red"))
                    return Color.red;
                if (typeName.Equals("green"))
                    return Color.green;
                if (typeName.Equals("blue"))
                    return Color.blue;

                throw new NotSupportedException("Unsupported type:" + typeName);
            }

            public override void Reset(string typeName, GameObject obj)
            {
                obj.SetActive(false);
                obj.name = string.Format("Cube {0}-Idle", typeName);
                obj.transform.position = Vector3.zero;
                obj.transform.rotation = Quaternion.Euler(Vector3.zero);
            }

            public override void Destroy(string typeName, GameObject obj)
            {
                base.Destroy(typeName, obj);
                Debug.LogFormat("Destroy a cube.");
            }
        }
    }
}