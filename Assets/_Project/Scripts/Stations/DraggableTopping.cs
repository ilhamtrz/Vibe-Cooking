using UnityEngine;

namespace VibeCooking
{
    public class DraggableTopping : MonoBehaviour
    {
        private IngredientDataSO toppingData;
        private Camera mainCamera;
        private bool isDragging = true;

        public void Initialize(IngredientDataSO data)
        {
            toppingData = data;
            mainCamera = Camera.main;
        }

        private void Update()
        {
            if (isDragging)
            {
                if (mainCamera == null)
                    mainCamera = Camera.main;

                transform.position = InputHelper.GetMouseWorldPosition(mainCamera);

                if (InputHelper.IsMouseButtonUp(0))
                {
                    OnDrop();
                }
            }
        }

        private void OnDrop()
        {
            isDragging = false;

            // Check if dropped inside a bowl
            var hits = Physics2D.OverlapPointAll(transform.position);
            bool placed = false;

            foreach (var hit in hits)
            {
                var bowl = hit.GetComponent<BowlInstance>();
                if (bowl != null)
                {
                    bowl.AddTopping(toppingData, transform.position);
                    placed = true;
                    break;
                }
            }

            // Clean up temporary dragged object
            Destroy(gameObject);

            if (placed)
            {
                Debug.Log($"[DraggableTopping] Placed {toppingData.displayName} into bowl!");
            }
        }
    }
}
