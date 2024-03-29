﻿using System;
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
			this.lvServers = new System.Windows.Forms.ListView();
			this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader5 = new System.Windows.Forms.ColumnHeader();
			this.listBox1 = new System.Windows.Forms.ListBox();
			this.cmbCommandToSend = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.btnSendCommand = new System.Windows.Forms.Button();
			this.btnOpenProperties = new System.Windows.Forms.Button();
			this.btnServerStop = new System.Windows.Forms.Button();
			this.btnListUsers = new System.Windows.Forms.Button();
			this.lvUsers = new System.Windows.Forms.ListView();
			this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
			this.btnEditPermissions = new System.Windows.Forms.Button();
			this.btnServerStart = new System.Windows.Forms.Button();
			this.btnBackup = new System.Windows.Forms.Button();
			this.btnRestore = new System.Windows.Forms.Button();
			this.btnSettings = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.tbVersion = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// lvServers
			// 
			this.lvServers.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.lvServers.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader4,
            this.columnHeader5});
			this.lvServers.FullRowSelect = true;
			this.lvServers.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.lvServers.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1});
			this.lvServers.Location = new System.Drawing.Point(12, 12);
			this.lvServers.MultiSelect = false;
			this.lvServers.Name = "lvServers";
			this.lvServers.Size = new System.Drawing.Size(207, 888);
			this.lvServers.Sorting = System.Windows.Forms.SortOrder.Ascending;
			this.lvServers.TabIndex = 0;
			this.lvServers.UseCompatibleStateImageBehavior = false;
			this.lvServers.View = System.Windows.Forms.View.Details;
			this.lvServers.SelectedIndexChanged += new System.EventHandler(this.LvServers_SelectedIndexChanged);
			// 
			// columnHeader4
			// 
			this.columnHeader4.Text = "Name";
			this.columnHeader4.Width = 120;
			// 
			// columnHeader5
			// 
			this.columnHeader5.Text = "State";
			this.columnHeader5.Width = 40;
			// 
			// listBox1
			// 
			this.listBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.listBox1.FormattingEnabled = true;
			this.listBox1.HorizontalScrollbar = true;
			this.listBox1.ItemHeight = 25;
			this.listBox1.Location = new System.Drawing.Point(225, 238);
			this.listBox1.Name = "listBox1";
			this.listBox1.ScrollAlwaysVisible = true;
			this.listBox1.Size = new System.Drawing.Size(1172, 629);
			this.listBox1.TabIndex = 1;
			// 
			// cmbCommandToSend
			// 
			this.cmbCommandToSend.FormattingEnabled = true;
			this.cmbCommandToSend.Items.AddRange(new object[] {
            "gamerule",
            "list",
            "stop"});
			this.cmbCommandToSend.Location = new System.Drawing.Point(416, 12);
			this.cmbCommandToSend.Name = "cmbCommandToSend";
			this.cmbCommandToSend.Size = new System.Drawing.Size(240, 33);
			this.cmbCommandToSend.TabIndex = 2;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(290, 15);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(142, 25);
			this.label1.TabIndex = 3;
			this.label1.Text = "Send command:";
			// 
			// btnSendCommand
			// 
			this.btnSendCommand.Location = new System.Drawing.Point(674, 13);
			this.btnSendCommand.Name = "btnSendCommand";
			this.btnSendCommand.Size = new System.Drawing.Size(75, 23);
			this.btnSendCommand.TabIndex = 4;
			this.btnSendCommand.Text = "Send";
			this.btnSendCommand.UseVisualStyleBackColor = true;
			this.btnSendCommand.Click += new System.EventHandler(this.BtnSendCommand_Click);
			// 
			// btnOpenProperties
			// 
			this.btnOpenProperties.Location = new System.Drawing.Point(225, 54);
			this.btnOpenProperties.Name = "btnOpenProperties";
			this.btnOpenProperties.Size = new System.Drawing.Size(102, 23);
			this.btnOpenProperties.TabIndex = 5;
			this.btnOpenProperties.Text = "Edit properties";
			this.btnOpenProperties.UseVisualStyleBackColor = true;
			this.btnOpenProperties.Click += new System.EventHandler(this.BtnOpenProperties_Click);
			// 
			// btnServerStop
			// 
			this.btnServerStop.Location = new System.Drawing.Point(1072, 15);
			this.btnServerStop.Name = "btnServerStop";
			this.btnServerStop.Size = new System.Drawing.Size(97, 23);
			this.btnServerStop.TabIndex = 6;
			this.btnServerStop.Text = "Stop Server";
			this.btnServerStop.UseVisualStyleBackColor = true;
			this.btnServerStop.Click += new System.EventHandler(this.BtnRestart_Click);
			// 
			// btnListUsers
			// 
			this.btnListUsers.Location = new System.Drawing.Point(674, 54);
			this.btnListUsers.Name = "btnListUsers";
			this.btnListUsers.Size = new System.Drawing.Size(75, 23);
			this.btnListUsers.TabIndex = 7;
			this.btnListUsers.Text = "List users";
			this.btnListUsers.UseVisualStyleBackColor = true;
			this.btnListUsers.Click += new System.EventHandler(this.BtnListUsers_Click);
			// 
			// lvUsers
			// 
			this.lvUsers.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.lvUsers.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
			this.lvUsers.FullRowSelect = true;
			this.lvUsers.GridLines = true;
			this.lvUsers.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.lvUsers.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem2});
			this.lvUsers.Location = new System.Drawing.Point(225, 131);
			this.lvUsers.Name = "lvUsers";
			this.lvUsers.ShowGroups = false;
			this.lvUsers.Size = new System.Drawing.Size(547, 101);
			this.lvUsers.Sorting = System.Windows.Forms.SortOrder.Ascending;
			this.lvUsers.TabIndex = 8;
			this.lvUsers.UseCompatibleStateImageBehavior = false;
			this.lvUsers.View = System.Windows.Forms.View.Details;
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "User";
			this.columnHeader1.Width = 300;
			// 
			// columnHeader2
			// 
			this.columnHeader2.Text = "Xuid";
			this.columnHeader2.Width = 120;
			// 
			// columnHeader3
			// 
			this.columnHeader3.Text = "Permission";
			this.columnHeader3.Width = 120;
			// 
			// btnEditPermissions
			// 
			this.btnEditPermissions.Location = new System.Drawing.Point(225, 83);
			this.btnEditPermissions.Name = "btnEditPermissions";
			this.btnEditPermissions.Size = new System.Drawing.Size(102, 23);
			this.btnEditPermissions.TabIndex = 9;
			this.btnEditPermissions.Text = "Permisions...";
			this.btnEditPermissions.UseVisualStyleBackColor = true;
			this.btnEditPermissions.Click += new System.EventHandler(this.BtnEditPermissions_Click);
			// 
			// btnServerStart
			// 
			this.btnServerStart.Location = new System.Drawing.Point(1072, 44);
			this.btnServerStart.Name = "btnServerStart";
			this.btnServerStart.Size = new System.Drawing.Size(97, 23);
			this.btnServerStart.TabIndex = 10;
			this.btnServerStart.Text = "Start Server";
			this.btnServerStart.UseVisualStyleBackColor = true;
			this.btnServerStart.Click += new System.EventHandler(this.BtnServerStart_Click);
			// 
			// btnBackup
			// 
			this.btnBackup.Location = new System.Drawing.Point(1072, 73);
			this.btnBackup.Name = "btnBackup";
			this.btnBackup.Size = new System.Drawing.Size(97, 23);
			this.btnBackup.TabIndex = 11;
			this.btnBackup.Text = "Backup";
			this.btnBackup.UseVisualStyleBackColor = true;
			this.btnBackup.Click += new System.EventHandler(this.BtnBackup_Click);
			// 
			// btnRestore
			// 
			this.btnRestore.Location = new System.Drawing.Point(1072, 102);
			this.btnRestore.Name = "btnRestore";
			this.btnRestore.Size = new System.Drawing.Size(97, 23);
			this.btnRestore.TabIndex = 12;
			this.btnRestore.Text = "Restore";
			this.btnRestore.UseVisualStyleBackColor = true;
			this.btnRestore.Click += new System.EventHandler(this.BtnRestore_Click);
			// 
			// btnSettings
			// 
			this.btnSettings.Location = new System.Drawing.Point(969, 15);
			this.btnSettings.Name = "btnSettings";
			this.btnSettings.Size = new System.Drawing.Size(97, 23);
			this.btnSettings.TabIndex = 13;
			this.btnSettings.Text = "Settings";
			this.btnSettings.UseVisualStyleBackColor = true;
			this.btnSettings.Click += new System.EventHandler(this.BtnSettings_Click);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(785, 131);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(121, 31);
			this.label2.TabIndex = 15;
			this.label2.Text = "Version";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// tbVersion
			// 
			this.tbVersion.Location = new System.Drawing.Point(912, 131);
			this.tbVersion.Name = "tbVersion";
			this.tbVersion.ReadOnly = true;
			this.tbVersion.Size = new System.Drawing.Size(164, 31);
			this.tbVersion.TabIndex = 14;
			// 
			// MainWindow
			// 
			this.ClientSize = new System.Drawing.Size(1409, 912);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.tbVersion);
			this.Controls.Add(this.btnSettings);
			this.Controls.Add(this.btnRestore);
			this.Controls.Add(this.btnBackup);
			this.Controls.Add(this.btnServerStart);
			this.Controls.Add(this.btnEditPermissions);
			this.Controls.Add(this.lvUsers);
			this.Controls.Add(this.btnListUsers);
			this.Controls.Add(this.btnServerStop);
			this.Controls.Add(this.btnOpenProperties);
			this.Controls.Add(this.btnSendCommand);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.cmbCommandToSend);
			this.Controls.Add(this.listBox1);
			this.Controls.Add(this.lvServers);
			this.Name = "MainWindow";
			this.Shown += new System.EventHandler(this.MainWindow_Shown);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		private ListBox listBox1;
		private ComboBox cmbCommandToSend;
		private Label label1;
		private Button btnSendCommand;
		private Button btnOpenProperties;
		private Button btnServerStop;
		private Button btnListUsers;
		private ListView lvUsers;
		private Button btnEditPermissions;
		private ColumnHeader columnHeader1;
		private ColumnHeader columnHeader2;
		private ColumnHeader columnHeader3;
		private Button btnServerStart;
		private ColumnHeader columnHeader4;
		private ColumnHeader columnHeader5;
		private Button btnBackup;
		private Button btnRestore;
		private Button btnSettings;
		private Label label2;
		private TextBox tbVersion;
	}
}
