using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using NitroxModel.DataStructures.Surrogates;
using NitroxModel.Logger;
using NitroxModel.Networking;
using System.Collections.Generic;
using SevenZip.Compression.LZMA;

namespace NitroxModel.Packets
{
    [Serializable]
    public abstract class Packet
    {
        private static readonly SurrogateSelector surrogateSelector;
        private static readonly StreamingContext streamingContext;
        private static readonly BinaryFormatter serializer;

        private static readonly string[] blacklistedAssemblies = { "NLog" };
        private static readonly Decoder decoder = new Decoder();
        private static readonly Encoder encoder = new Encoder();

        static Packet()
        {
            surrogateSelector = new SurrogateSelector();
            streamingContext = new StreamingContext(StreamingContextStates.All); // Our surrogates can be safely used in every context.
            IEnumerable<Type> types = AppDomain.CurrentDomain.GetAssemblies()
                                               .Where(assembly => !blacklistedAssemblies.Contains(assembly.GetName().Name))
                                               .SelectMany(a => a.GetTypes()
                                                                 .Where(t =>
                                                                            t.BaseType != null &&
                                                                            t.BaseType.IsGenericType &&
                                                                            t.BaseType.GetGenericTypeDefinition() == typeof(SerializationSurrogate<>) &&
                                                                            t.IsClass &&
                                                                            !t.IsAbstract));

            foreach (Type type in types)
            {
                ISerializationSurrogate surrogate = (ISerializationSurrogate)Activator.CreateInstance(type);
                Type surrogatedType = type.BaseType.GetGenericArguments()[0];
                surrogateSelector.AddSurrogate(surrogatedType, streamingContext, surrogate);

                Log.Debug($"Added surrogate {surrogate.GetType().Name} for type {surrogatedType}");
            }

            // For completeness, we could pass a StreamingContextStates.CrossComputer.
            serializer = new BinaryFormatter(surrogateSelector, streamingContext);
        }

        public NitroxDeliveryMethod.DeliveryMethod DeliveryMethod { get; protected set; } = NitroxDeliveryMethod.DeliveryMethod.RELIABLE_ORDERED;
        public UdpChannelId UdpChannel { get; protected set; } = UdpChannelId.DEFAULT;

        public enum UdpChannelId
        {
            DEFAULT = 0,
            PLAYER_MOVEMENT = 1,
            VEHICLE_MOVEMENT = 2,
            PLAYER_STATS = 3
        }

        public byte[] Serialize()
        {
            using (MemoryStream input = new MemoryStream())
            using (MemoryStream output = new MemoryStream())
            {
                serializer.Serialize(input, this);
                input.Position = 0;
                
                // Write the data size and compress the data.
                encoder.WriteCoderProperties(output);
                output.Write(BitConverter.GetBytes((uint)input.Length), 0, 4);
                encoder.Code(input, output, input.Length, -1, null);
                
                return output.ToArray();
            }
        }

        public static Packet Deserialize(byte[] data)
        {
            using (MemoryStream input = new MemoryStream(data))
            using (MemoryStream output = new MemoryStream())
            {
                byte[] properties = new byte[5];
                input.Read(properties, 0, 5);
                
                // Read in the size of serialized (uncompressed) data.
                byte[] fileLengthBytes = new byte[4];
                input.Read(fileLengthBytes, 0, 4);
                uint fileLength = BitConverter.ToUInt32(fileLengthBytes, 0);
                
                // Decompress.
                decoder.SetDecoderProperties(properties);
                decoder.Code(input, output, input.Length, fileLength, null);
                output.Position = 0;
                
                return (Packet)serializer.Deserialize(output);
            }
        }

        public static bool IsTypeSerializable(Type type)
        {
            // We have our own surrogates to (de)serialize types that are not marked [Serializable]
            // This code is very similar to how serializability is checked in:
            // System.Runtime.Serialization.Formatters.Binary.BinaryCommon.CheckSerializable

            ISurrogateSelector selector;
            return (serializer.SurrogateSelector.GetSurrogate(type, Packet.serializer.Context, out selector) != null);
        }

        public WrapperPacket ToWrapperPacket()
        {
            return new WrapperPacket(Serialize());
        }
    }
}
