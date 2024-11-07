using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;

namespace QuickRuleTileEditor
{
    public static class Utilities
    {
        public static IEnumerable<Sprite> GetSpritesFromTexture(Object texture)
        {
            var path = AssetDatabase.GetAssetPath(texture);
            Object[] subAssets = AssetDatabase.LoadAllAssetsAtPath(path);

            //Sort the Sprite by their name's trailing number.
            System.Array.Sort(subAssets, (x, y) => EditorUtility.NaturalCompare(x.name, y.name));

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
