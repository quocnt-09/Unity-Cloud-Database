/* * * * * * 
 * Author: Quoc Nguyen
 * Email: ntq.quoc@gmail.com
 * Date: 2019-09-11
 * * * * * */

using System;

namespace Social.Database
{
    public class LoginGameCenter : ILoginSocial
    {
        protected Action<EnumLoginState, SocialUser> loginCallback;

        public virtual void Initialize(Action<EnumLoginState, SocialUser> callback = null)
        {
            loginCallback = callback;
        }

        public virtual void Logout()
        {
        }

        public virtual bool IsSigned()
        {
            return isSignedIn;
        }

        private bool isSignedIn = false;

        public virtual void Login(EnumProvider pro)
        {
#if UNITY_IOS && ENABLE_GAME_CENTER
            Social.localUser.Authenticate((bool success) =>
            {
                // handle success or failure
                isSignedIn = success;
                if (success)
                {
                    var id = "";
                    var name = "";
                    try
                    {
                        id = Social.localUser.id;
                        name = Social.localUser.userName;
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError("Game Center Exception: " + ex);
                    }

                    loginCallback?.Invoke(EnumLoginState.Success, new SocialUser
                    {
                        uid = id,
                        userName = name,
                    });
                }
                else
                {
                    loginCallback?.Invoke(EnumLoginState.Error, null);
                }
            });
#endif
        }
    }
}