namespace BeepLive.Net
{
    using ProtoBuf;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public class StreamProtobufWriter
    {
        public List<Type> Types = new List<Type>();

        public Dictionary<int, Type> TypeByIndex;
        public Dictionary<Type, int> IndexByType;

        public PrefixStyle PrefixStyle;

        public StreamProtobufWriter(List<Type> types, PrefixStyle prefixStyle)
        {
            Types = types;
            PrefixStyle = prefixStyle;

            var typesAndIndicies = Types.Select((x, i) => (x, i)).ToArray();
            TypeByIndex = typesAndIndicies.ToDictionary(x => x.i, x => x.x);
            IndexByType = typesAndIndicies.ToDictionary(x => x.x, x => x.i);
        }

        public void WriteNext(Stream stream, object obj)
        {
            int field = IndexByType[obj.GetType()];
            Serializer.NonGeneric.SerializeWithLengthPrefix(
                stream,
                obj,
                PrefixStyle,
                field);
        }

        public bool ReadNext(Stream stream)
        {
            if (!Serializer.NonGeneric.TryDeserializeWithLengthPrefix(
                stream,
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
