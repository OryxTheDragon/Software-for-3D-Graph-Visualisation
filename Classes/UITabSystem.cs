﻿using UnityEngine;
using UnityEngine.UI;

namespace Assets.Classes
{
    class UITabSystem  : MonoBehaviour 
    {
        public GameObject[] tabPages;
        public Button[] tabButtons;

        private int currentPageIndex = 0;
        void Start()
        {
            ShowPage(currentPageIndex);
        }

        public void ShowPage(int index){

            // Hide all pages except the current one
            for (int i = 0; i < tabPages.Length; i++)
            {
                if (i == index)
                {
                    tabPages[i].SetActive(true);
                }
                else
                {
                    tabPages[i].SetActive(false);
                }
            }
            currentPageIndex = index;
            UpdateTabButtonColors();
        }

        private void UpdateTabButtonColors()
        {
            for (int i = 0; i < tabButtons.Length; i++)
            {
                if (i == currentPageIndex)
                {
                    tabButtons[i].GetComponent<Image>().color = Color.white;
                }
                else
                {
                    tabButtons[i].GetComponent<Image>().color = Color.gray;
                }
            }
        }
    }
}
