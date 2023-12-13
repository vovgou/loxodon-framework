using NLog;
using NLog.Config;
using NLog.LayoutRenderers;
using System.IO;
using System.Text;

namespace Loxodon.Log.NLogger.Directories
{
    [LayoutRenderer("persistent-data-path")]
    [AppDomainFixedOutput]
    [ThreadAgnostic]
    public class PersistentDataPathLayoutRenderer : LayoutRenderer
    {
        private static string persistentDataPath;

        /// <summary>
        /// Gets or sets the name of the file to be Path.Combine()'d with the directory name.
        /// </summary>
        /// <docgen category='Advanced Options' order='50' />
        public string File { get; set; }

        /// <summary>
        /// Gets or sets the name of the directory to be Path.Combine()'d with the directory name.
        /// </summary>
        /// <docgen category='Advanced Options' order='50' />
        public string Dir { get; set; }

        /// <inheritdoc/>
        protected override void InitializeLayoutRenderer()
        {
            if (string.IsNullOrEmpty(persistentDataPath))
                persistentDataPath = UnityEngine.Application.persistentDataPath;

            base.InitializeLayoutRenderer();
        }

        /// <inheritdoc/>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            builder.Append(CombinePaths(persistentDataPath, Dir, File));
        }

        internal static string CombinePaths(string path, string dir, string file)
        {
            if (dir != null)
                path = Path.Combine(path, dir);

            if (file != null)
                path = Path.Combine(path, file);

            return path;
        }
    }
}
