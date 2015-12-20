using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace MeToo {
	public class MeToo {
		
		//* so we can really fuck shit up
		private const string MenuName = "*\\shell\\MeToo";
		private const string RegPrefix = "MeToo."; //Prefix to name all of our submenus to keep them seperate
		//Location the Submenus are stored, in HKLM
		private const string SubmenuPath = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Explorer\\CommandStore\\Shell";

		private const string SettingsFileLocation = "settings.xml";
		private const string BinName = "metoo.exe";

		//private IsolatedStorageFileStream SettingsFile;
		private Dictionary<String, CopySet> CopySets;

		public MeToo() {
			//LoadConfig
			XmlDocument xDoc = loadSettings();
			CopySets = buildCopySets(xDoc);
		}

		/// <summary>
		/// Loads the settings xml doc from the isolated storage file
		/// </summary>
		/// <returns>Contents of the settings file</returns>
		private XmlDocument loadSettings() {
			using (IsolatedStorageFile settings = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null)) { 
				IsolatedStorageFileStream SettingsFile = settings.OpenFile(SettingsFileLocation, FileMode.OpenOrCreate);

				XmlDocument xSettings = new XmlDocument();
				using(StreamReader sr = new StreamReader(SettingsFile)) {
					string xStr = sr.ReadToEnd();
					if(xStr.Length > 0) xSettings.LoadXml(xStr);

					return xSettings;
				}
			}
		}

		/// <summary>
		/// Builds CopySet objects from the given settings file
		/// </summary>
		/// <param name="xSettings"></param>
		/// <returns></returns>
		private Dictionary<string, CopySet> buildCopySets(XmlDocument xSettings) {
			Dictionary<String, CopySet> cSets = new Dictionary<String, CopySet>();

			XmlNodeList sets = xSettings.GetElementsByTagName("COPYSET");
			
			foreach(XmlNode node in sets) {
				string setName = node.SelectSingleNode("//NAME").InnerText;

				List<String> dirs = new List<String>();
				foreach (XmlNode dir in node.SelectNodes("//DIR")) {
					dirs.Add(dir.InnerText);
				}

				cSets[setName] = new CopySet(setName, dirs);
			}

			return cSets;
		}

		/// <summary>
		/// Displays the configuration form
		/// </summary>
		private void startGui() {
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			List<CopySet> inputList = new List<CopySet>(CopySets.Values);
			Application.Run(new Form1(inputList, this));
		}

		/// <summary>
		/// Copys the source binary to windir and inserts the default reg keys if necessary
		/// 
		/// Needs to be run as admin as it does some nasty shit
		/// </summary>
		/// <param name="programLocation">source bin to copy</param>
		//[PrincipalPermission(SecurityAction.Demand, Role = @"BUILTIN\Administrators")]
		private void addContextAddition(string programLocation) {
			RegistryKey regmenu = null;
			RegistryKey regcmd = null;

			try	{
				//Copy itself to Windows Dir so the program can be called from the context menu
				string winDir = Environment.GetEnvironmentVariable("WINDIR");

				if (!programLocation.StartsWith(winDir)) {
					File.Copy(programLocation, winDir + BinName, true);
				} else {
					//No need to make another copy
					//Currently do nothing
				}

				//Setup reg keys, starts with only the config and main option, copysets are created dynamically
				if (Registry.ClassesRoot.OpenSubKey(MenuName) == null) {
					regmenu = Registry.ClassesRoot.CreateSubKey(MenuName);
					if (regmenu != null) {
						regmenu.SetValue("MUIVerb", "Me Too!");
						regmenu.SetValue("SubCommands", RegPrefix + "Configure");
					}

					//Sub menu is setup in a completely different location
					string submenuKey = SubmenuPath + "\\" + RegPrefix + "Configure";

					if (Registry.LocalMachine.OpenSubKey(submenuKey) == null) {
						//Create the subkey to be linked to
						regcmd = Registry.LocalMachine.CreateSubKey(submenuKey);
						if (regcmd != null) {
							regcmd.SetValue("","Configure");
							//Create the command subkey as the action to be performed
							var subCommand = regcmd.CreateSubKey("command");
							subCommand.SetValue("", "metoo configure");
						}
					}
				}
			}
			catch(Exception ex)	{
				//On Failure write out an error log to a temp file and then display a somewhat helpful error message
				string tempLocation = Environment.GetEnvironmentVariable("TEMP");
				string failureLogName = tempLocation + "\\MeTooError-" + DateTime.UtcNow.ToString();

				using (StreamWriter errorLog = File.CreateText(tempLocation + "\\" + failureLogName)) {
					errorLog.WriteLine("Exception occured whilst attempting to build context menu by writing registry keys");
					errorLog.WriteLine(ex.Message);
					errorLog.WriteLine(ex.StackTrace.ToString());
					errorLog.Flush();
					errorLog.Close();
				}

				MessageBox.Show("A failure occured setting up the registry keys for the context menu, exception written to: " + failureLogName, 
								"Critical Failure", 
								MessageBoxButtons.OK,
								MessageBoxIcon.Error);
			}
			finally {
				if(regmenu != null)	regmenu.Close();
				if(regcmd != null)	regcmd.Close();
			}
		}

		/// <summary>
		/// Builds the list of submenu commands to be included in the submenu
		/// </summary>
		/// <param name="copySets"></param>
		private void buildSubmenuList(List<CopySet> copySets) {
			
			StringBuilder sb = new StringBuilder();

			if(copySets.Count > 0) {
				foreach (var item in copySets) {
					sb.Append(RegPrefix + item.NameSpaceless + ";");
				}
				sb.Append("|;");
			}
			
			sb.Append(RegPrefix + "Configure");

			//Write the new menu to the registry
			RegistryKey regmenu = Registry.ClassesRoot.OpenSubKey(MenuName);
			regmenu.SetValue("SubCommands", sb.ToString());
		}

		/// <summary>
		/// Adds a submenu key that can then be included in the submenu
		/// </summary>
		/// <param name="name"></param>
		/// <param name="command"></param>
		private void addSubmenu(CopySet cs) {
			try {
				string submenuKey = SubmenuPath + "\\" + RegPrefix + cs.NameSpaceless;
				string command = String.Format("{0} {1} \"%1\"", BinName, cs.NameSpaceless);

				RegistryKey menuKey = Registry.LocalMachine.OpenSubKey(submenuKey);
				if (menuKey == null) {
					//Create the subkey to be linked to
					menuKey = Registry.LocalMachine.CreateSubKey(submenuKey);
				}
				
				//Set the display name of the command
				menuKey.SetValue("", cs.Name);

				//Create the command subkey as the action to be performed
				var subCommand = menuKey.OpenSubKey("command");
				if(subCommand == null) {
					subCommand = menuKey.CreateSubKey("command");
				}

				subCommand.SetValue("", command);
			} catch (Exception ex) {
				//TODO Srs

			}
		}


		/// <summary>
		/// Saves and builds the current config
		/// </summary>
		public bool SaveSettings(List<CopySet> copySets) {
			
			//Write out the config
			using (IsolatedStorageFile settings = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null)) { 
				using(IsolatedStorageFileStream settingsFile = settings.OpenFile(SettingsFileLocation, FileMode.Truncate, FileAccess.Write)){
					//Serialize each of the sets
					XmlDocument xDoc = new XmlDocument();
					XmlSerializer xSez = new XmlSerializer(typeof(CopySet), new XmlRootAttribute("COPYSETS"));
					
					//Dont write out with any crappy namespaces
					XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
					ns.Add("", "");

					foreach (var cs in copySets)
					{
						xSez.Serialize(settingsFile, cs, ns);
					}

				}
			}
			
			//Delete all existing context submenus
			//TODO Registry.LocalMachine.

			//Update/Replace/Create context menu entries
			foreach (var cs in copySets) {
				addSubmenu(cs);
			}

			//Update the list of possible context menu entries
			buildSubmenuList(copySets);

			return false;
		}

		/// <summary>
		/// Main functionality method, loads the given copyset by string id and then
		/// copies the given file to all of the copy sets apart the the origin
		/// </summary>
		/// <param name="sourceFile">File to copy to other sets</param>
		/// <param name="copySetName">Name of the copyset the defines where to copy the file</param>
		private void doSomeCopying(string copySetName, string sourceFile) {
			
			if (CopySets.ContainsKey(copySetName)) {
				CopySet cs = CopySets[copySetName];
				doSomeCopying(sourceFile, cs);
			} else {
				MessageBox.Show("Could not find the CopySet: '" + copySetName + "'",
								"Invalid CopySet",
								MessageBoxButtons.OK,
								MessageBoxIcon.Error);
			}
		}


		/// <summary>
		/// Main functionality method, loads the given copyset by string id and then
		/// copies the given file to all of the copy sets apart the the origin
		/// </summary>
		/// <param name="sourceFile">File to copy to other sets</param>
		/// <param name="copySetName">Name of the copyset the defines where to copy the file</param>
		private void doSomeCopying(string sourceFile, CopySet set) {
			set.CopyFile(sourceFile);
		}


		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() {
			string[] args = Environment.GetCommandLineArgs();

			MeToo prog = new MeToo();

			if (args.Length > 1) {

				//Configure menu item selected start the gui
				if (args[1].ToLower().Equals("configure")) {
					prog.startGui();
				}
				else if(args.Length > 2) {
					//Default case, copy to the given copyset
					prog.doSomeCopying(args[1], args[2]);
				} else {
					//How did this happen, show usage? or just crash?
					MessageBox.Show("There was " + (args.Length - 1) + " arguements, there should be 0 or 2", 
									"Incorrect Arguement Count", 
									MessageBoxButtons.OK, 
									MessageBoxIcon.Error);
				}
			} else {
				prog.addContextAddition(args[0]);
			}
		}
	}
}
