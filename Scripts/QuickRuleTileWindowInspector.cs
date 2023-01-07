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
            selectedTileLabel.text = $"#{selectedTile}";
            selectedTilePatternImage.sprite = GetPatternSprite(selectedTile);
            selectedTileSpriteImage.sprite = GetTileSprite(selectedTile);

            var ruleOutput = tiles[selectedTile];
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
            var ruleOutput = tiles[selectedTile];
            ruleOutput.m_GameObject = value;
        }

        private void OnSelectedTileColliderChanged(ChangeEvent<System.Enum> e)
        {
            var value = (Tile.ColliderType)e.newValue;
            var ruleOutput = tiles[selectedTile];
            ruleOutput.m_ColliderType = value;
        }

        private void OnSelectedTileOutputChanged(ChangeEvent<System.Enum> e)
        {
            // TODO: Tile output support
        }

        private void ApplySelectedTileGameObjectForAll()
        {
            var gameObject = tiles[selectedTile].m_GameObject;
            foreach (var ruleOutput in tiles)
            {
                ruleOutput.m_GameObject = gameObject;
            }
        }

        private void ApplySelectedTileColliderForAll()
        {
            var collider = tiles[selectedTile].m_ColliderType;
            foreach (var ruleOutput in tiles)
            {
                ruleOutput.m_ColliderType = collider;
            }
        }

        private void ApplySelectedTileOutputForAll()
        {
            // TODO: Tile output support
        }
    }
}
