using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Builds the Loading scene from scratch: a dark background, a title, and a
/// slider with a percent readout driven by LoadingScreen. Also adds the scene
/// to Build Settings, after MainMenu so the boot order is untouched.
/// </summary>
static class LoadingSceneBuilder
{
    const string ScenePath = "Assets/Scenes/Loading.unity";
    const string RunOnceKey = "LoadingSceneBuilder.applied.v1";

    static readonly Vector2 ReferenceResolution = new Vector2(1920f, 1080f);
    static readonly Vector2 BarSize = new Vector2(760f, 36f);

    [MenuItem("Tools/Snooker/Build Loading Scene")]
    public static void Run()
    {
        Apply();
    }

    [InitializeOnLoadMethod]
    static void AutoRunOnce()
    {
        if (EditorPrefs.GetBool(RunOnceKey, false))
            return;

        EditorApplication.delayCall += () =>
        {
            if (EditorPrefs.GetBool(RunOnceKey, false))
                return;

            EditorPrefs.SetBool(RunOnceKey, true);
            Apply();
        };
    }

    static void Apply()
    {
        if (System.IO.File.Exists(ScenePath))
        {
            RegisterScene();
            Debug.Log($"[LoadingSceneBuilder] {ScenePath} already exists, left untouched. " +
                      "Build Settings re-checked.");
            return;
        }

        var previous = EditorSceneManager.GetActiveScene();
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
        EditorSceneManager.SetActiveScene(scene);

        try
        {
            BuildCamera();
            var canvas = BuildCanvas();
            var loader = BuildPanel(canvas.transform);

            if (!EditorSceneManager.SaveScene(scene, ScenePath))
            {
                Debug.LogError($"[LoadingSceneBuilder] Could not save the scene to {ScenePath}");
                return;
            }

            RegisterScene();

            Debug.Log($"[LoadingSceneBuilder] Built and saved {ScenePath}. " +
                      $"LoadingScreen component: {loader.name}.");
        }
        finally
        {
            if (previous.IsValid())
                EditorSceneManager.SetActiveScene(previous);

            EditorSceneManager.CloseScene(scene, true);
        }
    }

    static void BuildCamera()
    {
        var go = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener));
        go.tag = "MainCamera";

        var camera = go.GetComponent<Camera>();
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color(0.05f, 0.06f, 0.09f, 1f);
        camera.orthographic = true;

        go.transform.position = new Vector3(0f, 0f, -10f);
    }

    static Canvas BuildCanvas()
    {
        var go = new GameObject("Canvas",
            typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));

        var canvas = go.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        var scaler = go.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = ReferenceResolution;
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;

        return canvas;
    }

    static LoadingScreen BuildPanel(Transform parent)
    {
        var root = CreateUI("Panel", parent);
        root.anchorMin = root.anchorMax = root.pivot = new Vector2(0.5f, 0.5f);
        root.anchoredPosition = Vector2.zero;
        root.sizeDelta = new Vector2(900f, 300f);

        var loader = root.gameObject.AddComponent<LoadingScreen>();

        var title = CreateText("Title", root, "LOADING", 64f);
        title.rectTransform.anchoredPosition = new Vector2(0f, 70f);
        title.rectTransform.sizeDelta = new Vector2(root.sizeDelta.x, 100f);

        var slider = BuildSlider(root);
        slider.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, -20f);

        var percent = CreateText("Percent", root, "0%", 36f);
        percent.rectTransform.anchoredPosition = new Vector2(0f, -70f);
        percent.rectTransform.sizeDelta = new Vector2(root.sizeDelta.x, 60f);

        var so = new SerializedObject(loader);
        so.FindProperty("progressBar").objectReferenceValue = slider;
        so.FindProperty("percentText").objectReferenceValue = percent;
        so.ApplyModifiedProperties();

        return loader;
    }

    static Slider BuildSlider(Transform parent)
    {
        var rect = CreateUI("ProgressBar", parent);
        rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = BarSize;

        var background = rect.gameObject.AddComponent<Image>();
        background.color = new Color(1f, 1f, 1f, 0.15f);

        var slider = rect.gameObject.AddComponent<Slider>();
        slider.transition = Selectable.Transition.None;
        slider.interactable = false;

        var fillArea = CreateUI("Fill Area", rect);
        Stretch(fillArea);

        var fill = CreateUI("Fill", fillArea);
        Stretch(fill);
        var fillImage = fill.gameObject.AddComponent<Image>();
        fillImage.color = new Color(0.95f, 0.85f, 0.2f, 1f);

        slider.fillRect = fill;
        slider.targetGraphic = fillImage;
        slider.direction = Slider.Direction.LeftToRight;
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.value = 0f;

        return slider;
    }

    static Text CreateText(string name, Transform parent, string content, float size)
    {
        var rect = CreateUI(name, parent);
        rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0.5f, 0.5f);

        var text = rect.gameObject.AddComponent<Text>();
        text.text = content;
        text.fontSize = (int)size;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.white;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.raycastTarget = false;

        return text;
    }

    static RectTransform CreateUI(string name, Transform parent)
    {
        var go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        return (RectTransform)go.transform;
    }

    static void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        rect.localScale = Vector3.one;
    }

    static void RegisterScene()
    {
        var wanted = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);

        bool hasLoading = false;
        foreach (var existing in wanted)
            hasLoading |= existing.path == ScenePath;

        if (!hasLoading)
            wanted.Add(new EditorBuildSettingsScene(ScenePath, true));

        EditorBuildSettings.scenes = wanted.ToArray();
    }
}
