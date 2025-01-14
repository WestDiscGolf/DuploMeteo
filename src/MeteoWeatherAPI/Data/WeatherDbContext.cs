﻿using MeteoWeatherAPI.Dto;
using MongoDB.Driver;

namespace MeteoWeatherAPI.Data;

public class WeatherDbContext
{
    private readonly string DATABASE_NAME = "MeteoWeatherForecast";
    private readonly string WEATHER_COLLECTION_NAME = "WeatherForecast";
    private readonly string PREVIOUSLY_QUERIED_LATLONG_COLLECTION_NAME = "HistoricLatLongs";

    private readonly IMongoDatabase _dataBase;

    public WeatherDbContext(IMongoClient client)
    {
        _dataBase = client.GetDatabase(DATABASE_NAME);
    }

    public IMongoCollection<WeatherForecast> WeatherForecastContext => _dataBase.GetCollection<WeatherForecast>(WEATHER_COLLECTION_NAME);
    public IMongoCollection<HistoricLatLong> HistoricLatLongs => _dataBase.GetCollection<HistoricLatLong>(PREVIOUSLY_QUERIED_LATLONG_COLLECTION_NAME);
}