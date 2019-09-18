/* * * * * * 
 * Authurt: Quoc Nguyen
 * Email: ntq.quoc@gmail.com
 * Date: 2019-09-11
 * * * * * */

using System;

namespace Social.Database
{
    [Serializable]
    public class SocialUser
    {
        public string uid;
        public string userName;
        public string email;
        public string password;
        public string avatar;
        public string token;
        public string code;

        public string DebugString()
        {
            var txt = "";
            if (!string.IsNullOrEmpty(uid))
            {
                txt += $"uid: {uid}";
            }

            if (!string.IsNullOrEmpty(userName))
            {
                txt += $"\nuserName: {userName}";
            }

            if (!string.IsNullOrEmpty(email))
            {
                txt += $"\nemail: {email}";
            }

            /*if (!string.IsNullOrEmpty(token))
            {
                txt += $"\ntoken: {token}";
            }

            if (!string.IsNullOrEmpty(code))
            {
                txt += $"\ncode: {code}";
            }*/

            return txt;
        }
    }
}