using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SupportApi.Controllers;
using SupportApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SupportApi.Tests
{
    public class BasicTests : IClassFixture<WebApplicationFactory<SupportApi.Startup>>
    {
        private readonly WebApplicationFactory<SupportApi.Startup> _factory;
        public BasicTests(WebApplicationFactory<SupportApi.Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Get_EndpointsReturnSuccessAndCorrectContentType()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response1 = await client.GetAsync("api/ticket/generate");
            var response2 = await client.GetAsync("api/ticket/");

            // Assert
            response1.EnsureSuccessStatusCode();
            response2.EnsureSuccessStatusCode();
            Assert.Equal("application/json; charset=utf-8", response2.Content.Headers.ContentType.ToString());

            var responseString = await response2.Content.ReadAsStringAsync();
            var deserialized = JsonConvert.DeserializeObject<IEnumerable<TicketItem>>(responseString);
            Assert.True(deserialized.Any());
        }
    }

    public class UnitTest1
    {
        [Fact]
        public async Task Test1()
        {
            // Arrange (ülesseadmine)
            var builder = new DbContextOptionsBuilder<TicketContext>()
                .UseInMemoryDatabase("TicketList");
            var context = new TicketContext(builder.Options);
            var controller = new TicketController(context);
            var tTicket = new TicketItem() { };

            // Act (käivitad "system under test")
            var returnVal = await controller.PostTicketItem(tTicket);

            // Assert (vaatad et tulemus oleks nii nagu peab)
            var newContext = new TicketContext(builder.Options);

            var ticketFromDatabase = newContext.TicketItems.Single();
            Assert.True(newContext.TicketItems.Contains(tTicket)); // bugine tegelt... referencide teema

            Assert.IsType<CreatedAtActionResult>(returnVal.Result);

            var returnVal2 = returnVal.Result as CreatedAtActionResult;
            Assert.True(returnVal2.Value.Equals(tTicket));
        }
        [Fact]
        public async Task Test2()
        {
            // Arrange (ülesseadmine)
            var builder = new DbContextOptionsBuilder<TicketContext>()
                .UseInMemoryDatabase("TicketList");
            var context = new TicketContext(builder.Options);
            var controller = new TicketController(context);
            var tTicket = new TicketItem() { Id = 123 };
            await controller.PostTicketItem(tTicket);

            tTicket.query = "test";

            // Act (käivitad "system under test")
            var badRequestResult = await controller.PutTicketItem(999, tTicket);
            var correctResult = await controller.PutTicketItem(123, tTicket);


            // Assert
            Assert.IsType<BadRequestResult>(badRequestResult);
            var newContext = new TicketContext(builder.Options);

            var ticketFromDatabase = newContext.TicketItems.Single();
            Assert.True(ticketFromDatabase.query == "test");

            Assert.IsType<NoContentResult>(correctResult);

            
        }
        [Fact]
        public async Task Test3()
        {
            // Arrange (ülesseadmine)
            var builder = new DbContextOptionsBuilder<TicketContext>()
                .UseInMemoryDatabase("TicketList");
            var context = new TicketContext(builder.Options);
            var controller = new TicketController(context);
            var deletedTicket = new TicketItem() { Id = 123 };
            await controller.PostTicketItem(deletedTicket);
            await controller.PostTicketItem(new TicketItem() { Id = 555 });

            // Act (käivitad "system under test")
            var deleteRequest = await controller.DeleteTicketItem(deletedTicket.Id);
            var wrongKeyRequest = await controller.DeleteTicketItem(99);


            // Assert
            Assert.False(context.TicketItems.Any(x => x.Id == deletedTicket.Id));
            Assert.IsType<NotFoundResult>(wrongKeyRequest);
            Assert.IsType<NoContentResult>(deleteRequest);

        }
    }
}
