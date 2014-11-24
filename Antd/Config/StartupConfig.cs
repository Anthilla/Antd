using IniParser;
using IniParser.Model;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Antd
{
    /// <summary>
    /// Create a New INI file to store or load data
    /// </summary>
    public class StartupConfig
    {
        public string folder;
        public string file;
        public string path;

        public FileIniDataParser parser;
        /*
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);
        */

        /// <summary>
        /// INIFile Constructor
        /// </summary>
        /// <param name="iniFolder"></param>
        /// <param name="iniFile"></param>
        public StartupConfig()
        {
            folder = "./appconfig/";
            Directory.CreateDirectory(folder);
            file = "coreparam.ini";
            path = Path.Combine(folder, file);
            parser = new FileIniDataParser();
        }

        /// <summary>
        /// Write Data to the INI File
        /// </summary>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void WriteValue(string section, string key, string value)
        {
            if(!File.Exists(this.path))
            {
                using (FileStream fs = File.Create(this.path))
                {
                    for (byte i = 0; i < 1; i++)
                    {
                        fs.WriteByte(i);
                    }
                }
                File.WriteAllText(this.path, "");
            }
            IniData data = parser.ReadFile(this.path);
            if (!data.Sections.ContainsSection(section)) {
                data.Sections.AddSection(section);
            }
            if (!data[section].ContainsKey(key)) {
                data[section].AddKey(key);
            }
            data[section][key] = value;
            parser.WriteFile(this.path, data);
        }

        /// <summary>
        /// Read Data Value From the Ini File
        /// </summary>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public string ReadValue(string section, string key)
        {
            if (File.Exists(this.path)) 
            {
                IniData data = parser.ReadFile(this.path);
                return    (!data.Sections.ContainsSection(section)) ? null
                        : (!data.Sections[section].ContainsKey(key)) ? null
                        : data[section][key];
            } 
            else 
            {
                return null;
            }
        }

        /// <summary>
        /// Check if Config Directory Exists
        /// </summary>
        /// <returns></returns>
        public bool Check()
        {
            string pathString = Path.Combine(this.path);
            if (File.Exists(pathString))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Check if a Value in Config File Exists
        /// </summary>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool CheckValue(string section, string key)
        {
            string value = ReadValue(section, key);
            if (value != null && value != "")
            {
                return true;
            }
            return false;
        }
    }
}
