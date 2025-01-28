using System;
using System.Collections.Generic;
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
        public NodeSegement selected;

        public Controller(GameManager manager)
        {
            this.manager = manager;
        }

        public void Update()
        {
            VeryGrossMethod(); //calls input and actions on selected segementNode. There is 100% a better way to do this, but this is a throwaway prototype
        }

        public void SetSelected(NodeSegement segement)
        {
            selected = segement;
            if (selected == null)
                UI.NothingSelected();
            else
                UI.DisplayControls(segement.availableSelectedOptions);
            //selected.Select();
        }

        //The smart thing to do would be ethier extract the action map from the NodeSegement classes, or to use reflection mapping on an interface/parent 
        //That would be ultimatly scalable
        //This is a throwaway prototype, so let's do the fast thing
        public void VeryGrossMethod()
        {
            if(selected == null)
                return;

            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                if (Input.GetKeyDown(KeyCode.Plus) || Input.GetKeyDown(KeyCode.KeypadPlus))
                    selected.Group_width(true);
                else if (Input.GetKeyDown(KeyCode.Minus) || Input.GetKeyDown(KeyCode.KeypadMinus))
                    selected.Group_width(false);
                else if (Input.GetKeyDown(KeyCode.LeftBracket))
                    selected.Group_texture(false);
                else if (Input.GetKeyDown(KeyCode.RightBracket))
                    selected.Group_texture(true);
                else if (Input.GetKeyDown(KeyCode.O))
                    selected.Group_height(true);
                else if (Input.GetKeyDown(KeyCode.L))
                    selected.Group_height(false);
            }
            else if (Input.GetKeyDown(KeyCode.P))
                selected.ConvertToPlatform();
            else if (Input.GetKeyDown(KeyCode.R))
                selected.ConvertToRamp();
            else if (Input.GetKeyDown(KeyCode.Plus) || Input.GetKeyDown(KeyCode.KeypadPlus))
            {
                selected.ModLength(true);
                selected.ModWidth(true);
            }
            else if (Input.GetKeyDown(KeyCode.Minus) || Input.GetKeyDown(KeyCode.KeypadMinus))
            {
                selected.ModLength(false);
                selected.ModWidth(false);
            }
            else if (Input.GetKeyDown(KeyCode.O))
                selected.Group_height(true);
            else if (Input.GetKeyDown(KeyCode.L))
                selected.Group_height(false);
            else if (Input.GetKeyDown(KeyCode.LeftBracket))
                selected.Group_texture(false);
            else if (Input.GetKeyDown(KeyCode.RightBracket))
                selected.Group_texture(true);
            else if (Input.GetKeyDown(KeyCode.Delete))
                selected.Delete();
            else if (Input.GetKeyDown(KeyCode.D))
                selected.Divide();
            else if (Input.GetKeyDown(KeyCode.T))
                selected.ToggleVisible();
            else if (Input.GetKeyDown(KeyCode.B))
                selected.Branch();
        }

    
    }


    

}
