using UnityEngine;

namespace Popcron.Events
{
    public struct PauseStateWasChanged : IGameEvent
    {
        public bool paused;

        public PauseStateWasChanged(bool paused)
        {
            this.paused = paused;
        }
    }
}
