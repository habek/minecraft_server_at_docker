namespace MinecraftServerManager.Windows
{
	partial class ConfigurationWindow
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
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnSave = new System.Windows.Forms.Button();
			this.tbBackupFolder = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.tbDockerHostAddress = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnCancel.Location = new System.Drawing.Point(704, 456);
			this.btnCancel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(123, 43);
			this.btnCancel.TabIndex = 4;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
			// 
			// btnSave
			// 
			this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnSave.Location = new System.Drawing.Point(572, 456);
			this.btnSave.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.btnSave.Name = "btnSave";
			this.btnSave.Size = new System.Drawing.Size(123, 43);
			this.btnSave.TabIndex = 3;
			this.btnSave.Text = "Save";
			this.btnSave.UseVisualStyleBackColor = true;
			this.btnSave.Click += new System.EventHandler(this.BtnSave_Click);
			// 
			// tbBackupFolder
			// 
			this.tbBackupFolder.Location = new System.Drawing.Point(192, 12);
			this.tbBackupFolder.Name = "tbBackupFolder";
			this.tbBackupFolder.Size = new System.Drawing.Size(503, 31);
			this.tbBackupFolder.TabIndex = 5;
			this.tbBackupFolder.VisibleChanged += new System.EventHandler(this.TbBackupFolder_VisibleChanged);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(65, 15);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(121, 25);
			this.label1.TabIndex = 6;
			this.label1.Text = "Backup folder";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(65, 52);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(120, 25);
			this.label2.TabIndex = 8;
			this.label2.Text = "Docker server";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// tbDockerHostAddress
			// 
			this.tbDockerHostAddress.Location = new System.Drawing.Point(192, 49);
			this.tbDockerHostAddress.Name = "tbDockerHostAddress";
			this.tbDockerHostAddress.Size = new System.Drawing.Size(503, 31);
			this.tbDockerHostAddress.TabIndex = 7;
			// 
			// ConfigurationWindow
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(840, 513);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.tbDockerHostAddress);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.tbBackupFolder);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnSave);
			this.Name = "ConfigurationWindow";
			this.Text = "ConfigurationWindow";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private Button btnCancel;
		private Button btnSave;
		private TextBox tbBackupFolder;
		private Label label1;
		private Label label2;
		private TextBox tbDockerHostAddress;
	}
}