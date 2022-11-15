using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Rendering;

public class PostprocessController : MonoBehaviour
{
    public bool IsTAAOn = true;
    public bool IsDOFOn = true;
    public bool IsMotionBlurOn = true;
    public bool IsBloomOn = true;
    public bool IsSSReflectionOn = true;
    public bool IsSSRefreactionOn = true;
    public bool IsSSGIOn = true;
    public bool IsSSAOOn = true;

    private HDAdditionalCameraData HDCData;
    private Volume volume;

    // Start is called before the first frame update
    void Start()
    {
        string setting = PlayerPrefs.GetString("param");
        if (setting != "")
        {
            if (setting == "-off taa")
            {
                IsTAAOn = false;
            }
            else if (setting == "-off dof")
            {
                IsDOFOn = false;
            }
            else if (setting == "-off motionblur")
            {
                IsMotionBlurOn = false;
            }
            else if (setting == "-off bloom")
            {
                IsBloomOn = false;
            }
            else if (setting == "-off ssrl")
            {
                IsSSReflectionOn = false;
            }
            else if (setting == "-off ssrr")
            {
                IsSSRefreactionOn = false;
            }
            else if (setting == "-off gi")
            {
                IsSSGIOn = false;
            }
            else if (setting == "-off ao")
            {
                IsSSAOOn = false;
            }
        }

        if (!IsTAAOn)
        {
            HDCData = gameObject.GetComponent<HDAdditionalCameraData>();
            HDCData.antialiasing = HDAdditionalCameraData.AntialiasingMode.None;
        }
        else if (!IsDOFOn)
        {
            volume = gameObject.GetComponent<Volume>();
            DepthOfField depthOfField;
            if (volume.profile.TryGet(out depthOfField))
            {
                depthOfField.active = false;
            }
        }
        else if (!IsMotionBlurOn)
        {
            volume = gameObject.GetComponent<Volume>();
            MotionBlur motionBlur;
            if (volume.profile.TryGet(out motionBlur))
            {
                motionBlur.active = false;
            }
        }
        else if (!IsBloomOn)
        {
            volume = gameObject.GetComponent<Volume>();
            Bloom bloom;
            if (volume.profile.TryGet(out bloom))
            {
                bloom.active = false;
            }
        }
        else if (!IsSSReflectionOn)
        {
            volume = gameObject.GetComponent<Volume>();
            ScreenSpaceReflection ssReflection;
            if (volume.profile.TryGet(out ssReflection))
            {
                ssReflection.active = false;
            }
        }
        else if (!IsSSRefreactionOn)
        {
            volume = gameObject.GetComponent<Volume>();
            ScreenSpaceRefraction ssRefraction;
            if (volume.profile.TryGet(out ssRefraction))
            {
                ssRefraction.active = false;
            }
        }
        else if (!IsSSGIOn)
        {
            volume = gameObject.GetComponent<Volume>();
            GlobalIllumination gi;
            if (volume.profile.TryGet(out gi))
            {
                gi.active = false;
            }
        }
        else if (!IsSSAOOn)
        {
            volume = gameObject.GetComponent<Volume>();
            AmbientOcclusion ai;
            if (volume.profile.TryGet(out ai))
            {
                ai.active = false;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
