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
        public virtual void Initialize(Action<EnumLoginState, SocialUser> callback)
        {
            loginCallback = callback;
            credentialUser = new SocialUser
            {
                email = $"{SystemInfo.deviceName}@gmail.com", 
                password = SystemInfo.deviceUniqueIdentifier,
            };
        }

        public virtual void Login(EnumProvider provider)
        {
            loginCallback?.Invoke(EnumLoginState.Success, credentialUser);
        }

        public virtual void Logout()
        {
        }

        public virtual bool IsSigned()
        {
            return true;
        }
    }
}