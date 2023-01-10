# Quick Rule Tile

An Unity editor window for creating and then editing rule tiles (aka auto tiles).

![Quick Rule Tile min](https://user-images.githubusercontent.com/33173619/211613196-5b1c6b38-571d-4d53-8c92-91bd321e3452.gif)

*<sub>Example uses assets by [Kenney](https://kenney.nl/assets/)</sub>*


## Features

- Create and edit rule tiles in custom handy window
- Support for often used 16, 15 and 47 tiles pattern
- Support for any custom patterns (just other rule tiles)
- Support for editing rule's game object and collider type (with "Apply for all" button)

Planned features:
- Animations/sprite variants editing support (but now editor preserves unsupported properties like output type as is, you can change them directly)
- Grid-ish pattern view (instead of just list)


## Requirements

Package depends on Unity UI Toolkit (for editor) and 2D Tilemap Extras package (in which the rule tile itself). 

If you start 2D project on Unity 2021+, these packages should be included automatically.

Tested on Unity 2021.3.9f1


## How to use

1. Navigate to `Window/Quick Rule Tile`
2. Select pattern in dropdown list under "New" and "Open" buttons.
3. In that window drag and drop sprites or texture (should be sliced into sprites) to the top empty area (sprites list).
4. Pay attention to the current tile in the bottom area (tiles list) and click on the corresponding sprite, then next and next, current tile will move as you selecting sprites. You can also change selected tile by clicking on them and re-select sprite for it.
5. As soon as all tiles are assigned, press the "Save as..." button to save asset.

Later you will be able to edit this rule tile by clicking "Open" button on the top toolbar.