﻿using BepInEx.Logging;
using HarmonyLib;
using Studio;
using UnityEngine;
using UnityEngine.UI;

namespace KeelPlugins
{
    internal class SceneParam
    {
        private static ParamData.SceneData _sceneData;

        public static void Init(ParamData.SceneData data)
        {
            _sceneData = data;
        }

        public static void Save()
        {
            var sceneInfo = Studio.Studio.Instance.sceneInfo;
            var systemButtonCtrl = Studio.Studio.Instance.systemButtonCtrl;

            if(sceneInfo != null && systemButtonCtrl != null)
            {
                _sceneData.aceNo = sceneInfo.aceNo;
                _sceneData.aceBlend = sceneInfo.aceBlend;

                var aoe = Traverse.Create(systemButtonCtrl).Field("amplifyOcculusionEffectInfo").Property("aoe").GetValue<AmplifyOcclusionEffect>();
                _sceneData.enableAOE = (bool)aoe.GetType().GetProperty("enabled").GetValue(aoe, null);

                _sceneData.aoeColor = sceneInfo.aoeColor;
                _sceneData.aoeRadius = sceneInfo.aoeRadius;
                _sceneData.enableBloom = sceneInfo.enableBloom;
                _sceneData.bloomIntensity = sceneInfo.bloomIntensity;
                _sceneData.bloomThreshold = sceneInfo.bloomThreshold;
                _sceneData.bloomBlur = sceneInfo.bloomBlur;
                _sceneData.enableDepth = sceneInfo.enableDepth;
                _sceneData.depthFocalSize = sceneInfo.depthFocalSize;
                _sceneData.depthAperture = sceneInfo.depthAperture;
                _sceneData.enableVignette = sceneInfo.enableVignette;
                _sceneData.enableFog = sceneInfo.enableFog;
                _sceneData.fogColor = sceneInfo.fogColor;
                _sceneData.fogHeight = sceneInfo.fogHeight;
                _sceneData.fogStartDistance = sceneInfo.fogStartDistance;
                _sceneData.enableSunShafts = sceneInfo.enableSunShafts;
                _sceneData.sunThresholdColor = sceneInfo.sunThresholdColor;
                _sceneData.sunColor = sceneInfo.sunColor;

                var toggleEnable = Traverse.Create(systemButtonCtrl).Field("selfShadowInfo").Field("toggleEnable").GetValue<Toggle>();
                _sceneData.enableShadow = (bool)toggleEnable.GetType().GetProperty("isOn").GetValue(toggleEnable, null);

                _sceneData.rampG = sceneInfo.rampG;
                _sceneData.ambientShadowG = sceneInfo.ambientShadowG;
                _sceneData.lineWidthG = sceneInfo.lineWidthG;
                _sceneData.lineColorG = sceneInfo.lineColorG;
                _sceneData.ambientShadow = sceneInfo.ambientShadow;
                _sceneData.cameraNearClip = Camera.main.nearClipPlane;
                _sceneData.fov = Studio.Studio.Instance.cameraCtrl.fieldOfView;

                _sceneData.saved = true;
                DefaultParamEditor.Logger.Log(LogLevel.Message, "Default scene settings saved");
            }
        }

        public static void Reset()
        {
            _sceneData.saved = false;
        }

        public static void LoadDefaults()
        {
            if(_sceneData.saved && Studio.Studio.Instance)
            {
                DefaultParamEditor.Logger.Log(LogLevel.Info, "Loading scene defaults");
                SetSceneInfoValues(Studio.Studio.Instance.sceneInfo);
                Studio.Studio.Instance.systemButtonCtrl.UpdateInfo();
            }
        }

        private static void SetSceneInfoValues(SceneInfo sceneInfo)
        {
            sceneInfo.aceNo = _sceneData.aceNo;
            sceneInfo.aceBlend = _sceneData.aceBlend;
            sceneInfo.enableAOE = _sceneData.enableAOE;
            sceneInfo.aoeColor = _sceneData.aoeColor;
            sceneInfo.aoeRadius = _sceneData.aoeRadius;
            sceneInfo.enableBloom = _sceneData.enableBloom;
            sceneInfo.bloomIntensity = _sceneData.bloomIntensity;
            sceneInfo.bloomThreshold = _sceneData.bloomThreshold;
            sceneInfo.bloomBlur = _sceneData.bloomBlur;
            sceneInfo.enableDepth = _sceneData.enableDepth;
            sceneInfo.depthFocalSize = _sceneData.depthFocalSize;
            sceneInfo.depthAperture = _sceneData.depthAperture;
            sceneInfo.enableVignette = _sceneData.enableVignette;
            sceneInfo.enableFog = _sceneData.enableFog;
            sceneInfo.fogColor = _sceneData.fogColor;
            sceneInfo.fogHeight = _sceneData.fogHeight;
            sceneInfo.fogStartDistance = _sceneData.fogStartDistance;
            sceneInfo.enableSunShafts = _sceneData.enableSunShafts;
            sceneInfo.sunThresholdColor = _sceneData.sunThresholdColor;
            sceneInfo.sunColor = _sceneData.sunColor;
            sceneInfo.enableShadow = _sceneData.enableShadow;
            sceneInfo.rampG = _sceneData.rampG;
            sceneInfo.ambientShadowG = _sceneData.ambientShadowG;
            sceneInfo.lineWidthG = _sceneData.lineWidthG;
            sceneInfo.lineColorG = _sceneData.lineColorG;
            sceneInfo.ambientShadow = _sceneData.ambientShadow;
        }

        private class Hooks
        {
            [HarmonyPostfix, HarmonyPatch(typeof(SceneInfo), nameof(SceneInfo.Init))]
            public static void HarmonyPatch_SceneInfo_Init(SceneInfo __instance)
            {
                if(_sceneData.saved)
                {
                    DefaultParamEditor.Logger.Log(LogLevel.Debug, "Loading defaults for a new scene");
                    SetSceneInfoValues(__instance);
                    Camera.main.nearClipPlane = _sceneData.cameraNearClip;
                    Studio.Studio.Instance.cameraCtrl.fieldOfView = _sceneData.fov;
                }
            }
        }
    }
}
