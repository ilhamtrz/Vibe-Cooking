using System.Collections.Generic;
using UnityEngine;

namespace VibeCooking
{
    [CreateAssetMenu(fileName = "NewCustomer", menuName = "Vibe Cooking/Customer Data")]
    public class CustomerDataSO : ScriptableObject
    {
        [Header("Identity")]
        public string customerName;
        public string title;
        public Sprite portrait;

        [Header("Dialogues")]
        [TextArea(2, 4)]
        public List<string> arrivalDialogues = new List<string>();
        [TextArea(2, 4)]
        public List<string> satisfactionDialogues = new List<string>();

        [Header("Preferences")]
        public List<RecipeDataSO> favoriteRecipes = new List<RecipeDataSO>();
        public NoodleFirmness preferredFirmness = NoodleFirmness.Futsuu;
    }
}
