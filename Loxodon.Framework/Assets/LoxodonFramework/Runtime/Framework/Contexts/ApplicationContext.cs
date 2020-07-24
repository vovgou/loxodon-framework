/*
 * MIT License
 *
 * Copyright (c) 2018 Clark Yang
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of 
 * this software and associated documentation files (the "Software"), to deal in 
 * the Software without restriction, including without limitation the rights to 
 * use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies 
 * of the Software, and to permit persons to whom the Software is furnished to do so, 
 * subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all 
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE 
 * SOFTWARE.
 */

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
