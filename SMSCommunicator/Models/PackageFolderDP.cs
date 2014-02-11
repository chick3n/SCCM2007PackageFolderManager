using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMSCommunicator.Models
{
    public class PackageFolderDP : DistributionPoint
    {
        public bool UsePoint { get; set; }

        public PackageFolderDP()
        {
            UsePoint = false;
        }

        public void CopyDistributionPointData(DistributionPoint dp)
        {
            this.ServerName = dp.ServerName;
            this.ServerRemoteName = dp.ServerRemoteName;
            this.RoleName = dp.RoleName;
            this.ResourceType = dp.ResourceType;
            this.NALPath = dp.NALPath;
            this.SiteCode = dp.SiteCode;
        }

        public override string ToString()
        {
            return String.Format("{0} - {1}",
                DisplayName,
                SiteCode);
        }
    }
}
