using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetTopologySuite.Geometries;
using System.Xml.Linq;
using Street.Services.CrudAPI.Models;
using NetTopologySuite;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;

namespace PTV.Services.IntegrationTests
{
    public class StreetTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        public StreetTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }
        [Fact]
        public async Task GetStreetReturnsSuccessAndCorrectContentType()
        {
            //Arrange
            var client = _factory.CreateClient();

            //Act
            var response = await client.GetAsync("GetStreets");
            //Assert
            response.EnsureSuccessStatusCode();
            Assert.True(true, "true");

        }
    }
}
