using System;

namespace Stride.Core.Yaml.Serialization
{
    /// <summary>
    /// Allows specifying an <see cref="IYamlSerializable"/> override for a particular member.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class MemberYamlSerializerAttribute : Attribute
    {
        /// <summary>
        /// Type of the serializer to be used.
        /// </summary>
        public Type SerializerType { get; }

        public MemberYamlSerializerAttribute(Type serializerType) => SerializerType = serializerType;
    }
}
