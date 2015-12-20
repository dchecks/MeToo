namespace MeToo {
	partial class Form1 {
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
			this.SaveButton = new System.Windows.Forms.Button();
			this.AddDirButton = new System.Windows.Forms.Button();
			this.NewButton = new System.Windows.Forms.Button();
			this.csListView = new System.Windows.Forms.ListView();
			this.cancelButton = new System.Windows.Forms.Button();
			this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
			this.RemoveDirButton = new System.Windows.Forms.Button();
			this.RemoveCSButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// SaveButton
			// 
			this.SaveButton.Location = new System.Drawing.Point(338, 281);
			this.SaveButton.Name = "SaveButton";
			this.SaveButton.Size = new System.Drawing.Size(75, 23);
			this.SaveButton.TabIndex = 0;
			this.SaveButton.Text = "Done";
			this.SaveButton.UseVisualStyleBackColor = true;
			this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
			// 
			// AddDirButton
			// 
			this.AddDirButton.Location = new System.Drawing.Point(14, 281);
			this.AddDirButton.Name = "AddDirButton";
			this.AddDirButton.Size = new System.Drawing.Size(75, 23);
			this.AddDirButton.TabIndex = 1;
			this.AddDirButton.Text = "Add Dir";
			this.AddDirButton.UseVisualStyleBackColor = true;
			this.AddDirButton.Click += new System.EventHandler(this.AddDirButton_Click);
			// 
			// NewButton
			// 
			this.NewButton.Location = new System.Drawing.Point(13, 252);
			this.NewButton.Name = "NewButton";
			this.NewButton.Size = new System.Drawing.Size(75, 23);
			this.NewButton.TabIndex = 2;
			this.NewButton.Text = "New CS";
			this.NewButton.UseVisualStyleBackColor = true;
			this.NewButton.Click += new System.EventHandler(this.NewButton_Click);
			// 
			// csListView
			// 
			this.csListView.Location = new System.Drawing.Point(13, 13);
			this.csListView.Name = "csListView";
			this.csListView.Size = new System.Drawing.Size(481, 221);
			this.csListView.TabIndex = 3;
			this.csListView.UseCompatibleStateImageBehavior = false;
			// 
			// cancelButton
			// 
			this.cancelButton.Location = new System.Drawing.Point(419, 281);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(75, 23);
			this.cancelButton.TabIndex = 4;
			this.cancelButton.Text = "Cancel";
			this.cancelButton.UseVisualStyleBackColor = true;
			this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
			// 
			// RemoveDirButton
			// 
			this.RemoveDirButton.Location = new System.Drawing.Point(95, 281);
			this.RemoveDirButton.Name = "RemoveDirButton";
			this.RemoveDirButton.Size = new System.Drawing.Size(75, 23);
			this.RemoveDirButton.TabIndex = 5;
			this.RemoveDirButton.Text = "Remove Dir";
			this.RemoveDirButton.UseVisualStyleBackColor = true;
			this.RemoveDirButton.Click += new System.EventHandler(this.RemoveDirButton_Click);
			// 
			// RemoveCSButton
			// 
			this.RemoveCSButton.Location = new System.Drawing.Point(95, 252);
			this.RemoveCSButton.Name = "RemoveCSButton";
			this.RemoveCSButton.Size = new System.Drawing.Size(75, 23);
			this.RemoveCSButton.TabIndex = 6;
			this.RemoveCSButton.Text = "Remove CS";
			this.RemoveCSButton.UseVisualStyleBackColor = true;
			this.RemoveCSButton.Click += new System.EventHandler(this.RemoveCSButton_Click);
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(504, 323);
			this.Controls.Add(this.RemoveCSButton);
			this.Controls.Add(this.RemoveDirButton);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.csListView);
			this.Controls.Add(this.NewButton);
			this.Controls.Add(this.AddDirButton);
			this.Controls.Add(this.SaveButton);
			this.Name = "Form1";
			this.Text = "Form1";
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

	}
}

