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

using Loxodon.Framework.Asynchronous;
using Loxodon.Framework.ViewModels;

namespace Loxodon.Framework.Interactivity
{
    public interface IDialogService
    {
        /// <summary>
        /// Displays information to the user. 
        /// </summary>
        /// <param name="title">The title of the dialog box. This may be null.</param>
        /// <param name="message">The message to be shown to the user.</param>
        /// <returns>The result</returns>
        IAsyncResult<int> ShowDialog(string title, string message);

        /// <summary>
        /// Displays information to the user. 
        /// </summary>
        /// <param name="title">The title of the dialog box. This may be null.</param>
        /// <param name="message">The message to be shown to the user.</param>
        /// <param name="buttonText">The text shown in the only button
        /// in the dialog box. If left null, the button will be invisible.</param>
        /// <returns>The result</returns>
        IAsyncResult<int> ShowDialog(string title, string message, string buttonText);

        /// <summary>
        /// Displays information to the user. 
        /// </summary>
        /// <param name="title">The title of the dialog box. This may be null.</param>
        /// <param name="message">The message to be shown to the user.</param>
        /// <param name="confirmButtonText">The text shown in the "confirm" button
        /// in the dialog box. If left null, the button will be invisible.</param>
        /// <param name="cancelButtonText">The text shown in the "cancel" button
        /// in the dialog box. If left null, the button will be invisible.</param>
        /// <returns>The result</returns>
        IAsyncResult<int> ShowDialog(string title, string message, string confirmButtonText, string cancelButtonText);

        /// <summary>
        /// Displays information to the user. 
        /// </summary>
        /// <param name="title">The title of the dialog box. This may be null.</param>
        /// <param name="message">The message to be shown to the user.</param>
        /// <param name="confirmButtonText">The text shown in the "confirm" button
        /// in the dialog box. If left null, the button will be invisible.</param>
        /// <param name="cancelButtonText">The text shown in the "cancel" button
        /// in the dialog box. If left null, the button will be invisible.</param>
        /// <param name="neutralButtonText">The text shown in the "neutral" button
        /// in the dialog box. If left null, the button will be invisible.</param>
        /// <returns>The result</returns>
        IAsyncResult<int> ShowDialog(string title, string message, string confirmButtonText, string cancelButtonText, string neutralButtonText);

        /// <summary>
        /// Displays information to the user. 
        /// </summary>
        /// <param name="title">The title of the dialog box. This may be null.</param>
        /// <param name="message">The message to be shown to the user.</param>
        /// <param name="confirmButtonText">The text shown in the "confirm" button
        /// in the dialog box. If left null, the button will be invisible.</param>
        /// <param name="cancelButtonText">The text shown in the "cancel" button
        /// in the dialog box. If left null, the button will be invisible.</param>
        /// <param name="neutralButtonText">The text shown in the "neutral" button
        /// in the dialog box. If left null, the button will be invisible.</param>       
        /// <param name="canceledOnTouchOutside">Whether the dialog box is canceled when 
        /// touched outside the window's bounds. </param>
        /// <returns>The result</returns>
        IAsyncResult<int> ShowDialog(string title, string message, string confirmButtonText, string cancelButtonText, string neutralButtonText, bool canceledOnTouchOutside);

        /// <summary>
        /// Displays information to the user.
        /// </summary>
        /// <param name="viewName">The name of the dialog view, loading the dialog view based on the view name</param>
        /// <param name="viewModel">The view model of the dialog</param>
        /// <returns>The result</returns>
        IAsyncResult<IViewModel> ShowDialog(string viewName, IViewModel viewModel);

        /// <summary>
        /// Displays information to the user.
        /// </summary>
        /// <param name="viewName">The name of the dialog view, loading the dialog view based on the view name</param>
        /// <param name="viewModel">The view model of the dialog</param>
        /// <returns>The result</returns>
        IAsyncResult<TViewModel> ShowDialog<TViewModel>(string viewName, TViewModel viewModel) where TViewModel : IViewModel;
    }
}
