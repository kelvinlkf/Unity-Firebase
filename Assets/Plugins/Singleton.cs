using UnityEngine;
using System.Collections;

namespace Framework
{
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T instance = null;

        /// <summary>
        /// Get the instance.
        /// </summary>
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    //Creating the singleton game object
                    GameObject gameObject = new GameObject(typeof(T).ToString());
                    gameObject.AddComponent<T>();
                }
                return instance;
            }
        }

        /// <summary>
        /// Assign instance when game object was Awake.
        /// <para/> WARNING: In order to avoid the missing feature after override the Awake method, the child must call base.Awake().
        /// </summary>
        protected void Awake()
        {
            if (instance != null)
            {
                //Delete for duplicated game object with Singleton class
                //Usually the case will be happen when singleton game object attaached at the scene and return back to same scene again
                Destroy(this.gameObject);
            }
            else
            {
                //Assign singleton instance and mark dont destroy
                instance = gameObject.GetComponent<T>();
                instance.gameObject.name = "~" + instance.gameObject.name;
                DontDestroyOnLoad(instance.gameObject);
            }
        }

        /// <summary>
        /// Destroy the singleton instance.
        /// </summary>
        public void Destroy()
        {
            Destroy(instance.gameObject);
            instance = null;
            //Debug.Log("Singleton destroy");
        }
    }
}
