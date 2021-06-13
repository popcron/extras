using Popcron.Events;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

#if USE_DOUBLE_FOR_RUNNER
using dt = System.Double;
#else
using dt = System.Single;
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
            RenderPipelineManager.beginCameraRendering += OnRender;
        }

        private static void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {

        }

        private static void OnRender(ScriptableRenderContext context, Camera camera)
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
                if (sceneView is not null)
                {
                    validCamera = sceneView.camera == camera;
                }
            }
#endif

            if (validCamera)
            {
                new BeginCameraRendering(context, camera).Dispatch();
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
            dt delta = (dt)(Time.timeAsDouble - lastUpdateTime);
            lastUpdateTime = (dt)Time.timeAsDouble;

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
            dt delta = (dt)(Time.fixedTimeAsDouble - lastFixedUpdateTime);
            lastFixedUpdateTime = (dt)Time.fixedTimeAsDouble;

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
            dt delta = (dt)(Time.timeAsDouble - lastLateUpdateTime);
            lastLateUpdateTime = (dt)Time.timeAsDouble;

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