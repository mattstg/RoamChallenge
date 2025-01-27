using System.Collections.Generic;
using TerrainSystem;
using UnityEngine;
namespace Controllers
{
    public class GameManager : MonoBehaviour
    {
        public ControlsUI controlsUI;
        public Chain chain;
        public WarningController warningController;
        public EndPoint startPt;
        public EndPoint endPt;

        Controller controller;
        MapGenerator mapGenerator;
        void Start()
        {
            Factory.SetupFactory(chain.transform);
            mapGenerator = new MapGenerator(this);
            controller = new Controller(this);
        }

        void Update()
        {

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
    }
}
