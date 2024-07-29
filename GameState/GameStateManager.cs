using Flux.Core;
using Flux.Core.Extensions;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Flux.State
{
    public class GameStateManager : A_MonoSingleton<GameStateManager>
    {
        #region serialization
        [field: Tooltip("The state that will be applied on start-up.")]
        [field: SerializeField] public int InitialState { get; set; }

        [field: Tooltip("If true, will log state events to console.")]
        [field: SerializeField] public bool Logging { get; private set; } = true;

        [Tooltip("The states registered with the state manager.")]
        [SerializeField] private GameState[] states;
        #endregion

        #region data
        /// <summary> 
        /// The index (in <see cref="states"/>) of the currently active state.
        /// A negative value indicates no state. 
        /// </summary>
        private int currentStateIndex;

        /// <summary>
        /// The game state that is currently active.
        /// </summary>
        public GameState CurrentState => states.IsValidIndex(currentStateIndex) ? states[currentStateIndex] : null;

        /// <summary> 
        /// The index (in <see cref="states"/>) of the previously active state.
        /// A negative value indicates no state. 
        /// </summary>
        private int previousStateIndex;

        /// <summary>
        /// The game state that was previously active.
        /// </summary>
        public GameState PreviousState => states.IsValidIndex(previousStateIndex) ? states[previousStateIndex] : null;
        #endregion

        protected override void Awake()
        {
            base.Awake();

            currentStateIndex = -1;
            previousStateIndex = -1;

            for (int i = 0; i < states.Length; i++)
            {
                states[i].Awake();
            }

            // try and set state to the InitialState
            if (!TrySetState(InitialState))
            {
                Debug.LogError($"Invalid starting state {InitialState}");
            }
        }

        private void Update()
        {
            CurrentState?.Update();
        }

        /// <summary>
        /// Attempts to get the <paramref name="state"/> at <paramref name="stateIndex"/>.
        /// If <paramref name="stateIndex"/> is invalid, <paramref name="state"/> will be null.
        /// </summary>
        /// <param name="stateIndex"></param>
        /// <param name="state"></param>
        /// <returns>True if <paramref name="stateIndex"/> is valid, otherwise false.</returns>
        public bool TryGetState(int stateIndex, out GameState state)
        {
            if (states.IsValidIndex(stateIndex))
            {
                state = states[stateIndex];
                return true;
            }
            else
            {
                LogInvalidStateIndex(stateIndex);
                state = null;
                return false;
            }
        }

        /// <summary>
        /// Attempts to get the <paramref name="state"/> with <paramref name="name"/>.
        /// If <paramref name="name"/> is invalid, <paramref name="state"/> will be null.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="state"></param>
        /// <returns>True if <paramref name="name"/> is valid, otherwise false.</returns>
        public bool TryGetState(string name, out GameState state)
        {
            if (name.IsNullOrEmpty())
            {
                LogInvalidStateName(name);
                state = null;
                return false;
            }

            for (int i = 0; i < states.Length; i++)
            {
                if (states[i].Name.EqualsIgnoreCase(name))
                {
                    state = states[i];
                    return true;
                }
            }

            LogInvalidStateName(name);
            state = null;
            return false;
        }

        /// <summary>
        /// Attempts to set the <see cref="CurrentState"/> to the state at index 
        /// <paramref name="stateIndex"/> in <see cref="states"/>.
        /// </summary>
        /// <param name="stateIndex"></param>
        /// <returns>True if successful, flase otherwise.</returns>
        public bool TrySetState(int stateIndex)
        {
            if (!states.IsValidIndex(stateIndex))
            {
                LogInvalidStateIndex(stateIndex);
                return false;
            }

            SetState_Internal(stateIndex);
            return true;
        }

        /// <summary>
        /// Attempts to set the <see cref="CurrentState"/> to the state in <see cref="states"/>
        /// with <paramref name="name"/>.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>True if successful, flase otherwise.</returns>
        public bool TrySetState(string name)
        {
            if (name.IsNullOrEmpty())
            {
                LogInvalidStateName(name);
                return false;
            }

            for (int i = 0; i < states.Length; i++)
            {
                if (states[i].Name.EqualsIgnoreCase(name))
                {
                    SetState_Internal(i);
                    return true;
                }
            }

            LogInvalidStateName(name);
            return false;
        }

        /// <summary>
        /// Sets the <see cref="CurrentState"/> to <paramref name="stateIndex"/>
        /// in <see cref="states"/> with NO validation.
        /// </summary>
        /// <param name="stateIndex"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetState_Internal(int stateIndex)
        {
            previousStateIndex = currentStateIndex;
            if (PreviousState != null)
            {
                PreviousState.OnStateChange += MoveNextOnBecomeInactive;
                PreviousState.Deactivate();
            }
            else
            {
                SetCurrentState(stateIndex);
            }

            void MoveNextOnBecomeInactive(GameState.E_State state)
            {
                if (state != GameState.E_State.Inactive)
                    return;

                PreviousState.OnStateChange -= MoveNextOnBecomeInactive;
                SetCurrentState(stateIndex);
            }

            void SetCurrentState(int stateIndex)
            {
                currentStateIndex = stateIndex;
                CurrentState.Activate();
            }
        }

        private void LogInvalidStateName(string name)
        {
            Debug.LogWarning($"Failed to move to state: \"{name}\". Invalid state name.\nValid names: {states.ToStringExt()}");
        }

        private void LogInvalidStateIndex(int index)
        {
            Debug.LogWarning($"Failed to move to state: \"{index}\". Invalid state index.\nValid indices: 0-{states.Length - 1} (inclusive)");
        }
    }
}