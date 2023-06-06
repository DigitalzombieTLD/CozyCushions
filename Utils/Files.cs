using Il2Cpp;
using Il2CppSystem.IO;
using System.IO;
using UnityEngine;

namespace CozyCushions.Utils.Filesystem
{
    public static class Files
    {
        public static string rootPath = Application.dataPath + "/../Mods/";

        public static bool FileExist(string fileName)
        {
            if (System.IO.File.Exists(rootPath + fileName))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool DirectoryExists(string directory)
        {
            if (System.IO.Directory.Exists(rootPath + directory))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void MaybeCreateDirectory(string directory) 
        {
            System.IO.Directory.CreateDirectory(rootPath + directory);
        }
    }
}
