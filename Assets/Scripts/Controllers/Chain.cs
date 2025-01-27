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
        }
    }
}
