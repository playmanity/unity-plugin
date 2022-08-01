using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class AdHandler : MonoBehaviour
{

    public Object AdPrefab;
    public AdCanvas Canvas;

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
        if (Canvas) 
        {
            Destroy(Canvas.gameObject);
        }
        GameObject Ad = Instantiate(AdPrefab) as GameObject;
        Canvas = Ad.GetComponent<AdCanvas>();
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
                Canvas.TryAgainButton.SetActive(true);
                Canvas.TryAgainButton.GetComponent<Button>().onClick.AddListener(ShowVideoAd);
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
        if (Canvas)
        {
            Destroy(Canvas.gameObject);
        }
        GameObject Ad = Instantiate(AdPrefab) as GameObject;
        Canvas = Ad.GetComponent<AdCanvas>();
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
                Canvas.TryAgainButton.SetActive(true);
                Canvas.TryAgainButton.GetComponent<Button>().onClick.AddListener(ShowImageAd);
            }
            else
            {
                MediaAd ad = JsonUtility.FromJson<MediaAd>(req.downloadHandler.text);
                Canvas.ShowImage(ad.url);
            }
        }
    }
}
