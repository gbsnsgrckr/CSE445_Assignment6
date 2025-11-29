using System;
using System.ServiceModel;

namespace CSE445_Assignment6.SolarService
{
    [ServiceContract]
    public interface ISolarService
    {
        // Returns the annual average solar intensity  for a user-selected US ZIP code
        [OperationContract]
        decimal SolarIntensity(decimal latitude, decimal longitude);
    }
}
