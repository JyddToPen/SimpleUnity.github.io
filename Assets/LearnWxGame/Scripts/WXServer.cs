#if ENABLE_WECHAT
using System;
using System.Collections;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using King;
using UnityEngine;
using UnityEngine.Networking;

namespace DefaultNamespace
{
    public class Code2SessionResult
    {
        public string openid;
        public string session_key;
        public string unionid;
        public int errcode;
        public string errmsg;
    }

    public class WXServer
    {
        public static readonly WXServer Instance = new WXServer();

        /// <summary>
        /// 请求转换code
        /// </summary>
        /// <param name="code"></param>
        public void RequestCode2Session(string code)
        {
            string code2SessionUrl =
                $"https://api.weixin.qq.com/sns/jscode2session?appid={WXSDKUtility.Instance.AppId}&secret={WXSDKUtility.Instance.AppSecret}&js_code={code}&grant_type=authorization_code";
            WxMain.Instance.StartCoroutine(OnCode2Session(code2SessionUrl));
        }

        /// <summary>
        /// 转化code
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private IEnumerator OnCode2Session(string url)
        {
            UnityWebRequest unityWebRequest = UnityWebRequest.Get(url);
            yield return unityWebRequest.SendWebRequest();
            Code2SessionResult code2SessionResult = null;
            if (unityWebRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"url:{url} e:{unityWebRequest.error}");
            }
            else
            {
                string content = unityWebRequest.downloadHandler.text;
                try
                {
                    code2SessionResult = JsonUtility.FromJson<Code2SessionResult>(content);
                }
                catch (Exception e)
                {
                    Debug.LogError($"OnCode2Session success,but parse data error content:{content} e:{e}");
                }

                code2SessionResult ??= new Code2SessionResult();
                Debug.Log($"OnCode2Session success {content}");
            }

            if (code2SessionResult.errcode > 0)
            {
                Debug.LogError(
                    $"OnCode2Session error, errcode:{code2SessionResult.errcode} errmsg:{code2SessionResult.errmsg}");
            }
        }

        /// <summary>
        /// 原生C#版
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task<Code2SessionResult> AsyncCode2Session(string code)
        {
            Code2SessionResult code2SessionResult = new Code2SessionResult();
            Debug.Log($"WXServer.AsyncCode2Session, code:{code}");
            string code2SessionUrl =
                $"https://api.weixin.qq.com/sns/jscode2session?appid={WXSDKUtility.Instance.AppId}&secret={WXSDKUtility.Instance.AppSecret}&js_code={code}&grant_type=authorization_code";
            using (HttpClient httpClient = new HttpClient())
            {
                try
                {
                    string content = await httpClient.GetStringAsync(code2SessionUrl);
                    code2SessionResult = JsonUtility.FromJson<Code2SessionResult>(content);
                    if (code2SessionResult.errcode > 0)
                    {
                        Debug.LogError(
                            $"AsyncCode2Session error, code:{code} errcode:{code2SessionResult.errcode} errmsg:{code2SessionResult.errmsg}");
                    }
                }
                catch (HttpRequestException ex)
                {
                    Debug.LogError($"AsyncCode2Session error, url:{code2SessionUrl} e:{ex}");
                }
            }

            return code2SessionResult;
        }
    }
}
#endif