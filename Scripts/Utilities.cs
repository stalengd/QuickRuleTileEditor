using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;

namespace QuickRuleTileEditor
{
    public static class Utilities
    {
        public static IEnumerable<Sprite> GetSpritesFromTexture(Object texture)
        {
            var path = AssetDatabase.GetAssetPath(texture);
            var subAssets = AssetDatabase.LoadAllAssetsAtPath(path);
            return subAssets.OfType<Sprite>();
        }
    }

    public static class VisualElementExtensions
    {
        internal static void SetHidden(this VisualElement element, bool isHidden)
        {
            element.EnableInClassList("hidden", isHidden);
        }
    }
}
