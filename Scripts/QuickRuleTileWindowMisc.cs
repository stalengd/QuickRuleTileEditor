using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;

namespace QuickRuleTileEditor
{
    // Some other methods which cannot be removed from the class
    public partial class QuickRuleTileWindow
    {
        public void ShowObjectPicker<T>
            (Object obj, bool allowSceneObjects, string searchFilter, System.Action<Object> callback) where T : Object
        {
            objectPickerCallback = callback;
            var controlID = GUIUtility.GetControlID(FocusType.Passive);
            EditorGUIUtility.ShowObjectPicker<T>(obj, allowSceneObjects, searchFilter, controlID);
        }
    }
}
