using System;
using System.Collections.Generic;

namespace XD.SDK.Common{
    public class XDGCommon{
        public static void InitSDK(bool isCN, Action<bool, string> callback){
            XDGCommonImpl.GetInstance().InitSDK(isCN, callback);
        }

        public static void IsInitialized(Action<bool> callback){
            XDGCommonImpl.GetInstance().IsInitialized(callback);
        }

        public static void SetLanguage(LangType langType){
            XDGCommonImpl.GetInstance().SetLanguage(langType);
        }

        public static void Share(ShareFlavors shareFlavors, string uri, XDGShareCallback callback){
            XDGCommonImpl.GetInstance().Share(shareFlavors, uri, callback);
        }

        public static void Share(ShareFlavors shareFlavors, string uri, string message, XDGShareCallback callback){
            XDGCommonImpl.GetInstance().Share(shareFlavors, uri, message, callback);
        }

        public static void Report(string serverId, string roleId, string roleName){
            XDGCommonImpl.GetInstance().Report(serverId, roleId, roleName);
        }

        public static void TrackRole(string serverId, string roleId, string roleName, int level){
            XDGCommonImpl.GetInstance().TrackRole(serverId, roleId, roleName, level);
        }

        public static void TrackUser(string userId){
            XDGCommonImpl.GetInstance().TrackUser(userId);
        }

        public static void TrackEvent(string eventName){
            XDGCommonImpl.GetInstance().TrackEvent(eventName);
        }

        public static void EventCompletedTutorial(){
            XDGCommonImpl.GetInstance().EventCompletedTutorial();
        }

        public static void EventCreateRole(){
            XDGCommonImpl.GetInstance().EventCreateRole();
        }

        public static void GetVersionName(Action<string> callback){
            XDGCommonImpl.GetInstance().GetVersionName(callback);
        }

        public static void TrackAchievement(){
            XDGCommonImpl.GetInstance().TrackAchievement();
        }

        public static void SetCurrentUserPushServiceEnable(bool enable){
            XDGCommonImpl.GetInstance().SetCurrentUserPushServiceEnable(enable);
        }

        public static void IsCurrentUserPushServiceEnable(Action<bool> callback){
            XDGCommonImpl.GetInstance().IsCurrentUserPushServiceEnable(callback);
        }

        public static void StoreReview(){
            XDGCommonImpl.GetInstance().StoreReview();
        }

        public static void ShowLoading(){
            XDGCommonImpl.GetInstance().ShowLoading();
        }

        public static void HideLoading(){
            XDGCommonImpl.GetInstance().HideLoading();
        }

        public static void GetRegionInfo(Action<XDGRegionInfoWrapper> callback){
            XDGCommonImpl.GetInstance().GetRegionInfo(callback);
        }
        
        public static void DisableAgreementUI(){
            XDGCommonImpl.GetInstance().disableAgreementUI();
        }

    }
}