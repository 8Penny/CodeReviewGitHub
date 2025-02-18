using System;
using System.Collections.Generic;
using System.Linq;

namespace LeTai.Asset.TranslucentImage
{
	public static class BlurSourcesAggregator
	{
		public static event Action<BlurSource> OnSourceRegistered;
		public static event Action<BlurSource> OnSourceDeregistered;

		private static readonly Dictionary<BlurSourceId, List<BlurSource>> _sources =
			new Dictionary<BlurSourceId, List<BlurSource>>();

		public static void Register(BlurSource source)
		{
			if (!_sources.ContainsKey(source.Id))
			{
				_sources.Add(source.Id, new List<BlurSource>());
			}

			_sources[source.Id].Add(source);

			source.Source.enabled = false;

			OnSourceRegistered?.Invoke(source);
		}

		public static void Deregister(BlurSource source)
		{
			if (!_sources.ContainsKey(source.Id))
			{
				return;
			}

			_sources[source.Id].Remove(source);

			source.Source.enabled = false;

			OnSourceDeregistered?.Invoke(source);
		}

		public static BlurSource GetSourceById(BlurSourceId id)
		{
			return _sources.ContainsKey(id) ? _sources[id].FirstOrDefault(source => source.IsCameraEnabled) : null;
		}
	}
}