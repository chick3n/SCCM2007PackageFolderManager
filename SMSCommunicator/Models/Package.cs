using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.ConfigurationManagement.ManagementProvider;

namespace SMSCommunicator.Models
{
    public class Package : BaseDataItem
    {
        public string Name { get; set; }
        public string PackageID { get; set; }
        public string ContainerID { get; set; }

        public Package()
        {
        }

        public Package(string name, string package_id, string container_id)
        {
            this.Name = name;
            this.PackageID = package_id;
            this.ContainerID = container_id;
        }

        public override bool SetObject(IResultObject smsObejct)
        {
            try
            {
                Name = smsObejct["Name"].StringValue;
                PackageID = smsObejct["PackageID"].StringValue;
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
