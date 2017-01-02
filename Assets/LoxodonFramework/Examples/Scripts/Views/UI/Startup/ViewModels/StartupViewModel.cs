using System;
using System.Collections;
using System.Threading;
using UnityEngine;

using Loxodon.Log;
using Loxodon.Framework.Messaging;
using Loxodon.Framework.Asynchronous;
using Loxodon.Framework.Commands;
using Loxodon.Framework.ViewModels;
using Loxodon.Framework.Localizations;

namespace Loxodon.Framework.Examples
{
	public class StartupViewModel : ViewModelBase
	{
		private static readonly ILog log = LogManager.GetLogger (System.Reflection.MethodBase.GetCurrentMethod ().DeclaringType);

		private ProgressBar progressBar = new ProgressBar ();
		private ICommand command;
		private Localization localization;

		public StartupViewModel () : this (new Messenger ())
		{
		}

		public StartupViewModel (IMessenger messenger) : base (messenger)
		{		
			this.localization = Localization.Current;
		}

		public ProgressBar ProgressBar {
			get{ return this.progressBar; }
		}

		public ICommand Click {
			get{ return this.command; }
			set{ this.command = value; }
		}

		public void OnClick ()
		{
			log.Debug ("onClick");
		}

		/// <summary>
		/// run on the background thread.
		/// </summary>
		/// <param name="promise">Promise.</param>
		protected void DoUnzip (IProgressPromise<float> promise)
		{		
			var progress = 0f;
			while (progress < 1f) {			
				progress += 0.01f;
				promise.UpdateProgress (progress);
				//Thread.Sleep (50);
				Thread.Sleep (30);
			}
			promise.SetResult ();
		}

		/// <summary>
		/// Simulate a unzip task.
		/// </summary>
		public IProgressTask<float> Unzip ()
		{				
			ProgressTask<float> task = new ProgressTask<float> (new Action<IProgressPromise<float>> (DoUnzip));
			task.OnPreExecute (() => {			
				this.progressBar.Enable = true;
				this.ProgressBar.Tip = R.startup_progressbar_tip_unziping;
			}).OnProgressUpdate (progress => {
				this.ProgressBar.Progress = progress;/* update progress */
			}).OnPostExecute (() => {
				this.ProgressBar.Tip = ""; 
			}).OnFinish (() => {			
				this.progressBar.Enable = false;
			});
			return task;
		}


		/// <summary>
		/// run on the main thread.
		/// </summary>
		/// <returns>The check.</returns>
		/// <param name="promise">Promise.</param>
		protected IEnumerator DoLoadScene (IProgressPromise<float> promise)
		{
			ResourceRequest request = Resources.LoadAsync<GameObject> ("Scenes/Jungle");
			while (!request.isDone) {
				promise.UpdateProgress (request.progress);
				yield return null;
			}

			GameObject sceneTemplate = (GameObject)request.asset;
			GameObject.Instantiate (sceneTemplate);
			promise.UpdateProgress (1f);
			promise.SetResult ();
		}

		/// <summary>
		/// Simulate a loading task.
		/// </summary>
		/// <returns>The scene.</returns>
		public IProgressTask<float> LoadScene ()
		{					
			ProgressTask<float> task = new ProgressTask<float> (new Func<IProgressPromise<float>,IEnumerator> (DoLoadScene));
			task.OnPreExecute (() => {			
				this.progressBar.Enable = true;
				this.ProgressBar.Tip = R.startup_progressbar_tip_loading;
			}).OnProgressUpdate (progress => {
				this.ProgressBar.Progress = progress;/* update progress */
			}).OnPostExecute (() => {
				this.ProgressBar.Tip = ""; 
			}).OnFinish (() => {			
				this.progressBar.Enable = false;
			});
			return task;
		}

	}
}