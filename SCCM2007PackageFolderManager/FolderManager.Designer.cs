namespace SCCM2007PackageFolderManager
{
    partial class FolderManager
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FolderManager));
            this.treePackages = new System.Windows.Forms.TreeView();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chkAllToggle = new System.Windows.Forms.CheckBox();
            this.lstDistributionPoints = new System.Windows.Forms.CheckedListBox();
            this.chkHidePxe = new System.Windows.Forms.CheckBox();
            this.btnApply = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtServerName = new System.Windows.Forms.TextBox();
            this.btnLoad = new System.Windows.Forms.Button();
            this.imgList = new System.Windows.Forms.ImageList(this.components);
            this.btnExpand = new System.Windows.Forms.Button();
            this.btnCloseFolders = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lstOutput = new System.Windows.Forms.ListBox();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnSaveOnly = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // treePackages
            // 
            this.treePackages.HideSelection = false;
            this.treePackages.Location = new System.Drawing.Point(3, 3);
            this.treePackages.Name = "treePackages";
            this.treePackages.Size = new System.Drawing.Size(268, 509);
            this.treePackages.TabIndex = 0;
            this.treePackages.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treePackages_AfterSelect);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chkAllToggle);
            this.groupBox1.Controls.Add(this.lstDistributionPoints);
            this.groupBox1.Controls.Add(this.chkHidePxe);
            this.groupBox1.Location = new System.Drawing.Point(277, 38);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(571, 308);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Distribution Nodes";
            // 
            // chkAllToggle
            // 
            this.chkAllToggle.AutoSize = true;
            this.chkAllToggle.Location = new System.Drawing.Point(10, 19);
            this.chkAllToggle.Name = "chkAllToggle";
            this.chkAllToggle.Size = new System.Drawing.Size(15, 14);
            this.chkAllToggle.TabIndex = 2;
            this.chkAllToggle.UseVisualStyleBackColor = true;
            this.chkAllToggle.CheckedChanged += new System.EventHandler(this.chkAllToggle_CheckedChanged);
            // 
            // lstDistributionPoints
            // 
            this.lstDistributionPoints.FormattingEnabled = true;
            this.lstDistributionPoints.Location = new System.Drawing.Point(7, 44);
            this.lstDistributionPoints.Name = "lstDistributionPoints";
            this.lstDistributionPoints.Size = new System.Drawing.Size(558, 259);
            this.lstDistributionPoints.TabIndex = 1;
            this.lstDistributionPoints.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.lstDistributionPoints_ItemCheck);
            // 
            // chkHidePxe
            // 
            this.chkHidePxe.AutoSize = true;
            this.chkHidePxe.Location = new System.Drawing.Point(432, 19);
            this.chkHidePxe.Name = "chkHidePxe";
            this.chkHidePxe.Size = new System.Drawing.Size(133, 17);
            this.chkHidePxe.TabIndex = 0;
            this.chkHidePxe.Text = "Hide PXEImage nodes";
            this.chkHidePxe.UseVisualStyleBackColor = true;
            this.chkHidePxe.CheckedChanged += new System.EventHandler(this.chkHidePxe_CheckedChanged);
            // 
            // btnApply
            // 
            this.btnApply.Location = new System.Drawing.Point(764, 518);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(83, 23);
            this.btnApply.TabIndex = 2;
            this.btnApply.Text = "Save && Apply";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(284, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(82, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Server Address:";
            // 
            // txtServerName
            // 
            this.txtServerName.Location = new System.Drawing.Point(373, 12);
            this.txtServerName.Name = "txtServerName";
            this.txtServerName.Size = new System.Drawing.Size(384, 20);
            this.txtServerName.TabIndex = 4;
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(764, 13);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(84, 19);
            this.btnLoad.TabIndex = 5;
            this.btnLoad.Text = "Load";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // imgList
            // 
            this.imgList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imgList.ImageSize = new System.Drawing.Size(16, 16);
            this.imgList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // btnExpand
            // 
            this.btnExpand.Location = new System.Drawing.Point(13, 519);
            this.btnExpand.Name = "btnExpand";
            this.btnExpand.Size = new System.Drawing.Size(75, 23);
            this.btnExpand.TabIndex = 6;
            this.btnExpand.Text = "Expand All";
            this.btnExpand.UseVisualStyleBackColor = true;
            this.btnExpand.Click += new System.EventHandler(this.btnExpand_Click);
            // 
            // btnCloseFolders
            // 
            this.btnCloseFolders.Location = new System.Drawing.Point(94, 519);
            this.btnCloseFolders.Name = "btnCloseFolders";
            this.btnCloseFolders.Size = new System.Drawing.Size(75, 23);
            this.btnCloseFolders.TabIndex = 7;
            this.btnCloseFolders.Text = "Collapse All";
            this.btnCloseFolders.UseVisualStyleBackColor = true;
            this.btnCloseFolders.Click += new System.EventHandler(this.btnCloseFolders_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.lstOutput);
            this.groupBox2.Location = new System.Drawing.Point(277, 353);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(571, 159);
            this.groupBox2.TabIndex = 8;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Output";
            // 
            // lstOutput
            // 
            this.lstOutput.FormattingEnabled = true;
            this.lstOutput.Location = new System.Drawing.Point(6, 20);
            this.lstOutput.Name = "lstOutput";
            this.lstOutput.Size = new System.Drawing.Size(559, 134);
            this.lstOutput.TabIndex = 0;
            this.lstOutput.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lstOutput_MouseDoubleClick);
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(283, 519);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 23);
            this.btnClear.TabIndex = 9;
            this.btnClear.Text = "Clear Output";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnSaveOnly
            // 
            this.btnSaveOnly.Location = new System.Drawing.Point(682, 519);
            this.btnSaveOnly.Name = "btnSaveOnly";
            this.btnSaveOnly.Size = new System.Drawing.Size(75, 23);
            this.btnSaveOnly.TabIndex = 10;
            this.btnSaveOnly.Text = "Save";
            this.btnSaveOnly.UseVisualStyleBackColor = true;
            this.btnSaveOnly.Click += new System.EventHandler(this.button1_Click);
            // 
            // FolderManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(860, 554);
            this.Controls.Add(this.btnSaveOnly);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.btnCloseFolders);
            this.Controls.Add(this.btnExpand);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.txtServerName);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.treePackages);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "FolderManager";
            this.Text = "Folder Manager";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView treePackages;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox chkHidePxe;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtServerName;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.CheckedListBox lstDistributionPoints;
        private System.Windows.Forms.ImageList imgList;
        private System.Windows.Forms.Button btnExpand;
        private System.Windows.Forms.Button btnCloseFolders;
        private System.Windows.Forms.CheckBox chkAllToggle;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ListBox lstOutput;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnSaveOnly;
    }
}

