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
        protected MeshRenderer meshRenderer;
        public virtual void Initialize()
        {
            meshRenderer = GetComponentInChildren<MeshRenderer>();
        }
        
        public virtual void ConvertToPlatform()
        {
        }

        public virtual void ConvertToRamp()
        {
        }

        public virtual void ModWidth(bool increase)
        {
        }

        public virtual void ModLength(bool increase)
        {
        }
        public virtual void ModHeight(bool increase) 
        {
        }

        int currentTexture = 0;
        public virtual void CycleTexture(bool increase)
        {
            try
            {
                if (increase)
                    currentTexture = currentTexture.IncreaseLoopingValue(GameManager.Instance.materials.Length);
                else
                    currentTexture = currentTexture.DecreaseLoopingValue(GameManager.Instance.materials.Length);
                meshRenderer.material = GameManager.Instance.materials[currentTexture];
            }
            catch (Exception e)
            {
                Debug.LogError("Error in : " + transform.name + e.Message);
            }
        }

        public virtual void Delete()
        {
        }

        public virtual void Divide()
        {
        }

        public virtual void ToggleVisible()
        {
        }

        public virtual void Move()
        {
        }

        public virtual void Branch()
        {
        }

        public virtual void Group_width(bool increase)
        {
        }

        public virtual void Group_height(bool increase)
        {
        }

        public virtual void Group_texture(bool increase)
        {
            GameManager.controller.Group_texture(increase, this);
        }
    }
}
