using Orleans.Hosting;
using Package = SignalR.Extensions.Orleans;

// ReSharper disable CheckNamespace

namespace Orleans
{
    public static class ClientExtensions
    {
        public static IClientBuilder UseSignalR(this IClientBuilder builder)
        {
            return builder
                .AddSimpleMessageStreamProvider(Package.SignalROrleansSdk.Constants.STREAM_PROVIDER, options =>
                {
                    options.FireAndForgetDelivery = true;
                })
                .ConfigureApplicationParts(parts => parts.AddApplicationPart(Package.SignalROrleansSdk.Assembly).WithReferences())
                ;
        }
    }
}