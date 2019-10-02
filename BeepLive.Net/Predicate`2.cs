namespace BeepLive.Net
{
    /// <summary>
    /// Represents the method that defines a set of criteria and determines whether the
    /// specified object meets those criteria.
    /// </summary>
    /// <typeparam name="T1">The object to compare against the criteria defined within the method represented
    /// by this delegate.</typeparam>
    /// <param name="obj1">The type of the object to compare.</param>
    /// <typeparam name="T2">The object to compare against the criteria defined within the method represented
    /// by this delegate.</typeparam>
    /// <param name="obj2">The type of the object to compare.</param>
    public delegate bool Predicate<in T1, in T2>(T1 obj1, T2 obj2);
}