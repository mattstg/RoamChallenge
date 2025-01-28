using Controllers;
using UnityEngine;
namespace TerrainSystem
{
    public class Gap : Connector
    {
        public override NodeSegementType type => NodeSegementType.Gap;
        public override ControllerActions[] availableSelectedOptions => new ControllerActions[]
        {
            ControllerActions.ConvertToPlatform,
            ControllerActions.ConvertToRamp,
        };
    }
}
