using System;
using System.IO;
using System.Linq;
using Google.Protobuf;
using quocnt.uset.data;
using UnityEngine;

namespace Social.Database.Example
{
    public class SaveManagerExample : MonoBehaviour
    {
        public static int DATA_VERSION = 1;

        public static string GetMainID()
        {
            if (_Instance != null)
            {
                if (_Instance._mainData != null)
                {
                    return _Instance._mainData.Meta.GameID;
                }
            }

            return "";
        }

        #region Singleton

        private static object _lock = new object();
        private static SaveManagerExample _Instance;

        public static SaveManagerExample Instance => _Instance;

        void Awake()
        {
            _Instance = this;
        }

        void Start()
        {
            dataPath = Application.persistentDataPath + "/";
            isDataDirty = false;
            _mainData = null;
            allowSaveHashKey = false;

            LoadData();
        }

        // Discard Singleton Reference
        void OnDestroy()
        {
            _Instance = null;
        }

        #endregion

        public string dataPath;
        private UserData _mainData;

        public UserData MainData
        {
            get => _mainData;
        }

        private bool isDataDirty;

        #region FUNCTION FOR SAVE

        private string FileBinary = "data.dat";
        private string FileJson = "data.json";

        public static float AUTO_SAVE_TIME = 3f * 60; //3m save
        private float timer = 0;

        public void SetDataDirty(bool forceSave = false)
        {
            isDataDirty = true;
            if (forceSave)
            {
                timer = AUTO_SAVE_TIME;
            }
        }

        public bool allowSaveHashKey = false;

        void SaveData()
        {
            if (_mainData == null)
            {
                return;
            }

            try
            {
                Debug.Log("Save Data Call".Red());

                _mainData.Meta.Version = DATA_VERSION;
                _mainData.Meta.LastSyncTime = DateTime.Now.Ticks;
                SetLastDevice(SystemInfo.deviceUniqueIdentifier);
                SetHashKey(allowSaveHashKey);

                File.WriteAllBytes(dataPath + FileBinary, _mainData.ToByteArray());
#if UNITY_EDITOR
                File.WriteAllText(dataPath + FileJson, _mainData.ToString());
#endif

                isDataDirty = false;
            }
            catch (Exception ex)
            {
                Debug.LogError("<>==### SaveData ==>:" + dataPath + " Error: " + ex);
            }
        }

        public void LoadData()
        {
            UserData userData = null;

            if (File.Exists(dataPath + FileBinary))
            {
                var bytes = File.ReadAllBytes(dataPath + FileBinary);
                try
                {
                    userData = UserData.Parser.ParseFrom(bytes);
                }
                catch (Exception ex)
                {
                    Debug.LogError("<>==### UserData ==>:" + dataPath + " Error: " + ex);
                }
            }
            else
            {
                userData = DefaultData();
                isDataDirty = true;
            }

            if (userData != null)
            {
                VerifyData(userData);
            }

            if (IsDataDirty())
            {
                SaveData();
            }
        }

        public bool SyncDataVersion(UserData userData)
        {
            if (userData.Meta.Version >= DATA_VERSION) return false;
            for (var version = userData.Meta.Version; version <= DATA_VERSION; version++)
            {
                SynNewVersionInfo(userData, version);
            }

            userData.Meta.Version = DATA_VERSION;
            return true;
        }

        private void SynNewVersionInfo(UserData userData, int version)
        {
        }

        public bool SynDataFromDesign(UserData userData)
        {
            //Patching Data by DATA_VERSION
            var forceSave = SyncDataVersion(userData);

            return forceSave;
        }

        private void Update()
        {
            timer += Time.deltaTime;
            if (timer > AUTO_SAVE_TIME)
            {
                if (IsDataDirty())
                {
                    SaveData();
                }

                timer = 0;
            }
        }

        private bool isQuittingCalled;

        public void OnApplicationQuit()
        {
            if (isQuittingCalled) return;

            SaveData();
            isQuittingCalled = true;
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus)
            {
                SaveData();
            }
        }

        private bool IsDataDirty()
        {
            return isDataDirty && _mainData != null;
        }

        public void RestoreData(UserData remote)
        {
            SetDataDirty();
            _mainData = remote;
            SynDataFromDesign(_mainData);
            //todo: check refresh game
        }

        void OnRemoteDataChosen(UserData remote, string gameIDRemote = "")
        {
            //Force save local
            EventSystemServiceStatic.DispatchAll("SHOW_VERIFY_POPUP", "Verify Data", "You have old data on Cloud! Do you want to restore form cloud or backup local?", remote);
            //todo: check refresh game
        }

        void OnLocalDataChosen()
        {
            if (SynDataFromDesign(_mainData))
            {
                SetDataDirty();
            }
        }

        public bool VerifyData(UserData remote)
        {
            if (_mainData == null && remote != null)
            {
                _mainData = remote;
                SynDataFromDesign(_mainData);
                //OnRemoteDataChosen(remote,gameIDRemote);
                return true;
            }

            if (_mainData != null && remote == null)
            {
                // OnLocalDataChosen();
                return true;
            }

            //Verify Cloud Data
            var state = CompareDataMeta(remote);
            if (state == EnumVerifyData.None)
            {
                //TODO: Show Conflicts Popup
                //force pause game
            }
            else if (state == EnumVerifyData.ServerNew)
            {
                OnRemoteDataChosen(remote);
                return true;
            }
            else if (state == EnumVerifyData.LocalNew)
            {
                OnLocalDataChosen();
                return true;
            }

            return false;
        }

        private EnumVerifyData CompareDataMeta(UserData local, UserData remote)
        {
            //check gameID config
            if (!local.Meta.GameID.Equals(remote.Meta.GameID)) return EnumVerifyData.GameIdConflict;

            //check version
            if (local.Meta.Version > remote.Meta.Version) return EnumVerifyData.LocalNew;

            if (local.Meta.Version < remote.Meta.Version) return EnumVerifyData.ServerNew;

            //check device
            if (!local.Meta.DeviceID[0].Equals(remote.Meta.DeviceID[0]))
            {
                return EnumVerifyData.DeviceIdConflict;
            }

            //check last time
            if (local.Meta.LastSyncTime > remote.Meta.LastSyncTime) return EnumVerifyData.LocalNew;

            if (local.Meta.LastSyncTime < remote.Meta.LastSyncTime) return EnumVerifyData.ServerNew;
            ;

            return EnumVerifyData.None;
        }

        public EnumVerifyData CompareDataMeta(UserData remote)
        {
            return CompareDataMeta(_mainData, remote);
        }

        #endregion

        public string SetHashKey(bool allow)
        {
            if (!allow) return "";

            var hashKey = new SaveHashKey
            {
                DeviceId = SystemInfo.deviceUniqueIdentifier, Version = DATA_VERSION, GameId = GetMainID(), LastTime = DateTime.Now.Ticks,
            };
            return JsonFormatter.Default.Format(hashKey);
        }

        public void SetLastDevice(string ID)
        {
            var ListDevice = _mainData.Meta.DeviceID.ToArray();
            var len = ListDevice.Length;
            if (!ListDevice.Contains(ID))
            {
                _mainData.Meta.DeviceID.Insert(0, ID);
            }
            else
            {
                _mainData.Meta.DeviceID.Clear();
                for (int i = 0; i < len; i++)
                {
                    if (ListDevice[i].Equals(ID))
                    {
                        _mainData.Meta.DeviceID.Insert(0, ID);
                    }
                    else
                    {
                        _mainData.Meta.DeviceID.Add(ListDevice[i]);
                    }
                }
            }
        }

        public string GetHashKey()
        {
            return _mainData.Meta.HashKey;
        }

        public SaveHashKey GetSaveHashKey()
        {
            return SaveHashKey.Parser.ParseJson(_mainData.Meta.HashKey);
        }

        public UserData DefaultData()
        {
            var data = new UserData();
            //game meta
            data.Meta = new GameMeta
            {
                Version = DATA_VERSION,
                DeviceID = {SystemInfo.deviceUniqueIdentifier},
                HashKey = "",
                SocialMaps = { },
                GameID = Guid.NewGuid().ToString(),
                UserName = SystemInfo.deviceName,
                LastSyncTime = DateTime.Now.Ticks,
            };

            //Setting
            data.Setting = new GameSetting {IsOnMusic = true, IsOnSound = true,};
            SetHashKey(true);

            return data;
        }

        public string GetFirebaseAuthenID(EnumProvider provider)
        {
            return "";
        }
    }
}