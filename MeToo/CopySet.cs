using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MeToo {
	public class CopySet {
		
		public class CopySetException : Exception {
			public CopySetException(string errorMessage) : base(errorMessage) {}
		}

		public string Name {get; set;}
		public string NameSpaceless {
			get{
				return Name.Replace(" ", String.Empty);
			}
		}

		public List<string> dirs { get; set;}

		public CopySet() { }
		public CopySet(string setName) {
			this.Name = setName;
			this.dirs = new List<string>();
		}

		public CopySet(string setName, List<string> dirs) {
			this.Name = setName;
			this.dirs = dirs;
		}


		/// <summary>
		/// Copies the given source file to all of the directories in this CopySet
		/// provided the sourceFile exists in one of the CopySets dirs
		/// Backs up any files that are replaced to the %TEMP% dir
		/// Will output a log of all actions taken to %TEMP% dir
		/// </summary>
		/// <param name="sourceFile"></param>
		public void CopyFile(string sourceFile) {
			int randSuffix = new Random().Next(10000);
			int dirIndex = 0;

			string tempLocation = Environment.GetEnvironmentVariable("TEMP");
			string suffix = String.Empty;
			string sourceDir = String.Empty;
			string fileName = sourceFile.Substring(sourceFile.LastIndexOf('\\') + 1);
			string logLocation = tempLocation + "\\MeToo_CopyLog-" + DateTime.Now.ToString("dd-MM-yy") + ".log";
			
			StringBuilder sb = new StringBuilder();

			try {
				sb.AppendLine("Copy Operation");
				sb.AppendLine(DateTime.Now.ToString("dd/MM/yyyy hh:mm.ss"));
				sb.Append("Source File: ");
				sb.AppendLine(sourceFile);

				//Locate the file suffix
				foreach (string dir in dirs) {
					if (sourceFile.StartsWith(dir)) {
						sourceDir = dir;

						//Get the relative file path to the dir
						suffix = sourceFile.Substring(dir.Length);
					}
				}

				sb.AppendLine("Source Dir Calculated: " + sourceDir);
				sb.AppendLine("Suffix Calculated: " + suffix);
				sb.AppendLine("Copying to...");

				if (sourceDir != String.Empty) {
					//Do the copying to the rest of the dirs
					foreach (string dir in dirs) {
						if (dir != sourceDir) {

							string dest = dir + suffix;
							//Log the file name
							sb.Append(dest);

							if (File.Exists(dest)) {
								//Copy the existing file to the temp directory as a backup	
								string backupName = String.Format("{0}\\MeToo_{1}-{2}-{3}", tempLocation, randSuffix, dirIndex++, fileName);
								sb.Append(" Backup: ");
								sb.AppendLine(backupName);
								
								File.Copy(dest, backupName);
								sb.AppendLine("Backup successful");
							}
							else sb.AppendLine("\nCreating new file");
							
							//Do the actual desired copy operation
							File.Copy(sourceFile, dest, true);
							sb.AppendLine("Copy operation successful");

							sb.AppendLine("---------------------------\n");
						}
					}
				}
				else {
					sb.AppendLine("Error: Could not find the source dir in this copyset");
					//Couldnt find the dir in this CopySet :(
					throw new CopySetException(String.Format("The source file must be in a directory that is in the copyset '{0}'\n" +
																"Add this DIR to the copyset or create a new copyset that includes it", Name));
				}
			}
			catch (Exception ex) {
				//Catch and throw so the logging still gets done
				throw ex;
			}
			finally {
				//Write out log fie
				using (StreamWriter copyLog = File.AppendText(logLocation)) {
					copyLog.Write(sb.ToString());
					copyLog.Flush();
					copyLog.Close();
				}
			}
		}
	}
}
