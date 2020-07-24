namespace BeepLive.Entities
{
    using BeepLive.Config;
    using BeepLive.World;
    using SFML.Graphics;
    using SFML.System;
    using System;

    public class Projectile<TShotConfig> : Entity
        where TShotConfig : ShotConfig
    {
        public int LifeTime;

        public Player Owner;
        public TShotConfig ShotConfig;
        public VoxelType VoxelTypeToPlace;

        public Projectile(Map map, Vector2f position, Vector2f velocity, TShotConfig shotConfig, Player owner)
        {
            Map = map;
            Velocity = velocity;
            ShotConfig = shotConfig;

            Owner = owner;

            VoxelTypeToPlace =
                ShotConfig.Destructive ? null :
                ShotConfig.Neutral ? Map.Config.GroundVoxelType :
                Owner == null ? null : Owner.Team?.VoxelType ?? Map.Config.GroundVoxelType;

            Shape = new CircleShape
            {
                Position = position,
                Radius = ShotConfig.Radius,//THIS IS SOMEHOW ZERO ------------------------------------------------------- 
                FillColor = VoxelTypeToPlace == null ?
                new Color(255, 255, 255) :
                    new Color(
                        (byte)(VoxelTypeToPlace.Color.R * .8),
                        (byte)(VoxelTypeToPlace.Color.G * .8),
                        (byte)(VoxelTypeToPlace.Color.B * .8))
            };

            Position = position;
        }

        public CircleShape CircleShape
        {
            get => (CircleShape)Shape;
            set => Shape = value;
        }

        public sealed override Vector2f Position
        {
            get => CircleShape.Position;
            set => CircleShape.Position = value;
        }

        public override void Step()
        {
            Voxel voxelUnderCenter = Map.GetVoxel(Position);

            Velocity += Map.Config.PhysicalEnvironment.Gravity;
            if (voxelUnderCenter.IsAir)
            {
                Velocity *= Map.Config.PhysicalEnvironment.AirResistance;
            }
            else
            {
                Velocity *= voxelUnderCenter.VoxelType.Resistance
                            * (voxelUnderCenter.GetTeamRelation(Owner.Team) switch
                            {
                                TeamRelation.Friendly => ShotConfig.FriendlyResistanceFactor,
                                TeamRelation.Neutral => ShotConfig.NeutralResistanceFactor,
                                TeamRelation.Hostile => ShotConfig.HostileResistanceFactor,
                                _ => throw new ArgumentOutOfRangeException(),
                            });
            }

            bool hitsPlayer = false;
            foreach (var player in Map.Players)
            {
                if (!player.Boundary.Contains(Position)) continue;

                hitsPlayer = true;

                if (player.Team == null) Velocity *= ShotConfig.NeutralResistanceFactor;
                if (player.Team == Owner.Team) Velocity *= ShotConfig.FriendlyResistanceFactor;
                else Velocity *= ShotConfig.HostileResistanceFactor;
            }

            float velocity = (Velocity.X * Velocity.X) + (Velocity.Y * Velocity.Y);

            if (velocity < ShotConfig.LowestSpeed ||
                LifeTime++ > ShotConfig.MaxLifeTime ||
                !Map.Config.EntityBoundary.Contains(Position) ||
                hitsPlayer)
            {
                Die();
            }

            Vector2f front = Velocity / velocity;
            Vector2f left = new Vector2f(front.Y, -front.X);

            for (float x = 0; x < velocity; x += 1.5f)
            {
                for (float y = -ShotConfig.Radius; y <= ShotConfig.Radius; y++)
                {
                    Vector2f position = Position + (front * x) + (left * y);

                    Chunk chunk = Map.GetChunk(position, out Vector2f chunkPosition);
                    if (chunk == null) continue;
                    uint xFloored = (uint)MathF.Floor(position.X - chunkPosition.X);
                    uint yFloored = (uint)MathF.Floor(position.Y - chunkPosition.Y);

                    if ((ShotConfig.Damages & chunk[xFloored, yFloored].GetTeamRelation(Owner.Team)) != TeamRelation.Air)
                    {
                        chunk[xFloored, yFloored] = new Voxel(Map, VoxelTypeToPlace);
                    }
                }
            }

            Position += Velocity;
        }

        protected override void Dispose(bool disposing)
        {
            if (!Disposed) CircleShape.Dispose();
            base.Dispose(disposing);
        }
    }
}