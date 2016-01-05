using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace MeToo {
	public partial class MeTooView : Form {

		private const string EmptyCSMessage = "- Add a directory to this Copy Set -";
		private readonly bool DEBUG;

		private MeToo LinkBack;
		private List<CopySet> CopySets;

		private bool Modified = false;

		public MeTooView(List<CopySet> copySets, bool debug, MeToo program) {
			this.DEBUG = debug;
			InitializeComponent();

			this.CopySets = copySets;
			this.LinkBack = program;

			if (DEBUG) {
				clearConfigButton.Visible = true;
			}

			//Setup of view component			
			csListView.View = View.Details; 
			csListView.FullRowSelect = true;
			csListView.GridLines = true;
			csListView.MultiSelect = false;
			csListView.HideSelection = false;
			csListView.ItemSelectionChanged += listView1_ItemSelectionChanged;

			//Context dependant buttons
			AddDirButton.Enabled = false;
			RemoveDirButton.Enabled = false;
			RemoveCSButton.Enabled = false;

			//Build the current list of items into the view
			foreach (CopySet cs in copySets) {
				int i = csListView.Groups.Add(new ListViewGroup(cs.Name, HorizontalAlignment.Left));

				foreach (var dir in cs.dirs) {
					//Build the dir and add it to this CS group
					ListViewItem lvi = new ListViewItem(dir);
					csListView.Items.Add(lvi);
					csListView.Groups[i].Items.Add(lvi);
				}
			}
		}

		/// <summary>
		/// Enables the context dependant buttons.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void listView1_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e) {
			//Clear all context dependant buttons to a known state
			AddDirButton.Enabled = false;
			RemoveDirButton.Enabled = false;
			RemoveCSButton.Enabled = false;
			
			//If anything is focused
			if (csListView.SelectedItems.Count > 0) {
				AddDirButton.Enabled = true;
				RemoveCSButton.Enabled = true;

				//Don't allow the removal of a placeholder
				if (csListView.SelectedItems[0].Text != EmptyCSMessage) {
					RemoveDirButton.Enabled = true;
				}
			}
		}

		//Prompts to add a new CopySet 
		private void NewButton_Click(object sender1, EventArgs e1) {
			Form prompt = new Form();
			prompt.Width = 500;
			prompt.Height = 150;
			prompt.FormBorderStyle = FormBorderStyle.FixedDialog;
			prompt.Text = "Create A New Copy Set";
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

				int i = csListView.Groups.Add(new ListViewGroup(textBox.Text, HorizontalAlignment.Left));
				addPlaceHolderDir(csListView.Groups[i]);

				Modified = true;
			}
		}

		/// <summary>
		/// Prompts to add a new directory to the selected CopySet
		/// Uses inbuild winforms folder browser
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void AddDirButton_Click(object sender, EventArgs e) {
			DialogResult result = folderBrowserDialog.ShowDialog();
			if (result == DialogResult.OK) {
				//Remove the placeholder if there is one

				string folder = folderBrowserDialog.SelectedPath;

				foreach (ListViewItem item in csListView.SelectedItems)
				{
					CopySet cs = CopySets.First(f => f.Name == item.Group.Header);

					if (!cs.dirs.Contains(folder)) {
						//Model
						cs.dirs.Add(folder);
						//View
						ListViewItem lvi = new ListViewItem(folder);
						csListView.Items.Add(lvi);
						item.Group.Items.Add(lvi);

						if (item.Text == EmptyCSMessage) item.Remove();

						Modified = true;
					}
				}
			}
		}

		/// <summary>
		/// Propts to store the current copySet back to the application data store and then
		/// closes the configure window. 
		/// If no changes have been made skips the save prompt / operation.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void SaveButton_Click(object sender, EventArgs e) {
			if (Modified) { 
				DialogResult dr = MessageBox.Show("All done? Save and write out?\nThis may do some registry additions / removals",
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

		/// <summary>
		/// Prompts to leave without saving changes if any have been made otherwise
		/// just leaves.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
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

		/// <summary>
		/// Removes CopySet from the model and view
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void RemoveCSButton_Click(object sender, EventArgs e) {
			
			foreach (ListViewItem item in csListView.SelectedItems)
			{
				ListViewGroup group = item.Group;

				//Remove from the model
				CopySets.RemoveAll(f => f.Name == item.Group.Header);

				//Remove from the view
				for (int i = group.Items.Count - 1; i >= 0; i--) {
					group.Items[i].Remove();
				}

				group.Items.Clear();
				csListView.Groups.Remove(group);
	
				Modified = true;
			}
		}

		/// <summary>
		/// Removes directory from its copyset based on what which group the focus was on
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void RemoveDirButton_Click(object sender, EventArgs e) {
				
			foreach (ListViewItem item in csListView.SelectedItems)
			{
				if (item.Text != EmptyCSMessage) {
					//Remove from its parent CS
					CopySet cs = CopySets.First(f => f.Name == item.Group.Header);
					if (cs != null) {
						cs.dirs.Remove(item.Text);
						Modified = true;
					}

					if (item.Group.Items.Count == 1) addPlaceHolderDir(item.Group);
					//Remove from the view
					item.Remove();
				}
			}
		}

		/// <summary>
		/// Adds a placeholder so the group will show and be clickable.
		/// ListViews winforms do not allow a completely empty group to be clickable.
		/// </summary>
		/// <param name="group"></param>
		private void addPlaceHolderDir(ListViewGroup group) {
			//Add in the spacing item
			ListViewItem lvi = new ListViewItem(EmptyCSMessage);
			csListView.Items.Add(lvi);
			group.Items.Add(lvi);
			lvi.Selected = true;
			lvi.Focused = true;
		}

		/// <summary>
		/// Debug option
		/// Wipes all of the application datastore configureation and registry keys.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void clearConfigButton_Click(object sender, EventArgs e) {
			//TODO Dan
		}
	}
}
