﻿using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using NitroxModel.Helper;
using ProtoBufNet;

namespace NitroxModel.DataStructures.Util
{
    /// <summary>
    ///     Used to give context on whether the wrapped value is nullable and to improve error logging.
    /// </summary>
    /// <remarks>
    ///     Used some hacks to circumvent C#' lack of reverse type inference (usually need to specify the type when returning a
    ///     value using a generic method).
    ///     Code from https://tyrrrz.me/blog/return-type-inference
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    [ProtoContract]
    public struct Optional<T> : ISerializable
    {
        [ProtoMember(1)]
        public T Value { get; private set; }

        private bool hasValue;

        [ProtoMember(2)]
        public bool HasValue
        {
            get
            {
                if (ReferenceEquals(Value, null))
                {
                    return false;
                }
                if (IsDestroyedUnityObject(Value))
                {
                    return false;
                }
                return hasValue;
            }
            set
            {
                hasValue = value;
            }
        }

        private static bool IsDestroyedUnityObject(T value)
        {
            // Check to satisfy unity objects. Sometimes they are internally destroyed but are not considered null.
            // For the purpose of optional, we consider a dead object to be the same as null.
            return NitroxUtils.IsUnityApiAvailable && GetUnityObjectInstanceID(value) == 0;
        }

        private static int? GetUnityObjectInstanceID(T value)
        {
            Func<object, int> method = ReflectionCache.InstanceMethod<int>("GetInstanceID", value);
            if (method == null)
            {
                // Not a unity object, should be higher than 0 because it's a valid instance.
                return 1;
            }

            return method.Invoke(value);
        }

        private Optional(T value)
        {
            Value = value;
            hasValue = true;
        }

        public T OrElse(T elseValue)
        {
            return HasValue ? Value : elseValue;
        }

        public Optional<TResult> IfValue<TResult>(Func<T, TResult> select)
        {
            if (HasValue)
            {
                return Optional.OfNullable(select(Value));
            }
            return Optional.Empty;
        }

        internal static Optional<T> Of(T value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value), $"Tried to set null on {typeof(Optional<T>)}");
            }

            return new Optional<T>(value);
        }

        internal static Optional<T> OfNullable(T value)
        {
            if (ReferenceEquals(value, null) || IsDestroyedUnityObject(value))
            {
                return Optional.Empty;
            }
            return new Optional<T>(value);
        }

        public override string ToString()
        {
            bool hasPtrMethod = ReflectionCache.InstanceMethod<IntPtr>("GetCachedPtr", Value) != null;
            return $"Optional<{typeof(T)}> has value: {HasValue}, is null: {Value == null}, has ptr method: {hasPtrMethod}, Unity ptr: {GetUnityObjectInstanceID(Value)}";
        }

        private Optional(SerializationInfo info, StreamingContext context)
        {
            Value = (T)info.GetValue("value", typeof(T));
            hasValue = info.GetBoolean("hasValue");
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("value", Value);
            info.AddValue("hasValue", HasValue);
        }

        public static implicit operator Optional<T>(OptionalEmpty none)
        {
            return new Optional<T>();
        }

        public static implicit operator Optional<T>?(T obj)
        {
            if (obj == null)
            {
                return null;
            }
            return new Optional<T>(obj);
        }

        public static implicit operator Optional<T>(T obj)
        {
            return Optional.Of(obj);
        }

        public static explicit operator T(Optional<T> value)
        {
            return value.Value;
        }
    }

    public struct OptionalEmpty
    {
    }

    public static class Optional
    {
        internal static readonly object[] EmptyArray = new object[0];
        internal static Func<object, int> CachedGetInstanceIDDelegate;

        public static OptionalEmpty Empty { get; } = new OptionalEmpty();

        public static Optional<T> Of<T>(T value)
        {
            return Optional<T>.Of(value);
        }

        public static Optional<T> OfNullable<T>(T value)
        {
            return Optional<T>.OfNullable(value);
        }
    }

    public sealed class OptionalNullException<T> : Exception
    {
        public OptionalNullException() : base($"Optional <{nameof(T)}> is null!")
        {
        }

        public OptionalNullException(string message) : base($"Optional <{nameof(T)}> is null:\n\t{message}")
        {
        }
    }

    [Serializable]
    public sealed class OptionalEmptyException<T> : Exception
    {
        public OptionalEmptyException() : base($"Optional <{nameof(T)}> is empty.")
        {
        }

        public OptionalEmptyException(string message) : base($"Optional <{nameof(T)}> is empty:\n\t{message}")
        {
        }
    }
}
