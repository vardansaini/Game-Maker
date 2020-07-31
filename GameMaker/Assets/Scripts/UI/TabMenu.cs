using Assets.Scripts.Core;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

namespace Assets.Scripts.UI
{
    public class TabMenu : MonoBehaviour
    {
        [SerializeField]
        private SpriteMenu spriteMenu;
        [SerializeField]
        private Button tabPrefab;
        private string currentTab;
        private string pendingTab;
        int r = 0;
        // put tab as value to all keys
        public Dictionary<string, Button> tabs;
        //public static bool Tab { get { return tab; } }

        private void Start()
        {
            tabs = new Dictionary<string, Button>();
            // Get initial tab
            currentTab = SpriteManager.Instance.GetTagList()[0];

            // Generate tabs
            foreach (string tag in SpriteManager.Instance.GetTagList())
            {
                Button temp = Instantiate(tabPrefab, transform);
                temp.GetComponentInChildren<Text>().text = tag;
                temp.onClick.AddListener(() => OnButtonClick(tag));
                tabs.Add(tag, temp);
                //temp.onClick.AddListener(OnTab); works but need to restart the whole program to see the result (Failed)
            }
            UpdateButtonState(currentTab);

            //Debug.Log("Did I click it?");
            //tabPrefab.onClick.AddListener(OnTab); Does not work (Failed)
        }

        private void Update()
        {
            if (pendingTab != null && !spriteMenu.IsLocked)
            {
                spriteMenu.DisplaySprites(SpriteManager.Instance.GetSpriteList(pendingTab));
                currentTab = pendingTab;
                pendingTab = null;
            }
        }

        private void OnButtonClick(string tag)
        {
            r += 1;
            // OnTab(); works but need to restart the whole program to see the result (Failed)
            // create dictionary map from string name of buttons and 

            if (currentTab == null || !currentTab.Equals(tag))
            {
                pendingTab = tag;
                UpdateButtonState(tag);
            }
        }

        

        private void UpdateButtonState(string tag)
        {
            foreach (KeyValuePair<string, Button> entry in tabs)
            {
                if (entry.Key == tag)
                    selectColor(tabs[entry.Key]);
                else
                    deselectColor(tabs[entry.Key]);
            }
        }

        private void deselectColor(Button button)
        {
            ColorBlock colors = button.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = Color.white;
            button.colors = colors;
        }

        private void selectColor(Button button)
        {
            ColorBlock colors = button.colors;
            colors.normalColor = Color.green;
            colors.highlightedColor = Color.green;
            button.colors = colors;
        }
    }
}