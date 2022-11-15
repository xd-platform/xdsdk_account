using System;
using System.Collections.Generic;
using XD.SDK.Common;

namespace XD.SDK.Account{
    public class XDGAccount
    {
        public static void Login(List<LoginType> loginTypes, Action<XDGUser> callback, Action<XDGError> errorCallback){
            XDGAccountImpl.GetInstance().Login(loginTypes, (u) =>
            {
                XDGCommon.UserId = u.userId;
                callback?.Invoke(u);
                EventManager.LoginSuccessEvent();

                if (u.loginType.ToLower().Equals("facebook")){
                    XDGTokenManager.updateFacebookRefreshTime();
                }
            }, (e) => {
                errorCallback(e);
                EventManager.LoginFailEvent(e.error_msg);
            });
        }

        public static void LoginByType(LoginType loginType, Action<XDGUser> callback, Action<XDGError> errorCallback){
            XDGAccountImpl.GetInstance().LoginByType(loginType, (u) => {
                XDGCommon.UserId = u.userId;
                callback?.Invoke(u);
                EventManager.LoginSuccessEvent();

                if (loginType == LoginType.Default){ //自动登录需要异步刷 Facebook token
                    XDGTokenManager.updateFacebookToken(u);
                } else if (loginType == LoginType.Facebook){
                    XDGTokenManager.updateFacebookRefreshTime();
                }

            }, (e) => {
                errorCallback(e);
                EventManager.LoginFailEvent(e.error_msg);
            });
        }
        
        /// <summary>
        /// 发行在主机(Steam/PS/Nintendo)上的游戏的自动登录接口
        /// </summary>
        /// <param name="successCallback">该主机的账号注册过心动账号，那么会返回登录成功（第二次登录的时候，如果主机账号没有发生改变，会直接返回登录成功，略过网络请求）</param>
        /// <param name="failCallback">该主机的账号未注册过心动账号，那么会返回失败。这个时候需要调用LoginByType登录</param>
        /// <param name="errorCallback">网络请求错误或者非主机平台的游戏调用，以及其他错误</param>
        public static void LoginByConsole(Action<XDGUser> successCallback, Action failCallback, Action<XDGError> errorCallback)
        {
            
        }


        public static void Logout()
        {
            XDGCommon.UserId = null;
            XDGAccountImpl.GetInstance().Logout();
        }

        public static void AddUserStatusChangeCallback(Action<XDGUserStatusCodeType, string> callback){
            XDGAccountImpl.GetInstance().AddUserStatusChangeCallback(callback);
        }

        public static void GetUser(Action<XDGUser> callback, Action<XDGError> errorCallback){
            XDGAccountImpl.GetInstance().GetUser((u) =>
            {
                XDGCommon.UserId = u.userId;
                callback?.Invoke(u);
            }, errorCallback);
        }

        public static void OpenUserCenter(){
            XDGAccountImpl.GetInstance().OpenUserCenter();
        }

        public static void OpenUnregister(){
            XDGAccountImpl.GetInstance().OpenUnregister();
        }
        
        //641 FB token
        public static void IsTokenActiveWithType(LoginType loginType, Action<bool> callback){
            XDGAccountImpl.GetInstance().IsTokenActiveWithType(loginType, callback);
        }
        
        //除了 Default 和 Guest
        public static void BindByType(LoginType loginType, Action<bool,XDGError> callback){
            XDGAccountImpl.GetInstance().BindByType(loginType, callback);
        }

        
    }
}