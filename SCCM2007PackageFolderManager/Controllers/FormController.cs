using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SMSCommunicator.Controllers;
using SMSCommunicator.Models;
using Microsoft.ConfigurationManagement.ManagementProvider;
using System.ComponentModel;

namespace SCCM2007PackageFolderManager.Controllers
{
    class FormController
    {
        private SMSController smsController;
        private MappingsController mappingController;
        private object connectLock;

        private List<PackageFolder> packageFolders;
        private List<DistributionPoint> distributionPoints;

        private class LoadDataContainer
        {
            public List<PackageFolder> folders;
            public List<DistributionPoint> distributionPoints;

            public LoadDataContainer() { folders = new List<PackageFolder>(); distributionPoints = new List<DistributionPoint>(); }
        }

        //events
        public event LoadCompleted LoadComplete;
        public delegate void LoadCompleted(FormController sender, List<PackageFolder> folders);
        public event SaveCompleted SaveComplete;
        public delegate void SaveCompleted(FormController sender);
        public event ConnectCompleted ConnectComplete;
        public delegate void ConnectCompleted(FormController sender, bool? status);

        public FormController()
        {
            packageFolders = new List<PackageFolder>();
            distributionPoints = new List<DistributionPoint>();
            mappingController = new MappingsController();
            connectLock = new object();
        }

        public bool Connect(string server_name)
        {
            if (smsController != null)
            {
                ErrorManager.AddOutput("Closing connection to: " + smsController.ServerName);
                smsController.Close();
            }

            packageFolders.Clear();
            distributionPoints.Clear();
            ErrorManager.Clear();

            BackgroundWorker worker = new BackgroundWorker();
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(EndConnect);
            worker.DoWork += new DoWorkEventHandler(StartConnect);

            ErrorManager.AddOutput("Connecting to " + server_name);
            worker.RunWorkerAsync(new object[] {server_name});

            return true;
        }

        void StartConnect(object sender, DoWorkEventArgs e)
        {
            object[] args = e.Argument as object[];
            bool connected = false;
            if (args.Length > 0)
            {
                lock (connectLock)
                {
                    smsController = new SMSController(args[0] as string);
                    connected = smsController.Connect();
                }
            }

            e.Result = connected;
        }

        void EndConnect(object sender, RunWorkerCompletedEventArgs e)
        {
            ErrorManager.AddOutput("Finished connecting to " + smsController.ServerName + ", connection has " + ((e.Result as bool? != null) ? ((bool?)e.Result).Value.ToString() : "failed"));
            ConnectComplete(this, e.Result as bool?);
        }

        public void GetPackageStructure()
        {
            BeginBuildPackageStructure();
        }

        public bool GetDistributionPoints(PackageFolder pf)
        {
            if (distributionPoints == null)
            {
                foreach (var o in smsController.GetDistributionPoints())
                {
                    DistributionPoint dp = new DistributionPoint();
                    dp.SetObject(o);

                    distributionPoints.Add(dp);
                }
            }

            foreach (var dp in distributionPoints)
            {
                PackageFolderDP pfdp = new PackageFolderDP();
                pfdp.CopyDistributionPointData(dp);

                //check if mapping file has entry for dp
                if (mappingController.FolderHasMapping(pf.Name, pf.FolderGuid, dp.NALPath))
                    pfdp.UsePoint = true;

                pf.AddDP(pfdp);
            }

            return true;
        }

        private void BeginBuildPackageStructure()
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += new DoWorkEventHandler(BuildPackageStructure);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(EndBuildPackageStructure);

            ErrorManager.AddOutput("Synching SCCM package folders and DP.");
            worker.RunWorkerAsync();
        }

        private void EndBuildPackageStructure(object sender, RunWorkerCompletedEventArgs e)
        {
            LoadDataContainer dataContainer = e.Result as LoadDataContainer;

            if (dataContainer != null)
            {
                packageFolders = dataContainer.folders;
                distributionPoints = dataContainer.distributionPoints;
            }

            ErrorManager.AddOutput("Finished Synching SCCM package folders and DP.");

            LoadComplete(this, packageFolders);
        }

        private void BuildPackageStructure(object sender, DoWorkEventArgs e)
        {
            List<Package> packages = new List<Package>();
            List<PackageLocation> packageLocations = new List<PackageLocation>();
            List<PackageFolder> _packageFolders = new List<PackageFolder>();
            List<DistributionPoint> _distributionPoints = new List<DistributionPoint>();

            //get list of distribution points to attach to folders
            foreach (var o in smsController.GetDistributionPoints())
            {
                DistributionPoint dp = new DistributionPoint();
                dp.SetObject(o);

                _distributionPoints.Add(dp);
            }

            //get a list of all packages and their location relative to folders
            foreach (var oPackageLocation in smsController.GetPackagesLocation())
            {
                PackageLocation packageLocation = new PackageLocation();
                packageLocation.SetObject(oPackageLocation);

                packageLocations.Add(packageLocation);
            }

            //get a list of all packages to attach to each folder (visual use only)
            foreach (IResultObject oPackage in smsController.GetPackages())
            {
                Package package = new Package();
                package.SetObject(oPackage);

                packages.Add(package);
            }

            //get all the folders, find the packages that belong to folder and assign it
            foreach (var oPackageFolder in smsController.GetPackageFolders())
            {
                PackageFolder packageFolder = new PackageFolder();
                packageFolder.SetObject(oPackageFolder);

                var _packages = (from p in packages
                               join pl in packageLocations on p.PackageID equals pl.InstanceKey
                               where pl.ContainerID == packageFolder.ContainerNodeId.ToString()
                               select p).ToList();

                if (_packages == null)
                    continue;
                
                foreach(var package in _packages)
                    packageFolder.Packages.Add(package);

                _packageFolders.Add(packageFolder);
            }

            //restructure children folders
            for(int x=_packageFolders.Count-1; x>=0; x--)
            {
                var parent = (from pf in _packageFolders
                              where pf.ContainerNodeId == _packageFolders[x].ParentContainerNodeID
                              select pf).FirstOrDefault();

                if (parent == null)
                    continue;

                parent.ChildFolders.Add(_packageFolders[x]);
                _packageFolders.Remove(_packageFolders[x]);
            }

            e.Result = new LoadDataContainer
            { 
                distributionPoints = _distributionPoints,
                folders = _packageFolders
            };
        }

        public bool Save()
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += new DoWorkEventHandler(BeginSave);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(EndSave);

            worker.RunWorkerAsync();

            return true;
        }

        private void BeginSave(object sender, DoWorkEventArgs e)
        {
            //Todo: check if service exists and is running, if it is push update to service
            foreach (var pl in packageFolders)
            {
                UpdatePackageFolders(pl);
            }

            mappingController.SaveMappings(packageFolders, smsController.ServerName);
        }

        private void EndSave(object sender, RunWorkerCompletedEventArgs e)
        {
            SaveComplete(this);
        }

        private void UpdatePackageFolders(PackageFolder pl)
        {
            if (pl == null)
                return;

            foreach (var _pl in pl.ChildFolders)
            {
                UpdatePackageFolders(_pl);
            }

            smsController.ModifyDistributionPointsForPackageFolder(pl);
        }
    }
}
