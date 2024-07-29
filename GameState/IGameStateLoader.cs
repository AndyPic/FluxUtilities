namespace Flux.State
{
    public interface IGameStateLoader
    {
        public float Progress { get; }

        public void Activate();
        public void Deactivate();
        public void SetProgress(float progress);
    }
}