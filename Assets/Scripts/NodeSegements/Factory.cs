using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.GameCenter;

namespace TerrainSystem
{
    public enum NodeSegementType { EndPt, Corner, Platform, Ramp, Gap }
    public static class Factory 
    {
        public static readonly float GAP_SIZE_REDUCTION = .8f; //So it doesnt overlap for clicking, although we can handle this with raycastall
        public  static readonly float RAMP_GAP_SIZE = .6f;

        static Dictionary<NodeSegementType, GameObject> resourceDict = new Dictionary<NodeSegementType, GameObject>();
        static Transform parent;

        public static void SetupFactory(Transform _parent)
        {
            parent = _parent;
            foreach (var item in System.Enum.GetValues(typeof(NodeSegementType)))
            {
                resourceDict.Add((NodeSegementType)item, Resources.Load<GameObject>("Prefabs/NodeSegement/" + item.ToString()));
            };
        }

        #region Nodes
        public static Corner CreateCorner(Vector3 position, Vector3 size)
        {
            GameObject nodeObject = GameObject.Instantiate(resourceDict[NodeSegementType.Corner], parent);
            nodeObject.transform.position = position;
            size.y = position.y - MapGenerator.WORLD_FLOOR;
            nodeObject.transform.localScale = size;
            nodeObject.transform.parent = parent;
            Corner node = nodeObject.GetComponent<Corner>();
            node.Initialize();
            return node;
        }

        #endregion


        #region Connectors
        public static Gap CreateGap(Vector3 startPos, Vector3 endPos)
        {
            Gap gap = CreateFitConnector(NodeSegementType.Gap, startPos, endPos) as Gap;
            gap.transform.localScale = new Vector3(gap.transform.localScale.x, gap.transform.localScale.y, gap.transform.localScale.z * GAP_SIZE_REDUCTION);
            return gap;
        }

        public static Platform CreatePlatform(Vector3 startPos, Vector3 endPos)
        {
            Platform platform = CreateFitConnector(NodeSegementType.Platform, startPos, endPos) as Platform;
            platform.transform.localEulerAngles = new Vector3(0, platform.transform.localEulerAngles.y, platform.transform.localEulerAngles.z);
            platform.transform.position = new Vector3(platform.transform.position.x, startPos.y, platform.transform.position.z);
            platform.transform.localScale = new Vector3(platform.transform.localScale.x, startPos.y - MapGenerator.WORLD_FLOOR, platform.transform.localScale.z);
            return platform;
        }

        public static Ramp CreateRamp(Vector3 startPos, Vector3 endPos)
        {
            Ramp ramp = CreateFitConnector(NodeSegementType.Ramp, startPos, endPos) as Ramp;
            ramp.transform.localScale = new Vector3(ramp.transform.localScale.x, ramp.transform.localScale.y, ramp.transform.localScale.z * RAMP_GAP_SIZE);
            return ramp;
        }


        static Connector CreateFitConnector(NodeSegementType type, Vector3 startPos, Vector3 endPos)
        {
            // Instantiate the Gap object
            GameObject gapObject = GameObject.Instantiate(resourceDict[type], parent);

            // Calculate the mid-point between startPos and endPos to position the object
            Vector3 midPoint = (startPos + endPos) / 2;

            // Calculate the direction and length of the gap
            Vector3 direction = (endPos - startPos).normalized;
            float distance = Vector3.Distance(startPos, endPos);

            // Set the position of the gap object
            gapObject.transform.position = midPoint;

            // Set the scale of the gap object
            Vector3 originalScale = gapObject.transform.localScale;
            gapObject.transform.localScale = new Vector3(originalScale.x, originalScale.y, distance);

            // Rotate the gap object to align with the direction from startPos to endPos
            Quaternion rotation = Quaternion.LookRotation(direction);
            gapObject.transform.rotation = rotation;

            Connector toReturn = gapObject.GetComponent<Connector>();
            toReturn.Initialize();
            // Return the Gap component
            return toReturn;
        }

        #endregion

    }
}
