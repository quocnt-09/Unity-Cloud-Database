using System;
using Facebook.Unity;
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
        private string currentGameId;
        private GameMapIDEX currentGameIdMap;

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
                currentGameId = SaveManagerExample.GetMainID();

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
            CheckMappingData();
        }

        public void CheckMappingData()
        {
            /*currentGameIdMap = new GameMapIDEX()
            {
                authFacebook = SaveManagerExample.Instance.GetFirebaseAuthenID(EnumProvider.Facebook),
                authGameCenter = SaveManagerExample.Instance.GetFirebaseAuthenID(EnumProvider.GameCenter),
                authGooglePlay = SaveManagerExample.Instance.GetFirebaseAuthenID(EnumProvider.GooglePlay),
                authEmail = SaveManagerExample.Instance.GetFirebaseAuthenID(EnumProvider.Email),
                authGuest = SaveManagerExample.Instance.GetFirebaseAuthenID(EnumProvider.Guest),
            };*/
            
            currentGameIdMap = new GameMapIDEX()
            {
                authFacebook = currentAuthenID,
                authGameCenter = currentAuthenID,
                authGooglePlay = currentAuthenID,
                authEmail = currentAuthenID,
                authGuest = currentAuthenID,
            };

            mappingData.MappingData(provider, currentAuthenID, currentGameId, currentGameIdMap, (dataState, outGameID, outGameIDsMap) =>
            {
                Debug.Log($"{DefineContain.DebugMapping} MappingData dataState: {dataState}");
                switch (dataState)
                {
                    case EnumMappingState.Success:
                        currentGameIdMap = (GameMapIDEX) outGameIDsMap;
                        currentGameId = ((SocialMapIDEX) outGameID).GameId;
                        SetSocialState(EnumSocialState.CheckHashKey);
                        break;
                   
                    case EnumMappingState.CreateNewData:
                        currentGameIdMap = (GameMapIDEX) outGameIDsMap;
                        currentGameId = ((SocialMapIDEX) outGameID).GameId;
                        SetSocialState(EnumSocialState.CreateNewData);
                        break;
                    
                    case EnumMappingState.NoDataAuthen_ExitsGameId:
                        currentGameIdMap = (GameMapIDEX) outGameIDsMap;
                        currentGameId = ((SocialMapIDEX) outGameID).GameId;
                        SetSocialState(EnumSocialState.CheckHashKey);
                        break;

                    case EnumMappingState.ExitsAuthen_NoDataGameID:
                        currentGameIdMap = (GameMapIDEX) outGameIDsMap;
                        currentGameId = ((SocialMapIDEX) outGameID).GameId;
                        SetSocialState(EnumSocialState.CreateNewData);
                        break;
                    
                    case EnumMappingState.TwoAuthenOneGameID:
                    case EnumMappingState.NoDataAuthen_NoMatchGameId:
                        currentGameId = Guid.NewGuid().ToString();
                        currentGameIdMap = (GameMapIDEX) outGameIDsMap;
                        SetSocialState(EnumSocialState.CreateNewData);
                        break;

                    default:
                        SetSocialState(EnumSocialState.MappingDataFaile);
                        break;
                }
            });
        }

        void SetSocialState(EnumSocialState state)
        {
            switch (state)
            {
                case EnumSocialState.CheckHashKey:
                    CheckHashKey();
                    break;
                case EnumSocialState.CreateNewData:
                    CreateNewData();
                    break;
                case EnumSocialState.MappingDataFaile:
                    break;
            }
        }

        void CreateNewData()
        {
            mappingData.UpdateMappingID(currentGameId, currentGameId, currentGameIdMap);
        }

        void CheckHashKey()
        {
            
        }

    }
}