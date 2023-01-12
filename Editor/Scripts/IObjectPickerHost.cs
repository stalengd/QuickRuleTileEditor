using UnityEngine;

namespace QuickRuleTileEditor
{
    public interface IObjectPickerHost
    {
        void ShowObjectPicker<T>(Object obj, bool allowSceneObjects, string searchFilter, System.Action<Object> callback) where T : Object;
    }
}
