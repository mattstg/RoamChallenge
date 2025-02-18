using Controllers;
using System.Collections.Generic;
using UnityEngine;
namespace TerrainSystem
{
    public class EndPoint : Node
    {
        public bool isEnd = false;
        public override NodeSegementType type => NodeSegementType.EndPt;
        public override ControllerActions[] availableSelectedOptions => new ControllerActions[]
        {
            ControllerActions.ModWidth,
            ControllerActions.ModHeight,
            ControllerActions.CycleTexture,
            ControllerActions.Group_width,
            ControllerActions.Group_height,
            ControllerActions.Group_texture
        };

        
        
    }
}
