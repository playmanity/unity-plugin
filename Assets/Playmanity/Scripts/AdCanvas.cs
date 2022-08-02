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
	public string Key;
	string Id;
	string Game;
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

	public void ShowVideo(string game, string id, string url)
	{
		// Debug.Log($"Video: {url}");
		Id = id;
		Game = game;
		TryAgainButton.SetActive(false);
		VideoTexture.Release();
		Player.url = url;
		Player.gameObject.SetActive(true);
		Player.Play();
		Player.loopPointReached += VideoEnd;
		Player.errorReceived += VideoError;

	}

	public void ShowImage(string game, string id, string url)
	{
		// Debug.Log($"Image: {url}");
		Id = id;
		Game = game;
		TryAgainButton.SetActive(false);
		StartCoroutine(DownloadImage(url));
	}

	void VideoError(VideoPlayer source, string message)
	{
		TryAgainButton.SetActive(true);
		TryAgainButton.GetComponent<Button>().onClick.RemoveAllListeners();
		TryAgainButton.GetComponent<Button>().onClick.AddListener(delegate { ShowVideo(Game, Id, source.url); });
	}

	void VideoEnd(UnityEngine.Video.VideoPlayer vp)
	{
		StartCoroutine(Report());
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
			TryAgainButton.GetComponent<Button>().onClick.RemoveAllListeners();
			TryAgainButton.GetComponent<Button>().onClick.AddListener(delegate { ShowImage(Game, Id, MediaUrl); });
		}
		else
		{
			AdImage.texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
			AdImage.gameObject.SetActive(true);
			yield return new WaitForSeconds(5f);
			StartCoroutine(Report());
			CloseButton.SetActive(true);
		}
	}

	IEnumerator Report()
	{
		using (UnityWebRequest req = UnityWebRequest.Post($"https://api.playmanity.com/ads/{Id}/report?game={Game}", ""))
		{
			req.SetRequestHeader("Authorization", Key);
			while (true)
			{
				yield return req.SendWebRequest();
				if (req.result != UnityWebRequest.Result.Success)
				{
					Debug.LogError(req.error);
					yield return new WaitForSeconds(1f);
				}
				else
				{
					break;
				}
			}
		}
	}

	public void Dissmiss ()
	{
		Destroy(gameObject);
	}

}
