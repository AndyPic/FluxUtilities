using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Flux.State.Example
{
    [RequireComponent(typeof(DontDestroyOnLoad))]
    public class LoadingScreen : MonoBehaviour, IGameStateLoader
    {
        [SerializeField] private byte numProgressTextDecimals = 0;
        [SerializeField] private TextMeshProUGUI progressText;
        [SerializeField] private Image progressBar;

        public float Progress { get; private set; }

        public void Deactivate()
        {
            gameObject.SetActive(false);
        }

        public void Activate()
        {
            gameObject.SetActive(true);
        }

        public void SetProgress(float progress)
        {
            Progress = progress;

            if (progressBar != null)
            {
                progressBar.fillAmount = progress;
            }

            if (progressText != null)
            {
                float percent = progress * 100f;
                progressText.text = percent.ToString($"F{numProgressTextDecimals}");
            }
        }
    }
}