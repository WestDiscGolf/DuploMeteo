using Application.Dto;
using Application.Validators;
using FluentValidation.TestHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.FluentValidatorTests
{
    [TestClass]
    public class FluentValidatorTests
    {
        private LatLongValidator validator;

        [TestInitialize]
        public void Init()
        {
            validator = new LatLongValidator();
        }

        [TestMethod]
        public void Given_ModelIsValid_FluentValidationPasses()
        {
            var validModel = new BasicLatLongDto()
            {
                Latitude = "34.0754",
                Longitude = "-84.2941"
            };

            var result = validator.TestValidate(validModel);
            Assert.IsTrue(result.IsValid);
            
        }

        [TestMethod]
        public void Given_LatitudeIsInvalidFormat_FluentValidationFails()
        {
            var validModel = new BasicLatLongDto()
            {
                Latitude = "90",
                Longitude = "-84.2941"
            };

            var result = validator.TestValidate(validModel);
            result.ShouldHaveValidationErrorFor(x => x.Latitude);
            //Assert.IsFalse(result.IsValid);
        }
    }
}
