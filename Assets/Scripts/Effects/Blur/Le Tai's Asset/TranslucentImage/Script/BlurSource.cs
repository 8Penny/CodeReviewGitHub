using System;
using LeTai.Asset.TranslucentImage;
using UnityEngine;

public class BlurSource : MonoBehaviour
{
	public static event Action<BlurSource> OnCameraEnabled;
	public static event Action<BlurSource> OnCameraDisabled;

	[SerializeField]
	private BlurSourceId _id;

	private TranslucentImageSource _source;
	private Camera _camera;
	private bool _cameraWasEnabled;

	public BlurSourceId Id => _id;
	public TranslucentImageSource Source => _source;
	public bool IsCameraEnabled => _camera.enabled;

	private void Awake()
	{
		_source = GetComponent<TranslucentImageSource>();
		_camera = GetComponent<Camera>();
		_cameraWasEnabled = _camera.enabled;

		if (_source == null)
		{
			Debug.LogError("Cant find TranslucentImageSource on " + gameObject.name);
		}
	}

	private void Update()
	{
		if (_cameraWasEnabled == _camera.enabled)
		{
			return;
		}

		_cameraWasEnabled = _camera.enabled;

		if (_camera.enabled)
		{
			OnCameraEnabled?.Invoke(this);
		}
		else
		{
			OnCameraDisabled?.Invoke(this);
		}
	}

	private void OnEnable()
	{
		if (_source != null)
		{
			BlurSourcesAggregator.Register(this);
		}
	}

	private void OnDisable()
	{
		if (_source != null)
		{
			BlurSourcesAggregator.Deregister(this);
		}
	}
}