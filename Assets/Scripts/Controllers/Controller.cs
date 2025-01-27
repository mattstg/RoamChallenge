using UnityEngine;

namespace Controllers
{
    public class Controller
    {
        GameManager manager;
        ControlsUI UI => manager.controlsUI;

        public Controller(GameManager manager)
        {
            this.manager = manager;
        }
    }
}
