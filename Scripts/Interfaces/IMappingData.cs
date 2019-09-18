/* * * * * * 
 * Author: Quoc Nguyen
 * Email: ntq.quoc@gmail.com
 * Date: 2019-09-11
 * * * * * */

using System;

namespace Social.Database
{
    public interface IMappingData
    {
        void Initialize(string dataURL);

        void MappingData(string authenID, string gameID, Action<EnumMappingState, string, GameMapID> callback);

        void GetAuthenticationMap(string authenID, Action<EnumDataState, SocialMapID> callback);

        void CreateAuthenticationMap(string authenID, SocialMapID socialMapId);

        void GetGameIdMap(string gameID, Action<EnumDataState, GameMapID> callback);

        void CreateGameIdMap(string authenID, GameMapID gameMap);

        void GetHashKey(string gameID, Action<EnumDataState, HashKeyID> callback);

        void CreateHashKey(string authenID, HashKeyID hashKey);
    }
}