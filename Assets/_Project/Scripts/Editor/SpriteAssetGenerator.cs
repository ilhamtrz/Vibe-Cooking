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
    }
}
#endif
