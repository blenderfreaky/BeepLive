using System;
using BeepLive.World;
using SFML.Graphics;
using SFML.System;

namespace BeepLive.Entities
{
    public class Player : Entity
    {
        private Vector2f _lastSafePosition;

        public float Health;
        public string Name;

        public Player(Map map, Vector2f position, int size)
        {
            Shape = new RectangleShape
            {
                Position = position,
                Size = new Vector2f(size, size),
                FillColor = Color.Red
            };

            Map = map;
            Position = position;
            Size = size;
        }

        internal Player(Map map)
        {
            Map = map;
        }

        public Boundary Boundary => new Boundary {Min = Position, Max = Position + new Vector2f(Size, Size)};

        public int Size { get; set; }

        public sealed override Vector2f Position
        {
            get => ((RectangleShape) Shape).Position;
            set => ((RectangleShape) Shape).Position = value;
        }

        internal void GenerateShape()
        {
            Shape = new RectangleShape
            {
                Position = Position,
                Size = new Vector2f(Size, Size),
                FillColor = Color.Red
            };
        }

        public override void Step()
        {
            CollisionCheck();

            Velocity += Map.Config.PhysicalEnvironment.Gravity;
            Velocity *= Map.Config.PhysicalEnvironment.AirResistance;

            Position += Velocity;
        }

        public void CollisionCheck()
        {
            switch (Map.Config.PhysicalEnvironment.CollisionResponseMode)
            {
                case CollisionResponseMode.NoClip:

                    break;
                case CollisionResponseMode.Raise:
                    var collides = false;

                    for (var i = 0; i < Size; i++)
                    for (var j = 0; j < Size; j++)
                        if (!GetVoxel(i, j).IsAir)
                            collides = true;

                    if (collides)
                    {
                        Position = _lastSafePosition;
                        Velocity *= -0.5f;
                    }
                    else
                    {
                        _lastSafePosition = Position;
                    }

                    break;
                case CollisionResponseMode.LeastResistance:
                    // The center of mass of voxels intersecting the player
                    Vector2f center = new Vector2f(0, 0);
                    // The amount of voxels intersecting the Player
                    var collisionCount = 0;

                    for (var i = 0; i < Size; i++)
                    for (var j = 0; j < Size; j++)
                    {
                        if (GetVoxel(i, j).IsAir) continue;

                        center.X += i;
                        center.Y += j;

                        collisionCount++;
                    }

                    center /= collisionCount;

                    center.X -= Size / 2f;
                    center.Y -= Size / 2f;

                    Velocity -= center;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (!Disposed) ((RectangleShape) Shape).Dispose();
            base.Dispose(disposing);
        }

        #region Fluent API

        public Player SetSize(int size)
        {
            Size = size;

            return this;
        }

        public Player SetPosition(Vector2f position)
        {
            Position = position;

            return this;
        }

        public Player SetName(string name)
        {
            Name = name;

            return this;
        }

        public Player SetHealth(float health)
        {
            Health = health;

            return this;
        }

        #endregion
    }
}