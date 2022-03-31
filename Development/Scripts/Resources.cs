using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Phedg1Studios {
    namespace StartingItemsGUI {
        public class Resources : MonoBehaviour {
            static public List<Sprite> tierTextures = new List<Sprite>();
            static public List<Sprite> panelTextures = new List<Sprite>();
            static public List<TMPro.TMP_FontAsset> fonts = new List<TMPro.TMP_FontAsset>();

            static private List<string> tierTextureNames = new List<string>() {
                "textier1bgicon.png",
                "textier2bgicon.png",
                "textier3bgicon.png",
                "texbossbgicon.png",
                "texlunarbgicon.png",
                "texequipmentbgicon.png",
            };
            static private List<string> panelTextureNames = new List<string>() {
                "texUICleanButton.png",
                "texUIOutlineOnly.png",
                "texUIHighlightBoxOutlineThick.png",
                "texUIAnimateSliceNakedButton.png",
                "texUIAnimateSliceNakedButtonCheat.png",
                "texUIHighlightBoxOutlineThickIcon.png",
                "texUIHandle.png",
                "texUIHighlightHeader.png",
                "texUIBottomUpFade.png",
                "texUITopDownFade.png",
            };
            static private List<string> fontNames = new List<string>() {
                "BOMBARD_ SDF.asset",
            };

            static public List<Color> colours = new List<Color>() {
                new Color(193f / 255f, 193f / 255f, 193f / 255f),
                new Color(88f / 255f, 149f / 255f, 88f / 255f),
                new Color(142f / 255f, 50f / 255f, 50f / 255f),
                new Color(189f / 255f, 180f / 255f, 61f / 255f),
                new Color(50f / 255f, 127f / 255f, 255f / 255f),
                new Color(255f / 255f, 128f / 255f, 0f / 255f),
                new Color(0 / 255f, 0 / 255f, 0f / 255f),
            };

            // The textures and fonts lists will be populated.
            static public void LoadResources()
            {
                var pluginDirectory = System.IO.Directory.GetParent(StartingItemsGUI.Instance.PInfo.Location);
                var assetLocation = System.IO.Path.Combine(pluginDirectory.FullName, "Resources", "StartingItemsGUIAssets");
                var fileAssets = UnityEngine.AssetBundle.LoadFromFile(assetLocation);

                foreach (string tierTextureName in tierTextureNames)
                {
                    tierTextures.Add(fileAssets.LoadAsset<Sprite>(tierTextureName));
                }

                foreach (string panelTextureName in panelTextureNames)
                {
                    panelTextures.Add(fileAssets.LoadAsset<Sprite>(panelTextureName));
                }

                foreach (string fontName in fontNames)
                {
                    fonts.Add(fileAssets.LoadAsset<TMPro.TMP_FontAsset>(fontName));
                }

                fileAssets.Unload(false);

                Log.LogInfo("Finished loading assets.");
                Log.LogInfo($"Tier texture count: {tierTextures.Count}");
                Log.LogInfo($"Panel texture count: {panelTextures.Count}");
                Log.LogInfo($"Font count: {fonts.Count}");
            }
        }
    }
}
