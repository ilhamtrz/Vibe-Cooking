using System.Collections;
using UnityEngine;
using TMPro;

namespace VibeCooking
{
    public class WalletHUD : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TMP_Text balanceText;
        [SerializeField] private TMP_Text floatingPopupText;

        private Coroutine popupCoroutine;

        private void OnEnable()
        {
            GameEvents.OnMoneyChanged += HandleMoneyChanged;
        }

        private void OnDisable()
        {
            GameEvents.OnMoneyChanged -= HandleMoneyChanged;
        }

        private void Start()
        {
            if (floatingPopupText != null)
            {
                floatingPopupText.gameObject.SetActive(false);
            }

            if (EconomyManager.Instance != null)
            {
                UpdateBalanceDisplay(EconomyManager.Instance.CurrentYen);
            }
        }

        private void HandleMoneyChanged(int totalYen, int delta)
        {
            UpdateBalanceDisplay(totalYen);

            if (delta > 0 && floatingPopupText != null)
            {
                if (popupCoroutine != null)
                    StopCoroutine(popupCoroutine);
                popupCoroutine = StartCoroutine(ShowFloatingReward($"+¥{delta}"));
            }
        }

        private void UpdateBalanceDisplay(int yen)
        {
            if (balanceText != null)
            {
                balanceText.text = $"<b>¥ {yen:N0}</b>";
            }
        }

        private IEnumerator ShowFloatingReward(string text)
        {
            floatingPopupText.gameObject.SetActive(true);
            floatingPopupText.text = text;

            var rt = floatingPopupText.rectTransform;
            Vector2 startPos = new Vector2(0f, -40f);
            Vector2 endPos = new Vector2(0f, 10f);

            Color initialColor = new Color(1f, 0.88f, 0.2f, 1f); // Gold
            floatingPopupText.color = initialColor;

            float duration = 1.8f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;

                // Move upward
                rt.anchoredPosition = Vector2.Lerp(startPos, endPos, Mathf.SmoothStep(0f, 1f, t));

                // Fade out near the end
                if (t > 0.6f)
                {
                    float alpha = Mathf.Lerp(1f, 0f, (t - 0.6f) / 0.4f);
                    floatingPopupText.color = new Color(initialColor.r, initialColor.g, initialColor.b, alpha);
                }

                yield return null;
            }

            floatingPopupText.gameObject.SetActive(false);
            popupCoroutine = null;
        }
    }
}
