/* * * * * * 
 * Author: Quoc Nguyen
 * Email: ntq.quoc@gmail.com
 * Date: 2019-09-11
 * * * * * */ 

namespace Quocnt.Social.Database
{
    public interface ILoginSocial
    {
        void Initialize();
        void Login();
        void Logout();
        bool IsLogin();
    }
}