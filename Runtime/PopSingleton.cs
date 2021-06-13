using UnityEngine;

namespace Popcron.Extras
{
    public class PopSingleton : PopBehaviour
    {

    }

    public class PopSingleton<T> : PopSingleton where T : PopSingleton
    {
        private static T instance;

        public static T Instance
        {
            get
            {
                if (!instance)
                {
                    instance = FindObjectOfType<T>();
                    if (!instance)
                    {
                        instance = new GameObject(typeof(T).Name).AddComponent<T>();
                    }
                }

                return instance;
            }
        }
    }
}
