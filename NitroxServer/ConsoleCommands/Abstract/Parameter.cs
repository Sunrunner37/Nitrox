﻿using NitroxModel.Helper;

namespace NitroxServer.ConsoleCommands.Abstract
{
    public class Parameter<T> : IParameter
    {
        public TypeAbstract<T> Type { get; }
        public bool IsRequired { get; }
        public string Name { get; }

        public Parameter(TypeAbstract<T> type, string name, bool isRequired)
        {
            Validate.NotNull(type);
            Validate.IsFalse(string.IsNullOrEmpty(name));

            Type = type;
            Name = name;
            IsRequired = isRequired;
        }

        public override string ToString()
        {
            return string.Format("{0}{1}{2}",
                IsRequired ? '{' : '[',
                Name,
                IsRequired ? '}' : ']'
            );
        }
    }

    public interface IParameter
    {
        string Name { get; }
        bool IsRequired { get; }

        string ToString();
    }
}
