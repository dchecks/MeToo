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
		
		public static readonly string BinName = "metoo.exe";
		private const string SettingsFileLocation = "settings.xml";

		private RegistryBuilder regBuilder;

		private Dictionary<String, CopySet> CopySets;

		public MeToo() {
			//LoadConfig
			XmlDocument xDoc = loadSettings();
			CopySets = buildCopySets(xDoc);

			regBuilder = new RegistryBuilder();
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

			XmlNodeList sets = xSettings.GetElementsByTagName("CopySet");
			
			foreach(XmlNode node in sets) {
				string setName = node.SelectSingleNode("Name").InnerText;

				List<String> dirs = new List<String>();
				foreach (XmlNode dir in node.SelectNodes("dirs/string")) {
					dirs.Add(dir.InnerText);
				}

				CopySet cs = new CopySet(setName, dirs);
				cSets[cs.NameSpaceless] = cs;
			}

			return cSets;
		}

		/// <summary>
		/// Displays the configuration form
		/// </summary>
		private void startGui() {
			startGui(false);
		}

		private void startGui(bool debug) {
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			List<CopySet> inputList = new List<CopySet>(CopySets.Values);
			Application.Run(new MeTooView(inputList, debug, this));
		}

		/// <summary>
		/// Copys the source binary to windir and inserts the default reg keys if necessary
		/// 
		/// Needs to be run as admin as it does some nasty shit
		/// </summary>
		/// <param name="programLocation">source bin to copy</param>
		//[PrincipalPermission(SecurityAction.Demand, Role = @"BUILTIN\Administrators")]
		private void addContextAddition(string programLocation) {

			try	{
				//Copy itself to Windows Dir so the program can be called from the context menu
				string winDir = Environment.GetEnvironmentVariable("WINDIR");

				if (!programLocation.StartsWith(winDir)) {
					File.Copy(programLocation, winDir + "\\" + BinName, true);
				} else {
					//If were already being run from the windir there's no need to make another copy
					//Currently do nothing
				}

				//Setup the reg keys for the main, submenu and config
				regBuilder.buildInitialMenu();

				//Show confirm message
				MessageBox.Show("Context addition created!\nTo get started, right click any file and then go to Me Too!->Configure", "MeToo Installation Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
			catch(Exception ex)	{
				logError("Exception occured whilst attempting to build context menu by writing registry keys", ex);

				MessageBox.Show(@"A failure occured setting up the registry keys for the context menu.\n" + 
									"Make sure you are running as root.\n" + 
									"Exception written to temp directory",
								"Critical Failure", 
								MessageBoxButtons.OK,
								MessageBoxIcon.Error);
			}
		}

		/// <summary>
		/// Saves and builds the current config
		/// </summary>
		public bool SaveSettings(List<CopySet> copySets) {
			
			//Remove any CopySets that don't have any dirs
			copySets.RemoveAll(f => f.dirs.Count == 0);

			//Write out the config
			using (IsolatedStorageFile settings = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null)) { 
				using(IsolatedStorageFileStream settingsFile = settings.OpenFile(SettingsFileLocation, FileMode.Truncate, FileAccess.Write)){
					//Serialize each of the sets
					XmlDocument xDoc = new XmlDocument();
					XmlSerializer xSez = new XmlSerializer(typeof(CopySet[]), new XmlRootAttribute("COPYSETS"));
					
					//Dont write out with any crappy namespaces
					XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
					ns.Add("", "");

					xSez.Serialize(settingsFile, copySets.ToArray(), ns);
				}
			}
			
			//Delete all existing context submenus
			regBuilder.deleteAllSubmenus();

			//Update/Replace/Create context menu entries
			foreach (var cs in copySets) {
				try {
					regBuilder.buildSubmenu(cs.Name, cs.NameSpaceless);
				}
				catch (Exception ex) {
					logError("Failed to add submenu: " + cs.Name, ex);

					MessageBox.Show("Failed to build registry key for new copyset " + cs.Name,
									"Operation Failed",
									MessageBoxButtons.OK,
									MessageBoxIcon.Exclamation);
				}
			}

			//Update the list of possible context menu entries
			//Get all of the spaceless names of the submenus into a list
			IEnumerable<string> names = copySets.Select(f => f.NameSpaceless);
			regBuilder.buildSubMenuLink(names);

			return true;
		}

		/// <summary>
		/// Main functionality method, loads the given copyset by string id and then
		/// copies the given file to all of the copy sets apart the the origin
		/// </summary>
		/// <param name="sourceFile">File to copy to other sets</param>
		/// <param name="copySetName">Name of the copyset the defines where to copy the file</param>
		private void doSomeCopying(string copySetName, string sourceFile) {
			
			//As arguements cant have spaces, this is done to the CS names
			copySetName = copySetName.Replace(" ", String.Empty);

			if (CopySets.ContainsKey(copySetName)) {
				CopySet cs = CopySets[copySetName];
				doSomeCopying(cs, sourceFile);
			} else {

				StringBuilder sb = new StringBuilder();
				foreach (CopySet item in CopySets.Values) {
					sb.Append(item.NameSpaceless);
					sb.Append(", ");
				}

				MessageBox.Show("Could not find the CopySet: '" + copySetName + "'\n Found: " + sb.ToString(),
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
		private void doSomeCopying(CopySet set, string sourceFile) {

			try {
				set.CopyFile(sourceFile);
			}
			catch(IOException ioe) {
				logError(String.Format("Critical failure to copy file {0} to copy set {1}", sourceFile, set.Name), ioe);
				MessageBox.Show("IO Exception occured, check the logs but possibly a permission issue on dest / source",
									"Failed to Copy", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
			catch(CopySet.CopySetException cse) {
				logError(String.Format("Failure to copy file {0} to copy set {1}", sourceFile, set.Name), cse);
				MessageBox.Show(cse.Message, "Failed to Copy", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
		}

		/// <summary>
		/// Logs the given exception and message to the temp directory error log with some nice formatting and stuff
		/// </summary>
		/// <param name="message">Freetext message to be displayed</param>
		/// <param name="ex"></param>
		private void logError(String message, Exception ex) {
			
			string tempLocation = Environment.GetEnvironmentVariable("TEMP");
			string failureLogName = tempLocation + "\\MeTooError.log";

			using (StreamWriter errorLog = File.AppendText(failureLogName)) {
				errorLog.WriteLine("Date: " + DateTime.Now);
				errorLog.WriteLine(message);
				errorLog.WriteLine(ex.Message);
				errorLog.WriteLine(ex.StackTrace.ToString());
				errorLog.Flush();
				errorLog.Close();
			}
		}

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() {
			string[] args = Environment.GetCommandLineArgs();

			MeToo prog = new MeToo();

			if (args.Length == 1) {
				prog.addContextAddition(args[0]);
			}
			else if (args[1].ToLower().Equals("configure")) {
				//Configure menu item selected start the gui
				if (args.Length == 2) prog.startGui();
				else if (args.Length == 3 && args[2].ToLower().Equals("debug")) {
					prog.startGui(true);
				}
				else showIncorrectArguements(args.Length);
			}
			else if (args[1].ToLower().Equals("configure_admin")) {
				//Process has been restarted as admin by itself
				prog.startGui();
			}
			else if(args.Length == 3) {
				//Default case, copy to the given copyset
				prog.doSomeCopying(args[1], args[2]);
			} else {
				//How did this happen, show usage
				showIncorrectArguements(args.Length);
			}
		}

		private static void showIncorrectArguements(int argCount) {
			//File name is always passed as an arguement, so -1 to get user passed args
			if (argCount == 1 || argCount == 3) {
				MessageBox.Show("There were " + (argCount - 1) + " arguements, there should be 0 for installation, 1 for setup or 2 for copying",
							"Incorrect Arguement Count",
							MessageBoxButtons.OK,
							MessageBoxIcon.Error);
			}
			else {
				MessageBox.Show("The correct number of arguements were supplied but not understood");
			}
		}
	}
}

/*
 * 
					//ProcessStartInfo proc = new ProcessStartInfo();
					//proc.UseShellExecute = true;
					//proc.WorkingDirectory = Environment.CurrentDirectory;
					//proc.FileName = Application.ExecutablePath;
					//proc.Verb = "runas";
					//proc.Arguments = "configure_admin";

					//try {
					//	Process.Start(proc);
					//}

					//catch {
					//	// The user refused the elevation. Do nothing and return directly ...
					//	return;
					//}

					//Application.Exit();
 */
