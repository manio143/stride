using Stride.Core.Yaml.Serialization.Serializers;
using Stride.Core.Annotations;
using Stride.Core.Yaml.Serialization;
using System.Reflection;
using Stride.Core.Diagnostics;

namespace Stride.Core.Yaml
{
    /// <summary>
    /// This serializer issues a warning when a member with <see cref="NotNullAttribute"/> has value null.
    /// </summary>
    public class MemberNotNullSerializer : ChainedSerializer
    {
        /// <inheritdoc/>
        public override void WriteYaml(ref ObjectContext objectContext)
        {
            if (objectContext.ParentTypeMemberDescriptor?.MemberInfo?.GetCustomAttribute<NotNullAttribute>() != null)
            {
                if (objectContext.Instance == null)
                {
                    var memberName = objectContext.ParentTypeMemberDescriptor.Name;
                    var parentTypeName = objectContext.ParentTypeDescriptor.Type.FullName;
                    objectContext.SerializerContext.Logger.Warning(
                        $"Member '{memberName}' of '{parentTypeName}' has a [NotNull] attribute, but value 'null' is being written.");
                }
            }
            base.WriteYaml(ref objectContext);
        }

        /// <inheritdoc/>
        public override object ReadYaml(ref ObjectContext objectContext)
        {
            var result = base.ReadYaml(ref objectContext);
            if (objectContext.ParentTypeMemberDescriptor?.MemberInfo?.GetCustomAttribute<NotNullAttribute>() != null)
            {
                if (result == null)
                {
                    var memberName = objectContext.ParentTypeMemberDescriptor.Name;
                    var parentTypeName = objectContext.ParentTypeDescriptor.Type.FullName;
                    objectContext.SerializerContext.Logger.Warning(
                        $"Member '{memberName}' of '{parentTypeName}' has a [NotNull] attribute, but value 'null' is being read.");
                }
            }
            return result;
        }
    }
}
