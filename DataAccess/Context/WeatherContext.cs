using Domain;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Context
{
    public class WeatherContext
    {
        private readonly string DATABASE_NAME = "MeteoWeatherForecast";
        private readonly string WEATHER_COLLECTION_NAME = "WeatherForecast";
        private readonly string PREVIOUSLY_QUERIED_LATLONG_COLLECTION_NAME = "HistoricLatLongs";

        private readonly IMongoDatabase _dataBase;

        public WeatherContext(IMongoClient client)
        {
            _dataBase = client.GetDatabase(DATABASE_NAME);
        }

        public IMongoCollection<WeatherForecast> WeatherForecastContext => _dataBase.GetCollection<WeatherForecast>(WEATHER_COLLECTION_NAME);
        public IMongoCollection<HistoricLatLong> HistoricLatLongs => _dataBase.GetCollection<HistoricLatLong>(PREVIOUSLY_QUERIED_LATLONG_COLLECTION_NAME);
    }
}
