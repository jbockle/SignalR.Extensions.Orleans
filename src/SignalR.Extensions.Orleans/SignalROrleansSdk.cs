using System.Reflection;

namespace SignalR.Extensions.Orleans
{
    public static class SignalROrleansSdk
    {
        internal static Assembly Assembly { get; } = typeof(SignalROrleansSdk).Assembly;

        public static class Constants
        {
            public const string STREAM_PROVIDER = "SIGNALR";

            public const string SIGNALR_GRAIN_STREAM = "SIGNALR_GRAIN_STREAM";
        }
    }
}
