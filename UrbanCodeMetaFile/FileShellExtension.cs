using System;
using System.Diagnostics;
using Microsoft.Win32;

namespace UrbanCodeMetaFile
{
    internal static class FileShellExtension
	{
		public static void RegisterFileTypeCommand(string fileType, string shellKeyName, string menuText, string menuCommand)
		{
			string regPath = $@"{fileType}\shell\{shellKeyName}";

			using (var key = Registry.ClassesRoot.CreateSubKey(regPath))
			{
				key.SetValue(null, menuText);
			}
			
			using (var key = Registry.ClassesRoot.CreateSubKey($@"{regPath}\command"))
			{				
				key.SetValue(null, menuCommand);
			}
		}

        public static void RegisterFileType(string fileType, string keyName)
        {
            string regPath = $@".{fileType}";

            using (var key = Registry.ClassesRoot.CreateSubKey(regPath))
            {
                key.SetValue(null, keyName);
            }
        }

        public static void UnregisterFileType(string fileType)
        {
            string regPath = $@".{fileType}";

            Registry.ClassesRoot.DeleteSubKeyTree(regPath);
        }


        public static void UnregisterFileTypeCommand(string fileType, string shellKeyName)
		{
			string regPath = $@"{fileType}\shell\{shellKeyName}";

			Registry.ClassesRoot.DeleteSubKeyTree(regPath);
		}
	}

}
