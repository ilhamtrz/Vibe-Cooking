#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace VibeCooking.Editor
{
    public static class CookingSceneSetup
    {
        private const string ScenePath = "Assets/Scenes/GameScene.unity";
        private const string ArtPath = "Assets/_Project/Art";

        [MenuItem("Vibe Cooking/Setup Cooking Visuals & UI")]
        public static void SetupSceneVisuals()
        {
            var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);

            // 1. Load Generated Sprites
            var sprBowl = AssetDatabase.LoadAssetAtPath<Sprite>($"{ArtPath}/spr_bowl_base.png");
            var sprBroth = AssetDatabase.LoadAssetAtPath<Sprite>($"{ArtPath}/spr_broth_layer.png");
            var sprNoodles = AssetDatabase.LoadAssetAtPath<Sprite>($"{ArtPath}/spr_noodles_layer.png");
            var sprChashu = AssetDatabase.LoadAssetAtPath<Sprite>($"{ArtPath}/spr_chashu.png");
            var sprTamago = AssetDatabase.LoadAssetAtPath<Sprite>($"{ArtPath}/spr_tamago.png");
            var sprScallions = AssetDatabase.LoadAssetAtPath<Sprite>($"{ArtPath}/spr_scallions.png");
            var sprBasket = AssetDatabase.LoadAssetAtPath<Sprite>($"{ArtPath}/spr_basket.png");
            var sprPot = AssetDatabase.LoadAssetAtPath<Sprite>($"{ArtPath}/spr_pot_shoyu.png");

            // 2. Setup AssemblyBowl
            var bowlObj = GameObject.Find("AssemblyBowl");
            if (bowlObj != null)
            {
                bowlObj.transform.position = new Vector3(0f, -1.75f, 0f);

                var bowlSR = bowlObj.GetComponent<SpriteRenderer>();
                if (bowlSR != null && sprBowl != null)
                {
                    bowlSR.sprite = sprBowl;
                    bowlSR.color = Color.white;
                    bowlSR.sortingOrder = 0;
                }

                var brothObj = bowlObj.transform.Find("BrothLayer")?.gameObject;
                SpriteRenderer brothSR = null;
                if (brothObj != null)
                {
                    brothSR = brothObj.GetComponent<SpriteRenderer>();
                    if (brothSR == null) brothSR = brothObj.AddComponent<SpriteRenderer>();
                    brothSR.sprite = sprBroth;
                    brothSR.color = Color.white;
                    brothSR.sortingOrder = 1;
                }

                var noodleObj = bowlObj.transform.Find("NoodlesLayer")?.gameObject;
                SpriteRenderer noodleSR = null;
                if (noodleObj != null)
                {
                    noodleSR = noodleObj.GetComponent<SpriteRenderer>();
                    if (noodleSR == null) noodleSR = noodleObj.AddComponent<SpriteRenderer>();
                    noodleSR.sprite = sprNoodles;
                    noodleSR.color = Color.white;
                    noodleSR.sortingOrder = 2;
                }

                // Status text display ABOVE the bowl to avoid overlapping with ServeBell
                var statusObj = bowlObj.transform.Find("BowlStatusDisplay")?.gameObject;
                if (statusObj == null)
                {
                    statusObj = new GameObject("BowlStatusDisplay");
                    statusObj.transform.SetParent(bowlObj.transform, false);
                }

                var tmp = statusObj.GetComponent<TextMeshPro>();
                if (tmp == null) tmp = statusObj.AddComponent<TextMeshPro>();
                tmp.fontSize = 2.4f;
                tmp.alignment = TextAlignmentOptions.Center;
                tmp.color = Color.white;
                tmp.sortingOrder = 15;
                var rect = tmp.rectTransform;
                rect.anchorMin = new Vector2(0.5f, 0.5f);
                rect.anchorMax = new Vector2(0.5f, 0.5f);
                rect.pivot = new Vector2(0.5f, 0.5f);
                rect.anchoredPosition = new Vector2(0f, 1.55f);
                rect.sizeDelta = new Vector2(5.5f, 2.5f);

                var bowl = bowlObj.GetComponent<BowlInstance>();
                if (bowl != null)
                {
                    bowl.SetRenderers(brothSR, noodleSR);
                    bowl.SetStatusLabel(tmp);
                }
            }

            // 3. Setup Broth Pot
            var potObj = GameObject.Find("BrothPot_Shoyu");
            if (potObj != null)
            {
                potObj.transform.position = new Vector3(-5.2f, -1.75f, 0f);

                var potSR = potObj.GetComponent<SpriteRenderer>();
                if (potSR != null && sprPot != null)
                {
                    potSR.sprite = sprPot;
                    potSR.color = Color.white;
                    potSR.sortingOrder = 5;
                }
                var label = potObj.transform.Find("Label")?.GetComponent<TextMeshPro>();
                if (label != null)
                {
                    label.text = "<b>Shoyu Broth</b>\n<size=75%><color=#AAAAAA>[Drag to Bowl]</color></size>";
                    label.sortingOrder = 15;
                }
            }

            // 4. Setup Noodle Station
            var noodleStationObj = GameObject.Find("NoodleStation");
            if (noodleStationObj != null)
            {
                noodleStationObj.transform.position = new Vector3(-2.6f, -1.75f, 0f);

                var stationSR = noodleStationObj.GetComponent<SpriteRenderer>();
                if (stationSR != null && sprBasket != null)
                {
                    stationSR.sprite = sprBasket;
                    stationSR.color = Color.white;
                    stationSR.sortingOrder = 5;
                }

                // Child Noodles in basket
                var noodlesInBasketObj = noodleStationObj.transform.Find("NoodlesInBasket")?.gameObject;
                if (noodlesInBasketObj == null)
                {
                    noodlesInBasketObj = new GameObject("NoodlesInBasket");
                    noodlesInBasketObj.transform.SetParent(noodleStationObj.transform, false);
                }
                noodlesInBasketObj.transform.localPosition = Vector3.zero;
                noodlesInBasketObj.transform.localScale = new Vector3(0.6f, 0.6f, 1f);
                var basketNoodleSR = noodlesInBasketObj.GetComponent<SpriteRenderer>();
                if (basketNoodleSR == null) basketNoodleSR = noodlesInBasketObj.AddComponent<SpriteRenderer>();
                basketNoodleSR.sprite = sprNoodles;
                basketNoodleSR.color = new Color(1f, 0.95f, 0.7f, 1f);
                basketNoodleSR.sortingOrder = 6;
                noodlesInBasketObj.SetActive(false);

                // Status label
                var statusLabel = noodleStationObj.transform.Find("StatusLabel")?.GetComponent<TextMeshPro>();
                if (statusLabel != null)
                {
                    statusLabel.text = "<b>Noodle Basket</b>\n<size=75%><color=#AAAAAA>[Click to Boil]</color></size>";
                    statusLabel.sortingOrder = 15;
                }

                // Timer bar
                var timerBar = noodleStationObj.transform.Find("TimerBar")?.gameObject;
                if (timerBar == null)
                {
                    timerBar = new GameObject("TimerBar");
                    timerBar.transform.SetParent(noodleStationObj.transform, false);
                    timerBar.transform.localPosition = new Vector3(0, 0.95f, 0);
                    var barSR = timerBar.AddComponent<SpriteRenderer>();
                    barSR.sprite = sprBowl; // reuse a circle or simple sprite
                    barSR.sortingOrder = 12;
                    timerBar.transform.localScale = Vector3.zero;
                }

                var ns = noodleStationObj.GetComponent<NoodleStation>();
                if (ns != null)
                {
                    ns.SetReferences(statusLabel, stationSR, basketNoodleSR);
                }
            }

            // 5. Setup Topping Trays
            SetupTray("Tray_Chashu", sprChashu, "<b>Chashu</b>\n<size=75%><color=#AAAAAA>[Drag to Bowl]</color></size>", new Vector3(2.2f, -1.75f, 0f));
            SetupTray("Tray_Tamago", sprTamago, "<b>Ajitama Egg</b>\n<size=75%><color=#AAAAAA>[Drag to Bowl]</color></size>", new Vector3(3.9f, -1.75f, 0f));
            SetupTray("Tray_Scallions", sprScallions, "<b>Scallions</b>\n<size=75%><color=#AAAAAA>[Drag to Bowl]</color></size>", new Vector3(5.6f, -1.75f, 0f));

            // 6. Setup Serve Bell
            var bellObj = GameObject.Find("ServeBell");
            if (bellObj != null)
            {
                bellObj.transform.position = new Vector3(0f, -3.6f, 0f);
                var label = bellObj.transform.Find("Label")?.GetComponent<TextMeshPro>();
                if (label != null)
                {
                    label.text = "<b>SERVE BELL</b>\n<size=75%><color=#AAAAAA>[Click to Serve]</color></size>";
                    label.sortingOrder = 15;
                }
            }

            // 7. Setup Cooking Guide Canvas
            SetupGuideCanvas();

            // 8. Ensure EventSystem exists and uses modern Input System
            EnsureEventSystem();

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveOpenScenes();
            AssetDatabase.SaveAssets();
            Debug.Log("<color=green>[VibeCooking] Cooking Visuals, Labels, and Guide Banner configured successfully!</color>");
        }

        private static void SetupTray(string trayName, Sprite sprite, string labelText, Vector3 pos)
        {
            var trayObj = GameObject.Find(trayName);
            if (trayObj == null) return;

            trayObj.transform.position = pos;

            var visualObj = trayObj.transform.Find("SampleVisual")?.gameObject;
            if (visualObj != null && sprite != null)
            {
                var sr = visualObj.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    sr.sprite = sprite;
                    sr.color = Color.white;
                    sr.sortingOrder = 6;
                }
            }

            var label = trayObj.transform.Find("Label")?.GetComponent<TextMeshPro>();
            if (label != null)
            {
                label.text = labelText;
                label.sortingOrder = 15;
            }
        }

        private static void SetupGuideCanvas()
        {
            var canvasObj = GameObject.Find("CookingGuideCanvas");
            if (canvasObj == null)
            {
                canvasObj = new GameObject("CookingGuideCanvas");
                var canvas = canvasObj.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.sortingOrder = 100;

                var scaler = canvasObj.AddComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1920, 1080);
                scaler.matchWidthOrHeight = 0.5f;

                canvasObj.AddComponent<GraphicRaycaster>();
            }

            // Ribbon panel
            var ribbon = canvasObj.transform.Find("GuideRibbon")?.gameObject;
            if (ribbon == null)
            {
                ribbon = new GameObject("GuideRibbon");
                ribbon.transform.SetParent(canvasObj.transform, false);
                var img = ribbon.AddComponent<Image>();
                img.color = new Color(0.08f, 0.10f, 0.16f, 0.90f);

                var rt = ribbon.GetComponent<RectTransform>();
                rt.anchorMin = new Vector2(0.5f, 1f);
                rt.anchorMax = new Vector2(0.5f, 1f);
                rt.pivot = new Vector2(0.5f, 1f);
                rt.anchoredPosition = new Vector2(0, -18f);
                rt.sizeDelta = new Vector2(980f, 75f);
            }

            // Title
            var titleObj = ribbon.transform.Find("Title")?.gameObject;
            TextMeshProUGUI titleTMP = null;
            if (titleObj == null)
            {
                titleObj = new GameObject("Title");
                titleObj.transform.SetParent(ribbon.transform, false);
                titleTMP = titleObj.AddComponent<TextMeshProUGUI>();
            }
            else
            {
                titleTMP = titleObj.GetComponent<TextMeshProUGUI>();
            }
            titleTMP.text = "~ VIBE KITCHEN COOKING GUIDE ~";
            titleTMP.fontSize = 14;
            titleTMP.fontStyle = FontStyles.Bold;
            titleTMP.color = new Color(0.92f, 0.72f, 0.45f, 1f);
            titleTMP.alignment = TextAlignmentOptions.Center;

            var titleRt = titleTMP.rectTransform;
            titleRt.anchorMin = new Vector2(0f, 0.65f);
            titleRt.anchorMax = new Vector2(1f, 1f);
            titleRt.offsetMin = Vector2.zero;
            titleRt.offsetMax = Vector2.zero;

            // Guide Text Prompt
            var promptObj = ribbon.transform.Find("GuidePrompt")?.gameObject;
            TextMeshProUGUI promptTMP = null;
            if (promptObj == null)
            {
                promptObj = new GameObject("GuidePrompt");
                promptObj.transform.SetParent(ribbon.transform, false);
                promptTMP = promptObj.AddComponent<TextMeshProUGUI>();
            }
            else
            {
                promptTMP = promptObj.GetComponent<TextMeshProUGUI>();
            }
            promptTMP.text = "<b>STEP 1: Pour Broth</b> — Drag the <color=#FFAA44><b>Shoyu Pot</b></color> into the Ramen Bowl";
            promptTMP.fontSize = 20;
            promptTMP.color = Color.white;
            promptTMP.alignment = TextAlignmentOptions.Center;

            var promptRt = promptTMP.rectTransform;
            promptRt.anchorMin = new Vector2(0f, 0f);
            promptRt.anchorMax = new Vector2(1f, 0.65f);
            promptRt.offsetMin = Vector2.zero;
            promptRt.offsetMax = Vector2.zero;

            var guideUI = canvasObj.GetComponent<CookingGuideUI>();
            if (guideUI == null) guideUI = canvasObj.AddComponent<CookingGuideUI>();

            // Wire serialized field via SerializedObject
            var so = new SerializedObject(guideUI);
            so.FindProperty("guideText").objectReferenceValue = promptTMP;
            so.ApplyModifiedProperties();
        }

        private static void EnsureEventSystem()
        {
            var es = Object.FindAnyObjectByType<EventSystem>();
            if (es == null)
            {
                var esObj = new GameObject("EventSystem");
                es = esObj.AddComponent<EventSystem>();
                var inputModuleType = System.Type.GetType("UnityEngine.InputSystem.UI.InputSystemUIInputModule, Unity.InputSystem");
                if (inputModuleType != null)
                {
                    esObj.AddComponent(inputModuleType);
                }
                else
                {
                    esObj.AddComponent<StandaloneInputModule>();
                }
            }
            else
            {
                // Ensure modern input module
                var standalone = es.GetComponent<StandaloneInputModule>();
                var inputModuleType = System.Type.GetType("UnityEngine.InputSystem.UI.InputSystemUIInputModule, Unity.InputSystem");
                if (standalone != null && inputModuleType != null)
                {
                    Object.DestroyImmediate(standalone);
                    es.gameObject.AddComponent(inputModuleType);
                }
            }
        }
    }
}
#endif
