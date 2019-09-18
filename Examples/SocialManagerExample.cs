using System;
using UnityEngine;
using UnityEngine.UI;

namespace Social.Database.Example
{
    public class SocialManagerExample : MonoBehaviour
    {
        public Text txtDebug;
        public GameObject buttonFirebase;
        public GameObject buttonSyncData;
        private string text;

        private LoginFacebook loginFacebook;
        private LoginGameCenter loginGameCenter;
        private LoginGooglePlay loginGooglePlay;
        private LoginEmail loginEmail;
        private LoginFirebase loginFirebase;
        private MappingDataExample mappingData;
        private EnumProvider provider;
        private string currentAuthenID;
        private string localGameId;

        private void Start()
        {
            buttonSyncData.SetActive(false);
            buttonFirebase.SetActive(false);

            loginFacebook = new LoginFacebook();
            loginFacebook.Initialize(OnLoginFacebookSuccess);

            loginGameCenter = new LoginGameCenter();
            loginGameCenter.Initialize(OnLoginGameCenterSuccess);

            loginGooglePlay = new LoginGooglePlay();
            loginGooglePlay.Initialize(OnLoginGooglePlaySuccess);

            loginEmail = new LoginEmail();
            loginEmail.Initialize(OnLoginEmailSuccess);

            loginFirebase = new LoginFirebase();
            loginFirebase.Initialize(OnLoginFirebaseSuccess);

            mappingData = new MappingDataExample();
            mappingData.Initialize("https://unity-framework-1109.firebaseio.com/");
        }

        private void OnLoginFirebaseSuccess(EnumLoginState state, SocialUser user)
        {
            text += $"\nLogin: {state}";
            if (state == EnumLoginState.Success)
            {
                loginFirebase.credentialUser = user;
                currentAuthenID = user.uid;
                localGameId = SaveManagerExample.GetMainID();

                text += $"\nInfo: {user.DebugString()}";
                buttonSyncData.SetActive(true);
            }
        }

        private void OnLoginEmailSuccess(EnumLoginState state, SocialUser user)
        {
            text += $"\nLogin: {state}";
            if (state == EnumLoginState.Success)
            {
                loginFirebase.credentialUser = user;
                text += $"\nInfo: {user.DebugString()}";
                buttonFirebase.gameObject.SetActive(true);
            }
        }

        private void OnLoginGooglePlaySuccess(EnumLoginState state, SocialUser user)
        {
            if (state == EnumLoginState.Success)
            {
                buttonFirebase.gameObject.SetActive(true);
            }
        }

        private void OnLoginGameCenterSuccess(EnumLoginState state, SocialUser user)
        {
            if (state == EnumLoginState.Success)
            {
                buttonFirebase.gameObject.SetActive(true);
            }
        }

        private void OnLoginFacebookSuccess(EnumLoginState state, SocialUser user)
        {
            text += $"\nLogin: {state}";
            if (state == EnumLoginState.Success)
            {
                loginFirebase.credentialUser = user;
                text += $"\nInfo: {user.DebugString()}";
                buttonFirebase.gameObject.SetActive(true);
            }
        }

        private void Update()
        {
            txtDebug.text = text;
        }

        public void OnButtonFacebook()
        {
            provider = EnumProvider.Facebook;
            text = $"Platform: {provider}";
            loginFacebook.Login(provider);
        }

        public void OnButtonGameCenter()
        {
            loginGameCenter.Login(EnumProvider.GameCenter);
        }

        public void OnButtonGooglePlay()
        {
            loginGooglePlay.Login(EnumProvider.GooglePlay);
        }

        public void OnButtonEmail()
        {
            provider = EnumProvider.Email;
            text = $"Platform: {provider}";
            loginEmail.Login(provider);
        }

        public void OnButtonFirebase()
        {
            text += "\n\nLogin Firebase";
            loginFirebase.Login(provider);
        }

        public void OnSyncData()
        {
            text += "\n\nSync Data: " + currentAuthenID;
            mappingData.MappingData(currentAuthenID, localGameId, (state, outGameId, outGameMaps) =>
            {
                text += "\nState: " + state;
                text += "\nOutGameID: " + outGameId;
                text += "\nOutGameMaps: " + outGameMaps;
                switch (state)
                {
                    case EnumMappingState.Success:
                        break;
                    case EnumMappingState.Error:
                        break;
                    case EnumMappingState.NoData:
                        mappingData.CreateAuthenticationMap(currentAuthenID, new SocialMapIDEX {GameId = localGameId});
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(state), state, null);
                }
            });
        }
    }
}