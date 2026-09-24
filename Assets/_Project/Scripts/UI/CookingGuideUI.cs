using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace VibeCooking
{
    public class CookingGuideUI : MonoBehaviour
    {
        [Header("Target Stations")]
        [SerializeField] private BowlInstance bowl;
        [SerializeField] private NoodleStation noodleStation;
        [SerializeField] private CustomerAgent customerAgent;

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

            if (customerAgent == null)
                customerAgent = Object.FindAnyObjectByType<CustomerAgent>();
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
                var reqFirmness = (customerAgent != null) ? customerAgent.RequestedFirmness : NoodleFirmness.Futsuu;

                if (noodleStation == null || noodleStation.CurrentState == NoodleStationState.Empty)
                {
                    guideText.text = $"<b>STEP 2: Boil Noodles</b> — Click <color=#55CCFF><b>Noodle Basket</b></color> (Order wants <color=#FFFF44><b>{reqFirmness}</b></color>)";
                }
                else
                {
                    var current = noodleStation.CurrentFirmness;
                    if (reqFirmness == NoodleFirmness.Kata)
                    {
                        switch (current)
                        {
                            case NoodleFirmness.Raw:
                                guideText.text = "<b>STEP 2: Boiling...</b> Order wants <color=#FFAA44><b>Kata (Firm / Orange)</b></color>. Keep watch!";
                                break;
                            case NoodleFirmness.Kata:
                                guideText.text = "<color=#FFFF44><b>PERFECT KATA (FIRM)!</b></color> Drag <color=#FFFF44><b>Noodle Basket</b></color> to Bowl now!";
                                break;
                            case NoodleFirmness.Futsuu:
                                guideText.text = "<color=#FFAA33><b>Past Kata (Now Futsuu)!</b></color> Drag Basket to Bowl before it softens!";
                                break;
                            default:
                                guideText.text = "<color=#FF4444><b>Overcooked!</b></color> Drag Basket to Bowl to finish.";
                                break;
                        }
                    }
                    else if (reqFirmness == NoodleFirmness.Yawa)
                    {
                        switch (current)
                        {
                            case NoodleFirmness.Raw:
                            case NoodleFirmness.Kata:
                                guideText.text = "<b>STEP 2: Boiling...</b> Order wants <color=#FFEE66><b>Yawa (Soft / Yellow)</b></color>. Let it simmer!";
                                break;
                            case NoodleFirmness.Futsuu:
                                guideText.text = "<b>STEP 2: Cooking along...</b> Almost ready for <color=#FFEE66><b>Yawa (Soft / Yellow)</b></color>";
                                break;
                            case NoodleFirmness.Yawa:
                                guideText.text = "<color=#FFFF44><b>PERFECT YAWA (SOFT)!</b></color> Drag <color=#FFFF44><b>Noodle Basket</b></color> to Bowl now!";
                                break;
                            default:
                                guideText.text = "<color=#FF4444><b>Overcooked!</b></color> Drag Basket to Bowl to finish.";
                                break;
                        }
                    }
                    else // Futsuu
                    {
                        switch (current)
                        {
                            case NoodleFirmness.Raw:
                                guideText.text = "<b>STEP 2: Boiling...</b> Watch timer bar, wait for <color=#44FF44><b>Futsuu (Green)</b></color>";
                                break;
                            case NoodleFirmness.Kata:
                                guideText.text = "<b>STEP 2: Almost ready!</b> Just a few seconds more for <color=#44FF44><b>Futsuu (Green)</b></color>";
                                break;
                            case NoodleFirmness.Futsuu:
                                guideText.text = "<color=#FFFF44><b>PERFECT FUTSUU!</b></color> Drag <color=#FFFF44><b>Noodle Basket</b></color> to Bowl now!";
                                break;
                            case NoodleFirmness.Yawa:
                                guideText.text = "<color=#FFAA33><b>Noodles Softening (Yawa)!</b></color> Drag Basket to Bowl now!";
                                break;
                            default:
                                guideText.text = "<color=#FF4444><b>Overcooked!</b></color> Drag Basket to Bowl to finish.";
                                break;
                        }
                    }
                }
                return;
            }

            // Step 3: Toppings
            if (customerAgent != null && customerAgent.CurrentRecipe != null)
            {
                var reqToppings = customerAgent.CurrentRecipe.requiredToppings;
                var placedCopy = new List<IngredientDataSO>(bowl.PlacedToppings);
                var missingToppings = new List<string>();

                foreach (var req in reqToppings)
                {
                    if (req == null) continue;
                    var match = placedCopy.Find(p => p != null && p.id == req.id);
                    if (match != null)
                    {
                        placedCopy.Remove(match);
                    }
                    else
                    {
                        missingToppings.Add(req.displayName);
                    }
                }

                if (missingToppings.Count > 0)
                {
                    guideText.text = $"<b>STEP 3: Add Toppings</b> — Still needed: <color=#FFFF44><b>{string.Join(", ", missingToppings)}</b></color>";
                    return;
                }
            }
            else if (bowl.PlacedToppings.Count == 0)
            {
                guideText.text = "<b>STEP 3: Add Toppings</b> — Drag toppings into the Bowl";
                return;
            }

            // Step 4: Serve
            guideText.text = $"<b>STEP 4: Serve Ramen</b> — All toppings set! Click the <color=#FFDD44><b>Serve Bell</b></color> below the Bowl!";
        }
    }
}
