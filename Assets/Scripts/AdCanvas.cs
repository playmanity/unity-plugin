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
	public GameObject CloseButton;
	public RenderTexture VideoTexture;

	string Key;

	RawImage img;

	public void Start()
	{
		// Bg.gameObject.SetActive(true);
		AdImage.gameObject.SetActive(false);
		Player.gameObject.SetActive(false);
		CloseButton.SetActive(false);
		VideoTexture.Release();
	}

	public void ShowVideo(string url) 
	{
		Debug.Log($"Video: {url}");
		VideoTexture.Release();
		Player.url = url;

		Player.gameObject.SetActive(true);
		Player.Play();
		Player.loopPointReached += VideoEnd;
		Player.errorReceived += VideoEnd;

	}

	public void ShowImage(string url)
	{
		Debug.Log($"Image: {url}");
		StartCoroutine(DownloadImage(url));
	}

	void VideoEnd(VideoPlayer source, string message)
	{
		CloseButton.SetActive(true);
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
			CloseButton.SetActive(true);
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
