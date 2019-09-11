/* * * * * * 
 * Author: Quoc Nguyen
 * Email: ntq.quoc@gmail.com
 * Date: 2019-09-11
 * * * * * */

namespace Quocnt.Social.Database
{
    public enum EnumProvider
    {
        None,
        Facebook,
        GooglePlay,
        GameCenter,
        Email,
        Firebase
    }

    public enum EnumLoginState
    {
        Success,
        Cancel,
        Error,
    }

    public class DefineContain
    {
        public static readonly string SOCIAL_MAPPING = "SOCIAL_MAPPING";
        public static readonly string GAME_MAPPING = "GAME_MAPPING";
        public static readonly string USER_DATA = "USERS_DATA";
        public static readonly string HASH_KEY = "HASH_KEYS";
    }
}