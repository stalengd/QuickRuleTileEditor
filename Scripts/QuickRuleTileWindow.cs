using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;

namespace QuickRuleTileEditor
{
    // Class is partial, so this file file contains the main part.
    // Other files:
    //   - QuickRuleTileWindowStructure.cs - UI setup
    //   - QuickRuleTileWindowInspector.cs - Inspector
    //   - QuickRuleTileWindowMisc.cs - Other
    public partial class QuickRuleTileWindow : EditorWindow, IObjectPickerHost
    {
        [SerializeField] private List<Sprite> sprites = new();
        [SerializeField] private int selectedTile = 0;
        [SerializeField] private List<RuleTile.TilingRuleOutput> tiles = new();
        [SerializeField] private RuleTile pattern;
        [SerializeField] private string patternId;
        [SerializeField] private RuleTile tileToEdit;
        [SerializeField] private TilesDisplayMode displayMode = TilesDisplayMode.Mixed;

        private int TilesCount => tiles.Count;
        private int PatternSize => pattern.m_TilingRules.Count;

        private StyleSheet styleSheet;
        private System.Action<Object> objectPickerCallback;
        private IRuleTilePatternsProvider patternsProvider = new RuleTilePatternsProvider();


        private enum TilesDisplayMode
        {
            Pattern,
            Mixed,
            Sprite
        }


        [MenuItem("Window/Quick Rule Tile")]
        public static void Open()
        {
            GetWindow<QuickRuleTileWindow>("Quick Rule Tile");
        }


        private void Awake()
        {
            SelectPattern(patternsProvider.FirstDefaultPatternId);
        }

        private void OnEnable()
        {
            styleSheet = Resources.Load<StyleSheet>("QuickRuleTileWindow");

            CreateRoot();
            rootVisualElement.Add(root);

            RefreshSelectedTile();
            SetDisplayMode(displayMode);
            RefreshEditorMode();
        }

        private void OnGUI()
        {
            if (Event.current.commandName == "ObjectSelectorClosed" && objectPickerCallback != null)
            {
                objectPickerCallback(EditorGUIUtility.GetObjectPickerObject());
                objectPickerCallback = null;
            }
        }


        private void SelectPattern(string id)
        {
            patternsProvider.LoadPattern(id, this, pattern =>
            {
                if (pattern != null)
                {
                    SetPattern(id, pattern);
                }
                else
                {
                    patternDropdown.value = patternId;
                }
            });
        }

        private void SetPattern(string id, RuleTile pattern)
        {
            patternId = id;
            this.pattern = pattern;
            selectedTile = 0;
            var oldTilesCount = TilesCount;

            var existingTileSpritesCount = 0;
            for (int i = tiles.Count - 1; i >= 0; i--)
            {
                if (GetTileSprite(i) != null)
                {
                    existingTileSpritesCount = i + 1;
                    break;
                }
            }

            var tilesCount = Mathf.Max(pattern.m_TilingRules.Count, existingTileSpritesCount);

            for (int i = 0; i < tilesCount - oldTilesCount; i++)
            {
                var index = tiles.Count;
                RuleTile.TilingRuleOutput ruleOutput;
                if (pattern.m_TilingRules.Count < index)
                {
                    ruleOutput = pattern.m_TilingRules[index].Clone();
                }
                else
                {
                    ruleOutput = new RuleTile.TilingRuleOutput();
                }
                tiles.Add(ruleOutput);
            }
            if (tilesContainer != null)
            {
                tilesContainer.Clear();
                for (int i = 0; i < tilesCount; i++)
                {
                    AddTileToContainer(i);
                }
                RefreshSelectedTile();
            }
        }

        private Sprite GetPatternSprite(int index)
        {
            if (index < 0 || index >= PatternSize) return null;
            return pattern.m_TilingRules[index].m_Sprites[0];
        }

        private void Clear()
        {
            tileToEdit = null;
            tiles.Clear();
            ClearSprites();
            SetPattern(patternId, pattern);
            RefreshEditorMode();
        }

        private void OpenRuleTile()
        {
            ShowObjectPicker<RuleTile>(null, false, null, obj =>
            {
                if (obj == null)
                {
                    return;
                }

                var tile = (RuleTile)obj;
                tileToEdit = tile;

                var textures = new HashSet<Texture>();

                tiles.Clear();
                tiles.AddRange(tile.m_TilingRules.Select(r => r.Clone()));
                SetPattern(patternId, pattern);
                for (int i = 0; i < tile.m_TilingRules.Count; i++)
                {
                    var sprite = tile.m_TilingRules[i].m_Sprites[0];
                    SetSpriteForTile(i, sprite);
                    if (sprite != null)
                    {
                        textures.Add(sprite.texture);
                    }
                }

                ClearSprites();
                foreach (var texture in textures)
                {
                    foreach (var sprite in Utilities.GetSpritesFromTexture(texture))
                    {
                        AddSprite(sprite);
                    }
                }
                RefreshEditorMode();
            });
        }
        
        private void RefreshEditorMode()
        {
            creationModeContainer.SetHidden(tileToEdit != null);
            editModeContainer.SetHidden(tileToEdit == null);

            if (tileToEdit != null) 
            {
                var infoLabel = editModeContainer.Query<Label>(className: "edit-info-label").First();
                infoLabel.text = $"Editing \"{tileToEdit.name}\"";
            }
        }

        private void SpriteClicked(Sprite sprite)
        {
            SetSpriteForTile(selectedTile, sprite);
            selectedTile = (selectedTile + 1) % PatternSize;
            RefreshSelectedTile();
        }

        private void SetSpriteForTile(int tileIndex, Sprite sprite)
        {
            var tileImage = tilesContainer
                .ElementAt(tileIndex)
                .ElementAt(0) as Image;
            tileImage.sprite = sprite;
            tiles[tileIndex].m_Sprites[0] = sprite;
        }

        private Sprite GetTileSprite(int tileIndex)
        {
            return tiles[tileIndex].m_Sprites[0];
        }

        private void SpriteMouseUp(MouseUpEvent e)
        {
            if (e.button == 1)
            {
                var image = e.target as Image;
                var index = image.parent.IndexOf(image);
                sprites.RemoveAt(index);
                image.RemoveFromHierarchy();
            }
        }

        private void TileClicked(int tileIndex)
        {
            selectedTile = tileIndex;
            RefreshSelectedTile();
        }

        private void RefreshSelectedTile()
        {
            int i = 0;
            foreach (var tile in tilesContainer.Children())
            {
                tile.EnableInClassList("selected-tile", i == selectedTile);
                i++;
            }
            RefreshInspector();
        }

        private void SetDisplayMode(TilesDisplayMode mode)
        {
            displayMode = mode;

            root.EnableInClassList("tiles-display__pattern", mode == TilesDisplayMode.Pattern);
            root.EnableInClassList("tiles-display__mixed", mode == TilesDisplayMode.Mixed);
            root.EnableInClassList("tiles-display__sprite", mode == TilesDisplayMode.Sprite);
        }

        private void OnAssetDrop(Object asset)
        {
            if (asset is Sprite)
            {
                AddSprite((Sprite)asset);
            }
            else if (asset is Texture)
            {
                foreach (var sprite in Utilities.GetSpritesFromTexture(asset))
                {
                    AddSprite(sprite);
                }
            }
            else
            {
                Debug.LogError($"Assets of type {asset.GetType()} are not supported; try to drop sprites.");
            }
        }

        private void AddSprite(Sprite sprite)
        {
            sprites.Add(sprite);
            AddSpriteImageToContainer(sprite);
        }

        private void ClearSprites()
        {
            sprites.Clear();
            spritesContainer.Clear();
        }

        private void GenerateRuleTileAsset()
        {
            var path =  EditorUtility.SaveFilePanelInProject("Save Rule Tile", "Rule Tile", "asset", "Message");
            if (string.IsNullOrEmpty(path)) return;

            var tile = CreateInstance<RuleTile>();

            for (int i = 0; i < PatternSize; i++)
            {
                var ruleOutput = tiles[i];

                var rule = pattern.m_TilingRules[i].Clone();
                rule.m_Sprites = (Sprite[])ruleOutput.m_Sprites.Clone();
                rule.m_GameObject = ruleOutput.m_GameObject;
                rule.m_ColliderType = ruleOutput.m_ColliderType;

                tile.m_TilingRules.Add(rule);
            }

            AssetDatabase.CreateAsset(tile, path);

            tileToEdit = tile;
            RefreshEditorMode();
        }

        private void SaveRuleTileAsset()
        {
            var targetRulesList = tileToEdit.m_TilingRules;
            for (int i = 0; i < PatternSize; i++)
            {
                var ruleOutput = tiles[i];
                RuleTile.TilingRule rule;

                if (targetRulesList.Count > i) 
                {
                    rule = targetRulesList[i];
                }
                else
                {
                    rule = pattern.m_TilingRules[i].Clone();
                    targetRulesList.Add(rule);
                }
                rule.m_NeighborPositions.Clear();
                rule.m_NeighborPositions.AddRange(pattern.m_TilingRules[i].m_NeighborPositions);
                rule.m_Neighbors.Clear();
                rule.m_Neighbors.AddRange(pattern.m_TilingRules[i].m_Neighbors);
                rule.m_Sprites = (Sprite[])ruleOutput.m_Sprites.Clone();
                rule.m_GameObject = ruleOutput.m_GameObject;
                rule.m_ColliderType = ruleOutput.m_ColliderType;
            }

            for (int i = PatternSize; i < targetRulesList.Count; i++)
            {
                targetRulesList.RemoveAt(i);
                i--;
            }
        }
    }
}