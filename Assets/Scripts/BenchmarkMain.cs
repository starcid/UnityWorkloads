using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;
using Unity.Collections;
using LitJson;

[Serializable]
public class SceneInfo
{
    public string name;
    public string category;
    public string paramaters;
    public string comment;
};

public class SceneBench
{
    public SceneBench(SceneInfo i)
    {
        info = i;
        frameTimes = new List<float>();
        startTime = DateTime.MinValue;
    }
    public SceneInfo info;

    public float totalFps;
    public DateTime startTime;
    public List<float> frameTimes;
};

public class BenchmarkMain : MonoBehaviour
{
    List<SceneBench> sceneLists = new List<SceneBench>();
    GameObject startUI;
    GameObject benchUI;
    GameObject benchFpsUI;
    TextMeshProUGUI benchFps;
    GameObject intervalUI;
    GameObject intervalIntroUI;
    TextMeshProUGUI intervalIntro;
    GameObject intervalFpsUI;
    TextMeshProUGUI intervalFps;
    float benchTime;
    float intervalTime;
    bool uploadFinished;

    enum State
    { 
        Setting,
        Interval,
        Benchmark,
        Uploading,
    };
    State state;
    float timeCount;
    int sceneCount;
    int benchIndex;

    float totalFrameTime;
    int totalFrame;

    float fpsStartCountWaitTime;

    int displayFpsFrame;
    float displayFps;
    float displayFpsTimeCount;
    float displayFpsUpdateInterval;

    bool isLightSettingScene;
    bool haveSetMode;

    FrameSettings frameSettings;
    FrameSettingsOverrideMask frameSettingsOverrideMask;
    HDAdditionalCameraData HDCData;

    // Start is called before the first frame update
    void Start()
    {
        state = State.Setting;
        uploadFinished = false;
        sceneCount = -1;
        displayFpsUpdateInterval = 2.0f;
        fpsStartCountWaitTime = 5.0f;
        isLightSettingScene = false;
        haveSetMode = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (state == State.Interval)
        {
            displayFpsFrame++;
            displayFpsTimeCount += Time.deltaTime;
            if (displayFpsTimeCount >= displayFpsUpdateInterval)
            {
                displayFps = (float)displayFpsFrame / displayFpsTimeCount;
                intervalFps.text = "fps:" + displayFps.ToString("f2");
                displayFpsTimeCount = 0.0f;
                displayFpsFrame = 0;
            }

            timeCount += Time.deltaTime;
            if (timeCount >= intervalTime)
            {
                timeCount = 0.0f;
                totalFrameTime = 0.0f;
                totalFrame = 0;
                state = State.Benchmark;

                Application.targetFrameRate = 500;

                displayFpsFrame = 0;
                displayFpsTimeCount = 0.0f;

                SceneInfo info = sceneLists[sceneCount].info;
                PlayerPrefs.SetString("param", info.paramaters);
                SceneManager.LoadScene("Resources/Scenes/" + info.category + "/" + info.name);

                startUI.SetActive(false);
                benchUI.SetActive(true);
                intervalUI.SetActive(false);

                isLightSettingScene = (info.category != "postprocess_test") ? true : false;
                haveSetMode = false;
            }
        }
        else if (state == State.Benchmark)
        {
            if (!haveSetMode && Camera.main != null)
            {
                SetLightMode(isLightSettingScene, Camera.main.gameObject);
                haveSetMode = true;
            }

            displayFpsFrame++;
            displayFpsTimeCount += Time.deltaTime;
            if (displayFpsTimeCount >= displayFpsUpdateInterval)
            {
                displayFps = (float)displayFpsFrame / displayFpsTimeCount;
                benchFps.text = "fps:" + displayFps.ToString("f2");
                displayFpsTimeCount = 0.0f;
                displayFpsFrame = 0;
            }

            // delay 4 sec to start count
            timeCount += Time.deltaTime;
            if (timeCount > fpsStartCountWaitTime)
            {
                totalFrame++;
                totalFrameTime += Time.deltaTime;
                sceneLists[sceneCount].frameTimes.Add(Time.deltaTime * 1000.0f);
                if (sceneLists[sceneCount].startTime == DateTime.MinValue)
                {
                    sceneLists[sceneCount].startTime = DateTime.Now;
                }
                if (timeCount >= benchTime)
                {
                    sceneLists[sceneCount].totalFps = (float)totalFrame / totalFrameTime;
                    Next((benchIndex != -1));
                }
            }
        }
        else if (state == State.Uploading)
        {
            if (uploadFinished)
            {
                state = State.Setting;

                startUI.SetActive(true);
                benchUI.SetActive(false);
                intervalUI.SetActive(false);

                // back to original
                timeCount = 0.0f;
                displayFpsTimeCount = 0.0f;
                SceneManager.LoadScene("Resources/Scenes/start");

                // destroy self
                GameObject.Destroy(startUI.transform.parent.gameObject);
                GameObject.Destroy(gameObject);
            }
        }
    }

    public void Initialize(ref SceneInfo[] infos, GameObject startui, GameObject benchui, GameObject intervalui)
    {
        sceneLists.Clear();
        foreach(SceneInfo info in infos)
        {
            sceneLists.Add(new SceneBench(info));
        }

        startUI = startui;
        benchUI = benchui;
        benchFpsUI = Utils.FindChildObjectWithName(benchUI, "FPS");
        benchFps = benchFpsUI.GetComponent<TextMeshProUGUI>();
        intervalUI = intervalui;
        intervalFpsUI = Utils.FindChildObjectWithName(intervalui, "FPS");
        intervalFps = intervalFpsUI.GetComponent<TextMeshProUGUI>();
        intervalIntroUI = Utils.FindChildObjectWithName(intervalui, "Intro");
        intervalIntro = intervalIntroUI.GetComponent<TextMeshProUGUI>();

        startUI.SetActive(true);
        benchUI.SetActive(false);
        intervalUI.SetActive(false);
    }

    public void Begin(int benchIdx, float benchtime, float intervaltime, bool showfps)
    {
        benchTime = benchtime;
        intervalTime = intervaltime;
        benchFpsUI.SetActive(showfps);
        benchIndex = benchIdx;
        if (benchIdx == -1)
        {
            sceneCount = -1;
        }
        else
        {
            sceneCount = benchIdx - 1;
        }

        Next();
    }

    public void Next(bool oneScene = false)
    {
        sceneCount++;

        Application.targetFrameRate = 30;
        if (oneScene || sceneCount >= sceneLists.Count)
        {
            sceneCount = 0;
            End();
            return;
        }

        state = State.Interval;
        startUI.SetActive(false);
        benchUI.SetActive(false);
        intervalUI.SetActive(true);

        timeCount = 0.0f;
        displayFpsTimeCount = 0.0f;
        SceneManager.LoadScene("Resources/Scenes/interval");

        SceneInfo info = sceneLists[sceneCount].info;
        intervalIntro.text = info.name + "\n" + info.category + "\n" + info.comment;
    }

    public void End()
    {
        // recording
        SaveCSV("unity_bench_" + UnityEngine.SystemInfo.graphicsDeviceName + "_" + Utils.GetTimeStr());

        // uploading result
        UploadTestResult("unity_bench_" + Utils.GetTimeStr(), SystemInfo.processorType, SystemInfo.graphicsDeviceName, Application.version, Application.unityVersion, DateTime.Now, sceneLists);
        
        state = State.Uploading;
    }

    void SetLightMode( bool isLightMode, GameObject camObject )
    {
        // clear taa motion blur bloom ao ssr dof
        // taa
        HDCData = camObject.GetComponent<HDAdditionalCameraData>();
        if (HDCData != null)
        {
            HDCData.customRenderingSettings = true;

            frameSettings = HDCData.renderingPathCustomFrameSettings;
            frameSettingsOverrideMask = HDCData.renderingPathCustomFrameSettingsOverrideMask;
            
            frameSettingsOverrideMask.mask[(uint)FrameSettingsField.ContactShadows] = true;
            frameSettingsOverrideMask.mask[(uint)FrameSettingsField.MotionVectors] = true;
            frameSettingsOverrideMask.mask[(uint)FrameSettingsField.SSR] = true;
            frameSettingsOverrideMask.mask[(uint)FrameSettingsField.SSAO] = true;
            frameSettingsOverrideMask.mask[(uint)FrameSettingsField.DepthOfField] = true;
            frameSettingsOverrideMask.mask[(uint)FrameSettingsField.Tonemapping] = true;
            frameSettingsOverrideMask.mask[(uint)FrameSettingsField.SSGI] = true;
            frameSettingsOverrideMask.mask[(uint)FrameSettingsField.ScreenSpaceShadows] = true;
            frameSettingsOverrideMask.mask[(uint)FrameSettingsField.Postprocess] = true;
            frameSettingsOverrideMask.mask[(uint)FrameSettingsField.Refraction] = true;
            frameSettingsOverrideMask.mask[(uint)FrameSettingsField.SkyReflection] = true;
            frameSettingsOverrideMask.mask[(uint)FrameSettingsField.ColorGrading] = true;

            HDCData.renderingPathCustomFrameSettingsOverrideMask = frameSettingsOverrideMask;

            frameSettings.SetEnabled(FrameSettingsField.ContactShadows, !isLightMode);
            frameSettings.SetEnabled(FrameSettingsField.MotionVectors, !isLightMode);
            frameSettings.SetEnabled(FrameSettingsField.SSR, !isLightMode);
            frameSettings.SetEnabled(FrameSettingsField.SSAO, !isLightMode);
            frameSettings.SetEnabled(FrameSettingsField.DepthOfField, !isLightMode);
            frameSettings.SetEnabled(FrameSettingsField.Tonemapping, !isLightMode);
            frameSettings.SetEnabled(FrameSettingsField.SSGI, !isLightMode);
            frameSettings.SetEnabled(FrameSettingsField.ScreenSpaceShadows, !isLightMode);
            frameSettings.SetEnabled(FrameSettingsField.Postprocess, !isLightMode);
            frameSettings.SetEnabled(FrameSettingsField.Refraction, !isLightMode);
            frameSettings.SetEnabled(FrameSettingsField.SkyReflection, !isLightMode);
            frameSettings.SetEnabled(FrameSettingsField.ColorGrading, !isLightMode);
            frameSettings.SetEnabled(FrameSettingsField.Antialiasing, !isLightMode);

            HDCData.renderingPathCustomFrameSettings = frameSettings;
        }
    }

    void SaveCSV(string nombreArchivo)
    {
#if UNITY_EDITOR
        string ruta = Application.persistentDataPath + "/" + nombreArchivo + ".csv";
#else
        string ruta = System.Environment.CurrentDirectory + "/" + nombreArchivo + ".csv";
#endif

        //El archivo existe? lo BORRAMOS
        if (File.Exists(ruta))
        {
            File.Delete(ruta);
        }

        //Crear el archivo
        var sr = File.CreateText(ruta);

        string datosCSV = "workload,category,comment,fps" + System.Environment.NewLine;
        if (benchIndex == -1)
        {
            foreach (SceneBench bench in sceneLists)
            {
                datosCSV += bench.info.name + "," + bench.info.category + "," + bench.info.comment + "," + bench.totalFps.ToString("f2") + System.Environment.NewLine;
            }
        }
        else
        {
            SceneBench bench = sceneLists[benchIndex];
            datosCSV += bench.info.name + "," + bench.info.category + "," + bench.info.comment + "," + bench.totalFps.ToString("f2") + System.Environment.NewLine;
        }

        sr.WriteLine(datosCSV);

        //Dejar como s¨®lo de lectura
        FileInfo fInfo = new FileInfo(ruta);
        fInfo.IsReadOnly = true;

        //Cerrar
        sr.Close();
    }

    void UploadTestResult(string testId, string cpuInfo, string gpuInfo, string benchVersion, string engineVersion, DateTime startTime, List<SceneBench> sceneLists)
    {
        string url = "https://uebench.intel.com/api/test/upload";
        JsonData benchDatas = new JsonData();

        WWWForm form = new WWWForm();
        form.AddField("id", testId);
        form.AddField("cpu", cpuInfo);
        form.AddField("gpu", gpuInfo);
        form.AddField("benchmark_version", benchVersion);
        form.AddField("engine_version", engineVersion);
        form.AddField("start_time", startTime.ToString("yyyy/MM/dd hh:mm:ss"));

        if (benchIndex == -1)
        {
            foreach (SceneBench bench in sceneLists)
            {
                JsonData benchData = new JsonData();

                SetBenchSceneJsonData(benchData, bench);

                benchDatas.Add(benchData);
            }
        }
        else
        {
            JsonData benchData = new JsonData();
            SceneBench bench = sceneLists[benchIndex];

            SetBenchSceneJsonData(benchData, bench);

            benchDatas.Add(benchData);
        }
        form.AddField("cases", benchDatas.ToJson());

        StartCoroutine(UnityWebRequestPost(url, form));
    }

    void SetBenchSceneJsonData(JsonData benchData, SceneBench bench)
    {
        benchData["name"] = bench.info.category + "_" + bench.info.name;
        benchData["param"] = 0;

        JsonData frameTimeDatas = new JsonData();
        for (int i = 0; i < bench.frameTimes.Count; i++)
        {
            frameTimeDatas.Add(bench.frameTimes[i].ToString());
        }

        benchData["frame_time_list"] = frameTimeDatas;
        benchData["start_time"] = bench.startTime.ToString("yyyy/MM/dd hh:mm:ss");
    }

    IEnumerator UnityWebRequestPost(string url, WWWForm form)
    {
        UnityWebRequest request = UnityWebRequest.Post(url, form);
        yield return request.SendWebRequest();
        Debug.Log("Status Code: " + request.responseCode);
        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log(request.error);
        }
        else
        {
            Debug.Log(request.downloadHandler.text);
        }
        request.Dispose();
        uploadFinished = true;
    }
}
