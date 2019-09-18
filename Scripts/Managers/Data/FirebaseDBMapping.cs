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

        public virtual void MappingData(string authenID, string gameID, Action<EnumMappingState, string, GameMapID> callback)
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

        public virtual void CreateGameIdMap(string authenID, GameMapID gameMap)
        {
        }

        public virtual void GetHashKey(string gameID, Action<EnumDataState, HashKeyID> callback)
        {
        }

        public virtual void CreateHashKey(string authenID, HashKeyID hashKey)
        {
        }
    }
}