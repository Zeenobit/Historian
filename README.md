# Historian

![KSEA Logo](http://i.imgur.com/L7rpnwm.png)

Historian is a screenshot utility mod for Kerbal Space Program that adds fully configurable and dynamic captions and overlay graphics to screenshots to better describe the context of screenshots and record your Kerbal adventures.

## Installation

1. Download the latest version of Historian from either [Kerbal Stuff](https://kerbalstuff.com/mod/821/Historian) or [GitHub](https://github.com/Zeenobit/Historian/releases).
2. Extract the archive into your KSP installation folder, and overwrite all existing files.
3. Enjoy!

## Configuration

You can open the Historian configuration window by using either the stock application launcher, or [Blizzy's Toolbar](http://forum.kerbalspaceprogram.com/threads/60863).

* __Suppressed__: When suppressed, Historian will not display the overlay when taking screenshots.
* __Always Active__: If this is turned on, the overlay will always show on top of the game. This is useful when editting layouts.
* __Load__: Reloads all layouts while the game is running.
* __Save__: Saves the current layout as the default layout (selected automatically everytime you launch KSP).

Press the configuration window again to close the configuration window.
Note that the configuration window shows up even if you have GUI disabled using the 'F2' key. This is intentional to allow layout editting while the game GUI is off.

### Layouts

All Historian layout files must be located inside `<KSP Root>/GameData/KSEA/Historian/Layouts` folder, and must have a `*.layout` extension to be recognized by Historian. Even though the files have a `*.layout` extension, they follow the same syntax as KSP's `*.cfg` files. This is to prevent the game from loading them into the database by default. You can edit these files using your favorite text editor.

The following documentations assumes that you have a basic knowledge about KSP's configuration file syntax. To create a new layout, simply create a new empty text file and call it `<Layout Name>.layout`. Make sure the file has a `*.layout` extension. To modify a layout, simply open it in any text editor.

All layouts have a root node defined by `KSEA_HISTORIAN_LAYOUT`. Inside this node, you can define your elements. Elements are the basic building blocks of a layout. Each layout has a number of elements, rendered in the order defined (this is useful to remember if you plan on layering the elements). Each element is defined by a configuration node and has a few properties that determine the element's behaviour.

#### Elements

The following element types are currently supported by Historian:

* `RECTANGLE` Draws a simple 2D rectangle with a solid color on the screen.
* `TEXT` Draws some text on the screen. Supports rich text and value placeholders.
* `PICTURE` Renders a 2D image onto the screen.
* `FLAG` Renders the current mission's flag onto the screen.
* `SITUATION_TEXT` Selects a different text to display based on flight situation and renders it on the screen much like `TEXT`.

##### Common Properties

Each element has the following common properties:

* `Anchor` Anchor of the element, relative to itself, expressed as a 2D vector. _Default: 0.0,0.0_
* `Position` Anchored position of the element, relative to the screen, expressed as a 2D vector. _Default: 0.0,0.0_
* `Size` Width and height of the element, relative to the screen, expressed as a 2D vector. _Default: 0.0,0.0_

Note that all properties are expressed in relative percentage values. For example, `Anchor = 0.5,0.5` means the center of the element, `Position = 0.5,0.0` means top center of the screen, and `Size = 1.0,1.0` means the entire size of the screen.

In addition to these properties, each element may have a few additional properties:

##### Rectangle

A `RECTANGLE` fills a rectangular area of the screen with a solid color. You can use semi-transparent colors.

* `Color` The color of the rectangular area, expressed in RGBA values. _Default: 0.0,0.0,0.0,1.0_

##### Text

A `TEXT` element renders a string of text.

* `Text` Value of the text that is to be displayed. Supports rich text and placeholder values. _Default: Empty_
* `TextAnchor` Alignment of the text relative to the bounds of the element. Supports any one of these values: `UpperLeft`, `UpperCenter`, `UpperRight`, `MiddleLeft`,`MiddleCenter`, `MiddleRight`, `LowerLeft`, `LowerCenter`, and `LowerRight`. _Default: MiddleCenter_
* `FontSize` Size of the font. Note that rich text format specifiers can override this. _Default: 10_
* `FontStyle` Style of the font. Supports any one of these values: `Normal`, `Bold`, `Italic`, and `BoldAndItalic`. Note that rich text format specifiers can override this. _Default: Normal_
* `Color` Color of the font. Note that rich text format specifiers can override this. _Default: 1.0,1.0,1.0,1.0_

Refer to [Unity's Manual](http://docs.unity3d.com/Manual/StyledText.html) for details on Rich Text formatting syntax.

The following pre-defined placeholder values can be used inside a text element. These placeholders will be replaced with their corresponding values when a screenshot is taken.

* `<N>` Inserts a new line.
* `<UT>` KSP Universal Time. Example: _Y12, D29, 2:02:12_
* `<Year>` Current year in Kerbal time
* `<Day>` Current day in Kerbal time
* `<Hour>` Current hour in Kerbal time
* `<Minute>` Current minute in Kerbal time
* `<Second>` Current second in Kerbal time
* `<T+>` Current mission time for the active vessel, in Kerbal time (Only available in Flight Mode). Example: _T+ 2y, 23d, 02:04:12_
* `<Vessel>` Name of the active vessel or Kerbal (Only available in Flight Mode). Example: _Jebediah Kerman_, _Kerbal X_
* `<Body>` Name of the main body (Only available in Flight Mode). Example: _Kerbin_
* `<Situation>` Current situation of the active vessel (Only available in Flight Mode). Example: _Flying_, _Orbiting_
* `<Biome>` Current biome of the active vessel based on its location (Only available in Flight Mode). Example: _Shores_
* `<Latitude>` Latitude of the active vessel relative to the main body (Only available in Flight Mode)
* `<Longitude>` Longitude of the active vessel relative to the main body (Only available in Flight Mode)
* `<Altitude>` Altitude of the active vessel relative to the sea level of the main body in the most appropriate unit (Only available in Flight Mode). The unit is also included as of version 1.0.1.
* `<Mach>` The current Mach number of the active vessel (Only available in Flight Mode).
* `<LandingZone>` The name of the current location the vessel is landed at (Only available in Flight Mode). Example: _Launchpad_
* `<Speed>` Surface speed of the active vessel in the most appropriate unit (Only available in Flight Mode). The unit is also included as of version 1.0.1.
* `<Custom>` The current value of the Custom Text. You can set this value using the configuration window. If custom text is not persistent (default), it will be cleared after the next screenshot.

Note that all placeholder values are case-sensitive.

##### Situation Text

A `SITUATION_TEXT` element behaves similar to the `TEXT` element. It has all of its properties except `Text`. Instead, it has the following additional properties, each corresponding to a different flight situation:

* `Default` Used when no flight situation is available.
* `Landed` Used when the vessel is landed.
* `Splashed` Used when the vessel is splashed in water.
* `Prelaunch` Used when the vessel is on the launchpad.
* `Flying` Used when the vessel is flying in atmosphere.
* `SubOrbital` Used when the vessel is in a sub-orbital trajectory.
* `Orbiting` Used when the vessel is orbiting a body.
* `Escaping` Used when the vessel is escaping from a body.
* `Docked` Used when the vessel is docked to another.

When a screen shot is taken, the `SITUATION_TEXT` element uses only one of the above values for its text, depending on the situation. This is useful for making more descriptive captions such as: `Landed on <Body>'s <LandingZone>` or `Flying at Mach <Mach> (<Speed>) <Altitude> over <Body>'s <Biome>`.

Note that just like `TEXT`, `SITUATION_TEXT` also supports rich text and placeholder values.

##### Picture

A `PICTURE` element renders a static 2D texture onto the screen. To save KSP memory usage, you can use any of the pre-loaded textures, such as missions flags, agency flags, or even part textures. You can also use any other custom texture as long as the path to your image is valid. Vintage border effects, anyone? ;)

* `Texture` Path to the image file, relative to the `GameData` directory. Example: `Squad/Flags/default`
* `Scale` Scale of the image relative to itself. For example, a value of `2.0,2.0` doubles the size of the texture, while maintaining the aspect ratio. _Default: 1.0,1.0_

If a `Size` property is not defined (or if the size is a zero vector), the size of the image is used automatically. Otherwise it denotes  the size of the image relative to screen dimensions. For example, a value of `1.0,1.0` ensures the image takes up the size of the entire screen. _Default: 0.0,0.0_

##### Flag

A `FLAG` element can be used to render the current mission's flag onto the screen. It behaves very much like a `PICTURE` otherwise.

* `DefaultTexture` Path to a default image file to show if no flag can be determined for the active vessel, or if there is no active vessel. Example: `Squad/Flags/default`
* `Scale` Scale of the image relative to itself. For example, a value of `2.0,2.0` doubles the size of the texture, while maintaining the aspect ratio. _Default: 1.0,1.0_

If a `Size` property is not defined (or if the size is a zero vector), the size of the image is used automatically. Otherwise it denotes  the size of the image relative to screen dimensions. For example, a value of `1.0,1.0` ensures the image takes up the size of the entire screen. _Default: 0.0,0.0_

### Sample Configuration

Below, you can find an example of a default layout:

    KSEA_HISTORIAN_LAYOUT
    {
        Name = Default
        
        RECTANGLE
        {
            Anchor = 0.0,0.5
            Size = 1.0,0.125
            Position = 0.0,0.85
            Color = 0.0,0.0,0.0,0.5
        }
    
        FLAG
        {
            Anchor = 0.5,0.5
            Position = 0.1,0.85
            Scale = 1,1
            DefaultTexture = Squad/Flags/default
        }
    
        TEXT
        {
            Anchor = 0.0,0.5
            Position = 0.25,0.85
            Size = 0.5,0.1
            Color = 1.0,1.0,1.0,1.0
            Text = <size=22><b><Vessel></b></size><N><size=8><N></size><b><UT></b> (<T+>)<N><size=12><Situation> @ <Body> (<Biome>, <Latitude>° <Longitude>°) </size>
            TextAnchor = MiddleLeft
            FontSize = 12
            FontStyle = Normal
        }
    }

This layout would produce screenshots that looks like this:

![](http://i.imgur.com/nqsvA09l.png)
