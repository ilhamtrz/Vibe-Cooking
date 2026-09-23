using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace VibeCooking
{
    public class BowlInstance : MonoBehaviour
    {
        [Header("State")]
        [SerializeField] private bool hasBroth;
        [SerializeField] private IngredientDataSO currentBroth;
        [SerializeField] private bool hasNoodles;
        [SerializeField] private NoodleFirmness noodleFirmness = NoodleFirmness.Raw;
        [SerializeField] private List<IngredientDataSO> placedToppings = new List<IngredientDataSO>();

        [Header("Visual Components")]
        [SerializeField] private SpriteRenderer bowlBaseRenderer;
        [SerializeField] private SpriteRenderer brothRenderer;
        [SerializeField] private SpriteRenderer noodleRenderer;
        [SerializeField] private Transform toppingsContainer;

        [Header("UI Feedback")]
        [SerializeField] private TMP_Text statusLabel;

        [Header("Bowl Radius (for Plating)")]
        [SerializeField] private float bowlRadius = 1.2f;

        public bool HasBroth => hasBroth;
        public IngredientDataSO CurrentBroth => currentBroth;
        public bool HasNoodles => hasNoodles;
        public NoodleFirmness NoodleFirmness => noodleFirmness;
        public IReadOnlyList<IngredientDataSO> PlacedToppings => placedToppings;
        public float BowlRadius => bowlRadius;

        private List<GameObject> spawnedToppingObjects = new List<GameObject>();

        private void Awake()
        {
            UpdateVisuals();
        }

        public bool AddBroth(IngredientDataSO broth)
        {
            if (hasBroth) return false;

            hasBroth = true;
            currentBroth = broth;
            UpdateVisuals();

            GameEvents.TriggerBowlUpdated(this);
            Debug.Log($"[BowlInstance] Added broth: {broth.displayName}");
            return true;
        }

        public bool AddNoodles(NoodleFirmness firmness)
        {
            if (hasNoodles) return false;

            hasNoodles = true;
            noodleFirmness = firmness;
            UpdateVisuals();

            GameEvents.TriggerBowlUpdated(this);
            Debug.Log($"[BowlInstance] Added noodles with firmness: {firmness}");
            return true;
        }

        public bool AddTopping(IngredientDataSO topping, Vector2 worldPosition)
        {
            placedToppings.Add(topping);

            // Clamp position within bowl radius
            Vector2 localPos = (Vector2)transform.InverseTransformPoint(worldPosition);
            if (localPos.magnitude > bowlRadius)
            {
                localPos = localPos.normalized * bowlRadius * 0.85f;
            }

            // Spawn visual
            var toppingObj = new GameObject($"Topping_{topping.displayName}");
            toppingObj.transform.SetParent(toppingsContainer != null ? toppingsContainer : transform, false);
            toppingObj.transform.localPosition = localPos;

            var sr = toppingObj.AddComponent<SpriteRenderer>();
            sr.sprite = topping.bowlVisual != null ? topping.bowlVisual : topping.icon;
            sr.color = Color.white;
            sr.sortingOrder = 10 + placedToppings.Count;

            spawnedToppingObjects.Add(toppingObj);

            UpdateVisuals();
            GameEvents.TriggerBowlUpdated(this);
            Debug.Log($"[BowlInstance] Placed topping: {topping.displayName} at {localPos}");
            return true;
        }

        public void SetStatusLabel(TMP_Text label)
        {
            statusLabel = label;
            UpdateVisuals();
        }

        public void SetRenderers(SpriteRenderer broth, SpriteRenderer noodles)
        {
            brothRenderer = broth;
            noodleRenderer = noodles;
            UpdateVisuals();
        }

        public void ClearBowl()
        {
            hasBroth = false;
            currentBroth = null;
            hasNoodles = false;
            noodleFirmness = NoodleFirmness.Raw;
            placedToppings.Clear();

            foreach (var obj in spawnedToppingObjects)
            {
                if (obj != null) Destroy(obj);
            }
            spawnedToppingObjects.Clear();

            UpdateVisuals();
            GameEvents.TriggerBowlUpdated(this);
            Debug.Log("[BowlInstance] Bowl cleared and ready for new order.");
        }

        public void UpdateVisuals()
        {
            if (brothRenderer != null)
            {
                brothRenderer.gameObject.SetActive(hasBroth);
                if (hasBroth && currentBroth != null)
                {
                    if (currentBroth.bowlVisual != null)
                        brothRenderer.sprite = currentBroth.bowlVisual;
                    brothRenderer.color = Color.white;
                }
            }

            if (noodleRenderer != null)
            {
                noodleRenderer.gameObject.SetActive(hasNoodles);
                if (hasNoodles)
                {
                    switch (noodleFirmness)
                    {
                        case NoodleFirmness.Raw: noodleRenderer.color = new Color(0.96f, 0.96f, 0.90f); break;
                        case NoodleFirmness.Kata: noodleRenderer.color = new Color(1.0f, 0.96f, 0.80f); break;
                        case NoodleFirmness.Futsuu: noodleRenderer.color = new Color(1.0f, 0.90f, 0.65f); break;
                        case NoodleFirmness.Yawa: noodleRenderer.color = new Color(1.0f, 0.82f, 0.55f); break;
                        case NoodleFirmness.Overcooked: noodleRenderer.color = new Color(0.85f, 0.72f, 0.55f); break;
                    }
                }
            }

            if (statusLabel != null)
            {
                var sb = new System.Text.StringBuilder();
                sb.AppendLine("<b><size=120%>[RAMEN BOWL]</size></b>");
                if (!hasBroth && !hasNoodles && placedToppings.Count == 0)
                {
                    sb.AppendLine("<color=#AAAAAA>(Empty - Ready for Broth)</color>");
                }
                else
                {
                    string brothStr = (hasBroth && currentBroth != null) ? $"<color=#FFAA44>{currentBroth.displayName}</color>" : "<color=#777777>None</color>";
                    string noodleStr = hasNoodles ? $"<color=#55FF55>{noodleFirmness}</color>" : "<color=#777777>None</color>";

                    string toppingStr;
                    if (placedToppings.Count > 0)
                    {
                        var list = new List<string>();
                        foreach (var t in placedToppings) list.Add(t.displayName);
                        toppingStr = $"<color=#FFEE88>{string.Join(", ", list)}</color>";
                    }
                    else
                    {
                        toppingStr = "<color=#777777>None</color>";
                    }

                    sb.AppendLine($"Broth: {brothStr}");
                    sb.AppendLine($"Noodles: {noodleStr}");
                    sb.AppendLine($"Toppings: {toppingStr}");
                }
                statusLabel.text = sb.ToString();
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, bowlRadius);
        }
    }
}
