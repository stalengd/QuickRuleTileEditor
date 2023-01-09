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
        [SerializeField] private PatternedRuleTileEditModel model;
        [SerializeField] private TilesDisplayMode displayMode = TilesDisplayMode.Mixed;

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
            model = new PatternedRuleTileEditModel();
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
                    SetPattern(new RuleTilePattern(id, pattern));
                }
                else
                {
                    patternDropdown.value = model.PatternId;
                }
            });
        }

        private void SetPattern(RuleTilePattern pattern)
        {
            model.SetPattern(pattern);
            RefreshTilesContainer();
        }

        private void RefreshTilesContainer()
        {
            if (tilesContainer != null)
            {
                tilesContainer.Clear();
                for (int i = 0; i < model.TilesCount; i++)
                {
                    AddTileToContainer(i);
                }
                RefreshSelectedTile();
            }
        }

        private void Clear()
        {
            var pattern = model.Pattern;
            model = new PatternedRuleTileEditModel(pattern);

            ClearSprites();
            RefreshEditorMode();
            RefreshTilesContainer();
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
                model = new PatternedRuleTileEditModel(model.Pattern, tile);
                RefreshTilesContainer();

                var textures = new HashSet<Texture>();
                for (int i = 0; i < model.TilesCount; i++)
                {
                    var sprite = model.GetTileSprite(i);
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
            creationModeContainer.SetHidden(model.TileToEdit != null);
            editModeContainer.SetHidden(model.TileToEdit == null);

            if (model.TileToEdit != null) 
            {
                var infoLabel = editModeContainer.Query<Label>(className: "edit-info-label").First();
                infoLabel.text = $"Editing \"{model.TileToEdit.name}\"";
            }
        }

        private void SpriteClicked(Sprite sprite)
        {
            SetSpriteForTile(model.SelectedTile, sprite);
            model.SelectNextTile();
            RefreshSelectedTile();
        }

        private void SetSpriteForTile(int tileIndex, Sprite sprite)
        {
            var tileImage = tilesContainer
                .ElementAt(tileIndex)
                .ElementAt(0) as Image;
            tileImage.sprite = sprite;
            model.SetSpriteForTile(tileIndex, sprite);
        }

        private void SpriteMouseUp(MouseUpEvent e)
        {
            if (e.button == 1)
            {
                var image = e.target as Image;
                var index = image.parent.IndexOf(image);
                model.RemoveSpriteAt(index);
                image.RemoveFromHierarchy();
            }
        }

        private void TileClicked(int tileIndex)
        {
            model.SelectedTile = tileIndex;
            RefreshSelectedTile();
        }

        private void RefreshSelectedTile()
        {
            int i = 0;
            var selectedTile = model.SelectedTile;
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
            model.AddSprite(sprite);
            AddSpriteImageToContainer(sprite);
        }

        private void ClearSprites()
        {
            model.ClearSprites();
            spritesContainer.Clear();
        }

        private void GenerateRuleTileAsset()
        {
            model.GenerateRuleTileAsset();
            RefreshEditorMode();
        }

        private void SaveRuleTileAsset()
        {
            model.SaveRuleTileAsset();
        }
    }
}