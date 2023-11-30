using MeteoWeatherAPI.Dto;
using MeteoWeatherAPI.Services;
using Moq;
using Moq.AutoMock;

namespace Tests.ServicesTest;

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

        autoMocker.GetMock<IWeatherDataService>()
            .Setup(x => x.GetWeatherForecastAsync(key))
            .ReturnsAsync(forecast);

        await weatherService.DeleteWeatherForecastAsync(LATITUDE, LONGITUDE).ConfigureAwait(false);

        autoMocker.GetMock<IWeatherDataService>()
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

        autoMocker.GetMock<IWeatherDataService>()
            .Setup(x => x.GetWeatherForecastAsync(key))
            .ReturnsAsync(forecast);

        autoMocker.GetMock<IWeatherDataService>()
            .Setup(x => x.DeleteWeatherForecastAsync(key))
            .Returns(Task.CompletedTask);

        await weatherService.DeleteWeatherForecastAsync(LATITUDE, LONGITUDE).ConfigureAwait(false);

        autoMocker.GetMock<IWeatherDataService>()
            .Verify(x => x.DeleteWeatherForecastAsync(key), Times.Once());
    }

    [TestMethod]
    public async Task Given_CachedValueForWeatherForecastExists_ReturnsCachedValue()
    {
        var key = LatLongKey.Key(LATITUDE, LONGITUDE);
        
        var result = await weatherService.GetWeatherForecastAsync(LATITUDE, LONGITUDE).ConfigureAwait(false);

        Assert.IsTrue(result != null);
        Assert.IsTrue(result.Latitude == LATITUDE && result.Longitude == LONGITUDE);

        autoMocker.GetMock<IWeatherDataService>()
            .Verify(x => x.GetWeatherForecastAsync(key), Times.Never());
    }    

    [TestMethod]
    public async Task Given_CachedValueForWeather_DoesNotExist_ReturnsValueFromDb()
    {
        var key = LatLongKey.Key(LATITUDE, LONGITUDE);
        
        autoMocker.GetMock<IWeatherDataService>()
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

        autoMocker.GetMock<IWeatherDataService>()
            .Verify(x => x.GetWeatherForecastAsync(key), Times.Once());
    }


    [TestMethod]
    public async Task Given_CachedValueForWeatherForecastExists_ItWillNotSaveToDb()
    {
        var result = await weatherService.SaveWeatherForecastAync(LATITUDE,LONGITUDE).ConfigureAwait(false);
            
        Assert.IsTrue(result != null);
        Assert.IsTrue(result.Latitude == LATITUDE && result.Longitude == LONGITUDE);
    }

    [TestMethod]
    public async Task Given_CachedValueForWeatherForecast_DoesNotExist_ItWillSaveToDb()
    {
        var result = await weatherService.SaveWeatherForecastAync(LATITUDE, LONGITUDE).ConfigureAwait(false);

        Assert.IsTrue(result != null);
        Assert.IsTrue(result.Latitude == LATITUDE && result.Longitude == LONGITUDE);

        autoMocker.GetMock<IWeatherDataService>()
            .Verify(x => x.SaveWeatherForecastAsync(It.IsAny<WeatherForecast>()), Times.Once());
    }
}