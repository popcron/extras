#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Popcron.Extras.Editor
{
    [InitializeOnLoad]
    public static class EditorMonoBehaviourRunner
    {
        private static List<IEditorObject> editorObjects = new List<IEditorObject>();
        private static List<IEditorObject> alreadyRanObjects = new List<IEditorObject>();
        private static double lastTime;
        private static double fixedTime;

        static EditorMonoBehaviourRunner()
        {
            FindMonoBehaviours();

            EditorApplication.update -= OnUpdate;
            EditorApplication.update += OnUpdate;
            EditorApplication.hierarchyChanged -= OnHierarchyChanged;
            EditorApplication.hierarchyChanged += OnHierarchyChanged;
            EditorApplication.playModeStateChanged -= OnPlayStateChanged;
            EditorApplication.playModeStateChanged += OnPlayStateChanged;
        }

        private static void OnPlayStateChanged(PlayModeStateChange obj)
        {
            FindMonoBehaviours();
        }

        /// <summary>
        /// Finds all MonoBehaviour components that have ExecuteAlways attribute.
        /// </summary>
        private static void FindMonoBehaviours()
        {
            //dont do this if playing
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                return;
            }

            PopBehaviour[] popBehaviours = Object.FindObjectsOfType<PopBehaviour>();
            foreach (PopBehaviour mb in popBehaviours)
            {
                if (mb is IEditorObject editorObject)
                {
                    editorObjects.Add(editorObject);
                }
            }
        }

        private static void OnHierarchyChanged()
        {
            FindMonoBehaviours();
        }

        private static void OnUpdate()
        {
            //dont run if playing
            if (Application.isPlaying)
            {
                return;
            }

            //last time is un-initialized
            if (lastTime == default)
            {
                lastTime = EditorApplication.timeSinceStartup;
                return;
            }

            double delta = Mathf.Clamp((float)(EditorApplication.timeSinceStartup - lastTime), float.Epsilon, 1);

            //remove nulls
            for (int i = editorObjects.Count - 1; i >= 0; i--)
            {
                if (editorObjects[i] is MonoBehaviour mb && !mb)
                {
                    editorObjects.RemoveAt(i);
                }
            }

            lastTime = EditorApplication.timeSinceStartup;
            try
            {
                alreadyRanObjects.Clear();
                foreach (IEditorObject editorObject in editorObjects)
                {
                    if (editorObject is IEditorUpdate update && editorObject.enabled)
                    {
                        if (!alreadyRanObjects.Contains(editorObject))
                        {
                            update.OnUpdate((float)delta);
                            alreadyRanObjects.Add(editorObject);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);  
            }

            try
            {
                double fixedDelta = Time.fixedDeltaTime;
                fixedTime += delta;
                if (fixedTime >= fixedDelta)
                {
                    fixedTime = 0;
                    alreadyRanObjects.Clear();
                    foreach (IEditorObject editorObject in editorObjects)
                    {
                        if (editorObject is IEditorFixedUpdate fixedUpdate && editorObject.enabled)
                        {
                            if (!alreadyRanObjects.Contains(editorObject))
                            {
                                fixedUpdate.OnFixedUpdate((float)fixedDelta);
                                alreadyRanObjects.Add(editorObject);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
    }
}
#endif