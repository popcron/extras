using Popcron.Events;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

#if USE_DOUBLE_FOR_RUNNER
using dt = System.Double;
#else
using dt = System.Single;
using UnityEngine.Experimental.Rendering;
#endif

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Popcron.Extras
{
    [AddComponentMenu("")]
    public class Runner : MonoBehaviour
    {
        private static Runner runner;
        private static Dictionary<PopType, bool> ignorePauseTypes = new Dictionary<PopType, bool>();

        [SerializeField]
        private List<PopBehaviour> all = new List<PopBehaviour>();

        private dt lastUpdateTime;
        private dt lastLateUpdateTime;
        private dt lastFixedUpdateTime;
        private bool isPaused;

        /// <summary>
        /// Is the runner currently paused?
        /// </summary>
        public static bool IsPaused
        {
            get
            {
                if (runner)
                {
                    return runner.isPaused;
                }

                return false;
            }
            set
            {
                if (runner)
                {
                    if (runner.isPaused != value)
                    {
                        runner.isPaused = value;
                        new PauseStateWasChanged(value).Dispatch();
                    }
                }
            }
        }

        private static double UnityTime
        {
            get
            {
#if UNITY_2020_2_OR_NEWER
                return Time.timeAsDouble;
#else
                return Time.time;
#endif
            }
        }

        private static double UnityFixedTime
        {
            get
            {
#if UNITY_2020_2_OR_NEWER
                return Time.fixedTimeAsDouble;
#else
                return Time.fixedTime;
#endif
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Bootstrap()
        {
            Runner[] allRunners = Resources.FindObjectsOfTypeAll<Runner>();
            for (int i = allRunners.Length - 1; i >= 0; i--)
            {
                Destroy(allRunners[i].gameObject);
            }

            runner = new GameObject("Runner").AddComponent<Runner>();
            DontDestroyOnLoad(runner.gameObject);
            Initialize();
        }

#if UNITY_EDITOR
        [UnityEditor.Callbacks.DidReloadScripts]
#endif
        private static void Recompiled()
        {
            Initialize();
        }

        private static void Initialize()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;


#if UNITY_2019_1_OR_NEWER
            RenderPipelineManager.beginCameraRendering += (camera) => OnRender(camera);
#else
            Camera.onPreRender += OnRender;
#endif
        }

        private static void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {

        }

        private static void OnRender(Camera camera)
        {
            bool validCamera = false;
            if (camera == Camera.main)
            {
                validCamera = true;
            }

#if UNITY_EDITOR
            if (!validCamera)
            {
                SceneView sceneView = SceneView.currentDrawingSceneView;
                if (sceneView != null)
                {
                    validCamera = sceneView.camera == camera;
                }
            }
#endif

            if (validCamera)
            {
                new BeginCameraRendering(camera).Dispatch();
            }
        }

        private bool IgnoresPause(PopBehaviour popBehaviour)
        {
            PopType type = popBehaviour.GetType();
            if (!ignorePauseTypes.TryGetValue(type, out bool ignorePause))
            {
                ignorePause = type.GetCustomAttribute<IgnorePauseAttribute>() != null;
                ignorePauseTypes[type] = ignorePause;
            }

            return ignorePause;
        }

        private void Update()
        {
            runner = this;
            dt delta = (dt)(UnityTime - lastUpdateTime);
            lastUpdateTime = (dt)UnityTime;

            for (int i = 0; i < PopBehaviour.All.Count; i++)
            {
                PopBehaviour mb = PopBehaviour.All[i];
                if (isPaused && !IgnoresPause(mb))
                {
                    continue;
                }

                mb.OnUpdate(delta);
            }

            all = PopBehaviour.All;
            PopBehaviour.OnUpdateEvent?.Invoke(delta);
        }

        private void FixedUpdate()
        {
            runner = this;
            dt delta = (dt)(UnityFixedTime - lastFixedUpdateTime);
            lastFixedUpdateTime = (dt)UnityFixedTime;

            for (int i = 0; i < PopBehaviour.All.Count; i++)
            {
                PopBehaviour mb = PopBehaviour.All[i];
                if (isPaused && !IgnoresPause(mb))
                {
                    continue;
                }

                mb.OnFixedUpdate(delta);
            }

            PopBehaviour.OnFixedUpdateEvent?.Invoke(delta);
        }

        private void LateUpdate()
        {
            runner = this;
            dt delta = (dt)(UnityTime - lastLateUpdateTime);
            lastLateUpdateTime = (dt)UnityTime;

            for (int i = 0; i < PopBehaviour.All.Count; i++)
            {
                PopBehaviour mb = PopBehaviour.All[i];
                if (isPaused && !IgnoresPause(mb))
                {
                    continue;
                }

                mb.OnLateUpdate(delta);
            }

            PopBehaviour.OnLateUpdateEvent?.Invoke(delta);
        }
    }
}