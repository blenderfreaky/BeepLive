namespace BeepLive.Entities
{
    using BeepLive.Config;
    using BeepLive.Game;
    using BeepLive.World;
    using SFML.Graphics;
    using SFML.System;
    using System;

    public class Player : Entity
    {
        public string UserName;
        private Vector2f _lastSafePosition;
        public Guid Guid;

        public float Health;
        public string Name;

        public Team Team;

        public Player(Map map, Vector2f position, int size, Team team, Guid guid, string userName)
        {
            Size = size;
            Team = team;

            GenerateShape(position);

            Map = map;
            Position = position;

            Guid = guid;
            UserName = userName;
        }

        public Boundary Boundary => new Boundary { Min = Position, Max = Position + new Vector2f(Size, Size) };

        public int Size { get; set; }

        public sealed override Vector2f Position
        {
            get => ((RectangleShape)Shape).Position;
            set => ((RectangleShape)Shape).Position = value;
        }

        internal void GenerateShape(Vector2f position)
        {
            Shape = new RectangleShape
            {
                Position = position,
                Size = new Vector2f(Size, Size),
                FillColor = Team.VoxelType.Color
            };
        }

        public Projectile<ShotConfig> Shoot(ShotConfig shotConfig, Vector2f velocity)
        {
            var projectile =
                new Projectile<ShotConfig>(Map, Position + (velocity.Normalized() * Size * 2), velocity, shotConfig, this);
            Map.EntitiesBuffer.TryAdd(projectile, 0);
            return projectile;
        }

        public ClusterProjectile Shoot(ClusterShotConfig shotConfig, Vector2f velocity)
        {
            var projectile =
                new ClusterProjectile(Map, Position + (velocity.Normalized() * Size * 2), velocity, shotConfig, this);
            Map.EntitiesBuffer.TryAdd(projectile, 0);
            return projectile;
        }

        public override void Step()
        {
            CollisionCheck();

            Velocity += Map.Config.PhysicalEnvironment.Gravity;
            Velocity *= Map.Config.PhysicalEnvironment.AirResistance;

            Alive = Map.Config.EntityBoundary.Contains(Position);

            Position += Velocity;
        }

        public void CollisionCheck()
        {
            switch (Map.Config.PhysicalEnvironment.CollisionResponseMode)
            {
                case CollisionResponseMode.NoClip:

                    break;

                case CollisionResponseMode.Raise:
                    bool collides = false;

                    for (int i = 0; i < Size; i++)
                    {
                        for (int j = 0; j < Size; j++)
                        {
                            if (!GetVoxel(i, j).IsAir)
                            {
                                collides = true;
                            }
                        }
                    }

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

        protected override void Dispose(bool disposing)
        {
            if (!Disposed) ((RectangleShape)Shape).Dispose();
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

        #endregion Fluent API
    }
}