using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace VibeCooking
{
    public enum CustomerState
    {
        Arriving,
        WaitingForOrder,
        Eating,
        Departing
    }

    public class CustomerAgent : MonoBehaviour
    {
        [Header("Customer Data")]
        [SerializeField] private CustomerDataSO customerData;
        [SerializeField] private RecipeDataSO currentRecipe;
        [SerializeField] private NoodleFirmness requestedFirmness = NoodleFirmness.Futsuu;

        [Header("Visual Components")]
        [SerializeField] private SpriteRenderer portraitRenderer;
        [SerializeField] private GameObject speechBubbleObject;
        [SerializeField] private TMP_Text speechText;

        [Header("State")]
        [SerializeField] private CustomerState currentState = CustomerState.Arriving;

        private OrderTicket activeTicket;
        private int orderCounter = 1;

        public CustomerDataSO CustomerData => customerData;
        public RecipeDataSO CurrentRecipe => currentRecipe;
        public NoodleFirmness RequestedFirmness => requestedFirmness;
        public CustomerState CurrentState => currentState;

        private void Start()
        {
            SetupCustomerDataIfNull();
            SpawnNewOrder();
        }

        private void SetupCustomerDataIfNull()
        {
            if (customerData == null)
            {
                customerData = Resources.Load<CustomerDataSO>("ScriptableObjects/Customers/NightCoderCustomer");
                if (customerData == null)
                {
                    // Fallback search
                    var allCustomers = Resources.FindObjectsOfTypeAll<CustomerDataSO>();
                    if (allCustomers.Length > 0) customerData = allCustomers[0];
                }
            }

            if (portraitRenderer != null && customerData != null && customerData.portrait != null)
            {
                portraitRenderer.sprite = customerData.portrait;
            }
        }

        public void SpawnNewOrder()
        {
            currentState = CustomerState.WaitingForOrder;

            // Pick recipe
            if (customerData != null && customerData.favoriteRecipes.Count > 0)
            {
                currentRecipe = customerData.favoriteRecipes[0];
                requestedFirmness = customerData.preferredFirmness;
            }

            // Arrival dialogue
            string arrivalMsg = "Been debugging for 6 hours straight...\nCould I get a warm <b>Classic Shoyu Ramen</b> with <b>Futsuu</b> noodles?";
            if (customerData != null && customerData.arrivalDialogues.Count > 0)
            {
                arrivalMsg = customerData.arrivalDialogues[Random.Range(0, customerData.arrivalDialogues.Count)];
            }
            SetSpeechDialogue(arrivalMsg);

            // Create OrderTicket
            if (activeTicket == null)
            {
                var ticketObj = new GameObject("ActiveOrderTicket");
                ticketObj.transform.SetParent(transform, false);
                activeTicket = ticketObj.AddComponent<OrderTicket>();
            }

            activeTicket.Initialize(orderCounter++, currentRecipe, requestedFirmness, customerData);
            GameEvents.TriggerOrderCreated(activeTicket);
            Debug.Log($"<color=cyan>[CustomerAgent] New Order #{activeTicket.OrderNumber}: {currentRecipe.recipeName} ({requestedFirmness})</color>");
        }

        public void ServeOrder(BowlInstance bowl)
        {
            if (currentState != CustomerState.WaitingForOrder || bowl == null || currentRecipe == null)
                return;

            currentState = CustomerState.Eating;

            // --- Recipe Evaluation Algorithm ---
            int basePrice = currentRecipe.basePrice; // e.g. 800
            int firmnessBonus = 0;
            int presentationBonus = 0;
            int accuracyScore = 0;

            // 1. Broth Check
            bool brothCorrect = (bowl.HasBroth && bowl.CurrentBroth != null &&
                                 currentRecipe.requiredBroth != null &&
                                 bowl.CurrentBroth.id == currentRecipe.requiredBroth.id);
            if (brothCorrect) accuracyScore += 40;
            else if (bowl.HasBroth) accuracyScore += 20;

            // 2. Noodles & Firmness Check
            if (bowl.HasNoodles)
            {
                accuracyScore += 30;

                if (bowl.NoodleFirmness == requestedFirmness)
                {
                    firmnessBonus = currentRecipe.firmnessBonus; // +150
                }
                else if ((requestedFirmness == NoodleFirmness.Futsuu && (bowl.NoodleFirmness == NoodleFirmness.Kata || bowl.NoodleFirmness == NoodleFirmness.Yawa)))
                {
                    firmnessBonus = 50; // Close enough tip
                }
            }

            // 3. Toppings Check
            int matchedToppings = 0;
            var placedCopy = new List<IngredientDataSO>(bowl.PlacedToppings);

            foreach (var req in currentRecipe.requiredToppings)
            {
                var match = placedCopy.Find(p => p != null && p.id == req.id);
                if (match != null)
                {
                    matchedToppings++;
                    placedCopy.Remove(match);
                }
            }

            if (currentRecipe.requiredToppings.Count > 0)
            {
                float toppingFraction = (float)matchedToppings / currentRecipe.requiredToppings.Count;
                accuracyScore += Mathf.RoundToInt(toppingFraction * 30f);
            }

            // 4. Presentation Neatness Bonus
            if (brothCorrect && bowl.HasNoodles && matchedToppings >= currentRecipe.requiredToppings.Count)
            {
                presentationBonus = currentRecipe.presentationBonus; // +100
            }

            // Total Payout calculation (Non-punitive: minimum 50% base payout)
            int earnedBase = Mathf.Max(Mathf.RoundToInt(basePrice * (accuracyScore / 100f)), basePrice / 2);
            int totalPayout = earnedBase + firmnessBonus + presentationBonus;

            // Dialogue reaction
            string feedback;
            if (accuracyScore >= 80 && firmnessBonus > 0)
            {
                feedback = $"<color=#FFE899>\"Ahhh, this warms my soul! The {bowl.NoodleFirmness} noodles are cooked to perfection. Thank you chef!\"</color>";
            }
            else if (accuracyScore >= 50)
            {
                feedback = "<color=#FFF>\"Mmm, piping hot ramen always hits the spot. Thanks for the meal!\"</color>";
            }
            else
            {
                feedback = "<color=#E0E0E0>\"A unique experimental bowl! Appreciate the warm food, chef.\"</color>";
            }

            SetSpeechDialogue(feedback);

            // Award Money
            if (EconomyManager.Instance != null)
            {
                EconomyManager.Instance.AddMoney(totalPayout, $"Order #{activeTicket.OrderNumber} Served");
            }

            GameEvents.TriggerOrderServed(bowl, this);

            // Eating sequence then clear & reset
            StartCoroutine(EatingAndResetRoutine(bowl));
        }

        private IEnumerator EatingAndResetRoutine(BowlInstance bowl)
        {
            yield return new WaitForSeconds(3.5f);

            if (bowl != null)
            {
                bowl.ClearBowl();
            }

            // Brief pause before next customer arrives
            yield return new WaitForSeconds(1.0f);
            SpawnNewOrder();
        }

        public void SetSpeechDialogue(string text)
        {
            if (speechBubbleObject != null)
                speechBubbleObject.SetActive(true);

            if (speechText != null)
                speechText.text = text;
        }

        public void SetReferences(SpriteRenderer portrait, GameObject speechBubble, TMP_Text speechTMP, CustomerDataSO data, RecipeDataSO recipe)
        {
            portraitRenderer = portrait;
            speechBubbleObject = speechBubble;
            speechText = speechTMP;
            customerData = data;
            currentRecipe = recipe;
        }
    }
}
