﻿using Assets.Scripts.UI;
using Assets.Scripts.Util;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Assets.Scripts.Core
{
	public class GridPlacement : Lockable
	{
		[HideInInspector]
		public SpriteData CurrentSprite; // Initialized by the sprite menu

		[SerializeField]
		private GridObject previewObject;
		[SerializeField]
		private DialogueMenu dialogueMenu;

		private Vector2? previousMousePosition;
		private bool? deletionLayer; // Functional if true, decorative if false

		public enum PlacementMode { Level, Pathfinding }
		[SerializeField] private PlacementMode mode;

		[SerializeField]
		private Text posX;
		[SerializeField]
		private Text posY;

		private Vector2 textPosX;
		private Vector2 textPosY;

		protected override void Awake()
		{
			base.Awake();

			dialogueMenu.DialogueOpened += () => AddLock(dialogueMenu);
			dialogueMenu.DialogueOpened += () => previewObject.gameObject.SetActive(false);
			dialogueMenu.DialogueClosed += () => RemoveLock(dialogueMenu);
			mode = PlacementMode.Level;
			//Map.pathForBots = null;
		}

		private void Update()
		{

			// Calculate sprite coordinates for the current mouse position
			Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			//Debug.Log(mousePosition);
			if (previousMousePosition == null)
				previousMousePosition = mousePosition;


			if (!IsLocked)
			{
				// Interpolate between previous and current mouse position
				int spriteX = 0, spriteY = 0;
				for (float i = 0.25f; i <= 1; i += 0.25f)
				{

					
					spriteX = Mathf.RoundToInt(Mathf.Lerp(previousMousePosition.Value.x, mousePosition.x, i) - (float)CurrentSprite.Width / 2);
					spriteY = Mathf.RoundToInt(Mathf.Lerp(previousMousePosition.Value.y, mousePosition.y, i) - (float)CurrentSprite.Height / 2);
					//Debug.Log("MouseX = " + spriteX + " MouseY = " + spriteY);
					posX.text = "X:" + spriteX.ToString();
					posY.text = "Y:" + spriteY.ToString();
					Debug.Log(GridManager.Instance.GridWidth);
					Debug.Log(spriteX);
					if (spriteX >= (GridManager.Instance.GridWidth-3)) {
						// Text Position along mouse
						if (spriteY <= 0)
						{
							textPosX = new Vector2(Input.mousePosition.x - 100, Input.mousePosition.y + 50);
							textPosY = new Vector2(Input.mousePosition.x - 60, Input.mousePosition.y + 50);
						}
						else
						{
							textPosX = new Vector2(Input.mousePosition.x - 100, Input.mousePosition.y - 50);
							textPosY = new Vector2(Input.mousePosition.x - 60, Input.mousePosition.y - 50);
						}
						
					}
					else if (spriteY <= 0)
					{
						textPosX = new Vector2(Input.mousePosition.x + 60, Input.mousePosition.y + 50);
						textPosY = new Vector2(Input.mousePosition.x + 100, Input.mousePosition.y + 50);
					}
                    else
                    {
						// Text Position along mouse
						textPosX = new Vector2(Input.mousePosition.x + 60, Input.mousePosition.y - 50);
						textPosY = new Vector2(Input.mousePosition.x + 100, Input.mousePosition.y - 50);
					}
					posX.transform.position = textPosX;
					posY.transform.position = textPosY;
					//Debug.Log("I am here to check");

					if (mode == PlacementMode.Level)
					{
						if (Input.GetMouseButton(1))
						{
							int mouseX = Mathf.RoundToInt(Mathf.Lerp(previousMousePosition.Value.x, mousePosition.x, i) - 0.5f);
							int mouseY = Mathf.RoundToInt(Mathf.Lerp(previousMousePosition.Value.y, mousePosition.y, i) - 0.5f);
							
							// Set deletion layer if not set, prioritizing the functional layer
							if (deletionLayer == null)
							{
								if (GridManager.Instance.ContainsGridObject(true, mouseX, mouseY))
									deletionLayer = true;
								else if (GridManager.Instance.ContainsGridObject(false, mouseX, mouseY))
									deletionLayer = false;
							}

							// Remove existing grid object based on deletion layer
							if (deletionLayer != null)
								if (GridManager.Instance.ContainsGridObject(deletionLayer.Value, mouseX, mouseY))
									GridManager.Instance.RemoveGridObject(deletionLayer.Value, mouseX, mouseY);
						}
						else if (Input.GetMouseButton(0) && CurrentSprite.HoldToPlace && GridManager.Instance.CanAddGridObject(CurrentSprite, spriteX, spriteY))
						{
							// Place new grid object (if hold-to-place)
							GridManager.Instance.AddGridObject(CurrentSprite, spriteX, spriteY, true);
						}

					}
				}
				
				if (mode == PlacementMode.Level)
				{
					// Place new grid object (if not hold-to-place)
					if (Input.GetMouseButtonDown(0) && !Input.GetMouseButton(1) && GridManager.Instance.CanAddGridObject(CurrentSprite, spriteX, spriteY))
						GridManager.Instance.AddGridObject(CurrentSprite, spriteX, spriteY, true);

					// Remove deletion layer
					if (Input.GetMouseButtonUp(1))
						deletionLayer = null;
				}
				// Update preview object
				if (CurrentSprite.Sprite != previewObject.Data.Sprite)
				{
					//Debug.Log("I am in update preview object.");
					previewObject.SetSprite(CurrentSprite);
				}
				previewObject.SetPosition(spriteX, spriteY);
				//textPos = previewObject.GetPosition();
				//Debug.Log(textPos);
				previewObject.gameObject.SetActive(GridManager.Instance.CanAddGridObject(CurrentSprite, spriteX, spriteY));
				bool activeStatus = GridManager.Instance.GridBounds(CurrentSprite, spriteX, spriteY);
				posX.gameObject.SetActive(activeStatus);
				posY.gameObject.SetActive(activeStatus);

				

			}

			// Store mouse position
			previousMousePosition = mousePosition;

		}

		private void OnApplicationFocus(bool hasFocus)
		{
			previewObject.gameObject.SetActive(false);
			previousMousePosition = null;
			posX.gameObject.SetActive(false);
			posY.gameObject.SetActive(false);

		}
    }
}