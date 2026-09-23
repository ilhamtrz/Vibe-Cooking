using System.Collections.Generic;
using UnityEngine;

namespace VibeCooking
{
    [CreateAssetMenu(fileName = "NewRecipe", menuName = "Vibe Cooking/Recipe Data")]
    public class RecipeDataSO : ScriptableObject
    {
        [Header("Recipe Details")]
        public string recipeName;
        [TextArea(2, 3)]
        public string description;

        [Header("Broth & Base")]
        public IngredientDataSO requiredBroth;
        public NoodleFirmness requiredFirmness = NoodleFirmness.Futsuu;

        [Header("Toppings")]
        public List<IngredientDataSO> requiredToppings = new List<IngredientDataSO>();

        [Header("Economy")]
        public int basePrice = 800;
        public int firmnessBonus = 150;
        public int presentationBonus = 100;
    }
}
