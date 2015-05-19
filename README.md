# Historian

![KSEA Logo](http://i.imgur.com/L7rpnwm.png)

Historian is a small utility for Kerbal Space Program to automatically add useful and aesthetically pleasing information to screenshots in a highly configurable manner.

![Sample](http://i.imgur.com/QBYlcd9l.png)

# Configuration

You can configure Historian on the fly by editting the settings file. While the game is running, use the Historian button in the Application Launcher to open the settings menu.

* __Suppressed__: When suppressed, Historian will not display the overlay when taking screenshots.
* __Always Active__: If this is turned on, the overlay will always show on top of the game. This is useful when editting the layout.
* __Reload__: Click this button to reload the layout configuration while the game is running.
* __Close__: Closes the settings window.

To modify the layout, open `<KSP Root>/GameData/KSEA/Historian/Historian.cfg`. There are various type of _Element_ nodes that you can add, remove, or modify to customize the layout to your liking.

The following element types are currently supposed by Historian:

* `RECTANGLE` Draws a simple 2D rectangle with a solid color on the screen.
* `TEXT` Draws some text on the screen. Supports rich text and value placeholders.
* `PICTURE` Renders a 2D image onto the screen.
* `FLAG` Renders the current vessel's flag onto the screen.

Each element has the following common properties:

* `Anchor` (2D Vector) Anchor of the element, relative to itself.
* `Position` (2D Vector) Anchored position of the element, relative to the screen.
* `Size` (2D Vector) Width and height of the element, relative to the screen.
* `Color` (Color) Primary color of the element.

Note that all properties are expressed in relative percentage values. For example, `Anchor = 0.5,0.5` means the center of the element, `Position = 0.5,0.0` means top center of the screen, and `Size = 1.0,1.0` means the entire size of the screen.

In addition to these properties, each element may have a few additional properties:

### Text

* `Value` (String) Value of the text that is to be displayed. Supports rich text and placeholder values.
* `Alignment` (Enum) Alignment of the text relative to the size of the element. Supports any one of these values: `UpperLeft`, `UpperCenter`, `UpperRight`, `MiddleLeft`,`MiddleCenter`, `MiddleRight`, `LowerLeft`, `LowerCenter`, and `LowerRight`.
* `Height` (Integer) Font height (size), in pixels
* `Style` (Enum) Style of the font. Supports any one of these values: `Normal`, `Bold`, `Italic`, and `BoldAndItalic`.

When setting the `Value` property, you can use Rich Text formatting. You can refer to [Unity's Manual](http://docs.unity3d.com/Manual/StyledText.html) for details.

You can also use value placeholders. Currently, the following placeholders are supported:

* `<N>` New line.
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
* `<Altitude>` Altitude of the active vessel relative to the sea level of the main body (Only available in Flight Mode)

Note that all placeholder values are case-sensitive.

### Picture

* `Path` (String) Path to the image file, relative to the `GameData` directory. Example: `Squad/Flags/default`
* `Scale` (2D Vector) Scale of the image relative to itself.
* `Size` (2D Vector) If a size parameter is not provided, the size of the image is used automatically. Otherwise it denotes  the size of the image relative to screen dimensions.

### Flag

* `Default` (String) Path to a default image file to show if no flag can be determined for the active vessel, or if there is no active vessel. Example: `Squad/Flags/default`
* `Scale` (2D Vector) Scale of the image.
* `Size` (2D Vector) If a size parameter is not provided, the size of the image is used automatically. Otherwise it denotes the size of the image relative to screen dimensions.

## Sample Configuration

Below, you can find the default Historian configuration file as an example on how to set up a layout.

    KSEA_HISTORIAN_CONFIGURATION
    {
        Version = 0.1
    	
        LAYOUT
        {
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
                Default = Squad/Flags/default
            }
            
            TEXT
            {
                Anchor = 0.0,0.5
                Position = 0.25,0.85
                Size = 0.5,0.1
                Color = 1.0,1.0,1.0,1.0
                Value = <size=22><b><Vessel></b></size><N><size=8><N></size><b><UT></b> (<T+>)<N><size=12><Situation> @ <Body> (<Biome>, <Latitude>° <Longitude>°) </size>
                Alignment = MiddleLeft
                Height = 12
                Style = Normal
            }
        }
    }
