namespace BeepLive.Network
{
    using ProtoBuf;
    using SFML.System;

    [ProtoContract]
    public struct Vector2fSerializable
    {
        [ProtoMember(1)] public float X;
        [ProtoMember(2)] public float Y;

        public Vector2fSerializable(float x, float y)
        {
            X = x;
            Y = y;
        }

        public static implicit operator Vector2f(Vector2fSerializable v) =>
            new Vector2f(v.X, v.Y);

        public static implicit operator Vector2fSerializable(Vector2f v) =>
            new Vector2fSerializable(v.X, v.Y);

        public override string ToString() => $"({X}, {Y})";
    }
}