using Stride.Core.Annotations;
using Stride.Core.Yaml.Serialization;
using Stride.Core.Yaml.Serialization.Serializers;

namespace Stride.Core.Assets
{
    /// <summary>
    /// A data asset for custom user classes.
    /// </summary>
    /// <typeparam name="TData">Serializable type.</typeparam>
    [AssetDescription(FileExtension, AllowArchetype = true)]
    [AssetFormatVersion("Stride", CurrentVersion, "1.0.0.0")]
    public sealed class DataAsset<TData> : Asset where TData : IDataAsset
    {
        private const string CurrentVersion = "1.0.0.0";
        public const string FileExtension = ".sddata";

        /// <summary>
        /// Value of the associated data.
        /// </summary>
        [DataMember(DataMemberMode.Assign)]
        [InlineProperty, MemberRequired]
        [MemberYamlSerializer(typeof(ObjectSerializer))]
        public TData Data { get; set; }
    }
}
