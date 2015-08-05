/**
 * This file is part of Historian.
 * 
 * Historian is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * Historian is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with Historian. If not, see <http://www.gnu.org/licenses/>.
 **/

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;


namespace KSEA.Historian
{
    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
	public class Historian : Singleton<Historian>
    {
        private List<Layout> m_Layouts = new List<Layout>();
        private int m_CurrentLayoutIndex = -1;
        private bool m_Active = false;
        private bool m_AlwaysActive = false;
        private bool m_Suppressed = false;
        private Configuration m_Configuration = null;
        private Editor m_Editor = null;
        private bool m_SuppressEditorWindow = false;

        public bool Suppressed
        {
            get
            {
                return m_Suppressed;
            }

            set
            {
                m_Suppressed = value;
            }
        }

        public bool AlwaysActive
        {
            get
            {
                return m_AlwaysActive;
            }

            set
            {
                m_AlwaysActive = value;
            }
        }

        public int CurrentLayoutIndex
        {
            get
            {
                return m_CurrentLayoutIndex;
            }

            set
            {
                m_CurrentLayoutIndex = value;
            }
        }
        
        private string PluginDirectory
        {
            get
            {
                var path = Assembly.GetExecutingAssembly().Location;
                return Path.GetDirectoryName(path);
            }
        }

        public void Reload()
        {
            m_Layouts.Clear();
            LoadLayouts();
            m_CurrentLayoutIndex = FindLayoutIndex(m_Configuration.Layout);
        }

        public Configuration GetConfiguration()
        {
            return m_Configuration;
        }

        public string[] GetLayoutNames()
        {
            return m_Layouts.Select(item => item.Name).ToArray();
        }

        public string GetLayoutName(int index)
        {
            return GetLayout(index).Name;
        }

        public string GetCurrentLayoutName()
        {
            return GetCurrentLayout().Name;
        }

        public void SetConfiguration(Configuration configuration)
        {
            m_Configuration = configuration;
            m_Configuration.Save(Path.Combine(PluginDirectory, "Historian.cfg"));
        }

        void Awake()
        {
            DontDestroyOnLoad(this);

            m_Configuration = Configuration.Load(Path.Combine(PluginDirectory, "Historian.cfg"));

            LoadLayouts();

            m_CurrentLayoutIndex = FindLayoutIndex(m_Configuration.Layout);
            Print("Current Layout Index {0}", m_CurrentLayoutIndex);
            m_Editor = new Editor(m_Configuration);

            GameEvents.onHideUI.Add(Game_OnHideGUI);
            GameEvents.onShowUI.Add(Game_OnShowGUI);
            GameEvents.onGamePause.Add(Game_OnPause);
            GameEvents.onGameUnpause.Add(Game_OnUnpause);
        }
		public void set_m_Active()
		{
			m_Active = true;
		}

        void Update()
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
                    if (!m_Configuration.PersistentCustomText)
                    {
                        m_Configuration.CustomText = "";
                        m_Configuration.Save(Path.Combine(PluginDirectory, "Historian.cfg"));
                    }

                    m_Active = false;
                }
            }
        }

        void OnGUI()
        {
            if (!m_Suppressed && (m_Active || m_AlwaysActive))
            {
                var layout = GetCurrentLayout();
                layout.Draw();
            }

            if (!m_SuppressEditorWindow)
            {
                m_Editor.Draw();
            }
        }

        private void Game_OnHideGUI()
        {
            if (!m_Configuration.PersistentConfigurationWindow)
            {
                m_SuppressEditorWindow = true;
            }
        }

        private void Game_OnShowGUI()
        {
            m_SuppressEditorWindow = false;
        }

        private void Game_OnUnpause()
        {
            m_SuppressEditorWindow = false;
        }

        private void Game_OnPause()
        {
            m_SuppressEditorWindow = true;
        }

        private int FindLayoutIndex(string name)
        {
            return m_Layouts.FindIndex(layout => layout.Name == name);
        }

        private Layout FindLayout(string name)
        {
            var index = FindLayoutIndex(name);

            return GetLayout(index);
        }

        private void LoadLayouts()
        {
            Print("Searching for layouts ...");

            var directory = Path.Combine(PluginDirectory, "Layouts");
            var files = Directory.GetFiles(directory, "*.layout");

            foreach (var file in files)
            {
                LoadLayout(file);
            }
        }

        private void LoadLayout(string file)
        {
            try
            {
                var name = Path.GetFileNameWithoutExtension(file);
                var node = ConfigNode.Load(file).GetNode("KSEA_HISTORIAN_LAYOUT");

                if (m_Layouts.FindIndex(layout => layout.Name == name) < 0)
                {
                    var layout = Layout.Load(name, node);
                    m_Layouts.Add(layout);

                    Print("Found layout '{0}'.", name);
                }
                else
                {
                    Print("Layout with name '{0}' already exists. Unable to load duplicate.", name);
                }
            }
            catch
            {
                Print("Failed to load layout '{0}'.", name);
            }
        }

        private Layout GetLayout(int index)
        {
            if (index >= 0 && index < m_Layouts.Count)
            {
                return m_Layouts[index];
            }

            return Layout.Empty;
        }

        private Layout GetCurrentLayout()
        {
            return GetLayout(m_CurrentLayoutIndex);
        }

        public static void Print(string format, params object[] args)
        {
            Print(string.Format(format, args));
        }

        public static void Print(string message)
        {
            Debug.Log("[KSEA.Historian] " + message);
        }
    }
}