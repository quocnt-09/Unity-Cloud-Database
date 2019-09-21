/* * * * * * 
 * Author: Quoc Nguyen
 * Email: ntq.quoc@gmail.com
 * Date: 2019-09-11
 * * * * * */

namespace Social.Database
{
    public enum EnumVerifyData
    {
        None,
        LocalNew,
        ServerNew,
        GameIdConflict,
        DeviceIdConflict,
    }

    public enum EnumProvider
    {
        None,
        Guest,
        Email,
        Facebook,
        GooglePlay,
        GameCenter,
        Firebase,
    }

    public enum EnumLoginState
    {
        Success,
        Error,
    }

    public enum EnumDataState
    {
        Error,
        Exists,
        NoData,
    }

    public enum EnumMappingState
    {
        Success,
        Error,
        CreateNewData,
        NoDataAuthen_ExitsGameId,
        NoDataAuthen_NoMatchGameId,
        ExitsAuthen_NoDataGameID,
        TwoAuthenOneGameID
    }

    public enum EnumSocialState
    {
        CheckHashKey,
        CreateNewData,
        MappingDataFaile
    }

    [System.Serializable]
    public class GameMapID
    {
    }

    public interface SocialMapID
    {
        string ToDataString();
    }

    [System.Serializable]
    public class HashKeyID
    {
    }

    [System.Serializable]
    public class SocialData
    {
    }

    [System.Serializable]
    public class HashKey
    {
    }

    public class DefineContain
    {
        public static readonly string SOCIAL_MAPPING = "SOCIAL_MAPPING";
        public static readonly string GAME_MAPPING = "GAME_MAPPING";
        public static readonly string USER_DATA = "USERS_DATA";
        public static readonly string HASH_KEY = "HASH_KEYS";

        public static readonly string DebugFacebook = "<--social facebook-->";
        public static readonly string DebugGoogle = "<--social google play-->";
        public static readonly string DebugGameCenter = "<--social game center-->";
        public static readonly string DebugFirebase = "<--social firebase-->";
        public static readonly string DebugMapping = "<--social mapping-->";
    }
}