using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopDumpTexture : MonoBehaviour
{
    Material mat;
    int count;
    List<Material> mats = new List<Material>();
    MeshRenderer render;
    List<Texture2D> texs = new List<Texture2D>();

    public enum TexRes{
        High,
        Medium,
        Low,
    }
    public TexRes textureResolution = TexRes.High;
    public int totalCount = 512;

    // Start is called before the first frame update
    void Start()
    {
        render = gameObject.GetComponent<MeshRenderer>();
        render.GetSharedMaterials(mats);
        count = 0;
        Utils.GenerateRandomTextures(texs, textureResolution, totalCount);
    }

    // Update is called once per frame
    void Update()
    {
        mats[0].mainTexture = texs[count];
        count++;
        if (count >= totalCount)
        {
            count = 0;
        }
    }
}
