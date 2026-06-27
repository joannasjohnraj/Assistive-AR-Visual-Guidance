using System.Collections.Generic;
using UnityEngine;

namespace AssistiveARVisualGuidance
{
    // Draws a transparent floor grid and corner boundary lines to simulate spatial awareness overlays.
    public class SpatialGridOverlay : MonoBehaviour
    {
        [SerializeField] private KeyCode toggleKey = KeyCode.G;
        [SerializeField] private bool visible = true;

        private readonly List<LineRenderer> lines = new List<LineRenderer>();

        public void Build(float roomWidth, float roomDepth, float roomHeight, float spacing, Material lineMaterial)
        {
            ClearLines();

            float halfWidth = roomWidth * 0.5f;
            float halfDepth = roomDepth * 0.5f;

            // Floor grid lines provide simple depth and distance cues without requiring AR packages.
            for (float x = -halfWidth; x <= halfWidth + 0.01f; x += spacing)
            {
                AddLine(new Vector3(x, 0.04f, -halfDepth), new Vector3(x, 0.04f, halfDepth), lineMaterial);
            }

            for (float z = -halfDepth; z <= halfDepth + 0.01f; z += spacing)
            {
                AddLine(new Vector3(-halfWidth, 0.04f, z), new Vector3(halfWidth, 0.04f, z), lineMaterial);
            }

            AddBoundaryPillar(new Vector3(-halfWidth, 0f, -halfDepth), roomHeight, lineMaterial);
            AddBoundaryPillar(new Vector3(halfWidth, 0f, -halfDepth), roomHeight, lineMaterial);
            AddBoundaryPillar(new Vector3(-halfWidth, 0f, halfDepth), roomHeight, lineMaterial);
            AddBoundaryPillar(new Vector3(halfWidth, 0f, halfDepth), roomHeight, lineMaterial);

            ApplyVisibility();
        }

        private void Update()
        {
            if (Input.GetKeyDown(toggleKey))
            {
                visible = !visible;
                ApplyVisibility();
            }
        }

        private void AddBoundaryPillar(Vector3 basePosition, float height, Material material)
        {
            AddLine(basePosition + Vector3.up * 0.05f, basePosition + Vector3.up * height, material);
        }

        private void AddLine(Vector3 start, Vector3 end, Material material)
        {
            GameObject lineObject = new GameObject("Grid Line");
            lineObject.transform.SetParent(transform, false);

            LineRenderer line = lineObject.AddComponent<LineRenderer>();
            line.useWorldSpace = false;
            line.positionCount = 2;
            line.SetPosition(0, start);
            line.SetPosition(1, end);
            line.startWidth = 0.025f;
            line.endWidth = 0.025f;
            line.material = material;
            line.startColor = new Color(0.1f, 0.9f, 1f, 0.35f);
            line.endColor = new Color(0.1f, 0.9f, 1f, 0.35f);

            lines.Add(line);
        }

        private void ClearLines()
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                Destroy(transform.GetChild(i).gameObject);
            }

            lines.Clear();
        }

        private void ApplyVisibility()
        {
            foreach (LineRenderer line in lines)
            {
                if (line != null)
                {
                    line.enabled = visible;
                }
            }
        }
    }
}
