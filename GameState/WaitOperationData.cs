using UnityEngine;

namespace Flux.State
{
    [CreateAssetMenu(fileName = "WaitOperation", menuName = "Flux/Game State/Wait Operation")]
    public class WaitOperationData : A_StateOperationData
    {
        [field: Tooltip("Duration of time to wait for.")]
        [field: SerializeField] public float Duration { get; private set; }

        public override A_StateOperation CreateInstance()
        {
            return new WaitOperation(this);
        }
    }

    public class WaitOperation : A_StateOperation
    {
        private readonly WaitOperationData data;

        private float elapsed;

        public WaitOperation(WaitOperationData data)
        {
            this.data = data;
        }

        public override float GetProgress()
        {
            return elapsed / data.Duration;
        }

        public override void Start()
        {
            elapsed = 0;
        }

        public override void Update()
        {
            elapsed += Time.deltaTime;
        }
    }
}