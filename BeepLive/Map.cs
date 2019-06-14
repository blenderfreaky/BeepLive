using System.Collections.Generic;
using UltimateQuadTree;

namespace BeepLive
{
    public class Map
    {
        public QuadTree<Voxel> VoxelTree;
        public List<Enitity> Entities;

        public Map()
        {
            VoxelTree = new QuadTree<Voxel>(0, 0, 1000, 1000, new ObjectBounds());
            VoxelTree.Insert(Voxel)
        }
    }

    public class ObjectBounds : IQuadTreeObjectBounds<Voxel>
    {
        public double GetBottom(Voxel obj)
        {
            var bounds = obj.shape.GetGlobalBounds();
            return bounds.Top + bounds.Height;
        }

        public double GetLeft(Voxel obj)
        {
            var bounds = obj.shape.GetGlobalBounds();
            return bounds.Left;
        }

        public double GetRight(Voxel obj)
        {
            var bounds = obj.shape.GetGlobalBounds();
            return bounds.Left + bounds.Width;
        }

        public double GetTop(Voxel obj)
        {
            var bounds = obj.shape.GetGlobalBounds();
            return bounds.Top;
        }
    }
}