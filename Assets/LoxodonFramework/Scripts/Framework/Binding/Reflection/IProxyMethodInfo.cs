using System;
using System.Reflection;

namespace Loxodon.Framework.Binding.Reflection
{
    public interface IProxyMethodInfo : IProxyMemberInfo
    {
        Type ReturnType { get; }

        ParameterInfo[] Parameters { get; }

        ParameterInfo ReturnParameter { get; }

        object Invoke(object target, params object[] args);
    }

    public interface IProxyFuncInfo<T, TResult> : IProxyMethodInfo
    {
        TResult Invoke(T target);
    }

    public interface IProxyFuncInfo<T, P1, TResult> : IProxyMethodInfo
    {
        TResult Invoke(T target, P1 p1);
    }

    public interface IProxyFuncInfo<T, P1, P2, TResult> : IProxyMethodInfo
    {
        TResult Invoke(T target, P1 p1, P2 p2);
    }

    public interface IProxyFuncInfo<T, P1, P2, P3, TResult> : IProxyMethodInfo
    {
        TResult Invoke(T target, P1 p1, P2 p2, P3 p3);
    }

    public interface IProxyActionInfo<T> : IProxyMethodInfo
    {
        void Invoke(T target);
    }

    public interface IProxyActionInfo<T, P1> : IProxyMethodInfo
    {
        void Invoke(T target, P1 p1);
    }

    public interface IProxyActionInfo<T, P1, P2> : IProxyMethodInfo
    {
        void Invoke(T target, P1 p1, P2 p2);
    }

    public interface IProxyActionInfo<T, P1, P2, P3> : IProxyMethodInfo
    {
        void Invoke(T target, P1 p1, P2 p2, P3 p3);
    }

    public interface IStaticProxyFuncInfo<T, TResult> : IProxyMethodInfo
    {
        TResult Invoke();
    }

    public interface IStaticProxyFuncInfo<T, P1, TResult> : IProxyMethodInfo
    {
        TResult Invoke(P1 p1);
    }

    public interface IStaticProxyFuncInfo<T, P1, P2, TResult> : IProxyMethodInfo
    {
        TResult Invoke(P1 p1, P2 p2);
    }

    public interface IStaticProxyFuncInfo<T, P1, P2, P3, TResult> : IProxyMethodInfo
    {
        TResult Invoke(P1 p1, P2 p2, P3 p3);
    }

    public interface IStaticProxyActionInfo<T> : IProxyMethodInfo
    {
        void Invoke();
    }

    public interface IStaticProxyActionInfo<T, P1> : IProxyMethodInfo
    {
        void Invoke(P1 p1);
    }

    public interface IStaticProxyActionInfo<T, P1, P2> : IProxyMethodInfo
    {
        void Invoke(P1 p1, P2 p2);
    }

    public interface IStaticProxyActionInfo<T, P1, P2, P3> : IProxyMethodInfo
    {
        void Invoke(P1 p1, P2 p2, P3 p3);
    }
}
