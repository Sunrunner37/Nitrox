﻿using System.Linq;
using System.Diagnostics;
using System;
using System.Collections;
using System.Reflection;
using NitroxModel.DataStructures.Util;

namespace NitroxModel.Helper
{
    public static class Validate
    {
        public static void NotNull<T>(T o)
            // Prevent non-nullable valuetypes from getting boxed to object.
            // In other words: Error when trying to assert non-null on something that can't be null in the first place.
            where T : class
        {
            if (o == null)
            {
                Optional<string> paramName = GetParameterName<T>();
                if (paramName.IsPresent())
                {
                    throw new ArgumentNullException(paramName.Get());
                }
                else
                {
                    throw new ArgumentNullException();
                }
            }
        }

        public static void NotNull<T>(T o, string message)
            where T : class
        {
            if (o == null)
            {
                Optional<string> paramName = GetParameterName<T>();
                if (paramName.IsPresent())
                {
                    throw new ArgumentNullException(paramName.Get(), message);
                }
                else
                {
                    throw new ArgumentNullException(message);
                }
            }
        }

        public static void IsTrue(bool b)
        {
            if (!b)
            {
                throw new ArgumentException();
            }
        }

        public static void IsTrue(bool b, string message)
        {
            if (!b)
            {
                throw new ArgumentException(message);
            }
        }

        public static void IsFalse(bool b)
        {
            if (b)
            {
                throw new ArgumentException();
            }
        }

        public static void IsFalse(bool b, string message)
        {
            if (b)
            {
                throw new ArgumentException(message);
            }
        }

        public static void IsPresent<T>(Optional<T> opt)
        {
            if (opt.IsEmpty())
            {
                throw new OptionalEmptyException<T>();
            }
        }

        public static void IsPresent<T>(Optional<T> opt, string message)
        {
            if (opt.IsEmpty())
            {
                throw new OptionalEmptyException<T>(message);
            }
        }

        private static Optional<string> GetParameterName<TParam>()
        {
            ParameterInfo[] parametersOfMethodBeforeValidate = new StackFrame(2).GetMethod().GetParameters();
            return Optional<string>.OfNullable(parametersOfMethodBeforeValidate.SingleOrDefault(pi => pi.ParameterType == typeof(TParam))?.Name);
        }

        public static void NotNullOrEmpty(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                Optional<string> paramName = GetParameterName<string>();
                if (paramName.IsPresent())
                {
                    throw new ArgumentNullException(paramName.Get());
                }
                else
                {
                    throw new ArgumentNullException();
                }
            }
        }

        public static void NotNullOrWhitespace(string value)
        {
            if (string.IsNullOrEmpty(value) || value.All(c => c == ' '))
            {
                Optional<string> paramName = GetParameterName<string>();
                if (paramName.IsPresent())
                {
                    throw new ArgumentNullException(paramName.Get());
                }
                else
                {
                    throw new ArgumentNullException();
                }
            }
        }

        public static void NotNullOrEmpty<T>(T value) where T : ICollection
        {
            if (value == null || value.Count > 0)
            {
                Optional<string> paramName = GetParameterName<T>();
                if (paramName.IsPresent())
                {
                    throw new ArgumentNullException(paramName.Get());
                }
                else
                {
                    throw new ArgumentNullException();
                }
            }
        }
    }
}
