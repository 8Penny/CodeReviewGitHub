using UnityEngine;
using UnityEngine.UI;

namespace LeTai.Asset.TranslucentImage
{
    /// <summary>
    /// Dynamic blur-behind UI element
    /// </summary>
    [HelpURL("https://leloctai.com/asset/translucentimage/docs/articles/customize.html#translucent-image")]
    public partial class TranslucentImage : Image, IMeshModifier
    {
        static readonly int _vibrancyPropId = Shader.PropertyToID("_Vibrancy");
        static readonly int _brightnessPropId = Shader.PropertyToID("_Brightness");
        static readonly int _flattenPropId = Shader.PropertyToID("_Flatten");
        static readonly int _blurTexPropId = Shader.PropertyToID("_BlurTex");
        static readonly int _cropRegionPropId = Shader.PropertyToID("_CropRegion");

        /// <summary>
        /// Source of blur for this image
        /// </summary>
        public TranslucentImageSource source;

        /// <summary>
        /// (De)Saturate them image, 1 is normal, 0 is grey scale, below zero make the image negative
        /// </summary>
        [Tooltip("(De)Saturate them image, 1 is normal, 0 is black and white, below zero make the image negative")]
        [Range(-1, 3)]
        public float vibrancy = 1;

        /// <summary>
        /// Brighten/darken them image
        /// </summary>
        [Tooltip("Brighten/darken them image")] [Range(-1, 1)]
        public float brightness = 0;

        /// <summary>
        /// Flatten the color behind to help keep contrast on varying background
        /// </summary>
        [Tooltip("Flatten the color behind to help keep contrast on varying background")] [Range(0, 1)]
        public float flatten = .1f;

        private Shader _shader;

        private Material _replacedMaterial;
        private Material _cachedMaterial; // Update every frame
        private bool _shouldRun = true;

        private float _oldVibrancy;
        private float _oldBrightness;
        private float _oldFlatten;

        protected override void Awake()
        {
            base.Awake();

            CacheCorrectShader();
        }

        protected override void Start()
        {
            base.Start();

            _oldVibrancy = vibrancy;
            _oldBrightness = brightness;
            _oldFlatten = flatten;
        }

        private void Update()
        {
            _cachedMaterial = material;
            _shouldRun = Validate();

            if (!_shouldRun)
                return;

            if (_vibrancyPropId == 0 || _brightnessPropId == 0 || _flattenPropId == 0)
                return;

            _replacedMaterial = materialForRendering;

            bool isPropertySet = SyncMaterialProperty(_vibrancyPropId, ref vibrancy, ref _oldVibrancy);
            isPropertySet |= SyncMaterialProperty(_brightnessPropId, ref brightness, ref _oldBrightness);
            isPropertySet |= SyncMaterialProperty(_flattenPropId, ref flatten, ref _oldFlatten);

            if (isPropertySet)
            {
                SetMaterialDirty();
            }
        }

        private void LateUpdate()
        {
            if (!_shouldRun || source == null)
            {
                return;
            }

            if (_replacedMaterial)
            {
                _replacedMaterial.SetTexture(_blurTexPropId, source.BlurredScreen);
                _replacedMaterial.SetVector(_cropRegionPropId, source.BlurRegionNormalizedScreenSpace.ToMinMaxVector());
            }
            else
            {
                _cachedMaterial.SetTexture(_blurTexPropId, source.BlurredScreen);
                _cachedMaterial.SetVector(_cropRegionPropId, source.BlurRegionNormalizedScreenSpace.ToMinMaxVector());
            }

#if UNITY_EDITOR
            if (!Application.isPlaying && _replacedMaterial)
            {
                _cachedMaterial.SetTexture(_blurTexPropId, source.BlurredScreen);
                _cachedMaterial.SetVector(_cropRegionPropId, source.BlurRegionNormalizedScreenSpace.ToMinMaxVector());
            }
#endif
        }

        private void CacheCorrectShader()
        {
            _shader = Shader.Find("UI/TranslucentImage");
        }

        private bool Validate()
        {
            if (!IsActive() || source == null || !source.BlurredScreen || !_cachedMaterial)
                return false;

            if (!source)
            {
                if (Application.isPlaying)
                    Debug.LogWarning("TranslucentImageSource is missing. Please add the TranslucentImageSource component to your main camera, then assign it to the Source field of the Translucent Image(s)");
                return false;
            }

            if (_cachedMaterial.shader != _shader)
            {
                Debug.LogWarning($"Translucent Image requires a material using the \"UI/TranslucentImage\" shader. " +
                                 $"Current shader: {_cachedMaterial.shader.name} GO: {gameObject.name}");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Sync material property with instance
        /// </summary>
        /// <param name="propId">material property id</param>
        /// <param name="value"></param>
        /// <param name="oldValue"></param>
        private bool SyncMaterialProperty(int propId, ref float value, ref float oldValue)
        {
            bool isPropertySet = false;

            float matValue = _replacedMaterial.GetFloat(propId);

            if (Mathf.Abs(matValue - value) > 1e-4)
            {
                if (Mathf.Abs(value - oldValue) > 1e-4)
                {
                    if (_replacedMaterial)
                        _replacedMaterial.SetFloat(propId, value);

                    _cachedMaterial.SetFloat(propId, value);
                    isPropertySet = true;
                }
                else
                {
                    value = matValue;
                }
            }

            oldValue = value;

            return isPropertySet;
        }

        public void SetSource(TranslucentImageSource newSource)
        {
            source = newSource;

            _cachedMaterial = material;
            _cachedMaterial.SetTexture(_blurTexPropId, source.BlurredScreen);

            if (canvas)
                canvas.additionalShaderChannels |= AdditionalCanvasShaderChannels.TexCoord1;
        }
    }
}