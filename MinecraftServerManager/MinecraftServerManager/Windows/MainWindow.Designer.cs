using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftServerManager.Windows
{
    public partial class MainWindow: Form
    {
		private ListView lvServers;

		private void InitializeComponent()
		{
			System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem("itema");
			System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem("");
			System.Windows.Forms.ListViewItem listViewItem3 = new System.Windows.Forms.ListViewItem("");
			System.Windows.Forms.ListViewItem listViewItem4 = new System.Windows.Forms.ListViewItem("");
			System.Windows.Forms.ListViewItem listViewItem5 = new System.Windows.Forms.ListViewItem("tem 4");
			this.lvServers = new System.Windows.Forms.ListView();
			this.listBox1 = new System.Windows.Forms.ListBox();
			this.SuspendLayout();
			// 
			// lvServers
			// 
			this.lvServers.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.lvServers.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1,
            listViewItem2,
            listViewItem3,
            listViewItem4,
            listViewItem5});
			this.lvServers.Location = new System.Drawing.Point(12, 12);
			this.lvServers.Name = "lvServers";
			this.lvServers.Size = new System.Drawing.Size(207, 414);
			this.lvServers.TabIndex = 0;
			this.lvServers.UseCompatibleStateImageBehavior = false;
			this.lvServers.View = System.Windows.Forms.View.List;
			this.lvServers.SelectedIndexChanged += new System.EventHandler(this.lvServers_SelectedIndexChanged);
			// 
			// listBox1
			// 
			this.listBox1.FormattingEnabled = true;
			this.listBox1.ItemHeight = 15;
			this.listBox1.Location = new System.Drawing.Point(225, 317);
			this.listBox1.Name = "listBox1";
			this.listBox1.Size = new System.Drawing.Size(900, 109);
			this.listBox1.TabIndex = 1;
			// 
			// MainWindow
			// 
			this.ClientSize = new System.Drawing.Size(1137, 438);
			this.Controls.Add(this.listBox1);
			this.Controls.Add(this.lvServers);
			this.Name = "MainWindow";
			this.Shown += new System.EventHandler(this.MainWindow_Shown);
			this.ResumeLayout(false);

		}

		private ListBox listBox1;
	}
}
