using Loxodon.Framework.Execution;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using UnityEngine.UI;
using Loxodon.Framework.Asynchronous;

namespace HotFix_Project
{
    public class TaskTest
    {

        public static void Run()
        {
            UnityEngine.Debug.LogFormat("aaaaaaaaaaaaaaaaaaaaaaaaa");
            User user = new User();

            try
            {
                //For<Text, string>(v => v.text);
                Task.Run(() =>
                {
                    return user;
                });
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogFormat("eeeeeeeee {0}", e);

            }
        }

        /// <summary>
        ///  测试异步调用，成功
        /// </summary>
        public static async void AsyncRun()
        {
            UnityEngine.Debug.LogFormat("aaaaaaaaaaaaaaaaaaaaaaaaa");

            await Task.Delay(2000);

            var result = Executors.RunAsync(()=> 
            {
                UnityEngine.Debug.LogFormat("ssssssssssssss");
            });

            await result;

            UnityEngine.Debug.LogFormat("bbbbbbbbbbbbbbbbbb");

        }

        public static void For<TTarget, TResult>(Expression<Func<TTarget, TResult>> memberExpression)
        {
            //ILRuntimePathParser parser = new ILRuntimePathParser();

            //string targetName = parser.ParseMemberName(memberExpression);

            //UnityEngine.Debug.LogFormat("targetName:{0}", targetName);

            //this.description.TargetName = targetName;
            //this.description.UpdateTrigger = null;
        }
    }
}
