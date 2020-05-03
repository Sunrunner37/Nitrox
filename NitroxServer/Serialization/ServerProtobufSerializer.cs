﻿using System;
using System.IO;
using System.Linq;
using System.Reflection;
using NitroxModel.DataStructures;
using ProtoBufNet;
using ProtoBufNet.Meta;
using NitroxModel.DataStructures.GameLogic;
using NitroxServer.UnityStubs;

namespace NitroxServer.Serialization
{
    public class ServerProtobufSerializer
    {
        protected RuntimeTypeModel Model { get; } = TypeModel.Create();

        public ServerProtobufSerializer(params string[] assemblies)
        {
            foreach(string assembly in assemblies)
            {
                RegisterAssemblyClasses(assembly);
            }

            RegisterHardCodedTypes();
        }

        public void Serialize(Stream stream, object o)
        {
            Model.SerializeWithLengthPrefix(stream, o, o.GetType(), PrefixStyle.Base128, 0);
        }

        public T Deserialize<T>(Stream stream)
        {
            T t = (T)Activator.CreateInstance(typeof(T));
            Model.DeserializeWithLengthPrefix(stream, t, typeof(T), PrefixStyle.Base128, 0);
            return t;
        }

        public void Deserialize(Stream stream, object o, Type t)
        {
            Model.DeserializeWithLengthPrefix(stream, o, t, PrefixStyle.Base128, 0);
        }

        private void RegisterHardCodedTypes()
        {
            Assembly unity = AppDomain.CurrentDomain.GetAssemblies().First(assembly => assembly.GetName().Name == "UnityEngine");
            Model.Add(unity.GetType("UnityEngine.Light"), true);
            Model.Add(unity.GetType("UnityEngine.BoxCollider"), true);
            Model.Add(unity.GetType("UnityEngine.SphereCollider"), true);
            Model.Add(unity.GetType("UnityEngine.MeshCollider"), true);
            Model.Add(unity.GetType("UnityEngine.Vector3"), false).SetSurrogate(typeof(Vector3));
            Model.Add(unity.GetType("UnityEngine.Quaternion"), false).SetSurrogate(typeof(Quaternion));
            Model.Add(unity.GetType("UnityEngine.Transform"), false).SetSurrogate(typeof(NitroxTransform));
            Model.Add(unity.GetType("UnityEngine.GameObject"), false).SetSurrogate(typeof(GameObject));            
        }
        
        private void RegisterAssemblyClasses(string assemblyName)
        {
            foreach (Type type in Assembly.Load(assemblyName).GetTypes())
            {
                bool hasNitroxProtobuf = (type.GetCustomAttributes(typeof(ProtoContractAttribute), false).Length > 0);

                if (hasNitroxProtobuf)
                {
                    // As of the latest protobuf update they will automatically register detected attributes.
                    Model.Add(type, true);
                }
                else if(HasUweProtoContract(type))
                {
                    Model.Add(type, true);

                    ManuallyRegisterUweProtoMembers(type.GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance), type);
                }
            }
        }

        private bool HasUweProtoContract(Type type)
        {
            foreach(object o in type.GetCustomAttributes(false))
            {
                if (o.GetType().ToString().Contains("ProtoContractAttribute"))
                {
                    return true;
                }
            }

            return false;
        }

        private void ManuallyRegisterUweProtoMembers(MemberInfo[] info, Type type)
        {
            foreach (MemberInfo property in info)
            {
                if (!(property.DeclaringType != type))
                {
                    foreach (object customAttribute in property.GetCustomAttributes(false))
                    {
                        Type attributeType = customAttribute.GetType();

                        if (attributeType.ToString().Contains("ProtoMemberAttribute"))
                        {
                            int tag = (int)attributeType.GetProperty("Tag", BindingFlags.Public | BindingFlags.Instance).GetValue(customAttribute, new object[] { });
                            Model[type].Add(tag, property.Name);
                        }
                    }
                }
            }
        }
    }
}
