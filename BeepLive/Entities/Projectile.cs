using System;
using System.Linq;
using BeepLive.Config;
using BeepLive.World;
using SFML.Graphics;
using SFML.System;

namespace BeepLive.Entities
{
    public class Projectile<TShotConfig> : Entity
        where TShotConfig : ShotConfig
    {
        public TShotConfig ShotConfig;

        public Player Owner;
        public VoxelType VoxelTypeToPlace;

        public Projectile(Map map, Vector2f position, Vector2f velocity, TShotConfig shotConfig, Player owner = null)
        {
            Shape = new CircleShape
            {
                Position = position,
                Radius = shotConfig.Radius,
                FillColor = Color.Yellow
            };

            Map = map;
            Position = position;
            Velocity = velocity;
            ShotConfig = shotConfig;

            Owner = owner;

            VoxelTypeToPlace = 
                ShotConfig.Destructive ? null :
                ShotConfig.Neutral ? Map.Config.GroundVoxelType :
                Owner == null ? null : Owner.Team?.VoxelType ?? Map.Config.GroundVoxelType;
        }

        public CircleShape CircleShape
        {
            get => (CircleShape) Shape;
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
            Velocity *= voxelUnderCenter.IsAir
                ? Map.Config.PhysicalEnvironment.AirResistance
                : voxelUnderCenter.VoxelType.Resistance *
                  (voxelUnderCenter.VoxelType.OwnerTeam == null
                      ? ShotConfig.NeutralResistanceFactor
                      : voxelUnderCenter.VoxelType.OwnerTeam == Owner.Team
                          ? ShotConfig.FriendlyResistanceFactor
                          : ShotConfig.HostileResistanceFactor);

            float dist = MathF.Sqrt(Velocity.X * Velocity.X + Velocity.Y * Velocity.Y);

            if (dist < ShotConfig.LowestSpeed ||
                LifeTime++ > ShotConfig.MaxLifeTime ||
                !Map.Config.EntityBoundary.Contains(Position) ||
                Map.Players.Any(p => p.Boundary.Contains(Position)))
                Die();

            Vector2f front = Velocity / dist;
            var left = new Vector2f(front.Y, -front.X);

            for (float x = 0; x < dist; x += .5f)
            for (float y = -ShotConfig.Radius; y <= ShotConfig.Radius; y++)
            {
                Vector2f position = Position + front * x + left * y;
                
                Chunk chunk = Map.GetChunk(position, out Vector2f chunkPosition);
                if (chunk == null) continue;
                uint xFloored = (uint) MathF.Floor(position.X - chunkPosition.X);
                uint yFloored = (uint) MathF.Floor(position.Y - chunkPosition.Y);

                if ((ShotConfig.Damages & chunk[xFloored, yFloored].GetTeamRelation(Owner.Team)) != TeamRelation.Air)
                {
                    chunk[xFloored,yFloored] = new Voxel(Map, VoxelTypeToPlace);
                }
            }

            Position += Velocity;
        }

        public int LifeTime;

        public virtual void Die()
        {
            Map.Entities.Remove(this);
            Dispose(true);
        }

        protected override void Dispose(bool disposing)
        {
            if (!Disposed) CircleShape.Dispose();
            base.Dispose(disposing);
        }
    }
}