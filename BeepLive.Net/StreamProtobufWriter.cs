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

        private readonly IReadOnlyDictionary<int, Type> _typeByIndex;
        private readonly IReadOnlyDictionary<Type, int> _indexByType;

        public StreamProtobuf(PrefixStyle prefixStyle, params Type[] types)
        {
            Types = types;
            PrefixStyle = prefixStyle;

            var typesAndIndicies = Types.Select((x, i) => (x, i)).ToArray();
            _typeByIndex = typesAndIndicies.ToDictionary(x => x.i, x => x.x);
            _indexByType = typesAndIndicies.ToDictionary(x => x.x, x => x.i);
        }

        public void WriteNext<T>(Stream stream, T obj) =>
            Serializer.SerializeWithLengthPrefix(
                stream,
                obj,
                PrefixStyle,
                _indexByType[typeof(T)]);

        public bool ReadNext(Stream stream, out object value) =>
            Serializer.NonGeneric.TryDeserializeWithLengthPrefix(
                stream,
                PrefixStyle,
                field => _typeByIndex[field],
                out value);
    }
}