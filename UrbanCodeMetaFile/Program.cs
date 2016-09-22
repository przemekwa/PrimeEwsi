using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using NDesk.Options;
using UrbanCodeMetaFile.Component;
using UrbanCodeMetaFileCreator;
using UrbanCodeMetaFileCreator.Dto;

namespace UrbanCodeMetaFile
{
    class Program
    {
        static void Main(string[] args)
        {

            var showHelp = false;
            string component = null;
            string zipCreateFile = null;

            var register = false;
            var unregister = false;
            
            try
            {
                var parseElements = new OptionSet
                {
                    {"r|register", "Instalacja aplikacji", v => register = v != null},
                    {"u|unregister", "Deinstalacja aplikacji", v => unregister = v != null},
                    {"c=|component=", "Definicja komonentu", v => component = v},
                    {"z=", "Tworzenie pliku zip z manifestem z pliku kontrolnego", v => zipCreateFile = v},
                    {"h|help", "Wyświeltanie pomocy", v => showHelp = v != null},
                };

                parseElements.Parse(args);

                if (register)
                {
                    Register();
                    return;
                }

                if (unregister)
                {
                    UnRegister();
                    return;
                }

                if (showHelp)
                {
                    ShowHelp(parseElements);
                    return;
                }

                if (string.IsNullOrEmpty(component))
                {
                    throw new ArgumentException("Parametr Component jest wymagany");
                }

                if (!string.IsNullOrEmpty(zipCreateFile))
                {
                    CreateZipFile(new SqlComponent(zipCreateFile, component), Path.GetDirectoryName(zipCreateFile));
                }
                

                Console.ReadKey();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Wystapił błąd {e.Message} ");
                Console.Read();
            }
        }

        private static void ShowHelp(OptionSet p)
        {
            p.WriteOptionDescriptions(Console.Out);
            Console.ReadKey();
        }

        private static void CreateZipFile(IComponent component, string path)
        {
            Console.WriteLine($"Rozpoczecie tworzenia paczki {component.Name}");

            Console.Write("Stworzenie Deployment Component...");

            var dc = component.GetDeploymentComponent();
              
            var dp = new DeploymentPackage
            {
                DeploymentComponent = new[]
                {
                    dc
                }
            };

            Console.WriteLine("OK");

            Console.Write("Stworzenie manifestu...");

            var manifestFile = Helper.SaveXml(dp);

            Console.WriteLine("OK");

            var allFileZip = new List<FileInfo>();

            allFileZip.AddRange(component.Files);
            allFileZip.Add(manifestFile);

            var pathToWorkingFolder = Path.Combine(path, $"{component.Name}.zip");

            Console.Write($"Stworzenie archiwum w {pathToWorkingFolder}...");

            Helper.CreateZipFile(allFileZip, pathToWorkingFolder);

            Console.WriteLine("OK");

            Console.WriteLine("Zakończenie generowania paczki");
        }

        private static void Register()
        {
            FileShellExtension.RegisterFileType("Urban", "Urban");

            string menuCommandZip = $"\"{System.Reflection.Assembly.GetEntryAssembly().Location}\" -z \"%L\" -c Addon_live";

            FileShellExtension.RegisterFileTypeCommand("Urban", "UrbanCodeMetaFile", "Create Addon_live component",
                menuCommandZip);

            Console.WriteLine("UrbanCodeMetaFile został zainstalowany.");

            Console.ReadKey();
        }

        private static void UnRegister()
        {
            FileShellExtension.UnregisterFileType("Urban");

            FileShellExtension.UnregisterFileTypeCommand("Urban", "UrbanCodeMetaFile");

            Console.WriteLine("UrbanCodeMetaFile została pomyślnie odinstalowana");

            Console.ReadKey();
        }
    }
}
