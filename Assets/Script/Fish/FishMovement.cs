using UnityEngine;

namespace DeepScan
{
    public class FishMovement : MonoBehaviour
    {
        private FishMovementData data;
        private Vector3 startPosition;
        private float movementTime;
        private float offset;
        private bool isPaused;

        [Header("Swimming Bounds")]
        [SerializeField] private Vector3 minBounds = new Vector3(-8f, 1f, -8f);
        [SerializeField] private Vector3 maxBounds = new Vector3(8f, 8f, 8f);

        [Header("Collision")]
        [SerializeField] private LayerMask obstacleLayers = ~0;
        [SerializeField, Min(0.01f)] private float fishRadius = 0.25f;
        [SerializeField, Min(0f)] private float surfacePadding = 0.1f;

        public void Configure(FishMovementData movementData)
        {
            data = movementData;
            startPosition = transform.position;
            movementTime = 0f;
            offset = Random.Range(0f, 100f);
            isPaused = false;
        }

        public void SetPaused(bool paused)
        {
            isPaused = paused;
        }

        private void Update()
        {
            if (data == null || isPaused)
                return;

            movementTime += Time.deltaTime;
            float t = movementTime * data.Speed + offset;

            Vector3 targetPosition = startPosition + new Vector3(
                Mathf.Sin(t) * data.HorizontalRange,
                Mathf.Sin(t * 0.7f) * data.VerticalRange,
                Mathf.Sin(t * 0.45f + offset) * data.HorizontalRange * 0.5f
            );

            targetPosition.x = Mathf.Clamp(targetPosition.x, minBounds.x, maxBounds.x);
            targetPosition.y = Mathf.Clamp(targetPosition.y, minBounds.y, maxBounds.y);
            targetPosition.z = Mathf.Clamp(targetPosition.z, minBounds.z, maxBounds.z);

            Vector3 currentPosition = transform.position;
            Vector3 direction = targetPosition - currentPosition;
            float distance = direction.magnitude;

            if (distance > 0.001f &&
                Physics.SphereCast(
                    currentPosition,
                    fishRadius,
                    direction.normalized,
                    out RaycastHit hit,
                    distance,
                    obstacleLayers,
                    QueryTriggerInteraction.Ignore))
            {
                // หยุดก่อนถึงพื้นหรือกำแพง
                targetPosition = hit.point + hit.normal * (fishRadius + surfacePadding);
            }

            transform.position = targetPosition;
        }
    }
}