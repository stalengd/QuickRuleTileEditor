using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;

namespace QuickRuleTileEditor
{
    public class QuickRuleTileWindow : EditorWindow
    {
        [SerializeField] private List<Sprite> sprites = new();
        [SerializeField] private int selectedTile = 0;
        [SerializeField] private Sprite[] tileSprites;
        [SerializeField] private RuleTile pattern;
        [SerializeField] private RuleTile tileToEdit;
        [SerializeField] private TilesDisplayMode displayMode = TilesDisplayMode.Mixed;

        private VisualElement root;
        private VisualElement spritesContainer;
        private VisualElement tilesContainer;
        private VisualElement creationModeContainer;
        private VisualElement editModeContainer;
        private StyleSheet styleSheet;
        private int tilesCount = 16;
        private System.Action<Object> objectPickerCallback;


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
            SelectPattern(Resources.Load<RuleTile>("Pattern-16"));
        }

        private void OnEnable()
        {
            styleSheet = Resources.Load<StyleSheet>("QuickRuleTileWindow");

            root = new VisualElement()
            {
                name = "root",
            };
            root.styleSheets.Add(styleSheet);
            root.AddToClassList("root");
            rootVisualElement.Add(root);

            root.Add(CreateMenu());

            var mainContainer = new VisualElement()
            {
                name = "main"
            };
            mainContainer.AddToClassList("main");
            root.Add(mainContainer);

            mainContainer.Add(CreateHeader());
            mainContainer.Add(CreateSpritesContainer());
            mainContainer.Add(CreateTilesContainer());
            mainContainer.Add(CreateCreationModeContainer());
            mainContainer.Add(CreateEditModeContainer());

            RefreshSelectedTile();
            SetDisplayMode(displayMode);
            RefreshEditorMode();
        }

        private void OnGUI()
        {
            if (Event.current.commandName == "ObjectSelectorClosed")
            {
                objectPickerCallback(EditorGUIUtility.GetObjectPickerObject());
            }
        }

        private void SelectPattern(RuleTile pattern)
        {
            this.pattern = pattern;
            selectedTile = 0;
            tileSprites = new Sprite[tilesCount];
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
            return pattern.m_TilingRules[index].m_Sprites[0];
        }

        private void Clear()
        {
            tileToEdit = null;
            ClearSprites();
            SelectPattern(pattern);
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
                    foreach (var sprite in GetSpritesFromTexture(texture))
                    {
                        AddSprite(sprite);
                    }
                }
                RefreshEditorMode();
            });
        }
        
        private void RefreshEditorMode()
        {
            SetHidden(creationModeContainer, tileToEdit != null);
            SetHidden(editModeContainer, tileToEdit == null);

            if (tileToEdit != null) 
            {
                var infoLabel = editModeContainer.Query<Label>(className: "edit-info-label").First();
                infoLabel.text = $"Editing \"{tileToEdit.name}\"";
            }
        }

        private void ShowObjectPicker<T>
            (Object obj, bool allowSceneObjects, string searchFilter, System.Action<Object> callback) where T : Object
        {
            objectPickerCallback = callback;
            var controlID = GUIUtility.GetControlID(FocusType.Passive);
            EditorGUIUtility.ShowObjectPicker<T>(obj, allowSceneObjects, searchFilter, controlID);
        }


        private VisualElement CreateMenu()
        {
            var menu = new Toolbar();
            menu.AddToClassList("menu");

            var newButton = new ToolbarButton(Clear)
            {
                text = "New"
            };
            newButton.AddToClassList("menu-button-new");
            menu.Add(newButton);

            var openButton = new ToolbarButton(OpenRuleTile)
            {
                text = "Open"
            };
            openButton.AddToClassList("menu-button-open");
            menu.Add(openButton);

            return menu;
        }

        private VisualElement CreateHeader()
        {
            var header = new VisualElement();
            header.AddToClassList("header");

            var clearSpritesButton = new Button(ClearSprites) { text = "Clear Sprites" };
            clearSpritesButton.AddToClassList("clear-sprites-button");
            header.Add(clearSpritesButton);

            var displayPattern = new Button(() => SetDisplayMode(TilesDisplayMode.Pattern)) 
            { 
                text = "P", 
                tooltip = "Tiles display mode: display only pattern" 
            };
            displayPattern.AddToClassList("display-pattern-button");
            displayPattern.AddToClassList("btn-merged-left");
            header.Add(displayPattern);

            var displayMixed = new Button(() => SetDisplayMode(TilesDisplayMode.Mixed))
            {
                text = "M",
                tooltip = "Tiles display mode: mixed (both pattern and sprite 50/50)"
            };
            displayMixed.AddToClassList("display-mixed-button");
            displayMixed.AddToClassList("btn-merged-mid");
            header.Add(displayMixed);

            var displaySprite = new Button(() => SetDisplayMode(TilesDisplayMode.Sprite))
            {
                text = "S",
                tooltip = "Tiles display mode: sprite (if selected)"
            };
            displaySprite.AddToClassList("display-sprite-button");
            displaySprite.AddToClassList("btn-merged-right");
            header.Add(displaySprite);

            return header;
        }

        private VisualElement CreateSpritesContainer()
        {
            spritesContainer = new ScrollView();
            spritesContainer.AddToClassList("sprites-container");

            foreach (var sprite in sprites)
            {
                AddSpriteImageToContainer(sprite);
            }

            spritesContainer.RegisterCallback<DragUpdatedEvent>((e) =>
            {
                var objects = DragAndDrop.objectReferences;

                var isAnySupported = false;
                foreach (var obj in objects)
                {
                    if (obj is Sprite || obj is Texture)
                    {
                        isAnySupported = true;
                        break;
                    }
                }

                if (isAnySupported)
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Link;
                }
                else
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
                }
            });

            spritesContainer.RegisterCallback<DragPerformEvent>((e) =>
            {
                var objects = DragAndDrop.objectReferences;

                foreach (var obj in objects)
                {
                    OnAssetDrop(obj);
                }
            });

            return spritesContainer;
        }

        private VisualElement CreateTilesContainer()
        {
            tilesContainer = new ScrollView();
            tilesContainer.AddToClassList("tiles-container");

            for (int i = 0; i < tilesCount; i++)
            {
                AddTileToContainer(i);
            }

            return tilesContainer;
        }

        private VisualElement CreateCreationModeContainer()
        {
            creationModeContainer = new VisualElement();
            creationModeContainer.AddToClassList("creation-mode-container");
            var generateButton = new Button(GenerateRuleTileAsset)
            {
                text = "Save as..."
            };
            creationModeContainer.Add(generateButton);

            SetHidden(creationModeContainer, true);
            return creationModeContainer;
        }

        private VisualElement CreateEditModeContainer()
        {
            editModeContainer = new VisualElement();
            editModeContainer.AddToClassList("edit-mode-container");

            var infoLabel = new Label();
            infoLabel.AddToClassList("edit-info-label");
            editModeContainer.Add(infoLabel);

            var saveButton = new Button(SaveRuleTileAsset)
            {
                text = "Save"
            };
            editModeContainer.Add(saveButton);
            
            var saveAsButton = new Button(GenerateRuleTileAsset)
            {
                text = "Save as..."
            };
            editModeContainer.Add(saveAsButton);

            SetHidden(editModeContainer, true);
            return editModeContainer;
        }

        private void SetHidden(VisualElement element, bool isHidden)
        {
            element.EnableInClassList("hidden", isHidden);
        }


        private void AddSpriteImageToContainer(Sprite sprite)
        {
            var image = new Image
            {
                sprite = sprite
            };
            image.AddToClassList("sprite");
            image.RegisterCallback<ClickEvent>(e => SpriteClicked(sprite));
            image.RegisterCallback<MouseUpEvent>(SpriteMouseUp);

            spritesContainer.Add(image);
        }

        private void AddTileToContainer(int tileIndex)
        {
            var image = new Image
            {
                sprite = GetPatternSprite(tileIndex)
            };
            image.AddToClassList("tile");
            image.RegisterCallback<ClickEvent>(e => TileClicked(tileIndex));

            var selectedSpriteImage = new Image
            {
                sprite = tileSprites[tileIndex]
            };
            selectedSpriteImage.AddToClassList("tile-sprite");
            image.Add(selectedSpriteImage);

            tilesContainer.Add(image);
        }


        private void SpriteClicked(Sprite sprite)
        {
            SetSpriteForTile(selectedTile, sprite);
            selectedTile = (selectedTile + 1) % tilesCount;
            RefreshSelectedTile();
        }

        private void SetSpriteForTile(int tileIndex, Sprite sprite)
        {
            var tileImage = tilesContainer
                .ElementAt(tileIndex)
                .ElementAt(0) as Image;
            tileImage.sprite = sprite;
            tileSprites[tileIndex] = sprite;
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
                foreach (var sprite in GetSpritesFromTexture(asset))
                {
                    AddSprite(sprite);
                }
            }
            else
            {
                Debug.LogError($"Assets of type {asset.GetType()} are not supported; try to drop sprites.");
            }
        }

        private IEnumerable<Sprite> GetSpritesFromTexture(Object texture)
        {
            var path = AssetDatabase.GetAssetPath(texture);
            var subAssets = AssetDatabase.LoadAllAssetsAtPath(path);
            return subAssets.OfType<Sprite>();
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

            for (int i = 0; i < tilesCount; i++)
            {
                var sprite = tileSprites[i];

                var rule = pattern.m_TilingRules[i].Clone();
                rule.m_Sprites[0] = sprite;

                tile.m_TilingRules.Add(rule);
            }

            AssetDatabase.CreateAsset(tile, path);

            tileToEdit = tile;
            RefreshEditorMode();
        }

        private void SaveRuleTileAsset()
        {
            var targetRulesList = tileToEdit.m_TilingRules;
            for (int i = 0; i < tilesCount; i++)
            {
                var sprite = tileSprites[i];
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
                rule.m_Sprites[0] = sprite;
            }
        }
    }
}