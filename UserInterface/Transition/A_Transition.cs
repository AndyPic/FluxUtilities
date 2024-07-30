namespace Flux.UserInterface
{
    public abstract class A_Transition
    {
        protected abstract A_TransitionData BaseData { get; }
        protected readonly Panel panel;

        public A_Transition(Panel panel)
        {
            this.panel = panel;
        }

        /// <summary>
        /// Sets up to transition the <see cref="panel"/> from OFF to ON.
        /// (Note: Transition occurs in <see cref="Update(float)"/>.)
        /// </summary>
        public abstract void SetUpTransitionToOn();

        /// <summary>
        /// Sets up to transition the <see cref="panel"/> from ON to OFF.
        /// (Note: Transition occurs in <see cref="Update(float)"/>.)
        /// </summary>
        public abstract void SetUpTransitionToOff();

        /// <summary>
        /// Updates the transition.
        /// </summary>
        /// <param name="elapsedTime"></param>
        public abstract void Update(float elapsedTime);
    }
}