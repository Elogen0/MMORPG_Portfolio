using System.Collections;
using System.Collections.Generic;
using Kame;
using UnityEngine;

namespace RPG.SceneManagement
{
    public class Fader : SingletonMono<Fader>
    {
        CanvasGroup canvasGroup;
        private Coroutine currentActiveFade = null;
        private GameObject fader;
        protected override void Awake()
        {
            base.Awake();
            fader = ResourceLoader.Instantiate("Assets/Game/Prefab/UI/Components/Fader.prefab", transform);
            canvasGroup = fader.GetOrAddComponent<CanvasGroup>();
            Canvas canvas = fader.GetOrAddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 400;
        }

        public void FadeOutImmediate()
        {
            canvasGroup.alpha = 1f;
        }

        public void FadeOut(float time)
        {
            fader.SetActive(true);
            Fade(1, time);
        }
        public void FadeIn(float time)
        {
            Fade(0, time);
        }

        private Coroutine Fade(float alphaTarget, float time)
        {
            if (currentActiveFade != null)
            {
                StopCoroutine(currentActiveFade);
            }
            currentActiveFade = StartCoroutine(CoFade(alphaTarget, time));
            return currentActiveFade;
        }
        
        private IEnumerator CoFade(float alphaTarget, float time)
        {
            while(!Mathf.Approximately(canvasGroup.alpha,  alphaTarget))
            {
                canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, alphaTarget, Time.deltaTime / time);
                yield return null;
            }

            if (Mathf.Approximately(canvasGroup.alpha, 0))
            {
                fader.SetActive(false);
            }
        }
    }

}
