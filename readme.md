# Prerequisite

In order to properly run this project, you must have docker installed and running on your machine.
This project is designed with docker in mind, namely with respect to how we connect to mongo. 

# Steps

- In the root directory, run `docker compose -f docker-compose.yml up`
- You may either run the project via VS, or run `dotnet run` in the MetoWeatherApi directory. 
- - If running via `dotnet run`, navigate to the url specified. In my case, `localhost:5080`
- Enjoy

# Considerations and Rationale Behind Code Choices

I've approached this project with an N-Tier application approach that tries to have clear separation of concerns
and responsibilities. The Application layer is the main driver, it is what the API layer will only ever communicate with.
The API layer itself is extremely thin. 

Whilst probably not needed, a basic in-memory cache as defined in `WeatherCacheService` helps reduces unneeded calls to the DB. 
Weather data for the same location usually doesn't change fast enough to where having fresh data is important. A standard 30 minute cache 
is sufficient for our use cases. 

The DataAccess layer, along with our mongo context, is entirely dedicated purely to simple CRUD operations on our aggregate data. All of the dates are stored
as UTC, with respect to the `Timezone` provided in the API response being region specific. This way, the consumer can always easily convert the time back to the 
appropriate locale as they know what `Timezone` was used when the dates were made.

# A Note About The Endpoints

I've purposefully implemented an action filter which will run for any endpoint which has the `MatchesLatLongAttribute`. The rationale behind this being
that our Lat/Long is sufficiently unique and idempotent that it acts as our primary key for our data later down the chain. The POST endpoint has a fluent validator
set up which ensures that we do pass in valid data when saving. 

However, assigning a fluent validator to a basic GET/DELETE endpoint which just accepts route parameters
is a hassle, at least from what I gather. As such, the attribute comes into play. In order to reduce load on the system, and properly convey to the user if they have passed 
data in a malformed fashion, we will throw a 400 with an object explaining what went wrong. This is explicitly to prevent the system from having to try to read/delete from
the cache/db when we know there will be nothing to do as the provided parameters were incorrect. 

No test cases were written for this as the logic is very much 1 : 1 as to what the
fluent validator does, so testing this would be duplicating test cases. 
