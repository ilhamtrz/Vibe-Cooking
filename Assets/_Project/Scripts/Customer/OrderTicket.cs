using System.Collections.Generic;
using UnityEngine;

namespace VibeCooking
{
    public class OrderTicket : MonoBehaviour
    {
        [Header("Order Information")]
        [SerializeField] private int orderNumber = 1;
        [SerializeField] private RecipeDataSO recipe;
        [SerializeField] private NoodleFirmness requestedFirmness = NoodleFirmness.Futsuu;
        [SerializeField] private CustomerDataSO customer;

        public int OrderNumber => orderNumber;
        public RecipeDataSO Recipe => recipe;
        public NoodleFirmness RequestedFirmness => requestedFirmness;
        public CustomerDataSO Customer => customer;

        public void Initialize(int number, RecipeDataSO orderRecipe, NoodleFirmness firmness, CustomerDataSO orderingCustomer)
        {
            orderNumber = number;
            recipe = orderRecipe;
            requestedFirmness = firmness;
            customer = orderingCustomer;
        }

        public string GetOrderSummary()
        {
            if (recipe == null) return "No active order";

            var toppingsList = new List<string>();
            foreach (var t in recipe.requiredToppings)
            {
                if (t != null) toppingsList.Add(t.displayName);
            }

            string toppingsStr = toppingsList.Count > 0 ? string.Join(", ", toppingsList) : "None";
            string brothStr = recipe.requiredBroth != null ? recipe.requiredBroth.displayName : "None";

            return $"<b>{recipe.recipeName}</b>\n" +
                   $"Broth: {brothStr}\n" +
                   $"Firmness: {requestedFirmness}\n" +
                   $"Toppings: {toppingsStr}\n" +
                   $"Reward: ¥{recipe.basePrice} + Tip";
        }
    }
}
