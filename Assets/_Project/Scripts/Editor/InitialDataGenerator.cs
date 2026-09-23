#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

namespace VibeCooking.Editor
{
    public static class InitialDataGenerator
    {
        private const string IngredientsPath = "Assets/_Project/ScriptableObjects/Ingredients";
        private const string RecipesPath = "Assets/_Project/ScriptableObjects/Recipes";
        private const string CustomersPath = "Assets/_Project/ScriptableObjects/Customers";

        [MenuItem("Vibe Cooking/Generate Initial Data")]
        public static void GenerateData()
        {
            EnsureFolders();

            // 1. Ingredients
            var shoyu = CreateOrUpdateIngredient("ShoyuBroth", "shoyu_broth", "Shoyu Broth", IngredientType.Broth, new Color(0.55f, 0.27f, 0.07f), 0);
            var noodles = CreateOrUpdateIngredient("ThinNoodles", "thin_noodles", "Thin Noodles", IngredientType.Noodle, new Color(1.0f, 0.97f, 0.86f), 0);
            var chashu = CreateOrUpdateIngredient("Chashu", "chashu", "Chashu Pork", IngredientType.Meat, new Color(0.80f, 0.52f, 0.25f), 100);
            var tamago = CreateOrUpdateIngredient("TamagoEgg", "tamago_egg", "Ajitsuke Tamago", IngredientType.Egg, new Color(1.0f, 0.65f, 0.0f), 80);
            var scallions = CreateOrUpdateIngredient("Scallions", "scallions", "Scallions", IngredientType.Garnish, new Color(0.20f, 0.80f, 0.20f), 30);

            // 2. Recipe
            var recipe = CreateOrUpdateRecipe("ClassicShoyuRamen", "Classic Shoyu Ramen",
                "A comforting bowl of traditional soy sauce broth with springy noodles, braised chashu, soft-boiled egg, and freshly chopped scallions.",
                shoyu, NoodleFirmness.Futsuu, 800, chashu, tamago, scallions);

            // 3. Customer
            CreateOrUpdateCustomer("NightCoderCustomer", "The Night Coder", "Sleepy Software Developer",
                new string[]
                {
                    "I've been debugging a race condition for 6 hours... please give me a warm bowl of Shoyu ramen.",
                    "Still waiting for CI to pass... smelled your stall from across the alley."
                },
                new string[]
                {
                    "*Slurp*... Ahh, this is giving me the dopamine I needed. Back to fixing the memory leak!",
                    "Delicious. My code will compile on the first try now, I can feel it."
                },
                recipe, NoodleFirmness.Futsuu);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("<color=green>[VibeCooking] Initial ScriptableObject assets generated successfully!</color>");
        }

        private static void EnsureFolders()
        {
            if (!AssetDatabase.IsValidFolder(IngredientsPath))
                Directory.CreateDirectory(IngredientsPath);
            if (!AssetDatabase.IsValidFolder(RecipesPath))
                Directory.CreateDirectory(RecipesPath);
            if (!AssetDatabase.IsValidFolder(CustomersPath))
                Directory.CreateDirectory(CustomersPath);
        }

        private static IngredientDataSO CreateOrUpdateIngredient(string assetName, string id, string displayName, IngredientType type, Color color, int cost)
        {
            string path = $"{IngredientsPath}/{assetName}.asset";
            var item = AssetDatabase.LoadAssetAtPath<IngredientDataSO>(path);
            if (item == null)
            {
                item = ScriptableObject.CreateInstance<IngredientDataSO>();
                AssetDatabase.CreateAsset(item, path);
            }

            item.id = id;
            item.displayName = displayName;
            item.type = type;
            item.placeholderColor = color;
            item.cost = cost;

            EditorUtility.SetDirty(item);
            return item;
        }

        private static RecipeDataSO CreateOrUpdateRecipe(string assetName, string recipeName, string desc,
            IngredientDataSO broth, NoodleFirmness firmness, int basePrice, params IngredientDataSO[] toppings)
        {
            string path = $"{RecipesPath}/{assetName}.asset";
            var recipe = AssetDatabase.LoadAssetAtPath<RecipeDataSO>(path);
            if (recipe == null)
            {
                recipe = ScriptableObject.CreateInstance<RecipeDataSO>();
                AssetDatabase.CreateAsset(recipe, path);
            }

            recipe.recipeName = recipeName;
            recipe.description = desc;
            recipe.requiredBroth = broth;
            recipe.requiredFirmness = firmness;
            recipe.basePrice = basePrice;
            recipe.requiredToppings.Clear();
            recipe.requiredToppings.AddRange(toppings);

            EditorUtility.SetDirty(recipe);
            return recipe;
        }

        private static CustomerDataSO CreateOrUpdateCustomer(string assetName, string customerName, string title,
            string[] arrivals, string[] satisfactions, RecipeDataSO favorite, NoodleFirmness preferredFirmness)
        {
            string path = $"{CustomersPath}/{assetName}.asset";
            var customer = AssetDatabase.LoadAssetAtPath<CustomerDataSO>(path);
            if (customer == null)
            {
                customer = ScriptableObject.CreateInstance<CustomerDataSO>();
                AssetDatabase.CreateAsset(customer, path);
            }

            customer.customerName = customerName;
            customer.title = title;
            customer.arrivalDialogues.Clear();
            customer.arrivalDialogues.AddRange(arrivals);
            customer.satisfactionDialogues.Clear();
            customer.satisfactionDialogues.AddRange(satisfactions);
            customer.favoriteRecipes.Clear();
            customer.favoriteRecipes.Add(favorite);
            customer.preferredFirmness = preferredFirmness;

            EditorUtility.SetDirty(customer);
            return customer;
        }
    }
}
#endif
