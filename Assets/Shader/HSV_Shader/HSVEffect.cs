using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HSV
{
    [AddComponentMenu("Effects/HSV Adjust")]
    [RequireComponent(typeof(SpriteRenderer)), DisallowMultipleComponent, ExecuteInEditMode]
    public class HSVEffect : MonoBehaviour
    {
        public SpriteRenderer Renderer { get; private set; }
        //public Color GlowColor
        //{
        //    get { return glowColor; }
        //    set { if (glowColor != value) { glowColor = value; SetMaterialProperties(); } }
        //}
        //public float GlowBrightness
        //{
        //    get { return glowBrightness; }
        //    set { if (glowBrightness != value) { glowBrightness = value; SetMaterialProperties(); } }
        //}
        //public int OutlineWidth
        //{
        //    get { return outlineWidth; }
        //    set { if (outlineWidth != value) { outlineWidth = value; SetMaterialProperties(); } }
        //}
        //public float AlphaThreshold
        //{
        //    get { return alphaThreshold; }
        //    set { if (alphaThreshold != value) { alphaThreshold = value; SetMaterialProperties(); } }
        //}
        //public bool DrawOutside
        //{
        //    get { return drawOutside; }
        //    set { if (drawOutside != value) { drawOutside = value; SetMaterialProperties(); } }
        //}
        //public bool EnableInstancing
        //{
        //    get { return enableInstancing; }
        //    set { if (enableInstancing != value) { enableInstancing = value; SetMaterialProperties(); } }
        //}

        public float Hue
        {
            get { return hue; }
            set { if (hue != value) { hue = value; SetMaterialProperties(); } }
        }
        public float Sat
        {
            get { return sat; }
            set { if (sat != value) { sat = value; SetMaterialProperties(); } }
        }
        public float Val
        {
            get { return val; }
            set { if (val != value) { val = value; SetMaterialProperties(); } }
        }
        public bool EnableInstancing
        {
            get { return enableInstancing; }
            set { if (enableInstancing != value) { enableInstancing = value; SetMaterialProperties(); } }
        }

        [Range(-360, 360)]
        [SerializeField] private float hue = 0;
        [Range(0, 10)]
        [SerializeField] private float sat = 1;
        [Range(0, 10)]
        [SerializeField] private float val = 1;
        [SerializeField] private bool enableInstancing = true;

        //[Tooltip("Base color of the glow.")]
        //[SerializeField] private Color glowColor = Color.white;
        //[Tooltip("The brightness (power) of the glow."), Range(1, 10)]
        //[SerializeField] private float glowBrightness = 2f;
        //[Tooltip("Width of the outline, in texels."), Range(0, 10)]
        //[SerializeField] private int outlineWidth = 1;
        //[Tooltip("Threshold to determine sprite borders."), Range(0f, 1f)]
        //[SerializeField] private float alphaThreshold = .01f;
        //[Tooltip("Whether the outline should only be drawn outside of the sprite borders. Make sure sprite texture has sufficient transparent space for the required outline width.")]
        //[SerializeField] private bool drawOutside = false;
        //[Tooltip("Whether to enable GPU instancing.")]


        private static readonly int hueId = Shader.PropertyToID("_HueShift");
        private static readonly int saturationId = Shader.PropertyToID("_Sat");
        private static readonly int valId = Shader.PropertyToID("_Val");
        //private static readonly int alphaThresholdId = Shader.PropertyToID("_AlphaThreshold");

        private MaterialPropertyBlock materialProperties;

        private void Awake()
        {
            Renderer = GetComponent<SpriteRenderer>();
        }

        private void OnEnable()
        {
            SetMaterialProperties();
        }

        private void OnDisable()
        {
            SetMaterialProperties();
        }

        private void OnValidate()
        {
            if (!isActiveAndEnabled) return;

            // Update material properties when changing serialized fields with editor GUI.
            SetMaterialProperties();
        }

        private void OnDidApplyAnimationProperties()
        {
            // Update material properties when changing serialized fields with Unity animation.
            SetMaterialProperties();
        }

        private void SetMaterialProperties()
        {
            if (!Renderer) return;

            Renderer.sharedMaterial = HSVMaterial.GetSharedFor(this);

            if (materialProperties == null) // Initializing it at `Awake` or `OnEnable` causes nullref in editor in some cases.
                materialProperties = new MaterialPropertyBlock();

            //materialProperties.SetFloat(isOutlineEnabledId, isActiveAndEnabled ? 1 : 0);
            materialProperties.SetFloat(hueId, hue);
            materialProperties.SetFloat(saturationId, sat);
            materialProperties.SetFloat(valId, val);

            //materialProperties.SetColor(outlineColorId, GlowColor * GlowBrightness);
            //materialProperties.SetFloat(outlineSizeId, OutlineWidth);
            //materialProperties.SetFloat(alphaThresholdId, AlphaThreshold);

            Renderer.SetPropertyBlock(materialProperties);
        }


        private void Reset()
        {
            SetMaterialProperties();
        }
    }
}
