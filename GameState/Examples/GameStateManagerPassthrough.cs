using UnityEngine;

namespace Flux.State.Example
{
    public class GameStateManagerPassthrough : MonoBehaviour
    {
        public void OnClick_SetState(string stateName)
        {
            GameStateManager.Instance.TrySetState(stateName);
        }

        public void OnClick_SetState(int stateIndex)
        {
            GameStateManager.Instance.TrySetState(stateIndex);
        }
    }
}