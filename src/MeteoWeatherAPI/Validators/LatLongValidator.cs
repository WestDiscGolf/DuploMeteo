using FluentValidation;
using MeteoWeatherAPI.Dto;

namespace MeteoWeatherAPI.Validators;

public class LatLongValidator : AbstractValidator<BasicLatLongDto>
{
    public LatLongValidator()
    {
        RuleFor(x => x.Latitude).NotNull().NotEmpty();
        RuleFor(x => x.Longitude).NotNull().NotEmpty();

        RuleFor(x => x.Latitude).Matches(LatLongRegex.LATITUDE_REGEX)
            .WithMessage("Valid latitudes are between -90 and 90");
        RuleFor(x => x.Longitude).Matches(LatLongRegex.LONGITUDE_REGEX)
            .WithMessage("Valid longitudes are between -180 and 180");
    }
}