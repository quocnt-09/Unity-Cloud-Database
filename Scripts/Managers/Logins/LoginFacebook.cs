/* * * * * * 
 * Author: Quoc Nguyen
 * Email: ntq.quoc@gmail.com
 * Date: 2019-09-11
 * * * * * */

using System;
using System.Collections.Generic;
using UnityEngine;
#if ENABLE_FACEBOOK
using Facebook.Unity;

#endif

namespace Quocnt.Social.Database
{
    public class LoginFacebook : ILoginSocial
    {
        protected Action<EnumLoginState, SocialUser> loginCallback;
        List<string> loginPermission = new List<string>() {"public_profile", "email"};

        public virtual void Initialize(Action<EnumLoginState, SocialUser> callback)
        {
            loginCallback = callback;
#if ENABLE_FACEBOOK
            if (!FB.IsInitialized)
            {
                // Initialize the Facebook SDK
                FB.Init(InitCallback, OnHideUnity);
            }
            else
            {
                // Already initialized, signal an app activation App Event
                FB.ActivateApp();
            }
#endif
        }

        public virtual void Login(EnumProvider provider)
        {
#if ENABLE_FACEBOOK
            FB.LogInWithReadPermissions(loginPermission, AuthCallback);
#endif
        }

        public virtual void Logout()
        {
        }

        public virtual bool IsLogin()
        {
            return false;
        }

#if ENABLE_FACEBOOK
        private void InitCallback()
        {
            if (FB.IsInitialized)
            {
                // Signal an app activation App Event
                FB.ActivateApp();
                // Continue with Facebook SDK
                // ...
            }
            else
            {
                Debug.LogError("Failed to Initialize the Facebook SDK");
            }
        }

        private void OnHideUnity(bool isGameShown)
        {
            if (!isGameShown)
            {
                // Pause the game - we will need to hide
                Time.timeScale = 0;
            }
            else
            {
                // Resume the game - we're getting focus again
                Time.timeScale = 1;
            }
        }

        private void AuthCallback(ILoginResult result)
        {
            var socialUser = new SocialUser();
            if (FB.IsLoggedIn)
            {
                // AccessToken class will have session details
                var aToken = AccessToken.CurrentAccessToken;
                // Print current access token's User ID
                Debug.Log(aToken.UserId);
                // Print current access token's granted permissions
                foreach (string perm in aToken.Permissions)
                {
                    Debug.Log("LoginFacebook: " + perm);
                }

                socialUser.uid = aToken.UserId;
                socialUser.token = aToken.TokenString;
                loginCallback?.Invoke(EnumLoginState.Success, socialUser);
            }
            else
            {
                Debug.LogError("LoginFacebook: => User cancelled login");
                loginCallback?.Invoke(EnumLoginState.Cancel, socialUser);
            }
        }
#endif
    }
}