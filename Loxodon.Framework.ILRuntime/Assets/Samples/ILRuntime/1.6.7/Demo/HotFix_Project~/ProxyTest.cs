using Loxodon.Framework.Binding.Reflection;
using Loxodon.Framework.Observables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotFix_Project
{
    public class ProxyTest
    {
        public static void Run()
        {
            Account account = new Account()
            {
                ID = 1,
                Username = "test",
                Password = "test",
                Email = "test@gmail.com",
                Birthday = new DateTime(2000, 3, 3)
            };

            User user = new User()
            {
                FirstName = "Tom"
            };

            try
            {
                IProxyType userProxyType = ProxyFactory.Default.Get(typeof(User));
                UnityEngine.Debug.LogFormat("user.FirstName:{0}", userProxyType.GetProperty("FirstName").GetValue(user));
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogFormat("user exception:{0}", e);
            }

            try
            {
                IProxyType accountProxyType = ProxyFactory.Default.Get(typeof(Account));
                UnityEngine.Debug.LogFormat("account.Username:{0}", accountProxyType.GetProperty("Username").GetValue(account));

            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogFormat("account exception:{0}", e);
            }

            //Test t = new Test()
            //{
            //    Username = "Tom"
            //};

            //try
            //{
            //    IProxyType testProxyType = ProxyFactory.Default.Get(typeof(Test));
            //    UnityEngine.Debug.LogFormat("test.Username:{0}", testProxyType.GetProperty("Username").GetValue(t));
            //}
            //catch (Exception e)
            //{
            //    UnityEngine.Debug.LogFormat("user exception:{0}", e);
            //}

        }


    }
}
