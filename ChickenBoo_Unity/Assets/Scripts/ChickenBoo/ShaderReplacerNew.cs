#nullable enable
using System;
using System.Collections.Generic;
using UnityEngine;
#nullable enable
[Serializable]
enum ShaderType
{
    Alpha,
    Blob,
    Bonemass,
    Clouds,
    Creature,
    Decal,
    Distortion,
    Flow,
    FlowOpaque,
    Grass,
    GuiScroll,
    HeightMap,
    Icon,
    InteriorSide,
    LitGui,
    LitParticles,
    MapShader,
    ParticleDecal,
    Piece,
    Player,
    Rug,
    ShadowBlob,
    SkyboxProcedural,
    SkyObject,
    StaticRock,
    Tar,
    TrilinearMap,
    BGBlur,
    Vegetation,
    Water,
    WaterBottom,
    WaterMask,
    Yggdrasil,
    YggdrasilRoot,
    ToonDeferredShading2017
}

public class ShaderReplacerNew : MonoBehaviour
{
    [Tooltip("Use this Field For Normal Renderers")] 
    [SerializeField] internal List<Renderer> _renderers = new List<Renderer>();
    [SerializeField] internal ShaderType _shaderType = ShaderType.Creature;
    [Tooltip("Use this to debug what gameobject is getting its shader replaced in case there is errors. TimedDestruction lower than 4 can cause the script to fail to run")]
    [SerializeField] internal bool DebugOutput = false;
    private void Awake()
    {
        if (IsHeadlessMode()) return;
        if(_renderers.Count <=0) return;
        if(!this.gameObject.activeInHierarchy)return;
        foreach (var renderer in _renderers)
        {
            if(renderer == null) continue;
            foreach (var material in renderer.sharedMaterials)
            {
                if (material == null)
                {
                    renderer.gameObject.SetActive(false);
                    continue;
                }
                
                material.shader = Shader.Find(ReturnEnumString(_shaderType));
            }
        }
    }

    internal string ReturnEnumString(ShaderType shaderchoice)
    {
        var s="";
        switch (shaderchoice)
        {
            case ShaderType.Alpha:
                s = "Custom/AlphaParticle";
                break;
            case ShaderType.Blob:
                s = "Custom/Blob";
                break;
            case ShaderType.Bonemass:
                s = "Custom/Bonemass";
                break;
            case ShaderType.Clouds:
                s = "Custom/Clouds";
                break;
            case ShaderType.Creature:
                s = "Custom/Creature";
                break;
            case ShaderType.Decal:
                s = "Custom/Decal";
                break;
            case ShaderType.Distortion:
                s = "Custom/Distortion";
                break;
            case ShaderType.Flow:
                s = "Custom/Flow";
                break;
            case ShaderType.FlowOpaque:
                s = "Custom/FlowOpaque";
                break;
            case ShaderType.Grass:
                s = "Custom/Grass";
                break;
            case ShaderType.GuiScroll:
                s = "Custom/GuiScroll";
                break;
            case ShaderType.HeightMap:
                s = "Custom/HeightMap";
                break;
            case ShaderType.Icon:
                s = "Custom/Icon";
                break;
            case ShaderType.InteriorSide:
                s = "Custom/InteriorSide";
                break;
            case ShaderType.LitGui:
                s = "Custom/LitGui";
                break;
            case ShaderType.LitParticles:
                s = "Lux Lit Particles/ Bumped";
                break;
            case ShaderType.MapShader:
                s = "Custom/mapshader";
                break;
            case ShaderType.ParticleDecal:
                s = "Custom/ParticleDecal";
                break;
            case ShaderType.Piece:
                s = "Custom/Piece";
                break;
            case ShaderType.Player:
                s = "Custom/Player";
                break;
            case ShaderType.Rug:
                s = "Custom/Rug";
                break;
            case ShaderType.ShadowBlob:
                s = "Custom/ShadowBlob";
                break;
            case ShaderType.SkyboxProcedural:
                s = "Custom/SkyboxProcedural";
                break;
            case ShaderType.SkyObject:
                s = "Custom/SkyObject";
                break;
            case ShaderType.StaticRock:
                s = "Custom/StaticRock";
                break;
            case ShaderType.Tar:
                s = "Custom/Tar";
                break;
            case ShaderType.TrilinearMap:
                s = "Custom/Trilinearmap";
                break;
            case ShaderType.BGBlur:
                s = "Custom/UI_BGBlur";
                break;
            case ShaderType.Water:
                s = "Custom/Water";
                break;
            case ShaderType.Vegetation:
                s = "Custom/Vegetation";
                break;
            case ShaderType.WaterBottom:
                s = "Custom/WaterBottom";
                break;
            case ShaderType.WaterMask:
                s = "Custom/WaterMask";
                break;
            case ShaderType.Yggdrasil:
                s = "Custom/Yggdrasil";
                break;
            case ShaderType.YggdrasilRoot:
                s = "Custom/Yggdrasil_root";
                break;
            case ShaderType.ToonDeferredShading2017:
                s = "ToonDeferredShading2017";
                break;
        }
        return s;
    }
    
    public static bool IsHeadlessMode()
    {
        return UnityEngine.SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null;
    }
}