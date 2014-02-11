using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.ConfigurationManagement.ManagementProvider;

namespace SMSCommunicator.Models
{
    public class BaseDataContainer
    {
        public enum DataContainerType { DistributionPoint, Package, PackageFolder, PackageFolderDP, PackageLocation };
        public DataContainerType ContainerType { get; set;  }

        public BaseDataContainer(DataContainerType _type)
        {
            this.ContainerType = _type;
        }

        public virtual void SetObject(IResultObject o)
        {
        }
    }
}
