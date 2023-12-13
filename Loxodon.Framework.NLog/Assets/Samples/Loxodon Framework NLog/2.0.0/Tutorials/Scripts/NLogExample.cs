using Loxodon.Log;
using UnityEngine;
namespace Loxodon.Framework.Tutorials
{
    public class NLogExample : MonoBehaviour
    {
        private ILog log;
        private void Start()
        {
            log = LogManager.GetLogger(typeof(NLogExample));
            log.Debug("This is a debug test.");
        }

        void Update()
        {
            int r = Random.Range(0, 3);
            switch (r)
            {
                case 0:
                    if (log.IsDebugEnabled)
                        log.DebugFormat("This is a debug test.frame count:{0}", Time.frameCount);
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
