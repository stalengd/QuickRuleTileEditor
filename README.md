# Quick Rule Tile

An Unity editor window for creating and then editing rule tiles (aka auto tiles).

**!!! Package in development !!!** 

At the moment, the minimum number of features has been implemented, you can only use pattern of 16 tiles to create and edit simple rule tiles.

Some planned features:
- Editing existing rule tiles support (partially implemented)
- Animations/sprite variants support
- Different patterns support, including custom ones


## Requirements

Package depends on Unity UI Toolkit (for editor) and Unity Tiles package. If you start 2D project on Unity 2021+, these packages should be included automatically.

Tested on Unity 2021.3.9f1


## How to use

1. Navigate to `Window/Quick Rule Tile`
2. In that window drag and drop sprites or texture (should be sliced into sprites) to the top empty area (sprites list).
3. Pay attention to the current tile in the bottom area (tiles list) and click on the corresponding sprite, then next and next, current tile will move as you selecting sprites. You can also change selected tile by clicking on them and re-select sprite for it.
4. As soon as all tiles are assigned, press the "Save as..." button to save asset.

Later you will be able to edit this rule tile by clicking "Open" button on the top toolbar.