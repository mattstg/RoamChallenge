using System.Collections.Generic;
using System.Linq;
using TerrainSystem;
using UnityEngine;
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
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0)) 
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
            controller.Update();
        }

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
        }

        public int segements = 5;
        public float lengthVariation = 0; 
        public float curvyness = .5f; //degree
        public float heightChance = .2f; //% chance it raises or lowers the height
        public GameObject obj;
        public void GenerateMap(int segements, float curvyness)
        {
            ClearMap();
            List<Vector3> points = mapGenerator.GeneratePoints(segements,lengthVariation, curvyness, heightChance);
            mapGenerator.GenerateMap(points);
        }

        public List<NodeSegement> GetClickedNodeSegments()
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
        }

    }
}
