using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Video;

public class AdCanvas : MonoBehaviour
{
    public RawImage AdImage;
    public VideoPlayer Player;
	public GameObject CloseButton, TryAgainButton;
	public RenderTexture VideoTexture;

	string Key;

	RawImage img;

	public void Start()
	{
		// Bg.gameObject.SetActive(true);
		AdImage.gameObject.SetActive(false);
		Player.gameObject.SetActive(false);
		CloseButton.SetActive(false);
		TryAgainButton.SetActive(false);
		VideoTexture.Release();
	}

	public void ShowVideo(string url) 
	{
		// Debug.Log($"Video: {url}");
		TryAgainButton.SetActive(false);
		VideoTexture.Release();
		Player.url = url;
		Player.gameObject.SetActive(true);
		Player.Play();
		Player.loopPointReached += VideoEnd;
		Player.errorReceived += VideoError;

	}

	public void ShowImage(string url)
	{
		Debug.Log($"Image: {url}");
		TryAgainButton.SetActive(false);
		StartCoroutine(DownloadImage(url));
	}

	void VideoError(VideoPlayer source, string message) 
	{
		TryAgainButton.SetActive(true);
		TryAgainButton.GetComponent<Button>().onClick.AddListener(delegate { ShowImage(source.url); });
	}

	void VideoEnd(UnityEngine.Video.VideoPlayer vp)
	{
		CloseButton.SetActive(true);
	}


	IEnumerator DownloadImage(string MediaUrl)
	{
		UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl);
		yield return request.SendWebRequest();
		if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
		{
			Debug.Log(request.error);
			TryAgainButton.SetActive(true);
			TryAgainButton.GetComponent<Button>().onClick.AddListener(delegate { ShowImage(MediaUrl); });
		}
		else 
		{
			AdImage.texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
			AdImage.gameObject.SetActive(true);
			yield return new WaitForSeconds(5f);
			CloseButton.SetActive(true);
		}
	}

	public void Dissmiss ()
	{
		Destroy(gameObject);
	}

}
