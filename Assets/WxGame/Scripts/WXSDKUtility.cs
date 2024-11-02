using System;
using System.IO;
using System.Text.RegularExpressions;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.UI;
using WeChatWASM;

namespace King
{
    public struct WxLoginData
    {
        /// <summary>
        /// 标识码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 微信用户信息
        /// </summary>
        public WXUserInfo WxUserInfo { get; set; }
    }

    public class InternalData
    {
        public Button m_ButtonUGUIWeChat => WxMain.Instance.btnWxUserInfo;

        public static implicit operator bool(InternalData outData)
        {
            return outData != null && outData.m_ButtonUGUIWeChat;
        }
    }

    public class WXSDKUtility
    {
        public static WXSDKUtility Instance = new WXSDKUtility();
        private string _wxCode;
        public string WxCode => _wxCode;

        //出于安全考虑，通过这个链接获取下，这里不再展示 https://mp.weixin.qq.com/
        private string[] _wxIds;
        private string[] WxIds
        {
            get
            {
                if (_wxIds == null)
                {
                    var ret = Regex.Match(File.ReadAllText(Application.dataPath + "/../Library/Wx.txt"), @"(\w+)\s(\w+)");
                    _wxIds = new[] { ret.Groups[1].Value, ret.Groups[2].Value };
                }
                return _wxIds;
            }
        }

        public string AppId => WxIds[0];
        
        public string AppSecret => WxIds[1];

        public event Action<WxLoginData> LoginResponseEvent;

        public InternalData _view = new InternalData();

        public void InitSDK()
        {
            WX.InitSDK(OnSdkInitComplete);
        }

        /// <summary>
        /// 初始化结束
        /// </summary>
        /// <param name="code"></param>
        private void OnSdkInitComplete(int code)
        {
            Debug.Log($"WeiXinLogin.OnSdkInitComplete, and then start login, code:{code}");
            LoginOption loginOption = new LoginOption()
            {
                success = OnSdkLoginSuccess,
                fail = OnSdkLoginFail
            };
            WX.Login(loginOption);
        }

        #region SDK登录

        /// <summary>
        /// sdk登录成功
        /// </summary>
        /// <param name="result"></param>
        private void OnSdkLoginSuccess(LoginSuccessCallbackResult result)
        {
            Debug.Log($"WeiXinLogin.OnSdkLoginSuccess, code={result.code} msg={result.errMsg}");
            _wxCode = result.code;
            GetSettingOption getSettingOption = new GetSettingOption()
            {
                success = GetSettingSuccess,
                fail = GetSettingFail
            };
            WX.GetSetting(getSettingOption);
        }

        /// <summary>
        /// sdk登录失败
        /// </summary>
        /// <param name="requestFailCallbackErr"></param>
        private void OnSdkLoginFail(RequestFailCallbackErr requestFailCallbackErr)
        {
            Debug.LogError(
                $"WeiXinLogin.OnSdkLoginFail, code={requestFailCallbackErr.errno} msg={requestFailCallbackErr.errMsg}");
        }

        #endregion

        #region 获取设置信息

        /// <summary>
        /// 获取玩家配置成功
        /// </summary>
        /// <param name="result"></param>
        private void GetSettingSuccess(GetSettingSuccessCallbackResult result)
        {
            Debug.Log($"WeiXinLogin.GetSettingSuccess");
            if (!result.authSetting.ContainsKey("scope.userInfo") || !result.authSetting["scope.userInfo"])
            {
                if (!_view)
                {
                    Debug.LogError(
                        $"WeiXinLogin.GetSettingSuccess,while simulation button is null!!");
                    return;
                }

                (Vector2 wxPosition, Vector2 size) =
                    GetUIWxScreenPosition(_view.m_ButtonUGUIWeChat.gameObject,
                        _view.m_ButtonUGUIWeChat.gameObject.GetComponentInParent<Canvas>());
                Debug.Log(
                    $"WeiXinLogin.GetSettingSuccess create a visible button to simulate login");
                WXUserInfoButton wxUserInfoButton =
                    WX.CreateUserInfoButton((int)wxPosition.x, (int)wxPosition.y, (int)size.x, (int)size.y, "zh_CN",
                        false);
                wxUserInfoButton.Show();
                wxUserInfoButton.OnTap((data) =>
                {
                    if (data.errCode == 0)
                    {
                        Debug.Log(
                            $"WeiXinLogin.GetSettingSuccess user agrees authorisation!");
                        OnResponseUserInfo(data.userInfo);
                    }
                    else
                    {
                        Debug.Log(
                            $"WeiXinLogin.GetSettingSuccess user refuses authorisation!");
                    }

                    wxUserInfoButton.Hide();
                    if (_view)
                    {
                        _view.m_ButtonUGUIWeChat.gameObject.SetActive(false);
                    }
                });
            }
            else
            {
                Debug.Log(
                    $"WeiXinLogin.GetSettingSuccess user have been authorised!");
                GetUserInfoOption getUserInfoOption = new GetUserInfoOption()
                {
                    lang = "zh_CN",
                    withCredentials = false,
                    success = GetUserInfoSuccess,
                    fail = GetUserInfoFail
                };
                WX.GetUserInfo(getUserInfoOption);
            }
        }

        /// <summary>
        /// 获取设置失败
        /// </summary>
        /// <param name="result"></param>
        private void GetSettingFail(GeneralCallbackResult result)
        {
            Debug.LogError($"WeiXinLogin.GetSettingFail, {result.errMsg}");
        }

        #endregion


        /// <summary>
        /// 获取微信的屏幕坐标
        /// </summary>
        /// <param name="uiNode"></param>
        /// <param name="canvas"></param>
        public (Vector2 wxPosition, Vector2 size) GetUIWxScreenPosition(GameObject uiNode, Canvas canvas)
        {
            RectTransform nodeRect = uiNode.GetComponent<RectTransform>();
            Vector2 size = nodeRect.sizeDelta;
            Vector2 screenPosition = Vector2.zero;
            switch (canvas.renderMode)
            {
                case RenderMode.ScreenSpaceCamera:
                    screenPosition =
                        RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, uiNode.transform.position);
                    break;
                case RenderMode.ScreenSpaceOverlay:
                    screenPosition = nodeRect.position;
                    screenPosition = new Vector2(screenPosition.x - size.x / 2, screenPosition.y + size.y / 2);
                    break;
            }

            Vector2 wxScreenPosition = new Vector2(screenPosition.x, Screen.height - screenPosition.y);
            Debug.Log(
                $"WeiXinLogin.GetWxScreenPosition, wxScreenPosition:{wxScreenPosition} screenPosition:{screenPosition} Screen.width:{Screen.width} Screen.height:{Screen.height} size:{size}");
            return (wxScreenPosition, size);
        }

        #region 获取用户信息

        /// <summary>
        /// 获取玩家信息成功回调
        /// </summary>
        /// <param name="data"></param>
        private void GetUserInfoSuccess(GetUserInfoSuccessCallbackResult data)
        {
            WXUserInfo wxUserInfo = new WXUserInfo()
            {
                nickName = data.userInfo.nickName,
                avatarUrl = data.userInfo.avatarUrl,
                gender = (int)data.userInfo.gender
            };
            OnResponseUserInfo(wxUserInfo);
        }

        /// <summary>
        /// 获取用户信息失败
        /// </summary>
        /// <param name="data"></param>
        private void GetUserInfoFail(GeneralCallbackResult data)
        {
            Debug.LogError($"WeiXinLogin.GetUserInfoFail, {data.errMsg}");
        }

        #endregion

        /// <summary>
        /// 响应用户的信息
        /// </summary>
        /// <param name="wxUserInfo"></param>
        private void OnResponseUserInfo(WXUserInfo wxUserInfo)
        {
            Debug.Log(
                $"OnResponseUserInfo 用户名：{wxUserInfo.nickName} 用户头像{wxUserInfo.avatarUrl} LoginResponseEvent:{LoginResponseEvent}");
            LoginResponseEvent?.Invoke(new WxLoginData()
            {
                Code = _wxCode,
                WxUserInfo = wxUserInfo
            });
        }
    }
}