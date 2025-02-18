using System.Collections.Generic;
using UnityEngine;

namespace LeTai.Asset.TranslucentImage
{
	public class BlurView : MonoBehaviour
	{
		[SerializeField]
		private List<BlurSourceId> _sourcesIds = new List<BlurSourceId>();

		private TranslucentImage _translucentImage;

		public TranslucentImage TranslucentImage => _translucentImage;
		public List<BlurSourceId> SourcesIds => _sourcesIds;

		private void Awake()
		{
			_translucentImage = GetComponent<TranslucentImage>();
		}

		private void OnEnable()
		{
			BlurManager.ConnectViewToSource(this, _sourcesIds);

			BlurSourcesAggregator.OnSourceRegistered += OnBlurSourceRegistered;
			BlurSourcesAggregator.OnSourceDeregistered += OnBlurSourceDeregistered;
			BlurSource.OnCameraEnabled += OnBlurSourceCameraEnabled;
		}

		private void OnDisable()
		{
			BlurManager.DisconnectViewFromSource(this);

			BlurSourcesAggregator.OnSourceRegistered -= OnBlurSourceRegistered;
			BlurSourcesAggregator.OnSourceDeregistered -= OnBlurSourceDeregistered;
			BlurSource.OnCameraEnabled -= OnBlurSourceCameraEnabled;
		}

		private void OnBlurSourceCameraEnabled(BlurSource blurSource)
		{
			CheckSourceForValidity(blurSource);
		}

		private void OnBlurSourceRegistered(BlurSource blurSource)
		{
			CheckSourceForValidity(blurSource);
		}

		private void OnBlurSourceDeregistered(BlurSource blurSource)
		{
			CheckSourceForValidity(blurSource);
		}

		private void CheckSourceForValidity(BlurSource blurSource)
		{
			if (!_sourcesIds.Contains(blurSource.Id))
			{
				return;
			}

			BlurManager.DisconnectViewFromSource(this);
			BlurManager.ConnectViewToSource(this, _sourcesIds);
		}
	}
}