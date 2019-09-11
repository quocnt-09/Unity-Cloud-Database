/* * * * * * 
 * Author: Quoc Nguyen
 * Email: ntq.quoc@gmail.com
 * Date: 2019-09-11
 * * * * * */

using System;
using Firebase.Auth;
using Firebase.Extensions;
using UnityEngine;

namespace Quocnt.Social.Database
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
                }

                user = auth.CurrentUser;
                if (signedIn)
                {
                    Debug.Log($"AuthStateChanged: Signed in {user.DisplayName} - {user.UserId}");
                }
            }
        }

        public virtual void Login(EnumProvider provider)
        {
            Debug.Log("LoginFirebase with: " + provider);
            if (currentProvider != EnumProvider.None && currentProvider != provider)
            {
                Logout();
            }

            if (signedIn && (currentProvider == provider || currentProvider == EnumProvider.None))
            {
                currentProvider = provider;
                try
                {
                    loginCallback?.Invoke(EnumLoginState.Success, new SocialUser
                    {
                        uid = user.UserId,
                        userName = user.DisplayName ?? "",
                        avatar = user.PhotoUrl.ToString(),
                        email = user.Email,
                        code = user.ProviderId,
                    });
                }
                catch (Exception ex)
                {
                    loginCallback(EnumLoginState.Error, null);
                }
            }
            else
            {
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
                        bool isSuccess = true;
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

        private void AuthenticationWithEmailPassword()
        {
            auth.SignInWithEmailAndPasswordAsync(credentialUser.email, credentialUser.password)
                .ContinueWithOnMainThread(task =>
                {
                    if (task.IsCompleted)
                    {
                        FirebaseUser newUser = task.Result;
                        var firebaseUser = new SocialUser
                        {
                            uid = newUser.UserId,
                            userName = newUser.DisplayName ?? "",
                            avatar = newUser.PhotoUrl.ToString(),
                            email = newUser.Email,
                            code = newUser.ProviderId,
                        };
                        loginCallback?.Invoke(EnumLoginState.Success, firebaseUser);
                        Debug.LogFormat("User signed in successfully: {0} ({1})", newUser.DisplayName, newUser.UserId);
                        return;
                    }

                    if (task.IsCanceled)
                    {
                        Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                        loginCallback(EnumLoginState.Error, null);
                        return;
                    }

                    if (task.IsFaulted)
                    {
                        Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                        loginCallback(EnumLoginState.Error, null);
                        return;
                    }

                    Credential credential = EmailAuthProvider.GetCredential(credentialUser.email, credentialUser.password);
                    SignInWithCredentialAsync(credential);
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
                    var firebaseUser = new SocialUser
                    {
                        uid = newUser.UserId,
                        userName = newUser.DisplayName ?? "",
                        avatar = newUser.PhotoUrl.ToString(),
                        email = newUser.Email,
                        code = newUser.ProviderId,
                    };
                    loginCallback?.Invoke(EnumLoginState.Success, firebaseUser);
                    Debug.LogFormat("User signed in successfully: {0} ({1})", newUser.DisplayName, newUser.UserId);
                });
        }

        public virtual void Logout()
        {
            auth.SignOut();
            signedIn = false;
        }

        public virtual bool IsLogin()
        {
            return signedIn;
        }
    }
}