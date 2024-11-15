#if ENABLE_WECHAT
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using King;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class WxMain : MonoBehaviour
{
    /// <summary>
    /// 图片
    /// </summary>
    [SerializeField] RawImage head;

    private static WxMain _instance;

    public static WxMain Instance => _instance;

    public Button btnWxUserInfo;
    public Button btnWxCode2Session;

    private void Awake()
    {
        _instance = this;
        btnWxCode2Session.onClick.AddListener(() =>
        {
            WXServer.Instance.RequestCode2Session(WXSDKUtility.Instance.WxCode);
        });
    }

    IEnumerator Start()
    {
        Debug.Log($"游戏启动了, 添加了组件 view:{WXSDKUtility.Instance._view == true}");
        RequestWxLogin();

        yield break;
    }

    /// <summary>
    /// 请求微信登录
    /// </summary>
    private void RequestWxLogin()
    {
        WXSDKUtility.Instance.LoginResponseEvent += OnWxLoginSuccess;
        WXSDKUtility.Instance.InitSDK();
        WXSDKUtility.Instance.GetUIWxScreenPosition(btnWxUserInfo.gameObject,
            btnWxUserInfo.gameObject.GetComponentInParent<Canvas>());
    }

    /// <summary>
    /// 登录成功
    /// </summary>
    /// <param name="wxLoginData"></param>
    private void OnWxLoginSuccess(WxLoginData wxLoginData)
    {
        Debug.Log(
            $"OnWxLoginSuccess {wxLoginData.WxUserInfo.avatarUrl} nickName:{wxLoginData.WxUserInfo.avatarUrl}");
        StartCoroutine(ShowWxHead(wxLoginData.WxUserInfo.avatarUrl));
    }

    /// <summary>
    /// 展示微信头像
    /// </summary>
    /// <param name="headUrl"></param>
    /// <returns></returns>
    private IEnumerator ShowWxHead(string headUrl)
    {
        Debug.Log($"开始请求头像 {headUrl}");
        UnityWebRequest unityWebRequest = UnityWebRequestTexture.GetTexture(headUrl);
        yield return unityWebRequest.SendWebRequest();
        if (unityWebRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"Error downloading image: {unityWebRequest.error}");
        }
        else
        {
            // Get the texture from the response
            Texture2D texture = DownloadHandlerTexture.GetContent(unityWebRequest);
            head.texture = texture;
            head.SetNativeSize();
            head.enabled = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}
#endif