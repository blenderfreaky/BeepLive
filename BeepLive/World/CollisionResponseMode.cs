namespace BeepLive.World
{
    using System.ComponentModel;

    public enum CollisionResponseMode
    {
        [Description("Don't handle collisions at all")]
        NoClip,

        [Description("Move object up vertically")]
        Raise,

        [Description("Move object to direction with least collisions")]
        LeastResistance
    }
}