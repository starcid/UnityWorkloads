using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Startup : MonoBehaviour
{
    public float intervalTime;
    public float benchTime;

    public SceneInfo[] sceneInfos;

    BenchmarkMain benchMain;

    TMP_InputField intervalTimeUI;
    TMP_InputField benchTimeUI;
    TMP_Dropdown selectUI;
    Toggle showFpsUI;

    TextMeshProUGUI sysInfoText;

    bool showFps;

    private void Awake()
    {
        string[] arguments = Environment.GetCommandLineArgs();

        Application.targetFrameRate = 30;
        if (arguments.Length >= 2)
        {
            int width = Screen.mainWindowDisplayInfo.width;
            int height = Screen.mainWindowDisplayInfo.height;
            FullScreenMode mode = FullScreenMode.FullScreenWindow;

            // check resolution / screen mode
            for (int i = 1; i < arguments.Length; i++)
            {
                if (arguments[i] == "-res")
                {
                    string[] resStr = arguments[i + 1].Split("x");
                    if (resStr != null && resStr.Length == 2)
                    {
                        int.TryParse(resStr[0], out width);
                        int.TryParse(resStr[1], out height);
                    }
                }
                else if (arguments[i] == "-fullscreenmode")
                {
                    int intMode;
                    if (int.TryParse(arguments[i + 1], out intMode))
                    {
                        mode = (FullScreenMode)intMode;
                    }
                }
            }
            Screen.SetResolution(width, height, mode);
        }
        else
        {
            DisplayInfo dispInfo = Screen.mainWindowDisplayInfo;
            Screen.SetResolution(dispInfo.width, dispInfo.height, FullScreenMode.FullScreenWindow);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        GameObject intervalTimeObj = GameObject.Find("IntervalTime");
        intervalTimeUI = intervalTimeObj.GetComponent<TMP_InputField>();
        GameObject benchTimeObj = GameObject.Find("BenchTime");
        benchTimeUI = benchTimeObj.GetComponent<TMP_InputField>();
        GameObject showFpsObj = GameObject.Find("ShowFPS");
        showFpsUI = showFpsObj.GetComponent<Toggle>();

        GameObject sysInfo = GameObject.Find("SystemInfo");
        GameObject graphicsApi = GameObject.Find("GraphicsApi");

        sysInfoText = sysInfo.GetComponent<TextMeshProUGUI>();

        TextMeshProUGUI graphicsApiText = graphicsApi.GetComponent<TextMeshProUGUI>();
        graphicsApiText.text = UnityEngine.SystemInfo.graphicsDeviceType.ToString();

        // init select
        GameObject selectObj = GameObject.Find("Select");
        selectUI = selectObj.GetComponent<TMP_Dropdown>();
        selectUI.ClearOptions();
        List<string> options = new List<string>();
        options.Add("FullBenchmark");
        for (int i = 0; i < sceneInfos.Length; i++)
        {
            options.Add(sceneInfos[i].category + "_" + sceneInfos[i].name + sceneInfos[i].paramaters);
        }
        selectUI.AddOptions(options);

        GameObject benchmarkFlow = GameObject.Find("BenchmarkFlow");
        GameObject.DontDestroyOnLoad(benchmarkFlow);
        GameObject canvas = GameObject.Find("Canvas");
        GameObject.DontDestroyOnLoad(canvas);

        GameObject startUI = GameObject.Find("Canvas/StartScene");
        GameObject benchUI = GameObject.Find("Canvas/BenchScene");
        GameObject intervalUI = GameObject.Find("Canvas/IntervalScene");

        benchMain = benchmarkFlow.GetComponent<BenchmarkMain>();
        benchMain.Initialize(ref sceneInfos, startUI, benchUI, intervalUI);
    }

    // Update is called once per frame
    void Update()
    {
        sysInfoText.text = UnityEngine.SystemInfo.graphicsDeviceName + "\n" + UnityEngine.Screen.currentResolution;
    }

    public void OnClickStart()
    {
        showFps = showFpsUI.isOn;
        float.TryParse(intervalTimeUI.text, out intervalTime);
        float.TryParse(benchTimeUI.text, out benchTime);

        if (benchMain)
        {
            benchMain.Begin(selectUI.value - 1, benchTime, intervalTime, showFps);
        }
    }
}
