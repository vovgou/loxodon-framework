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

using UnityEngine;
using System.Collections;

using Loxodon.Framework.Views;
using Loxodon.Framework.Views.Animations;

namespace Loxodon.Framework.Examples
{
	public class AlphaAnimation : UIAnimation
	{
		[Range (0f, 1f)]
		public float from = 1f;
		[Range (0f, 1f)]
		public float to = 1f;

		public float duration = 2f;

		private IUIView view;

		void OnEnable ()
		{
			this.view = this.GetComponent<IUIView> ();
			switch (this.AnimationType) {
			case AnimationType.EnterAnimation:
				this.view.EnterAnimation = this;
				break;
			case AnimationType.ExitAnimation:
				this.view.ExitAnimation = this;
				break;
			case AnimationType.ActivationAnimation:
				if (this.view is IWindowView)
					(this.view as IWindowView).ActivationAnimation = this;
				break;
			case AnimationType.PassivationAnimation:
				if (this.view is IWindowView)
					(this.view as IWindowView).PassivationAnimation = this;
				break;
			}

			if (this.AnimationType == AnimationType.ActivationAnimation || this.AnimationType == AnimationType.EnterAnimation) {
				this.view.CanvasGroup.alpha = from;
			}
		}

		public override IAnimation Play ()
		{
//		//use the DoTween
//		this.view.CanvasGroup.DOFade (this.to, this.duration).OnStart (this.OnStart).OnComplete (this.OnEnd).Play ();		

			this.StartCoroutine (DoPlay ());
			return this;
		}

		IEnumerator DoPlay ()
		{
			this.OnStart ();

			var delta = (to - from) / duration;
			var alpha = from;
			this.view.Alpha = alpha;
			if (delta > 0f) {
				while (alpha < to) {
					alpha += delta * Time.deltaTime;
					if (alpha > to) {
						alpha = to;
					}
					this.view.Alpha = alpha;
					yield return null;
				}
			} else {
				while (alpha > to) {
					alpha += delta * Time.deltaTime;
					if (alpha < to) {
						alpha = to;
					}
					this.view.Alpha = alpha;
					yield return null;
				}
			}

			this.OnEnd ();
		}

	}
}
