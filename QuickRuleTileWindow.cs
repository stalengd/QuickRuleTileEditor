using System.Collections;
using System.Collections.Generic;
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
        [SerializeField] private TilesDisplayMode displayMode = TilesDisplayMode.Mixed;

        private VisualElement root;
        private VisualElement spritesContainer;
        private VisualElement tilesContainer;
        private StyleSheet styleSheet;
        private int tilesCount = 16;


        private enum TilesDisplayMode
        {
            Pattern,
            Mixed,
            Sprite
        }


        [MenuItem("Window/Quick Rule Tile")]
        public static void Open()
        {
            var window = GetWindow<QuickRuleTileWindow>("Quick Rule Tile");
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
                name = "Root",
            };
            root.styleSheets.Add(styleSheet);
            root.AddToClassList("root");
            rootVisualElement.Add(root);


            root.Add(CreateHeader());
            root.Add(CreateSpritesContainer());
            root.Add(CreateTilesContainer());

            var generateButton = new Button(GenerateRuleTileAsset)
            {
                text = "Generate"
            };
            root.Add(generateButton);

            RefreshSelectedTile();
            SetDisplayMode(displayMode);
        }

        private void SelectPattern(RuleTile pattern)
        {
            this.pattern = pattern;
            tileSprites = new Sprite[tilesCount];
        }

        private Sprite GetPatternSprite(int index)
        {
            return pattern.m_TilingRules[index].m_Sprites[0];
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
            spritesContainer = new VisualElement();
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
            tilesContainer = new VisualElement();
            tilesContainer.AddToClassList("tiles-container");

            for (int i = 0; i < tilesCount; i++)
            {
                AddTileToContainer(i);
            }

            return tilesContainer;
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
            var tileImage = tilesContainer
                .ElementAt(selectedTile)
                .ElementAt(0) as Image;
            tileImage.sprite = sprite;
            tileSprites[selectedTile] = sprite;

            selectedTile = (selectedTile + 1) % tilesCount;
            RefreshSelectedTile();
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
                var path = AssetDatabase.GetAssetPath(asset);
                var subAssets = AssetDatabase.LoadAllAssetsAtPath(path);

                foreach (var subAsset in subAssets)
                {
                    if (subAsset is Sprite)
                    {
                        AddSprite((Sprite)subAsset);
                    }
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
            var path = 
                EditorUtility.SaveFilePanelInProject("Save Rule Tile", "Rule Tile", "asset", "Message");

            var tile = CreateInstance<RuleTile>();

            for (int i = 0; i < tilesCount; i++)
            {
                var sprite = tileSprites[i];

                var rule = pattern.m_TilingRules[i].Clone();
                rule.m_Sprites[0] = sprite;

                tile.m_TilingRules.Add(rule);
            }

            AssetDatabase.CreateAsset(tile, path);
        }
    }
}