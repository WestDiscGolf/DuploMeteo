using Application.Dto;
using FluentValidation;

namespace Application.Validators
{
    public class LatLongValidator : AbstractValidator<BasicLatLongDto>
    {
        public LatLongValidator()
        {
            RuleFor(x => x.Latitude).NotNull().NotEmpty();
            RuleFor(x => x.Longitude).NotNull().NotEmpty();

            RuleFor(x => x.Latitude).Matches(Constants.LatLongRegex.LATITUDE_REGEX)
                .WithMessage("Valid latitudes are between -90 and 90");
            RuleFor(x => x.Longitude).Matches(Constants.LatLongRegex.LONGITUDE_REGEX)
                .WithMessage("Valid longitudes are between -180 and 180");
        }
    }
}
