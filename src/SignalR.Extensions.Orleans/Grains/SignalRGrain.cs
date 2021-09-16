using System;
using System.Threading.Tasks;
using Orleans;
using Orleans.Concurrency;
using Orleans.Streams;
using SignalR.Extensions.Orleans.Streams;

namespace SignalR.Extensions.Orleans.Grains
{
    public interface ISignalRGrain : IGrainWithIntegerKey
    {
        [AlwaysInterleave]
        Task SendToNodes(Message message);
    }

    public class SignalRGrain : Grain, ISignalRGrain
    {
        private IAsyncStream<Message> _stream = default!;

        public override Task OnActivateAsync()
        {
            var streamProvider = GetStreamProvider(SignalROrleansSdk.Constants.STREAM_PROVIDER);
            _stream = streamProvider.GetStream<Message>(Guid.Empty, SignalROrleansSdk.Constants.SIGNALR_GRAIN_STREAM);

            return Task.CompletedTask;
        }

        public Task SendToNodes(Message message)
        {
            return _stream.OnNextAsync(message);
        }
    }
}