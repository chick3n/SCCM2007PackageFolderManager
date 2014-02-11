using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.ConfigurationManagement.ManagementProvider;
using System.Text.RegularExpressions;

namespace SMSCommunicator.Models
{
    public class DistributionPoint: BaseDataItem
    {
        public String ServerName { get; set; }
        public String RoleName { get; set; }
        public string SiteCode { get; set; }
        public string ServerRemoteName { get; set; }
        public string NALPath { get; set; }
        public string ResourceType { get; set; }
        private string displayName;
        public string DisplayName 
        { 
            get 
            { 
                if (displayName == null) 
                    ParseDisplayName(); 
                return displayName; 
            } 
        }

        private void ParseDisplayName()
        {
            if (String.IsNullOrEmpty(NALPath))
                displayName = ServerName;

            Match match = Regex.Match(NALPath, @"""Display=(.+?)""");
            if (!match.Success)
            {
                displayName = ServerName;
                return;
            }

            displayName = match.Groups[match.Groups.Count-1].Value;
        }

        public override bool SetObject(IResultObject smsObject)
        {
            try
            {
                ServerName = smsObject["ServerName"].StringValue;
                RoleName = smsObject["RoleName"].StringValue;
                SiteCode = smsObject["SiteCode"].StringValue;
                ServerRemoteName = smsObject["ServerRemoteName"].StringValue;
                NALPath = smsObject["NALPath"].StringValue;
                ResourceType = smsObject["ResourceType"].StringValue;
            }
            catch (SmsException e)
            {
                ErrorManager.AddOutput(e);
                return false;
            }

            return true;
        }
    }
}
