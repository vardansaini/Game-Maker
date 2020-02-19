using Assets.Scripts.UI;
using Assets.Scripts.Util;
using System;
using System.Collections;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.IO;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Assets.Scripts.Core;
using UnityEngine;
namespace Assets.Scripts.UI
{
    public class FadeManager: MonoBehaviour

    {

        public GridManager gridManager;

        //public CanvasGroup uiElement;

        public void MakeOldSpritesTransparent()
        {
            gridManager.SetAllTransparent(0.5f);
        }
            /*
        public void FadeIn()
        {
            StartCoroutine(FadeCanvasGroup(uiElement, uiElement.alpha, 1));
        }
        public void FadeOut()
        {
            StartCoroutine(FadeCanvasGroup(uiElement, uiElement.alpha, 0));
        }
        public IEnumerator FadeCanvasGroup(CanvasGroup cg, float start, float end, float lerpTime = 0.1f)
        {
            float timeStartedLerping = Time.time;
            float timeSiceStarted = Time.time - timeStartedLerping;
            float perncentageComplete = timeSiceStarted / lerpTime;

            while (true)
            {
                timeSiceStarted = Time.time - timeStartedLerping;
                perncentageComplete = timeSiceStarted / lerpTime;

                float currentValue = Mathf.Lerp(start, end, perncentageComplete);
                cg.alpha = currentValue;
                if (perncentageComplete >= 1) break;
                yield return new WaitForEndOfFrame();
            }
        }
        */
    }
}
