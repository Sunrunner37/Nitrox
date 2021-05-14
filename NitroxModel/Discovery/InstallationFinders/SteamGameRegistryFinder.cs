using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Win32;

namespace NitroxModel.Discovery.InstallationFinders
{
    public class SteamGameRegistryFinder : IFindGameInstallation
    {
        public const string SUBNAUTICA_GAME_NAME = "Subnautica";
        public const int SUBNAUTICA_APP_ID = 264710;

        private static readonly Regex steamJsonRegex = new("\"(.*)\"\t*\"(.*)\"", RegexOptions.Compiled);

        /// <summary>
        ///     Finds game install directory by iterating through all the steam game libraries configured and finding the appid
        ///     that matches.
        /// </summary>
        /// <param name="steamAppId"></param>
        /// <returns></returns>
        /// <exception cref="Exception">If steam is not installed or game could not be found.</exception>
        public static string FindGame(uint steamAppId)
        {
            string steamPath = (string) ReadRegistrySafe("Software\\Valve\\Steam", "SteamPath");
            if (string.IsNullOrEmpty(steamPath))
            {
                try
                {
                    steamPath = (string) ReadRegistrySafe(@"SOFTWARE\Valve\Steam",
                                                          "InstallPath",
                                                          RegistryHive.LocalMachine);
                }
                finally
                {
                    if (string.IsNullOrEmpty(steamPath))
                    {
                        throw new Exception("Steam could not be found. Check if it is installed.");
                    }
                }
            }

            string appsPath = Path.Combine(steamPath, "steamapps");

            // Test main steamapps.
            string game = GameDataFromAppManifest(Path.Combine(appsPath, $"appmanifest_{steamAppId}.acf"));
            if (game == null)
            {
                // Test steamapps on other drives (as defined by Steam).
                game = SearchAllInstallations(Path.Combine(appsPath, "libraryfolders.vdf"), steamAppId);
                if (game == null)
                {
                    throw new Exception($"Steam game with id {steamAppId} is not installed.");
                }
            }

            return game;
        }

        public string FindGame(IList<string> errors = null)
        {
            try
            {
                return FindGame(SUBNAUTICA_APP_ID);
            }
            catch (Exception ex)
            {
                errors?.Add(ex.Message);
                return null;
            }
        }

        private static string SearchAllInstallations(
            string libraryfoldersFile, uint appId)
        {
            if (!File.Exists(libraryfoldersFile)) return null;
            // Turn contents of file into dictionary lookup.
            Dictionary<string, string> steamLibraryData = JsonAsDictionary(File.ReadAllText(libraryfoldersFile));

            int steamLibraryIndex = 0;
            while (true)
            {
                steamLibraryIndex++;
                if (!steamLibraryData.TryGetValue(steamLibraryIndex.ToString(), out string steamLibraryPath)) return null;
                string manifestFile = Path.Combine(steamLibraryPath, $"steamapps/appmanifest_{appId}.acf");
                if (!File.Exists(manifestFile)) continue;

                return GameDataFromAppManifest(manifestFile);
            }
        }

        private static string GameDataFromAppManifest(string manifestFile)
        {
            Dictionary<string, string> gameData;
            try
            {
                gameData = JsonAsDictionary(File.ReadAllText(manifestFile));
            }
            catch (FileNotFoundException)
            {
                return null;
            }

            // Validate steam game data exists.
            if (!gameData.TryGetValue("installdir", out string gameInstallFolderName)) return null;
            string gameDir =
                Path.GetFullPath(Path.Combine(Path.GetDirectoryName(manifestFile), "common", gameInstallFolderName));
            if (!Directory.Exists(gameDir)) return null;

            return gameDir;
        }

        private static Dictionary<string, string> JsonAsDictionary(string json)
        {
            return steamJsonRegex.Matches(json)
                                 .Cast<Match>()
                                 .ToDictionary(m => m.Groups[1].Value.ToLowerInvariant(), m => m.Groups[2].Value);
        }

        private static object ReadRegistrySafe(string path, string key, RegistryHive hive = RegistryHive.CurrentUser)
        {
            using RegistryKey subkey = RegistryKey.OpenBaseKey(hive, RegistryView.Registry32).OpenSubKey(path);
            return subkey?.GetValue(key);
        }
    }
}
