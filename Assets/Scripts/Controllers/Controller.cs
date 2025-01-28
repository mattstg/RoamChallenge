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
        public static float HEIGHT_MOD = 1;
        public static float WIDTH_MOD = .25f;

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
                selected.ModHeight(true);
            else if (Input.GetKeyDown(KeyCode.L))
                selected.ModHeight(false);
            else if (Input.GetKeyDown(KeyCode.LeftBracket))
                selected.CycleTexture(false);
            else if (Input.GetKeyDown(KeyCode.RightBracket))
                selected.CycleTexture(true);
            else if (Input.GetKeyDown(KeyCode.Delete))
                selected.Delete();
            else if (Input.GetKeyDown(KeyCode.D))
                selected.Divide();
            else if (Input.GetKeyDown(KeyCode.T))
                selected.ToggleVisible();
            else if (Input.GetKeyDown(KeyCode.B))
                selected.Branch();
        }

        public void Group_height(bool increase, NodeSegement target)
        {
            List<NodeSegement> connectedNodes = GameManager.controller.GetAllConnectedSegements(target);
            foreach (NodeSegement node in connectedNodes)
            {
                node.ModHeight(increase);
            }

            manager.chain.FixAllRamps();
        }

        public void Group_width(bool increase, NodeSegement target)
        {
            List<NodeSegement> connectedNodes = GameManager.controller.GetAllConnectedSegements(target);
            foreach (NodeSegement node in connectedNodes)
            {
                node.ModWidth(increase);
            }

            manager.chain.FixAllRamps();
        }

        public void Group_texture(bool increase, NodeSegement target)
        {
            List<NodeSegement> connectedNodes = GameManager.controller.GetAllConnectedSegements(target);
            foreach (NodeSegement node in connectedNodes)
            {
                node.CycleTexture(increase);
            }
        }

        public List<NodeSegement> GetAllConnectedSegements(NodeSegement nodeSegement)
        {
            // Validate input
            if (nodeSegement == null)
                throw new ArgumentNullException(nameof(nodeSegement));

            // Valid types for inclusion
            var validTypes = new HashSet<NodeSegementType>
            {
                NodeSegementType.EndPt,
                NodeSegementType.Corner,
                NodeSegementType.Platform
            };

            // Set to track visited nodes and avoid infinite loops
            HashSet<NodeSegement> visited = new HashSet<NodeSegement>();
            List<NodeSegement> result = new List<NodeSegement>();

            // Queue for breadth-first search
            Queue<NodeSegement> toVisit = new Queue<NodeSegement>();
            toVisit.Enqueue(nodeSegement);

            while (toVisit.Count > 0)
            {
                NodeSegement current = toVisit.Dequeue();

                // Skip if already visited
                if (visited.Contains(current))
                    continue;

                // Mark as visited and add to result
                visited.Add(current);
                if (validTypes.Contains(current.type))
                {
                    result.Add(current);

                    // Enqueue all valid neighbors (both forward and backward)
                    foreach (NodeSegement next in current.nextSegements)
                    {
                        if (!visited.Contains(next) && validTypes.Contains(next.type))
                        {
                            toVisit.Enqueue(next);
                        }
                    }

                    foreach (NodeSegement prev in current.previousSegements)
                    {
                        if (!visited.Contains(prev) && validTypes.Contains(prev.type))
                        {
                            toVisit.Enqueue(prev);
                        }
                    }
                }
            }

            return result;
        }


        public void DeleteNode(Corner toDelete)
        {
            //GameObject.Destroy(toDelete.gameObject);
        }

    }




}
