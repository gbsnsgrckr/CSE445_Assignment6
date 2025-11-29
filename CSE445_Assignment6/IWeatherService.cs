using System;
using System.ServiceModel;

namespace CSE445_Assignment6.WeatherService
{
    [ServiceContract]
    public interface IWeatherService
    {
        // Returns the 5-day forecast for the user-selected US ZIP code.
        [OperationContract]
        string[] Weather5day(string zipcode);
    }
}
