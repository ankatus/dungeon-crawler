using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace DungeonCrawler.GameObjects
{
    public enum Status
    {
        Active,
        Inactive
    };

    public abstract class GameObject : Drawable
    {
        private static long _lastId;
        public long Id { get; }
        public Vector2 Velocity;
        private Status _status;
        public Status Status
        {
            get
            {
                return _status;
            }
            set
            {
                _status = value;

                if (_status == Status.Active)
                {
                    DrawThis = true;
                }
                else if (_status == Status.Inactive)
                {
                    DrawThis = false;
                }
            }
        }
        public virtual List<GameObject> GameObjectChildren { get; }

        protected GameObject(ObjectType textureID, int x, int y, int width, int height) : base(textureID, x, y, width, height)
        {
            Id = _lastId++;
            Status = Status.Active;
            GameObjectChildren = new List<GameObject>();
        }
    }
}