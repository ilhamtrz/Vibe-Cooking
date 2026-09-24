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
        [Header("Customer Pool")]
        [SerializeField] private List<CustomerDataSO> customerPool = new List<CustomerDataSO>();
        private int lastCustomerIndex = -1;

        [Header("Active Customer Data")]
        [SerializeField] private CustomerDataSO customerData;
        [SerializeField] private RecipeDataSO currentRecipe;
        [SerializeField] private NoodleFirmness requestedFirmness = NoodleFirmness.Futsuu;

        [Header("Visual Components")]
        [SerializeField] private SpriteRenderer portraitRenderer;
        [SerializeField] private GameObject speechBubbleObject;
        [SerializeField] private TMP_Text speechText;

        [Header("State")]
        [SerializeField] private CustomerState currentState = CustomerState.WaitingForOrder;

        [SerializeField] private OrderTicket activeTicket;
        private int orderCounter = 1;

        public List<CustomerDataSO> CustomerPool => customerPool;
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
            if (customerPool == null) customerPool = new List<CustomerDataSO>();

            if (customerPool.Count == 0)
            {
                var loaded = Resources.FindObjectsOfTypeAll<CustomerDataSO>();
                if (loaded != null && loaded.Length > 0)
                {
                    customerPool.AddRange(loaded);
                }
            }

            if (customerData == null && customerPool.Count > 0)
            {
                customerData = customerPool[0];
            }
            else if (customerData == null)
            {
                customerData = Resources.Load<CustomerDataSO>("ScriptableObjects/Customers/NightCoderCustomer");
            }
        }

        public void SetCustomerPool(List<CustomerDataSO> pool)
        {
            customerPool.Clear();
            if (pool != null) customerPool.AddRange(pool);
        }

        public void SpawnNewOrder()
        {
            currentState = CustomerState.WaitingForOrder;

            // 1. Pick Customer from Pool (avoid immediate repeat if pool > 1)
            if (customerPool != null && customerPool.Count > 0)
            {
                int chosenIndex = Random.Range(0, customerPool.Count);
                if (customerPool.Count > 1 && chosenIndex == lastCustomerIndex)
                {
                    chosenIndex = (chosenIndex + 1) % customerPool.Count;
                }
                lastCustomerIndex = chosenIndex;
                customerData = customerPool[chosenIndex];
            }

            // 2. Update Portrait
            if (portraitRenderer != null && customerData != null && customerData.portrait != null)
            {
                portraitRenderer.sprite = customerData.portrait;
            }

            // 3. Pick Recipe & Firmness
            if (customerData != null && customerData.favoriteRecipes.Count > 0)
            {
                currentRecipe = customerData.favoriteRecipes[Random.Range(0, customerData.favoriteRecipes.Count)];
                requestedFirmness = customerData.preferredFirmness;
            }

            // 4. Arrival dialogue
            string arrivalMsg = "Could I get a warm bowl of ramen?";
            if (customerData != null && customerData.arrivalDialogues.Count > 0)
            {
                arrivalMsg = customerData.arrivalDialogues[Random.Range(0, customerData.arrivalDialogues.Count)];
            }
            SetSpeechDialogue(arrivalMsg);

            // 5. Create / Update OrderTicket
            if (activeTicket == null)
            {
                var ticketObj = new GameObject("ActiveOrderTicket");
                ticketObj.transform.SetParent(transform, false);
                activeTicket = ticketObj.AddComponent<OrderTicket>();
            }

            activeTicket.Initialize(orderCounter++, currentRecipe, requestedFirmness, customerData);
            GameEvents.TriggerOrderCreated(activeTicket);
            Debug.Log($"<color=cyan>[CustomerAgent] Customer '{customerData?.customerName}' ordered #{activeTicket.OrderNumber}: {currentRecipe?.recipeName} ({requestedFirmness})</color>");
        }

        public void ServeOrder(BowlInstance bowl)
        {
            if (currentState == CustomerState.Eating || currentState == CustomerState.Departing || bowl == null || currentRecipe == null)
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
                string praise = (customerData != null && customerData.satisfactionDialogues.Count > 0)
                    ? customerData.satisfactionDialogues[Random.Range(0, customerData.satisfactionDialogues.Count)]
                    : $"Ahhh, this warms my soul! The {bowl.NoodleFirmness} noodles are cooked to perfection. Thank you chef!";
                feedback = $"<color=#FFE899>\"{praise}\"</color>";
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

            if (activeTicket == null)
                activeTicket = GetComponentInChildren<OrderTicket>();

            int orderNum = activeTicket != null ? activeTicket.OrderNumber : orderCounter;

            // Award Money
            if (EconomyManager.Instance != null)
            {
                EconomyManager.Instance.AddMoney(totalPayout, $"Order #{orderNum} Served");
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

        public void SetReferences(SpriteRenderer portrait, GameObject speechBubble, TMP_Text speechTMP, CustomerDataSO data, RecipeDataSO recipe, List<CustomerDataSO> pool = null)
        {
            portraitRenderer = portrait;
            speechBubbleObject = speechBubble;
            speechText = speechTMP;
            customerData = data;
            currentRecipe = recipe;
            if (pool != null)
            {
                customerPool.Clear();
                customerPool.AddRange(pool);
            }
        }
    }
}
