using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using SMSCommunicator.Controllers;
using SMSCommunicator.Models;
using SCCM2007PackageFolderManager.Controllers;

namespace SCCM2007PackageFolderManager
{
    public partial class FolderManager : Form
    {
        private FormController formController;

        public FolderManager()
        {
            formController = new FormController();

            InitializeComponent();

            Begin_Load();
        }

        private void Begin_Load()
        {
            txtServerName.Text = "SRVDEVSCCM01";
            DisableElements();

            imgList.Images.Add("Folder_Closed", Properties.Resources.Folder);
            imgList.Images.Add("Folder_Open", Properties.Resources.FolderOpen);
            imgList.Images.Add("Package", Properties.Resources.Package);

            treePackages.BeforeSelect += new TreeViewCancelEventHandler(treePackages_BeforeSelect);
            treePackages.ImageList = imgList;

            formController.LoadComplete += new FormController.LoadCompleted(formController_LoadComplete);
            formController.SaveComplete += new FormController.SaveCompleted(formController_SaveComplete);
            formController.ConnectComplete += new FormController.ConnectCompleted(formController_ConnectComplete);

            ErrorManager.Instance.OutputMessageEvent += new ErrorManager.OutputMessageAdded(Instance_OutputMessageEvent);
            ErrorManager.Instance.OutputMessageCleared += new EventHandler(Instance_OutputMessageCleared);
        }

        void Instance_OutputMessageCleared(object sender, EventArgs e)
        {
            lstOutput.Items.Clear();
        }

        void Instance_OutputMessageEvent(string output)
        {
            lstOutput.Items.Add(output);
        }

        void formController_ConnectComplete(FormController sender, bool? status)
        {
            if (status != null && status.Value)
            {
                GetPackageStructure();
            }
            else
            {
                DisableElements();
                ClearElements();
            }
        }

        void treePackages_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node.Tag.GetType() == typeof(SMSCommunicator.Models.Package))
                e.Cancel = true;
        }

        void formController_SaveComplete(FormController sender)
        {
            EnableElements();
            chkAllToggle.Enabled = true;
            ErrorManager.AddOutput("Saving file complete.");
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtServerName.Text))
            {
                MessageBox.Show("Server name cannot be empty.");
                return;
            }

            lstOutput.Items.Clear();
            btnLoad.Enabled = false;
            formController.Connect(txtServerName.Text);            
        }

        private void ClearElements()
        {
            lstDistributionPoints.Items.Clear();
            treePackages.Nodes.Clear();
        }

        private void EnableElements()
        {
            btnLoad.Enabled = true;
            treePackages.Enabled = true;
            btnApply.Enabled = true;
            lstDistributionPoints.Enabled = true;
            chkHidePxe.Enabled = true;
            //chkAllToggle.Enabled = true;
        }

        private void DisableElements()
        {
            treePackages.Enabled = false;
            btnApply.Enabled = false;
            lstDistributionPoints.Enabled = false;
            chkHidePxe.Enabled = false;
            chkAllToggle.Enabled = false;
        }

        private void GetPackageStructure()
        {
            formController.GetPackageStructure();
        }

        void formController_LoadComplete(FormController f, List<PackageFolder> nodes)
        {
            ClearElements();

            if (nodes == null)
            {
                btnLoad.Enabled = true;
                DisableElements();
                return;
            }

            EnableElements();

            foreach (var node in nodes)
            {
                var tNode = GetPackageStructure(node);
                tNode.ExpandAll();
                treePackages.Nodes.Add(tNode);
            }
        }

        private TreeNode GetPackageStructure(PackageFolder node)
        {
            TreeNode tNode = new TreeNode();
            tNode.Tag = node;
            tNode.Name = node.Name;
            tNode.Text = node.Name;
            tNode.ImageKey = "Folder_Closed";

            if (node.ChildFolders.Count > 0)
            {
                foreach (var childNode in node.ChildFolders)
                    tNode.Nodes.Add(GetPackageStructure(childNode));
            }

            foreach (var children in node.Packages)
            {
                TreeNode tChildren = new TreeNode();
                tChildren.Tag = children;
                tChildren.Name = children.Name;
                tChildren.Text = children.Name;
                tChildren.ImageKey = "Package";

                tNode.Nodes.Add(tChildren);
            }

            return tNode;
        }

        private void treePackages_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Tag.GetType() == typeof(SMSCommunicator.Models.PackageFolder))
            {
                PackageFolder pf = e.Node.Tag as PackageFolder;

                if (pf.IsDistributionPointsSet() == false)
                    formController.GetDistributionPoints(pf);

                DisplayDistributionPoints(pf);
                    
            }
        }

        private void DisplayDistributionPoints(PackageFolder pf)
        {
            lstDistributionPoints.Items.Clear();
            chkAllToggle.CheckState = CheckState.Unchecked;

            bool hidePxe = chkHidePxe.Checked;

            foreach (var dp in pf.IterateDistributionPoints(hidePxe))
            {
                lstDistributionPoints.Items.Add(dp, dp.UsePoint);
            }

            chkAllToggle.Enabled = lstDistributionPoints.Items.Count > 0;
        }

        private void chkHidePxe_CheckedChanged(object sender, EventArgs e)
        {
            if (treePackages.SelectedNode != null
                && treePackages.SelectedNode.Tag != null
                    && treePackages.SelectedNode.Tag.GetType() == typeof(PackageFolder))
            {
                DisplayDistributionPoints(treePackages.SelectedNode.Tag as PackageFolder);
            }
        }

        private void lstDistributionPoints_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            PackageFolderDP pfdp = lstDistributionPoints.Items[e.Index] as PackageFolderDP;

            if (pfdp != null)
                pfdp.UsePoint = e.NewValue == CheckState.Checked;
        }

        private void btnExpand_Click(object sender, EventArgs e)
        {
            treePackages.ExpandAll();
        }

        private void btnCloseFolders_Click(object sender, EventArgs e)
        {
            treePackages.CollapseAll();
        }

        private void chkAllToggle_CheckedChanged(object sender, EventArgs e)
        {
            for (int x=0; x<lstDistributionPoints.Items.Count; x++)
            {
                lstDistributionPoints.SetItemCheckState(x, chkAllToggle.CheckState);
            }
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            ErrorManager.AddOutput("Beginning to Save folder mappings, please wait.");
            DisableElements();
            formController.Save();
        }

        private void lstOutput_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int index = lstOutput.IndexFromPoint(e.Location);
            if (index != System.Windows.Forms.ListBox.NoMatches && lstOutput.Items.Count > index)
            {
                object message = lstOutput.Items[index];
                OutputReader or = new OutputReader();
                or.OutputMessage = message.ToString();
                or.ShowDialog();
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            lstOutput.Items.Clear();
        }

    }
}
