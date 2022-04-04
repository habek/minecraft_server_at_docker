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
			this.lvServers = new System.Windows.Forms.ListView();
			this.listBox1 = new System.Windows.Forms.ListBox();
			this.comboBox1 = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.button1 = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// lvServers
			// 
			this.lvServers.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.lvServers.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1});
			this.lvServers.Location = new System.Drawing.Point(12, 12);
			this.lvServers.Name = "lvServers";
			this.lvServers.Size = new System.Drawing.Size(207, 740);
			this.lvServers.TabIndex = 0;
			this.lvServers.UseCompatibleStateImageBehavior = false;
			this.lvServers.View = System.Windows.Forms.View.List;
			this.lvServers.SelectedIndexChanged += new System.EventHandler(this.lvServers_SelectedIndexChanged);
			// 
			// listBox1
			// 
			this.listBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.listBox1.FormattingEnabled = true;
			this.listBox1.ItemHeight = 15;
			this.listBox1.Location = new System.Drawing.Point(225, 238);
			this.listBox1.Name = "listBox1";
			this.listBox1.Size = new System.Drawing.Size(944, 514);
			this.listBox1.TabIndex = 1;
			// 
			// comboBox1
			// 
			this.comboBox1.FormattingEnabled = true;
			this.comboBox1.Items.AddRange(new object[] {
            "gamerule",
            "list"});
			this.comboBox1.Location = new System.Drawing.Point(416, 12);
			this.comboBox1.Name = "comboBox1";
			this.comboBox1.Size = new System.Drawing.Size(240, 23);
			this.comboBox1.TabIndex = 2;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(290, 15);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(94, 15);
			this.label1.TabIndex = 3;
			this.label1.Text = "Send command:";
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(674, 13);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(75, 23);
			this.button1.TabIndex = 4;
			this.button1.Text = "Send";
			this.button1.UseVisualStyleBackColor = true;
			// 
			// MainWindow
			// 
			this.ClientSize = new System.Drawing.Size(1181, 764);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.comboBox1);
			this.Controls.Add(this.listBox1);
			this.Controls.Add(this.lvServers);
			this.Name = "MainWindow";
			this.Shown += new System.EventHandler(this.MainWindow_Shown);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		private ListBox listBox1;
		private ComboBox comboBox1;
		private Label label1;
		private Button button1;
	}
}
