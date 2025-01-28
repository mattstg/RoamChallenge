using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Controllers
{
    public class ControlsUI : MonoBehaviour
    {
        public Transform uiGrid;
        public GameObject hotkeyUIPrefab;
        Dictionary<ControllerActions, GameObject> hotkeyUIs = new Dictionary<ControllerActions, GameObject>();
        
        public void SetupUIDict()
        {
            foreach(Transform t in uiGrid.transform)
            {
                hotkeyUIs.Add((ControllerActions)System.Enum.Parse(typeof(ControllerActions), t.name), t.gameObject);
                t.gameObject.SetActive(false);
            }
        }

        public void NothingSelected()
        {
            foreach (Transform t in uiGrid.transform)
            {
                t.gameObject.SetActive(false);
            }
        }

        public void DisplayControls(ControllerActions[] visibleOptions)
        {
            hotkeyUIs.ForEach(kv => kv.Value.SetActive(false));
            foreach (var option in visibleOptions)
            {
                hotkeyUIs[option].SetActive(true);
            }
        }
    }
}
