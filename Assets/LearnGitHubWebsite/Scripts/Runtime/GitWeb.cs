using System;
using System.Collections;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace LearnGitHubWebsite.Scripts.Runtime
{
    public class GitWeb : MonoBehaviour
    {
        private const string AbsoluteUrl = "https://jyddtopen.github.io/SimpleUnity.github.io/?message=";

        public Button btnGetUserName;
        public Button btnGetUserNameByTask;

        public Text labMessage;
    
        // Start is called before the first frame update
        void Start()
        {
            btnGetUserName.onClick.AddListener(OnClickUserName);
            btnGetUserNameByTask.onClick.AddListener(OnClickLoadContentByTask);
        }

        #region 使用unityAPI获取信息
        private void OnClickUserName()
        {
            StartCoroutine(OnGetUserName());
        }

        private IEnumerator OnGetUserName()
        {
            string url = $"{AbsoluteUrl}{Uri.EscapeDataString("HelloWorld King")}";
            using UnityWebRequest unityWebRequest = UnityWebRequest.Get(url);
            yield return unityWebRequest.SendWebRequest();
            string userName = unityWebRequest.downloadHandler.text;
            labMessage.text = userName;
        }
        #endregion

        #region 使用C#API获取信息
        private void OnClickLoadContentByTask()
        {
            Task.Run(OnLoadContentByTask);
        }

        private async Task OnLoadContentByTask()
        {
            string url = $"{AbsoluteUrl}{Uri.EscapeDataString("HelloWorld King")}";
            using HttpClient client = new HttpClient();
            try
            {
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                labMessage.text = responseBody;
            }
            catch (HttpRequestException e)
            {
                labMessage.text = e.Message;
            }
        }
        #endregion

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
