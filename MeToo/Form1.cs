using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MeToo {
	public partial class Form1 : Form {
			
		private MeToo LinkBack;
		private List<CopySet> CopySets;
		private CopySet currentCs;

		private bool Modified = false;

		public Form1(List<CopySet> copySets, MeToo program) {
			InitializeComponent();

			this.CopySets = copySets;
			this.LinkBack = program;

			//Build the current list of items into the view
			foreach (CopySet cs in copySets) {
				ListViewItem item = new ListViewItem(cs.Name);
				foreach (var dir in cs.dirs) {
					item.SubItems.Add(dir);
				}

				csListView.Items.Add(item);
			}
		}

		//Prompts to add a new CopySet 
		private void NewButton_Click(object sender1, EventArgs e1) {
			Form prompt = new Form();
			prompt.Width = 500;
			prompt.Height = 150;
			prompt.FormBorderStyle = FormBorderStyle.FixedDialog;
			prompt.Text = "Create A New CopySet";
			prompt.StartPosition = FormStartPosition.CenterScreen;
			Label textLabel = new Label() { Left = 50, Top=20, Width=250, Text="Enter a name for the context menu addition"  };
			TextBox textBox = new TextBox() { Left = 50, Top=50, Width=400 };
			Button confirmation = new Button() { Text = "Ok", Left=350, Width=100, Top=70, DialogResult = DialogResult.OK };
			confirmation.Click += (sender, e) => { prompt.Close(); };
			prompt.Controls.Add(textBox);
			prompt.Controls.Add(confirmation);
			prompt.Controls.Add(textLabel);
			prompt.AcceptButton = confirmation;

			if(prompt.ShowDialog() == DialogResult.OK){
				CopySets.Add(new CopySet(textBox.Text));

				csListView.Items.Add(new ListViewItem(textBox.Text));
				Modified = true;
			}
		}

		//Promps to add a new directory to the selected CopySet
		private void AddDirButton_Click(object sender, EventArgs e) {
			DialogResult result = folderBrowserDialog.ShowDialog();
			if (result == DialogResult.OK) {
				string folder = folderBrowserDialog.SelectedPath;

				foreach (ListViewItem item in csListView.SelectedItems)
				{
					CopySet cs = CopySets.First(f => f.Name == item.Text);

					if (!cs.dirs.Contains(folder)) {
						//Model
						cs.dirs.Add(folder);
						//View
						item.SubItems.Add(folder);

						Modified = true;
					}
				}
			}
		}

		//Stores the current copySet back to the application data store
		private void SaveButton_Click(object sender, EventArgs e) {
			if (Modified) { 
				DialogResult dr = MessageBox.Show("All done? Save and write out? (Expect an escilator ride)",
													"Save Changes And Exit?",
													 MessageBoxButtons.YesNo,
													 MessageBoxIcon.Question);
				if (dr == DialogResult.Yes) {
					//Save if need be
					if (LinkBack.SaveSettings(CopySets)) {
						this.Close();
					}
					else {
						MessageBox.Show("Failed to save changes. Soz.",
									"Save Operation Failed",
									MessageBoxButtons.OK,
									MessageBoxIcon.Error);
					}
				}
				//Else do nothing
			}
			else this.Close();
		}

		private void cancelButton_Click(object sender, EventArgs e) {
			if (Modified) { 
				DialogResult dr = MessageBox.Show("Don't Save? Just Leave???", "Cancel Changes?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
				if (dr == DialogResult.Yes) {
					this.Close();
				}
				//Else do nothing
			}
			else this.Close();
		}

		private void RemoveCSButton_Click(object sender, EventArgs e) {
			
			foreach (ListViewItem item in csListView.SelectedItems)
			{
				//Remove from the view
				item.Remove();
				
				//Remove from the model
				//TODO Make sure this is fine given that were replacing the list
				CopySets = CopySets.Where(f => f.Name != Name).ToList();

				Modified = true;
			}
			
		}

		private void RemoveDirButton_Click(object sender, EventArgs e) {
				
			foreach (ListViewItem item in csListView.SelectedItems)
			{
				//Remove from the view
				item.Remove();

				//Remove from its parent CS
				CopySet cs = CopySets.First(f => f.Name == Name);
				if(cs != null) {
					cs.dirs.Remove(item.Text);
					Modified = true;
				}
			}
		}
	}
}
