using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeToo
{
	/// <summary>
	/// Responsible for changes made to the registry by MeToo
	/// </summary>
	public class RegistryBuilder
	{
		//Location the menu addition is added to the files types(in HK_Classes_Root
		//* so we can really fuck shit up
		private const string MenuName = "*\\shell\\MeToo";
		private const string RegPrefix = "MeToo."; //Prefix to name all of our submenus to keep them seperate
		//Location the Submenus are stored, in HKLM
		private const string SubmenuPath = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Explorer\\CommandStore\\Shell";

		public RegistryBuilder() { }

		/// <summary>
		/// Sets up a Me Too! and Configure submenu context addition
		/// </summary>
		public void buildInitialMenu() {
			RegistryKey regmenu = null;
			RegistryKey regcmd = null;

			try {
				//Setup reg keys, starts with only the config and main option, copysets are created dynamically
				if (Registry.ClassesRoot.OpenSubKey(MenuName) == null) {
					regmenu = Registry.ClassesRoot.CreateSubKey(MenuName);
					if (regmenu != null) {
						regmenu.SetValue("MUIVerb", "Me Too!");
						regmenu.SetValue("SubCommands", RegPrefix + "Configure");
					}
				}

				//Sub menu is setup in a completely different location
				string submenuKey = SubmenuPath + "\\" + RegPrefix + "Configure";

				if ((regcmd = Registry.LocalMachine.OpenSubKey(submenuKey)) == null) {
					//Create the subkey to be linked to
					regcmd = Registry.LocalMachine.CreateSubKey(submenuKey);
					if (regcmd != null) {
						//Set the name
						regcmd.SetValue("", "Configure");
						//Create the command subkey as the action to be performed
						var subCommand = regcmd.CreateSubKey("command");
						subCommand.SetValue("", "metoo configure");
					}
				}
			}
			catch (Exception ex) {
				throw ex;
			}
			finally {
				if(regmenu != null)	regmenu.Close();
				if(regcmd != null)	regcmd.Close();
			}
		}

		public void buildSubMenuLink(IEnumerable<string> linkNames) {
			StringBuilder sb = new StringBuilder();

			if(linkNames.Count() > 0) {
				foreach (var item in linkNames) {
					sb.Append(RegPrefix + item + ";");
				}
				//<HR /> for context menu
				sb.Append("|;");
			}
			
			//Always append the configure menu link
			sb.Append(RegPrefix + "Configure");

			//Write the new menu to the registry
			RegistryKey regmenu = Registry.ClassesRoot.OpenSubKey(MenuName, true);
			regmenu.SetValue("SubCommands", sb.ToString());
		}

		/// <summary>
		/// Adds a submenu key that can then be included in the submenu
		/// </summary>
		/// <param name="name"></param>
		/// <param name="command"></param>
		public void buildSubmenu(String name, String nameSpaceless) {
			string submenuKey = SubmenuPath + "\\" + RegPrefix + nameSpaceless;
			string command = String.Format("{0} {1} \"%1\"", MeToo.BinName, nameSpaceless);

			RegistryKey menuKey = Registry.LocalMachine.OpenSubKey(submenuKey);
			if (menuKey == null) {
				//Create the subkey to be linked to
				menuKey = Registry.LocalMachine.CreateSubKey(submenuKey);
			}

			//Set the display name of the command
			menuKey.SetValue("", name);

			//Create the command subkey as the action to be performed
			var subCommand = menuKey.OpenSubKey("command");
			if (subCommand == null) {
				subCommand = menuKey.CreateSubKey("command");
			}

			subCommand.SetValue("", command);
		}

		public List<string> deleteAllSubmenus() {
			//Clear out the link before we delete the keys
			buildSubMenuLink(new List<string>());

			List<string> deletedKeys = new List<string>();

			using (RegistryKey menuKey = Registry.LocalMachine.OpenSubKey(SubmenuPath, true)) {
				string[] allSubKeys = menuKey.GetSubKeyNames();

				foreach (string subKeyName in allSubKeys) {
					if (subKeyName.StartsWith(RegPrefix) && !subKeyName.Equals(RegPrefix + "Configure")) {
						//Make a note that we deleted this key
						deletedKeys.Add(subKeyName);

						//Delete the subkey
						menuKey.DeleteSubKeyTree(subKeyName);
					}
				}
			}

			return deletedKeys;
		}
	}
}
