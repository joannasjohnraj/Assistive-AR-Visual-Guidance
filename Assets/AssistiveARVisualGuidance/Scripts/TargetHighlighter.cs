using UnityEngine;

namespace AssistiveARVisualGuidance
{
    // Toggles a clear visual emphasis state for the object the user is being guided toward.
    public class TargetHighlighter : MonoBehaviour
    {
        [SerializeField] private KeyCode toggleKey = KeyCode.H;
        [SerializeField] private Renderer targetRenderer;
        [SerializeField] private Light targetLight;

        private Color baseColor;
        private bool highlighted;
        private Material runtimeMaterial;
        private Vector3 baseScale;

        public void Initialize(Renderer rendererToHighlight, Light lightToToggle)
        {
            targetRenderer = rendererToHighlight;
            targetLight = lightToToggle;
        }

        private void Awake()
        {
            baseScale = transform.localScale;

            if (targetRenderer == null)
            {
                targetRenderer = GetComponentInChildren<Renderer>();
            }

            if (targetRenderer != null)
            {
                runtimeMaterial = targetRenderer.material;
                baseColor = runtimeMaterial.color;
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(toggleKey))
            {
                highlighted = !highlighted;
                ApplyHighlight();
            }

            if (highlighted)
            {
                transform.localScale = baseScale * (1f + Mathf.Sin(Time.time * 5f) * 0.04f);
            }
        }

        private void ApplyHighlight()
        {
            if (runtimeMaterial != null)
            {
                runtimeMaterial.color = highlighted ? new Color(1f, 0.85f, 0.1f, 1f) : baseColor;
                runtimeMaterial.SetColor("_EmissionColor", highlighted ? new Color(0.7f, 0.5f, 0.05f, 1f) : Color.black);

                if (highlighted)
                {
                    runtimeMaterial.EnableKeyword("_EMISSION");
                }
                else
                {
                    runtimeMaterial.DisableKeyword("_EMISSION");
                    transform.localScale = baseScale;
                }
            }

            if (targetLight != null)
            {
                targetLight.enabled = highlighted;
            }
        }
    }
}
