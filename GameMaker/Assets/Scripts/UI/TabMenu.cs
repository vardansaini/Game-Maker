using Assets.Scripts.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class TabMenu : MonoBehaviour
    {
        private static bool tab;
        [SerializeField]
        private Button tabPrefab;
        [SerializeField]
        private SpriteMenu spriteMenu;

        private string currentTab;
        private string pendingTab;
        int r = 0;
        private void Start()
        {
            // Get initial tab
            currentTab = SpriteManager.Instance.GetTagList()[0];

            // Generate tabs
            foreach(string tag in SpriteManager.Instance.GetTagList())
            {
                Button temp = Instantiate(tabPrefab, transform);
                temp.GetComponentInChildren<Text>().text = tag;
                temp.onClick.AddListener(() => OnButtonClick(tag));
            }
        }

        private void Update()
        {
            if(pendingTab != null && !spriteMenu.IsLocked)
            {
                spriteMenu.DisplaySprites(SpriteManager.Instance.GetSpriteList(pendingTab));
                currentTab = pendingTab;
                pendingTab = null;
            }
        }
        public void OnTab()
        {
            Debug.Log("I am on tab");
            tab = !tab;

            UpdateButtonState(tab, tabPrefab);
        }
       

            private void OnButtonClick(string tag)
        {
            
            Debug.Log("I was here"+ r);
            r += 1;
            OnTab();
            if (currentTab == null || !currentTab.Equals(tag))
            {
                pendingTab = tag;                     

            }
        }        
            public void ResetKeys()
        {
            tab = false;
            UpdateButtonState(tab, tabPrefab);
        }
        private void UpdateButtonState(bool off, Button button)
        {
            Debug.Log("I came here to change color");
            ColorBlock colors = button.colors;
            if (!off)
            {
                Debug.Log("Button is not selected");
                colors.normalColor = Color.white;
                colors.highlightedColor = Color.white;
            }
            else
            {
                Debug.Log("Button is selected");
                colors.normalColor = Color.green;
                colors.highlightedColor = Color.green;
            }
            button.colors = colors;

        }
    }
}