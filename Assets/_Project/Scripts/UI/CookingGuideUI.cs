using UnityEngine;
using TMPro;

namespace VibeCooking
{
    public class CookingGuideUI : MonoBehaviour
    {
        [Header("Target Stations")]
        [SerializeField] private BowlInstance bowl;
        [SerializeField] private NoodleStation noodleStation;

        [Header("UI Component")]
        [SerializeField] private TMP_Text guideText;

        private void Start()
        {
            FindReferencesIfNull();
        }

        private void Update()
        {
            FindReferencesIfNull();
            UpdateGuidePrompt();
        }

        private void FindReferencesIfNull()
        {
            if (bowl == null)
                bowl = Object.FindAnyObjectByType<BowlInstance>();

            if (noodleStation == null)
                noodleStation = Object.FindAnyObjectByType<NoodleStation>();
        }

        private void UpdateGuidePrompt()
        {
            if (guideText == null || bowl == null) return;

            // Step 1: Broth
            if (!bowl.HasBroth)
            {
                guideText.text = "<b>STEP 1: Pour Broth</b> — Drag the <color=#FFAA44><b>Shoyu Pot</b></color> into the Ramen Bowl";
                return;
            }

            // Step 2: Noodles
            if (!bowl.HasNoodles)
            {
                if (noodleStation == null || noodleStation.CurrentState == NoodleStationState.Empty)
                {
                    guideText.text = "<b>STEP 2: Boil Noodles</b> — Click the <color=#55CCFF><b>Noodle Basket</b></color> to start boiling";
                }
                else
                {
                    switch (noodleStation.CurrentFirmness)
                    {
                        case NoodleFirmness.Raw:
                            guideText.text = "<b>STEP 2: Boiling Noodles...</b> Watch timer bar, wait for <color=#44FF44><b>Futsuu (Green)</b></color>";
                            break;
                        case NoodleFirmness.Kata:
                            guideText.text = "<b>STEP 2: Almost Cooked!</b> A few seconds more for standard <color=#44FF44><b>Futsuu</b></color>";
                            break;
                        case NoodleFirmness.Futsuu:
                            guideText.text = "<color=#FFFF44><b>PERFECT COOK!</b></color> Drag the <color=#FFFF44><b>Noodle Basket</b></color> to the Bowl now!";
                            break;
                        case NoodleFirmness.Yawa:
                            guideText.text = "<color=#FFAA33><b>Noodles Softening (Yawa)!</b></color> Drag the Basket to the Bowl now!";
                            break;
                        case NoodleFirmness.Overcooked:
                            guideText.text = "<color=#FF4444><b>Overcooked!</b></color> Drag the Basket to the Bowl to finish.";
                            break;
                    }
                }
                return;
            }

            // Step 3: Toppings
            if (bowl.PlacedToppings.Count == 0)
            {
                guideText.text = "<b>STEP 3: Add Toppings</b> — Drag <color=#FFAA44><b>Chashu</b></color>, <color=#FFDD44><b>Tamago</b></color>, or <color=#55FF55><b>Scallions</b></color> into the Bowl";
                return;
            }

            // Step 4: Serve
            guideText.text = $"<b>STEP 4: Serve Ramen</b> — Click the <color=#FFDD44><b>Serve Bell</b></color> below the Bowl! ({bowl.PlacedToppings.Count} topping{(bowl.PlacedToppings.Count > 1 ? "s" : "")} added)";
        }
    }
}
