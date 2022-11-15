using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class EditorUtils
{
    [MenuItem("Tools/Generator Random Textures")]
    static void GenerateRandomTextures()
    {
        if (!File.Exists(Application.dataPath + "/Resources/DummyTextures/High/DummyTex0.png"))
        {
            for (int i = 0; i < 512; i++)
            {
                Texture2D t = new Texture2D(2048, 2048);

                for (int x = 0; x < t.width; x++)
                {
                    for (int y = 0; y < t.height; y++)
                    {
                        float r = Random.Range(0f, 1f);
                        float g = Random.Range(0f, 1f);
                        float b = Random.Range(0f, 1f);
                        t.SetPixel(x, y, new Color(r, g, b));
                    }
                }
                byte[] bytes = t.EncodeToPNG();
                File.WriteAllBytes(Application.dataPath + "/Resources/DummyTextures/High/DummyTex" + i.ToString() + ".png", bytes);
                EditorUtility.DisplayProgressBar("Random high res textures.", "Making " + i.ToString() + " of 512", (float)(i + 1) / 512);
            }
        }

        if (!File.Exists(Application.dataPath + "/Resources/DummyTextures/Medium/DummyTex0.png"))
        {
            for (int i = 0; i < 512; i++)
            {
                Texture2D t = new Texture2D(1024, 1024);

                for (int x = 0; x < t.width; x++)
                {
                    for (int y = 0; y < t.height; y++)
                    {
                        float r = Random.Range(0f, 1f);
                        float g = Random.Range(0f, 1f);
                        float b = Random.Range(0f, 1f);
                        t.SetPixel(x, y, new Color(r, g, b));
                    }
                }
                byte[] bytes = t.EncodeToPNG();
                File.WriteAllBytes(Application.dataPath + "/Resources/DummyTextures/Medium/DummyTex" + i.ToString() + ".png", bytes);
                EditorUtility.DisplayProgressBar("Random medium res textures.", "Making " + i.ToString() + " of 512", (float)(i + 1) / 512);
            }
        }

        if (!File.Exists(Application.dataPath + "/Resources/DummyTextures/Low/DummyTex0.png"))
        {
            for (int i = 0; i < 512; i++)
            {
                Texture2D t = new Texture2D(512, 512);

                for (int x = 0; x < t.width; x++)
                {
                    for (int y = 0; y < t.height; y++)
                    {
                        float r = Random.Range(0f, 1f);
                        float g = Random.Range(0f, 1f);
                        float b = Random.Range(0f, 1f);
                        t.SetPixel(x, y, new Color(r, g, b));
                    }
                }
                byte[] bytes = t.EncodeToPNG();
                File.WriteAllBytes(Application.dataPath + "/Resources/DummyTextures/Low/DummyTex" + i.ToString() + ".png", bytes);
                EditorUtility.DisplayProgressBar("Random low res textures.", "Making " + i.ToString() + " of 512", (float)(i + 1) / 512);
            }
        }
        EditorUtility.ClearProgressBar();
        AssetDatabase.Refresh();
    }
}
