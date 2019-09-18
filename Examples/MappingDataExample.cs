using System;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;

namespace Social.Database.Example
{
    [Serializable]
    public class SocialMapIDEX : SocialMapID
    {
        public string GameId;

        public string ToDataString()
        {
            return GameId;
        }
    }

    public class MappingDataExample : FirebaseDBMapping
    {
        public override void Initialize(string dataUrl)
        {
            base.Initialize(dataUrl);
            socialMapping = FirebaseDatabase.DefaultInstance.GetReference("SOCIAL_MAPS");
            gameMapping = FirebaseDatabase.DefaultInstance.GetReference("GAME_MAPS");
            userData = FirebaseDatabase.DefaultInstance.GetReference("USERS_DATA");
            hashKeyMapping = FirebaseDatabase.DefaultInstance.GetReference("HASH_KEYS");
        }

        public override void MappingData(string authenID, string gameID, Action<EnumMappingState, string, GameMapID> callback)
        {
            GetAuthenticationMap(authenID, (state, SocialMapID) =>
            {
                Debug.Log($"GetAuthenticationMap: {state}".Blue());
                if (state == EnumDataState.NoData)
                {
                    callback?.Invoke(EnumMappingState.NoData, "", null);
                }
                else if (state == EnumDataState.Success)
                {
                    callback?.Invoke(EnumMappingState.Success, SocialMapID.ToDataString(), null);
                }
                else
                {
                    callback?.Invoke(EnumMappingState.Error, "", null);
                }
            });
        }

        public override void GetAuthenticationMap(string authenID, Action<EnumDataState, SocialMapID> callback)
        {
            socialMapping.OrderByChild(authenID)
                //.EqualTo(authenID)
                .GetValueAsync()
                .ContinueWithOnMainThread(task =>
                {
                    if (task.IsFaulted || task.IsCanceled)
                    {
                        Debug.LogError("<Mapping SocialID>==## CheckDataBaseExist IsFaulted");
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
                            Debug.Log("<Mapping SocialID>==## Database EXSIT, we will re-create the database " + socialMap);
                            callback?.Invoke(EnumDataState.Success, socialMap);
                        }
                        else
                        {
                            Debug.Log("<Mapping SocialID>==## Database NOT EXSIT, we will re-create the database".Red());
                            callback?.Invoke(EnumDataState.NoData, null);
                        }
                    }
                });
        }

        public override void CreateAuthenticationMap(string authenID, SocialMapID socialMapId)
        {
            var data = (SocialMapIDEX) socialMapId;
            var json = JsonUtility.ToJson(data);
            socialMapping.Child(authenID)
                .SetRawJsonValueAsync(json)
                .ContinueWithOnMainThread(task =>
                {
                    if (task.IsCompleted)
                    {
                        Debug.LogError("<CreateAuthenticationMap>==## Success");
                    }
                    else
                    {
                        Debug.LogError("<CreateAuthenticationMap>==## Database NOT EXSIT, we will re-create the database");
                    }
                });
        }

        public override void GetGameIdMap(string gameID, Action<EnumDataState, GameMapID> callback)
        {
            base.GetGameIdMap(gameID, callback);
        }

        public override void CreateGameIdMap(string authenID, GameMapID gameMap)
        {
            base.CreateGameIdMap(authenID, gameMap);
        }

        public override void GetHashKey(string gameID, Action<EnumDataState, HashKeyID> callback)
        {
            base.GetHashKey(gameID, callback);
        }

        public override void CreateHashKey(string authenID, HashKeyID hashKey)
        {
            base.CreateHashKey(authenID, hashKey);
        }
    }
}