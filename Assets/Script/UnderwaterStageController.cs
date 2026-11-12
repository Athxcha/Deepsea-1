using UnityEngine;

namespace DeepScan
{
    public class UnderwaterStageController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        private FishSpawner fishSpawner;

        [SerializeField]
        private DiveTimer diveTimer;

        [SerializeField]
        private EndDiveTransition endDiveTransition;

        [Header("Game")]
        [SerializeField]
        private float diveDuration = 180f;

        private void OnEnable()
        {
            if (diveTimer != null)
            {
                diveTimer.DiveEnded += HandleDiveEnded;
            }
        }

        private void OnDisable()
        {
            if (diveTimer != null)
            {
                diveTimer.DiveEnded -= HandleDiveEnded;
            }
        }

        private void Start()
        {
            if (GameSession.Instance == null)
            {
                Debug.LogError(
                    "GameSession does not exist."
                );

                return;
            }

            if (fishSpawner == null)
            {
                Debug.LogError(
                    "FishSpawner is missing."
                );

                return;
            }

            if (diveTimer == null)
            {
                Debug.LogError(
                    "DiveTimer is missing."
                );

                return;
            }

            // ไม่มี EndDiveTransition ก็ยังเริ่มเกมและจับเวลา
            if (endDiveTransition == null)
            {
                Debug.LogWarning(
                    "EndDiveTransition is not assigned."
                );
            }

            fishSpawner.SpawnFish();
            diveTimer.StartTimer(diveDuration);
        }

        private void HandleDiveEnded()
        {
            if (endDiveTransition != null)
            {
                endDiveTransition.StartEndTransition();
            }
            else
            {
                Debug.LogError(
                    "Cannot end dive: EndDiveTransition is missing."
                );
            }
        }
    }
}