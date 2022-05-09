using System.Collections.Generic;
using UnityEngine;

namespace StartingItemsGUI
{
    public class Resources : MonoBehaviour
    {
        public static List<Sprite> tierTextures = new();
        public static List<Sprite> panelTextures = new();
        public static List<TMPro.TMP_FontAsset> fonts = new();

        private static readonly List<string> tierTextureNames = new()
        {
            "textier1bgicon.png",
            "textier2bgicon.png",
            "textier3bgicon.png",
            "texbossbgicon.png",
            "texlunarbgicon.png",
            "texequipmentbgicon.png",
        };
        private static readonly List<string> panelTextureNames = new()
        {
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
        private static readonly List<string> fontNames = new()
        {
            "BOMBARD_ SDF.asset",
        };

        public static readonly List<Color> colours = new()
        {
            new Color(193f / 255f, 193f / 255f, 193f / 255f),
            new Color(88f / 255f, 149f / 255f, 88f / 255f),
            new Color(142f / 255f, 50f / 255f, 50f / 255f),
            new Color(189f / 255f, 180f / 255f, 61f / 255f),
            new Color(50f / 255f, 127f / 255f, 255f / 255f),
            new Color(255f / 255f, 128f / 255f, 0f / 255f),
            new Color(0 / 255f, 0 / 255f, 0f / 255f),
        };

        /// <summary>
        /// Load all the resources from the assets file.
        /// </summary>
        public static void LoadResources()
        {
            var pluginDirectory = System.IO.Directory.GetParent(StartingItemsGUI.Instance.PInfo.Location);
            var assetLocation = System.IO.Path.Combine(pluginDirectory.FullName, "Resources", "StartingItemsGUIAssets");
            var fileAssets = UnityEngine.AssetBundle.LoadFromFile(assetLocation);

            foreach (var tierTextureName in tierTextureNames)
            {
                tierTextures.Add(fileAssets.LoadAsset<Sprite>(tierTextureName));
            }

            foreach (var panelTextureName in panelTextureNames)
            {
                panelTextures.Add(fileAssets.LoadAsset<Sprite>(panelTextureName));
            }

            foreach (var fontName in fontNames)
            {
                fonts.Add(fileAssets.LoadAsset<TMPro.TMP_FontAsset>(fontName));
            }

            fileAssets.Unload(false);

            Log.LogInfo("Finished loading assets.");
            Log.LogInfo($"Tier texture count: {tierTextures.Count}");
            Log.LogInfo($"Panel texture count: {panelTextures.Count}");
            Log.LogInfo($"Font count: {fonts.Count}");

            // Ensure we have loaded all the resources correctly.
            System.Diagnostics.Debug.Assert(tierTextures.Count == tierTextureNames.Count);
            System.Diagnostics.Debug.Assert(panelTextures.Count == panelTextureNames.Count);
            System.Diagnostics.Debug.Assert(fonts.Count == fontNames.Count);
        }
    }
}
