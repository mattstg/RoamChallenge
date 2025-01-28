using Controllers;
using System.Collections.Generic;
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


        public override void Group_height(bool increase)
        {
            GameManager.controller.Group_height(increase, this);
        }

        public override void ModHeight(bool increase)
        {
            base.ModHeight(increase);
            transform.position = new Vector3(transform.position.x, transform.position.y + (increase ? Controller.HEIGHT_MOD : -Controller.HEIGHT_MOD), transform.position.z);
            transform.localScale = new Vector3(transform.localScale.x, transform.position.y - MapGenerator.WORLD_FLOOR, transform.localScale.z);
        }

        public override void Group_width(bool increase)
        {
            GameManager.controller.Group_width(increase, this);
        }

        public override void ModWidth(bool increase)
        {
            base.ModWidth(increase);
            transform.localScale = new Vector3(transform.localScale.x + (increase ? Controller.WIDTH_MOD : -Controller.WIDTH_MOD), transform.localScale.y, transform.localScale.z);
        }

        public override void Fix()
        {
            // Calculate the mid-point between startPos and endPos to position the object
            Node next = nextSegements[0] as Node;
            Node prev = previousSegements[0] as Node;

            Vector3 nextPos = next.transform.position;
            Vector3 prevPos = prev.transform.position;
            Vector3 midPoint = (prevPos + nextPos) / 2;

            // Calculate the direction and length of the gap
            Vector3 direction = (prevPos - nextPos).normalized;
            float distance = Vector3.Distance(nextPos, prevPos);
            transform.position = midPoint;

            // Set the scale of the gap object
            Vector3 originalScale = transform.localScale;
            transform.localScale = new Vector3(originalScale.x, originalScale.y, distance);

            // Rotate the gap object to align with the direction from startPos to endPos
            Quaternion rotation = Quaternion.LookRotation(direction);
            transform.rotation = rotation;


            transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, transform.localEulerAngles.z);
            transform.position = new Vector3(transform.position.x, prevPos.y, transform.position.z);
            transform.localScale = new Vector3(transform.localScale.x, prevPos.y - MapGenerator.WORLD_FLOOR, transform.localScale.z);
        }
    }
}
