using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils
{
    public static void GenerateRandomTextures(List<Texture2D> texs, LoopDumpTexture.TexRes res, int totalNumber)
    {
        int resolution = 2048;
        if (res == LoopDumpTexture.TexRes.Medium)
        {
            resolution = 1024;
        }
        else if (res == LoopDumpTexture.TexRes.Low)
        {
            resolution = 512;
        }

        for (int i = 0; i < totalNumber; i++)
        {
            Texture2D t = new Texture2D(resolution, resolution);

            /*for (int x = 0; x < t.width; x++)
            {
                for (int y = 0; y < t.height; y++)
                {
                    float r = Random.Range(0f, 1f);
                    float g = Random.Range(0f, 1f);
                    float b = Random.Range(0f, 1f);
                    t.SetPixel(x, y, new Color(r, g, b));
                }
            }*/
            float r = UnityEngine.Random.Range(0f, 1f);
            float g = UnityEngine.Random.Range(0f, 1f);
            float b = UnityEngine.Random.Range(0f, 1f);
            t.SetPixel(i % resolution, i / resolution, new Color(r, g, b));

            t.Apply();
            texs.Add(t);
        }
    }

    public static GameObject FindChildObjectWithName(GameObject parent, string childName)
    {
        Transform trans = FindChildTransformWithName(parent.transform, childName);
        return trans == null ? null : trans.gameObject;
    }

    public static Transform FindChildTransformWithName(Transform parent, string childName)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            Transform childTrans = parent.GetChild(i);
            if (childTrans.gameObject.name == childName)
            {
                return childTrans;
            }

            Transform ret = FindChildTransformWithName(childTrans, childName);
            if (ret != null)
            {
                return ret;
            }
        }
        return null;
    }

    public static string GetTimeStr()
    {
        return DateTime.Now.ToString("MM_dd_yyyy_HH_mm_ss");
    }

    public static long GetTimeStamp()
    {
        TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0);
        return (long)ts.TotalSeconds;
    }
}
