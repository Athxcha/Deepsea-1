using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DeepScan
{
    public class DiveHUD : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private DiveTimer diveTimer;
        [SerializeField] private LidarScanner scanner;

        [Header("Oxygen")]
        [SerializeField] private TMP_Text oxygenText;

        [Header("Fish Count")]
        [SerializeField] private TMP_Text fishCountText;

        [Header("Scan")]
        [SerializeField] private GameObject scanUI;
        [SerializeField] private Slider scanSlider;
        [SerializeField] private TMP_Text fishNameText;
        [SerializeField] private TMP_Text percentageText;

        private void OnEnable()
        {
            if (diveTimer != null)
                diveTimer.OxygenChanged += UpdateOxygen;
        }

        private void OnDisable()
        {
            if (diveTimer != null)
                diveTimer.OxygenChanged -= UpdateOxygen;
        }

        private void Start()
        {
            if (diveTimer != null)
                UpdateOxygen(diveTimer.OxygenPercent);
        }

        private void Update()
        {
            UpdateFishCount();
            UpdateScanUI();
        }

        private void UpdateOxygen(float value)
        {
            if (oxygenText != null)
                oxygenText.text = Mathf.RoundToInt(value).ToString();
        }

        private void UpdateFishCount()
        {
            if (fishCountText == null)
                return;

            int count = GameSession.Instance != null
                ? GameSession.Instance.CaughtFish.Count
                : 0;

            fishCountText.text = count.ToString();
        }

        private void UpdateScanUI()
        {
            if (scanner == null || scanUI == null)
                return;

            FishActor fish = scanner.CurrentFish;

            if (fish == null || !Input.GetMouseButton(0))
            {
                scanUI.SetActive(false);
                return;
            }

            scanUI.SetActive(true);

            float progress = fish.ScanProgress;

            if (scanSlider != null)
                scanSlider.value = progress;

            if (percentageText != null)
                percentageText.text =
                    Mathf.RoundToInt(progress * 100f) + "%";

            if (fishNameText != null)
            {
                fishNameText.text = progress >= 1f
                    ? fish.Data.FishName
                    : "UNKNOWN LIFEFORM";
            }
        }
    }
}