using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;

namespace QuickRuleTileEditor
{
    // Constains UI setup
    public partial class QuickRuleTileWindow
    {
        private VisualElement root;
        private VisualElement spritesContainer;
        private VisualElement tilesContainer;
        private VisualElement creationModeContainer;
        private VisualElement editModeContainer;
        private DropdownField patternDropdown;


        private VisualElement CreateRoot()
        {
            root = new VisualElement()
            {
                name = "root",
            };
            root.styleSheets.Add(styleSheet);
            root.AddToClassList("root");

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

            return root;
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

            System.Func<string, string> nameFormatter = patternsProvider.GetPatternName;
            var patternsList = patternsProvider.GetAllPatternsIds();
            patternDropdown = new DropdownField(patternsList, patternId, nameFormatter, nameFormatter);
            patternDropdown.RegisterValueChangedCallback(e => SelectPattern(e.newValue));
            patternDropdown.AddToClassList("pattern-dropdown");
            header.Add(patternDropdown);

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

            var clearSpritesButton = new Button(ClearSprites) { text = "Clear Sprites" };
            clearSpritesButton.AddToClassList("clear-sprites-button");
            header.Add(clearSpritesButton);

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

            for (int i = 0; i < TilesCount; i++)
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

            creationModeContainer.SetHidden(true);
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

            editModeContainer.SetHidden(true);
            return editModeContainer;
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
            var isExcess = tileIndex >= PatternSize;

            var image = new Image
            {
                sprite = GetPatternSprite(tileIndex)
            };
            image.AddToClassList("tile");
            image.EnableInClassList("tile-excess", isExcess);
            image.RegisterCallback<ClickEvent>(e => TileClicked(tileIndex));

            if (isExcess)
            {
                image.tooltip =
                    "This tile index is over size of the selected pattern and will not be included in the saved asset.";
            }

            var selectedSpriteImage = new Image
            {
                sprite = tileSprites[tileIndex]
            };
            selectedSpriteImage.AddToClassList("tile-sprite");
            image.Add(selectedSpriteImage);

            tilesContainer.Add(image);
        }
    }
}
