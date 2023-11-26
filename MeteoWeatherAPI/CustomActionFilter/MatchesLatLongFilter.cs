using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MeteoWeatherAPI.CustomAttribute;
using System.Text.RegularExpressions;
using Application.Constants;

namespace MeteoWeatherAPI.CustomActionFilter
{
    public class MatchesLatLongFilter : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var attrib = (context.ActionDescriptor as ControllerActionDescriptor)
                .MethodInfo.GetCustomAttributes(typeof(MatchesLatLongAttribute), false).FirstOrDefault();
            
            if (attrib is null) return;

            string latitude = null, longitude = null;
            
            if(context.ActionArguments.TryGetValue("latitude", out object value))
                latitude = (string)value;
            if(context.ActionArguments.TryGetValue("longitude", out object value2))
                longitude = (string)value2;

            if(string.IsNullOrEmpty(latitude) || string.IsNullOrEmpty(longitude))
            {
                context.HttpContext.Response.StatusCode = 400;
                context.Result = new BadRequestObjectResult(new {
                    FailureReason = "Latitude and Longitude must be provided"
                });

                return;
            }

            var regexMatchLatitude = Regex.IsMatch(latitude, LatLongRegex.LATITUDE_REGEX);
            var regexMatchLongtitude = Regex.IsMatch(longitude, LatLongRegex.LONGITUDE_REGEX);

            if(!regexMatchLatitude || !regexMatchLongtitude)
            {
                context.HttpContext.Response.StatusCode = 400;
                context.Result = new BadRequestObjectResult(new {
                    FailureReason = "Latitude and Longtitude must be in the correct format"
                });

                return;
            }
                

            context.HttpContext.Response.StatusCode = 200;
        }
    }
}
