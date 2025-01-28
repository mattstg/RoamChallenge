using TerrainSystem;
using UnityEngine;

namespace Controllers
{
    public enum ControllerActions
    {
        ConvertToPlatform,
        ConvertToRamp,
        ModWidth,
        ModLength,
        ModHeight,
        CycleTexture,
        Delete,
        Divide,
        ToggleVisible,
        Move,
        Branch,
        Group_width,
        Group_height,
        Group_texture
    }
    public class Controller
    {
        GameManager manager;
        ControlsUI UI => manager.controlsUI;
        public NodeSegement selected { get; private set; }

        public Controller(GameManager manager)
        {
            this.manager = manager;
        }

        public void SetSelected(NodeSegement segement)
        {
            Debug.Log($"Selected: {segement.name}");
            if (selected != null)
            {
                //selected.Deselect();
            }
            selected = segement;
            UI.DisplayControls(segement.availableSelectedOptions);
            //selected.Select();
        }
    }
}
