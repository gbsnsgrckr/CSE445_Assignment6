using System;
using System.ServiceModel;

namespace CSE445_Assignment6.WindService
{
    [ServiceContract]
    public interface IWindService
    {
        // Returns the annual average wind speed for a user-selected US ZIP code
        [OperationContract]
        decimal WindIntensity(decimal latitude, decimal longitude);
    }
}
