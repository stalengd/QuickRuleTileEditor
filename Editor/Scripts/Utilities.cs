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
            subAssets = subAssets.OrderBy(x =>
            {
                string trailingNumber = string.Concat(x.name.ToArray().Reverse().TakeWhile(char.IsNumber).Reverse());
                int index = 0;
                int.TryParse(trailingNumber, out index);
                return index;
            }).ToArray();

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
