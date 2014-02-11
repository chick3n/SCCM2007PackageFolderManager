using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SCCM2007PackageFolderManager
{
    public partial class OutputReader : Form
    {
        public string OutputMessage { set { txtMessage.Text = value; txtMessage.Select(value.Length, 0); } }

        public OutputReader()
        {
            InitializeComponent();
        }


    }
}
