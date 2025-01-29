using System.Collections.Generic;
using System.Linq;
using TerrainSystem;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
namespace Controllers
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance; //lazy singleton
        public ControlsUI controlsUI;
        public Chain chain;
        public WarningController warningController;
        public EndPoint startPt;
        public EndPoint endPt;
        public Material[] materials;
        public Material glowMat;
        public TMPro.TextMeshProUGUI warningText;
        public FreeCamera freeCamera;
        public PlaneGenerator planeGenerator;
        public MeshManipulation meshManipulation;
        

        public static Controller controller; //Should be singleton, but quick prototype dirty code
        MapGenerator mapGenerator;
        void Start()
        {
            Instance = this;
            startPt.Initialize();
            endPt.Initialize();
            Factory.SetupFactory(chain.transform);
            mapGenerator = new MapGenerator(this);
            controller = new Controller(this);
            controlsUI.SetupUIDict();
            planeGenerator.GeneratePlane();
            meshManipulation.LinkMesh();
        }

        float warningTimer;
        public void DisplayWarning(string text)
        {
            warningTimer = 3;
            warningText.text = text;
            warningText.gameObject.SetActive(true);
        }

        void Update()
        {
            if(warningTimer < 0)
                warningText.gameObject.SetActive(false);

            if(Input.GetKeyDown(KeyCode.V))
            {
                meshManipulation.ClampMeshHeightForBuildings(Controllers.GameManager.Instance.chain.GetAllColliders());
            }

            if(Input.GetKeyDown(KeyCode.T))
            {
                //;
                planeGenerator.PerlinTheGrid();
                meshManipulation.ClampMeshHeightForBuildings(Controllers.GameManager.Instance.chain.GetAllColliders());
            }

            if(Input.GetKeyDown(KeyCode.C))
            {
                freeCamera.enabled = !freeCamera.enabled;
                if (freeCamera.enabled)
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }
                else
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }
            }

            if(Input.GetKeyDown(KeyCode.G))
            {
                segements = int.Parse(segementsInput.text);
                lengthVariation = float.Parse(lengthVariationInput.text);
                curvyness = float.Parse(curvynessInput.text);
                heightChance = float.Parse(heightChanceInput.text);
                GenerateMap();                
            }

            if (Input.GetMouseButtonDown(0) && !Input.GetKey(KeyCode.LeftAlt)) 
            {
                List<NodeSegement> clickedSegments = GetClickedNodeSegments();
                if (clickedSegments.Count > 0)
                {
                    if(controller.selected == clickedSegments[0])
                    {
                        if(clickedSegments.Count > 1)
                            controller.SetSelected(clickedSegments[1]);
                    }
                    else
                    {
                        controller.SetSelected(clickedSegments[0]);
                    }
                }
                else
                    controller.SetSelected(null);
            }

            if (Input.GetMouseButton(0) && controller.selected != null && controller.selected is Node corner && !Input.GetKey(KeyCode.LeftAlt))
            {
                moveTimer += Time.deltaTime;
                if(moveModeActive)
                {
                    MoveModeUpdate();
                }
                else if (moveTimer > holdMoveTimer)
                {
                    MoveModeActivated();
                }
            }
            else
            {
                moveTimer = 0;
                if (moveModeActive)
                    MoveModeDeactivate();
            }

            controller.Update();
        }
        #region Move Mode
        const float holdMoveTimer = .2f;
        float moveTimer = 0;
        bool moveModeActive = false;
        public float MOVE_SENSITIVITY = .25f;
        void MoveModeActivated()
        {
            if (Cursor.lockState != CursorLockMode.Locked && Cursor.visible)
            {
                Cursor.visible = false;
                moveModeActive = true;
            }
        }

        void MoveModeUpdate()
        {
            Node n = controller.selected as Node;
            if (n == null)
            {
                MoveModeDeactivate();
                return;
            }

            Vector2 mouseDelta = Input.mousePositionDelta * MOVE_SENSITIVITY;

            // Get the camera's forward and right vectors
            Vector3 cameraForward = Camera.main.transform.forward;
            Vector3 cameraRight = Camera.main.transform.right;

            // Ignore the vertical (Y-axis) component for ground movement
            cameraForward.y = 0;
            cameraRight.y = 0;

            // Normalize the vectors to ensure consistent movement
            cameraForward.Normalize();
            cameraRight.Normalize();

            // Calculate movement relative to the camera's orientation
            Vector3 moveDirection = (cameraRight * mouseDelta.x + cameraForward * mouseDelta.y);
            //n.transform.position += moveDirection;
            // Apply movement to the object's position
            n.ExternalMove(moveDirection);
        }
        void MoveModeDeactivate()
        {
            Cursor.visible = true;
            moveModeActive = false;
        }

        #endregion

        [ExposeMethodInEditor()]
        public void ClearMap()
        {
            chain.ClearMap();
        }

        [ExposeMethodInEditor()]
        public void GenerateMap()
        {
            ClearMap();
            GenerateMap(segements, curvyness);
            chain.BakeChain();
            Invoke("DelayCut", .1f);
        }

        void DelayCut()
        {
            meshManipulation.ClampMeshHeightForBuildings(Controllers.GameManager.Instance.chain.GetAllColliders());
        }

        public int segements = 5;
        public float lengthVariation = 0; 
        public float curvyness = .5f; //degree
        public float heightChance = .2f; //% chance it raises or lowers the height
        public GameObject obj;
        public TMPro.TMP_InputField segementsInput;
        public TMPro.TMP_InputField lengthVariationInput;
        public TMPro.TMP_InputField curvynessInput;
        public TMPro.TMP_InputField heightChanceInput;
        public void GenerateMap(int segements, float curvyness)
        {
            ClearMap();
            List<Vector3> points = mapGenerator.GeneratePoints(segements,lengthVariation, curvyness, heightChance);
            mapGenerator.GenerateMap(points);
        }

        /*public static List<NodeSegement> GetClickedNodeSegments()
        {
            // Ensure the main camera is available
            Camera mainCamera = Camera.main;
            if (mainCamera == null)
            {
                Debug.LogError("No main camera found in the scene.");
                return new List<NodeSegement>();
            }

            // Cast a ray from the mouse position
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits = Physics.RaycastAll(ray);

            // Filter hits by objects with NodeSegement component
            List<NodeSegement> nodeSegments = hits
                .Select(hit => hit.collider.GetComponentInParent<NodeSegement>())
                .Where(segment => segment != null)
                .OrderBy(segment => segment.type) // Sort by NodeSegementType
                .ToList();

            return nodeSegments;
        }*/

        public static float DistanceThreshold = 1f;
        public static List<NodeSegement> GetClickedNodeSegments()
        {
            // Ensure the main camera is available
            Camera mainCamera = Camera.main;
            if (mainCamera == null)
            {
                Debug.LogError("No main camera found in the scene.");
                return new List<NodeSegement>();
            }

            // Cast a ray from the mouse position
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits = Physics.RaycastAll(ray);

            if (hits.Length == 0)
            {
                return new List<NodeSegement>();
            }

            // Find the first hit point
            RaycastHit firstHit = hits.OrderBy(hit => hit.distance).First();
            float firstHitDistance = firstHit.distance;

            // Filter hits by distance threshold and NodeSegment presence
            List<NodeSegement> nodeSegments = hits
                .Where(hit => hit.distance <= firstHitDistance + DistanceThreshold) // Check if within threshold
                .Select(hit => hit.collider.GetComponentInParent<NodeSegement>())
                .Where(segment => segment != null) // Exclude null segments
                .OrderBy(segment => segment.type) // Sort by NodeSegementType
                .ToList();

            return nodeSegments;
        }

    }
}
