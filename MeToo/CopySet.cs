using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeToo {
	public class CopySet {
		
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
		/// </summary>
		/// <param name="sourceFile"></param>
		public void CopyFile(string sourceFile) {
			
			string suffix = String.Empty;
			string sourceDir = String.Empty;
			
			//Locate the file suffix
			foreach(string dir in dirs) {
				if(sourceFile.StartsWith(dir)) {
					sourceDir = dir;

					//Get the relative file path to the dir
					suffix = sourceFile.Substring(dir.Length);
				}
			}

			if (sourceDir != String.Empty) {
				//Do the copying to the rest of the dirs
				foreach(string dir in dirs) {
					if(dir != sourceDir) {
						//Copy some stuff to some other places
						//TODO REMOVE 2 safety
						File.Copy(sourceFile, dir + suffix + "2", true);
					}	
				}
			} else {
				//Couldnt find the dir in this CopySet :(
			}
		}
	}
}
