/* * * * * * 
 * Author: Quoc Nguyen
 * Email: ntq.quoc@gmail.com
 * Date: 2019-09-11
 * * * * * */

using System;

namespace Social.Database
{
    public interface ILoginSocial
    {
        void Initialize(Action<EnumLoginState, SocialUser> callback = null);
        void Login(EnumProvider provider);
        void Logout();
        bool IsSigned(); 
    }
}