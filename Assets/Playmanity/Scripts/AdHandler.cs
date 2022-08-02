using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class AdHandler : MonoBehaviour
{

    public string gameIdentifier;
    Object AdPrefab;
    AdCanvas Canvas;

	private void Start()
	{
        AdPrefab = Resources.Load<GameObject>("Playmanity Ad Canvas");
	}

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
        Canvas.Key = key;
        if (key.Length == 0)
        {
            Debug.LogError("Missing Playmanity api key");
        }

        using (UnityWebRequest req = UnityWebRequest.Get($"https://api.playmanity.com/ads/watch?type=2&game={gameIdentifier}"))
        {
            req.SetRequestHeader("Authorization", key);
            yield return req.SendWebRequest();
            if (req.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(req.error);
                Canvas.TryAgainButton.SetActive(true);
                Canvas.TryAgainButton.GetComponent<Button>().onClick.RemoveAllListeners();
                Canvas.TryAgainButton.GetComponent<Button>().onClick.AddListener(ShowVideoAd);
            }
            else
            {
                MediaAd ad = JsonUtility.FromJson<MediaAd>(req.downloadHandler.text);
                Canvas.ShowVideo(gameIdentifier, ad.id, ad.url);
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
        Canvas.Key = key;
        if (key.Length == 0)
        {
            Debug.LogError("Missing Playmanity api key");
        }

        using (UnityWebRequest req = UnityWebRequest.Get($"https://api.playmanity.com/ads/watch?type=1&game={gameIdentifier}"))
        {
            req.SetRequestHeader("Authorization", key);
            yield return req.SendWebRequest();
            if (req.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(req.error);
                Canvas.TryAgainButton.SetActive(true);
                Canvas.TryAgainButton.GetComponent<Button>().onClick.RemoveAllListeners();
                Canvas.TryAgainButton.GetComponent<Button>().onClick.AddListener(ShowImageAd);
            }
            else
            {
                MediaAd ad = JsonUtility.FromJson<MediaAd>(req.downloadHandler.text);
                Canvas.ShowImage(gameIdentifier, ad.id, ad.url);
            }
        }
    }
}



