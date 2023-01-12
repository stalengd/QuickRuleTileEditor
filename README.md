# Quick Rule Tile

[![openupm](https://img.shields.io/npm/v/com.stalengd.quickruletileeditor?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.stalengd.quickruletileeditor/)

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

Package requires Unity 2021.1+ and [2D Tilemap Extras package](https://docs.unity3d.com/Manual/com.unity.2d.tilemap.extras.html) (in which the rule tile itself). 

Unity 2021 is required because tool is developed based on UI Toolkit, older versions do not have some built-in components. However, you can try to work around this in Unity 2020.3 by manually installing the `com.unity.ui` preview package (not tested).

Tested on Unity 2021.3.9f1


## How to use

1. Navigate to `Window/Quick Rule Tile`
2. Select pattern in dropdown list under "New" and "Open" buttons.
3. In that window drag and drop sprites or texture (should be sliced into sprites) to the top empty area (sprites list).
4. Pay attention to the current tile in the bottom area (tiles list) and click on the corresponding sprite, then next and next, current tile will move as you selecting sprites. You can also change selected tile by clicking on them and re-select sprite for it.
5. As soon as all tiles are assigned, press the "Save as..." button to save asset.

Later you will be able to edit this rule tile by clicking "Open" button on the top toolbar.


## Installation

### Add from [OpenUPM](https://openupm.com/packages/com.stalengd.quickruletileeditor) *<sub>(Recommended)</sub>*

**CLI**

```
openupm add com.stalengd.quickruletileeditor
```

<details>
<summary><b>Manual installation</b></summary>

- open `Edit/Project Settings/Package Manager`
- add a new Scoped Registry (or edit the existing OpenUPM entry):
  ```
  Name: package.openupm.com
  URL:  https://package.openupm.com/
  Scope(s): com.stalengd.quickruletileeditor
  ```
- click <kbd>Save</kbd> (or <kbd>Apply</kbd>)
- open `Window/Package Manager`
- click <kbd>+</kbd>
- select <kbd>Add package by name...</kbd> or <kbd>Add from Git URL...</kbd>
- paste `com.stalengd.quickruletileeditor` into name
- paste `0.3.0` into version
- click <kbd>Add</kbd>
</details>



### Add from Git URL

- open `Window/Package Manager`
- click <kbd>+</kbd>
- select <kbd>Add from Git URL...</kbd>
- paste `https://github.com/stalengd/QuickRuleTileEditor.git`
- click <kbd>Add</kbd>
