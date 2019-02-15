using System;
using UnityEngine;

namespace Loxodon.Framework.Views
{
    public class View : MonoBehaviour, IView
    {
        [NonSerialized]
        private IAttributes attributes = new Attributes();

        public virtual string Name
        {
            get { return this.gameObject != null ? this.gameObject.name : null; }
            set
            {
                if (this.gameObject == null)
                    return;

                this.gameObject.name = value;
            }
        }

        public virtual Transform Parent
        {
            get { return this.transform != null ? this.transform.parent : null; }
        }

        public virtual GameObject Owner
        {
            get { return this.gameObject; }
        }

        public virtual Transform Transform
        {
            get { return this.transform; }
        }

        public virtual bool Visibility
        {
            get { return this.gameObject != null ? this.gameObject.activeSelf : false; }
            set
            {
                if (this.gameObject == null)
                    return;
                if (this.gameObject.activeSelf == value)
                    return;

                this.gameObject.SetActive(value);
            }
        }

        protected virtual void OnEnable()
        {
            this.OnVisibilityChanged();
        }

        protected virtual void OnDisable()
        {
            this.OnVisibilityChanged();
        }

        public virtual IAttributes ExtraAttributes { get { return this.attributes; } }       

        protected virtual void OnVisibilityChanged()
        {
        }
    }
}
