using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace VibeCooking
{
    public class OrderTicketUI : MonoBehaviour
    {
        [Header("UI Text References")]
        [SerializeField] private TMP_Text orderNumberText;
        [SerializeField] private TMP_Text recipeTitleText;
        [SerializeField] private TMP_Text detailsText;
        [SerializeField] private TMP_Text rewardText;

        [Header("Root Panel")]
        [SerializeField] private GameObject ticketPanel;

        private void OnEnable()
        {
            GameEvents.OnOrderCreated += HandleOrderCreated;
            GameEvents.OnOrderServed += HandleOrderServed;
        }

        private void OnDisable()
        {
            GameEvents.OnOrderCreated -= HandleOrderCreated;
            GameEvents.OnOrderServed -= HandleOrderServed;
        }

        public void DisplayOrder(OrderTicket ticket)
        {
            if (ticket == null || ticket.Recipe == null)
            {
                if (ticketPanel != null) ticketPanel.SetActive(false);
                return;
            }

            if (ticketPanel != null) ticketPanel.SetActive(true);

            if (orderNumberText != null)
                orderNumberText.text = $"ORDER #{ticket.OrderNumber:D2}";

            if (recipeTitleText != null)
                recipeTitleText.text = ticket.Recipe.recipeName;

            if (detailsText != null)
            {
                string broth = ticket.Recipe.requiredBroth != null ? ticket.Recipe.requiredBroth.displayName : "Shoyu";
                string firmness = ticket.RequestedFirmness.ToString();

                var toppingsList = new List<string>();
                foreach (var t in ticket.Recipe.requiredToppings)
                {
                    if (t != null) toppingsList.Add(t.displayName);
                }
                string toppings = toppingsList.Count > 0 ? string.Join("\n• ", toppingsList) : "None";

                detailsText.text = $"<b>Broth:</b> {broth}\n" +
                                   $"<b>Noodles:</b> <color=#2E7D32>{firmness}</color>\n" +
                                   $"<b>Toppings:</b>\n• {toppings}";
            }

            if (rewardText != null)
            {
                rewardText.text = $"Base: ¥{ticket.Recipe.basePrice} + Tip";
            }
        }

        private void HandleOrderCreated(OrderTicket ticket)
        {
            DisplayOrder(ticket);
        }

        private void HandleOrderServed(BowlInstance bowl, CustomerAgent customer)
        {
            // Ticket completed
            if (detailsText != null)
            {
                detailsText.text += "\n\n<color=#2E7D32><b>[ORDER SERVED!]</b></color>";
            }
        }
    }
}
