using System.Collections.Generic;

namespace Loxodon.Framework.Views
{
    public interface IWindowManager
    {
        bool Activated { get; set; }

        IWindow Current { get; }

        int Count { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerator<IWindow> Visibles();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        IWindow Get(int index);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="window"></param>
        void Add(IWindow window);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="window"></param>
        /// <returns></returns>
        bool Remove(IWindow window);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        IWindow RemoveAt(int index);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="window"></param>
        /// <returns></returns>
        bool Contains(IWindow window);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="window"></param>
        /// <returns></returns>
        int IndexOf(IWindow window);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="visible"></param>
        /// <returns></returns>
        List<IWindow> Find(bool visible);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T Find<T>() where T : IWindow;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        T Find<T>(string name) where T : IWindow;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        List<T> FindAll<T>() where T : IWindow;

        /// <summary>
        /// 
        /// </summary>
        void Clear();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="window"></param>
        /// <returns></returns>
        ITransition Show(IWindow window);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="window"></param>
        /// <param name="dismiss"></param>
        /// <returns></returns>
        ITransition Hide(IWindow window);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="window"></param>
        /// <returns></returns>
        ITransition Dismiss(IWindow window);

    }
}
