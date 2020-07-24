using System.IO;
using UnityEngine;
using log4net.Appender;

namespace Loxodon.Log.Log4Net.Appender
{
    public class UnityRollingFileAppender : RollingFileAppender
    {
        public override string File
        {
            set
            {
                string path;
                if (Application.isEditor)
                    path = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
                else
                    path = Path.Combine(Application.temporaryCachePath, "Logs");

                base.File = Path.Combine(path, value); 
            }
        }
    }
}
