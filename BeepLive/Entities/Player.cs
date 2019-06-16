using BeepLive.World;
using SFML.Graphics;
using SFML.System;
using System;

namespace BeepLive.Entities
{
    public class Player : Entity
    {
        public int Size { get; set; }
        public sealed override Vector2f Position
        {
            get => ((RectangleShape)Shape).Position;
            set => ((RectangleShape)Shape).Position = value;
        }

        public float Health;

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

        public override void Step()
        {
            //CollisionCheck();
            //TODO check surrounding chunks

            Velocity += Map.PhysicalEnvironment.Gravity;
            Velocity *= Map.PhysicalEnvironment.AirResistance;

            Position += Velocity;
        }

        public void CollisionCheck()
        {
            switch (Map.PhysicalEnvironment.CollisionResponseMode)
            {
                case CollisionResponseMode.NoClip:

                    break;
                case CollisionResponseMode.Raise:
                    if (Functional.Evaluate(() =>
                    {
                        for (int i = 0; i < Size; i++)
                            for (int j = 0; j < Size; j++)
                                if (!GetVoxel(i, j).IsAir)
                                    return true;
                        return false;
                    }))
                    {
                        Velocity += new Vector2f(0, 1);
                    }

                    break;
                case CollisionResponseMode.LeastResistance:
                    // The center of mass of voxels intersecting the player
                    Vector2f center = new Vector2f(0, 0);
                    // The amount of voxels intersecting the Player
                    int collisionCount = 0;

                    for (int i = 0; i < Size; i++)
                    {
                        for (int j = 0; j < Size; j++)
                        {
                            if (GetVoxel(i, j).IsAir) continue;

                            center.X += i;
                            center.Y += j;

                            collisionCount++;
                        }
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
    }
}