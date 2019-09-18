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
        Email,
        Facebook,
        GooglePlay,
        GameCenter,
        Firebase
    }

    public enum EnumLoginState
    {
        Success,
        Cancel,
        Error,
    }

    public enum EnumDataState
    {
        Error,
        Cancel,
        Success,
        NoData,
    }

    public enum EnumMappingState
    {
        Success,
        Error,
        NoData
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
    }
}