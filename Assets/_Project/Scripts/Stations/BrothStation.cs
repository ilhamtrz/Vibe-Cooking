using UnityEngine;

namespace VibeCooking
{
    public class BrothStation : MonoBehaviour
    {
        [Header("Broth Data")]
        [SerializeField] private IngredientDataSO brothData;

        [Header("Audio (Optional)")]
        [SerializeField] private AudioClip ladleSound;
        [SerializeField] private AudioClip pourSound;

        [Header("Visual Pot")]
        [SerializeField] private SpriteRenderer potLiquidRenderer;

        private Vector3 startPosition;
        private bool isDragging;
        private Camera mainCamera;

        private void Start()
        {
            mainCamera = Camera.main;
            startPosition = transform.position;

            if (potLiquidRenderer != null && brothData != null)
            {
                potLiquidRenderer.color = brothData.placeholderColor;
            }
        }

        private void OnMouseDown()
        {
            isDragging = true;
        }

        private void OnMouseDrag()
        {
            if (!isDragging) return;
            if (mainCamera == null) mainCamera = Camera.main;
            transform.position = InputHelper.GetMouseWorldPosition(mainCamera);
        }

        private void OnMouseUp()
        {
            if (!isDragging) return;
            isDragging = false;

            // Check if dropped over active bowl
            var hits = Physics2D.OverlapPointAll(transform.position);
            bool poured = false;

            foreach (var hit in hits)
            {
                var bowl = hit.GetComponent<BowlInstance>();
                if (bowl != null && !bowl.HasBroth)
                {
                    bowl.AddBroth(brothData);
                    poured = true;
                    break;
                }
            }

            // Snap back
            transform.position = startPosition;

            if (poured)
            {
                Debug.Log($"[BrothStation] Poured {brothData.displayName} into bowl!");
            }
        }

        public void PourDirectly(BowlInstance targetBowl)
        {
            if (targetBowl != null && !targetBowl.HasBroth)
            {
                targetBowl.AddBroth(brothData);
            }
        }
    }
}
