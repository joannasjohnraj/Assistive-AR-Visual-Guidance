using UnityEngine;
using UnityEngine.UI;

namespace AssistiveARVisualGuidance
{
    // Builds the entire prototype from code so the scene stays portable in a fresh 3D Core project.
    public class PrototypeSceneBuilder : MonoBehaviour
    {
        private const float RoomWidth = 12f;
        private const float RoomDepth = 14f;
        private const float RoomHeight = 3f;

        [SerializeField] private bool buildOnStart = true;

        private Material floorMaterial;
        private Material wallMaterial;
        private Material obstacleMaterial;
        private Material targetMaterial;
        private Material gridMaterial;

        private void Start()
        {
            if (buildOnStart)
            {
                BuildPrototype();
            }
        }

        public void BuildPrototype()
        {
            CreateMaterials();
            CreateLighting();

            // Build order matters: UI needs both the player camera and target transform.
            Camera playerCamera = CreatePlayer();
            Transform target = CreateRoomAndTarget();
            CreateSpatialGrid();
            CreateUi(playerCamera, target);
        }

        private void CreateMaterials()
        {
            floorMaterial = CreateMaterial("AR Floor", new Color(0.18f, 0.2f, 0.22f, 1f));
            wallMaterial = CreateMaterial("Soft Clinical Wall", new Color(0.72f, 0.78f, 0.8f, 1f));
            obstacleMaterial = CreateMaterial("Obstacle Blue", new Color(0.15f, 0.32f, 0.55f, 1f));
            targetMaterial = CreateMaterial("Target Green", new Color(0.1f, 0.75f, 0.45f, 1f));

            gridMaterial = new Material(Shader.Find("Sprites/Default"));
            gridMaterial.name = "Transparent AR Grid";
            gridMaterial.color = new Color(0.1f, 0.9f, 1f, 0.35f);
        }

        private Material CreateMaterial(string materialName, Color color)
        {
            Material material = new Material(Shader.Find("Standard"));
            material.name = materialName;
            material.color = color;
            return material;
        }

        private void CreateLighting()
        {
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.55f, 0.58f, 0.6f, 1f);

            GameObject lightObject = new GameObject("Clinical Area Light");
            Light light = lightObject.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.15f;
            light.color = new Color(1f, 0.97f, 0.9f, 1f);
            lightObject.transform.rotation = Quaternion.Euler(50f, -35f, 0f);
        }

        private Camera CreatePlayer()
        {
            GameObject player = new GameObject("First Person AR User");
            player.transform.position = new Vector3(0f, 1f, -5.2f);

            CharacterController controller = player.AddComponent<CharacterController>();
            controller.height = 1.75f;
            controller.radius = 0.28f;
            controller.center = new Vector3(0f, 0.85f, 0f);

            GameObject cameraObject = new GameObject("AR Glasses Camera");
            cameraObject.tag = "MainCamera";
            cameraObject.transform.SetParent(player.transform, false);
            cameraObject.transform.localPosition = new Vector3(0f, 1.58f, 0f);

            Camera camera = cameraObject.AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.Skybox;
            camera.fieldOfView = 68f;
            camera.nearClipPlane = 0.05f;
            cameraObject.AddComponent<AudioListener>();

            FirstPersonController movement = player.AddComponent<FirstPersonController>();
            movement.Initialize(cameraObject.transform);

            return camera;
        }

        private Transform CreateRoomAndTarget()
        {
            GameObject environment = new GameObject("Simulated Clinical Room");

            CreateCube("Floor", environment.transform, new Vector3(0f, -0.05f, 0f), new Vector3(RoomWidth, 0.1f, RoomDepth), floorMaterial);
            CreateCube("Back Wall", environment.transform, new Vector3(0f, RoomHeight * 0.5f, RoomDepth * 0.5f), new Vector3(RoomWidth, RoomHeight, 0.15f), wallMaterial);
            CreateCube("Left Wall", environment.transform, new Vector3(-RoomWidth * 0.5f, RoomHeight * 0.5f, 0f), new Vector3(0.15f, RoomHeight, RoomDepth), wallMaterial);
            CreateCube("Right Wall", environment.transform, new Vector3(RoomWidth * 0.5f, RoomHeight * 0.5f, 0f), new Vector3(0.15f, RoomHeight, RoomDepth), wallMaterial);
            CreateCube("Front Low Boundary", environment.transform, new Vector3(0f, 0.5f, -RoomDepth * 0.5f), new Vector3(RoomWidth, 1f, 0.15f), wallMaterial);

            CreateCube("Imaging Cart Obstacle", environment.transform, new Vector3(-3.1f, 0.65f, -0.6f), new Vector3(1.25f, 1.3f, 1.1f), obstacleMaterial);
            CreateCube("Procedure Table Obstacle", environment.transform, new Vector3(1.3f, 0.45f, 0.9f), new Vector3(2.8f, 0.9f, 1.1f), obstacleMaterial);
            CreateCube("Supply Cabinet Obstacle", environment.transform, new Vector3(3.8f, 0.9f, 3.7f), new Vector3(1.1f, 1.8f, 1.1f), obstacleMaterial);

            GameObject target = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            target.name = "Navigation Target";
            target.transform.SetParent(environment.transform, false);
            target.transform.position = new Vector3(-3.7f, 0.55f, 4.6f);
            target.transform.localScale = new Vector3(0.75f, 0.55f, 0.75f);
            target.GetComponent<Renderer>().material = targetMaterial;

            GameObject beacon = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            beacon.name = "Target Beacon";
            beacon.transform.SetParent(target.transform, false);
            beacon.transform.localPosition = new Vector3(0f, 1.35f, 0f);
            beacon.transform.localScale = Vector3.one * 0.35f;
            beacon.GetComponent<Renderer>().material = targetMaterial;
            Destroy(beacon.GetComponent<Collider>());

            Light targetLight = target.AddComponent<Light>();
            targetLight.type = LightType.Point;
            targetLight.range = 3f;
            targetLight.intensity = 2.5f;
            targetLight.color = new Color(1f, 0.82f, 0.08f, 1f);
            targetLight.enabled = false;

            TargetHighlighter highlighter = target.AddComponent<TargetHighlighter>();
            highlighter.Initialize(target.GetComponent<Renderer>(), targetLight);

            return target.transform;
        }

        private GameObject CreateCube(string name, Transform parent, Vector3 position, Vector3 scale, Material material)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = name;
            cube.transform.SetParent(parent, false);
            cube.transform.position = position;
            cube.transform.localScale = scale;
            cube.GetComponent<Renderer>().material = material;
            return cube;
        }

        private void CreateSpatialGrid()
        {
            GameObject gridObject = new GameObject("Toggleable Spatial Grid Overlay");
            SpatialGridOverlay grid = gridObject.AddComponent<SpatialGridOverlay>();
            grid.Build(RoomWidth, RoomDepth, RoomHeight, 1f, gridMaterial);
        }

        private void CreateUi(Camera playerCamera, Transform target)
        {
            GameObject canvasObject = new GameObject("AR Guidance HUD");
            Canvas canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
            scaler.scaleFactor = 1f;
            scaler.referencePixelsPerUnit = 100f;
            canvasObject.AddComponent<GraphicRaycaster>();

            // Built-in UI Text avoids TextMeshPro setup prompts and external dependencies.
            Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

            Text title = CreateText("Prototype Title", canvas.transform, font, "Assistive AR Visual Guidance Prototype", 28, TextAnchor.UpperCenter, new Color(1f, 1f, 1f, 0.9f));
            RectTransform titleRect = title.rectTransform;
            titleRect.anchorMin = new Vector2(0.5f, 1f);
            titleRect.anchorMax = new Vector2(0.5f, 1f);
            titleRect.pivot = new Vector2(0.5f, 1f);
            titleRect.anchoredPosition = new Vector2(0f, -26f);
            titleRect.sizeDelta = new Vector2(760f, 42f);

            Text arrowText = CreateText("Direction Arrow", canvas.transform, font, "▲", 64, TextAnchor.MiddleCenter, new Color(0.1f, 0.95f, 1f, 0.95f));
            RectTransform arrowRect = arrowText.rectTransform;
            arrowRect.anchorMin = new Vector2(0.5f, 0.5f);
            arrowRect.anchorMax = new Vector2(0.5f, 0.5f);
            arrowRect.anchoredPosition = new Vector2(0f, 150f);
            arrowRect.sizeDelta = new Vector2(130f, 130f);

            Text distance = CreateText("Target Distance", canvas.transform, font, "TARGET", 32, TextAnchor.MiddleCenter, Color.white);
            RectTransform distanceRect = distance.rectTransform;
            distanceRect.anchorMin = new Vector2(0.5f, 0.5f);
            distanceRect.anchorMax = new Vector2(0.5f, 0.5f);
            distanceRect.anchoredPosition = new Vector2(0f, 78f);
            distanceRect.sizeDelta = new Vector2(420f, 52f);

            Text fps = CreateText("FPS Counter", canvas.transform, font, "FPS", 30, TextAnchor.UpperLeft, new Color(0.92f, 1f, 0.92f, 1f));
            RectTransform fpsRect = fps.rectTransform;
            fpsRect.anchorMin = new Vector2(0f, 1f);
            fpsRect.anchorMax = new Vector2(0f, 1f);
            fpsRect.pivot = new Vector2(0f, 1f);
            fpsRect.anchoredPosition = new Vector2(28f, -28f);
            fpsRect.sizeDelta = new Vector2(220f, 48f);

            Text controls = CreateText("Controls", canvas.transform, font, "WASD move  |  Mouse look  |  G grid  |  H highlight  |  Esc unlock", 28, TextAnchor.LowerCenter, new Color(1f, 1f, 1f, 0.82f));
            RectTransform controlsRect = controls.rectTransform;
            controlsRect.anchorMin = new Vector2(0.5f, 0f);
            controlsRect.anchorMax = new Vector2(0.5f, 0f);
            controlsRect.pivot = new Vector2(0.5f, 0f);
            controlsRect.anchoredPosition = new Vector2(0f, 30f);
            controlsRect.sizeDelta = new Vector2(980f, 48f);

            DirectionalTargetIndicator indicator = canvasObject.AddComponent<DirectionalTargetIndicator>();
            indicator.Initialize(playerCamera, target, arrowRect, distance);

            FpsCounter fpsCounter = canvasObject.AddComponent<FpsCounter>();
            fpsCounter.Initialize(fps);
        }

        private Text CreateText(string name, Transform parent, Font font, string value, int size, TextAnchor alignment, Color color)
        {
            GameObject textObject = new GameObject(name);
            textObject.transform.SetParent(parent, false);

            Text text = textObject.AddComponent<Text>();
            text.font = font;
            text.text = value;
            text.fontSize = size;
            text.alignment = alignment;
            text.color = color;
            text.raycastTarget = false;

            return text;
        }
    }
}
