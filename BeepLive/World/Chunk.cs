namespace BeepLive.World
{
    using SFML.Graphics;
    using SFML.System;
    using System;

    public class Chunk
    {
        public readonly Map Map;

        public readonly Sprite Sprite;

        private readonly byte[] _pixels;

        public Chunk(Map map, Vector2f offset)
        {
            Map = map;

            _pixels = new byte[Map.Config.ChunkSize * Map.Config.ChunkSize * 4];
            Sprite = new Sprite(new Texture(Map.Config.ChunkSize, Map.Config.ChunkSize)) { Texture = { Smooth = true }, Position = offset };
        }

        public Voxel this[uint x, uint y]
        {
            get
            {
                var pos = (x + (y * Map.Config.ChunkSize)) * 4;
                return new Voxel(Map, new Color(
                    _pixels[pos + 0],
                    _pixels[pos + 1],
                    _pixels[pos + 2],
                    _pixels[pos + 3]));
            }

            set
            {
                var pos = (x + (y * Map.Config.ChunkSize)) * 4;
                _pixels[pos + 0] = value.Color.R;
                _pixels[pos + 1] = value.Color.G;
                _pixels[pos + 2] = value.Color.B;
                _pixels[pos + 3] = value.Color.A;
            }
        }

        public void Update()
        {
            Sprite.Texture.Update(_pixels);
        }

        public static Vector2u GetVoxelIndex(Vector2f position)
        {
            return new Vector2u((uint)Math.Floor(position.X),
                (uint)MathF.Floor(position.Y));
        }

        public Voxel GetVoxel(Vector2f position)
        {
            return this[(uint)Math.Floor(position.X),
                (uint)MathF.Floor(position.Y)];
        }
    }
}