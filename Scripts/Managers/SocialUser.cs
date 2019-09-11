/* * * * * * 
 * Authurt: Quoc Nguyen
 * Email: ntq.quoc@gmail.com
 * Date: 2019-09-11
 * * * * * */ 

namespace Quocnt.Social.Database
{
    [System.Serializable]
    public class SocialUser
    {
        public string uid;
        public string userName;
        public string email;
        public string password;
        public string avatar;
        public string token;
        public string code;
    }
}