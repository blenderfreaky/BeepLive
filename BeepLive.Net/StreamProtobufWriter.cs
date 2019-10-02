namespace BeepLive.Net
{
    using ProtoBuf;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public class StreamProtobuf
    {
        public readonly PrefixStyle PrefixStyle;

        public readonly IReadOnlyCollection<Type> Types;

        private readonly IReadOnlyDictionary<int, Type> TypeByIndex;
        private readonly IReadOnlyDictionary<Type, int> IndexByType;

        public StreamProtobuf(PrefixStyle prefixStyle, params Type[] types)
        {
            Types = types;
            PrefixStyle = prefixStyle;

            var typesAndIndicies = Types.Select((x, i) => (x, i)).ToArray();
            TypeByIndex = typesAndIndicies.ToDictionary(x => x.i, x => x.x);
            IndexByType = typesAndIndicies.ToDictionary(x => x.x, x => x.i);
        }

        public void WriteNext(Stream stream, object obj) =>
            Serializer.NonGeneric.SerializeWithLengthPrefix(
                stream,
                obj,
                PrefixStyle,
                IndexByType[obj.GetType()]);

        public void WriteNext<T>(Stream stream, T obj) =>
            Serializer.SerializeWithLengthPrefix(
                stream,
                obj,
                PrefixStyle,
                IndexByType[typeof(T)]);

        public bool ReadNext(Stream stream, out object value) =>
            Serializer.NonGeneric.TryDeserializeWithLengthPrefix(
                stream,
                PrefixStyle,
                field => TypeByIndex[field],
                out value);
    }
}
