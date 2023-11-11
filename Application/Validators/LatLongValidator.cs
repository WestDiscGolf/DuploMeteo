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

            RuleFor(x => x.Latitude).Matches(@"^(\+|-)?(?:90(?:(?:\.0{1,7})?)|(?:[0-9]|[1-8][0-9])(?:(?:\.[0-9]{1,7})?))$")
                .WithMessage("Valid latitudes are between -90 and 90");
            RuleFor(x => x.Longitude).Matches(@"^(\+|-)?(?:180(?:(?:\.0{1,7})?)|(?:[0-9]|[1-9][0-9]|1[0-7][0-9])(?:(?:\.[0-9]{1,7})?))$")
                .WithMessage("Valid longitudes are between -180 and 180");
        }
    }
}
