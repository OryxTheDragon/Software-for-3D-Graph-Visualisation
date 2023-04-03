using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        void Update()
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
