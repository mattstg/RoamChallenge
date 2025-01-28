using Controllers;
using UnityEngine;

namespace TerrainSystem
{
    public class Ramp : Connector
    {
        public override NodeSegementType type => NodeSegementType.Ramp;
        public override ControllerActions[] availableSelectedOptions => new ControllerActions[]
        {
            ControllerActions.ModLength,
            ControllerActions.Delete,
        };
    }
}