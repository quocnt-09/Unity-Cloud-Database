﻿/* * * * * * 
 * Author: Quoc Nguyen
 * Email: ntq.quoc@gmail.com
 * Date: 2019-09-11
 * * * * * */

using System;

namespace Social.Database
{
    public class LoginGooglePlay : ILoginSocial
    {
        protected Action<EnumLoginState, SocialUser> loginCallback;

        public virtual void Initialize(Action<EnumLoginState, SocialUser> callback)
        {
            loginCallback = callback;
        }

        public virtual void Login(EnumProvider provider)
        {
        }

        public virtual void Logout()
        {
        }

        public virtual bool IsLogin()
        {
            return false;
        }
    }
}