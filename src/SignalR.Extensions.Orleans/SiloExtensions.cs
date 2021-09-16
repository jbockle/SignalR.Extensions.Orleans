
// ReSharper disable CheckNamespace

using Package = SignalR.Extensions.Orleans;

namespace Orleans.Hosting
{
    public static class SiloExtensions
    {
        public static ISiloBuilder UseSignalR(this ISiloBuilder builder)
        {
            return builder
                .AddSimpleMessageStreamProvider(Package.SignalROrleansSdk.Constants.STREAM_PROVIDER, options =>
                {
                    options.FireAndForgetDelivery = true;
                })
                .ConfigureApplicationParts(parts => parts.AddApplicationPart(Package.SignalROrleansSdk.Assembly).WithReferences())
                ;
        }

        public static ISiloHostBuilder UseSignalR(this ISiloHostBuilder builder)
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
