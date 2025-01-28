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

        public override void Fix()
        {
            NodeSegement next = nextSegements[0];
            NodeSegement prev = previousSegements[0];
            // Calculate the mid-point between startPos and endPos to position the object
            Vector3 midPoint = (prev.transform.position + next.transform.position) / 2;

            // Calculate the direction and length of the gap
            Vector3 direction = (prev.transform.position - next.transform.position).normalized;
            float distance = Vector3.Distance(next.transform.position, prev.transform.position);

            // Set the position of the gap object
            transform.position = midPoint;

            // Set the scale of the gap object
            Vector3 originalScale = transform.localScale;
            transform.localScale = new Vector3(originalScale.x, originalScale.y, distance);

            // Rotate the gap object to align with the direction from startPos to endPos
            Quaternion rotation = Quaternion.LookRotation(direction);
            transform.rotation = rotation;

            float gapSize = transform.localScale.z - (prev.transform.localScale.z / 2) - (next.transform.localScale.z / 2);

            transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, gapSize);
        }

        public override void ConvertToRamp()
        {
            base.ConvertToRamp();
            NodeSegement next = nextSegements[0];
            NodeSegement prev = previousSegements[0];

            Ramp newPlatform = Factory.CreateRamp(next.transform.position, prev.transform.position);
            newPlatform.previousSegements.Add(prev);
            newPlatform.nextSegements.Add(next);
            prev.nextSegements.Add(newPlatform);
            next.previousSegements.Add(newPlatform);

            prev.nextSegements.Remove(this);
            next.previousSegements.Remove(this);
            GameObject.DestroyImmediate(gameObject);
            GameManager.controller.SetSelected(newPlatform);
        }

        public override void ConvertToPlatform()
        {
            base.ConvertToPlatform();
            NodeSegement next = nextSegements[0];
            NodeSegement prev = previousSegements[0];

            if (next.transform.position.y == prev.transform.position.y)
            {
                Platform newPlatform = Factory.CreatePlatform(next.transform.position, prev.transform.position);
                newPlatform.previousSegements.Add(prev);
                newPlatform.nextSegements.Add(next);
                prev.nextSegements.Add(newPlatform);
                next.previousSegements.Add(newPlatform);

                prev.nextSegements.Remove(this);
                next.previousSegements.Remove(this);
                GameObject.DestroyImmediate(gameObject);
                GameManager.controller.SetSelected(newPlatform);
            }
            else
            {
                GameManager.Instance.DisplayWarning("Operation blocked, cannot convert to playform because the heights don't match");
            }
        }
    }


}
