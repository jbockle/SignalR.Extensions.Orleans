using System;
using System.Threading;
using System.Threading.Tasks;

namespace SignalR.Extensions.Orleans.Tests
{
    public class Signaler : IDisposable
    {
        private readonly AutoResetEvent _signal;

        private Signaler()
        {
            _signal = new AutoResetEvent(false);
        }

        public static Signaler Create()
        {
            return new Signaler();
        }

        public void Signal()
        {
            _signal.Set();
        }

        public Task WaitForSignal(TimeSpan timeout = default)
        {
            if (timeout == default)
            {
                timeout = TimeSpan.FromSeconds(2);
            }

            if (!_signal.WaitOne(timeout))
            {
                throw new TimeoutException("signal was not received");
            }

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _signal?.Dispose();
        }
    }
}
