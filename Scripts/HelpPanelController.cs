using UnityEngine;

namespace Assets.Scripts
{
    public class HelpPanelController : MonoBehaviour
    {
        public GameObject helpPanel;
        private bool isActive;

        private void Start()
        {
            helpPanel.SetActive(false);
            isActive = false;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.H))
            {
                // If the panel instance is null, create a new panel
                if (isActive)
                {
                    isActive = false;
                    helpPanel.SetActive(false);
                }
                // If the panel instance is not null, destroy it
                else
                {
                    isActive = true;
                    helpPanel.SetActive(true);
                }
            }
        }
    }
}
