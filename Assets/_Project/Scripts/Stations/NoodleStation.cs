using UnityEngine;
using TMPro;

namespace VibeCooking
{
    public enum NoodleStationState
    {
        Empty,
        Boiling,
        ReadyToPlunge
    }

    public class NoodleStation : MonoBehaviour
    {
        [Header("Timers & Firmness Windows (Seconds)")]
        [SerializeField] private float futsuuTime = 8f;  // 6 - 10s (standard perfect)
        [SerializeField] private float yawaTime = 12f;   // 10 - 14s

        [Header("Station State")]
        [SerializeField] private NoodleStationState currentState = NoodleStationState.Empty;
        [SerializeField] private float currentBoilTime = 0f;
        [SerializeField] private NoodleFirmness currentFirmness = NoodleFirmness.Raw;

        [Header("Visuals")]
        [SerializeField] private SpriteRenderer basketRenderer;
        [SerializeField] private SpriteRenderer waterSteamRenderer;
        [SerializeField] private Transform timerBarTransform;
        [SerializeField] private SpriteRenderer timerBarRenderer;
        [SerializeField] private SpriteRenderer noodleVisualInBasket;

        [Header("UI Feedback")]
        [SerializeField] private TMP_Text statusLabel;

        private Vector3 startPosition;
        private bool isDragging;
        private Camera mainCamera;

        public NoodleStationState CurrentState => currentState;
        public NoodleFirmness CurrentFirmness => currentFirmness;
        public float BoilProgress => Mathf.Clamp01(currentBoilTime / futsuuTime);

        private void Start()
        {
            mainCamera = Camera.main;
            startPosition = transform.position;
            UpdateVisuals();
        }

        private void Update()
        {
            if (currentState == NoodleStationState.Boiling)
            {
                currentBoilTime += Time.deltaTime;
                EvaluateFirmness();
                UpdateTimerVisual();
            }
        }

        public void StartBoiling()
        {
            if (currentState != NoodleStationState.Empty) return;

            currentState = NoodleStationState.Boiling;
            currentBoilTime = 0f;
            currentFirmness = NoodleFirmness.Raw;
            UpdateVisuals();
            Debug.Log("[NoodleStation] Started boiling noodles!");
        }

        private void EvaluateFirmness()
        {
            if (currentBoilTime < 3f)
                currentFirmness = NoodleFirmness.Raw;
            else if (currentBoilTime < 6f)
                currentFirmness = NoodleFirmness.Kata;
            else if (currentBoilTime < 10f)
                currentFirmness = NoodleFirmness.Futsuu;
            else if (currentBoilTime < 14f)
                currentFirmness = NoodleFirmness.Yawa;
            else
                currentFirmness = NoodleFirmness.Overcooked;
        }

        private void OnMouseDown()
        {
            if (currentState == NoodleStationState.Empty)
            {
                StartBoiling();
            }
            else if (currentState == NoodleStationState.Boiling)
            {
                isDragging = true;
            }
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

            // Check if dropped onto bowl
            var hits = Physics2D.OverlapPointAll(transform.position);
            bool plunged = false;

            foreach (var hit in hits)
            {
                var bowl = hit.GetComponent<BowlInstance>();
                if (bowl != null && !bowl.HasNoodles)
                {
                    bowl.AddNoodles(currentFirmness);
                    plunged = true;
                    break;
                }
            }

            transform.position = startPosition;

            if (plunged)
            {
                currentState = NoodleStationState.Empty;
                currentBoilTime = 0f;
                currentFirmness = NoodleFirmness.Raw;
                UpdateVisuals();
                Debug.Log($"[NoodleStation] Plunged {currentFirmness} noodles into bowl!");
            }
        }

        public void SetReferences(TMP_Text label, SpriteRenderer basket, SpriteRenderer noodleVisual)
        {
            statusLabel = label;
            basketRenderer = basket;
            noodleVisualInBasket = noodleVisual;
            UpdateVisuals();
        }

        private void UpdateTimerVisual()
        {
            if (timerBarTransform != null)
            {
                float progress = Mathf.Clamp01(currentBoilTime / yawaTime);
                timerBarTransform.localScale = new Vector3(progress, 1f, 1f);

                if (timerBarRenderer != null)
                {
                    switch (currentFirmness)
                    {
                        case NoodleFirmness.Raw: timerBarRenderer.color = Color.gray; break;
                        case NoodleFirmness.Kata: timerBarRenderer.color = new Color(0.2f, 0.8f, 1f); break;
                        case NoodleFirmness.Futsuu: timerBarRenderer.color = new Color(0.2f, 0.9f, 0.3f); break;
                        case NoodleFirmness.Yawa: timerBarRenderer.color = new Color(1f, 0.7f, 0.2f); break;
                        case NoodleFirmness.Overcooked: timerBarRenderer.color = new Color(0.9f, 0.2f, 0.2f); break;
                    }
                }
            }

            if (noodleVisualInBasket != null)
            {
                switch (currentFirmness)
                {
                    case NoodleFirmness.Raw: noodleVisualInBasket.color = new Color(0.96f, 0.96f, 0.90f); break;
                    case NoodleFirmness.Kata: noodleVisualInBasket.color = new Color(1.0f, 0.96f, 0.80f); break;
                    case NoodleFirmness.Futsuu: noodleVisualInBasket.color = new Color(1.0f, 0.90f, 0.65f); break;
                    case NoodleFirmness.Yawa: noodleVisualInBasket.color = new Color(1.0f, 0.82f, 0.55f); break;
                    case NoodleFirmness.Overcooked: noodleVisualInBasket.color = new Color(0.85f, 0.72f, 0.55f); break;
                }
            }

            if (statusLabel != null)
            {
                switch (currentFirmness)
                {
                    case NoodleFirmness.Raw:
                        statusLabel.text = $"<color=#CCCCCC>Boiling: <b>RAW</b> ({currentBoilTime:F1}s)</color>\n<size=80%>Wait for Futsuu...</size>";
                        break;
                    case NoodleFirmness.Kata:
                        statusLabel.text = $"<color=#55CCFF>Boiling: <b>KATA (Firm)</b> ({currentBoilTime:F1}s)</color>\n<size=80%>Almost ready...</size>";
                        break;
                    case NoodleFirmness.Futsuu:
                        statusLabel.text = $"<color=#44FF44>* <b>PERFECT FUTSUU!</b> ({currentBoilTime:F1}s) *</color>\n<size=85%><b><color=#FFFF88>Drag Basket to Bowl!</color></b></size>";
                        break;
                    case NoodleFirmness.Yawa:
                        statusLabel.text = $"<color=#FFAA33>Boiling: <b>YAWA (Soft)</b> ({currentBoilTime:F1}s)</color>\n<size=80%>Drag to Bowl now!</size>";
                        break;
                    case NoodleFirmness.Overcooked:
                        statusLabel.text = $"<color=#FF4444>Boiling: <b>OVERCOOKED</b> ({currentBoilTime:F1}s)</color>\n<size=80%>Drag to Bowl anyway</size>";
                        break;
                }
            }
        }

        private void UpdateVisuals()
        {
            if (timerBarTransform != null)
            {
                timerBarTransform.gameObject.SetActive(currentState == NoodleStationState.Boiling);
                timerBarTransform.localScale = Vector3.zero;
            }

            if (noodleVisualInBasket != null)
            {
                noodleVisualInBasket.gameObject.SetActive(currentState == NoodleStationState.Boiling);
            }

            if (statusLabel != null && currentState == NoodleStationState.Empty)
            {
                statusLabel.text = "<b>Noodle Basket</b>\n<size=80%><color=#888888>[Click to Boil]</color></size>";
            }
        }
    }
}
