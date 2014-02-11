using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.ConfigurationManagement.ManagementProvider;

namespace SMSCommunicator.Models
{
    public abstract class BaseDataItem
    {
        public abstract bool SetObject(IResultObject o);
    }
}
