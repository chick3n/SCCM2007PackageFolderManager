using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using SMSCommunicator.Controllers;
using SMSCommunicator.Models;

namespace PackageFolderManager
{
    class PackageFolderManager
    {
        private StreamWriter loggingStream;
        private MappingsController mappingController;
        private SMSController smsController;

        public PackageFolderManager()
        {
            mappingController = new MappingsController();
            loggingStream = null;
            ErrorManager.Instance.OutputMessageEvent += new ErrorManager.OutputMessageAdded(Instance_OutputMessageEvent);
        }

        public PackageFolderManager(string mapping_path, string log_path)
        {
            mappingController = new MappingsController(mapping_path);
            loggingStream = null;

            if (!String.IsNullOrEmpty(log_path))
            {
                try
                {
                    loggingStream = new StreamWriter(Path.Combine(log_path, "task.log"));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            ErrorManager.Instance.OutputMessageEvent += new ErrorManager.OutputMessageAdded(Instance_OutputMessageEvent);
        }

        void Instance_OutputMessageEvent(string output)
        {
            if (loggingStream == null)
            {
                Console.WriteLine(output);
                return;
            }

            loggingStream.WriteLine(output);
        }

        public void Run()
        {
            List<string> servers = mappingController.GetServers();

            foreach (var server in servers)
            {
                if (smsController != null)
                    smsController.Close();

                smsController = new SMSController(server);
                if (!smsController.Connect())
                    continue;

                List<Package> packages = new List<Package>();
                List<PackageLocation> packageLocations = new List<PackageLocation>();

                //get a list of all packages and their location relative to folders
                foreach (var oPackageLocation in smsController.GetPackagesLocation())
                {
                    PackageLocation packageLocation = new PackageLocation();
                    packageLocation.SetObject(oPackageLocation);

                    packageLocations.Add(packageLocation);
                }

                //get a list of all packages to attach to each folder (visual use only)
                foreach (var oPackage in smsController.GetPackages())
                {
                    Package package = new Package();
                    package.SetObject(oPackage);

                    packages.Add(package);
                }

                List<PackageFolder> folders = mappingController.GetMappings(server);

                ErrorManager.AddOutput("Modifying Distribution points for mapped folders.");
                foreach (var folder in folders)
                {
                    var _packages = (from p in packages
                                     join pl in packageLocations on p.PackageID equals pl.InstanceKey
                                     where pl.ContainerID == folder.ContainerNodeId.ToString()
                                     select p).ToList();

                    if (_packages == null)
                        continue;

                    foreach (var package in _packages)
                        folder.Packages.Add(package);

                    smsController.ModifyDistributionPointsForPackageFolder(folder);
                }
            }
        }

        public void Close()
        {
            if (loggingStream != null)
            {
                loggingStream.Close();
                loggingStream.Dispose();
            }
        }
    }
}
