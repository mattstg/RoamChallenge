using UnityEngine;
using TerrainSystem;

namespace Controllers
{
    public class Chain : MonoBehaviour
    {
        public EndPoint start;
        public EndPoint end;

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
        }
    }
}
