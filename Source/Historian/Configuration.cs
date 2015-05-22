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

using System;
using System.IO;

namespace KSEA.Historian
{
    public class Configuration
    {
        public class InvalidVersionException : Exception
        {
        }

        private static readonly Version CurrentVersion = new Version("0.2");
        
        private string m_Layout = "";
        private bool m_EnableLauncherButton = true;
        private bool m_EnableToolbarButton = true;

        public string Layout
        {
            get
            {
                return m_Layout;
            }

            set
            {
                m_Layout = value;
            }
        }

        public bool EnableLauncherButton
        {
            get
            {
                return m_EnableLauncherButton;
            }

            set
            {
                m_EnableLauncherButton = value;
            }
        }

        public bool EnableToolbarButton
        {
            get
            {
                return m_EnableToolbarButton;
            }

            set
            {
                m_EnableToolbarButton = value;
            }
        }

        public static Configuration Load(string file)
        {
            try
            {
                var node = ConfigNode.Load(file).GetNode("KSEA_HISTORIAN_CONFIGURATION");
                var configuration = new Configuration();

                var version = node.GetVersion("Version", new Version());

                if (version != CurrentVersion)
                {
                    Historian.Print("Version mismatch detected on configuration file.");

                    throw new InvalidVersionException();
                }
                else
                {
                    configuration.m_Layout = node.GetString("Layout", "Default");
                    configuration.m_EnableLauncherButton = node.GetBoolean("EnableLauncherButton", true);
                    configuration.m_EnableToolbarButton = node.GetBoolean("EnableToolbarButton", true);
                }

                return configuration;
            }
            catch
            {
                Historian.Print("Failed to load configuration file '{0}'. Attempting recovery ...", file);

                try
                {
                    if (File.Exists(file))
                    {
                        File.Delete(file);

                        var configuration = new Configuration();
                        configuration.m_Layout = "Default";
                        configuration.m_EnableLauncherButton = true;
                        configuration.m_EnableToolbarButton = true;
                        configuration.Save(file);

                        return configuration;
                    }
                }
                catch
                {
                    Historian.Print("Failed to overwrite invalid configuration file '{0}'.", file);
                    throw;
                }
            }

            return null;
        }

        public void Save(string file)
        {
            try
            {
                var root = new ConfigNode();
                var node = root.AddNode("KSEA_HISTORIAN_CONFIGURATION");

                node.AddValue("Version", CurrentVersion.ToString());
                node.AddValue("Layout", m_Layout);
                node.AddValue("EnableLauncherButton", m_EnableLauncherButton);
                node.AddValue("EnableToolbarButton", m_EnableToolbarButton);

                if (File.Exists(file))
                {
                    File.Delete(file);
                }

                root.Save(file);

                Historian.Print("Configuration saved at '{0}'.", file);
            }
            catch
            {
                Historian.Print("Failed to save configuration file '{0}'.", file);
            }
        }
    }
}