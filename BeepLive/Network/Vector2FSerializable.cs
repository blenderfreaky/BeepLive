namespace BeepLive.Network
{
    using ProtoBuf;
    using SFML.System;

    [ProtoContract]
    public class Vector2FSerializable
    {
        [ProtoMember(1)] public float X;
        [ProtoMember(2)] public float Y;

        public Vector2FSerializable()
        {
        }

        public Vector2FSerializable(float x, float y)
        {
            X = x;
            Y = y;
        }

        public static implicit operator Vector2f(Vector2FSerializable v)
        {
            return new Vector2f(v.X, v.Y);
        }

        public static implicit operator Vector2FSerializable(Vector2f v)
        {
            return new Vector2FSerializable(v.X, v.Y);
        }
    }
}