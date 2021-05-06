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
    public class NodeBindingAndOrderTests
    {
        private ThirdPartyApiMock _thirdPartyApiMock;
        private AppTestWebApplicationFactory<ThirdPartyApiMock> _factory;

        [SetUp]
        public void Init()
        {
            _thirdPartyApiMock = new ThirdPartyApiMock();
            _factory = new AppTestWebApplicationFactory<ThirdPartyApiMock>(_thirdPartyApiMock);
        }

        [TearDown]
        public void Dispose()
        {

        }

        [Test]
        public async Task ThatNodesBindsProperlyAlongWithTheirOrder()
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
            var nodes = _thirdPartyApiMock.GetPaths();

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.AreEqual(5, nodes.Count);
            Assert.IsTrue(nodes.ElementAt(0).Name == keyValues[1].Value); // London 
            Assert.IsTrue(nodes.ElementAt(1).Name == keyValues[3].Value); // Warsaw 
            Assert.IsTrue(nodes.ElementAt(2).Name == keyValues[0].Value); // Paris 
            Assert.IsTrue(nodes.ElementAt(3).Name == keyValues[2].Value); // Oslo
            Assert.IsTrue(nodes.ElementAt(4).Name == keyValues[4].Value); // Barcelona

            Assert.IsTrue(nodes.ElementAt(0).From.Count() == 1); // London from somewhere
            Assert.IsTrue(nodes.ElementAt(0).From.First().Name == keyValues[3].Value); // London from Warsaw

            Assert.IsTrue(nodes.ElementAt(1).From.Count() == 2); // Warsaw from two cities
            Assert.IsTrue(nodes.ElementAt(1).From.All(x => x.Name == keyValues[2].Value || x.Name == keyValues[0].Value)); // Warsaw from Oslo or Paris

            Assert.IsTrue(nodes.ElementAt(2).From.Count() == 1); // Paris from somewhere
            Assert.IsTrue(nodes.ElementAt(2).From.First().Name == keyValues[2].Value); // Paris from Oslo

            Assert.IsTrue(nodes.ElementAt(3).From.Count() == 1); // Oslo from somewhere
            Assert.IsTrue(nodes.ElementAt(3).From.First().Name == keyValues[4].Value); // Oslo from Barcelona
        }

        [Test]
        public async Task ThatNodesBindsProperlyAlongWithTheirOrderForLinearCase()
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

            keyValues.Add(new KeyValuePair<string, string>("paths[0].to", "1"));
            keyValues.Add(new KeyValuePair<string, string>("paths[1].to", "2"));
            keyValues.Add(new KeyValuePair<string, string>("paths[2].to", "3"));
            keyValues.Add(new KeyValuePair<string, string>("paths[3].to", "4"));

            var formContent = new FormUrlEncodedContent(keyValues);

            // Act
            var response = await client.PostAsync(url, formContent);
            var nodes = _thirdPartyApiMock.GetPaths();

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.AreEqual(5, nodes.Count);
            Assert.IsTrue(nodes.ElementAt(0).Name == keyValues[4].Value); // Barcelona 
            Assert.IsTrue(nodes.ElementAt(1).Name == keyValues[3].Value); // Warsaw 
            Assert.IsTrue(nodes.ElementAt(2).Name == keyValues[2].Value); // Oslo 
            Assert.IsTrue(nodes.ElementAt(3).Name == keyValues[1].Value); // London
            Assert.IsTrue(nodes.ElementAt(4).Name == keyValues[0].Value); // Paris

            Assert.IsTrue(nodes.ElementAt(0).From.Count() == 1); // Barcelona from somewhere
            Assert.IsTrue(nodes.ElementAt(0).From.First().Name == keyValues[3].Value); // Barcelona from Warsaw

            Assert.IsTrue(nodes.ElementAt(1).From.Count() == 1); // Warsaw from somewhere
            Assert.IsTrue(nodes.ElementAt(1).From.First().Name == keyValues[2].Value); // Warsaw from Oslo

            Assert.IsTrue(nodes.ElementAt(2).From.Count() == 1); // Oslo from somewhere
            Assert.IsTrue(nodes.ElementAt(2).From.First().Name == keyValues[1].Value); // Oslo from London

            Assert.IsTrue(nodes.ElementAt(3).From.Count() == 1); // London from somewhere
            Assert.IsTrue(nodes.ElementAt(3).From.First().Name == keyValues[0].Value); // London from Paris
        }

        [Test]
        public async Task ThatNodesBindsProperlyAlongWithTheirOrderForFancyLinearCase()
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

            keyValues.Add(new KeyValuePair<string, string>("paths[2].to", "3"));
            keyValues.Add(new KeyValuePair<string, string>("paths[3].to", "1"));
            keyValues.Add(new KeyValuePair<string, string>("paths[1].to", "4"));
            keyValues.Add(new KeyValuePair<string, string>("paths[4].to", "0"));

            var formContent = new FormUrlEncodedContent(keyValues);

            // Act
            var response = await client.PostAsync(url, formContent);
            var nodes = _thirdPartyApiMock.GetPaths();

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.AreEqual(5, nodes.Count);
            Assert.IsTrue(nodes.ElementAt(0).Name == keyValues[0].Value); // Paris 
            Assert.IsTrue(nodes.ElementAt(1).Name == keyValues[4].Value); // Barcelona 
            Assert.IsTrue(nodes.ElementAt(2).Name == keyValues[1].Value); // London 
            Assert.IsTrue(nodes.ElementAt(3).Name == keyValues[3].Value); // Warsaw
            Assert.IsTrue(nodes.ElementAt(4).Name == keyValues[2].Value); // Oslo

            Assert.IsTrue(nodes.ElementAt(0).From.Count() == 1); // Paris from somewhere
            Assert.IsTrue(nodes.ElementAt(0).From.First().Name == keyValues[4].Value); // Paris from Barcelona

            Assert.IsTrue(nodes.ElementAt(1).From.Count() == 1); // Barcelona from somewhere
            Assert.IsTrue(nodes.ElementAt(1).From.First().Name == keyValues[1].Value); // Barcelona from London

            Assert.IsTrue(nodes.ElementAt(2).From.Count() == 1); // London from somewhere
            Assert.IsTrue(nodes.ElementAt(2).From.First().Name == keyValues[3].Value); // London from Warsaw

            Assert.IsTrue(nodes.ElementAt(3).From.Count() == 1); // Warsaw from somewhere
            Assert.IsTrue(nodes.ElementAt(3).From.First().Name == keyValues[2].Value); // Warsaw from Oslo
        }

        [Test]
        public async Task ThatNodesBindsProperlyAlongWithTheirOrderForParalellNodes()
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

            keyValues.Add(new KeyValuePair<string, string>("paths[1].to", "0"));
            keyValues.Add(new KeyValuePair<string, string>("paths[2].to", "0"));
            keyValues.Add(new KeyValuePair<string, string>("paths[3].to", "0"));
            keyValues.Add(new KeyValuePair<string, string>("paths[4].to", "0"));

            var formContent = new FormUrlEncodedContent(keyValues);

            // Act
            var response = await client.PostAsync(url, formContent);
            var nodes = _thirdPartyApiMock.GetPaths();

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.AreEqual(5, nodes.Count);
            Assert.IsTrue(nodes.ElementAt(0).Name == keyValues[0].Value); // Paris 
            // order of rest is random

            Assert.IsTrue(nodes.ElementAt(0).From.Count() == 4); // Paris from 4 cities

            Assert.IsTrue(nodes.ElementAt(1).From.Count() == 0); // London from nowhere
            Assert.IsTrue(nodes.ElementAt(2).From.Count() == 0); // Oslo from nowhere
            Assert.IsTrue(nodes.ElementAt(3).From.Count() == 0); // Warsaw from nowhere
            Assert.IsTrue(nodes.ElementAt(4).From.Count() == 0); // Barcelona from nowhere
        }

        [Test]
        public async Task ThatNodesBindsProperlyAlongWithTheirOrderForParalellNodes2()
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

            keyValues.Add(new KeyValuePair<string, string>("paths[0].to", "1,2,3,4"));

            var formContent = new FormUrlEncodedContent(keyValues);

            // Act
            var response = await client.PostAsync(url, formContent);
            var nodes = _thirdPartyApiMock.GetPaths();

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.AreEqual(5, nodes.Count);
            // order is random, but last one is paris
            Assert.IsTrue(nodes.ElementAt(4).Name == keyValues[0].Value); // Paris 

            Assert.IsTrue(nodes.ElementAt(4).From.Count() == 0); // Paris from 4 nowhere

            Assert.IsTrue(nodes.ElementAt(0).From.Count() == 1); // Random from Paris
            Assert.IsTrue(nodes.ElementAt(0).From.First().Name == keyValues[0].Value); // Warsaw from Paris
            Assert.IsTrue(nodes.ElementAt(1).From.Count() == 1); // Random from Paris
            Assert.IsTrue(nodes.ElementAt(1).From.First().Name == keyValues[0].Value); // Warsaw from Paris
            Assert.IsTrue(nodes.ElementAt(2).From.Count() == 1); // Random from Paris
            Assert.IsTrue(nodes.ElementAt(2).From.First().Name == keyValues[0].Value); // Warsaw from Paris
            Assert.IsTrue(nodes.ElementAt(3).From.Count() == 1); // Random from Paris
            Assert.IsTrue(nodes.ElementAt(3).From.First().Name == keyValues[0].Value); // Warsaw from Paris
        }

        [Test]
        public async Task ThatNodesBindsProperlyAlongWithTheirOrderWhenTwoGraphsExsits()
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
            var nodes = _thirdPartyApiMock.GetPaths();

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.AreEqual(5, nodes.Count);
            // this has two possible orders of solutions:

            if (nodes.ElementAt(0).Name == keyValues[4].Value)
            {
                Assert.IsTrue(nodes.ElementAt(0).Name == keyValues[4].Value); // Barcelona 
                Assert.IsTrue(nodes.ElementAt(1).Name == keyValues[3].Value); // Warsaw 
                Assert.IsTrue(nodes.ElementAt(2).Name == keyValues[0].Value); // Paris 
                Assert.IsTrue(nodes.ElementAt(3).Name == keyValues[1].Value); // London
                Assert.IsTrue(nodes.ElementAt(4).Name == keyValues[2].Value); // Oslo

                // Barcelona first

                Assert.IsTrue(nodes.ElementAt(0).From.Count() == 1); // Barcelona from somewhere
                Assert.IsTrue(nodes.ElementAt(0).From.First().Name == keyValues[3].Value); // Barcelona from Warsaw


                Assert.IsTrue(nodes.ElementAt(1).From.Count() == 1); // Warsaw from somewhere
                Assert.IsTrue(nodes.ElementAt(1).From.First().Name == keyValues[0].Value); // Warsaw from Paris

                Assert.IsTrue(nodes.ElementAt(2).From.Count() == 0); // Paris from nowhere

                // then London 
                Assert.IsTrue(nodes.ElementAt(3).From.Count() == 1); // London from somewhere
                Assert.IsTrue(nodes.ElementAt(3).From.First().Name == keyValues[2].Value); // London from Oslo

                Assert.IsTrue(nodes.ElementAt(4).From.Count() == 0); // London from nowhere

            }
            else
            {
                Assert.IsTrue(nodes.ElementAt(0).Name == keyValues[1].Value); // London
                Assert.IsTrue(nodes.ElementAt(1).Name == keyValues[2].Value); // Oslo
                Assert.IsTrue(nodes.ElementAt(2).Name == keyValues[4].Value); // Barcelona 
                Assert.IsTrue(nodes.ElementAt(3).Name == keyValues[3].Value); // Warsaw 
                Assert.IsTrue(nodes.ElementAt(4).Name == keyValues[0].Value); // Paris 

                // London first
                Assert.IsTrue(nodes.ElementAt(0).From.Count() == 1); // London from somewhere
                Assert.IsTrue(nodes.ElementAt(0).From.First().Name == keyValues[2].Value); // London from Oslo

                Assert.IsTrue(nodes.ElementAt(1).From.Count() == 0); // Oslo from nowhere

                //then Barcelona
                Assert.IsTrue(nodes.ElementAt(2).From.Count() == 1); // Barcelona from somewhere
                Assert.IsTrue(nodes.ElementAt(2).From.First().Name == keyValues[3].Value); // Barcelona from Warsaw


                Assert.IsTrue(nodes.ElementAt(3).From.Count() == 1); // Warsaw from somewhere
                Assert.IsTrue(nodes.ElementAt(3).From.First().Name == keyValues[0].Value); // Warsaw from Paris

                Assert.IsTrue(nodes.ElementAt(4).From.Count() == 0); // Paris from nowhere

            }

        }

        [Test]
        public async Task ThatTestCaseFromReadmePasses()
        {
            // Arrange
            var client = _factory.CreateClient();
            var url = "api/paths";

            var keyValues = new List<KeyValuePair<string, string>>();
            keyValues.Add(new KeyValuePair<string, string>("locations[0].Name", "New york"));
            keyValues.Add(new KeyValuePair<string, string>("locations[1].Name", "London"));
            keyValues.Add(new KeyValuePair<string, string>("locations[2].Name", "Moscow"));
            keyValues.Add(new KeyValuePair<string, string>("locations[3].Name", "Tokyo"));

            keyValues.Add(new KeyValuePair<string, string>("paths[2].to", "1"));
            keyValues.Add(new KeyValuePair<string, string>("paths[2].to", "0"));
            keyValues.Add(new KeyValuePair<string, string>("paths[3].to", "2"));

            var formContent = new FormUrlEncodedContent(keyValues);

            // Act
            var response = await client.PostAsync(url, formContent);
            var nodes = _thirdPartyApiMock.GetPaths();

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.AreEqual(4, nodes.Count);
            // this has two possible orders of solutions:

            //Tokyo, Moscow. [London/New York]
            if (nodes.ElementAt(0).Name == keyValues[1].Value)
            {
                // London first
                Assert.IsTrue(nodes.ElementAt(0).Name == keyValues[0].Value); // London
                Assert.IsTrue(nodes.ElementAt(1).Name == keyValues[1].Value); // New york
                Assert.IsTrue(nodes.ElementAt(2).Name == keyValues[2].Value); // Moscow
                Assert.IsTrue(nodes.ElementAt(3).Name == keyValues[3].Value); // Tokyo


                Assert.IsTrue(nodes.ElementAt(0).From.Count() == 1); // London from somewhere
                Assert.IsTrue(nodes.ElementAt(0).From.First().Name == keyValues[2].Value); // London from Moscow

                Assert.IsTrue(nodes.ElementAt(1).From.Count() == 1); // New york from somewhere
                Assert.IsTrue(nodes.ElementAt(1).From.First().Name == keyValues[2].Value); // New york from Moscow

                Assert.IsTrue(nodes.ElementAt(2).From.Count() == 1); // Moscow from somewhere
                Assert.IsTrue(nodes.ElementAt(2).From.First().Name == keyValues[3].Value); // Moscow from Tokyo

            }
            else
            {
                // New york first
                Assert.IsTrue(nodes.ElementAt(0).Name == keyValues[0].Value); // New york
                Assert.IsTrue(nodes.ElementAt(1).Name == keyValues[1].Value); // London
                Assert.IsTrue(nodes.ElementAt(2).Name == keyValues[2].Value); // Moscow
                Assert.IsTrue(nodes.ElementAt(3).Name == keyValues[3].Value); // Tokyo


                Assert.IsTrue(nodes.ElementAt(0).From.Count() == 1); // New york from somewhere
                Assert.IsTrue(nodes.ElementAt(0).From.First().Name == keyValues[2].Value); // New york from Moscow

                Assert.IsTrue(nodes.ElementAt(1).From.Count() == 1); // London from somewhere
                Assert.IsTrue(nodes.ElementAt(1).From.First().Name == keyValues[2].Value); // London from Moscow

                Assert.IsTrue(nodes.ElementAt(2).From.Count() == 1); // Moscow from somewhere
                Assert.IsTrue(nodes.ElementAt(2).From.First().Name == keyValues[3].Value); // Moscow from Tokyo
            }

        }
    }
}