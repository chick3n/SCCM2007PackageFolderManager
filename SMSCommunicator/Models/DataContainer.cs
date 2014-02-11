using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.ConfigurationManagement.ManagementProvider;

namespace SMSCommunicator.Models
{
    public class DataContainer<T>: BaseDataContainer where T : BaseDataItem, new()
    {
        private List<T> dataSet;

        public DataContainer(DataContainerType _type): base(_type)
        {
            dataSet = new List<T>();
        }

        public override void SetObject(IResultObject o)
        {
            var dataItem = new T();
            dataItem.SetObject(o);
            dataSet.Add(dataItem);
        }
            
            /*
            switch (ContainerType)
            {
                case DataContainerType.DistributionPoint:
                    AddDataItem(new DistributionPoint(), o);
                    break;
                case DataContainerType.Package:
                    AddDataItem(new Package(), o);
                    break;
                case DataContainerType.PackageFolder:
                    AddDataItem(new PackageFolder(), o);
                    break;
                case DataContainerType.PackageLocation:
                    AddDataItem(new PackageLocation(), o);
                    break;
            }
        }

        private void AddDataItem(BaseDataItem dataItem, IResultObject o)
        {
            if (dataItem != null)
            {
                dataItem.SetObject(o);
                dataSet.Add(dataItem as T);
            }
        }*/

    }
}
