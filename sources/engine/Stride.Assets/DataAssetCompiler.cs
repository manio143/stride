using System;
using System.Threading.Tasks;
using Stride.Core;
using Stride.Core.Assets;
using Stride.Core.Assets.Compiler;
using Stride.Core.BuildEngine;
using Stride.Core.Serialization.Contents;

namespace Stride.Assets
{
    /// <summary>
    /// A compiler of <see cref="DataAsset{TData}"/> which extracts the underlying data and saves it under the same URL.
    /// </summary>
    [AssetCompiler(typeof(DataAsset<>), typeof(AssetCompilationContext))]
    public sealed class DataAssetCompiler : AssetCompilerBase
    {
        protected override void Prepare(AssetCompilerContext context, AssetItem assetItem, string targetUrlInStorage, AssetCompilerResult result)
        {
            var asset = assetItem.Asset;
            var genericArgs = asset.GetType().GetGenericArguments();
            var commandType = typeof(DataAssetCommand<>).MakeGenericType(genericArgs);

            var buildStep = (Command)Activator.CreateInstance(commandType, targetUrlInStorage, asset, assetItem.Package);

            result.BuildSteps = new AssetBuildStep(assetItem);
            result.BuildSteps.Add(buildStep);
        }

    }

    internal class DataAssetCommand<TData> : AssetCommand<DataAsset<TData>>
        where TData : IDataAsset
    {
        public DataAssetCommand(string url, DataAsset<TData> parameters, IAssetFinder assetFinder) : base(url, parameters, assetFinder)
        {
        }

        protected override Task<ResultStatus> DoCommandOverride(ICommandContext commandContext)
        {
            var assetManager = new ContentManager(MicrothreadLocalDatabases.ProviderService);

            var data = Parameters.Converter.Convert(Parameters.Data);

            assetManager.Save(Url, data);

            return Task.FromResult(ResultStatus.Successful);
        }
    }
}
