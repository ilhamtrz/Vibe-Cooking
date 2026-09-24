#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

namespace VibeCooking.Editor
{
    public static class MainMenuBuilder
    {
        private const string MainMenuScenePath = "Assets/Scenes/MainMenuScene.unity";
        private const string GameScenePath = "Assets/Scenes/GameScene.unity";
        private const string SettingsPrefabPath = "Assets/_Project/Prefabs/SettingsManager.prefab";

        [MenuItem("Vibe Cooking/Build Main Menu Scene & Configure Flow")]
        public static void BuildMainMenuScene()
        {
            // 1. Configure Build Settings
            ConfigureBuildSettings();

            // 2. Open / Create MainMenuScene
            var scene = EditorSceneManager.OpenScene(MainMenuScenePath, OpenSceneMode.Single);

            // 3. Clear existing root objects to avoid duplicates
            var rootObjects = scene.GetRootGameObjects();
            foreach (var obj in rootObjects)
            {
                Object.DestroyImmediate(obj);
            }

            // 4. Setup Main Camera
            var cameraObj = new GameObject("Main Camera");
            var cam = cameraObj.AddComponent<Camera>();
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.08f, 0.09f, 0.12f, 1f); // Dark cozy night
            cam.orthographic = true;
            cam.orthographicSize = 5f;
            cameraObj.AddComponent<AudioListener>();
            cameraObj.tag = "MainCamera";

            // 5. Instantiate SettingsManager Prefab
            var settingsPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(SettingsPrefabPath);
            if (settingsPrefab != null)
            {
                var settingsInst = (GameObject)PrefabUtility.InstantiatePrefab(settingsPrefab);
                settingsInst.name = "SettingsManager";
            }
            else
            {
                Debug.LogWarning("[MainMenuBuilder] SettingsManager prefab not found at " + SettingsPrefabPath);
            }

            // 5b. Ensure AudioManager exists
            var audioObj = GameObject.Find("AudioManager");
            if (audioObj == null)
            {
                audioObj = new GameObject("AudioManager");
                audioObj.AddComponent<AudioManager>();
            }

            // 6. Setup EventSystem
            var eventSystemObj = new GameObject("EventSystem");
            eventSystemObj.AddComponent<EventSystem>();
            // Use modern input module if available, otherwise Standalone
            var inputModuleType = System.Type.GetType("UnityEngine.InputSystem.UI.InputSystemUIInputModule, Unity.InputSystem");
            if (inputModuleType != null)
            {
                eventSystemObj.AddComponent(inputModuleType);
            }
            else
            {
                eventSystemObj.AddComponent<StandaloneInputModule>();
            }

            // 7. Setup Canvas
            var canvasObj = new GameObject("MainMenuCanvas");
            var canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;
            canvasObj.AddComponent<GraphicRaycaster>();

            // Controller script
            var menuController = canvasObj.AddComponent<MainMenuController>();

            // 8. Background Panel
            var bgObj = CreateUIRect("Background", canvasObj.transform);
            SetStretch(bgObj);
            var bgImage = bgObj.AddComponent<Image>();
            bgImage.color = new Color(0.07f, 0.08f, 0.11f, 1f);

            // Decorative subtitle header
            var topStripe = CreateUIRect("TopStripe", bgObj.transform);
            var topStripeRect = topStripe.GetComponent<RectTransform>();
            topStripeRect.anchorMin = new Vector2(0f, 1f);
            topStripeRect.anchorMax = new Vector2(1f, 1f);
            topStripeRect.pivot = new Vector2(0.5f, 1f);
            topStripeRect.sizeDelta = new Vector2(0, 8);
            var topStripeImg = topStripe.AddComponent<Image>();
            topStripeImg.color = new Color(0.95f, 0.65f, 0.25f, 0.8f);

            // 9. Title Group
            var titleGroup = CreateUIRect("TitleGroup", canvasObj.transform);
            var titleRect = titleGroup.GetComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0.5f, 0.68f);
            titleRect.anchorMax = new Vector2(0.5f, 0.68f);
            titleRect.pivot = new Vector2(0.5f, 0.5f);
            titleRect.sizeDelta = new Vector2(800, 180);

            var titleTextObj = CreateUIRect("TitleText", titleGroup.transform);
            var titleTextRect = titleTextObj.GetComponent<RectTransform>();
            titleTextRect.anchorMin = new Vector2(0, 0.35f);
            titleTextRect.anchorMax = new Vector2(1, 1f);
            titleTextRect.offsetMin = Vector2.zero;
            titleTextRect.offsetMax = Vector2.zero;
            var titleText = titleTextObj.AddComponent<TextMeshProUGUI>();
            titleText.text = "VIBE COOKING";
            titleText.fontSize = 72;
            titleText.fontStyle = FontStyles.Bold;
            titleText.color = new Color(1f, 0.78f, 0.42f); // Warm cozy amber
            titleText.alignment = TextAlignmentOptions.Center;

            var subtitleObj = CreateUIRect("SubtitleText", titleGroup.transform);
            var subRect = subtitleObj.GetComponent<RectTransform>();
            subRect.anchorMin = new Vector2(0, 0f);
            subRect.anchorMax = new Vector2(1, 0.35f);
            subRect.offsetMin = Vector2.zero;
            subRect.offsetMax = Vector2.zero;
            var subText = subtitleObj.AddComponent<TextMeshProUGUI>();
            subText.text = "~ Lo-Fi Ramen Shop ~";
            subText.fontSize = 28;
            subText.color = new Color(0.78f, 0.76f, 0.72f);
            subText.alignment = TextAlignmentOptions.Center;

            // 10. Navigation Buttons Group
            var btnGroup = CreateUIRect("ButtonGroup", canvasObj.transform);
            var btnGroupRect = btnGroup.GetComponent<RectTransform>();
            btnGroupRect.anchorMin = new Vector2(0.5f, 0.35f);
            btnGroupRect.anchorMax = new Vector2(0.5f, 0.35f);
            btnGroupRect.pivot = new Vector2(0.5f, 0.5f);
            btnGroupRect.sizeDelta = new Vector2(320, 240);

            var vlg = btnGroup.AddComponent<VerticalLayoutGroup>();
            vlg.spacing = 20;
            vlg.childControlWidth = true;
            vlg.childControlHeight = true;
            vlg.childForceExpandWidth = true;
            vlg.childForceExpandHeight = false;

            var startBtn = CreateMenuButton("StartButton", "START", new Color(0.85f, 0.55f, 0.20f), btnGroup.transform);
            var settingsBtn = CreateMenuButton("SettingsButton", "SETTINGS", new Color(0.25f, 0.30f, 0.40f), btnGroup.transform);
            var exitBtn = CreateMenuButton("ExitButton", "EXIT", new Color(0.25f, 0.25f, 0.30f), btnGroup.transform);

            // 11. Settings Modal Panel Overlay
            var settingsPanelUI = BuildSettingsModal(canvasObj.transform);

            // Wire Serialized Properties on MainMenuController
            var serializedController = new SerializedObject(menuController);
            serializedController.FindProperty("startButton").objectReferenceValue = startBtn;
            serializedController.FindProperty("settingsButton").objectReferenceValue = settingsBtn;
            serializedController.FindProperty("exitButton").objectReferenceValue = exitBtn;
            serializedController.FindProperty("settingsPanel").objectReferenceValue = settingsPanelUI;
            serializedController.ApplyModifiedProperties();

            // 12. Save Scene
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log("<color=green>[VibeCooking] MainMenuScene built and saved successfully!</color>");
        }

        public static void ConfigureBuildSettings()
        {
            var scenes = new List<EditorBuildSettingsScene>
            {
                new EditorBuildSettingsScene(MainMenuScenePath, true),
                new EditorBuildSettingsScene(GameScenePath, true)
            };
            EditorBuildSettings.scenes = scenes.ToArray();
            Debug.Log("<color=green>[VibeCooking] Build Settings updated: MainMenuScene (Index 0), GameScene (Index 1)</color>");
        }

        public static SettingsPanelUI BuildSettingsModal(Transform canvasTransform)
        {
            var modalRoot = CreateUIRect("SettingsModalPanel", canvasTransform);
            SetStretch(modalRoot);

            // Dim background
            var modalBg = modalRoot.AddComponent<Image>();
            modalBg.color = new Color(0.04f, 0.05f, 0.07f, 0.88f);

            // Dialog Window
            var dialogObj = CreateUIRect("DialogWindow", modalRoot.transform);
            var dialogRect = dialogObj.GetComponent<RectTransform>();
            dialogRect.anchorMin = new Vector2(0.5f, 0.5f);
            dialogRect.anchorMax = new Vector2(0.5f, 0.5f);
            dialogRect.pivot = new Vector2(0.5f, 0.5f);
            dialogRect.sizeDelta = new Vector2(740, 580);
            var dialogBg = dialogObj.AddComponent<Image>();
            dialogBg.color = new Color(0.14f, 0.16f, 0.21f, 1f);

            var settingsUI = modalRoot.AddComponent<SettingsPanelUI>();

            // Header
            var headerObj = CreateUIRect("Header", dialogObj.transform);
            var headerRect = headerObj.GetComponent<RectTransform>();
            headerRect.anchorMin = new Vector2(0f, 1f);
            headerRect.anchorMax = new Vector2(1f, 1f);
            headerRect.pivot = new Vector2(0.5f, 1f);
            headerRect.sizeDelta = new Vector2(0, 60);
            var headerText = headerObj.AddComponent<TextMeshProUGUI>();
            headerText.text = "SETTINGS";
            headerText.fontSize = 36;
            headerText.fontStyle = FontStyles.Bold;
            headerText.color = new Color(1f, 0.78f, 0.42f);
            headerText.alignment = TextAlignmentOptions.Center;

            // Content Area Layout Group
            var contentObj = CreateUIRect("ContentArea", dialogObj.transform);
            var contentRect = contentObj.GetComponent<RectTransform>();
            contentRect.anchorMin = new Vector2(0.05f, 0.15f);
            contentRect.anchorMax = new Vector2(0.95f, 0.88f);
            contentRect.offsetMin = Vector2.zero;
            contentRect.offsetMax = Vector2.zero;

            var vlg = contentObj.AddComponent<VerticalLayoutGroup>();
            vlg.spacing = 16;
            vlg.childControlWidth = true;
            vlg.childControlHeight = false;
            vlg.childForceExpandWidth = true;
            vlg.childForceExpandHeight = false;

            // Section 1: DISPLAY
            CreateSectionHeader("DISPLAY", contentObj.transform);
            var resDropdown = CreateDropdownRow("Resolution", contentObj.transform);
            var modeDropdown = CreateDropdownRow("Screen Mode", contentObj.transform);

            // Section 2: AUDIO
            CreateSectionHeader("AUDIO", contentObj.transform);
            var (masterSlider, masterVal) = CreateSliderRow("Master Volume", contentObj.transform, 0.8f);
            var (bgmSlider, bgmVal) = CreateSliderRow("BGM (Music)", contentObj.transform, 0.7f);
            var (sfxSlider, sfxVal) = CreateSliderRow("SFX (ASMR)", contentObj.transform, 0.85f);

            // Close / Save Button
            var closeBtnObj = CreateUIRect("CloseButton", dialogObj.transform);
            var closeRect = closeBtnObj.GetComponent<RectTransform>();
            closeRect.anchorMin = new Vector2(0.5f, 0.03f);
            closeRect.anchorMax = new Vector2(0.5f, 0.03f);
            closeRect.pivot = new Vector2(0.5f, 0f);
            closeRect.sizeDelta = new Vector2(220, 48);

            var closeImg = closeBtnObj.AddComponent<Image>();
            closeImg.color = new Color(0.85f, 0.55f, 0.20f);
            var closeBtn = closeBtnObj.AddComponent<Button>();

            var closeTextObj = CreateUIRect("Text", closeBtnObj.transform);
            SetStretch(closeTextObj);
            var closeText = closeTextObj.AddComponent<TextMeshProUGUI>();
            closeText.text = "BACK / SAVE";
            closeText.fontSize = 22;
            closeText.fontStyle = FontStyles.Bold;
            closeText.color = Color.white;
            closeText.alignment = TextAlignmentOptions.Center;

            // Wire Serialized Properties on SettingsPanelUI
            var serialized = new SerializedObject(settingsUI);
            serialized.FindProperty("panelRoot").objectReferenceValue = modalRoot;
            serialized.FindProperty("resolutionDropdown").objectReferenceValue = resDropdown;
            serialized.FindProperty("screenModeDropdown").objectReferenceValue = modeDropdown;
            serialized.FindProperty("masterSlider").objectReferenceValue = masterSlider;
            serialized.FindProperty("bgmSlider").objectReferenceValue = bgmSlider;
            serialized.FindProperty("sfxSlider").objectReferenceValue = sfxSlider;
            serialized.FindProperty("masterValueText").objectReferenceValue = masterVal;
            serialized.FindProperty("bgmValueText").objectReferenceValue = bgmVal;
            serialized.FindProperty("sfxValueText").objectReferenceValue = sfxVal;
            serialized.FindProperty("closeButton").objectReferenceValue = closeBtn;
            serialized.ApplyModifiedProperties();

            // Deactivate modal by default
            modalRoot.SetActive(false);

            return settingsUI;
        }

        private static void CreateSectionHeader(string title, Transform parent)
        {
            var headerObj = CreateUIRect("SectionHeader_" + title, parent);
            var le = headerObj.AddComponent<LayoutElement>();
            le.minHeight = 28;
            le.preferredHeight = 28;

            var txt = headerObj.AddComponent<TextMeshProUGUI>();
            txt.text = title;
            txt.fontSize = 18;
            txt.fontStyle = FontStyles.Bold;
            txt.color = new Color(0.65f, 0.68f, 0.75f);
            txt.alignment = TextAlignmentOptions.Left;
        }

        private static TMP_Dropdown CreateDropdownRow(string label, Transform parent)
        {
            var rowObj = CreateUIRect("Row_" + label, parent);
            var le = rowObj.AddComponent<LayoutElement>();
            le.minHeight = 36;
            le.preferredHeight = 36;

            var hlg = rowObj.AddComponent<HorizontalLayoutGroup>();
            hlg.spacing = 15;
            hlg.childControlWidth = false;
            hlg.childControlHeight = true;
            hlg.childForceExpandWidth = false;
            hlg.childForceExpandHeight = true;

            // Label
            var lblObj = CreateUIRect("Label", rowObj.transform);
            var lblRect = lblObj.GetComponent<RectTransform>();
            lblRect.sizeDelta = new Vector2(180, 36);
            var lblText = lblObj.AddComponent<TextMeshProUGUI>();
            lblText.text = label;
            lblText.fontSize = 18;
            lblText.color = Color.white;
            lblText.alignment = TextAlignmentOptions.MidlineLeft;

            // Dropdown using TMP_DefaultControls
            var tmpRes = new TMP_DefaultControls.Resources();
            var dropdownObj = TMP_DefaultControls.CreateDropdown(tmpRes);
            dropdownObj.name = "Dropdown";
            dropdownObj.transform.SetParent(rowObj.transform, false);
            var ddRect = dropdownObj.GetComponent<RectTransform>();
            ddRect.sizeDelta = new Vector2(400, 36);

            var dropdown = dropdownObj.GetComponent<TMP_Dropdown>();
            return dropdown;
        }

        private static (Slider, TextMeshProUGUI) CreateSliderRow(string label, Transform parent, float initialVal)
        {
            var rowObj = CreateUIRect("Row_" + label, parent);
            var le = rowObj.AddComponent<LayoutElement>();
            le.minHeight = 34;
            le.preferredHeight = 34;

            var hlg = rowObj.AddComponent<HorizontalLayoutGroup>();
            hlg.spacing = 15;
            hlg.childControlWidth = false;
            hlg.childControlHeight = true;
            hlg.childForceExpandWidth = false;
            hlg.childForceExpandHeight = true;

            // Label
            var lblObj = CreateUIRect("Label", rowObj.transform);
            var lblRect = lblObj.GetComponent<RectTransform>();
            lblRect.sizeDelta = new Vector2(180, 34);
            var lblText = lblObj.AddComponent<TextMeshProUGUI>();
            lblText.text = label;
            lblText.fontSize = 18;
            lblText.color = Color.white;
            lblText.alignment = TextAlignmentOptions.MidlineLeft;

            // Slider using DefaultControls
            var uiRes = new UnityEngine.UI.DefaultControls.Resources();
            var sliderObj = UnityEngine.UI.DefaultControls.CreateSlider(uiRes);
            sliderObj.name = "Slider";
            sliderObj.transform.SetParent(rowObj.transform, false);
            var sRect = sliderObj.GetComponent<RectTransform>();
            sRect.sizeDelta = new Vector2(340, 24);
            var slider = sliderObj.GetComponent<Slider>();
            slider.minValue = 0f;
            slider.maxValue = 1f;
            slider.value = initialVal;

            // Value text
            var valObj = CreateUIRect("ValueText", rowObj.transform);
            var valRect = valObj.GetComponent<RectTransform>();
            valRect.sizeDelta = new Vector2(60, 34);
            var valText = valObj.AddComponent<TextMeshProUGUI>();
            valText.text = $"{Mathf.RoundToInt(initialVal * 100f)}%";
            valText.fontSize = 18;
            valText.color = new Color(1f, 0.78f, 0.42f);
            valText.alignment = TextAlignmentOptions.MidlineRight;

            return (slider, valText);
        }

        public static Button CreateMenuButton(string name, string label, Color bgColor, Transform parent)
        {
            var btnObj = CreateUIRect(name, parent);
            var le = btnObj.AddComponent<LayoutElement>();
            le.minHeight = 56;
            le.preferredHeight = 56;

            var img = btnObj.AddComponent<Image>();
            img.color = bgColor;

            var btn = btnObj.AddComponent<Button>();
            var colors = btn.colors;
            colors.highlightedColor = bgColor * 1.2f;
            colors.pressedColor = bgColor * 0.8f;
            btn.colors = colors;

            var textObj = CreateUIRect("Text", btnObj.transform);
            SetStretch(textObj);
            var txt = textObj.AddComponent<TextMeshProUGUI>();
            txt.text = label;
            txt.fontSize = 24;
            txt.fontStyle = FontStyles.Bold;
            txt.color = Color.white;
            txt.alignment = TextAlignmentOptions.Center;

            return btn;
        }

        public static GameObject CreateUIRect(string name, Transform parent)
        {
            var obj = new GameObject(name, typeof(RectTransform));
            obj.transform.SetParent(parent, false);
            return obj;
        }

        public static void SetStretch(GameObject obj)
        {
            var rect = obj.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }
    }
}
#endif
