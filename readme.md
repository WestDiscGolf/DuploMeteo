# Prerequisite

In order to properly run this project, you must have docker installed and running on your machine.
This project is designed with docker in mind, namely with respect to how we connect to mongo. 

# Steps

- In the root directory, run `docker compose -f docker-compose.yml up`
- You may either run the project via VS, or run `dotnet run` in the MetoWeatherApi directory. 
- Enjoy

# Considerations and Rationale

I've approached this project with an N-Tier application approach that tries to have clear separation of concerns
and responsibilities. The Application layer is the main driver, it is what the API layer will only ever communicate with.
The API layer itself is extremely thin. 

Whilst probably not needed, a basic in-memory cache as defined in `WeatherCacheService` helps reduces unneeded calls to the DB. 
Weather data for the same location usually doesn't change fast enough to where having fresh data is important. A standard 30 minute cache 
is sufficient for our use cases. 

The DataAccess layer, along with our mongo context, is entirely dedicated purely to simple CRUD operations on our aggregate data.