#if UNITY_EDITOR
using Loxodon.Framework.Asynchronous;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor;

namespace Loxodon.Framework.Editors
{
    [InitializeOnLoad]
    public class EditorExecutors
    {
        private static bool running = false;
        private static List<Task> pending = new List<Task>();
        private static List<IEnumerator> routines = new List<IEnumerator>();

        static EditorExecutors()
        {
        }

        public static void RunOnCoroutine(Task routine)
        {
            if (routine == null)
                return;

            pending.RemoveAll(r => r.ID == routine.ID);
            pending.Add(routine);

            RegisterUpdate();
        }

        public static void RunOnCoroutine(IEnumerator routine)
        {
            if (routine == null)
                return;

            routines.Add(routine);

            RegisterUpdate();
        }

        private static void RegisterUpdate()
        {
            if (running)
                return;

            if (!EditorApplication.isPlayingOrWillChangePlaymode)
            {
                EditorApplication.update += OnUpdate;
                running = true;
            }
        }

        private static void UnregisterUpdate()
        {
            if (!running)
                return;

            EditorApplication.update -= OnUpdate;
            running = false;
        }

        private static void OnUpdate()
        {
            if (routines.Count <= 0 && pending.Count <= 0)
            {
                UnregisterUpdate();
                return;
            }

            for (int i = routines.Count - 1; i >= 0; i--)
            {
                try
                {
                    var routine = routines[i];
                    if (!routine.MoveNext())
                        routines.RemoveAt(i);
                }
                catch (Exception e)
                {
                    routines.RemoveAt(i);
                    UnityEngine.Debug.LogError(e);
                }
            }

            for (int i = pending.Count - 1; i >= 0; i--)
            {
                var routine = pending[i];
                if (routine == null)
                    continue;

                if (routine.CanExecute())
                {
                    routines.Add(routine);
                    pending.RemoveAt(i);
                }
            }
        }

        private static void DoRunAsync(Action action)
        {
            ThreadPool.QueueUserWorkItem((state) => { action(); });
        }

        public static void RunAsyncNoReturn(Action action)
        {
            DoRunAsync(action);
        }

        public static void RunAsyncNoReturn<T>(Action<T> action, T t)
        {
            DoRunAsync(() => { action(t); });
        }

        public static Asynchronous.IAsyncResult RunAsync(Action action)
        {
            AsyncResult result = new AsyncResult(true);
            DoRunAsync(() =>
            {
                try
                {
                    action();
                    result.SetResult();
                }
                catch (Exception e)
                {
                    result.SetException(e);
                }
            });
            return result;
        }

        public static IAsyncResult<TResult> RunAsync<TResult>(Func<TResult> func)
        {
            AsyncResult<TResult> result = new AsyncResult<TResult>(true);
            DoRunAsync(() =>
            {
                try
                {
                    TResult tr = func();
                    result.SetResult(tr);
                }
                catch (Exception e)
                {
                    result.SetException(e);
                }
            });
            return result;
        }

        public static Asynchronous.IAsyncResult RunAsync(Action<IPromise> action)
        {
            AsyncResult result = new AsyncResult(true);
            DoRunAsync(() =>
            {
                try
                {
                    action(result);
                    if (!result.IsDone)
                        result.SetResult(null);
                }
                catch (Exception e)
                {
                    if (!result.IsDone)
                        result.SetException(e);
                }
            });
            return result;
        }

        public static IProgressResult<TProgress> RunAsync<TProgress>(Action<IProgressPromise<TProgress>> action)
        {
            ProgressResult<TProgress> result = new ProgressResult<TProgress>(true);
            DoRunAsync(() =>
            {
                try
                {
                    action(result);
                    if (!result.IsDone)
                        result.SetResult(null);
                }
                catch (Exception e)
                {
                    if (!result.IsDone)
                        result.SetException(e);
                }
            });
            return result;
        }

        public static IAsyncResult<TResult> RunAsync<TResult>(Action<IPromise<TResult>> action)
        {
            AsyncResult<TResult> result = new AsyncResult<TResult>(true);
            DoRunAsync(() =>
            {
                try
                {
                    action(result);
                    if (!result.IsDone)
                        result.SetResult(null);
                }
                catch (Exception e)
                {
                    if (!result.IsDone)
                        result.SetException(e);
                }
            });
            return result;
        }

        public static IProgressResult<TProgress, TResult> RunAsync<TProgress, TResult>(Action<IProgressPromise<TProgress, TResult>> action)
        {
            ProgressResult<TProgress, TResult> result = new ProgressResult<TProgress, TResult>(true);
            DoRunAsync(() =>
            {
                try
                {
                    action(result);
                    if (!result.IsDone)
                        result.SetResult(null);
                }
                catch (Exception e)
                {
                    if (!result.IsDone)
                        result.SetException(e);
                }
            });
            return result;
        }
    }

    public class Task : IEnumerator
    {
        private int id;
        private int delay;
        private IEnumerator routine;

        private long startTime;

        public Task(int id, IEnumerator routine) : this(id, 0, routine)
        {
        }

        public Task(int id, int delay, IEnumerator routine)
        {
            this.id = id;
            this.delay = delay;
            this.routine = routine;
            this.startTime = System.DateTime.Now.Ticks / 10000;
        }

        public int ID { get { return this.id; } }

        public int Delay { get { return this.delay; } }

        public bool CanExecute()
        {
            return System.DateTime.Now.Ticks / 10000 - startTime > delay;
        }

        public object Current { get { return routine.Current; } }

        public bool MoveNext()
        {
            return routine.MoveNext();
        }

        public void Reset()
        {
            routine.Reset();
        }
    }
}
#endif