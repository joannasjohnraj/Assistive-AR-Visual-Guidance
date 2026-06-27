using UnityEngine;
using UnityEngine.UI;

namespace AssistiveARVisualGuidance
{
    // Lightweight HUD performance readout for quick demo evaluation.
    public class FpsCounter : MonoBehaviour
    {
        [SerializeField] private Text fpsText;
        [SerializeField] private float refreshInterval = 0.25f;

        private int frames;
        private float elapsed;

        public void Initialize(Text targetText)
        {
            fpsText = targetText;
        }

        private void Update()
        {
            frames++;
            elapsed += Time.unscaledDeltaTime;

            if (elapsed < refreshInterval)
            {
                return;
            }

            float fps = frames / elapsed;
            if (fpsText != null)
            {
                fpsText.text = "FPS " + Mathf.RoundToInt(fps);
            }

            frames = 0;
            elapsed = 0f;
        }
    }
}
