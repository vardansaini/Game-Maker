using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Core
{
    public class GridObjectNext : MonoBehaviour
    {
        public SpriteData Data { get; private set; }

        // Top left corner grid coordinates
        public int X { get; private set; }
        public int Y { get; private set; }
        //width and height
        public int W { get; private set; }
        public int H { get; private set; }
        //Velocity x and y
        public int VX;
        public int VY;

        public string Name { get { return Data.Name; } } 

        [SerializeField]
        private Image image;
        private Color imageColor;

        public void SetSprite(SpriteData data)
        {
            Data = data;
            image.sprite = data.Sprite;
            if(data.MaintainAspect)
            {
                float scale = Mathf.Min(data.Width / data.Sprite.bounds.size.x, data.Height / data.Sprite.bounds.size.y);
                ((RectTransform)transform).sizeDelta = scale * data.Sprite.bounds.size;
            }
            else
            {
                ((RectTransform)transform).sizeDelta = new Vector2(data.Width, data.Height);
            }
        }

        public void SetPosition(int x, int y)
        {
            X = x;
            Y = y;
			W = Data.Width;
			H = Data.Height;
            //Debug.Log(X);
            //Debug.Log(Data.Width);
            //transform.position = new Vector2((x + (float)Data.Width / 2) + x + (float)Data.Width + 2, y + (float)Data.Height / 2);
            transform.position = new Vector2((x + (float)Data.Width / 2)+22+2 , y + (float)Data.Height / 2);
            //transform.position = new Vector2(((x + (float)Data.Width / 2)-22)-2, y + (float)Data.Height / 2);
        }

        public void SetAlpha(float alpha)
        {
            imageColor = image.color;
            imageColor.a = Mathf.Clamp(alpha, 0, 1);
            image.color = imageColor;
        }

		public float GetAlpha(){
			return image.color.a;
		}

        public Vector2 GetNamedPoint(string name)
        {
            if (name == "North")
            {
                return GetNorth();
            }
            else if (name == "South")
            {
                return GetSouth();
            }
            else if (name == "West")
            {
                return GetWest();
            }
            else if (name == "East")
            {
                return GetEast();
            }
            return GetCenter();
        }

        public Vector2 GetNorth()
        {
            return new Vector2(X + W / 2f, Y);
        }

        public Vector2 GetSouth()
        {
            return new Vector2(X + W / 2f, Y +H);
        }

        public Vector2 GetWest()
        {
            return new Vector2(X, Y + H/2f);
        }

        public Vector2 GetEast()
        {
            return new Vector2(X+W, Y + H / 2f);
        }

        public Vector2 GetCenter()
        {
            return new Vector2(X + W/2f, Y + H / 2f);
        }



    }
}