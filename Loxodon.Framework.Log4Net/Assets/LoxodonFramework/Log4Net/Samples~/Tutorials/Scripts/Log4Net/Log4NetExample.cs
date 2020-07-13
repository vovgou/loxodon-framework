using UnityEngine;
using Loxodon.Log;

namespace Loxodon.Framework.Tutorials
{
    public class Log4NetExample : MonoBehaviour
    {
        private ILog log;

        void Start()
        {
            log = LogManager.GetLogger(typeof(Log4NetExample));
        }


        void Update()
        {
            int r = Random.Range(0, 5);
            switch (r)
            {

                case 0:
                    if (log.IsDebugEnabled)
                        log.Debug("This is a debug test.");
                    break;
                case 1:
                    if (log.IsInfoEnabled)
                        log.Info("This is a info test.");
                    break;
                case 2:
                    if (log.IsWarnEnabled)
                        log.Warn("This is a warn test.");

                    break;
                case 3:
                    if (log.IsErrorEnabled)
                        log.Error(new System.Exception("This is a error test."));
                    break;
                case 4:
                    if (log.IsFatalEnabled)
                        log.Fatal(new System.Exception("This is a fatal test."));
                    break;
            }
        }
    }
}