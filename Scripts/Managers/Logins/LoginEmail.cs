/* * * * * * 
 * Author: Quoc Nguyen
 * Email: ntq.quoc@gmail.com
 * Date: 2019-09-11
 * * * * * */

using System;
using UnityEngine;

namespace Quocnt.Social.Database
{
    public class LoginEmail : ILoginSocial
    {
        public SocialUser credentialUser { get; set; }

        public void Initialize(Action<EnumLoginState, SocialUser> callback)
        {
            credentialUser = new SocialUser
            {
                email = $"{SystemInfo.deviceName}@gmail.com", 
                password = SystemInfo.deviceName,
            };
        }

        public void Login(EnumProvider provider)
        {
            
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