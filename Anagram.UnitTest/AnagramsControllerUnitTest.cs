using Anagram.Web;
using Anagram.Web.Controllers;
using Anagram.Web.Models;
using Anagram.Web.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;

namespace Anagram.UnitTest
{
    [TestClass]
    public class AnagramsControllerUnitTest
    {
        AnagramsController _controller;

        [TestInitialize]
        public void UnitTestBaseSetUp()
        {
            Mock<IConfigurationSection> configurationSectionStub = new Mock<IConfigurationSection>();
            configurationSectionStub.Setup(x => x["DefaultConnection"]).Returns("TestConnectionString");
            Mock<IConfiguration> configurationStub = new Mock<IConfiguration>();
            configurationStub.Setup(x => x.GetSection("ConnectionStrings")).Returns(configurationSectionStub.Object);

            IServiceCollection services = new ServiceCollection();
            var target = new Startup(configurationStub.Object);
            target.ConfigureServices(services);
            services.AddTransient<CombinationsService>();

            var wwwrootPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory.Replace("\\bin\\Debug\\netcoreapp2.1", string.Empty), "wwwroot");
            var mockHostingEnvironment = new Mock<IHostingEnvironment>();
            mockHostingEnvironment.SetupGet(x => x.WebRootPath).Returns(wwwrootPath);

            var serviceProvider = services.BuildServiceProvider();

            _controller = new AnagramsController(mockHostingEnvironment.Object, serviceProvider.GetService<CombinationsService>());
        }

        [TestMethod]
        public void PostValueAndUseRuDictTestMethod()
        {
            // Arrange
            var expected = new string[] { "автор", "втора", "тавро", "товар", "отвар", "рвота" };

            // Act
            var model = new AnagramModel { Value = "автор", Language = Language.RU };
            var result = _controller.Post(model);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okObjectResult = result as OkObjectResult;
            Assert.AreEqual(okObjectResult.StatusCode, 200);
            var actual = okObjectResult.Value as List<string>;
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void PostValueAndUseEnDictTestMethod()
        {
            // Arrange   
            var expected = new string[] { "paper" , "rappe" };

            // Act
            var model = new AnagramModel { Value = "paper", Language = Language.EN };
            var result = _controller.Post(model);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okObjectResult = result as OkObjectResult;
            Assert.AreEqual(okObjectResult.StatusCode, 200);
            var actual = okObjectResult.Value as List<string>;
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void PostValueAndUseInvalidDictTestMethod()
        {
            // Arrange   
            var expected = 0;

            // Act
            var model = new AnagramModel { Value = "воз", Language = Language.EN };
            var result = _controller.Post(model);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okObjectResult = result as OkObjectResult;
            Assert.AreEqual(okObjectResult.StatusCode, 200);
            var actual = okObjectResult.Value as List<string>;
            Assert.AreEqual(expected, actual.Count);
        }

        [TestMethod]
        public void PostNullValueTestMethod()
        {
            // Arrange

            // Act
            var model = new AnagramModel { Value = null, Language = Language.EN };
            var result = _controller.Post(model);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestObjectResult = result as BadRequestObjectResult;
            Assert.AreEqual(badRequestObjectResult.StatusCode, 400);
            Assert.AreEqual("Input string value can't be empty", badRequestObjectResult.Value);
        }

        [TestMethod]
        public void PostNullLanguageTestMethod()
        {
            // Arrange

            // Act
            var model = new AnagramModel { Value = "some" };
            var result = _controller.Post(model);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
            var notFoundObjectResult = result as NotFoundObjectResult;
            Assert.AreEqual(notFoundObjectResult.StatusCode, 404);
            Assert.AreEqual("Dictionary 0 file doesn't exist", notFoundObjectResult.Value);
        }

        [TestMethod]
        public void PostMultyWordValueTestMethod()
        {
            // Arrange
            var expected = new string[] { "лось" };

            // Act
            var model = new AnagramModel { Value = "соль сахар ~!@#$%^&*()_+`1234567890-=,./?\\|", Language = Language.RU };
            var result = _controller.Post(model);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okObjectResult = result as OkObjectResult;
            Assert.AreEqual(okObjectResult.StatusCode, 200);
            var actual = okObjectResult.Value as List<string>;
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void PostInvalidValueTestMethod()
        {
            // Arrange

            // Act
            var model = new AnagramModel { Value = "~!@#$%^&*()_+`1234567890-=,./?\\|", Language = Language.RU };
            var result = _controller.Post(model);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestObjectResult = result as BadRequestObjectResult;
            Assert.AreEqual(badRequestObjectResult.StatusCode, 400);
            Assert.AreEqual("Invalid string value", badRequestObjectResult.Value);
        }

        [TestMethod]
        public void PostValueWithDashTestMethod()
        {
            // Arrange
            var expected = new string[] { "лось" };

            // Act
            var model = new AnagramModel { Value = "соль-сахар", Language = Language.RU };
            var result = _controller.Post(model);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okObjectResult = result as OkObjectResult;
            Assert.AreEqual(okObjectResult.StatusCode, 200);
            var actual = okObjectResult.Value as List<string>;
            CollectionAssert.AreEqual(expected, actual); ;
        }
    }
}
