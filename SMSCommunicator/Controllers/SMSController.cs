using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.ConfigurationManagement;
using Microsoft.ConfigurationManagement.ManagementProvider;
using Microsoft.ConfigurationManagement.ManagementProvider.WqlQueryEngine;

using SMSCommunicator.Models;

namespace SMSCommunicator.Controllers
{
    public class SMSController
    {
        private WqlConnectionManager connection;
        public string ServerName { get; set; }
        private SmsBackgroundWorker backgroundWorker;

        //Async Event Handlers
        public event AsyncDataCompleted AsyncDataHandler;
        public delegate void AsyncDataCompleted(SMSController sms, BaseDataContainer dataContainer);

        public SMSController(string server_name)
        {
            ServerName = server_name;
            backgroundWorker = new SmsBackgroundWorker();

            backgroundWorker.QueryProcessorCompleted += new EventHandler<System.ComponentModel.RunWorkerCompletedEventArgs>(backgroundWorker_QueryProcessorCompleted);
            backgroundWorker.QueryProcessorObjectReady += new EventHandler<QueryProcessorObjectEventArgs>(backgroundWorker_QueryProcessorObjectReady);
        }

        public bool Connect()
        {
            try {
                connection = new WqlConnectionManager();
                connection.Connect(ServerName);
                return true;
            }
            catch(SmsException e)
            {
                ErrorManager.AddOutput(e);
            }
            catch(UnauthorizedAccessException a)
            {
                ErrorManager.AddOutput(a);
            }

            return false;
        }

        public void Close()
        {
            if (connection != null)
            {
                connection.Close();
                connection.Dispose();
            }

            if (backgroundWorker != null)
            {
                backgroundWorker.QueryProcessorCompleted -= backgroundWorker_QueryProcessorCompleted;
                backgroundWorker.QueryProcessorObjectReady -= backgroundWorker_QueryProcessorObjectReady;

                backgroundWorker.CancelAsync();
                backgroundWorker.Dispose();
            }
        }

        #region ASYNC
        /* Asynchronous Calls */

        public bool GetDistributionPointsAsync(string site_code = null)
        {
            String query = "SELECT * FROM SMS_SystemResourceList WHERE RoleName='SMS Distribution Point'";

            if (!String.IsNullOrEmpty(site_code))
                query += String.Format(" AND SiteCode='{0}'", site_code);

            if (!AsyncProcessQuery(query, new DataContainer<DistributionPoint>(BaseDataContainer.DataContainerType.DistributionPoint)))
                return false;

            return true;
        }

        bool AsyncProcessQuery(string query, object dataContainer)
        {
            if (!connection.IsQueryValid(query))
                return false;

            connection.QueryProcessor.ProcessQuery(backgroundWorker,
                query,
                dataContainer);

            return true;
        }

        void backgroundWorker_QueryProcessorObjectReady(object sender, QueryProcessorObjectEventArgs e)
        {
            BaseDataContainer dataContainer = e.UserState as BaseDataContainer;
            IResultObject o = (IResultObject)e.ResultObject;

            try
            {
                dataContainer.SetObject(o);
            }
            catch (SmsQueryException ex)
            {
                ErrorManager.AddOutput(ex);
            }

            return;
        }

        void backgroundWorker_QueryProcessorCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            AsyncDataHandler(this, e.Result as BaseDataContainer);
        }

        #endregion

        /* Synchronous Calls */

        public System.Collections.Generic.IEnumerable<IResultObject> GetDistributionPoints(string site_code = null)
        {
            String query = "SELECT * FROM SMS_SystemResourceList WHERE RoleName='SMS Distribution Point'";

            if (!String.IsNullOrEmpty(site_code))
                query += String.Format(" AND SiteCode='{0}'", site_code);

            IResultObject dataSet = ExecuteQuery(query);

            if (dataSet != null)
            {
                foreach (IResultObject data in dataSet)
                {
                    yield return data;
                    data.Dispose();
                }
            }
            else
                yield break;
        }

        public System.Collections.Generic.IEnumerable<IResultObject> GetPackageDistributionPoints(string PackageID)
        {
            if (String.IsNullOrEmpty(PackageID))
                yield break;

            String query = String.Format("Select * from SMS_DistributionPoint Where PackageID = '{0}'", PackageID);

            IResultObject dataSet = ExecuteQuery(query);

            if (dataSet != null)
            {
                foreach (IResultObject data in dataSet)
                {
                    yield return data;
                    data.Dispose();
                }
            }
            else yield break;
        }

        public System.Collections.Generic.IEnumerable<IResultObject> GetPackageFolders()
        {
            //http://msdn.microsoft.com/en-us/library/cc145264.aspx
            String query = "Select * from SMS_ObjectContainerNode WHERE ObjectType IN (2)";

            IResultObject dataSet = ExecuteQuery(query);

            if (dataSet != null)
            {
                foreach (IResultObject data in dataSet)
                {
                    yield return data;
                    data.Dispose();
                }
            }
            else
                yield break;
        }
        
        public System.Collections.Generic.IEnumerable<IResultObject> GetPackagesLocation()
        {
            String query = "Select * from SMS_ObjectContainerItem";

            IResultObject dataSet = ExecuteQuery(query);

            if (dataSet != null)
            {
                foreach (IResultObject data in dataSet)
                {
                    yield return data;
                    data.Dispose();
                }
            }
            else
                yield break;
        }

        public System.Collections.Generic.IEnumerable<IResultObject> GetPackages()
        {
            String query = "Select * From SMS_Package";

            IResultObject dataSet = ExecuteQuery(query);

            if (dataSet != null)
            {
                foreach (IResultObject data in dataSet)
                {
                    yield return data;
                    data.Dispose();
                }
            }
            else
                yield break;
        }

        private IResultObject ExecuteQuery(string query)
        {
            if (!connection.IsQueryValid(query))
            {
                ErrorManager.AddOutput(new Exception("Query is invalid", new Exception(query)));
                return null;
            }

            try
            {
                IResultObject o = connection.QueryProcessor.ExecuteQuery(query);
                return o;
            }
            catch (SmsException e)
            {
                ErrorManager.AddOutput(e);
            }

            return null;
        }

        private bool AddDistributionPointToPackage(string nal_path, string site_code, string package_id, bool refresh = true)
        {
            //add
            IResultObject distributionPoint = null;
            try
            {
                distributionPoint = connection.CreateInstance("SMS_DistributionPoint");
                distributionPoint["ServerNALPath"].StringValue = nal_path;
                distributionPoint["SiteCode"].StringValue = site_code;
                distributionPoint["PackageID"].StringValue = package_id;
                distributionPoint["RefreshNow"].BooleanValue = refresh;

                distributionPoint.Put();
                distributionPoint.Dispose();
            }
            catch (SmsException e)
            {
                ErrorManager.AddOutput(e);

                if (distributionPoint != null)
                    distributionPoint.Dispose();

                return false;
            }

            return true;
        }

        private bool RefreshDistributionPointForPackage(string nal_path, string site_code, string package_id)
        {
            //add
            IResultObject distributionPoint = null;
            try
            {
                distributionPoint = connection.CreateInstance("SMS_DistributionPoint");
                distributionPoint["ServerNALPath"].StringValue = nal_path;
                distributionPoint["SiteCode"].StringValue = site_code;
                distributionPoint["PackageID"].StringValue = package_id;
                distributionPoint["RefreshNow"].BooleanValue = true;

                distributionPoint.Put();
                distributionPoint.Dispose();
            }
            catch (SmsException e)
            {
                ErrorManager.AddOutput(e);

                if (distributionPoint != null)
                    distributionPoint.Dispose();

                return false;
            }

            return true;
        }

        private bool RemoveDistributionPointFromPackage(string nal_path, string site_code, string package_id)
        {
            IResultObject distributionPoint = null;
            try
            {
                distributionPoint = connection.CreateInstance("SMS_DistributionPoint");
                distributionPoint["ServerNALPath"].StringValue = nal_path;
                distributionPoint["SiteCode"].StringValue = site_code;
                distributionPoint["PackageID"].StringValue = package_id;

                distributionPoint.Delete();
                distributionPoint.Dispose();
            }
            catch (SmsException e)
            {
                ErrorManager.AddOutput(e);

                if (distributionPoint != null)
                    distributionPoint.Dispose();

                return false;
            }

            return true;
        }

        public bool ModifyDistributionPointsForPackageFolder(PackageFolder packageFolder)
        {
            if (packageFolder == null)
                return false;

            foreach (var package in packageFolder.Packages)
            {
                //if pcakagefolder never had dp added to it, no changes were made so skip
                if (packageFolder.IterateDistributionPoints(false).Count() == 0)
                    continue;

                List<DistributionPoint> assignedPoints = new List<DistributionPoint>();
                //get dp currently assigned to pkg
                foreach (var dp in GetPackageDistributionPoints(package.PackageID))
                {
                    DistributionPoint assignedPoint = new DistributionPoint();
                    assignedPoint.NALPath = dp["ServerNALPath"].StringValue;
                    assignedPoint.SiteCode = dp["SiteCode"].StringValue;

                    assignedPoints.Add(assignedPoint);
                }

                //validate it against current list and delete it if it isnt present
                foreach (var pfdp in packageFolder.IterateDistributionPoints(false))
                {
                    var assignedPoint = (from ap in assignedPoints
                                         where ap.NALPath == pfdp.NALPath
                                         select ap).FirstOrDefault();

                    if (assignedPoint != null)
                    {
                        if (pfdp.UsePoint == false)
                        {
                            ErrorManager.AddOutput(String.Format("Removing DP: Package {0} DP {1}", package.Name, assignedPoint.DisplayName));
                            RemoveDistributionPointFromPackage(assignedPoint.NALPath, assignedPoint.SiteCode, package.PackageID);
                        }
                    } 
                    else if(pfdp.UsePoint) //dp is not assigned to package already
                    {
                        ErrorManager.AddOutput(String.Format("Adding DP: Package {0} DP {1}", package.Name, pfdp.DisplayName));
                        AddDistributionPointToPackage(pfdp.NALPath, pfdp.SiteCode, package.PackageID);
                    }

                    assignedPoints.Remove(assignedPoint);
                }

                //remove remaining if any
                foreach (var dp in assignedPoints)
                {
                    ErrorManager.AddOutput(String.Format("Removing DP: Package {0} DP {1}", package.Name, dp.DisplayName));
                    RemoveDistributionPointFromPackage(dp.NALPath, dp.SiteCode, package.PackageID);
                }
            }            

            return true;
        }

    }
}
