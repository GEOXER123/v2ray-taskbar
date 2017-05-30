using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;

namespace v2ray_taskbar
{
    class IniOper
    {
        private string iniName;

        public IniOper(string fileName)
        {
            if (File.Exists(fileName))
            {
                iniName = fileName;
            }
            else
            {
                iniName = "";
            }
        }

        //写数据
        public void WriteValue(string Section, string Key, string Value)
        {
            WritePrivateProfileString(Section, Key, Value, iniName);
        }

        //读数据
        public string ReadValue(string Section, string Key)
        {
            StringBuilder val = new StringBuilder(500);
            int i = GetPrivateProfileString(Section, Key, "", val, 500, iniName);
            return val.ToString();
        }

        //Windows api
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

    }   
}
