/* * * * * * 
 * Author: Quoc Nguyen
 * Email: ntq.quoc@gmail.com
 * Date: 2019-09-11
 * * * * * */

using System;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine;

namespace Social.Database
{
    public class LoginGooglePlay : ILoginSocial
    {
        protected Action<EnumLoginState, SocialUser> loginCallback;
        private bool isSignedIn;

        public virtual void Initialize(Action<EnumLoginState, SocialUser> callback)
        {
            loginCallback = callback;

            PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
                // requests a server auth code be generated so it can be passed to an
                //  associated back end server application and exchanged for an OAuth token.
                .RequestServerAuthCode(false)
                // requests an ID token be generated.  This OAuth token can be used to
                //  identify the player to other services such as Firebase.         
                .Build();

            PlayGamesPlatform.InitializeInstance(config);
            // recommended for debugging:
            PlayGamesPlatform.DebugLogEnabled = true;
            // Activate the Google Play Games platform
            PlayGamesPlatform.Activate();
        }

        public virtual void Login(EnumProvider provider)
        {
#if UNITY_ANDROID && ENABLE_GOOGLE_PLAY
            Debug.Log($"{DefineContain.DebugGoogle} Login with google play game");
            Social.localUser.Authenticate((bool success) =>
            {
                isSignedIn = success;
                if (isSignedIn)
                {
                    Debug.Log($"{DefineContain.DebugGoogle} Login success {Social.localUser.id}");
                    loginCallback?.Invoke(EnumLoginState.Success, new SocialUser
                    {
                        uid = Social.localUser.id, 
                        userName = Social.localUser.userName, 
                        code = PlayGamesPlatform.Instance.GetServerAuthCode(),
                    });
                }
                else
                {
                    Debug.LogError($"{DefineContain.DebugGoogle} Login error");
                    loginCallback?.Invoke(EnumLoginState.Error, null);
                }
            });
#endif
        }

        public virtual void Logout()
        {
            PlayGamesPlatform.Instance.SignOut();
            isSignedIn = false;
        }

        public bool IsSigned()
        {
#if UNITY_ANDROID && ENABLE_GOOGLE_PLAY
            return Social.localUser.authenticated && isSignedIn;
#endif
            return false;
        }
    }
}