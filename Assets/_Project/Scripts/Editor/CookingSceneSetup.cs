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
            var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            if (scene.path != ScenePath)
            {
                scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            }

            // 1. Load Generated Sprites
            var sprBowl = AssetDatabase.LoadAssetAtPath<Sprite>($"{ArtPath}/spr_bowl_base.png");
            var sprBroth = AssetDatabase.LoadAssetAtPath<Sprite>($"{ArtPath}/spr_broth_layer.png");
            var sprNoodles = AssetDatabase.LoadAssetAtPath<Sprite>($"{ArtPath}/spr_noodles_layer.png");
            var sprChashu = AssetDatabase.LoadAssetAtPath<Sprite>($"{ArtPath}/spr_chashu.png");
            var sprTamago = AssetDatabase.LoadAssetAtPath<Sprite>($"{ArtPath}/spr_tamago.png");
            var sprScallions = AssetDatabase.LoadAssetAtPath<Sprite>($"{ArtPath}/spr_scallions.png");
            var sprBasket = AssetDatabase.LoadAssetAtPath<Sprite>($"{ArtPath}/spr_basket.png");
            var sprPot = AssetDatabase.LoadAssetAtPath<Sprite>($"{ArtPath}/spr_pot_shoyu.png");
            var sprCustomer = AssetDatabase.LoadAssetAtPath<Sprite>($"{ArtPath}/spr_customer_night_coder.png");
            var sprTicket = AssetDatabase.LoadAssetAtPath<Sprite>($"{ArtPath}/spr_order_ticket.png");
            var sprCoin = AssetDatabase.LoadAssetAtPath<Sprite>($"{ArtPath}/spr_coin.png");
            var sprBubble = AssetDatabase.LoadAssetAtPath<Sprite>($"{ArtPath}/spr_speech_bubble.png");

            // 1b. Ensure EconomyManager, SettingsManager, and AudioManager exist
            var ecoObj = GameObject.Find("EconomyManager");
            if (ecoObj == null)
            {
                ecoObj = new GameObject("EconomyManager");
                ecoObj.AddComponent<EconomyManager>();
            }

            var settingsPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Project/Prefabs/SettingsManager.prefab");
            var settingsObj = GameObject.Find("SettingsManager");
            if (settingsObj == null && settingsPrefab != null)
            {
                var inst = (GameObject)PrefabUtility.InstantiatePrefab(settingsPrefab);
                inst.name = "SettingsManager";
            }

            var audioObj = GameObject.Find("AudioManager");
            if (audioObj == null)
            {
                audioObj = new GameObject("AudioManager");
                audioObj.AddComponent<AudioManager>();
            }

            // 1c. Setup Customer Area (The Night Coder)
            var customerAgent = SetupCustomerArea(sprCustomer, sprBubble);

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

                var bell = bellObj.GetComponent<ServeBell>();
                if (bell != null)
                {
                    var bowl = bowlObj != null ? bowlObj.GetComponent<BowlInstance>() : null;
                    bell.SetActiveBowl(bowl);
                    bell.SetActiveCustomer(customerAgent);
                }
            }

            // 7. Setup Cooking Guide Canvas, Order Ticket, Wallet HUD, and In-Game Menu
            var canvasObj = SetupGuideCanvas(customerAgent);
            SetupOrderTicketUI(canvasObj, sprTicket);
            SetupWalletHUD(canvasObj, sprCoin);
            SetupInGameMenu(canvasObj);

            // 8. Ensure EventSystem exists and uses modern Input System
            EnsureEventSystem();

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveOpenScenes();
            AssetDatabase.SaveAssets();
            Debug.Log("<color=green>[VibeCooking] Step 1.7: End-to-End MVP Loop & In-Game Menu configured successfully!</color>");
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

        private static GameObject SetupGuideCanvas(CustomerAgent customerAgent)
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
            if (customerAgent != null)
                so.FindProperty("customerAgent").objectReferenceValue = customerAgent;
            so.ApplyModifiedProperties();

            return canvasObj;
        }

        private static CustomerAgent SetupCustomerArea(Sprite sprCustomer, Sprite sprBubble)
        {
            var customerArea = GameObject.Find("CustomerArea");
            if (customerArea == null)
            {
                customerArea = new GameObject("CustomerArea");
            }
            customerArea.transform.position = new Vector3(0f, 1.4f, 0f);

            // Customer Portrait
            var portraitObj = customerArea.transform.Find("CustomerPortrait")?.gameObject;
            if (portraitObj == null)
            {
                portraitObj = new GameObject("CustomerPortrait");
                portraitObj.transform.SetParent(customerArea.transform, false);
            }
            portraitObj.transform.localPosition = Vector3.zero;
            portraitObj.transform.localScale = new Vector3(1.1f, 1.1f, 1f);
            var portraitSR = portraitObj.GetComponent<SpriteRenderer>();
            if (portraitSR == null) portraitSR = portraitObj.AddComponent<SpriteRenderer>();
            portraitSR.sprite = sprCustomer;
            portraitSR.color = Color.white;
            portraitSR.sortingOrder = 2;

            // Speech Bubble
            var speechObj = customerArea.transform.Find("SpeechBubble")?.gameObject;
            if (speechObj == null)
            {
                speechObj = new GameObject("SpeechBubble");
                speechObj.transform.SetParent(customerArea.transform, false);
            }
            speechObj.transform.localPosition = new Vector3(0f, 1.65f, 0f);

            // Bubble background plate
            var bubbleBg = speechObj.transform.Find("BubbleBg")?.gameObject;
            if (bubbleBg == null)
            {
                bubbleBg = new GameObject("BubbleBg");
                bubbleBg.transform.SetParent(speechObj.transform, false);
            }
            var bgSR = bubbleBg.GetComponent<SpriteRenderer>();
            if (bgSR == null) bgSR = bubbleBg.AddComponent<SpriteRenderer>();
            bgSR.sprite = sprBubble;
            bgSR.color = Color.white;
            bgSR.sortingOrder = 13;

            // Bubble text
            var textObj = speechObj.transform.Find("SpeechText")?.gameObject;
            if (textObj == null)
            {
                textObj = new GameObject("SpeechText");
                textObj.transform.SetParent(speechObj.transform, false);
            }
            var speechTMP = textObj.GetComponent<TextMeshPro>();
            if (speechTMP == null) speechTMP = textObj.AddComponent<TextMeshPro>();
            speechTMP.fontSize = 2.0f;
            speechTMP.alignment = TextAlignmentOptions.Center;
            speechTMP.color = Color.white;
            speechTMP.sortingOrder = 15;
            speechTMP.rectTransform.sizeDelta = new Vector2(4.6f, 1.2f);
            speechTMP.rectTransform.localPosition = new Vector3(0f, 0.14f, 0f);
            speechTMP.text = "Been debugging for 6 hours straight...\nCould I get a warm <b>Classic Shoyu Ramen</b>?";

            var ca = customerArea.GetComponent<CustomerAgent>();
            if (ca == null) ca = customerArea.AddComponent<CustomerAgent>();

            var pool = new System.Collections.Generic.List<CustomerDataSO>();
            var guids = AssetDatabase.FindAssets("t:CustomerDataSO", new[] { "Assets/_Project/ScriptableObjects/Customers" });
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var c = AssetDatabase.LoadAssetAtPath<CustomerDataSO>(path);
                if (c != null && !pool.Contains(c)) pool.Add(c);
            }

            var customerData = AssetDatabase.LoadAssetAtPath<CustomerDataSO>("Assets/_Project/ScriptableObjects/Customers/NightCoderCustomer.asset");
            if (customerData == null && pool.Count > 0) customerData = pool[0];
            var recipeData = AssetDatabase.LoadAssetAtPath<RecipeDataSO>("Assets/_Project/ScriptableObjects/Recipes/ClassicShoyuRamen.asset");
            ca.SetReferences(portraitSR, speechObj, speechTMP, customerData, recipeData, pool);

            return ca;
        }

        private static void SetupOrderTicketUI(GameObject canvasObj, Sprite sprTicket)
        {
            var ticketObj = canvasObj.transform.Find("OrderTicketSlip")?.gameObject;
            if (ticketObj == null)
            {
                ticketObj = new GameObject("OrderTicketSlip");
                ticketObj.transform.SetParent(canvasObj.transform, false);
            }

            var img = ticketObj.GetComponent<Image>();
            if (img == null) img = ticketObj.AddComponent<Image>();
            if (sprTicket != null) img.sprite = sprTicket;

            var rt = ticketObj.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0f, 1f);
            rt.anchorMax = new Vector2(0f, 1f);
            rt.pivot = new Vector2(0f, 1f);
            rt.anchoredPosition = new Vector2(35f, -20f);
            rt.sizeDelta = new Vector2(235f, 305f);

            // OrderNumber
            var numObj = ticketObj.transform.Find("OrderNumber")?.gameObject;
            if (numObj == null)
            {
                numObj = new GameObject("OrderNumber");
                numObj.transform.SetParent(ticketObj.transform, false);
            }
            var numTMP = numObj.GetComponent<TextMeshProUGUI>();
            if (numTMP == null) numTMP = numObj.AddComponent<TextMeshProUGUI>();
            numTMP.fontSize = 12.5f;
            numTMP.fontStyle = FontStyles.Bold;
            numTMP.color = new Color(0.36f, 0.25f, 0.22f, 1f);
            numTMP.alignment = TextAlignmentOptions.Center;
            var numRt = numTMP.rectTransform;
            numRt.anchorMin = new Vector2(0.5f, 1f);
            numRt.anchorMax = new Vector2(0.5f, 1f);
            numRt.pivot = new Vector2(0.5f, 1f);
            numRt.anchoredPosition = new Vector2(0f, -54f);
            numRt.sizeDelta = new Vector2(200f, 20f);

            // RecipeTitle
            var titleObj = ticketObj.transform.Find("RecipeTitle")?.gameObject;
            if (titleObj == null)
            {
                titleObj = new GameObject("RecipeTitle");
                titleObj.transform.SetParent(ticketObj.transform, false);
            }
            var titleTMP = titleObj.GetComponent<TextMeshProUGUI>();
            if (titleTMP == null) titleTMP = titleObj.AddComponent<TextMeshProUGUI>();
            titleTMP.fontSize = 14.5f;
            titleTMP.fontStyle = FontStyles.Bold;
            titleTMP.color = new Color(0.13f, 0.13f, 0.13f, 1f);
            titleTMP.alignment = TextAlignmentOptions.Center;
            var titleRt = titleTMP.rectTransform;
            titleRt.anchorMin = new Vector2(0.5f, 1f);
            titleRt.anchorMax = new Vector2(0.5f, 1f);
            titleRt.pivot = new Vector2(0.5f, 1f);
            titleRt.anchoredPosition = new Vector2(0f, -76f);
            titleRt.sizeDelta = new Vector2(200f, 24f);

            // Details
            var detailsObj = ticketObj.transform.Find("Details")?.gameObject;
            if (detailsObj == null)
            {
                detailsObj = new GameObject("Details");
                detailsObj.transform.SetParent(ticketObj.transform, false);
            }
            var detailsTMP = detailsObj.GetComponent<TextMeshProUGUI>();
            if (detailsTMP == null) detailsTMP = detailsObj.AddComponent<TextMeshProUGUI>();
            detailsTMP.fontSize = 12f;
            detailsTMP.color = new Color(0.22f, 0.28f, 0.31f, 1f);
            detailsTMP.alignment = TextAlignmentOptions.TopLeft;
            var detailsRt = detailsTMP.rectTransform;
            detailsRt.anchorMin = new Vector2(0.5f, 1f);
            detailsRt.anchorMax = new Vector2(0.5f, 1f);
            detailsRt.pivot = new Vector2(0.5f, 1f);
            detailsRt.anchoredPosition = new Vector2(0f, -106f);
            detailsRt.sizeDelta = new Vector2(185f, 135f);

            // Reward
            var rewardObj = ticketObj.transform.Find("Reward")?.gameObject;
            if (rewardObj == null)
            {
                rewardObj = new GameObject("Reward");
                rewardObj.transform.SetParent(ticketObj.transform, false);
            }
            var rewardTMP = rewardObj.GetComponent<TextMeshProUGUI>();
            if (rewardTMP == null) rewardTMP = rewardObj.AddComponent<TextMeshProUGUI>();
            rewardTMP.fontSize = 13f;
            rewardTMP.fontStyle = FontStyles.Bold;
            rewardTMP.color = new Color(0.18f, 0.49f, 0.20f, 1f);
            rewardTMP.alignment = TextAlignmentOptions.Center;
            var rewardRt = rewardTMP.rectTransform;
            rewardRt.anchorMin = new Vector2(0.5f, 1f);
            rewardRt.anchorMax = new Vector2(0.5f, 1f);
            rewardRt.pivot = new Vector2(0.5f, 1f);
            rewardRt.anchoredPosition = new Vector2(0f, -252f);
            rewardRt.sizeDelta = new Vector2(200f, 22f);

            var ticketUI = ticketObj.GetComponent<OrderTicketUI>();
            if (ticketUI == null) ticketUI = ticketObj.AddComponent<OrderTicketUI>();

            var so = new SerializedObject(ticketUI);
            so.FindProperty("orderNumberText").objectReferenceValue = numTMP;
            so.FindProperty("recipeTitleText").objectReferenceValue = titleTMP;
            so.FindProperty("detailsText").objectReferenceValue = detailsTMP;
            so.FindProperty("rewardText").objectReferenceValue = rewardTMP;
            so.FindProperty("ticketPanel").objectReferenceValue = ticketObj;
            so.ApplyModifiedProperties();
        }

        private static void SetupWalletHUD(GameObject canvasObj, Sprite sprCoin)
        {
            var walletObj = canvasObj.transform.Find("WalletPanel")?.gameObject;
            if (walletObj == null)
            {
                walletObj = new GameObject("WalletPanel");
                walletObj.transform.SetParent(canvasObj.transform, false);
            }

            var img = walletObj.GetComponent<Image>();
            if (img == null) img = walletObj.AddComponent<Image>();
            img.color = new Color(0.08f, 0.10f, 0.16f, 0.90f);

            var rt = walletObj.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(1f, 1f);
            rt.anchorMax = new Vector2(1f, 1f);
            rt.pivot = new Vector2(1f, 1f);
            rt.anchoredPosition = new Vector2(-125f, -20f);
            rt.sizeDelta = new Vector2(210f, 68f);

            // Coin Icon
            var iconObj = walletObj.transform.Find("CoinIcon")?.gameObject;
            if (iconObj == null)
            {
                iconObj = new GameObject("CoinIcon");
                iconObj.transform.SetParent(walletObj.transform, false);
            }
            var coinImg = iconObj.GetComponent<Image>();
            if (coinImg == null) coinImg = iconObj.AddComponent<Image>();
            if (sprCoin != null) coinImg.sprite = sprCoin;
            coinImg.rectTransform.anchoredPosition = new Vector2(-65f, 10f);
            coinImg.rectTransform.sizeDelta = new Vector2(32f, 32f);

            // Balance Text
            var balanceObj = walletObj.transform.Find("BalanceText")?.gameObject;
            if (balanceObj == null)
            {
                balanceObj = new GameObject("BalanceText");
                balanceObj.transform.SetParent(walletObj.transform, false);
            }
            var balanceTMP = balanceObj.GetComponent<TextMeshProUGUI>();
            if (balanceTMP == null) balanceTMP = balanceObj.AddComponent<TextMeshProUGUI>();
            balanceTMP.text = "<b>¥ 1,000</b>";
            balanceTMP.fontSize = 20f;
            balanceTMP.color = new Color(1.0f, 0.84f, 0.20f, 1f);
            balanceTMP.alignment = TextAlignmentOptions.Center;
            balanceTMP.rectTransform.anchoredPosition = new Vector2(15f, 10f);
            balanceTMP.rectTransform.sizeDelta = new Vector2(120f, 32f);

            // Bowls Served Counter Text
            var ordersObj = walletObj.transform.Find("OrdersServedText")?.gameObject;
            if (ordersObj == null)
            {
                ordersObj = new GameObject("OrdersServedText");
                ordersObj.transform.SetParent(walletObj.transform, false);
            }
            var ordersTMP = ordersObj.GetComponent<TextMeshProUGUI>();
            if (ordersTMP == null) ordersTMP = ordersObj.AddComponent<TextMeshProUGUI>();
            ordersTMP.text = "Bowls Served: 0";
            ordersTMP.fontSize = 13.5f;
            ordersTMP.fontStyle = FontStyles.Normal;
            ordersTMP.color = new Color(0.92f, 0.72f, 0.45f, 0.95f);
            ordersTMP.alignment = TextAlignmentOptions.Center;
            ordersTMP.rectTransform.anchoredPosition = new Vector2(0f, -18f);
            ordersTMP.rectTransform.sizeDelta = new Vector2(195f, 22f);

            // Floating Reward Popup
            var popupObj = walletObj.transform.Find("FloatingPopup")?.gameObject;
            if (popupObj == null)
            {
                popupObj = new GameObject("FloatingPopup");
                popupObj.transform.SetParent(walletObj.transform, false);
            }
            var popupTMP = popupObj.GetComponent<TextMeshProUGUI>();
            if (popupTMP == null) popupTMP = popupObj.AddComponent<TextMeshProUGUI>();
            popupTMP.text = "+¥1,050";
            popupTMP.fontSize = 22f;
            popupTMP.fontStyle = FontStyles.Bold;
            popupTMP.color = new Color(1.0f, 0.94f, 0.55f, 1f);
            popupTMP.alignment = TextAlignmentOptions.Center;
            popupTMP.rectTransform.anchoredPosition = new Vector2(0f, -50f);
            popupTMP.rectTransform.sizeDelta = new Vector2(160f, 35f);
            popupObj.SetActive(false);

            var walletHUD = walletObj.GetComponent<WalletHUD>();
            if (walletHUD == null) walletHUD = walletObj.AddComponent<WalletHUD>();

            var so = new SerializedObject(walletHUD);
            so.FindProperty("balanceText").objectReferenceValue = balanceTMP;
            so.FindProperty("floatingPopupText").objectReferenceValue = popupTMP;
            so.FindProperty("ordersServedText").objectReferenceValue = ordersTMP;
            so.ApplyModifiedProperties();
        }

        private static void SetupInGameMenu(GameObject canvasObj)
        {
            // 1. Menu Toggle Button on HUD (top-right corner)
            var menuBtnObj = canvasObj.transform.Find("MenuToggleButton")?.gameObject;
            if (menuBtnObj == null)
            {
                menuBtnObj = MainMenuBuilder.CreateUIRect("MenuToggleButton", canvasObj.transform);
            }
            var menuBtnRT = menuBtnObj.GetComponent<RectTransform>();
            menuBtnRT.anchorMin = new Vector2(1f, 1f);
            menuBtnRT.anchorMax = new Vector2(1f, 1f);
            menuBtnRT.pivot = new Vector2(1f, 1f);
            menuBtnRT.anchoredPosition = new Vector2(-35f, -20f);
            menuBtnRT.sizeDelta = new Vector2(75f, 68f);

            var menuBtnImg = menuBtnObj.GetComponent<Image>();
            if (menuBtnImg == null) menuBtnImg = menuBtnObj.AddComponent<Image>();
            menuBtnImg.color = new Color(0.12f, 0.14f, 0.20f, 0.95f);

            var menuBtn = menuBtnObj.GetComponent<Button>();
            if (menuBtn == null) menuBtn = menuBtnObj.AddComponent<Button>();

            var menuColors = menuBtn.colors;
            menuColors.highlightedColor = new Color(0.20f, 0.24f, 0.35f, 1f);
            menuColors.pressedColor = new Color(0.08f, 0.10f, 0.15f, 1f);
            menuBtn.colors = menuColors;

            var menuLabelObj = menuBtnObj.transform.Find("Label")?.gameObject;
            if (menuLabelObj == null)
            {
                menuLabelObj = MainMenuBuilder.CreateUIRect("Label", menuBtnObj.transform);
                MainMenuBuilder.SetStretch(menuLabelObj);
            }
            var menuTMP = menuLabelObj.GetComponent<TextMeshProUGUI>();
            if (menuTMP == null) menuTMP = menuLabelObj.AddComponent<TextMeshProUGUI>();
            menuTMP.text = "<b>MENU\n<size=65%><color=#FFAA44>||</color></size></b>";
            menuTMP.fontSize = 17f;
            menuTMP.fontStyle = FontStyles.Bold;
            menuTMP.color = new Color(1f, 0.78f, 0.42f);
            menuTMP.alignment = TextAlignmentOptions.Center;

            // 2. Pause Modal Root (Fullscreen Overlay)
            var modalRoot = canvasObj.transform.Find("PauseModalPanel")?.gameObject;
            if (modalRoot == null)
            {
                modalRoot = MainMenuBuilder.CreateUIRect("PauseModalPanel", canvasObj.transform);
                MainMenuBuilder.SetStretch(modalRoot);
            }

            var modalBg = modalRoot.GetComponent<Image>();
            if (modalBg == null) modalBg = modalRoot.AddComponent<Image>();
            modalBg.color = new Color(0.04f, 0.05f, 0.07f, 0.88f);

            // Dialog Window
            var dialogObj = modalRoot.transform.Find("DialogWindow")?.gameObject;
            if (dialogObj == null)
            {
                dialogObj = MainMenuBuilder.CreateUIRect("DialogWindow", modalRoot.transform);
            }
            var dialogRect = dialogObj.GetComponent<RectTransform>();
            dialogRect.anchorMin = new Vector2(0.5f, 0.5f);
            dialogRect.anchorMax = new Vector2(0.5f, 0.5f);
            dialogRect.pivot = new Vector2(0.5f, 0.5f);
            dialogRect.sizeDelta = new Vector2(440f, 380f);

            var dialogBg = dialogObj.GetComponent<Image>();
            if (dialogBg == null) dialogBg = dialogObj.AddComponent<Image>();
            dialogBg.color = new Color(0.14f, 0.16f, 0.21f, 1f);

            // Header
            var headerObj = dialogObj.transform.Find("Header")?.gameObject;
            if (headerObj == null)
            {
                headerObj = MainMenuBuilder.CreateUIRect("Header", dialogObj.transform);
            }
            var headerRect = headerObj.GetComponent<RectTransform>();
            headerRect.anchorMin = new Vector2(0f, 1f);
            headerRect.anchorMax = new Vector2(1f, 1f);
            headerRect.pivot = new Vector2(0.5f, 1f);
            headerRect.anchoredPosition = new Vector2(0f, -20f);
            headerRect.sizeDelta = new Vector2(0f, 60f);

            var headerTMP = headerObj.GetComponent<TextMeshProUGUI>();
            if (headerTMP == null) headerTMP = headerObj.AddComponent<TextMeshProUGUI>();
            headerTMP.text = "<b>PAUSED</b>\n<size=50%><color=#C8C4B8>~ Lo-Fi Ramen Shop ~</color></size>";
            headerTMP.fontSize = 32f;
            headerTMP.fontStyle = FontStyles.Bold;
            headerTMP.color = new Color(1f, 0.78f, 0.42f);
            headerTMP.alignment = TextAlignmentOptions.Center;

            // Button Group
            var btnGroupObj = dialogObj.transform.Find("ButtonGroup")?.gameObject;
            if (btnGroupObj == null)
            {
                btnGroupObj = MainMenuBuilder.CreateUIRect("ButtonGroup", dialogObj.transform);
            }
            var btnGroupRect = btnGroupObj.GetComponent<RectTransform>();
            btnGroupRect.anchorMin = new Vector2(0.5f, 0.4f);
            btnGroupRect.anchorMax = new Vector2(0.5f, 0.4f);
            btnGroupRect.pivot = new Vector2(0.5f, 0.5f);
            btnGroupRect.sizeDelta = new Vector2(320f, 220f);

            var vlg = btnGroupObj.GetComponent<VerticalLayoutGroup>();
            if (vlg == null) vlg = btnGroupObj.AddComponent<VerticalLayoutGroup>();
            vlg.spacing = 16f;
            vlg.childControlWidth = true;
            vlg.childControlHeight = true;
            vlg.childForceExpandWidth = true;
            vlg.childForceExpandHeight = false;

            // Resume, Settings, Main Menu Buttons
            var resumeBtnObj = btnGroupObj.transform.Find("ResumeButton")?.gameObject;
            Button resumeBtn = null;
            if (resumeBtnObj == null)
            {
                resumeBtn = MainMenuBuilder.CreateMenuButton("ResumeButton", "RESUME", new Color(0.85f, 0.55f, 0.20f), btnGroupObj.transform);
            }
            else
            {
                resumeBtn = resumeBtnObj.GetComponent<Button>();
            }

            var settingsBtnObj = btnGroupObj.transform.Find("SettingsButton")?.gameObject;
            Button settingsBtn = null;
            if (settingsBtnObj == null)
            {
                settingsBtn = MainMenuBuilder.CreateMenuButton("SettingsButton", "SETTINGS", new Color(0.25f, 0.30f, 0.40f), btnGroupObj.transform);
            }
            else
            {
                settingsBtn = settingsBtnObj.GetComponent<Button>();
            }

            var mainMenuBtnObj = btnGroupObj.transform.Find("MainMenuButton")?.gameObject;
            Button mainMenuBtn = null;
            if (mainMenuBtnObj == null)
            {
                mainMenuBtn = MainMenuBuilder.CreateMenuButton("MainMenuButton", "MAIN MENU", new Color(0.25f, 0.25f, 0.30f), btnGroupObj.transform);
            }
            else
            {
                mainMenuBtn = mainMenuBtnObj.GetComponent<Button>();
            }

            // 3. Settings Modal Panel (reuses MainMenuBuilder.BuildSettingsModal)
            var settingsPanelObj = canvasObj.transform.Find("SettingsModalPanel")?.gameObject;
            SettingsPanelUI settingsPanel = null;
            if (settingsPanelObj == null)
            {
                settingsPanel = MainMenuBuilder.BuildSettingsModal(canvasObj.transform);
            }
            else
            {
                settingsPanel = settingsPanelObj.GetComponent<SettingsPanelUI>();
            }

            // 4. Attach InGameMenuController to canvasObj
            var menuCtrl = canvasObj.GetComponent<InGameMenuController>();
            if (menuCtrl == null) menuCtrl = canvasObj.AddComponent<InGameMenuController>();

            menuCtrl.SetReferences(menuBtn, resumeBtn, settingsBtn, mainMenuBtn, modalRoot, settingsPanel);

            // Ensure modals are initially inactive
            modalRoot.SetActive(false);
            if (settingsPanel != null) settingsPanel.ClosePanel();
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
