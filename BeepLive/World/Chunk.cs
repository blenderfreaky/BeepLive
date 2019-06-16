﻿using SFML.Graphics;
using SFML.System;
using System;

namespace BeepLive.World
{
    public class Chunk
    {
        public Map Map;
        //public Voxel[,] Voxels;
        public Image Voxels;
        public Sprite Sprite;

        public Chunk(Map map, Vector2f offset)
        {
            Map = map;

            Voxels = new Image(map.ChunkSize, map.ChunkSize);
            Sprite = new Sprite(new Texture(Voxels)) { Texture = { Smooth = true }, Position = offset };
        }

        public void Update() => Sprite.Texture.Update(Voxels);

        public Voxel this[uint x, uint y]
        {
            get => new Voxel(Map, Voxels.GetPixel(x, y));
            set => Voxels.SetPixel(x, y, value.Color);
        }

        public Vector2u GetVoxelIndex(Vector2f position) =>
            new Vector2u((uint)Math.Floor(position.X),
                (uint)MathF.Floor(position.Y));

        public Voxel GetVoxel(Vector2f position) =>
            this[(uint)Math.Floor(position.X),
                (uint)MathF.Floor(position.Y)];
    }
}