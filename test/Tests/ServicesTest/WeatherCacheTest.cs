using Application.Dto;
using MeteoWeatherAPI.Dto;
using MeteoWeatherAPI.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Moq.AutoMock;

namespace Tests.ServicesTest;

[TestClass]
public class WeatherCacheTest
{
    private AutoMocker autoMocker = new AutoMocker();
    private IWeatherService sut;
    //private IWeatherService concrete_weatherCacheService;

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
        //concrete_weatherCacheService = new WeatherCacheService(memoryCache);
    }

    [TestInitialize]
    public void Init()
    {
        sut = autoMocker.CreateInstance<WeatherCacheService>();
    }

    [TestMethod]
    public void Given_KeyDoesNotExistInCache_NoRemovalOccurs()
    {
        var key = new CacheKey(LATITUDE, LONGITUDE);

        sut.DeleteWeatherForecastAsync(LATITUDE, LONGITUDE);

        autoMocker.GetMock<IMemoryCache>()
            .Verify(x => x.Remove(key.GetCacheKey()), Times.Never());
    }

    [TestMethod]
    public async Task Given_WeatherForecastDto_SaveSuccessfullyToCache()
    {
        //// Arrange
        //var dto = new WeatherForecastDto()
        //{
        //    Latitude = LATITUDE, Longitude = LONGITUDE
        //};

        //// Save the record
        //await sut.SaveWeatherForecastAync(LATITUDE, LONGITUDE);

        //// Act


        //concrete_weatherCacheService.SaveForecast(dto);

        //var key = new CacheKey(LATITUDE, LONGITUDE);

        //var fromCache = concrete_weatherCacheService.GetForecastDto(key);

        //Assert.IsTrue(fromCache != null);
        //Assert.IsTrue(fromCache.Latitude == dto.Latitude && fromCache.Longitude == dto.Longitude);
    }

    [TestMethod]
    public async Task Given_WeatherForecastDto_DoesNotExistInCache_ReturnsNull()
    {
        // Arrange

        // Act
        var result = await sut.GetWeatherForecastAsync("blah", "blah");

        // Assert
        Assert.IsTrue(result == null);

        var weatherService = autoMocker.GetMock<IWeatherService>();
        weatherService.Verify(x => x.GetWeatherForecastAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }
}