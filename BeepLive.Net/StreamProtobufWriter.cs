namespace BeepLive.Net
{
    using ProtoBuf;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public class StreamProtobuf
    {
        public readonly IReadOnlyCollection<Type> Types;

        public readonly Stream Stream;

        private readonly IReadOnlyDictionary<int, Type> TypeByIndex;
        private readonly IReadOnlyDictionary<Type, int> IndexByType;

        public readonly PrefixStyle PrefixStyle;

        public StreamProtobuf(Stream stream, PrefixStyle prefixStyle, params Type[] types)
        {
            Types = types;
            Stream = stream;
            PrefixStyle = prefixStyle;

            var typesAndIndicies = Types.Select((x, i) => (x, i)).ToArray();
            TypeByIndex = typesAndIndicies.ToDictionary(x => x.i, x => x.x);
            IndexByType = typesAndIndicies.ToDictionary(x => x.x, x => x.i);
        }

        public void WriteNext(object obj)
        {
            int field = IndexByType[obj.GetType()];
            Serializer.NonGeneric.SerializeWithLengthPrefix(
                Stream,
                obj,
                PrefixStyle,
                field);
        }

        public bool ReadNext()
        {
            if (!Serializer.NonGeneric.TryDeserializeWithLengthPrefix(
                Stream,
                PrefixStyle,
                field => TypeByIndex[field],
                out object obj))
            {
                return false;
            }

            Console.WriteLine(obj);
            return true;
        }
    }
}
