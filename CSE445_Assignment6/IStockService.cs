using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace CSE445_Assignment6.StockService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IStockService
    {
        // this is my service to make rule-based recommendations on stocks based on key factors about where they are at 
        [OperationContract]
        string StockInfo(string symbol, double c, double d, double dp, double h, double l, double o, double pc);
    }
}
