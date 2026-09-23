using UnityEngine;

namespace VibeCooking
{
    public class ToppingTray : MonoBehaviour
    {
        [Header("Topping Data")]
        [SerializeField] private IngredientDataSO toppingData;

        [Header("Visuals")]
        [SerializeField] private SpriteRenderer trayIconRenderer;

        private Camera mainCamera;

        public IngredientDataSO ToppingData => toppingData;

        private void Start()
        {
            mainCamera = Camera.main;
            if (trayIconRenderer != null && toppingData != null)
            {
                if (toppingData.icon != null)
                    trayIconRenderer.sprite = toppingData.icon;
                trayIconRenderer.color = toppingData.placeholderColor;
            }
        }

        private void OnMouseDown()
        {
            if (toppingData == null) return;

            if (mainCamera == null)
                mainCamera = Camera.main;

            Vector3 spawnPos = InputHelper.GetMouseWorldPosition(mainCamera);

            var dragObj = new GameObject($"Draggable_{toppingData.displayName}");
            dragObj.transform.position = spawnPos;

            var sr = dragObj.AddComponent<SpriteRenderer>();
            sr.sprite = toppingData.icon != null ? toppingData.icon : toppingData.bowlVisual;
            sr.color = toppingData.placeholderColor;
            sr.sortingOrder = 20;

            var draggable = dragObj.AddComponent<DraggableTopping>();
            draggable.Initialize(toppingData);
        }

        public void SetToppingData(IngredientDataSO data)
        {
            toppingData = data;
            if (trayIconRenderer != null && toppingData != null)
            {
                if (toppingData.icon != null)
                    trayIconRenderer.sprite = toppingData.icon;
                trayIconRenderer.color = toppingData.placeholderColor;
            }
        }
    }
}
