using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.ConfigurationManagement.ManagementProvider;

namespace SMSCommunicator.Models
{
    public class PackageLocation
     :  BaseDataItem
    {
        public string InstanceKey { get; set; }
        public string ContainerID { get; set; }

        public PackageLocation() {
        }

        public PackageLocation(string instance_key, string container_id)
        {
            this.InstanceKey = instance_key;
            this.ContainerID = container_id;
        }

        public override bool SetObject(IResultObject smsObejct)
        {
            try
            {
                InstanceKey = smsObejct["InstanceKey"].StringValue;
                ContainerID = smsObejct["ContainerNodeID"].StringValue;
            }
            catch (SmsException ex)
            {
                ErrorManager.AddOutput(ex);
                return false;
            }

            return true;
        }
    }
}
