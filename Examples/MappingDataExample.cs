using System;
using System.IO;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using Firebase.Unity.Editor;
using UnityEngine;

namespace Social.Database.Example
{
    [Serializable]
    public class SocialMapIDEX : SocialMapID
    {
        public string GameId;

        public SocialMapIDEX(string id)
        {
            GameId = id;
        }

        public SocialMapIDEX()
        {
        }

        public string ToDataString()
        {
            return GameId;
        }
    }

    [Serializable]
    public class GameMapIDEX : GameMapID
    {
        public string authFacebook;
        public string authGooglePlay;
        public string authGameCenter;
        public string authEmail;
        public string authGuest;

        public string ToDataString()
        {
            var txt = "";
            if (!string.IsNullOrEmpty(authFacebook))
            {
                txt += $"fb:{authFacebook} - ";
            }

            if (!string.IsNullOrEmpty(authGooglePlay))
            {
                txt += $"gp:{authGooglePlay} - ";
            }

            if (!string.IsNullOrEmpty(authGameCenter))
            {
                txt += $"gc:{authGameCenter} - ";
            }

            if (!string.IsNullOrEmpty(authEmail))
            {
                txt += $"em:{authEmail} - ";
            }

            if (!string.IsNullOrEmpty(authGuest))
            {
                txt += $"gu:{authGuest} - ";
            }

            return txt;
        }
    }

    [Serializable]
    public class HashKeyIDEX : HashKeyID
    {
        public byte[] HashKey;
    }

    public class MappingDataExample : FirebaseDBMapping
    {
        public override void Initialize(string dataUrl)
        {
            FirebaseApp.DefaultInstance.SetEditorDatabaseUrl(dataUrl);
            socialMapping = FirebaseDatabase.DefaultInstance.GetReference("SOCIAL_MAPS");
            gameMapping = FirebaseDatabase.DefaultInstance.GetReference("GAME_MAPS");
            userData = FirebaseDatabase.DefaultInstance.GetReference("USERS_DATA");
            hashKeyMapping = FirebaseDatabase.DefaultInstance.GetReference("HASH_KEYS");
        }

        public override void MappingData(EnumProvider providerCheck, string authenID, string localGameID, GameMapID gameMaps, Action<EnumMappingState, SocialMapID, GameMapID> callback)
        {
            Debug.Log($"{DefineContain.DebugMapping} Mapping:  authenID:{authenID} - gameID:{localGameID} - GameIDMap:{JsonUtility.ToJson(gameMaps, true)}");
            GetAuthenticationMap(authenID, (checkAuthenState, outSocialMap) =>
            {
                Debug.Log($"{DefineContain.DebugMapping} stateGetAuthen: {checkAuthenState}");
                if (checkAuthenState == EnumDataState.NoData)
                {
                    GetGameIdMap(localGameID, (checkGameState, gameIDs) =>
                    {
                        Debug.Log($"{DefineContain.DebugMapping} checkGameState: {checkGameState}");
                        if (checkGameState == EnumDataState.NoData)
                        {
                            Debug.Log($"{DefineContain.DebugMapping} no data create new user data");
                            // 0 - 0
                            callback?.Invoke(EnumMappingState.CreateNewData, new SocialMapIDEX(localGameID), gameMaps);
                        }
                        else if (checkGameState == EnumDataState.Exists)
                        {
                            // 0 - 1
                            var outGameId = (GameMapIDEX) gameIDs;
                            var success = true;
                            var cloudId = "";
                            switch (providerCheck)
                            {
                                case EnumProvider.Email:
                                    if (string.IsNullOrEmpty(outGameId.authEmail))
                                    {
                                        outGameId.authEmail = authenID;
                                    }
                                    else
                                    {
                                        success = false;
                                    }

                                    break;

                                case EnumProvider.Facebook:
                                    if (string.IsNullOrEmpty(outGameId.authFacebook))
                                    {
                                        outGameId.authFacebook = authenID;
                                    }
                                    else
                                    {
                                        success = false;
                                    }

                                    break;

                                case EnumProvider.GooglePlay:
                                    if (string.IsNullOrEmpty(outGameId.authGooglePlay))
                                    {
                                        outGameId.authGooglePlay = authenID;
                                    }
                                    else
                                    {
                                        success = false;
                                    }

                                    break;

                                case EnumProvider.GameCenter:
                                    if (string.IsNullOrEmpty(outGameId.authGameCenter))
                                    {
                                        outGameId.authGameCenter = authenID;
                                    }
                                    else
                                    {
                                        success = false;
                                    }

                                    break;

                                case EnumProvider.Guest:
                                    if (string.IsNullOrEmpty(outGameId.authGuest))
                                    {
                                        outGameId.authGuest = authenID;
                                    }
                                    else
                                    {
                                        success = false;
                                    }

                                    break;
                            }

                            if (success)
                            {
                                Debug.Log($"{DefineContain.DebugMapping} not AuthenID - exits game ID");
                                callback?.Invoke(EnumMappingState.NoDataAuthen_ExitsGameId, new SocialMapIDEX(localGameID), outGameId);
                            }
                            else
                            {
                                Debug.Log($"{DefineContain.DebugMapping} not AuthenID - not match game ID");
                                callback?.Invoke(EnumMappingState.NoDataAuthen_NoMatchGameId, new SocialMapIDEX(localGameID), outGameId);
                            }
                        }
                        else if (checkGameState == EnumDataState.Error)
                        {
                            Debug.Log($"{DefineContain.DebugMapping} mapping data error");
                            callback?.Invoke(EnumMappingState.Error, new SocialMapIDEX(localGameID), gameMaps);
                        }
                    });
                }
                else if (checkAuthenState == EnumDataState.Exists)
                {
                    GetGameIdMap(localGameID, (checkGameState, gameIDs) =>
                    {
                        Debug.Log($"{DefineContain.DebugMapping} checkGameState: {checkGameState}");
                        // 1 - 0
                        if (checkGameState == EnumDataState.NoData)
                        {
                            Debug.Log($"{DefineContain.DebugMapping} exists AuthenID:{authenID} - no data GameID{outSocialMap.ToDataString()}");
                            callback?.Invoke(EnumMappingState.ExitsAuthen_NoDataGameID, new SocialMapIDEX(localGameID), gameMaps);
                        }
                        else if (checkGameState == EnumDataState.Exists)
                        {
                            var outGameId = (GameMapIDEX) gameIDs;
                            Debug.Log($"{DefineContain.DebugMapping} game mapping date: {outGameId.ToDataString()}");
                            // 1 - 1
                            var success = false;
                            var cloudID = "";
                            switch (providerCheck)
                            {
                                case EnumProvider.Facebook:
                                    cloudID = outGameId.authFacebook;
                                    if (authenID.Equals(outGameId.authFacebook) || string.IsNullOrEmpty(outGameId.authFacebook))
                                    {
                                        success = true;
                                    }

                                    break;

                                case EnumProvider.GooglePlay:
                                    cloudID = outGameId.authGooglePlay;
                                    if (authenID.Equals(outGameId.authGooglePlay) || string.IsNullOrEmpty(outGameId.authGooglePlay))
                                    {
                                        success = true;
                                    }

                                    break;

                                case EnumProvider.GameCenter:
                                    cloudID = outGameId.authGameCenter;
                                    if (authenID.Equals(outGameId.authGameCenter) || string.IsNullOrEmpty(outGameId.authGameCenter))
                                    {
                                        success = true;
                                    }

                                    break;

                                case EnumProvider.Email:
                                    cloudID = outGameId.authEmail;
                                    if (authenID.Equals(outGameId.authEmail) || string.IsNullOrEmpty(outGameId.authEmail))
                                    {
                                        success = true;
                                    }

                                    break;

                                case EnumProvider.Guest:
                                    cloudID = outGameId.authGuest;
                                    if (authenID.Equals(outGameId.authGuest) || string.IsNullOrEmpty(outGameId.authGuest))
                                    {
                                        success = true;
                                    }

                                    break;
                            }

                            if (success)
                            {
                                Debug.Log($"{DefineContain.DebugMapping} mapping date success");
                                callback?.Invoke(EnumMappingState.Success, outSocialMap, gameIDs);
                            }
                            else
                            {
                                Debug.Log($"{DefineContain.DebugMapping} have 2 Authen connect 1 game id: local:{authenID} - cloud: {cloudID}");
                                callback?.Invoke(EnumMappingState.TwoAuthenOneGameID, outSocialMap, gameIDs);
                            }
                        }
                        else if (checkGameState == EnumDataState.Error)
                        {
                            Debug.Log($"{DefineContain.DebugMapping} mapping data error");
                            callback?.Invoke(EnumMappingState.Error, new SocialMapIDEX(localGameID), gameMaps);
                        }
                    });
                }
                else if (checkAuthenState == EnumDataState.Error)
                {
                    Debug.Log($"{DefineContain.DebugMapping} mapping data error");
                    callback?.Invoke(EnumMappingState.Error, new SocialMapIDEX(localGameID), gameMaps);
                }
            });
        }

        public override void GetAuthenticationMap(string authenID, Action<EnumDataState, SocialMapID> callback)
        {
            Debug.Log($"{DefineContain.DebugMapping} get authen id maps {authenID}");
            socialMapping.OrderByKey()
                .EqualTo(authenID)
                .GetValueAsync()
                .ContinueWithOnMainThread(task =>
                {
                    if (task.IsFaulted || task.IsCanceled)
                    {
                        Debug.Log($"{DefineContain.DebugMapping} get authen id maps is faulted");
                        callback?.Invoke(EnumDataState.Error, null);
                    }
                    else if (task.IsCompleted)
                    {
                        DataSnapshot snapshot = task.Result;
                        bool isExist = (snapshot != null && snapshot.ChildrenCount > 0);
                        if (isExist)
                        {
                            var data = "";
                            foreach (var rules in snapshot.Children) // rules
                            {
                                data = rules.GetRawJsonValue();
                            }

                            var socialMap = JsonUtility.FromJson<SocialMapIDEX>(data);
                            Debug.Log($"{DefineContain.DebugMapping} get authen id maps is success");
                            callback?.Invoke(EnumDataState.Exists, socialMap);
                        }
                        else
                        {
                            Debug.Log($"{DefineContain.DebugMapping} get authen id maps is no data");
                            callback?.Invoke(EnumDataState.NoData, null);
                        }
                    }
                });
        }

        public override void CreateAuthenticationMap(string authenID, SocialMapID socialMapId)
        {
            var data = (SocialMapIDEX) socialMapId;
            Debug.Log($"{DefineContain.DebugMapping} Create social map ID {authenID} - {data.GameId}");
            var json = JsonUtility.ToJson(data);
            socialMapping.Child(authenID)
                .SetRawJsonValueAsync(json)
                .ContinueWithOnMainThread(task =>
                {
                    if (task.IsCompleted)
                    {
                        Debug.Log($"{DefineContain.DebugMapping} Create social map ID is complete");
                    }
                    else
                    {
                        Debug.LogError($"{DefineContain.DebugMapping} Create social map ID is faulted");
                    }
                });
        }

        public override void GetGameIdMap(string gameID, Action<EnumDataState, GameMapID> callback)
        {
            Debug.Log($"{DefineContain.DebugMapping} get game id maps {gameID}");
            gameMapping.OrderByKey()
                .EqualTo(gameID)
                .GetValueAsync()
                .ContinueWithOnMainThread(task =>
                {
                    if (task.IsFaulted || task.IsCanceled)
                    {
                        Debug.LogError($"{DefineContain.DebugMapping} get game id maps is faulted");
                        callback?.Invoke(EnumDataState.Error, null);
                    }
                    else if (task.IsCompleted)
                    {
                        DataSnapshot snapshot = task.Result;
                        bool isExist = (snapshot != null && snapshot.ChildrenCount > 0);
                        if (isExist)
                        {
                            var socialID = "";
                            foreach (var rules in snapshot.Children) // rules
                            {
                                socialID = rules.GetRawJsonValue();
                            }

                            var gameIDs = JsonUtility.FromJson<GameMapIDEX>(socialID);
                            Debug.LogError($"{DefineContain.DebugMapping} get game id maps is success");
                            callback?.Invoke(EnumDataState.Exists, gameIDs);
                        }
                        else
                        {
                            Debug.LogError($"{DefineContain.DebugMapping} get game id maps is no data");
                            callback?.Invoke(EnumDataState.NoData, null);
                        }
                    }
                });
        }

        public override void CreateGameIdMap(string gameID, GameMapID gameMap)
        {
            var data = (GameMapIDEX) gameMap;
            Debug.Log($"{DefineContain.DebugMapping} Create game ID map {gameID} - {data.ToDataString()}");
            var json = JsonUtility.ToJson(data);

            gameMapping.Child(gameID)
                .SetRawJsonValueAsync(json)
                .ContinueWithOnMainThread(task =>
                {
                    if (task.IsCompleted)
                    {
                        Debug.Log($"{DefineContain.DebugMapping} Create game id map is complete");
                    }
                    else
                    {
                        Debug.Log($"{DefineContain.DebugMapping} Create game id map is faulted");
                    }
                });
        }

        public override void GetHashKey(string gameID, Action<EnumDataState, HashKeyID> callback)
        {
            Debug.Log($"{DefineContain.DebugMapping} get hash key {gameID}");
            hashKeyMapping.OrderByKey()
                .EqualTo(gameID)
                .GetValueAsync()
                .ContinueWithOnMainThread(task =>
                {
                    if (task.IsFaulted || task.IsCanceled)
                    {
                        Debug.LogError($"{DefineContain.DebugMapping} get hash key is Faulted");
                        callback?.Invoke(EnumDataState.Error, null);
                    }
                    else if (task.IsCompleted)
                    {
                        DataSnapshot snapshot = task.Result;
                        bool isExist = (snapshot != null && snapshot.ChildrenCount > 0);
                        if (isExist)
                        {
                            var socialID = "";
                            foreach (var rules in snapshot.Children) // rules
                            {
                                socialID = rules.GetRawJsonValue();
                            }

                            var hashKey = JsonUtility.FromJson<HashKeyIDEX>(socialID);
                            Debug.Log($"{DefineContain.DebugMapping} get hash key is Success");
                            callback?.Invoke(EnumDataState.Exists, hashKey);
                        }
                        else
                        {
                            Debug.Log($"{DefineContain.DebugMapping} get hash key is no data");
                            callback?.Invoke(EnumDataState.NoData, null);
                        }
                    }
                });
        }

        public override void CreateHashKey(string gameID, HashKeyID hashKey)
        {
            var data = (HashKeyIDEX) hashKey;
            Debug.Log($"{DefineContain.DebugMapping} Create hash key {gameID}");
            var json = JsonUtility.ToJson(data);
            hashKeyMapping.Child(gameID)
                .SetRawJsonValueAsync(json)
                .ContinueWithOnMainThread(task =>
                {
                    if (task.IsCompleted)
                    {
                        Debug.Log($"{DefineContain.DebugMapping} Create hash key  is complete");
                    }
                    else
                    {
                        Debug.LogError($"{DefineContain.DebugMapping} Create hash key  is faulted");
                    }
                });
        }

        public override void UpdateMappingID(string authenID, string gameID, GameMapID gameMapId)
        {
            CreateGameIdMap(gameID, gameMapId);

            var ids = (GameMapIDEX) gameMapId;
            //facebook
            if (!ids.authFacebook.Equals(""))
            {
                CreateAuthenticationMap(ids.authFacebook, new SocialMapIDEX {GameId = gameID});
            }

            //game center
            if (!ids.authGameCenter.Equals(""))
            {
                CreateAuthenticationMap(ids.authGameCenter, new SocialMapIDEX {GameId = gameID});
            }

            //google play
            if (!ids.authGooglePlay.Equals(""))
            {
                CreateAuthenticationMap(ids.authGooglePlay, new SocialMapIDEX {GameId = gameID});
            }

            //email
            if (!ids.authEmail.Equals(""))
            {
                CreateAuthenticationMap(ids.authEmail, new SocialMapIDEX {GameId = gameID});
            }

            //guest
            if (!ids.authGuest.Equals(""))
            {
                CreateAuthenticationMap(ids.authGuest, new SocialMapIDEX {GameId = gameID});
            }
        }
    }
}