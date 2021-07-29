using UnityEngine;

namespace Popcron.Events
{
    public struct BeginCameraRendering : IGameEvent
    {
        public Camera camera;

        public BeginCameraRendering(Camera camera)
        {
            this.camera = camera;
        }
    }
}
