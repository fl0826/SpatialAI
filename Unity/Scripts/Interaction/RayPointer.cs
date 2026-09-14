using UnityEngine;
using UnityEngine.XR;

namespace SpatialAI.Interaction
{
    /// <summary>
    /// 手柄射线指针
    /// 检测手柄输入并发出射线
    /// </summary>
    public class RayPointer : MonoBehaviour
    {
        [Header("射线设置")]
        [Tooltip("射线长度")]
        public float rayLength = 10f;

        [Tooltip("射线可视化")]
        public LineRenderer lineRenderer;

        [Tooltip("射线颜色")]
        public Color rayColor = Color.cyan;

        [Header("输入设置")]
        [Tooltip("使用右手柄")]
        public bool useRightHand = true;

        private InputDevice targetDevice;
        private bool isInitialized = false;

        void Start()
        {
            InitializeController();
            SetupLineRenderer();
        }

        void InitializeController()
        {
            var inputDevices = new System.Collections.Generic.List<InputDevice>();
            InputDeviceCharacteristics controllerCharacteristics = 
                InputDeviceCharacteristics.Controller | 
                (useRightHand ? InputDeviceCharacteristics.Right : InputDeviceCharacteristics.Left);

            InputDevices.GetDevicesWithCharacteristics(controllerCharacteristics, inputDevices);

            if (inputDevices.Count > 0)
            {
                targetDevice = inputDevices[0];
                isInitialized = true;
                Debug.Log($"手柄已连接: {targetDevice.name}");
            }
            else
            {
                Debug.LogWarning("未找到手柄设备");
            }
        }

        void SetupLineRenderer()
        {
            if (lineRenderer == null)
            {
                lineRenderer = gameObject.AddComponent<LineRenderer>();
            }

            lineRenderer.startWidth = 0.01f;
            lineRenderer.endWidth = 0.01f;
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.startColor = rayColor;
            lineRenderer.endColor = rayColor;
            lineRenderer.positionCount = 2;
        }

        void Update()
        {
            if (!isInitialized)
            {
                InitializeController();
                return;
            }

            UpdateRay();
            CheckTriggerInput();
        }

        void UpdateRay()
        {
            Vector3 startPos = transform.position;
            Vector3 endPos = transform.position + transform.forward * rayLength;

            lineRenderer.SetPosition(0, startPos);
            lineRenderer.SetPosition(1, endPos);
        }

        void CheckTriggerInput()
        {
            if (targetDevice.TryGetFeatureValue(CommonUsages.triggerButton, out bool triggerPressed))
            {
                if (triggerPressed)
                {
                    OnTriggerPressed();
                }
            }
        }

        void OnTriggerPressed()
        {
            Debug.Log("手柄触发器按下");

            Ray ray = new Ray(transform.position, transform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, rayLength))
            {
                Debug.Log($"射线击中: {hit.collider.gameObject.name}");
                
                // 触发选择事件
                var selector = FindObjectOfType<ObjectSelector>();
                if (selector != null)
                {
                    selector.OnObjectSelected(hit.point);
                }
            }
            else
            {
                Debug.Log("射线未击中任何物体，捕获当前视图");
                
                // 即使没有击中，也触发捕获
                var selector = FindObjectOfType<ObjectSelector>();
                if (selector != null)
                {
                    selector.OnObjectSelected(Vector3.zero);
                }
            }
        }

        public Ray GetRay()
        {
            return new Ray(transform.position, transform.forward);
        }
    }
}
