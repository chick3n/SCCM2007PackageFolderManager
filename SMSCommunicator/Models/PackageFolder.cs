using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.ConfigurationManagement.ManagementProvider;

namespace SMSCommunicator.Models
{
    public class PackageFolder : BaseDataItem
    {
        public string Name { get; set; }
        public UInt32 ContainerNodeId { get; set; }
        public string FolderGuid { get; set; }
        public int ParentContainerNodeID { get; set; }

        public List<PackageFolder> ChildFolders;
        public List<Package> Packages;
        private List<PackageFolderDP> distributionPoints;

        public PackageFolder()
        {
            Packages = new List<Package>();
            ChildFolders = new List<PackageFolder>();
        }

        public PackageFolder(string name, int container_node_id, string folder_guid, int parent_container_node_id)
        {
            this.Name = name;
            this.ContainerNodeId = (UInt32)container_node_id;
            this.FolderGuid = folder_guid;
            this.ParentContainerNodeID = parent_container_node_id;

            Packages = new List<Package>();
            ChildFolders = new List<PackageFolder>();
        }

        public override bool SetObject(IResultObject smsObject)
        {
            try
            {
                Name = smsObject["Name"].StringValue;
                ContainerNodeId = (UInt32)smsObject["ContainerNodeID"].IntegerValue;
                FolderGuid = smsObject["FolderGuid"].StringValue;
                ParentContainerNodeID = smsObject["ParentContainerNodeID"].IntegerValue;
            }
            catch (SmsException ex)
            {
                ErrorManager.AddOutput(ex);
                return false;
            }

            return true;
        }


        public void AddDP(PackageFolderDP dp)
        {
            if (distributionPoints == null)
                distributionPoints = new List<PackageFolderDP>();

            distributionPoints.Add(dp);
        }

        public void RemoveDP(PackageFolderDP dp)
        {
            if (distributionPoints == null)
                return;

            distributionPoints.Remove(dp);
        }

        public void RemoveAllDP()
        {
            if (distributionPoints == null)
                return;

            distributionPoints.Clear();
        }

        public bool IsDistributionPointsSet()
        {
            return (distributionPoints != null) ? true : false;
        }

        public IEnumerable<PackageFolderDP> IterateDistributionPoints(bool hidePxeDP)
        {
            if (distributionPoints == null)
                yield break;

            foreach (var dp in distributionPoints)
            {
                if (hidePxeDP)
                {
                    if (!dp.NALPath.Contains("SMSPXEIMAGE"))
                        yield return dp;
                }
                else yield return dp;
            }
        }
    }
}
