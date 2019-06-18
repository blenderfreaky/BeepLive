using ProtoBuf;
using SFML.System;

namespace BeepLive.Network
{
    [ProtoContract]
    public class Vector2FSerializable
    {
        [ProtoMember(1)] public float X;
        [ProtoMember(2)] public float Y;

        public static implicit operator Vector2f(Vector2FSerializable v) => new Vector2f(v.X, v.Y);
        public static implicit operator Vector2FSerializable(Vector2f v) => new Vector2FSerializable(v.X, v.Y);

        public Vector2FSerializable(float x, float y)
        {
            X = x;
            Y = y;
        }
    }
}