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

using UnityEngine;

namespace KSEA.Historian
{
    public interface IElement
    {
        void Draw();
        void Load(ConfigNode node);
    }

    public abstract class Element : IElement
    {
        protected Vector2 Anchor { get; set; }
        protected Vector2 Position { get; set; }
        protected Vector2 Size { get; set; }

        public static IElement Create(string type)
        {
            switch (type)
            {
            case "RECTANGLE":
                return new Rectangle();
            case "TEXT":
                return new Text();
            case "PICTURE":
                return new Picture();
            case "FLAG":
                return new Flag();
            default:
                return null;
            }
        }

        public void Draw()
        {
            var width = Size.x * Screen.width;
            var height = Size.y * Screen.height;

            var left = (Position.x * Screen.width) - (Anchor.x * width);
            var top = (Position.y * Screen.height) - (Anchor.y * height);

            var bounds = new Rect(left, top, width, height);

            OnDraw(bounds);
        }

        public void Load(ConfigNode node)
        {
            Anchor = node.GetVector2("Anchor", Vector2.zero);
            Position = node.GetVector2("Position", Vector2.zero);
            Size = node.GetVector2("Size", Vector2.zero);

            OnLoad(node);
        }

        protected abstract void OnDraw(Rect bounds);
        protected abstract void OnLoad(ConfigNode node);
    }
}