using Controllers;
using UnityEngine;
namespace TerrainSystem
{
    public class Node : NodeSegement
    {
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
            float amt = increase ? Controller.WIDTH_MOD : -Controller.WIDTH_MOD;
            transform.localScale = new Vector3(transform.localScale.x + amt, transform.localScale.y, transform.localScale.z + amt);
        }
        
    }
}
