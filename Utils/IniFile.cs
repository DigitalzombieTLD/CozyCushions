using Il2Cpp;
using Il2CppSystem.IO;
using MelonLoader;
using CozyCushions.Utils.Filesystem;
using CozyCushions.Utils.Ini.Model;
using CozyCushions.Utils.Ini.Parser;

using System.Collections;
using System.IO;
using UnityEngine;


namespace CozyCushions.Utils.Ini
{
    public class IniFile
    {
        private string _rootPath = Application.dataPath + "/../Mods/";
        private string _fileName;
        private FileIniDataParser _thisIniParser;
        private IniData _thisIniData;
        private bool _isNew = true;


        public IniFile(string fileName)
        {
            _fileName = fileName;
            CreateIniParser();
            CreateOrGetIniData();
        }

        private void CreateIniParser()
        {
            _thisIniParser  = new FileIniDataParser();

            _thisIniParser.Parser.Configuration.AllowCreateSectionsOnFly = true;
            //thisIniParser.Parser.Configuration.AssigmentSpacer = "&&";
            _thisIniParser.Parser.Configuration.SkipInvalidLines = true;
            _thisIniParser.Parser.Configuration.OverrideDuplicateKeys = false;
            _thisIniParser.Parser.Configuration.AllowDuplicateKeys = false;
        }

        private IEnumerator SaveFileRoutine()
        {
            _thisIniParser.WriteFile(_rootPath + _fileName, _thisIniData);
            _isNew = false;
            yield return null;
        }

        public void SaveFileAsync()
        {
            MelonCoroutines.Start(SaveFileRoutine());
        }

        public void SaveFile()
        {
            _thisIniParser.WriteFile(_rootPath + _fileName, _thisIniData);
            _isNew = false;
        }

        private bool FileExist(string fileName)
        {
            if (Files.FileExist(fileName))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void CreateOrGetIniData()
        {
            if (!Files.FileExist(_fileName))
            {
                _thisIniData = new IniData();
                _thisIniParser.WriteFile(_rootPath + _fileName, _thisIniData);
                _isNew = true;
            }
            else
            {
                _thisIniData = _thisIniParser.ReadFile(_rootPath + _fileName);
                _isNew = false;
            }
        }

        public void SetString(string name, string section, string value)
        {
            MaybeAddSectionExist(section);

            if (!_thisIniData[section].ContainsKey(name))
            {
                _thisIniData[section].AddKey(name, value);
            }
            else
            {
                _thisIniData[section][name] = value;
            }
        }

        public string GetString(string name, string section)
        {
            string tempResult;
            
            if(_thisIniData.TryGetKey(section + "|" + name, out tempResult))
            {
                MelonLogger.Msg("Could not retrieve key from ini file: " + section + "|" + name);
            }
           
            return tempResult;            
        }

        public void SetFloat(string name, string section, float value)
        {
            SetString(name, section, value.ToString());
        }

        public float GetFloat(string name, string section)
        {
            return float.Parse(GetString(name, section));           
        }

        public void SetBool(string name, string section, bool value)
        {
            SetString(name, section, value.ToString());
        }

        public bool GetBool(string name, string section)
        {
            return bool.Parse(GetString(name, section));
        }

        public void SetInt(string name, string section, int value)
        {
            SetFloat(name, section, value);
        }

        public int GetInt(string name, string section)
        {
            return int.Parse(GetString(name, section));
        }

        public void SetKeyCode(string name, string section, KeyCode value)
        {
            SetString(name, section, value.ToString());
        }

        public KeyCode GetKeyCode(string name, string section)
        {
            return (KeyCode)System.Enum.Parse(typeof(KeyCode), GetString(name, section));
        }

        public void SetColor32(string name, string section, Color32 color)
        {
            SetInt(name + "_Red", section, color.r);
            SetInt(name + "_Green", section, color.g);
            SetInt(name + "_Blue", section, color.b);
            SetInt(name + "_Alpha", section, color.a);
        }

        public Color32 GetColor32(string name, string section)
        {
            return new Color(GetInt(name + "_Red", section), GetInt(name + "_Green", section), GetInt(name + "_Blue", section), GetInt(name + "_Alpha", section));
        }

        public void SetColor(string name, string section, Color color)
        {
            SetFloat(name + "_Red", section, color.r);
            SetFloat(name + "_Green", section, color.g);
            SetFloat(name + "_Blue", section, color.b);
            SetFloat(name + "_Alpha", section, color.a);
        }
        public Color GetColor(string name, string section)
        {
            return new Color(GetFloat(name + "_Red", section), GetFloat(name + "_Green", section), GetFloat(name + "_Blue", section), GetFloat(name + "_Alpha", section));
        }

        public void SetVector3(string name, string section, Vector3 vector3)
        {
            SetFloat(name + "_VectorX", section, vector3.x);
            SetFloat(name + "_VectorY", section, vector3.y);
            SetFloat(name + "_VectorZ", section, vector3.z);
        }

        public Vector3 GetVector3(string name, string section)
        {
            return new Vector3(GetFloat(name + "_VectorX", section), GetFloat(name + "_VectorY", section), GetFloat(name + "_VectorZ", section));
        }

        public void SetQuaternion(string name, string section, Quaternion quaternion)
        {
            SetFloat(name + "_QuaternionX", section, quaternion.x);
            SetFloat(name + "_QuaternionY", section, quaternion.y);
            SetFloat(name + "_QuaternionZ", section, quaternion.z);
            SetFloat(name + "_QuaternionW", section, quaternion.w);
        }

        public Quaternion GetQuaternion(string name, string section)
        {
            return new Quaternion(GetFloat(name + "_QuaternionX", section), GetFloat(name + "_QuaternionY", section), GetFloat(name + "_QuaternionZ", section), GetFloat(name + "_QuaternionW", section));
        }

        private void MaybeAddSectionExist(string section)
        {
            if (_thisIniData.Sections.ContainsSection(section))
            {
                _thisIniData.Sections.AddSection(section);
            }
        }

        public bool isNew
        {
            get
            {
                return _isNew;
            }
        }
    }
}
