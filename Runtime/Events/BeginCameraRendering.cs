using UnityEngine;
using UnityEngine.Rendering;

namespace Popcron.Events
{
    public struct BeginCameraRendering : IGameEvent
    {
        public ScriptableRenderContext context;
        public Camera camera;

        public BeginCameraRendering(ScriptableRenderContext context, Camera camera)
        {
            this.context = context;
            this.camera = camera;
        }
    }

    public struct PauseStateWasChanged : IGameEvent
    {
        public bool paused;

        public PauseStateWasChanged(bool paused)
        {
            this.paused = paused;
        }
    }
}
