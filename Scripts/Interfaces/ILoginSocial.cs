/* * * * * * 
 * Author: Quoc Nguyen
 * Email: ntq.quoc@gmail.com
 * Date: 2019-09-11
 * * * * * */

using System;

namespace Quocnt.Social.Database
{
    public interface ILoginSocial
    {
        void Initialize(Action<EnumLoginState, SocialUser> callback);
        void Login(EnumProvider provider);
        void Logout();
        bool IsLogin();
    }
}