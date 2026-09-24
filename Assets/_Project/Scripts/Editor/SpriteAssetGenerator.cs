#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

namespace VibeCooking.Editor
{
    public static class SpriteAssetGenerator
    {
        private const string ArtPath = "Assets/_Project/Art";

        [MenuItem("Vibe Cooking/Generate Visual Sprites")]
        public static void GenerateSprites()
        {
            if (!Directory.Exists(ArtPath))
                Directory.CreateDirectory(ArtPath);

            // 1. Create Texture files
            CreateBowlSprite();
            CreateBrothSprite();
            CreateNoodlesSprite();
            CreateChashuSprite();
            CreateTamagoSprite();
            CreateScallionsSprite();
            CreateBasketSprite();
            CreatePotSprite();
            CreateCustomerNightCoderSprite();
            CreateCustomerVinylDiggerSprite();
            CreateCustomerRainyStudentSprite();
            CreateOrderTicketSprite();
            CreateCoinSprite();
            CreateSpeechBubbleSprite();

            AssetDatabase.Refresh();

            // 2. Configure Texture Importers as Sprites
            ConfigureAsSprite($"{ArtPath}/spr_bowl_base.png");
            ConfigureAsSprite($"{ArtPath}/spr_broth_layer.png");
            ConfigureAsSprite($"{ArtPath}/spr_noodles_layer.png");
            ConfigureAsSprite($"{ArtPath}/spr_chashu.png");
            ConfigureAsSprite($"{ArtPath}/spr_tamago.png");
            ConfigureAsSprite($"{ArtPath}/spr_scallions.png");
            ConfigureAsSprite($"{ArtPath}/spr_basket.png");
            ConfigureAsSprite($"{ArtPath}/spr_pot_shoyu.png");
            ConfigureAsSprite($"{ArtPath}/spr_customer_night_coder.png");
            ConfigureAsSprite($"{ArtPath}/spr_customer_vinyl_digger.png");
            ConfigureAsSprite($"{ArtPath}/spr_customer_rainy_student.png");
            ConfigureAsSprite($"{ArtPath}/spr_order_ticket.png");
            ConfigureAsSprite($"{ArtPath}/spr_coin.png");
            ConfigureAsSprite($"{ArtPath}/spr_speech_bubble.png");

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            // 3. Link Sprites to ScriptableObjects
            LinkSpritesToData();

            Debug.Log("<color=green>[VibeCooking] Visual 2D Sprites generated and linked successfully!</color>");
        }

        private static void ConfigureAsSprite(string path)
        {
            var importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.spritePixelsPerUnit = 100f;
                importer.filterMode = FilterMode.Bilinear;
                importer.SaveAndReimport();
            }
        }

        private static void LinkSpritesToData()
        {
            var sprBroth = AssetDatabase.LoadAssetAtPath<Sprite>($"{ArtPath}/spr_broth_layer.png");
            var sprNoodles = AssetDatabase.LoadAssetAtPath<Sprite>($"{ArtPath}/spr_noodles_layer.png");
            var sprChashu = AssetDatabase.LoadAssetAtPath<Sprite>($"{ArtPath}/spr_chashu.png");
            var sprTamago = AssetDatabase.LoadAssetAtPath<Sprite>($"{ArtPath}/spr_tamago.png");
            var sprScallions = AssetDatabase.LoadAssetAtPath<Sprite>($"{ArtPath}/spr_scallions.png");

            // ShoyuBroth
            var brothSO = AssetDatabase.LoadAssetAtPath<IngredientDataSO>("Assets/_Project/ScriptableObjects/Ingredients/ShoyuBroth.asset");
            if (brothSO != null)
            {
                brothSO.icon = sprBroth;
                brothSO.bowlVisual = sprBroth;
                brothSO.placeholderColor = Color.white;
                EditorUtility.SetDirty(brothSO);
            }

            // ThinNoodles
            var noodlesSO = AssetDatabase.LoadAssetAtPath<IngredientDataSO>("Assets/_Project/ScriptableObjects/Ingredients/ThinNoodles.asset");
            if (noodlesSO != null)
            {
                noodlesSO.icon = sprNoodles;
                noodlesSO.bowlVisual = sprNoodles;
                noodlesSO.placeholderColor = Color.white;
                EditorUtility.SetDirty(noodlesSO);
            }

            // Chashu
            var chashuSO = AssetDatabase.LoadAssetAtPath<IngredientDataSO>("Assets/_Project/ScriptableObjects/Ingredients/Chashu.asset");
            if (chashuSO != null)
            {
                chashuSO.icon = sprChashu;
                chashuSO.bowlVisual = sprChashu;
                chashuSO.placeholderColor = Color.white;
                EditorUtility.SetDirty(chashuSO);
            }

            // TamagoEgg
            var tamagoSO = AssetDatabase.LoadAssetAtPath<IngredientDataSO>("Assets/_Project/ScriptableObjects/Ingredients/TamagoEgg.asset");
            if (tamagoSO != null)
            {
                tamagoSO.icon = sprTamago;
                tamagoSO.bowlVisual = sprTamago;
                tamagoSO.placeholderColor = Color.white;
                EditorUtility.SetDirty(tamagoSO);
            }

            // Scallions
            var scallionsSO = AssetDatabase.LoadAssetAtPath<IngredientDataSO>("Assets/_Project/ScriptableObjects/Ingredients/Scallions.asset");
            if (scallionsSO != null)
            {
                scallionsSO.icon = sprScallions;
                scallionsSO.bowlVisual = sprScallions;
                scallionsSO.placeholderColor = Color.white;
                EditorUtility.SetDirty(scallionsSO);
            }

            // Customer: Night Coder
            var sprCustomer = AssetDatabase.LoadAssetAtPath<Sprite>($"{ArtPath}/spr_customer_night_coder.png");
            var customerSO = AssetDatabase.LoadAssetAtPath<CustomerDataSO>("Assets/_Project/ScriptableObjects/Customers/NightCoderCustomer.asset");
            if (customerSO != null && sprCustomer != null)
            {
                customerSO.portrait = sprCustomer;
                EditorUtility.SetDirty(customerSO);
            }

            // Customer: Vinyl Digger
            var sprVinylDigger = AssetDatabase.LoadAssetAtPath<Sprite>($"{ArtPath}/spr_customer_vinyl_digger.png");
            var vinylDiggerSO = AssetDatabase.LoadAssetAtPath<CustomerDataSO>("Assets/_Project/ScriptableObjects/Customers/VinylDiggerCustomer.asset");
            if (vinylDiggerSO != null && sprVinylDigger != null)
            {
                vinylDiggerSO.portrait = sprVinylDigger;
                EditorUtility.SetDirty(vinylDiggerSO);
            }

            // Customer: Rainy Student
            var sprRainyStudent = AssetDatabase.LoadAssetAtPath<Sprite>($"{ArtPath}/spr_customer_rainy_student.png");
            var rainyStudentSO = AssetDatabase.LoadAssetAtPath<CustomerDataSO>("Assets/_Project/ScriptableObjects/Customers/RainyStudentCustomer.asset");
            if (rainyStudentSO != null && sprRainyStudent != null)
            {
                rainyStudentSO.portrait = sprRainyStudent;
                EditorUtility.SetDirty(rainyStudentSO);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        // --- Texture Generators (256x256) ---

        private static void CreateBowlSprite()
        {
            int size = 256;
            var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            float center = size * 0.5f;
            float outerR = size * 0.48f;
            float innerR = size * 0.44f;
            float rimR = size * 0.46f;

            Color rimColor = new Color(0.12f, 0.18f, 0.32f, 1f); // Deep Indigo rim
            Color bowlColor = new Color(0.94f, 0.92f, 0.88f, 1f); // Ceramic off-white

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dist = Vector2.Distance(new Vector2(x, y), new Vector2(center, center));
                    if (dist > outerR)
                        tex.SetPixel(x, y, Color.clear);
                    else if (dist > rimR)
                        tex.SetPixel(x, y, rimColor);
                    else if (dist > innerR)
                        tex.SetPixel(x, y, Color.Lerp(rimColor, bowlColor, (rimR - dist) / (rimR - innerR)));
                    else
                        tex.SetPixel(x, y, bowlColor);
                }
            }
            tex.Apply();
            File.WriteAllBytes($"{ArtPath}/spr_bowl_base.png", tex.EncodeToPNG());
        }

        private static void CreateBrothSprite()
        {
            int size = 256;
            var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            float center = size * 0.5f;
            float r = size * 0.44f;
            Color centerColor = new Color(0.72f, 0.36f, 0.10f, 0.95f); // Rich warm Shoyu
            Color edgeColor = new Color(0.48f, 0.22f, 0.05f, 0.98f);   // Deep amber edge

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dist = Vector2.Distance(new Vector2(x, y), new Vector2(center, center));
                    if (dist > r)
                        tex.SetPixel(x, y, Color.clear);
                    else
                    {
                        float t = dist / r;
                        Color c = Color.Lerp(centerColor, edgeColor, t);
                        // Subtle soup surface glint
                        if (dist > r * 0.7f && dist < r * 0.85f && (x + y) % 18 < 6)
                            c = Color.Lerp(c, Color.white, 0.15f);
                        tex.SetPixel(x, y, c);
                    }
                }
            }
            tex.Apply();
            File.WriteAllBytes($"{ArtPath}/spr_broth_layer.png", tex.EncodeToPNG());
        }

        private static void CreateNoodlesSprite()
        {
            int size = 256;
            var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            float center = size * 0.5f;
            float r = size * 0.42f;
            Color noodleColor = new Color(1.0f, 0.96f, 0.82f, 1f); // Creamy noodle
            Color shadeColor = new Color(0.85f, 0.78f, 0.58f, 1f);  // Depth shade

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dist = Vector2.Distance(new Vector2(x, y), new Vector2(center, center));
                    if (dist > r)
                        tex.SetPixel(x, y, Color.clear);
                    else
                    {
                        // Wavy noodle pattern
                        float wave = Mathf.Sin(x * 0.2f + y * 0.1f) + Mathf.Cos(y * 0.25f - x * 0.08f);
                        Color c = (wave > 0.2f) ? noodleColor : shadeColor;
                        tex.SetPixel(x, y, c);
                    }
                }
            }
            tex.Apply();
            File.WriteAllBytes($"{ArtPath}/spr_noodles_layer.png", tex.EncodeToPNG());
        }

        private static void CreateChashuSprite()
        {
            int size = 128;
            var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            float center = size * 0.5f;
            float rx = size * 0.45f;
            float ry = size * 0.38f;

            Color meatColor = new Color(0.82f, 0.56f, 0.36f, 1f);  // Braised pork meat
            Color fatColor = new Color(0.96f, 0.90f, 0.82f, 1f);   // Pork fat ribbon
            Color rindColor = new Color(0.42f, 0.22f, 0.10f, 1f);  // Dark soy-glazed rind

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dx = (x - center) / rx;
                    float dy = (y - center) / ry;
                    float dist = dx * dx + dy * dy;

                    if (dist > 1f)
                        tex.SetPixel(x, y, Color.clear);
                    else if (dist > 0.85f)
                        tex.SetPixel(x, y, rindColor); // Roasted outer rind
                    else
                    {
                        // Marbled fat band
                        float swirl = Mathf.Sin((x + y) * 0.15f);
                        Color c = (swirl > 0.5f) ? fatColor : meatColor;
                        tex.SetPixel(x, y, c);
                    }
                }
            }
            tex.Apply();
            File.WriteAllBytes($"{ArtPath}/spr_chashu.png", tex.EncodeToPNG());
        }

        private static void CreateTamagoSprite()
        {
            int size = 128;
            var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            float center = size * 0.5f;
            float outerR = size * 0.44f;
            float yolkR = size * 0.24f;
            Vector2 yolkCenter = new Vector2(center, center - 4f);

            Color eggWhite = new Color(0.98f, 0.96f, 0.90f, 1f);   // Marinated white
            Color eggRind = new Color(0.70f, 0.55f, 0.38f, 1f);    // Soy-steeped outer rim
            Color yolkColor = new Color(1.0f, 0.58f, 0.05f, 1f);   // Glossy orange yolk
            Color yolkCore = new Color(1.0f, 0.40f, 0.02f, 1f);    // Deep jammy yolk core

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float distOuter = Vector2.Distance(new Vector2(x, y), new Vector2(center, center));
                    float distYolk = Vector2.Distance(new Vector2(x, y), yolkCenter);

                    if (distOuter > outerR)
                        tex.SetPixel(x, y, Color.clear);
                    else if (distYolk <= yolkR)
                    {
                        float t = distYolk / yolkR;
                        Color c = Color.Lerp(yolkCore, yolkColor, t);
                        // Glossy shine highlight
                        if (distYolk > yolkR * 0.3f && distYolk < yolkR * 0.6f && y > yolkCenter.y && x < yolkCenter.x)
                            c = Color.Lerp(c, Color.white, 0.4f);
                        tex.SetPixel(x, y, c);
                    }
                    else if (distOuter > outerR * 0.9f)
                        tex.SetPixel(x, y, eggRind);
                    else
                        tex.SetPixel(x, y, eggWhite);
                }
            }
            tex.Apply();
            File.WriteAllBytes($"{ArtPath}/spr_tamago.png", tex.EncodeToPNG());
        }

        private static void CreateScallionsSprite()
        {
            int size = 128;
            var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);

            Color clear = Color.clear;
            for (int y = 0; y < size; y++)
                for (int x = 0; x < size; x++)
                    tex.SetPixel(x, y, clear);

            // Draw multiple tiny ring sprinkles
            Vector2[] rings = new Vector2[]
            {
                new Vector2(40, 50), new Vector2(70, 75), new Vector2(90, 45),
                new Vector2(55, 95), new Vector2(80, 100), new Vector2(35, 80),
                new Vector2(65, 35), new Vector2(100, 80), new Vector2(50, 60)
            };

            Color outerGreen = new Color(0.18f, 0.68f, 0.20f, 1f);
            Color innerGreen = new Color(0.65f, 0.92f, 0.45f, 1f);

            foreach (var pt in rings)
            {
                DrawRing(tex, (int)pt.x, (int)pt.y, 12, 6, outerGreen, innerGreen);
            }

            tex.Apply();
            File.WriteAllBytes($"{ArtPath}/spr_scallions.png", tex.EncodeToPNG());
        }

        private static void DrawRing(Texture2D tex, int cx, int cy, int outerR, int innerR, Color outerCol, Color innerCol)
        {
            for (int y = cy - outerR; y <= cy + outerR; y++)
            {
                for (int x = cx - outerR; x <= cx + outerR; x++)
                {
                    if (x < 0 || x >= tex.width || y < 0 || y >= tex.height) continue;
                    float dist = Vector2.Distance(new Vector2(x, y), new Vector2(cx, cy));
                    if (dist <= outerR && dist >= innerR)
                    {
                        tex.SetPixel(x, y, outerCol);
                    }
                    else if (dist < innerR && dist >= innerR - 2)
                    {
                        tex.SetPixel(x, y, innerCol);
                    }
                }
            }
        }

        private static void CreateBasketSprite()
        {
            int size = 128;
            var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            float center = size * 0.5f;
            float r = size * 0.42f;
            Color meshColor = new Color(0.65f, 0.72f, 0.80f, 1f); // Stainless steel mesh
            Color gridColor = new Color(0.40f, 0.45f, 0.55f, 1f);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dist = Vector2.Distance(new Vector2(x, y), new Vector2(center, center));
                    if (dist > r)
                        tex.SetPixel(x, y, Color.clear);
                    else
                    {
                        bool isGrid = (x % 8 == 0) || (y % 8 == 0);
                        tex.SetPixel(x, y, isGrid ? gridColor : meshColor);
                    }
                }
            }
            tex.Apply();
            File.WriteAllBytes($"{ArtPath}/spr_basket.png", tex.EncodeToPNG());
        }

        private static void CreatePotSprite()
        {
            int size = 128;
            var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            float center = size * 0.5f;
            float r = size * 0.45f;
            Color steelColor = new Color(0.55f, 0.58f, 0.62f, 1f);
            Color soupColor = new Color(0.52f, 0.25f, 0.08f, 1f); // Shoyu inside

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dist = Vector2.Distance(new Vector2(x, y), new Vector2(center, center));
                    if (dist > r)
                        tex.SetPixel(x, y, Color.clear);
                    else if (dist > r * 0.75f)
                        tex.SetPixel(x, y, steelColor);
                    else
                        tex.SetPixel(x, y, soupColor);
                }
            }
            tex.Apply();
            File.WriteAllBytes($"{ArtPath}/spr_pot_shoyu.png", tex.EncodeToPNG());
        }

        private static void CreateCustomerNightCoderSprite()
        {
            int size = 256;
            var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            float cx = size * 0.5f;

            Color hoodieCol = new Color(0.14f, 0.16f, 0.24f, 1f);     // Dark cozy hoodie
            Color hoodieShade = new Color(0.10f, 0.11f, 0.18f, 1f);
            Color skinCol = new Color(0.96f, 0.88f, 0.82f, 1f);       // Pale face
            Color hairCol = new Color(0.18f, 0.20f, 0.28f, 1f);       // Dark indigo messy hair
            Color glassesCol = new Color(0.45f, 0.75f, 1.0f, 0.9f);    // Blue screen glow reflection
            Color eyesCol = new Color(0.20f, 0.22f, 0.30f, 1f);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    tex.SetPixel(x, y, Color.clear);

                    // 1. Shoulders / Hoodie Body (y: 0 to 110)
                    float bodyDist = Mathf.Pow((x - cx) / 105f, 2) + Mathf.Pow((y - 20f) / 95f, 2);
                    if (bodyDist <= 1.0f && y < 115)
                    {
                        tex.SetPixel(x, y, (x > cx - 20 && x < cx + 20) ? hoodieShade : hoodieCol);
                    }

                    // 2. Head / Face (y: 95 to 195)
                    float faceDist = Mathf.Pow((x - cx) / 52f, 2) + Mathf.Pow((y - 145f) / 58f, 2);
                    if (faceDist <= 1.0f)
                    {
                        tex.SetPixel(x, y, skinCol);
                    }

                    // 3. Eyes / Glasses (y: 135 to 155)
                    float leftLens = Vector2.Distance(new Vector2(x, y), new Vector2(cx - 24, 142));
                    float rightLens = Vector2.Distance(new Vector2(x, y), new Vector2(cx + 24, 142));
                    if ((leftLens <= 15f && leftLens >= 12f) || (rightLens <= 15f && rightLens >= 12f))
                    {
                        tex.SetPixel(x, y, glassesCol);
                    }
                    else if (leftLens < 12f || rightLens < 12f)
                    {
                        // Soft blue tint inside glasses
                        tex.SetPixel(x, y, Color.Lerp(skinCol, glassesCol, 0.25f));
                    }

                    // Bridge of glasses
                    if (Mathf.Abs(y - 143) <= 2 && Mathf.Abs(x - cx) <= 12)
                    {
                        tex.SetPixel(x, y, glassesCol);
                    }

                    // Relaxed tired eyes behind lenses
                    if (y >= 140 && y <= 143)
                    {
                        if ((x >= cx - 30 && x <= cx - 18) || (x >= cx + 18 && x <= cx + 30))
                        {
                            tex.SetPixel(x, y, eyesCol);
                        }
                    }

                    // 4. Hair / Bangs (y: 160 to 225)
                    float hairTopDist = Mathf.Pow((x - cx) / 58f, 2) + Mathf.Pow((y - 170f) / 52f, 2);
                    if (hairTopDist <= 1.0f && y > 148)
                    {
                        tex.SetPixel(x, y, hairCol);
                    }
                    // Messy strand spikes
                    if (y > 140 && y < 175)
                    {
                        float wave = Mathf.Sin(x * 0.15f) * 12f;
                        if (y < 165 + wave && faceDist <= 1.15f)
                        {
                            tex.SetPixel(x, y, hairCol);
                        }
                    }

                    // 5. Hoodie Rim / Collar around neck
                    float collarDist = Mathf.Pow((x - cx) / 60f, 2) + Mathf.Pow((y - 105f) / 25f, 2);
                    if (collarDist <= 1.0f && collarDist >= 0.7f && y < 120)
                    {
                        tex.SetPixel(x, y, hoodieCol);
                    }
                }
            }
            tex.Apply();
            File.WriteAllBytes($"{ArtPath}/spr_customer_night_coder.png", tex.EncodeToPNG());
        }

        private static void CreateCustomerVinylDiggerSprite()
        {
            int size = 256;
            var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            float cx = size * 0.5f;

            Color sweaterCol = new Color(0.85f, 0.58f, 0.18f, 1f);   // Mustard amber knit sweater
            Color sweaterShade = new Color(0.70f, 0.46f, 0.12f, 1f);
            Color skinCol = new Color(0.94f, 0.84f, 0.76f, 1f);       // Warm skin tone
            Color hairCol = new Color(0.24f, 0.16f, 0.12f, 1f);       // Dark wavy brown hair
            Color phoneBandCol = new Color(0.18f, 0.20f, 0.26f, 1f);  // Studio headphone band
            Color phoneMetalCol = new Color(0.80f, 0.82f, 0.88f, 1f); // Brushed silver metal
            Color eyesCol = new Color(0.22f, 0.18f, 0.16f, 1f);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    tex.SetPixel(x, y, Color.clear);

                    // 1. Shoulders & Sweater Body (y: 0 to 115)
                    float bodyDist = Mathf.Pow((x - cx) / 105f, 2) + Mathf.Pow((y - 20f) / 95f, 2);
                    if (bodyDist <= 1.0f && y < 118)
                    {
                        bool isRibbed = (x % 6 == 0);
                        tex.SetPixel(x, y, isRibbed ? sweaterShade : sweaterCol);
                    }

                    // 2. Head / Face (y: 95 to 195)
                    float faceDist = Mathf.Pow((x - cx) / 52f, 2) + Mathf.Pow((y - 145f) / 58f, 2);
                    if (faceDist <= 1.0f)
                    {
                        tex.SetPixel(x, y, skinCol);
                    }

                    // 3. Relaxed Eyes (y: 138 to 144)
                    float leftEyeDist = Vector2.Distance(new Vector2(x, y), new Vector2(cx - 22, 142));
                    float rightEyeDist = Vector2.Distance(new Vector2(x, y), new Vector2(cx + 22, 142));
                    if (leftEyeDist <= 4f || rightEyeDist <= 4f)
                    {
                        tex.SetPixel(x, y, eyesCol);
                    }

                    // Subtle chill smile (y: 121 to 124)
                    if (y >= 121 && y <= 124 && Mathf.Abs(x - cx) <= 14)
                    {
                        float curve = Mathf.Pow((x - cx) / 14f, 2) * 3f;
                        if (y <= 122 + curve)
                        {
                            tex.SetPixel(x, y, new Color(0.55f, 0.30f, 0.24f, 1f));
                        }
                    }

                    // 4. Wavy Hair (y: 155 to 215)
                    float hairDist = Mathf.Pow((x - cx) / 58f, 2) + Mathf.Pow((y - 165f) / 52f, 2);
                    if (hairDist <= 1.05f && y > 150)
                    {
                        tex.SetPixel(x, y, hairCol);
                    }

                    // 5. Studio Headphones - Over-Ear Cushions (x: cx ± 56, y: 142)
                    float leftCup = Vector2.Distance(new Vector2(x, y), new Vector2(cx - 54, 142));
                    float rightCup = Vector2.Distance(new Vector2(x, y), new Vector2(cx + 54, 142));
                    if (leftCup <= 19f || rightCup <= 19f)
                    {
                        if (leftCup >= 15f || rightCup >= 15f)
                            tex.SetPixel(x, y, phoneMetalCol); // Silver metallic rim
                        else
                            tex.SetPixel(x, y, phoneBandCol);  // Dark leather cushion
                    }

                    // Headphone Arch over the head
                    float archDist = Mathf.Pow((x - cx) / 64f, 2) + Mathf.Pow((y - 165f) / 56f, 2);
                    if (archDist <= 1.22f && archDist >= 1.05f && y >= 165)
                    {
                        tex.SetPixel(x, y, phoneBandCol);
                    }
                }
            }
            tex.Apply();
            File.WriteAllBytes($"{ArtPath}/spr_customer_vinyl_digger.png", tex.EncodeToPNG());
        }

        private static void CreateCustomerRainyStudentSprite()
        {
            int size = 256;
            var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            float cx = size * 0.5f;

            Color coatCol = new Color(0.18f, 0.38f, 0.28f, 1f);      // Emerald green rain coat
            Color coatShade = new Color(0.12f, 0.26f, 0.18f, 1f);
            Color scarfCol = new Color(0.92f, 0.88f, 0.80f, 1f);     // Cozy cream knit scarf
            Color scarfShade = new Color(0.78f, 0.74f, 0.65f, 1f);
            Color skinCol = new Color(0.97f, 0.89f, 0.84f, 1f);       // Soft warm skin tone
            Color hairCol = new Color(0.38f, 0.25f, 0.18f, 1f);       // Warm chestnut brown hair
            Color glassesCol = new Color(0.88f, 0.74f, 0.36f, 1f);    // Gold wire round spectacles
            Color eyesCol = new Color(0.24f, 0.18f, 0.16f, 1f);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    tex.SetPixel(x, y, Color.clear);

                    // 1. Shoulders & Coat Body (y: 0 to 110)
                    float bodyDist = Mathf.Pow((x - cx) / 105f, 2) + Mathf.Pow((y - 20f) / 95f, 2);
                    if (bodyDist <= 1.0f && y < 112)
                    {
                        tex.SetPixel(x, y, (x > cx - 18 && x < cx + 18) ? coatShade : coatCol);
                    }

                    // 2. Thick Knitted Scarf (y: 85 to 125)
                    float scarfDist = Mathf.Pow((x - cx) / 58f, 2) + Mathf.Pow((y - 105f) / 22f, 2);
                    if (scarfDist <= 1.0f && y < 122)
                    {
                        bool isKnit = ((x + y) % 8 < 4);
                        tex.SetPixel(x, y, isKnit ? scarfCol : scarfShade);
                    }

                    // 3. Head / Face (y: 110 to 195)
                    float faceDist = Mathf.Pow((x - cx) / 50f, 2) + Mathf.Pow((y - 150f) / 52f, 2);
                    if (faceDist <= 1.0f)
                    {
                        tex.SetPixel(x, y, skinCol);
                    }

                    // 4. Soft Eyes behind glasses
                    float leftEyeDist = Vector2.Distance(new Vector2(x, y), new Vector2(cx - 22, 145));
                    float rightEyeDist = Vector2.Distance(new Vector2(x, y), new Vector2(cx + 22, 145));
                    if (leftEyeDist <= 3.5f || rightEyeDist <= 3.5f)
                    {
                        tex.SetPixel(x, y, eyesCol);
                    }

                    // Gentle smile (y: 125 to 128)
                    if (y >= 125 && y <= 128 && Mathf.Abs(x - cx) <= 12)
                    {
                        float curve = Mathf.Pow((x - cx) / 12f, 2) * 2.5f;
                        if (y <= 126 + curve)
                        {
                            tex.SetPixel(x, y, new Color(0.60f, 0.32f, 0.28f, 1f));
                        }
                    }

                    // 5. Round Gold Wire Glasses (radius 14)
                    float leftGlass = Vector2.Distance(new Vector2(x, y), new Vector2(cx - 22, 145));
                    float rightGlass = Vector2.Distance(new Vector2(x, y), new Vector2(cx + 22, 145));
                    if ((leftGlass <= 15f && leftGlass >= 13f) || (rightGlass <= 15f && rightGlass >= 13f))
                    {
                        tex.SetPixel(x, y, glassesCol);
                    }
                    // Bridge
                    if (Mathf.Abs(y - 146) <= 1.5f && Mathf.Abs(x - cx) <= 10)
                    {
                        tex.SetPixel(x, y, glassesCol);
                    }

                    // 6. Hair & Top-knot Bun (y: 160 to 235)
                    float hairDist = Mathf.Pow((x - cx) / 56f, 2) + Mathf.Pow((y - 170f) / 48f, 2);
                    if (hairDist <= 1.05f && y > 152)
                    {
                        tex.SetPixel(x, y, hairCol);
                    }

                    // Messy Top-knot Bun (cx, 218)
                    float bunDist = Vector2.Distance(new Vector2(x, y), new Vector2(cx, 218));
                    if (bunDist <= 24f)
                    {
                        tex.SetPixel(x, y, hairCol);
                    }
                }
            }
            tex.Apply();
            File.WriteAllBytes($"{ArtPath}/spr_customer_rainy_student.png", tex.EncodeToPNG());
        }

        private static void CreateOrderTicketSprite()
        {
            int w = 256;
            int h = 320;
            var tex = new Texture2D(w, h, TextureFormat.RGBA32, false);

            Color paperCol = new Color(0.97f, 0.95f, 0.90f, 1f);     // Parchment paper
            Color paperEdge = new Color(0.88f, 0.84f, 0.76f, 1f);    // Paper shadow/rim
            Color clipCol = new Color(0.65f, 0.42f, 0.22f, 1f);      // Wooden clothespin
            Color clipMetal = new Color(0.85f, 0.75f, 0.35f, 1f);    // Brass spring

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    tex.SetPixel(x, y, Color.clear);

                    // 1. Paper Body (margin 16px, top margin 40px)
                    if (x >= 18 && x <= w - 18 && y >= 14 && y <= h - 45)
                    {
                        // Torn serrated bottom edge
                        if (y < 24 && ((x + y) % 10 < 4))
                            continue;

                        bool isEdge = (x <= 22 || x >= w - 22 || y >= h - 49);
                        tex.SetPixel(x, y, isEdge ? paperEdge : paperCol);
                    }

                    // 2. Wooden Clip at top center
                    if (x >= w / 2 - 20 && x <= w / 2 + 20 && y >= h - 55 && y <= h - 10)
                    {
                        tex.SetPixel(x, y, clipCol);
                    }
                    // Metal hinge/spring on clip
                    if (x >= w / 2 - 12 && x <= w / 2 + 12 && y >= h - 35 && y <= h - 30)
                    {
                        tex.SetPixel(x, y, clipMetal);
                    }
                }
            }
            tex.Apply();
            File.WriteAllBytes($"{ArtPath}/spr_order_ticket.png", tex.EncodeToPNG());
        }

        private static void CreateCoinSprite()
        {
            int size = 128;
            var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            float center = size * 0.5f;
            float outerR = size * 0.45f;
            float rimR = size * 0.40f;
            float innerR = size * 0.32f;

            Color goldRim = new Color(0.85f, 0.65f, 0.12f, 1f);
            Color goldBody = new Color(1.0f, 0.84f, 0.20f, 1f);
            Color goldShine = new Color(1.0f, 0.94f, 0.55f, 1f);
            Color goldEmboss = new Color(0.78f, 0.56f, 0.08f, 1f);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dist = Vector2.Distance(new Vector2(x, y), new Vector2(center, center));
                    if (dist > outerR)
                    {
                        tex.SetPixel(x, y, Color.clear);
                    }
                    else if (dist > rimR)
                    {
                        tex.SetPixel(x, y, goldRim);
                    }
                    else if (dist > innerR)
                    {
                        // Inner ring groove
                        tex.SetPixel(x, y, goldEmboss);
                    }
                    else
                    {
                        // Coin surface with top-left specular highlight
                        Color c = goldBody;
                        if (x < center && y > center && dist < innerR * 0.7f)
                            c = goldShine;

                        // Center Yen symbol pattern (simple crossbars)
                        int dx = Mathf.Abs((int)(x - center));
                        int dy = Mathf.Abs((int)(y - center));
                        if ((dx <= 2 && y <= center + 14 && y >= center - 14) ||
                            (dy <= 2 && dx <= 10 && y >= center - 4) ||
                            (Mathf.Abs(dx - (y - center)) <= 1 && y >= center && y <= center + 14))
                        {
                            c = goldEmboss;
                        }

                        tex.SetPixel(x, y, c);
                    }
                }
            }
            tex.Apply();
            File.WriteAllBytes($"{ArtPath}/spr_coin.png", tex.EncodeToPNG());
        }

        private static void CreateSpeechBubbleSprite()
        {
            int w = 512;
            int h = 180;
            var tex = new Texture2D(w, h, TextureFormat.RGBA32, false);

            float minX = 16f, maxX = w - 16f;
            float minY = 36f, maxY = h - 16f;
            float radius = 24f;

            Color bgColor = new Color(0.08f, 0.10f, 0.16f, 0.94f);
            Color borderColor = new Color(0.85f, 0.70f, 0.40f, 0.92f);
            float borderThickness = 4f;

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    bool inBox = false;
                    float distToCorner = 0f;

                    // Clamped point for box rounded rectangle SDF
                    float cx = Mathf.Clamp(x, minX + radius, maxX - radius);
                    float cy = Mathf.Clamp(y, minY + radius, maxY - radius);
                    distToCorner = Vector2.Distance(new Vector2(x, y), new Vector2(cx, cy));

                    if (distToCorner <= radius && x >= minX && x <= maxX && y >= minY && y <= maxY)
                    {
                        inBox = true;
                    }

                    // Tail triangle at bottom center
                    bool inTail = false;
                    if (y < minY && y >= 6)
                    {
                        float halfW = ((y - 6f) / (minY - 6f)) * 26f;
                        if (Mathf.Abs(x - (w * 0.5f)) <= halfW)
                        {
                            inTail = true;
                        }
                    }

                    if (!inBox && !inTail)
                    {
                        tex.SetPixel(x, y, Color.clear);
                    }
                    else
                    {
                        // Check if on border
                        bool isBorder = false;

                        if (inBox)
                        {
                            if (distToCorner >= radius - borderThickness)
                                isBorder = true;
                        }

                        if (inTail)
                        {
                            float halfW = ((y - 6f) / (minY - 6f)) * 26f;
                            if (Mathf.Abs(Mathf.Abs(x - (w * 0.5f)) - halfW) <= borderThickness || y <= 6 + borderThickness)
                                isBorder = true;
                        }

                        // Remove inner seam between box bottom and tail top
                        if (inTail && y >= minY - borderThickness && Mathf.Abs(x - (w * 0.5f)) < 24f)
                            isBorder = false;
                        if (inBox && y <= minY + borderThickness && Mathf.Abs(x - (w * 0.5f)) < 24f)
                            isBorder = false;

                        tex.SetPixel(x, y, isBorder ? borderColor : bgColor);
                    }
                }
            }

            tex.Apply();
            File.WriteAllBytes($"{ArtPath}/spr_speech_bubble.png", tex.EncodeToPNG());
        }
    }
}
#endif
