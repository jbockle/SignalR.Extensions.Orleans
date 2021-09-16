using System.Threading.Tasks;
using SignalR.Extensions.Orleans.Tests.Fixtures;
using Xunit;

namespace SignalR.Extensions.Orleans.Tests
{
    /// <summary>
    /// Test ISignalRNode..but really here to validate TestApiFixture can support multiple "nodes"
    /// </summary>
    public class SignalRNodeTests : UsingChatApiFixtureCollection
    {
        public SignalRNodeTests(ChatApiFixture fixture)
            : base(fixture)
        {
        }

        [Fact]
        public async Task EachNodeHasDifferentId()
        {
            var responseA = await Fixture.NodeA.Client
                .GetAsync("/node")
                .EnsureSuccessStatusCodeAsync()
                .GetResponseContentAsStringAsync();
            var responseB = await Fixture.NodeB.Client
                .GetAsync("/node")
                .EnsureSuccessStatusCodeAsync()
                .GetResponseContentAsStringAsync();

            Assert.NotEqual(responseA, responseB);
        }

        [Fact]
        public async Task EachNodeIdIsSingleton()
        {
            var responseA1 = await Fixture.NodeA.Client
                .GetAsync("/node")
                .EnsureSuccessStatusCodeAsync()
                .GetResponseContentAsStringAsync();
            var responseA2 = await Fixture.NodeA.Client
                .GetAsync("/node")
                .EnsureSuccessStatusCodeAsync()
                .GetResponseContentAsStringAsync();
            var responseB1 = await Fixture.NodeB.Client
                .GetAsync("/node")
                .EnsureSuccessStatusCodeAsync()
                .GetResponseContentAsStringAsync();
            var responseB2 = await Fixture.NodeB.Client
                .GetAsync("/node")
                .EnsureSuccessStatusCodeAsync()
                .GetResponseContentAsStringAsync();

            Assert.Equal(responseA1, responseA2);
            Assert.Equal(responseB1, responseB2);
        }
    }
}
