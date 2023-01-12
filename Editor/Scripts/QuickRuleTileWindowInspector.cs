using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;

namespace QuickRuleTileEditor
{
    // Inspector part
    public partial class QuickRuleTileWindow
    {
        private void RefreshInspector()
        {
            var isDefaultSelected = model.SelectedTile == -1;
            selectedTileLabel.text = isDefaultSelected ? "Default" : $"#{model.SelectedTile}";
            selectedTilePatternImage.sprite = model.GetPatternSprite(model.SelectedTile);
            selectedTileSpriteImage.sprite = model.GetTileSprite(model.SelectedTile);

            var ruleOutput = model.GetTileOutput(model.SelectedTile);
            inspectorGameObjectField.value = ruleOutput.m_GameObject;
            inspectorColliderField.value = ruleOutput.m_ColliderType;
        }

        private void ToggleInspectorVisibility()
        {
            if (GetInspectorWidth() > 0) 
            {
                SetInspectorWidth(0);
            }
            else
            {
                SetInspectorWidth(QuickRuleTileConfig.initialInspectorSize);
            }
        }

        
        private void SetInspectorWidth(float value)
        {
            inspectorSplitView.fixedPaneInitialDimension = value;
        }
        private float GetInspectorWidth()
        {
            return inspectorSplitView.fixedPane.style.width.value.value;
        }
        

        private void OnSelectedTileGameObjectChanged(ChangeEvent<Object> e)
        {
            var value = (GameObject)e.newValue;
            var ruleOutput = model.GetTileOutput(model.SelectedTile);
            ruleOutput.m_GameObject = value;
        }

        private void OnSelectedTileColliderChanged(ChangeEvent<System.Enum> e)
        {
            var value = (Tile.ColliderType)e.newValue;
            var ruleOutput = model.GetTileOutput(model.SelectedTile);
            ruleOutput.m_ColliderType = value;
        }

        private void OnSelectedTileOutputChanged(ChangeEvent<System.Enum> e)
        {
            // TODO: Tile output support
        }

        private void ApplySelectedTileGameObjectForAll()
        {
            var gameObject = model.GetTileOutput(model.SelectedTile).m_GameObject;
            foreach (var output in model.AllOutputs)
            {
                output.m_GameObject = gameObject;
            }
        }

        private void ApplySelectedTileColliderForAll()
        {
            var collider = model.GetTileOutput(model.SelectedTile).m_ColliderType;
            foreach (var output in model.AllOutputs)
            {
                output.m_ColliderType = collider;
            }
        }

        private void ApplySelectedTileOutputForAll()
        {
            // TODO: Tile output support
        }
    }
}
