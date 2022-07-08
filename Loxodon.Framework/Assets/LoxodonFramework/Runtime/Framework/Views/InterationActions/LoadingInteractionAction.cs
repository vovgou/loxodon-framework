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

using Loxodon.Framework.Interactivity;
using System;
using System.Collections.Generic;

namespace Loxodon.Framework.Views.InteractionActions
{
    public class LoadingInteractionAction : InteractionActionBase<VisibilityNotification>
    {
        private List<Loading> list = new List<Loading>();
        public override void Action(VisibilityNotification notification, Action callback)
        {
            try
            {
                if (notification.Visible)
                {
                    Loading loading = Loading.Show(true);
                    if (loading != null)
                        list.Insert(0, loading);
                }
                else
                {
                    if (list.Count <= 0)
                        return;

                    Loading loading = list[0];
                    list.RemoveAt(0);
                    loading.Dispose();
                }
            }
            finally
            {
                callback?.Invoke();
            }
        }
    }
}
