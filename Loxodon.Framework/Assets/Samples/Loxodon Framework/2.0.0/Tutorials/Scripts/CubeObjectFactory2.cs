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
using UnityEngine;

namespace Loxodon.Framework.Examples
{
    public class CubeObjectFactory2 : UnityComponentFactoryBase<MeshRenderer>
    {
        private GameObject template;
        private Transform parent;
        public CubeObjectFactory2(GameObject template, Transform parent)
        {
            this.template = template;
            this.parent = parent;
        }

        protected override MeshRenderer Create()
        {
            Debug.LogFormat("Create a cube.");
            GameObject go = Object.Instantiate(this.template, parent);
            return go.GetComponent<MeshRenderer>();
        }

        public override void Reset(MeshRenderer obj)
        {
            obj.gameObject.SetActive(false);
            obj.gameObject.name = "Cube Idle";
            obj.transform.position = Vector3.zero;
            obj.transform.rotation = Quaternion.Euler(Vector3.zero);
        }

        public override void Destroy(MeshRenderer obj)
        {
            base.Destroy(obj);
            Debug.LogFormat("Destroy a cube.");
        }
    }
}
