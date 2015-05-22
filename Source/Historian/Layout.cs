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

namespace KSEA.Historian
{
    public class Layout
    {
        public static readonly Layout Empty = new Layout();
        private List<IElement> m_Elements = new List<IElement>();
        private string m_Name = "";

        public static Layout Load(string name, ConfigNode node)
        {
            try
            {
                var layout = new Layout(name);

                foreach (var child in node.GetNodes())
                {
                    IElement element = Element.Create(child.name);

                    if (element != null)
                    {
                        element.Load(child);
                        layout.AddElement(element);
                    }
                    else
                    {
                        Historian.Print("Failed to load layout element of type '{0}'.", child.name);
                    }
                }

                return layout;
            }
            catch
            {
                Historian.Print("Failed to load layout '{0}'.", name);
            }

            return Empty;
        }

        public string Name
        {
            get
            {
                return m_Name;
            }
        }

        public Layout()
        {
        }

        public Layout(string name)
        {
            m_Name = name;
        }

        public void Draw()
        {
            foreach (var element in m_Elements)
            {
                element.Draw();
            }
        }

        private void AddElement(IElement element)
        {
            m_Elements.Add(element);
        }
    }
}