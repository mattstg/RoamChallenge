using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace TerrainSystem
{
    public class NodeSegement : MonoBehaviour
    {
        public List<NodeSegement> nextSegements = new List<NodeSegement>();
        public List<NodeSegement> previousSegements = new List<NodeSegement>();
        public virtual void Initialize()
        {

        }
    }
}
