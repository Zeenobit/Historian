using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace KSEA
{
    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    public class Historian : MonoBehaviour
    {
        public string ScreenshotDirectory
        {
            get
            {
                
                return Path.Combine(KSPUtil.ApplicationRootPath, "Screenshots");
            }
        }

        public string PluginDirectory
        {
            get
            {
                var path = Assembly.GetExecutingAssembly().Location;
                return Path.GetDirectoryName(path);
            }
        }

        #region Elements
        public class Element
        {
            public Vector2 Anchor { get; set; }
            public Vector2 Position { get; set; }
            public Vector2 Size { get; set; }
            public Color Color { get; set; }

            public Rect Bounds
            {
                get
                {
                    var width = Size.x * Screen.width;
                    var height = Size.y * Screen.height;

                    var left = (Position.x * Screen.width) - (Anchor.x * width);
                    var top = (Position.y * Screen.height) - (Anchor.y * height);

                    return new Rect(left, top, width, height);
                }
            }

            public static Element Create(ConfigNode node)
            {
                Element element = null;

                switch (node.name)
                {
                case "RECTANGLE":

                    element = new Rectangle();
                    break;

                case "TEXT":

                    element = new Text();
                    break;

                case "PICTURE":

                    element = new Picture();
                    break;

                case "FLAG":

                    element = new Flag();
                    break;

                default:

                    Print("Unable to load unknown element configuration '{0}'.", node.name);
                    break;
                }

                if (element != null)
                {
                    element.Load(node);
                }

                return element;
            }

            public Element()
            {
                Anchor = Vector2.zero;
                Position = Vector2.zero;
                Size = Vector2.zero;
                Color = Color.white;
            }

            public void Load(ConfigNode node)
            {
                OnLoad(node);
            }

            public void Draw()
            {
                OnDraw();
            }

            protected virtual void OnLoad(ConfigNode node)
            {
                if (node.HasValue("Anchor"))
                {
                    Anchor = ConfigNode.ParseVector2(node.GetValue("Anchor"));
                }

                if (node.HasValue("Position"))
                {
                    Position = ConfigNode.ParseVector2(node.GetValue("Position"));
                }

                if (node.HasValue("Size"))
                {
                    Size = ConfigNode.ParseVector2(node.GetValue("Size"));
                }

                if (node.HasValue("Color"))
                {
                    Color = ConfigNode.ParseColor(node.GetValue("Color"));
                }
            }

            protected virtual void OnDraw()
            {
            }
        }

        public class Rectangle : Element
        {
            public Texture2D Texture { get; set; }

            public Rectangle()
            {
                Texture = null;
            }

            protected override void OnLoad(ConfigNode node)
            {
                base.OnLoad(node);

                var bounds = Bounds;

                var width = (int) bounds.width;
                var height = (int) bounds.height;

                Texture = new Texture2D(width, height);

                var pixels = Texture.GetPixels();

                for (var i = 0; i < pixels.Length; ++i)
                {
                    pixels[i] = Color;
                }

                Texture.SetPixels(pixels);
                Texture.Apply();
            }

            protected override void OnDraw()
            {
                base.OnDraw();

                var bounds = Bounds;

                GUI.DrawTexture(bounds, Texture);
            }
        }

        public class Text : Element
        {
            public string Value { get; set; }
            public TextAnchor Alignment { get; set; }
            public int Height { get; set; }
            public FontStyle Style { get; set; }

            public Text()
            {
                Value = "";
                Alignment = TextAnchor.MiddleCenter;
                Height = 12;
                Style = FontStyle.Normal;
            }

            protected override void OnLoad(ConfigNode node)
            {
                base.OnLoad(node);

                if (node.HasValue("Value"))
                {
                    Value = node.GetValue("Value");
                }

                if (node.HasValue("Alignment"))
                {
                    Alignment = (TextAnchor) ConfigNode.ParseEnum(typeof(TextAnchor), node.GetValue("Alignment"));
                }

                if (node.HasValue("Height"))
                {
                    Height = int.Parse(node.GetValue("Height"));
                }

                if (node.HasValue("Style"))
                {
                    Style = (FontStyle) ConfigNode.ParseEnum(typeof(FontStyle), node.GetValue("Style"));
                }
            }

            protected override void OnDraw()
            {
                base.OnDraw();

                var style = new GUIStyle(GUI.skin.label);

                style.alignment = Alignment;
                style.normal.textColor = Color;
                style.fontSize = Height;
                style.fontStyle = Style;
                style.richText = true;                

                var content = new GUIContent();
                content.text = Parse(Value);

                GUI.Label(Bounds, content, style);
            }

            private static string Parse(string text)
            {
                var ut = Planetarium.GetUniversalTime();
                var time = KSPUtil.GetKerbinDateFromUT((int) ut);
                var vessel = FlightGlobals.ActiveVessel;

                if (text.Contains("<N>"))
                {
                    text = text.Replace("<N>", Environment.NewLine);
                }

                if (text.Contains("<UT>"))
                {
                    var value = string.Format("Y{0}, D{1:D2}, {2}:{3:D2}:{4:D2}", time[4] + 1, time[3] + 1, time[2], time[1], time[0]);

                    text = text.Replace("<UT>", value);
                }

                if (text.Contains("<Year>"))
                {
                    text = text.Replace("<Year>", time[4].ToString());
                }

                if (text.Contains("<Day>"))
                {
                    text = text.Replace("<Day>", time[3].ToString());
                }

                if (text.Contains("<Hour>"))
                {
                    text = text.Replace("<Hour>", time[2].ToString());
                }

                if (text.Contains("<Minute>"))
                {
                    text = text.Replace("<Minute>", time[1].ToString());
                }

                if (text.Contains("<Second>"))
                {
                    text = text.Replace("<Second>", time[0].ToString());
                }

                if (text.Contains("<T+>"))
                {
                    var value = "";

                    if (vessel != null)
                    {
                        var t = KSPUtil.GetKerbinDateFromUT((int) vessel.missionTime);

                        if (t[4] > 0)
                        {
                            value = string.Format("T+ {0}y, {1}d, {2:D2}:{3:D2}:{4:D2}", t[4] + 1, t[3] + 1, t[2], t[1], t[0]);
                        }
                        else if (t[3] > 0)
                        {
                            value = string.Format("T+ {1}d, {2:D2}:{3:D2}:{4:D2}", t[4] + 1, t[3] + 1, t[2], t[1], t[0]);
                        }
                        else
                        {
                            value = string.Format("T+ {2:D2}:{3:D2}:{4:D2}", t[4] + 1, t[3] + 1, t[2], t[1], t[0]);
                        }
                    }

                    text = text.Replace("<T+>", value);
                }

                if (text.Contains("<Vessel>"))
                {
                    var value = "";

                    if (vessel != null)
                    {
                        value = vessel.vesselName;
                    }

                    text = text.Replace("<Vessel>", value);
                }

                if (text.Contains("<Body>"))
                {
                    var value = "";

                    if (vessel != null)
                    {
                        value = Planetarium.fetch.CurrentMainBody.bodyName;
                    }

                    text = text.Replace("<Body>", value);
                }

                if (text.Contains("<Situation>"))
                {
                    var value = "";

                    if (vessel != null)
                    {
                        switch (vessel.situation)
                        {
                        case Vessel.Situations.SUB_ORBITAL:
                            value = "Sub-orbital";
                            break;

                        default:
                            value = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(vessel.situation.ToString().ToLower());
                            break;
                        }
                    }

                    text = text.Replace("<Situation>", value);
                }

                if (text.Contains("<Biome>"))
                {
                    var value = "";

                    if (vessel != null)
                    {
                        var biome = ScienceUtil.GetExperimentBiome(FlightGlobals.ActiveVessel.mainBody, FlightGlobals.ActiveVessel.latitude, FlightGlobals.ActiveVessel.longitude);
                        value = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(biome.ToLower());
                    }

                    text = text.Replace("<Biome>", value);
                }

                if (text.Contains("<Latitude>"))
                {
                    var value = "";

                    if (vessel != null)
                    {
                        value = FlightGlobals.ActiveVessel.latitude.ToString("F5");
                    }

                    text = text.Replace("<Latitude>", value);
                }

                if (text.Contains("<Longitude>"))
                {
                    var value = "";

                    if (vessel != null)
                    {
                        value = FlightGlobals.ActiveVessel.longitude.ToString("F5");
                    }

                    text = text.Replace("<Longitude>", value);
                }

                if (text.Contains("<Altitude>"))
                {
                    var value = "";

                    if (vessel != null)
                    {
                        value = FlightGlobals.ActiveVessel.altitude.ToString("F5");
                    }

                    text = text.Replace("<Altitude>", value);
                }

                return text;
            }
        }

        public class Picture : Element
        {
            public string Path { get; set; }
            public Vector2 Scale { get; set; }
            public Texture2D Texture { get; set; }

            public Picture()
            {
                Path = "";
                Scale = Vector2.one;
                Texture = null;
            }

            protected override void OnLoad(ConfigNode node)
            {
                base.OnLoad(node);

                if (node.HasValue("Path"))
                {
                    Path = node.GetValue("Path");
                }

                if (node.HasValue("Scale"))
                {
                    Scale = ConfigNode.ParseVector2(node.GetValue("Scale"));
                }

                Texture = GameDatabase.Instance.GetTexture(Path, false);

                if (Size == Vector2.zero)
                {
                    Size = new Vector2((float) Texture.width / Screen.width * Scale.x, (float) Texture.height / Screen.height * Scale.y);
                }
            }

            protected override void OnDraw()
            {
                base.OnDraw();

                GUI.DrawTexture(Bounds, Texture);
            }
        }

        public class Flag : Element
        {
            public Vector2 Scale { get; set; }
            public string Default { get; set; }
            public Texture2D DefaultTexture { get; set; }
            public Texture2D Texture { get; set; }

            public Flag()
            {
                Default = "";
                DefaultTexture = null;
            }

            protected override void OnLoad(ConfigNode node)
            {
                base.OnLoad(node);

                if (node.HasValue("Scale"))
                {
                    Scale = ConfigNode.ParseVector2(node.GetValue("Scale"));
                }

                if (node.HasValue("Default"))
                {
                    Default = node.GetValue("Default");
                }

                DefaultTexture = GameDatabase.Instance.GetTexture(Default, false);

                if (Size == Vector2.zero)
                {
                    Size = new Vector2((float) DefaultTexture.width / Screen.width * Scale.x, (float) DefaultTexture.height / Screen.height * Scale.y);
                }
            }

            protected override void OnDraw()
            {
                base.OnDraw();

                UpdateTexture();
                GUI.DrawTexture(Bounds, Texture);
            }

            private void UpdateTexture()
            {
                var vessel = FlightGlobals.ActiveVessel;

                if (vessel != null)
                {
                    List<string> flags = new List<string>();

                    foreach (var part in vessel.Parts)
                    {
                        flags.Add(part.flagURL);
                    }

                    // Find the flag with the highest occurrance in the entire vessel

                    var url = flags.GroupBy(item => item).OrderByDescending(item => item.Count()).First().Key;

                    if (string.IsNullOrEmpty(url))
                    {
                        Texture = DefaultTexture;
                    }
                    else
                    {
                        Texture = GameDatabase.Instance.GetTexture(url, false);
                    }
                }
                else
                {
                    Texture = DefaultTexture;
                }
            }
        }
        #endregion

        private List<Element> m_Elements = new List<Element>();
        private bool m_Active = false;
        private ApplicationLauncherButton m_ApplicationLauncherButton = null;
        private Texture m_ApplicationLauncherButtonTexture = null;
        private Texture m_ApplicationLauncherButtonSuppressedTexture= null;
        private Rect m_ConfigurationWindowPosition;
        private bool m_DisplayConfigurationWindow = false;
        private bool m_AlwaysActive = false;
        private bool m_Suppressed = false;

        private void Awake()
        {
            DontDestroyOnLoad(this);
            LoadConfiguration();
            
            m_ConfigurationWindowPosition = new Rect(0.5f * Screen.width - 200.0f, 0.5f * Screen.height - 75.0f, 400.0f, 130.0f);

            InitializeApplicationLauncherButton();
        }

        private void Update()
        {
            if (!m_Suppressed)
            {
                if (!m_Active)
                {
                    if (GameSettings.TAKE_SCREENSHOT.GetKeyDown())
                    {
                        m_Active = true;
                    }
                }
                else
                {
                    m_Active = false;
                }
            }
        }

        private void OnGUI()
        {
            if (m_Active || m_AlwaysActive)
            {
                foreach (var element in m_Elements)
                {
                    element.Draw();
                }
            }

            if (m_DisplayConfigurationWindow)
            {
                m_ConfigurationWindowPosition = GUI.Window(0, m_ConfigurationWindowPosition, DrawConfigurationWindow, "Historian", HighLogic.Skin.window);
            }
        }

        private void InitializeApplicationLauncherButton()
        {
            var scenes = ApplicationLauncher.AppScenes.FLIGHT |
                         ApplicationLauncher.AppScenes.MAPVIEW |
                         ApplicationLauncher.AppScenes.SPACECENTER |
                         ApplicationLauncher.AppScenes.SPH |
                         ApplicationLauncher.AppScenes.TRACKSTATION |
                         ApplicationLauncher.AppScenes.VAB;

            m_ApplicationLauncherButtonTexture = GameDatabase.Instance.GetTexture("KSEA/Historian/Historian_Launcher", false);
            m_ApplicationLauncherButtonSuppressedTexture = GameDatabase.Instance.GetTexture("KSEA/Historian/Historian_Launcher_Suppressed", false);
            m_ApplicationLauncherButton = ApplicationLauncher.Instance.AddModApplication(ApplicationLauncherButton_OnTrue, ApplicationLauncherButton_OnFalse, null, null, null, null, scenes, m_ApplicationLauncherButtonTexture);
            UpdateApplicationLauncherButton();
        }

        private void LoadConfiguration()
        {
            m_Elements.Clear();

            var path = Path.Combine(PluginDirectory, "Historian.cfg");

            var root = ConfigNode.Load(path).GetNode("KSEA_HISTORIAN_CONFIGURATION");
            var layout = root.GetNode("LAYOUT");

            foreach (var node in layout.nodes)
            {
                var element = Element.Create((ConfigNode) node);

                if (element != null)
                {
                    m_Elements.Add(element);
                }
            }
        }

        private static void Print(string format, params object[] args)
        {
            var message = string.Format(format, args);
            Print(message);
        }

        private static void Print(string message)
        {
            Debug.Log("[KSEA.Historian] " + message);
        }

        private void ApplicationLauncherButton_OnTrue()
        {
            m_DisplayConfigurationWindow = true;
        }

        private void ApplicationLauncherButton_OnFalse()
        {
            m_DisplayConfigurationWindow = false;
        }

        private void DrawConfigurationWindow(int id)
        {
            GUI.skin = HighLogic.Skin;

            GUILayout.BeginVertical();

            m_Suppressed = GUILayout.Toggle(m_Suppressed, "Suppressed");
            m_AlwaysActive = GUILayout.Toggle(m_AlwaysActive, "Always Active");

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Reload", GUILayout.Width(100.0f)))
            {
                LoadConfiguration();
            }

            if (GUILayout.Button("Close", GUILayout.Width(100.0f)))
            {
                m_ApplicationLauncherButton.SetFalse();
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            UpdateApplicationLauncherButton();
            GUI.DragWindow();
        }

        private void UpdateApplicationLauncherButton()
        {
            if (m_Suppressed)
            {
                m_ApplicationLauncherButton.SetTexture(m_ApplicationLauncherButtonSuppressedTexture);
            }
            else
            {
                m_ApplicationLauncherButton.SetTexture(m_ApplicationLauncherButtonTexture);
            }
        }
    }
}