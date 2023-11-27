using Application.Cache;
using Application.Dto;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Moq.AutoMock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Tests.ServicesTest
{
    [TestClass]
    public class WeatherCacheTest
    {
        private AutoMocker autoMocker = new AutoMocker();
        private IWeatherCacheService mocked_weatherCacheService;
        private IWeatherCacheService concrete_weatherCacheService;

        private const string LATITUDE = "34.0754";
        private const string LONGITUDE = "-84.2941";

        //Because many of the methods on IMemoryCache are extension methods, and you cannot use
        //extension methods in the .Setup of an autoMocker, we have to use an actual concrete implementation.
        public WeatherCacheTest()
        {
            var services = new ServiceCollection();
            services.AddMemoryCache();
            var serviceProvider = services.BuildServiceProvider();

            var memoryCache = serviceProvider.GetService<IMemoryCache>();
            concrete_weatherCacheService = new WeatherCacheService(memoryCache);
        }

        [TestInitialize]
        public void Init()
        {
            mocked_weatherCacheService = autoMocker.CreateInstance<WeatherCacheService>();
        }

        [TestMethod]
        public void Given_KeyDoesNotExistInCache_NoRemovalOccurs()
        {
            var key = new CacheKey(LATITUDE, LONGITUDE);

            mocked_weatherCacheService.DeleteForecast(key);

            autoMocker.GetMock<IMemoryCache>()
                .Verify(x => x.Remove(key.GetCacheKey()), Times.Never());
        }

        [TestMethod]
        public void Given_WeatherForecastDto_SaveSuccessfullyToCache()
        {
            var dto = new WeatherForecastDto()
            {
                Latitude = LATITUDE, Longitude = LONGITUDE
            };

            concrete_weatherCacheService.SaveForecast(dto);

            var key = new CacheKey(LATITUDE, LONGITUDE);

            var fromCache = concrete_weatherCacheService.GetForecastDto(key);

            Assert.IsTrue(fromCache != null);
            Assert.IsTrue(fromCache.Latitude == dto.Latitude && fromCache.Longitude == dto.Longitude);
        }

        [TestMethod]
        public void Given_WeatherForecastDto_DoesNotExistInCache_ReturnsNull()
        {
            var key = new CacheKey("blah", "blah");

            var result = concrete_weatherCacheService.GetForecastDto(key);

            Assert.IsTrue(result == null);
        }
    }
}
