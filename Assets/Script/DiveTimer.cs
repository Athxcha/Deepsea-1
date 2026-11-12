using System;
using UnityEngine;

namespace DeepScan
{
    public class DiveTimer : MonoBehaviour
    {
        public event Action<float> OxygenChanged;
        public event Action DiveEnded;

        private float duration;
        private float remaining;
        private bool running;

        public float NormalizedOxygen
        {
            get
            {
                if (duration <= 0f)
                    return 0f;

                return Mathf.Clamp01(
                    remaining / duration
                );
            }
        }

        public int OxygenPercent
        {
            get
            {
                return Mathf.Clamp(
                    Mathf.CeilToInt(
                        NormalizedOxygen * 100f
                    ),
                    0,
                    100
                );
            }
        }

        public void StartTimer(float seconds)
        {
            duration = Mathf.Max(0f, seconds);
            remaining = duration;
            running = duration > 0f;

            OxygenChanged?.Invoke(
                OxygenPercent
            );

            if (!running)
            {
                DiveEnded?.Invoke();
            }
        }

        private void Update()
        {
            if (!running)
                return;

            remaining -= Time.deltaTime;

            if (remaining <= 0f)
            {
                remaining = 0f;
                running = false;

                OxygenChanged?.Invoke(0f);
                DiveEnded?.Invoke();

                return;
            }

            OxygenChanged?.Invoke(
                OxygenPercent
            );
        }
    }
}