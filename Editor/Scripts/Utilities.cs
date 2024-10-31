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
            Object[] subAssets = AssetDatabase.LoadAllAssetsAtPath(path);

            //Sort ths Sprite by their name's trail number.
            subAssets = subAssets.OrderBy(x => {
                string indexStr = x.name.Substring(x.name.LastIndexOf('_') + 1);
                int index = 0;
                int.TryParse(indexStr, out index);
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
