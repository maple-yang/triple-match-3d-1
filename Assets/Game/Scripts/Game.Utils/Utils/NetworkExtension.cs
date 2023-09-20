using System;
using System.Threading.Tasks;
using UniRx;
using UnityEngine.Networking;

namespace Game.Utils.Utils
{
    public static class NetworkExtension
    {
        private static bool m_hasResult = false;
        private static bool m_result = false;

        public static void CheckInternetConnection(Action<bool> action, string url = "http://www.google.com/")
        {
            UnityWebRequest webRequest = UnityWebRequest.Get(url);
            webRequest.SendWebRequest().AsObservable()
                .Subscribe(null, (error) =>
                {
                    action?.Invoke(false);
                }, () =>
                {
                    action.Invoke(webRequest.result == UnityWebRequest.Result.Success);
                });
        }

        public static async Task<bool> CheckInternetConnectionAsync(string url = "http://www.google.com/")
        {
            m_hasResult = false;

            UnityWebRequest webRequest = UnityWebRequest.Get(url);
        
            webRequest.SendWebRequest().AsObservable()
                .Subscribe(
                    onNext: (ao) => { },
                    onError: (error) =>
                    {
                        m_hasResult = true;
                        m_result = false;
                    },
                    onCompleted: () =>
                    {
                        m_hasResult = true;
                        m_result = (webRequest.result == UnityWebRequest.Result.Success);
                    }
                );

            while (!m_hasResult)
                await Task.Yield();

            return m_result;
        }
    }
}
