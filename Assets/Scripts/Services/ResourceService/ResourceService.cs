using System.Collections.Generic;
using Static;
using UIBasics.Views.ResourcesPanel;
using UnityEngine;

namespace Services.ResourceService
{
    public class ResourceService
    {
        private string PREFIX = "Sprites/";
        private Dictionary<string, Sprite> _sprites = new Dictionary<string, Sprite>();
        
        public Sprite GetSprite(ResourceType t)
        {
            if (t == ResourceType.None)
            {
                return null;
            }
            string path = PREFIX + t.ToString();
            return GetSpriteAtPath(t.ToString(), path);
        }
        
        private Sprite GetSpriteAtPath(string name, string path)
        {
            if (string.IsNullOrEmpty(name))
            {
                Debug.LogError("Sprite name is Empty. Return null");
                return null;
            }

            if (!_sprites.ContainsKey(name))
            {
                Sprite sprite = Resources.Load<Sprite>(path);

                if (sprite == null)
                {
                    Debug.LogErrorFormat("Can't load sprite of {0} by path: {1}", name, path);
                    return null;
                }
                _sprites.Add(name, sprite);
            }

            return _sprites[name];
        }
    }
}