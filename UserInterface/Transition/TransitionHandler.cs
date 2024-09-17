using System;

namespace Flux.UserInterface
{
    public class TransitionHandler
    {
        private readonly TransitionCollection transitionCollection;

        public TransitionHandler(TransitionCollection transitionCollection)
        {
            this.transitionCollection = transitionCollection;
        }

        public E_TransitionState TransitionState { get; private set; }

        #region Events
        /// <summary> Invoked when transition from OFF to ON starts. </summary>
        public event Action OnTransitionToOnStart;

        /// <summary> Invoked when transition from OFF to ON ends. </summary>
        public event Action OnTransitionToOnEnd;

        /// <summary> Invoked when transition from ON to OFF starts. </summary>
        public event Action OnTransitionToOffStart;

        /// <summary> Invoked when transition from ON to OFF ends. </summary>
        public event Action OnTransitionToOffEnd;

        /// <summary> Invoked once per update while transitioning. </summary>
        public event Action OnTransitionTick;

        /// <summary> Invoked when any transition starts. </summary>
        public event Action OnTransitionStart;

        /// <summary> Invoked when any transition ends. </summary>
        public event Action OnTransitionEnd;
        #endregion

        // Set up
        private bool hasSetUp = false;
        private float transitionDuration;
        private A_Transition[] transitions;

        private bool reverseDirection;
        private float elapsedTime;
        private Action onComplete;

        /// <summary> Forces all transitions to complete instantly. </summary>
        public void ForceCompleteAllTransitions()
        {
            elapsedTime = transitionDuration;
            Update(0);
        }

        private void TrySetUp(Panel panel)
        {
            if (!hasSetUp)
                SetUp(panel);
        }

        /// <summary>
        /// Transitions the <paramref name="panel"/> from OFF to ON.
        /// </summary>
        /// <param name="panel">The panel to target.</param>
        public void TransitionToOn(Panel panel)
        {
            TrySetUp(panel);

            for (int i = 0; i < transitions.Length; i++)
            {
                transitions[i].SetUpTransitionToOn();
            }

            onComplete = null;
            reverseDirection = false;
            panel.gameObject.SetActive(true);

            if (elapsedTime >= transitionDuration)
                elapsedTime = 0;

            TransitionState = E_TransitionState.TransitioningOn;
            OnTransitionToOnStart?.Invoke();
            OnTransitionStart?.Invoke();
        }

        /// <summary>
        /// Transitions the <paramref name="panel"/> from ON to OFF.
        /// </summary>
        /// <param name="panel">The panel to target.</param>
        public void TransitionToOff(Panel panel)
        {
            TrySetUp(panel);

            for (int i = 0; i < transitions.Length; i++)
            {
                transitions[i].SetUpTransitionToOff();
            }

            onComplete = () => { panel.gameObject.SetActive(false); };
            reverseDirection = true;

            if (elapsedTime >= transitionDuration || elapsedTime == 0)
                elapsedTime = transitionDuration;

            TransitionState = E_TransitionState.TransitioningOff;
            OnTransitionToOffStart?.Invoke();
            OnTransitionStart?.Invoke();
        }

        /// <summary>
        /// Updates the current transition(s) by <paramref name="deltaTime"/>.
        /// </summary>
        /// <param name="deltaTime"></param>
        public void Update(float deltaTime)
        {
            // Guard if transition has completed
            if (TransitionState == E_TransitionState.None)
                return;

            // Update elapsed time & transitions
            elapsedTime += reverseDirection ? -deltaTime : deltaTime;
            for (int i = 0; i < transitions.Length; i++)
            {
                transitions[i].Update(elapsedTime);
            }

            OnTransitionTick?.Invoke();

            // Handle transition end
            if (HasFinishedTransition())
            {
                onComplete?.Invoke();

                if (TransitionState == E_TransitionState.TransitioningOn)
                    OnTransitionToOnEnd?.Invoke();
                else if (TransitionState == E_TransitionState.TransitioningOff)
                    OnTransitionToOffEnd?.Invoke();

                OnTransitionEnd?.Invoke();
                TransitionState = E_TransitionState.None;
            }
        }

        /// <returns> true, if transition has finished or not currently transitioning. </returns>
        private bool HasFinishedTransition()
        {
            return TransitionState == E_TransitionState.None || (TransitionState == E_TransitionState.TransitioningOn && elapsedTime >= transitionDuration) || (TransitionState == E_TransitionState.TransitioningOff && elapsedTime <= 0);
        }

        /// <summary> Sets up the transitions for <paramref name="panel"/>. </summary>
        /// <param name="panel"></param>
        private void SetUp(Panel panel)
        {
            var transitionsData = transitionCollection.Transitions;
            transitions = new A_Transition[transitionsData.Length];
            for (int i = 0; i < transitions.Length; i++)
            {
                transitions[i] = transitionsData[i].GetInstance(panel);

                // Set up the transition duration
                var keys = transitionsData[i].TransitionCurve.keys;
                if (keys[^1].time > transitionDuration)
                    transitionDuration = keys[^1].time;
            }

            hasSetUp = true;
        }
    }
}