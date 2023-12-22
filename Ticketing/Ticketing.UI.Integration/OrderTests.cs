using Microsoft.AspNetCore.Mvc.Testing;

namespace Ticketing.UI.Integration
{
    public class OrderTests : IClassFixture<TicketingUiFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly TicketingUiFactory<Program> _factory;

        public OrderTests(TicketingUiFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }

        [Fact]
        public async Task OrdersRelease()
        {
            var response = await _client.DeleteAsync("api/Orders/release/1");

            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();

            Assert.Equal("true", responseString);
        }
    }
}