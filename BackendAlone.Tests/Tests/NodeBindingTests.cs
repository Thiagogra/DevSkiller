using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using BackendAlone.Tests.Startup;
using BackendAlone.Tests.ThirdPartyApi;

namespace BackendAlone.Tests
{
    [TestFixture]
    public class NodeBindingTests
    {
        private ThirdPartyApiNaiveMock _thirdPartyApiNaiveMock;
        private AppTestWebApplicationFactory<ThirdPartyApiNaiveMock> _factory;

        [SetUp]
        public void Init()
        {
            _thirdPartyApiNaiveMock = new ThirdPartyApiNaiveMock();
            _factory = new AppTestWebApplicationFactory<ThirdPartyApiNaiveMock>(_thirdPartyApiNaiveMock);
        }

        [TearDown]
        public void Dispose()
        {

        }

        [Test]
        public async Task ThatNodesBindsProperly()
        {
            // Arrange
            var client = _factory.CreateClient();
            var url = "api/paths";

            var keyValues = new List<KeyValuePair<string, string>>();
            keyValues.Add(new KeyValuePair<string, string>("locations[0].Name", "Paris"));
            keyValues.Add(new KeyValuePair<string, string>("locations[1].Name", "London"));
            keyValues.Add(new KeyValuePair<string, string>("locations[2].Name", "Oslo"));
            keyValues.Add(new KeyValuePair<string, string>("locations[3].Name", "Warsaw"));
            keyValues.Add(new KeyValuePair<string, string>("locations[4].Name", "Barcelona"));

            keyValues.Add(new KeyValuePair<string, string>("paths[3].to", "1"));
            keyValues.Add(new KeyValuePair<string, string>("paths[0].to", "3"));
            keyValues.Add(new KeyValuePair<string, string>("paths[2].to", "3,0"));
            keyValues.Add(new KeyValuePair<string, string>("paths[4].to", "2"));

            var formContent = new FormUrlEncodedContent(keyValues);

            // Act
            var response = await client.PostAsync(url, formContent);
            var nodes = _thirdPartyApiNaiveMock.GetPaths();

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.AreEqual(5, nodes.Count);
            Assert.IsTrue(nodes.Any(x => x.Name == keyValues[0].Value));
            Assert.IsTrue(nodes.Any(x => x.Name == keyValues[1].Value));
            Assert.IsTrue(nodes.Any(x => x.Name == keyValues[2].Value));
            Assert.IsTrue(nodes.Any(x => x.Name == keyValues[3].Value));
            Assert.IsTrue(nodes.Any(x => x.Name == keyValues[4].Value));
        }

        [Test]
        public async Task ThatNodesBindsProperlyWhenTwoGraphsExsits()
        {
            // Arrange
            var client = _factory.CreateClient();
            var url = "api/paths";

            var keyValues = new List<KeyValuePair<string, string>>();
            keyValues.Add(new KeyValuePair<string, string>("locations[0].Name", "Paris"));
            keyValues.Add(new KeyValuePair<string, string>("locations[1].Name", "London"));
            keyValues.Add(new KeyValuePair<string, string>("locations[2].Name", "Oslo"));
            keyValues.Add(new KeyValuePair<string, string>("locations[3].Name", "Warsaw"));
            keyValues.Add(new KeyValuePair<string, string>("locations[4].Name", "Barcelona"));

            keyValues.Add(new KeyValuePair<string, string>("paths[3].to", "4"));
            keyValues.Add(new KeyValuePair<string, string>("paths[0].to", "3"));
            keyValues.Add(new KeyValuePair<string, string>("paths[2].to", "1"));

            var formContent = new FormUrlEncodedContent(keyValues);

            // Act
            var response = await client.PostAsync(url, formContent);
            var nodes = _thirdPartyApiNaiveMock.GetPaths();

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.AreEqual(5, nodes.Count);
            Assert.IsTrue(nodes.Any(x => x.Name == keyValues[0].Value));
            Assert.IsTrue(nodes.Any(x => x.Name == keyValues[1].Value));
            Assert.IsTrue(nodes.Any(x => x.Name == keyValues[2].Value));
            Assert.IsTrue(nodes.Any(x => x.Name == keyValues[3].Value));
            Assert.IsTrue(nodes.Any(x => x.Name == keyValues[4].Value));

        }

        [Test]
        public async Task ThatNodesBindsProperlyWhenNoIndicesExist()
        {
            // Arrange
            var client = _factory.CreateClient();
            var url = "api/paths";

            var keyValues = new List<KeyValuePair<string, string>>();
            keyValues.Add(new KeyValuePair<string, string>("locations[0].Name", "Paris"));
            keyValues.Add(new KeyValuePair<string, string>("locations[1].Name", "London"));
            keyValues.Add(new KeyValuePair<string, string>("locations[2].Name", "Oslo"));
            keyValues.Add(new KeyValuePair<string, string>("locations[3].Name", "Warsaw"));
            keyValues.Add(new KeyValuePair<string, string>("locations[4].Name", "Barcelona"));

            var formContent = new FormUrlEncodedContent(keyValues);

            // Act
            var response = await client.PostAsync(url, formContent);
            var nodes = _thirdPartyApiNaiveMock.GetPaths();

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.AreEqual(5, nodes.Count);
            Assert.IsTrue(nodes.Any(x => x.Name == keyValues[0].Value));
            Assert.IsTrue(nodes.Any(x => x.Name == keyValues[1].Value));
            Assert.IsTrue(nodes.Any(x => x.Name == keyValues[2].Value));
            Assert.IsTrue(nodes.Any(x => x.Name == keyValues[3].Value));
            Assert.IsTrue(nodes.Any(x => x.Name == keyValues[4].Value));
        }

        [Test]
        public async Task ThatNodesBindsProperlyWhenNoNodesExist()
        {
            // Arrange
            var client = _factory.CreateClient();
            var url = "api/paths";

            var keyValues = new List<KeyValuePair<string, string>>();

            var formContent = new FormUrlEncodedContent(keyValues);

            // Act
            var response = await client.PostAsync(url, formContent);
            var nodes = _thirdPartyApiNaiveMock.GetPaths();

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.AreEqual(0, nodes.Count);
        }
    }
}