namespace MeToo {
	partial class MeTooView {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			System.Windows.Forms.ListViewGroup listViewGroup1 = new System.Windows.Forms.ListViewGroup("ListViewGroup", System.Windows.Forms.HorizontalAlignment.Left);
			this.SaveButton = new System.Windows.Forms.Button();
			this.AddDirButton = new System.Windows.Forms.Button();
			this.NewButton = new System.Windows.Forms.Button();
			this.csListView = new System.Windows.Forms.ListView();
			this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.cancelButton = new System.Windows.Forms.Button();
			this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
			this.RemoveDirButton = new System.Windows.Forms.Button();
			this.RemoveCSButton = new System.Windows.Forms.Button();
			this.clearConfigButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// SaveButton
			// 
			this.SaveButton.Location = new System.Drawing.Point(507, 432);
			this.SaveButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.SaveButton.Name = "SaveButton";
			this.SaveButton.Size = new System.Drawing.Size(112, 35);
			this.SaveButton.TabIndex = 0;
			this.SaveButton.Text = "Done";
			this.SaveButton.UseVisualStyleBackColor = true;
			this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
			// 
			// AddDirButton
			// 
			this.AddDirButton.Location = new System.Drawing.Point(21, 432);
			this.AddDirButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.AddDirButton.Name = "AddDirButton";
			this.AddDirButton.Size = new System.Drawing.Size(112, 35);
			this.AddDirButton.TabIndex = 1;
			this.AddDirButton.Text = "Add Dir";
			this.AddDirButton.UseVisualStyleBackColor = true;
			this.AddDirButton.Click += new System.EventHandler(this.AddDirButton_Click);
			// 
			// NewButton
			// 
			this.NewButton.Location = new System.Drawing.Point(20, 388);
			this.NewButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.NewButton.Name = "NewButton";
			this.NewButton.Size = new System.Drawing.Size(112, 35);
			this.NewButton.TabIndex = 2;
			this.NewButton.Text = "New CS";
			this.NewButton.UseVisualStyleBackColor = true;
			this.NewButton.Click += new System.EventHandler(this.NewButton_Click);
			// 
			// csListView
			// 
			this.csListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
			listViewGroup1.Header = "ListViewGroup";
			listViewGroup1.Name = "listViewGroup1";
			this.csListView.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup1});
			this.csListView.Location = new System.Drawing.Point(20, 20);
			this.csListView.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.csListView.Name = "csListView";
			this.csListView.Size = new System.Drawing.Size(720, 338);
			this.csListView.TabIndex = 3;
			this.csListView.UseCompatibleStateImageBehavior = false;
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "Copy Sets";
			this.columnHeader1.Width = -2;
			// 
			// cancelButton
			// 
			this.cancelButton.Location = new System.Drawing.Point(628, 432);
			this.cancelButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(112, 35);
			this.cancelButton.TabIndex = 4;
			this.cancelButton.Text = "Cancel";
			this.cancelButton.UseVisualStyleBackColor = true;
			this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
			// 
			// RemoveDirButton
			// 
			this.RemoveDirButton.Location = new System.Drawing.Point(142, 432);
			this.RemoveDirButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.RemoveDirButton.Name = "RemoveDirButton";
			this.RemoveDirButton.Size = new System.Drawing.Size(112, 35);
			this.RemoveDirButton.TabIndex = 5;
			this.RemoveDirButton.Text = "Remove Dir";
			this.RemoveDirButton.UseVisualStyleBackColor = true;
			this.RemoveDirButton.Click += new System.EventHandler(this.RemoveDirButton_Click);
			// 
			// RemoveCSButton
			// 
			this.RemoveCSButton.Location = new System.Drawing.Point(142, 388);
			this.RemoveCSButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.RemoveCSButton.Name = "RemoveCSButton";
			this.RemoveCSButton.Size = new System.Drawing.Size(112, 35);
			this.RemoveCSButton.TabIndex = 6;
			this.RemoveCSButton.Text = "Remove CS";
			this.RemoveCSButton.UseVisualStyleBackColor = true;
			this.RemoveCSButton.Click += new System.EventHandler(this.RemoveCSButton_Click);
			// 
			// clearConfigButton
			// 
			this.clearConfigButton.Location = new System.Drawing.Point(507, 388);
			this.clearConfigButton.Name = "clearConfigButton";
			this.clearConfigButton.Size = new System.Drawing.Size(112, 36);
			this.clearConfigButton.TabIndex = 7;
			this.clearConfigButton.Text = "Clear Config";
			this.clearConfigButton.UseVisualStyleBackColor = true;
			this.clearConfigButton.Visible = false;
			this.clearConfigButton.Click += new System.EventHandler(this.clearConfigButton_Click);
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(756, 497);
			this.Controls.Add(this.clearConfigButton);
			this.Controls.Add(this.RemoveCSButton);
			this.Controls.Add(this.RemoveDirButton);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.csListView);
			this.Controls.Add(this.NewButton);
			this.Controls.Add(this.AddDirButton);
			this.Controls.Add(this.SaveButton);
			this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.Name = "Me Too!";
			this.Text = "Me Too!";
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button SaveButton;
		private System.Windows.Forms.Button AddDirButton;
		private System.Windows.Forms.Button NewButton;
		private System.Windows.Forms.ListView csListView;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
		private System.Windows.Forms.Button RemoveDirButton;
		private System.Windows.Forms.Button RemoveCSButton;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.Button clearConfigButton;

	}
}

