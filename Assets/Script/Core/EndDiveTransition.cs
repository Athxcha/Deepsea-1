using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

namespace DeepScan
{
    public class EndDiveTransition : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField]
        private Image fadeOverlay;

        [SerializeField]
        private GameObject videoDisplay;

        [Header("Video")]
        [SerializeField]
        private VideoPlayer videoPlayer;

        [Header("Settings")]
        [SerializeField]
        private float fadeDuration = 1f;

        [SerializeField]
        private string resultSceneName = "Result";

        private bool isTransitioning;

        private void Start()
        {
            if (fadeOverlay != null)
            {
                fadeOverlay.gameObject.SetActive(true);
                fadeOverlay.raycastTarget = false;

                Color color = fadeOverlay.color;
                color.a = 0f;
                fadeOverlay.color = color;
            }

            if (videoDisplay != null)
            {
                videoDisplay.SetActive(false);
            }

            if (videoPlayer != null)
            {
                videoPlayer.playOnAwake = false;
                videoPlayer.isLooping = false;
                videoPlayer.loopPointReached += OnVideoFinished;
            }
        }

        public void StartEndTransition()
        {
            if (isTransitioning)
                return;

            if (fadeOverlay == null ||
                videoDisplay == null ||
                videoPlayer == null)
            {
                Debug.LogError(
                    "EndDiveTransition references are not assigned!"
                );

                return;
            }

            isTransitioning = true;
            StartCoroutine(FadeAndPlayVideo());
        }

        private IEnumerator FadeAndPlayVideo()
        {
            float elapsed = 0f;
            Color color = fadeOverlay.color;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.unscaledDeltaTime;

                color.a = Mathf.Clamp01(
                    elapsed / fadeDuration
                );

                fadeOverlay.color = color;

                yield return null;
            }

            color.a = 1f;
            fadeOverlay.color = color;

            videoDisplay.SetActive(true);

            // ให้ VideoDisplay อยู่เหนือ FadeOverlay
            videoDisplay.transform.SetAsLastSibling();

            videoPlayer.Play();
        }

        private void OnVideoFinished(VideoPlayer player)
        {
            SceneManager.LoadScene(resultSceneName);
        }

        private void OnDestroy()
        {
            if (videoPlayer != null)
            {
                videoPlayer.loopPointReached -= OnVideoFinished;
            }
        }
    }
}