/* * * * * * 
 * Author: Quoc Nguyen
 * Email: ntq.quoc@gmail.com
 * Date: 2019-09-11
 * * * * * */

using System;
using Firebase.Auth;
using Firebase.Extensions;
using UnityEngine;

namespace Social.Database
{
    public class LoginFirebase : ILoginSocial
    {
        protected Action<EnumLoginState, SocialUser> loginCallback;
        FirebaseAuth auth = FirebaseAuth.DefaultInstance;
        private FirebaseUser user;
        private bool signedIn;
        private EnumProvider currentProvider;
        public SocialUser credentialUser { get; set; }

        public virtual void Initialize(Action<EnumLoginState, SocialUser> callback)
        {
            signedIn = false;
            loginCallback = callback;
            currentProvider = EnumProvider.None;
            auth = FirebaseAuth.DefaultInstance;
            auth.StateChanged += AuthStateChanged;
            AuthStateChanged(this, null);
        }

        void AuthStateChanged(object sender, EventArgs eventArgs)
        {
            if (auth.CurrentUser != user)
            {
                signedIn = user != auth.CurrentUser && auth.CurrentUser != null;
                if (!signedIn && user != null)
                {
                    Debug.LogError("Signed out " + user.UserId);
                    return;
                }

                user = auth.CurrentUser;
                var providerData = user.ProviderData;
                foreach (var pro in providerData)
                {
                    var id = pro.ProviderId;
                    Debug.Log($"ProviderId: {id}");
                }

                if (signedIn)
                {
                    Debug.Log($"AuthStateChanged: Signed in {user.DisplayName} - {user.UserId} - {user.ProviderId}");
                }
            }
        }

        SocialUser CopyToSocialUser(FirebaseUser user)
        {
            var socialUser = new SocialUser();
            socialUser.uid = user.UserId ?? "";
            socialUser.userName = user.DisplayName ?? "";
            socialUser.avatar = user.PhotoUrl?.ToString();
            socialUser.email = user.Email ?? "";
            socialUser.code = user.ProviderId ?? "";
            return socialUser;
        }

        public virtual void Login(EnumProvider provider)
        {
            Debug.Log("LoginFirebase with: " + provider);
            if (currentProvider != EnumProvider.None && currentProvider != provider || (signedIn && currentProvider != provider))
            {
                Logout();
            }

            if (signedIn && (currentProvider == provider || currentProvider == EnumProvider.None))
            {
                currentProvider = provider;
                try
                {
                    loginCallback?.Invoke(EnumLoginState.Success, CopyToSocialUser(user));
                }
                catch (Exception ex)
                {
                    Debug.LogError("LoginFirebase error: " + ex);
                    loginCallback(EnumLoginState.Error, null);
                }
            }
            else
            {
                currentProvider = provider;
                switch (provider)
                {
                    case EnumProvider.Facebook:
                        AuthenticationWithFacebook();
                        break;
                    case EnumProvider.GooglePlay:
                        AuthenticationWithGooglePlay();
                        break;
                    case EnumProvider.GameCenter:
                        AuthenticationWithGameCenter();
                        break;
                    case EnumProvider.Email:
                        AuthenticationWithEmailPassword();
                        break;
                    case EnumProvider.Firebase:
                        break;
                }
            }
        }

        private void AuthenticationWithGameCenter()
        {
            if (GameCenterAuthProvider.IsPlayerAuthenticated())
            {
                GameCenterAuthProvider.GetCredentialAsync()
                    .ContinueWithOnMainThread(task =>
                    {
                        if (task.IsCanceled)
                        {
                            Debug.LogError("GetCredentialAsync was canceled ");
                            loginCallback(EnumLoginState.Error, null);
                            return;
                        }

                        if (task.IsFaulted)
                        {
                            Debug.LogError("GetCredentialAsync was faulted ");
                            loginCallback(EnumLoginState.Error, null);
                        }

                        var credential = task.Result;
                        SignInWithCredentialAsync(credential);
                    });
            }
            else
            {
                loginCallback(EnumLoginState.Error, null);
            }
        }

        private void AuthenticationWithGooglePlay()
        {
            Credential credential = PlayGamesAuthProvider.GetCredential(credentialUser.code);
            SignInWithCredentialAsync(credential);
        }

        void CreateUserWithEmailAndPasswordAsync()
        {
            auth.CreateUserWithEmailAndPasswordAsync(credentialUser.email, credentialUser.password)
                .ContinueWithOnMainThread(task =>
                {
                    if (task.IsCanceled)
                    {
                        Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                        loginCallback?.Invoke(EnumLoginState.Error, null);
                        return;
                    }

                    if (task.IsFaulted)
                    {
                        Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                        loginCallback?.Invoke(EnumLoginState.Error, null);
                        return;
                    }

                    // Firebase user has been created.
                    FirebaseUser newUser = task.Result;
                    loginCallback?.Invoke(EnumLoginState.Success, CopyToSocialUser(newUser));
                    Debug.LogFormat("User signed in successfully: {0} ({1})", newUser.DisplayName, newUser.UserId);
                });
        }

        private void AuthenticationWithEmailPassword()
        {
            auth.SignInWithEmailAndPasswordAsync(credentialUser.email, credentialUser.password)
                .ContinueWithOnMainThread(task =>
                {
                    if (task.IsCanceled)
                    {
                        Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                        CreateUserWithEmailAndPasswordAsync();
                        return;
                    }

                    if (task.IsFaulted)
                    {
                        Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                        CreateUserWithEmailAndPasswordAsync();
                        return;
                    }

                    FirebaseUser newUser = task.Result;
                    loginCallback?.Invoke(EnumLoginState.Success, CopyToSocialUser(newUser));
                    Debug.LogFormat("User signed in successfully: {0} ({1})", newUser.DisplayName, newUser.UserId);
                });
        }

        private void AuthenticationWithFacebook()
        {
            Credential credential = FacebookAuthProvider.GetCredential(credentialUser.token);
            SignInWithCredentialAsync(credential);
        }

        void SignInWithCredentialAsync(Credential credential)
        {
            auth.SignInWithCredentialAsync(credential)
                .ContinueWithOnMainThread(task =>
                {
                    if (task.IsCanceled)
                    {
                        Debug.LogError("SignInWithCredentialAsync was canceled.");
                        loginCallback(EnumLoginState.Error, null);
                        return;
                    }

                    if (task.IsFaulted)
                    {
                        Debug.LogError("SignInWithCredentialAsync encountered an error: " + task.Exception);
                        loginCallback(EnumLoginState.Error, null);
                        return;
                    }

                    FirebaseUser newUser = task.Result;
                    loginCallback?.Invoke(EnumLoginState.Success, CopyToSocialUser(newUser));
                    Debug.LogFormat("User signed in successfully: {0} ({1})", newUser.DisplayName, newUser.UserId);
                });
        }

        public virtual void Logout()
        {
            Debug.Log("LoginFirebase Logout: ");
            auth.SignOut();
            signedIn = false;
        }

        public virtual bool IsSigned()
        {
            return signedIn;
        }
    }
}