using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HSV
{
    public class HSVMaterial : Material
    {
        public Texture SpriteTexture { get { return mainTexture; } }
        //public bool DrawOutside { get { return IsKeywordEnabled(outsideMaterialKeyword); } }
        public bool InstancingEnabled { get { return enableInstancing; } }

        private const string hsvShaderName = "Custom/HSVShader";
        //private const string outsideMaterialKeyword = "SPRITE_OUTLINE_OUTSIDE";
        private static readonly Shader outlineShader = Shader.Find(hsvShaderName);

        private static List<HSVMaterial> sharedMaterials = new List<HSVMaterial>();

        public HSVMaterial(Texture spriteTexture, bool instancingEnabled = false)
            : base(outlineShader)
        {
            if (!outlineShader) Debug.LogError(string.Format("{0} shader not found. Make sure the shader is included to the build.", hsvShaderName));

            mainTexture = spriteTexture;
            //if (drawOutside) EnableKeyword(outsideMaterialKeyword);
            if (instancingEnabled) enableInstancing = true;
        }

        public static Material GetSharedFor(HSVEffect hsvEffect)
        {
            for (int i = 0; i < sharedMaterials.Count; i++)
            {
                if (sharedMaterials[i].SpriteTexture == hsvEffect.Renderer.sprite.texture &&
                    //sharedMaterials[i].DrawOutside == spriteGlow.DrawOutside &&
                    sharedMaterials[i].InstancingEnabled == hsvEffect.EnableInstancing)
                    return sharedMaterials[i];
            }

            var material = new HSVMaterial(hsvEffect.Renderer.sprite.texture, hsvEffect.EnableInstancing);
            material.hideFlags = HideFlags.DontSaveInBuild | HideFlags.DontSaveInEditor | HideFlags.NotEditable;
            sharedMaterials.Add(material);

            return material;
        }
    }
}
