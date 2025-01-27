using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.GameCenter;

namespace TerrainSystem
{
    public enum NodeSegementType { Gap, Platform, Ramp, EndPt, Corner}
    public static class Factory 
    {
        static readonly float GAP_SIZE = .8f; //So it doesnt overlap for clicking

        static Dictionary<NodeSegementType, GameObject> resourceDict = new Dictionary<NodeSegementType, GameObject>();
        static Transform parent;

        public static void SetupFactory(Transform _parent)
        {
            parent = _parent;
            foreach (var item in System.Enum.GetValues(typeof(NodeSegementType)))
            {
                resourceDict.Add((NodeSegementType)item, Resources.Load<GameObject>("Prefabs/NodeSegement" + item.ToString()));
            };
        }

        public static Gap CreateGap(Vector3 startPos, Vector3 endPos)
        {
            Gap gap = CreateFitConnector(NodeSegementType.Gap, startPos, endPos) as Gap;
            gap.transform.localScale = new Vector3(gap.transform.localScale.x, gap.transform.localScale.y, gap.transform.localScale.z * GAP_SIZE);
            return gap;
        }

        public static Platform CreatePlatform(Vector3 startPos, Vector3 endPos)
        {
            float height = startPos.y;
            Platform platform = CreateFitConnector(NodeSegementType.Platform, startPos, endPos) as Platform;
            platform.transform.localEulerAngles = new Vector3(0, platform.transform.localEulerAngles.y, platform.transform.localEulerAngles.z);
            platform.transform.position = new Vector3(platform.transform.position.x, height, platform.transform.position.z);
            platform.transform.localScale = new Vector3(platform.transform.localScale.x, height, platform.transform.localScale.z);

            return platform;
        }


        static Connector CreateFitConnector(NodeSegementType type, Vector3 startPos, Vector3 endPos)
        {
            // Instantiate the Gap object
            GameObject gapObject = GameObject.Instantiate(resourceDict[type]);

            // Calculate the mid-point between startPos and endPos to position the object
            Vector3 midPoint = (startPos + endPos) / 2;

            // Calculate the direction and length of the gap
            Vector3 direction = (endPos - startPos).normalized;
            float distance = Vector3.Distance(startPos, endPos);

            // Set the position of the gap object
            gapObject.transform.position = midPoint;

            // Set the scale of the gap object
            Vector3 originalScale = gapObject.transform.localScale;
            gapObject.transform.localScale = new Vector3(distance, originalScale.y, originalScale.z);

            // Rotate the gap object to align with the direction from startPos to endPos
            Quaternion rotation = Quaternion.LookRotation(direction);
            gapObject.transform.rotation = rotation;

            Connector toReturn = gapObject.GetComponent<Connector>();
            toReturn.Initialize();
            // Return the Gap component
            return toReturn;
        }
    }
}
