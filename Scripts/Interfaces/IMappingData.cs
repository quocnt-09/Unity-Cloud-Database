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

        void MappingData(EnumProvider providerCheck, string authenID, string gameID, GameMapID gameMaps, Action<EnumMappingState, SocialMapID, GameMapID> callback);

        void GetAuthenticationMap(string authenID, Action<EnumDataState, SocialMapID> callback);

        void CreateAuthenticationMap(string authenID, SocialMapID socialMapId);

        void GetGameIdMap(string gameID, Action<EnumDataState, GameMapID> callback);

        void CreateGameIdMap(string gameID, GameMapID gameMap);

        void GetHashKey(string gameID, Action<EnumDataState, HashKeyID> callback);

        void CreateHashKey(string gameID, HashKeyID hashKey);
        void UpdateMappingID(string authenID, string gameID, GameMapID gameMapId);
    }
}