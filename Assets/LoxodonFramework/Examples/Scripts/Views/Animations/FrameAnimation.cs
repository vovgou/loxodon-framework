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