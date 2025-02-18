using UnityEngine;

namespace Views
{
    public class CastleResourceView : MonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer _renderer;

        public void SetSprite(Sprite s)
        {
            _renderer.sprite = s;
        }
    }
}