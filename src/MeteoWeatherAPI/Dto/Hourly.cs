﻿namespace MeteoWeatherAPI.Dto;

public class Hourly
{
    public List<DateTime> Times { get; set; }

    public List<double> Temperatures { get; set; }
}