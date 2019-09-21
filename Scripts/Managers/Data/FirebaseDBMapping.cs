/* * * * * * 
 * Author: Quoc Nguyen
 * Email: ntq.quoc@gmail.com
 * Date: 2019-09-11
 * * * * * */

using System;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;

namespace Social.Database
{
    public class FirebaseDBMapping : IMappingData
    {
        protected DatabaseReference socialMapping;
        protected DatabaseReference gameMapping;
        protected DatabaseReference hashKeyMapping;
        protected DatabaseReference userData;

        public virtual void Initialize(string dataUrl)
        {
            FirebaseApp.DefaultInstance.SetEditorDatabaseUrl(dataUrl);
        }

        public virtual void MappingData(EnumProvider providerCheck, string authenID, string localGameID, GameMapID gameMaps, Action<EnumMappingState, SocialMapID, GameMapID> callback)
        {
        }

        public virtual void GetAuthenticationMap(string authenID, Action<EnumDataState, SocialMapID> callback)
        {
        }

        public virtual void CreateAuthenticationMap(string authenID, SocialMapID socialMapId)
        {
        }

        public virtual void GetGameIdMap(string gameID, Action<EnumDataState, GameMapID> callback)
        {
        }

        public virtual void CreateGameIdMap(string gameID, GameMapID gameMap)
        {
        }

        public virtual void GetHashKey(string gameID, Action<EnumDataState, HashKeyID> callback)
        {
        }

        public virtual void CreateHashKey(string gameID, HashKeyID hashKey)
        {
        }

        public virtual void UpdateMappingID(string authenID, string gameID, GameMapID gameMapId)
        {
        }
    }
}