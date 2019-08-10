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
using System.Collections.Generic;
using UnityEngine.UI;

namespace Loxodon.Framework.Examples
{
	[RequireComponent (typeof(Image))]
	public class FrameAnimation : MonoBehaviour
	{
		private Image image;
		private float deltaTime;
		private int index;
		public List<Sprite> frames;
		public float step = 0.02f;

		void Awake ()
		{
			this.image = this.GetComponent<Image> ();
			this.deltaTime = 0f;
			this.index = 0;
		}

		void Update ()
		{
			if (frames == null || frames.Count == 0)
				return;
		
			deltaTime += Time.deltaTime;
			if (deltaTime > step) {
				deltaTime = 0;
				index++;
				if (index == frames.Count)
					index = 0;
			
				this.image.sprite = frames [index];
				this.image.SetNativeSize ();
			}
		}
	}
}