using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HotFix_Project
{
    public class User
    {
        private string firstName;
        private string lastName;
        private int old;

        public string FirstName
        {
            get { return this.firstName; }
            set { this.firstName = value; }
        }

        public string LastName
        {
            get { return this.lastName; }
            set { this.lastName = value; }
        }

        public int Old
        {
            get { return this.old; }
            set { this.old = value; }
        }

        //public string FirstName
        //{
        //    get { return this.firstName; }
        //    set { this.Set<string>(ref this.firstName, value, "FirstName"); }
        //}

        //public string LastName
        //{
        //    get { return this.lastName; }
        //    set { this.Set<string>(ref this.lastName, value, "LastName"); }
        //}

        //public int Old
        //{
        //    get { return this.old; }
        //    set { this.Set<int>(ref this.old, value, "Old"); }
        //}


        //private readonly object _lock = new object();
        //private PropertyChangedEventHandler propertyChanged;
        //public event PropertyChangedEventHandler PropertyChanged
        //{
        //    add { lock (_lock) { this.propertyChanged += value; } }
        //    remove { lock (_lock) { this.propertyChanged -= value; } }
        //}

        ///// <summary>
        ///// Raises the PropertyChanging event.
        ///// </summary>
        ///// <param name="propertyName">Property name.</param>
        //protected virtual void RaisePropertyChanged(string propertyName = null)
        //{
        //    RaisePropertyChanged(new PropertyChangedEventArgs(propertyName));
        //}

        ///// <summary>
        ///// Raises the PropertyChanging event.
        ///// </summary>
        ///// <param name="eventArgs">Property changed event.</param>
        //protected virtual void RaisePropertyChanged(PropertyChangedEventArgs eventArgs)
        //{
        //    try
        //    {

        //        if (propertyChanged != null)
        //            propertyChanged(this, eventArgs);
        //    }
        //    catch (Exception e)
        //    {
        //        //if (log.IsWarnEnabled)
        //        //    log.WarnFormat("Set property '{0}', raise PropertyChanged failure.Exception:{1}", eventArgs.PropertyName, e);
        //    }
        //}

        ///// <summary>
        ///// Raises the PropertyChanging event.
        ///// </summary>
        ///// <param name="eventArgs"></param>
        //protected virtual void RaisePropertyChanged(params PropertyChangedEventArgs[] eventArgs)
        //{
        //    foreach (var args in eventArgs)
        //    {
        //        try
        //        {

        //            if (propertyChanged != null)
        //                propertyChanged(this, args);
        //        }
        //        catch (Exception e)
        //        {
        //            //if (log.IsWarnEnabled)
        //            //    log.WarnFormat("Set property '{0}', raise PropertyChanged failure.Exception:{1}", args.PropertyName, e);
        //        }
        //    }
        //}

        ///// <summary>
        /////  Set the specified propertyName, field, newValue.
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="field"></param>
        ///// <param name="newValue"></param>
        ///// <param name="propertyName"></param>
        ///// <returns></returns>
        //protected bool Set<T>(ref T field, T newValue, [CallerMemberName] string propertyName = null)
        //{
        //    if (object.Equals(field, newValue))
        //        return false;

        //    field = newValue;
        //    RaisePropertyChanged(propertyName);
        //    return true;
        //}

        ///// <summary>
        /////  Set the specified propertyName, field, newValue.
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="field"></param>
        ///// <param name="newValue"></param>
        ///// <param name="eventArgs"></param>
        ///// <returns></returns>
        //protected bool Set<T>(ref T field, T newValue, PropertyChangedEventArgs eventArgs)
        //{
        //    if (object.Equals(field, newValue))
        //        return false;

        //    field = newValue;
        //    RaisePropertyChanged(eventArgs);
        //    return true;
        //}
    }
}
