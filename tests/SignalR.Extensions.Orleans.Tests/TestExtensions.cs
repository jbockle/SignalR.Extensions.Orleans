using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using SignalR.Extensions.Orleans.Tests.Fixtures;

namespace SignalR.Extensions.Orleans.Tests
{
    public static class TestExtensions
    {
        public static async Task<HttpResponseMessage> EnsureSuccessStatusCodeAsync(this Task<HttpResponseMessage> responseTask)
        {
            var response = await responseTask;

            return response.EnsureSuccessStatusCode();
        }

        public static async Task<string> GetResponseContentAsStringAsync(this Task<HttpResponseMessage> responseTask)
        {
            var response = await responseTask;

            return await response.Content.ReadAsStringAsync();
        }

        public static IEnumerable<HubConnectionDetail> CreateHubConnections(
            this ChatApiNodeFixture apiFixture,
            params string[] users)
        {
            return users.Select(user => new HubConnectionDetail(apiFixture.NodeId, user)
            {
                HubConnection = apiFixture.CreateHubConnection(user),
            });
        }

        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            var list = collection.ToList();
            list.ForEach(action);

            return list;
        }

        public static IEnumerable<T> DropIndex<T>(this List<T> collection, int index)
        {
            for (int i = 0; i < collection.Count; i++)
            {
                if (i == index)
                {
                    continue;
                }

                yield return collection[i];
            }
        }
    }
}
