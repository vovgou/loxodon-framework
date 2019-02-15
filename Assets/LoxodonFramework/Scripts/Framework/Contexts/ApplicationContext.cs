using Loxodon.Framework.Services;
using Loxodon.Framework.Execution;
using Loxodon.Framework.Prefs;

namespace Loxodon.Framework.Contexts
{
    /// <summary>
    /// ApplicationContext
    /// </summary>
    public class ApplicationContext : Context
    {
        private IMainLoopExecutor mainLoopExecutor;

        public ApplicationContext() : this(null, null)
        {
        }

        public ApplicationContext(IServiceContainer container, IMainLoopExecutor mainLoopExecutor) : base(container, null)
        {
            this.mainLoopExecutor = mainLoopExecutor;
            if (this.mainLoopExecutor == null)
                this.mainLoopExecutor = new MainLoopExecutor();
        }

        /// <summary>
        /// Retrieve a executor on the main thread.
        /// </summary>
        /// <returns></returns>
        public virtual IMainLoopExecutor GetMainLoopExcutor()
        {
            return mainLoopExecutor;
        }

        /// <summary>
        /// Retrieve a global preferences.
        /// </summary>
        /// <returns></returns>
        public virtual Preferences GetGlobalPreferences()
        {
            return Preferences.GetGlobalPreferences();
        }

        /// <summary>
        /// Retrieve a user's preferences.
        /// </summary>
        /// <param name="name">The name of the preferences to retrieve.eg:username or username@zone</param>
        /// <returns></returns>
        public virtual Preferences GetUserPreferences(string name)
        {
            return Preferences.GetPreferences(name);
        }
    }
}
