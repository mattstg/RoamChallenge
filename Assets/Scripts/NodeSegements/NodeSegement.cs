using Controllers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace TerrainSystem
{
    public class NodeSegement : MonoBehaviour
    {
        public virtual NodeSegementType type { get; }
        public virtual ControllerActions[] availableSelectedOptions { get; }
        public List<NodeSegement> nextSegements = new List<NodeSegement>();
        public List<NodeSegement> previousSegements = new List<NodeSegement>();
        public virtual void Initialize()
        {

        }

    }
}
