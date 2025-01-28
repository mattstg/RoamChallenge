using Controllers;
using UnityEngine;

namespace TerrainSystem
{
    public class Corner : Node
    {
        public override NodeSegementType type => NodeSegementType.Corner;
        public override ControllerActions[] availableSelectedOptions => new ControllerActions[]
        {
            ControllerActions.ModWidth,
            ControllerActions.ModHeight,
            ControllerActions.CycleTexture,
            ControllerActions.ToggleVisible,
            ControllerActions.Move,
            ControllerActions.Branch,
            ControllerActions.Group_width,
            ControllerActions.Group_height,
            ControllerActions.Group_texture
        };


        public override void ToggleVisible()
        {
            base.ToggleVisible();
            meshRenderer.enabled = !meshRenderer.enabled;
        }

        public override void Delete()
        {
            base.Delete();
            GameManager.controller.DeleteNode(this);
        }

        public override void Move()
        {
            base.Move();
            GameManager.controller.MoveNode(this);
        }
    }
}
