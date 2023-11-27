using Application.Cache;
using Application.Dto;
using Application.Service;
using Domain;
using Domain.Keys;
using Domain.Services;
using Moq;
using Moq.AutoMock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.ServicesTest
{
    [TestClass]
    public class WeatherServiceTest
    {

        private AutoMocker autoMocker = new AutoMocker();
        private IWeatherService weatherService;

        private const string LATITUDE = "34.0754";
        private const string LONGITUDE = "-84.2941";

        [TestInitialize]
        public void Init()
        {
            weatherService = autoMocker.CreateInstance<WeatherService>();
        }

        [TestMethod]
        public async Task Given_NoWeatherForecastFound_DoesNotCallDeleteMethod()
        {
            var key = LatLongKey.Key(LATITUDE, LONGITUDE);
            WeatherForecast forecast = null;

            autoMocker.GetMock<IWeatherDomainService>()
                .Setup(x => x.GetWeatherForecastAsync(key))
                .ReturnsAsync(forecast);

            await weatherService.DeleteWeatherForecastAsync(LATITUDE, LONGITUDE).ConfigureAwait(false);

            autoMocker.GetMock<IWeatherDomainService>()
                .Verify(x => x.DeleteWeatherForecastAsync(key), Times.Never());         
        }

        [TestMethod]
        public async Task Given_WeatherForecastExists_DoesCallDeleteMethod()
        {
             var key = LatLongKey.Key(LATITUDE, LONGITUDE);
            WeatherForecast forecast = new()
            {
                Latitude = LATITUDE,
                Longitude = LONGITUDE
            };

            autoMocker.GetMock<IWeatherDomainService>()
                .Setup(x => x.GetWeatherForecastAsync(key))
                .ReturnsAsync(forecast);

            autoMocker.GetMock<IWeatherDomainService>()
                .Setup(x => x.DeleteWeatherForecastAsync(key))
                .Returns(Task.CompletedTask);

            await weatherService.DeleteWeatherForecastAsync(LATITUDE, LONGITUDE).ConfigureAwait(false);

            autoMocker.GetMock<IWeatherDomainService>()
                .Verify(x => x.DeleteWeatherForecastAsync(key), Times.Once());     
            autoMocker.GetMock<IWeatherCacheService>()
                .Verify(x => x.DeleteForecast(It.IsAny<CacheKey>()), Times.Once());    
        }

        [TestMethod]
        public async Task Given_CachedValueForWeatherForecastExists_ReturnsCachedValue()
        {
            var key = LatLongKey.Key(LATITUDE, LONGITUDE);
            SetupMockedCache_ReturnsValue();

            var result = await weatherService.GetWeatherForecastAsync(LATITUDE, LONGITUDE).ConfigureAwait(false);

            Assert.IsTrue(result != null);
            Assert.IsTrue(result.Latitude == LATITUDE && result.Longitude == LONGITUDE);

            autoMocker.GetMock<IWeatherDomainService>()
                .Verify(x => x.GetWeatherForecastAsync(key), Times.Never());
        }    

        [TestMethod]
        public async Task Given_CachedValueForWeather_DoesNotExist_ReturnsValueFromDb()
        {
            var key = LatLongKey.Key(LATITUDE, LONGITUDE);
            SetupMockedCache_ReturnsNULLValue();
            autoMocker.GetMock<IWeatherDomainService>()
                .Setup(x => x.GetWeatherForecastAsync(key))
                .ReturnsAsync(new WeatherForecast()
                {
                    Hourly = new()
                    {
                        Times = new List<DateTime>(),
                        Temperatures = new List<double>()
                    },
                    Timezone = "GMT",
                    HourlyUnits = new(),
                    Latitude = LATITUDE,
                    Longitude = LONGITUDE
                });

            var result = await weatherService.GetWeatherForecastAsync(LATITUDE, LONGITUDE).ConfigureAwait(false);

            Assert.IsTrue(result != null);
            Assert.IsTrue(result.Latitude == LATITUDE && result.Longitude == LONGITUDE);

            autoMocker.GetMock<IWeatherDomainService>()
                .Verify(x => x.GetWeatherForecastAsync(key), Times.Once());
        }


        [TestMethod]
        public async Task Given_CachedValueForWeatherForecastExists_ItWillNotSaveToDb()
        {
            SetupMockedCache_ReturnsValue();

            var result = await weatherService.SaveWeatherForecastAync(LATITUDE,LONGITUDE).ConfigureAwait(false);
            
            Assert.IsTrue(result != null);
            Assert.IsTrue(result.Latitude == LATITUDE && result.Longitude == LONGITUDE);
        }

        [TestMethod]
        public async Task Given_CachedValueForWeatherForecast_DoesNotExist_ItWillSaveToDb()
        {
            SetupMockedCache_ReturnsNULLValue();

            var result = await weatherService.SaveWeatherForecastAync(LATITUDE, LONGITUDE).ConfigureAwait(false);

            Assert.IsTrue(result != null);
            Assert.IsTrue(result.Latitude == LATITUDE && result.Longitude == LONGITUDE);

            autoMocker.GetMock<IWeatherDomainService>()
                .Verify(x => x.SaveWeatherForecastAsync(It.IsAny<WeatherForecast>()), Times.Once());

            autoMocker.GetMock<IWeatherCacheService>()
                .Verify(x => x.SaveForecast(It.IsAny<WeatherForecastDto>()), Times.Once());
        }

        private void SetupMockedCache_ReturnsNULLValue()
        {
            autoMocker.GetMock<IWeatherCacheService>()
                            .Setup(x => x.GetForecastDto(It.Is<CacheKey>(
                                z => z.Latitude == LATITUDE &&
                                z.Longitude == LONGITUDE))).Returns<WeatherForecastDto>(null);
        }

        private void SetupMockedCache_ReturnsValue()
        {
            autoMocker.GetMock<IWeatherCacheService>()
                            .Setup(x => x.GetForecastDto(It.Is<CacheKey>(
                                z => z.Latitude == LATITUDE &&
                                z.Longitude == LONGITUDE))).Returns(new WeatherForecastDto()
                                {
                                    Hourly = new()
                                    {
                                        Times = new List<string>(),
                                        Temperatures = new List<double>()
                                    },
                                    Timezone = "GMT",
                                    HourlyUnits = new(),
                                    Latitude = LATITUDE,
                                    Longitude = LONGITUDE
                                });
        }
    }
}
