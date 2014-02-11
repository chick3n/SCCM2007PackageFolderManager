using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

using SMSCommunicator.Models;

namespace SMSCommunicator.Controllers
{
    public class MappingsController
    {
        private XmlDocument config;
        private const string FILE_NAME = @"mappings.xml";
        private string file_path;

        public MappingsController(string file_path = "")
        {
            this.file_path = System.IO.Path.Combine(file_path, FILE_NAME);
            config = LoadMappings();
        }

        public bool SaveMappings(List<PackageFolder> packageFolders, string serverName)
        {
            if (String.IsNullOrEmpty(serverName))
                return false;

            if (packageFolders == null)
                return false;

            if (config == null)
                config = new XmlDocument();

            XmlElement root = null;

            try
            {
                root = (XmlElement)config.SelectSingleNode("//host[@name='" + XmlConvert.EncodeName(serverName) + "']");
            }
            catch (System.Xml.XPath.XPathException e)
            {
                ErrorManager.AddOutput(e);
            }

            if (root == null)
            {
                root = (XmlElement)config.FirstChild.AppendChild(config.CreateElement("host"));
                root.SetAttribute("name", XmlConvert.EncodeName(serverName));
            }
            /*else
            {
                while (root.FirstChild != null)
                    root.RemoveChild(root.FirstChild);
            }*/

            foreach(var packageFolder in packageFolders)
                SaveMappings(packageFolder, root);

            try
            {
                config.Save(file_path);
            }
            catch (XmlException xml)
            {
                ErrorManager.AddOutput(xml);
                return false;
            }

            return true;
        }

        private bool SaveMappings(PackageFolder packageFolder, XmlElement root)
        {
            if (packageFolder == null)
                return false;

            foreach (var child in packageFolder.ChildFolders)
                SaveMappings(child, root);

            //skip folders we didnt make changes on (or loaded)
            if (packageFolder.IterateDistributionPoints(false).Count() == 0)
                return false;

            XmlElement folder;

            string XPath = String.Format("./folder[@Name='{0}' and @FolderGuid='{1}']",
                XmlConvert.EncodeName(packageFolder.Name),
                XmlConvert.EncodeName(packageFolder.FolderGuid));

            folder = (XmlElement)root.SelectSingleNode(XPath);

            if (folder == null)
            {
                folder = (XmlElement)root.AppendChild(config.CreateElement("folder"));
                //XmlElement folder = doc.CreateElement("Folder");
                folder.SetAttribute("Name", XmlConvert.EncodeName(packageFolder.Name));
                folder.SetAttribute("FolderGuid", XmlConvert.EncodeName(packageFolder.FolderGuid));
                folder.SetAttribute("ContainerNodeId", XmlConvert.EncodeName(packageFolder.ContainerNodeId.ToString()));
                folder.SetAttribute("ParentContainerNodeID", XmlConvert.EncodeName(packageFolder.ParentContainerNodeID.ToString()));
            }
            else //clear nodes
            {
                while (folder.FirstChild != null)
                    folder.RemoveChild(folder.FirstChild);
            }

            foreach (PackageFolderDP dp in packageFolder.IterateDistributionPoints(false))
            {
                if (!dp.UsePoint)
                    continue;

                XmlElement node = (XmlElement)folder.AppendChild(config.CreateElement("node"));

                node.SetAttribute("SiteCode", XmlConvert.EncodeName(dp.SiteCode));
                node.SetAttribute("DisplayName", XmlConvert.EncodeName(dp.DisplayName));
                node.SetAttribute("NALPath", XmlConvert.EncodeName(dp.NALPath));
                node.SetAttribute("ResourceType", XmlConvert.EncodeName(dp.ResourceType));
                node.SetAttribute("RoleName", XmlConvert.EncodeName(dp.RoleName));
                node.SetAttribute("ServerName", XmlConvert.EncodeName(dp.ServerName));
                node.SetAttribute("ServerRemoteName", XmlConvert.EncodeName(dp.ServerRemoteName));
            }

            return true;
        }

        private XmlDocument LoadMappings()
        {
            config = new XmlDocument();

            try
            {
                config.Load(file_path);
            }
            catch (Exception e)
            {
                config.AppendChild(config.CreateElement("servers"));
                ErrorManager.AddOutput(e);
            }

            return config;
        }

        public bool FolderHasMapping(string folderName, string folderGuid, string NALPath)
        {
            string XPath = String.Format("//folder[@Name='{0}' and @FolderGuid='{1}']",
                XmlConvert.EncodeName(folderName),
                XmlConvert.EncodeName(folderGuid));

            XmlElement folder = (XmlElement)config.SelectSingleNode(XPath);

            if (folder == null)
                return false;

            XPath = String.Format("./node[@NALPath='{0}']", XmlConvert.EncodeName(NALPath));

            var nodes = folder.SelectNodes(XPath);

            if (nodes != null && nodes.Count > 0)
                return true;

            return false;
        }

        public List<string> GetServers()
        {
            List<string> servers = new List<string>();

            if (config == null)
                return servers;

            if (config.ChildNodes.Count == 0)
                return servers;

            XmlNode root = config.ChildNodes[0];
            foreach (XmlNode child in root.ChildNodes)
            {
                XmlNode hostName = child.Attributes["name"];
                if (hostName != null && !String.IsNullOrEmpty(hostName.Value))
                    servers.Add(hostName.Value);
            }

            return servers;
        }

        public List<PackageFolder> GetMappings(string serverName)
        {
            List<PackageFolder> folders = new List<PackageFolder>();
            if (config == null)
                return folders;

            XmlElement root = null;

            try
            {
                root = (XmlElement)config.SelectSingleNode("//host[@name='" + XmlConvert.EncodeName(serverName) + "']");
            }
            catch (System.Xml.XPath.XPathException e)
            {
                ErrorManager.AddOutput(e);
            }

            if (root == null)
                return folders;

            foreach (XmlElement child in root.ChildNodes)
            {
                XmlNode name = null, guid = null, container_node = null;

                name = child.Attributes["Name"];
                guid = child.Attributes["FolderGuid"];
                container_node = child.Attributes["ContainerNodeId"];

                if (name == null || guid == null || container_node == null)
                    continue;

                int container_node_int = -1;
                if (!Int32.TryParse(XmlConvert.DecodeName(container_node.Value), out container_node_int))
                    continue;

                PackageFolder folder = new PackageFolder(XmlConvert.DecodeName(name.Value),
                    container_node_int, XmlConvert.DecodeName(guid.Value), -1);

                if (child.ChildNodes.Count == 0)
                    continue;

                foreach (XmlElement node in child.ChildNodes)
                {
                    XmlNode site_code, nal_path, resource_type, role_name, server_name, server_remote_name;

                    site_code = node.Attributes["SiteCode"];
                    nal_path = node.Attributes["NALPath"];
                    resource_type = node.Attributes["ResourceType"];
                    role_name = node.Attributes["RoleName"];
                    server_name = node.Attributes["ServerName"];
                    server_remote_name = node.Attributes["ServerRemoteName"];

                    PackageFolderDP folderDp = new PackageFolderDP();
                    folderDp.UsePoint = true;

                    if (site_code != null)
                        folderDp.SiteCode = XmlConvert.DecodeName(site_code.Value);
                    if (nal_path != null)
                        folderDp.NALPath = XmlConvert.DecodeName(nal_path.Value);
                    if (resource_type != null)
                        folderDp.ResourceType = XmlConvert.DecodeName(resource_type.Value);
                    if (role_name != null)
                        folderDp.RoleName = XmlConvert.DecodeName(role_name.Value);
                    if (server_name != null)
                        folderDp.ServerName = XmlConvert.DecodeName(server_name.Value);
                    if (server_remote_name != null)
                        folderDp.ServerRemoteName = XmlConvert.DecodeName(server_remote_name.Value);

                    folder.AddDP(folderDp);
                }

                folders.Add(folder);
            }

            return folders;
        }
    }
}
