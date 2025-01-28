using UnityEngine;
using TerrainSystem;
using System.Linq;

namespace Controllers
{
    public class Chain : MonoBehaviour
    {
        public GameManager gameManager;
        EndPoint start => gameManager.startPt;
        EndPoint end => gameManager.endPt;
        

        public void ClearMap()
        {
            Transform[] children = transform.ChildrenArray();
            for (int i = children.Length - 1; i >= 0; i--)
            {
                if(children[i] != start.transform && children[i] != end.transform)
                {
                    Destroy(children[i].gameObject);
                }
            }
            start.previousSegements.Clear();
            start.nextSegements.Clear();
            end.previousSegements.Clear();
            end.nextSegements.Clear();
        }

        public void BakeChain()
        {
            start.previousSegements.Clear();
            start.nextSegements.Clear();
            end.previousSegements.Clear();
            end.nextSegements.Clear();

            NodeSegement[] children = transform.ChildrenArray().CollectionFromForEach(t => t.GetComponent<NodeSegement>()).ToArray();
            
            NodeSegement firstSegement = children[0];
            NodeSegement lastSegement = children[children.Length - 1];

            start.nextSegements.Add(firstSegement);
            firstSegement.previousSegements.Add(start);

            end.previousSegements.Add(lastSegement);
            lastSegement.nextSegements.Add(end);

            for (int i = 0; i < children.Length - 1; i++)
            {
                children[i].nextSegements.Add(children[i + 1]);
                children[i + 1].previousSegements.Add(children[i]);
            }

            FixAllRamps();
        }

        public void FixAllRamps()
        {
            NodeSegement[] children = transform.ChildrenArray().CollectionFromForEach(t => t.GetComponent<NodeSegement>()).ToArray();
            foreach(NodeSegement nodeSegement in children)
            {
                if(nodeSegement.type == NodeSegementType.Ramp)
                {
                    FixRamp(nodeSegement.transform, nodeSegement.previousSegements[0], nodeSegement.nextSegements[0]);
                }
            }
        }

        void FixRamp(Transform ramp, NodeSegement prev, NodeSegement next)
        {
            // Calculate the mid-point between startPos and endPos to position the object
            Vector3 midPoint = (prev.transform.position + next.transform.position) / 2;

            // Calculate the direction and length of the gap
            Vector3 direction = (prev.transform.position - next.transform.position).normalized;
            float distance = Vector3.Distance(next.transform.position, prev.transform.position);

            // Set the position of the gap object
            ramp.transform.position = midPoint;

            // Set the scale of the gap object
            Vector3 originalScale = ramp.transform.localScale;
            ramp.transform.localScale = new Vector3(originalScale.x, originalScale.y, distance);

            // Rotate the gap object to align with the direction from startPos to endPos
            Quaternion rotation = Quaternion.LookRotation(direction);
            ramp.transform.rotation = rotation;

            float gapSize = ramp.transform.localScale.z * Factory.RAMP_GAP_SIZE - (prev.transform.localScale.z / 2) - (next.transform.localScale.z / 2);

            ramp.transform.localScale = new Vector3(ramp.transform.localScale.x, ramp.transform.localScale.y, gapSize);
        }
    }
}
