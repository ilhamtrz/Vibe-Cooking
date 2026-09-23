using UnityEngine;

namespace VibeCooking
{
    [CreateAssetMenu(fileName = "NewIngredient", menuName = "Vibe Cooking/Ingredient Data")]
    public class IngredientDataSO : ScriptableObject
    {
        [Header("Identity")]
        public string id;
        public string displayName;
        public IngredientType type;

        [Header("Visuals")]
        public Sprite icon;          // Displayed in UI / trays
        public Sprite bowlVisual;    // Displayed when plated in the bowl
        public Color placeholderColor = Color.white; // For initial 2D prototyping

        [Header("Audio")]
        public AudioClip placeSound;

        [Header("Economics")]
        public int cost;
    }
}
