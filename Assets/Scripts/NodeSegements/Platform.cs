using Controllers;
using UnityEngine;

namespace TerrainSystem
{
    public class Platform : Connector
    {
        public override NodeSegementType type => NodeSegementType.Platform;
        public override ControllerActions[] availableSelectedOptions => new ControllerActions[]
        {
            ControllerActions.ModWidth,
            ControllerActions.CycleTexture,
            ControllerActions.Delete,
            ControllerActions.Divide,
            ControllerActions.Group_width,
            ControllerActions.Group_height,
            ControllerActions.Group_texture
        };
    }
}
