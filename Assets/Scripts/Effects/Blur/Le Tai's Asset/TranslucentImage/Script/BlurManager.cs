using System.Collections.Generic;

namespace LeTai.Asset.TranslucentImage
{
	public static class BlurManager
	{
		private static readonly Dictionary<BlurSource, List<BlurView>> _blurs =
			new Dictionary<BlurSource, List<BlurView>>();

		static BlurManager()
		{
			BlurSourcesAggregator.OnSourceRegistered += OnBlurSourceRegistered;
			BlurSourcesAggregator.OnSourceDeregistered += OnBlurSourceDeregistered;
		}

		public static void ConnectViewToSource(BlurView view, List<BlurSourceId> sourcesIds)
		{
			BlurSource source = FindSource(sourcesIds);

			if (source == null)
			{
				return;
			}

			if (!_blurs.ContainsKey(source))
			{
				_blurs.Add(source, new List<BlurView>());
			}

			_blurs[source].Add(view);

			view.TranslucentImage.SetSource(source.Source);

			source.Source.enabled = true;
		}

		public static void DisconnectViewFromSource(BlurView view)
		{
			BlurSource source = null;

			foreach (KeyValuePair<BlurSource, List<BlurView>> blur in _blurs)
			{
				if (blur.Value.Contains(view))
				{
					source = blur.Key;
					break;
				}
			}

			DisconnectViewFromSource(view, source);
		}

		private static void OnBlurSourceRegistered(BlurSource source)
		{
			TryToReconnectView(source);
		}

		private static void TryToReconnectView(BlurSource source)
		{
			foreach (KeyValuePair<BlurSource, List<BlurView>> blur in _blurs)
			{
				foreach (BlurView blurView in blur.Value)
				{
					foreach (BlurSourceId sourceId in blurView.SourcesIds)
					{
						if (source.Id == sourceId)
						{
							DisconnectViewFromSource(blurView, blur.Key);
							ConnectViewToSource(blurView, blurView.SourcesIds);

							return;
						}
					}
				}
			}
		}

		private static void OnBlurSourceDeregistered(BlurSource source)
		{
			if (_blurs.ContainsKey(source))
			{
				_blurs.Remove(source);
			}
		}

		private static void DisconnectViewFromSource(BlurView view, BlurSource source)
		{
			if (source == null)
			{
				return;
			}

			if (!_blurs.ContainsKey(source))
			{
				return;
			}

			_blurs[source].Remove(view);

			if (_blurs[source].Count == 0)
			{
				source.Source.enabled = false;
			}
		}

		private static BlurSource FindSource(List<BlurSourceId> sourcesIds)
		{
			BlurSource source = null;

			foreach (BlurSourceId sourceId in sourcesIds)
			{
				source = BlurSourcesAggregator.GetSourceById(sourceId);
				if (source != null)
				{
					break;
				}
			}

			return source;
		}
	}
}