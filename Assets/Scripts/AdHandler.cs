using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AdHandler : MonoBehaviour
{

    public Object AdPrefab;

    public void ShowVideoAd() 
    {
        StartCoroutine(GetVideoUrl());
    }

    public void ShowImageAd() 
    {
        StartCoroutine(GetImageUrl());
    }

    class MediaAd 
    {
        public string id;
        public string url;
    }

    IEnumerator GetVideoUrl()
    {
        GameObject Ad = Instantiate(AdPrefab) as GameObject;
        AdCanvas Canvas = Ad.GetComponent<AdCanvas>();
        string key = PlayerPrefs.GetString("PlaymanityAPIKey");
        if (key.Length == 0)
        {
            Debug.LogError("Missing Playmanity api key");

        }

        using (UnityWebRequest req = UnityWebRequest.Get("https://api.playmanity.com/ads/watch?type=2"))
        {
            req.SetRequestHeader("Authorization", key);
            yield return req.SendWebRequest();
            if (req.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(req.error);
                Canvas.CloseButton.SetActive(true);
            }
            else
            {
                MediaAd ad = JsonUtility.FromJson<MediaAd>(req.downloadHandler.text);
                Canvas.ShowVideo(ad.url);
            }
        }
    }

    IEnumerator GetImageUrl() 
    {
        GameObject Ad = Instantiate(AdPrefab) as GameObject;
        AdCanvas Canvas = Ad.GetComponent<AdCanvas>();
        string key = PlayerPrefs.GetString("PlaymanityAPIKey");
        if (key.Length == 0)
        {
            Debug.LogError("Missing Playmanity api key");
            
        }

        using (UnityWebRequest req = UnityWebRequest.Get("https://api.playmanity.com/ads/watch?type=1"))
        {
            req.SetRequestHeader("Authorization", key);
            yield return req.SendWebRequest();
            if (req.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(req.error);
                Canvas.CloseButton.SetActive(true);
            }
            else
            {
                MediaAd ad = JsonUtility.FromJson<MediaAd>(req.downloadHandler.text);
                Canvas.ShowImage(ad.url);
            }
        }
    }
}
