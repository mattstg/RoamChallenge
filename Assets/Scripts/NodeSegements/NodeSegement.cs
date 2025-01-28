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
        
        public virtual void ConvertToPlatform()
        {
            Debug.Log("Convert to platform");
        }

        public virtual void ConvertToRamp()
        {
            Debug.Log("Convert to ramp");
        }

        public virtual void ModWidth(bool increase)
        {
            Debug.Log("Mod width");
        }

        public virtual void ModLength(bool increase)
        {
            Debug.Log("Mod length");
        }
        public virtual void ModHeight(bool increase) 
        {
            Debug.Log("Mod height");
        }

        public virtual void CycleTexture(bool increase)
        {
            Debug.Log("Cycle texture");
        }

        public virtual void Delete()
        {
            Debug.Log("Delete");
        }

        public virtual void Divide()
        {
            Debug.Log("Divide");
        }

        public virtual void ToggleVisible()
        {
            Debug.Log("Toggle visible");
        }

        public virtual void Move()
        {
            Debug.Log("Move");
        }

        public virtual void Branch()
        {
            Debug.Log("Branch");
        }

        public virtual void Group_width(bool increase)
        {
            Debug.Log("Group width");
        }

        public virtual void Group_height(bool increase)
        {
            Debug.Log("Group height");
        }

        public virtual void Group_texture(bool increase)
        {
            Debug.Log("Group texture");
        }




    }
}
