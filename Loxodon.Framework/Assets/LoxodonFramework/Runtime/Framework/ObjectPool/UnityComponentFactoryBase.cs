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
    public abstract class UnityComponentFactoryBase<T> : IObjectFactory<T> where T : Component
    {
        public virtual T Create(IObjectPool<T> pool)
        {
            T target = this.Create();
            PooledUnityObject pooledObj = target.gameObject.AddComponent<PooledUnityObject>();
            pooledObj.pool = pool;
            pooledObj.target = target;
            return target;
        }

        protected abstract T Create();

        public abstract void Reset(T obj);

        public virtual void Destroy(T obj)
        {
            Object.Destroy(obj.gameObject);
        }

        public virtual bool Validate(T obj)
        {
            return true;
        }
    }

    class PooledUnityObject : MonoBehaviour, IPooledObject
    {
        internal IObjectPool pool;
        internal object target;

        public void Free()
        {
            if (pool != null)
                pool.Free(this.target);
        }
    }
}
