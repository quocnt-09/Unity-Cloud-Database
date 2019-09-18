/* * * * * * 
 * Author: Quoc Nguyen
 * Email: ntq.quoc@gmail.com
 * Date: 2019-09-11
 * * * * * */

using System;
using UnityEngine;

namespace Social.Database
{
    public class LoginEmail : ILoginSocial
    {
        public SocialUser credentialUser { get; set; }
        protected Action<EnumLoginState, SocialUser> loginCallback;
        public void Initialize(Action<EnumLoginState, SocialUser> callback)
        {
            loginCallback = callback;
            credentialUser = new SocialUser
            {
                email = $"{SystemInfo.deviceName}@gmail.com", 
                password = SystemInfo.deviceUniqueIdentifier,
            };
        }

        public void Login(EnumProvider provider)
        {
            loginCallback?.Invoke(EnumLoginState.Success, credentialUser);
        }

        public void Logout()
        {
        }

        public bool IsLogin()
        {
            return false;
        }
    }
}