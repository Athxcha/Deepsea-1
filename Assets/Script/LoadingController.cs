using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace DeepScan
{
    public class LoadingController : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField]
        private Image loadingImage;

        [SerializeField]
        private Image fadeOverlay;

        [SerializeField]
        private Button continueButton;

        [Header("Video")]
        [SerializeField]
        private VideoPlayer videoPlayer;

        [Header("Timing")]
        [SerializeField]
        private float loadingTime = 2f;

        [SerializeField]
        private float fadeInTime = 0.5f;

        [SerializeField]
        private float fadeToBlackTime = 1f;

        private bool isTransitioning;

        private void Start()
        {
            if (SceneFlowService.Instance == null)
            {
                Debug.LogError("SceneFlowService does not exist!");
                return;
            }

            if (loadingImage == null)
            {
                Debug.LogError("Loading Image is not assigned!");
                return;
            }

            if (fadeOverlay == null)
            {
                Debug.LogError("Fade Overlay is not assigned!");
                return;
            }

            if (continueButton == null)
            {
                Debug.LogError("Continue Button is not assigned!");
                return;
            }

            if (videoPlayer == null)
            {
                Debug.LogError("Video Player is not assigned!");
                return;
            }

            videoPlayer.playOnAwake = false;
            videoPlayer.isLooping = false;
            videoPlayer.loopPointReached += OnVideoFinished;

            loadingImage.gameObject.SetActive(true);

            Color imageColor = loadingImage.color;
            imageColor.a = 0f;
            loadingImage.color = imageColor;

            Color fadeColor = fadeOverlay.color;
            fadeColor.a = 0f;
            fadeOverlay.color = fadeColor;
            fadeOverlay.raycastTarget = false;
            fadeOverlay.gameObject.SetActive(true);

            continueButton.gameObject.SetActive(false);
            continueButton.interactable = true;
            continueButton.onClick.AddListener(ContinueToNextScene);

            StartCoroutine(ShowLoadingAndButton());
        }

        private IEnumerator ShowLoadingAndButton()
        {
            float elapsed = 0f;
            Color imageColor = loadingImage.color;

            while (elapsed < fadeInTime)
            {
                elapsed += Time.deltaTime;

                imageColor.a = Mathf.Clamp01(elapsed / fadeInTime);
                loadingImage.color = imageColor;

                yield return null;
            }

            imageColor.a = 1f;
            loadingImage.color = imageColor;

            yield return new WaitForSeconds(loadingTime);

            continueButton.gameObject.SetActive(true);
        }

        private void ContinueToNextScene()
        {
            if (isTransitioning)
                return;

            isTransitioning = true;
            continueButton.interactable = false;

            StartCoroutine(FadeThenPlayVideo());
        }

        private IEnumerator FadeThenPlayVideo()
        {
            float elapsed = 0f;
            Color fadeColor = fadeOverlay.color;

            while (elapsed < fadeToBlackTime)
            {
                elapsed += Time.deltaTime;

                fadeColor.a = Mathf.Clamp01(elapsed / fadeToBlackTime);
                fadeOverlay.color = fadeColor;

                yield return null;
            }

            fadeColor.a = 1f;
            fadeOverlay.color = fadeColor;

            videoPlayer.Play();
        }

        private void OnVideoFinished(VideoPlayer player)
        {
            if (SceneFlowService.Instance != null)
            {
                SceneFlowService.Instance.CompleteLoading();
            }
        }

        private void OnDestroy()
        {
            if (videoPlayer != null)
            {
                videoPlayer.loopPointReached -= OnVideoFinished;
            }

            if (continueButton != null)
            {
                continueButton.onClick.RemoveListener(ContinueToNextScene);
            }
        }
    }
}