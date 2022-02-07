using Assets.Scripts.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class LevelBackgroundNext : MonoBehaviour
    {
        void Start()
        {
            GridNext.Instance.GridSizeChanged += OnGridSizeChanged;
        }

        private void OnGridSizeChanged(int x, int y)
        {
            transform.position = new Vector3(((float)x*1.5f)+2, (float)y / 2, transform.position.z);
            ((RectTransform)transform).sizeDelta = new Vector2(x, y);
        }
       
    }
    }
