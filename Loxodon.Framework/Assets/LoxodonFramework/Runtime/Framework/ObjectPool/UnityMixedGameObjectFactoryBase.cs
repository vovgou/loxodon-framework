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

using UnityEngine;

namespace Loxodon.Framework.ObjectPool
{
    public abstract class UnityMixedGameObjectFactoryBase : IMixedObjectFactory<GameObject>
    {
        public virtual GameObject Create(IMixedObjectPool<GameObject> pool, string typeName)
        {
            GameObject target = this.Create(typeName);
            PooledUnityObject pooledObj = target.gameObject.AddComponent<PooledUnityObject>();
            pooledObj.pool = pool;
            pooledObj.target = target;
            pooledObj.typeName = typeName;
            return target;
        }

        protected abstract GameObject Create(string typeName);

        public abstract void Reset(string typeName, GameObject obj);

        public virtual void Destroy(string typeName, GameObject obj)
        {
            Object.Destroy(obj);
        }

        public virtual bool Validate(string typeName, GameObject obj)
        {
            return true;
        }

        class PooledUnityObject : MonoBehaviour, IPooledObject
        {
            internal IMixedObjectPool<GameObject> pool;
            internal GameObject target;
            internal string typeName;

            public void Free()
            {
                if (pool != null)
                    pool.Free(typeName, target);
            }
        }
    }
}
