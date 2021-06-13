using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

#if USE_DOUBLE_FOR_RUNNER
using dt = System.Double;
#else
using dt = System.Single;
#endif

namespace Popcron.Extras
{
    /// <summary>
    /// Base class for all custom components to inherit from.
    /// </summary>
    [AddComponentMenu("")]
    public abstract partial class PopBehaviour : MonoBehaviour
    {
        /// <summary>
        /// List of all mono behaviours in the scene.
        /// </summary>
        public static List<PopBehaviour> All { get; private set; } = new List<PopBehaviour>();

        public static OnUpdateDelegate OnUpdateEvent { get; set; }
        public static OnFixedUpdateDelegate OnFixedUpdateEvent { get; set; }
        public static OnLateUpdateDelegate OnLateUpdateEvent { get; set; }

        public delegate void OnUpdateDelegate(dt delta);
        public delegate void OnFixedUpdateDelegate(dt delta);
        public delegate void OnLateUpdateDelegate(dt delta);

        [NonSerialized]
        private PopType cachedType;

        private void Awake() => OnAwake();
        private void Start() => OnStart();
        private void OnDestroy() => OnDestroyed();

        private void OnEnable()
        {
            All.Add(this);
            OnEnabled();
        }

        private void OnDisable()
        {
            All.Remove(this);
            OnDisabled();
        }

        public new PopType GetType()
        {
            if (ReferenceEquals(cachedType, null))
            {
                cachedType = base.GetType();
            }

            return cachedType;
        }

        protected virtual void OnAwake() { }
        protected virtual void OnStart() { }
        public virtual void OnEvent(ref IGameEvent gameEvent) { }
        protected virtual void OnEnabled() { }
        protected virtual void OnDisabled() { }
        public virtual void OnFixedUpdate(dt delta) { }
        public virtual void OnLateUpdate(dt delta) { }
        public virtual void OnUpdate(dt delta) { }
        public virtual void OnGUI() { }
        public virtual void OnDestroyed() { }

        public new T GetComponent<T>()
        {
            if (Application.isPlaying && typeof(PopBehaviour).IsAssignableFrom(typeof(T)))
            {
                int count = All.Count;
                for (int i = 0; i < count; i++)
                {
                    PopBehaviour behaviour = All[i];
                    if (behaviour is T t && behaviour.gameObject == gameObject)
                    {
                        return t;
                    }
                }
            }

            return base.GetComponent<T>();
        }

        public T GetOrAddComponent<T>() where T : Component
        {
            T t = GetComponent<T>();
            if (t is null)
            {
                t = gameObject.AddComponent<T>();
            }

            return t;
        }

        public new List<T> GetComponents<T>()
        {
            List<T> list = new List<T>();
            if (Application.isPlaying && typeof(PopBehaviour).IsAssignableFrom(typeof(T)))
            {
                int count = All.Count;
                for (int i = 0; i < count; i++)
                {
                    PopBehaviour behaviour = All[i];
                    if (behaviour is T t && behaviour.gameObject == gameObject)
                    {
                        list.Add(t);
                    }
                }
            }
            else
            {
                list.AddRange(base.GetComponents<T>());
            }

            return list;
        }

        public static T GetFirst<T>()
        {
            int count = All.Count;
            for (int i = 0; i < count; i++)
            {
                if (All[i] is T t)
                {
                    return t;
                }
            }

            return default;
        }

        public static IEnumerable<T> GetAll<T>()
        {
            int count = All.Count;
            for (int i = 0; i < count; i++)
            {
                if (All[i] is T t)
                {
                    yield return t;
                }
            }
        }

        public static new List<T> FindObjectsOfType<T>() where T : Object
        {
            List<T> list = new List<T>();
            if (Application.isPlaying && typeof(PopBehaviour).IsAssignableFrom(typeof(T)))
            {
                int count = All.Count;
                for (int i = 0; i < count; i++)
                {
                    if (All[i] is T t)
                    {
                        list.Add(t);
                    }
                }
            }
            else
            {
                list = Object.FindObjectsOfType<T>().ToList();
            }

            return list;
        }
    }
}