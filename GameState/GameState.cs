using Flux.Core.Attributes;
using System;
using UnityEngine;

namespace Flux.State
{
    [Serializable]
    public class GameState
    {
        #region types
        public enum E_State : byte
        {
            Inactive,
            Deactivating,
            Active,
            Activating,
        }
        #endregion

        #region serialization
        [Tooltip("A UNIQUE name to identify the state (case insensitive).")]
        public string Name;

        [Tooltip("The loader to use when this state is activating (eg. a loading screen) Note: this may be empty if no loader is required.")]
        [RequireInterface(typeof(IGameStateLoader))]
        [SerializeField] private MonoBehaviour stateLoader;

        [Tooltip("Collection of operations to be performed while activating.")]
        [SerializeField] private A_StateOperationData[] operationsData;
        #endregion

        #region data
        public E_State State { get; private set; } = E_State.Inactive;
        public event Action<E_State> OnStateChange;

        protected IGameStateLoader loader;

        private A_StateOperation[] operations;
        #endregion

        public void Awake()
        {
            loader = stateLoader == null ? null : stateLoader as IGameStateLoader;
        }

        public void Update()
        {
            if (State == E_State.Activating)
            {
                float progress = 0;
                for (int i = 0; i < operations.Length; i++)
                {
                    operations[i].Update();
                    progress += operations[i].GetProgress();
                }
                progress /= operations.Length;

                loader?.SetProgress(progress);

                if (progress >= 1)
                {
                    State = E_State.Active;
                    loader?.Deactivate();

                    if (GameStateManager.Instance.Logging)
                    {
                        Debug.Log($"State: {Name} <color=green>Activated</color>");
                    }

                    OnStateChange?.Invoke(State);
                }
            }
        }

        public void Activate()
        {
            // prevent from enabling, if not the current active state
            if (GameStateManager.Instance.CurrentState != this)
            {
                return;
            }

            loader?.Activate();
            State = E_State.Activating;

            if (GameStateManager.Instance.Logging)
            {
                Debug.Log($"State: {Name} <color=orange>Activating</color>");
            }

            OnStateChange?.Invoke(State);

            operations = new A_StateOperation[operationsData.Length];
            for (int i = 0; i < operations.Length; i++)
            {
                operations[i] = operationsData[i].CreateInstance();
                operations[i].Start();
            }
        }

        public void Deactivate()
        {
            // prevent from disabling, if not the current active state
            if (GameStateManager.Instance.CurrentState != this)
            {
                return;
            }

            State = E_State.Deactivating;

            if (GameStateManager.Instance.Logging)
            {
                Debug.Log($"State: {Name} <color=orange>Deactivaing</color>");
            }

            OnStateChange?.Invoke(State);

            State = E_State.Inactive;

            if (GameStateManager.Instance.Logging)
            {
                Debug.Log($"State: {Name} <color=red>Deactivated</color>");
            }

            OnStateChange?.Invoke(State);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}