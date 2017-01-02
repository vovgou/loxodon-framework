using System;
using UnityEngine;
using System.Collections;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;

using Loxodon.Framework.Prefs;

namespace Loxodon.Framework.Tutorials
{
	/// <summary>
	/// Prefs example.
	/// Supported Types:ValueType, Vector2 ,Vector3 ,Vector4,Color,Color32 and  Serializable types
	/// </summary>
	public class PrefsExample : MonoBehaviour
	{

		void Start ()
		{
			BinaryFilePreferencesFactory factory = new BinaryFilePreferencesFactory ();

			/*Custom a ISerializationSurrogate for the type of DateTime. */
			SurrogateSelector selector = factory.Formatter.SurrogateSelector as SurrogateSelector;
			selector.AddSurrogate (typeof(DateTime), new StreamingContext (StreamingContextStates.All), new DateTimeSerializationSurrogate ());

		 
			Preferences.Register (factory);
//		Preferences.Register (new PlayerPrefsPreferencesFactory ()); 
	
			/* This is a global preferences. */
			Preferences prefs = Preferences.GetGlobalPreferences ();
			prefs.SetString ("username", "clark_ya@163.com");
			prefs.SetString ("name", "clark");
			prefs.SetInt ("zone", 5);
			prefs.Save ();

			/* This is a preferences that it's only clark's data in the fifth zone. */
			Preferences userPrefs = Preferences.GetPreferences ("clark@5"); /* username:clark, zone:5 */
			userPrefs.SetObject<Vector3> ("role.logout.map.position", new Vector3 (1f, 2f, 3f));
			userPrefs.SetObject<Vector3> ("role.logout.map.forward", new Vector3 (0f, 0f, 1f));
			userPrefs.SetObject<DateTime> ("role.logout.time", DateTime.Now);
			userPrefs.Save ();

			//-----------------

			Debug.LogFormat ("username:{0}; name:{1}; zone:{2};", prefs.GetString ("username"), prefs.GetString ("name"), prefs.GetInt ("zone"));

			Debug.LogFormat ("position:{0} forward:{1} logout time:{2}", userPrefs.GetObject<Vector3> ("role.logout.map.position"), userPrefs.GetObject<Vector3> ("role.logout.map.forward"), userPrefs.GetObject<DateTime> ("role.logout.time"));

		}
	}

	/// <summary>
	/// Custom a ISerializationSurrogate for the type of DateTime. 
	/// </summary>
	public class DateTimeSerializationSurrogate : ISerializationSurrogate
	{
		public void GetObjectData (object obj, SerializationInfo info, StreamingContext context)
		{
			DateTime v = (DateTime)obj;
			info.AddValue ("ticks", v.Ticks);
		}

		public object SetObjectData (object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
		{		
			long ticks = (long)info.GetValue ("ticks", typeof(long));
			DateTime v = new DateTime (ticks);
			return (object)v;
		}
	}
}