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

            // 2. Recipes
            var classicRecipe = CreateOrUpdateRecipe("ClassicShoyuRamen", "Classic Shoyu Ramen",
                "A comforting bowl of traditional soy sauce broth with springy noodles, braised chashu, soft-boiled egg, and freshly chopped scallions.",
                shoyu, NoodleFirmness.Futsuu, 800, chashu, tamago, scallions);

            var chashuRecipe = CreateOrUpdateRecipe("SavoryChashuShoyu", "Chashu Shoyu Ramen",
                "A savory, rich bowl focusing on tender simmered pork and scallions with crisp, firm (Kata) noodles.",
                shoyu, NoodleFirmness.Kata, 750, chashu, scallions);

            var eggRecipe = CreateOrUpdateRecipe("ComfortEggShoyu", "Tamago Comfort Ramen",
                "A soothing bowl featuring seasoned soft-boiled ajitsuke tamago and scallions over tender soft (Yawa) noodles.",
                shoyu, NoodleFirmness.Yawa, 700, tamago, scallions);

            var heartyRecipe = CreateOrUpdateRecipe("HeartyDoubleMeatShoyu", "Chashu & Egg Shoyu",
                "A protein-packed bowl loaded with braised chashu pork and seasoned egg, sans greens.",
                shoyu, NoodleFirmness.Futsuu, 820, chashu, tamago);

            // 3. Customers
            // Customer 1: The Night Coder
            CreateOrUpdateCustomer("NightCoderCustomer", "The Night Coder", "Sleepy Software Developer",
                new string[]
                {
                    "I've been debugging a race condition for 6 hours... please give me a warm bowl of Classic Shoyu ramen with Futsuu noodles.",
                    "Still waiting for CI to pass... smelled your stall from across the alley. A classic bowl with standard (Futsuu) noodles please!"
                },
                new string[]
                {
                    "*Slurp*... Ahh, this is giving me the dopamine I needed. Back to fixing the memory leak!",
                    "Delicious. My code will compile on the first try now, I can feel it."
                },
                "spr_customer_night_coder.png",
                NoodleFirmness.Futsuu,
                classicRecipe);

            // Customer 2: The Vinyl Digger
            CreateOrUpdateCustomer("VinylDiggerCustomer", "The Vinyl Digger", "Lo-Fi Beatmaker & Record Collector",
                new string[]
                {
                    "Yo chef! Just found a rare city-pop vinyl down the block. Can I get a Chashu Shoyu with firm (Kata) noodles? Need that crisp bite!",
                    "Late night crate-digging got me starved. A bowl of Chashu Shoyu please, keep the noodles Kata (firm)!"
                },
                new string[]
                {
                    "*Slurp*... Oh man, that crisp Kata texture hits like a fresh snare drum! Pure lo-fi bliss.",
                    "Top tier vibes, chef. That savory chashu and firm noodle combo is a whole groove."
                },
                "spr_customer_vinyl_digger.png",
                NoodleFirmness.Kata,
                chashuRecipe);

            // Customer 3: The Rainy Student
            CreateOrUpdateCustomer("RainyStudentCustomer", "The Rainy Student", "Late-Night Exam Studier",
                new string[]
                {
                    "Evening, chef... Cramming for tomorrow's literature exam in the rain. Could I get a Tamago Comfort ramen with soft (Yawa) noodles?",
                    "The rain outside is so chilly... A warm bowl with extra soft (Yawa) noodles and seasoned egg would be so comforting."
                },
                new string[]
                {
                    "*Sip*... So warm and soothing. The soft noodles and sweet tamago are exactly what I needed to finish studying.",
                    "My shoulders finally relaxed. Thank you chef, this gave me strength for tomorrow's test!"
                },
                "spr_customer_rainy_student.png",
                NoodleFirmness.Yawa,
                eggRecipe);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("<color=green>[VibeCooking] Initial ScriptableObject assets (Recipes & Customers) generated successfully!</color>");
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
            string[] arrivals, string[] satisfactions, string spriteFileName, NoodleFirmness preferredFirmness,
            params RecipeDataSO[] favorites)
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
            customer.favoriteRecipes.AddRange(favorites);
            customer.preferredFirmness = preferredFirmness;

            if (!string.IsNullOrEmpty(spriteFileName))
            {
                var sprite = AssetDatabase.LoadAssetAtPath<Sprite>($"Assets/_Project/Art/{spriteFileName}");
                if (sprite != null)
                {
                    customer.portrait = sprite;
                }
            }

            EditorUtility.SetDirty(customer);
            return customer;
        }
    }
}
#endif
