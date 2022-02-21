using Assets.Scripts.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class LevelBackground : MonoBehaviour
    {
        void Start()
        {
            GridManager.Instance.GridSizeChanged += OnGridSizeChanged;
        }

        private void OnGridSizeChanged(int x, int y)
        {
            transform.position = new Vector3(
                (float)x / 2,
                (float)y / 2,
                transform.position.z
                );
            ((RectTransform)transform).sizeDelta = new Vector2(x, y);
        }

        // CODE TO CHANGE BG COLOR
        /*Renderer m_ObjectRenderer;
        void Start()
        {
            Fetch the GameObject's Renderer component
         m_ObjectRenderer = GetComponentInChildren<Renderer>();
            Change the GameObject's Material Color to red
        m_ObjectRenderer.material.color = Color.red;
            GetComponentInChildren(Renderer).material.color = Color.red;
        }
        private void Update()
        {

        }
        private function applyColor(a_color: Color) {
            var renderers: Component[] = GetComponentsInChildren(Renderer);
            for (var renderer: Renderer in renderers)
                renderer.material.color = a_color;*/
    }
}
