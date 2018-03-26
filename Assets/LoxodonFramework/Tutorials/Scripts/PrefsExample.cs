using System;
using UnityEngine;

using Loxodon.Framework.Prefs;

namespace Loxodon.Framework.Tutorials
{
    /// <summary>
    /// Prefs example.
    /// Supported Types:ValueType, Vector2 ,Vector3 ,Vector4,Color,Color32 etc.
    /// </summary>
    public class PrefsExample : MonoBehaviour
    {

        void Start()
        {
            BinaryFilePreferencesFactory factory = new BinaryFilePreferencesFactory();

            /* Custom a ITypeEncoder for the type of CustomData. */
            factory.Serializer.AddTypeEncoder(new CustomDataTypeEncoder());

            Preferences.Register(factory);
            //Preferences.Register (new PlayerPrefsPreferencesFactory ()); 

            /* This is a global preferences. */
            Preferences prefs = Preferences.GetGlobalPreferences();
            prefs.SetString("username", "clark_ya@163.com");
            prefs.SetString("name", "clark");
            prefs.SetInt("zone", 5);
            prefs.Save();

            /* This is a preferences that it's only clark's data in the fifth zone. */
            Preferences userPrefs = Preferences.GetPreferences("clark@5"); /* username:clark, zone:5 */
            userPrefs.SetString("role.name", "clark");
            userPrefs.SetObject("role.logout.map.position", new Vector3(1f, 2f, 3f));
            userPrefs.SetObject("role.logout.map.forward", new Vector3(0f, 0f, 1f));
            userPrefs.SetObject("role.logout.time", DateTime.Now);
            userPrefs.SetObject("test.custom.data", new CustomData("test", "This is a test."));
            userPrefs.Save();

            //-----------------

            Debug.LogFormat("username:{0}; name:{1}; zone:{2};", prefs.GetString("username"), prefs.GetString("name"), prefs.GetInt("zone"));

            Debug.LogFormat("position:{0} forward:{1} logout time:{2}", userPrefs.GetObject<Vector3>("role.logout.map.position"), userPrefs.GetObject<Vector3>("role.logout.map.forward"), userPrefs.GetObject<DateTime>("role.logout.time"));

            Debug.LogFormat("CustomData name:{0}   description:{1}", userPrefs.GetObject<CustomData>("test.custom.data").name, userPrefs.GetObject<CustomData>("test.custom.data").description);

        }
    }

    public struct CustomData
    {
        public CustomData(string name, string description)
        {
            this.name = name;
            this.description = description;
        }

        public string name;
        public string description;
    }

    /// <summary>
    /// Custom a ITypeEncoder for the type of CustomData. 
    /// </summary>
    public class CustomDataTypeEncoder : ITypeEncoder
    {
        private int priority = 0;

        public int Priority
        {
            get { return this.priority; }
            set { this.priority = value; }
        }

        public bool IsSupport(Type type)
        {
            return typeof(CustomData).Equals(type);
        }

        public object Decode(Type type, string value)
        {
            return JsonUtility.FromJson(value, type);
        }

        public string Encode(object value)
        {
            return JsonUtility.ToJson(value);
        }
    }
}