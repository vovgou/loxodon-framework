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
using System.Collections.Generic;
using UnityEngine;

namespace Loxodon.Framework.Examples
{
    public class ComponentPoolExample : MonoBehaviour
    {
        public GameObject template;

        private IObjectPool<MeshRenderer> pool;
        private List<MeshRenderer> list;
        private List<Color> colors;

        private void Start()
        {
            CubeObjectFactory2 factory = new CubeObjectFactory2(this.template, this.transform);
            this.pool = new ObjectPool<MeshRenderer>(factory, 10, 20);

            this.list = new List<MeshRenderer>();
            this.colors = new List<Color>()
            {
                Color.black,
                Color.blue,
                Color.red,
                Color.yellow,
                Color.white,
                Color.green
            };

            Add(10);
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

            if (GUI.Button(new Rect(x, y + i++ * (height + padding), width, height), "Add"))
            {
                Add(1);
            }

            if (GUI.Button(new Rect(x, y + i++ * (height + padding), width, height), "Delete"))
            {
                Delete(1);
            }
        }

        protected void Add(int count)
        {
            for (int i = 0; i < count; i++)
            {
                MeshRenderer go = this.pool.Allocate();
                go.material.color = GetColor();
                go.transform.position = GetPosition();
                go.name = string.Format("Cube {0}", this.list.Count);
                go.gameObject.SetActive(true);
                this.list.Add(go);
            }
        }

        protected void Delete(int count)
        {
            for (int i = 0; i < count; i++)
            {
                if (this.list.Count <= 0)
                    return;

                int index = this.list.Count - 1;
                MeshRenderer go = this.list[index];
                this.list.RemoveAt(index);

                //this.pool.Free(go);
                //or
                IPooledObject freeable = go.GetComponent<IPooledObject>();
                freeable.Free();
            }
        }

        protected Color GetColor()
        {
            int index = Random.Range(0, colors.Count);
            return colors[index];
        }

        protected Vector3 GetPosition()
        {
            float x = Random.Range(-10, 10);
            float y = Random.Range(-5, 5);
            float z = Random.Range(-10, 10);
            return new Vector3(x, y, z);
        }
    }
}