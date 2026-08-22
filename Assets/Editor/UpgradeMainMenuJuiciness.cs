using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using TMPro;
using IndependenceGame.MainMenu;

namespace IndependenceGame.MainMenu.Editor
{
    public static class UpgradeMainMenuJuiciness
    {
        [MenuItem("Game/Upgrade Main Menu Juiciness")]
        public static void UpgradeScene()
        {
            var canvas = GameObject.Find("Canvas");
            if (canvas == null)
            {
                Debug.LogError("Canvas not found!");
                return;
            }

            var bgGO = GameObject.Find("CinematicBackground");
            var titleGO = GameObject.Find("TitleBlock");
            var stackGO = GameObject.Find("ButtonStack");

            // 1. Add Parallax to Canvas
            var parallax = canvas.GetComponent<MenuParallaxEffect>();
            if (parallax == null) parallax = canvas.AddComponent<MenuParallaxEffect>();

            var soParallax = new SerializedObject(parallax);
            if (bgGO != null) soParallax.FindProperty("backgroundTransform").objectReferenceValue = bgGO.GetComponent<RectTransform>();
            if (titleGO != null) soParallax.FindProperty("titleTransform").objectReferenceValue = titleGO.GetComponent<RectTransform>();
            if (stackGO != null) soParallax.FindProperty("buttonsTransform").objectReferenceValue = stackGO.GetComponent<RectTransform>();
            soParallax.FindProperty("backgroundStrength").floatValue = 22f;
            soParallax.FindProperty("titleStrength").floatValue = 10f;
            soParallax.FindProperty("buttonsStrength").floatValue = 14f;
            soParallax.FindProperty("smoothSpeed").floatValue = 6f;
            soParallax.ApplyModifiedProperties();

            // 2. Upgrade Buttons with JuicyMenuButton
            string[] buttonNames = { "Btn_Start", "Btn_Continue", "Btn_Settings", "Btn_Exit" };
            foreach (var bName in buttonNames)
            {
                var btnGO = GameObject.Find(bName);
                if (btnGO != null)
                {
                    var oldHover = btnGO.GetComponent<MenuButtonHoverEffect>();
                    if (oldHover != null) Object.DestroyImmediate(oldHover);

                    var juicy = btnGO.GetComponent<JuicyMenuButton>();
                    if (juicy == null) juicy = btnGO.AddComponent<JuicyMenuButton>();

                    var soJuicy = new SerializedObject(juicy);
                    soJuicy.FindProperty("hoverSlideDistance").floatValue = 16f;
                    soJuicy.FindProperty("slideSpeed").floatValue = 15f;
                    soJuicy.FindProperty("hoverScale").floatValue = 1.03f;
                    soJuicy.FindProperty("pressScale").floatValue = 0.94f;
                    soJuicy.FindProperty("scaleSpeed").floatValue = 18f;
                    soJuicy.FindProperty("normalTracking").floatValue = 3f;
                    soJuicy.FindProperty("hoverTracking").floatValue = 7f;
                    soJuicy.FindProperty("normalTextColor").colorValue = new Color(0.94f, 0.88f, 0.74f, 0.9f);
                    soJuicy.FindProperty("hoverTextColor").colorValue = new Color(1f, 0.98f, 0.88f, 1f);
                    soJuicy.FindProperty("normalOutlineColor").colorValue = new Color(0.85f, 0.70f, 0.35f, 0.35f);
                    soJuicy.FindProperty("hoverOutlineColor").colorValue = new Color(1f, 0.88f, 0.50f, 0.95f);
                    soJuicy.FindProperty("buttonText").objectReferenceValue = btnGO.GetComponentInChildren<TextMeshProUGUI>();
                    soJuicy.ApplyModifiedProperties();

                    // Set transition none on Unity Button to let JuicyMenuButton have full smooth control
                    var btn = btnGO.GetComponent<Button>();
                    if (btn != null)
                    {
                        btn.transition = Selectable.Transition.None;
                    }
                }
            }

            // 3. Create Atmospheric Particles
            var existingParticles = GameObject.Find("AtmosphereParticles");
            if (existingParticles == null)
            {
                var partGO = new GameObject("AtmosphereParticles", typeof(ParticleSystem));
                partGO.transform.SetParent(canvas.transform, false);
                partGO.transform.SetSiblingIndex(2); // Between vignette and UI

                var ps = partGO.GetComponent<ParticleSystem>();
                var main = ps.main;
                main.loop = true;
                main.startLifetime = 9f;
                main.startSpeed = new ParticleSystem.MinMaxCurve(8f, 22f);
                main.startSize = new ParticleSystem.MinMaxCurve(2f, 6f);
                main.startColor = new ParticleSystem.MinMaxGradient(
                    new Color(1f, 0.85f, 0.55f, 0.45f),
                    new Color(0.95f, 0.65f, 0.25f, 0.25f)
                );
                main.maxParticles = 55;
                main.simulationSpace = ParticleSystemSimulationSpace.Local;

                var emission = ps.emission;
                emission.rateOverTime = 6f;

                var shape = ps.shape;
                shape.shapeType = ParticleSystemShapeType.Rectangle;
                shape.scale = new Vector3(1000f, 650f, 1f);
                shape.position = new Vector3(0, -100f, 0);

                var vel = ps.velocityOverLifetime;
                vel.enabled = true;
                vel.x = new ParticleSystem.MinMaxCurve(3f, 10f); // Gentle rightward breeze
                vel.y = new ParticleSystem.MinMaxCurve(4f, 16f); // Gentle upward float

                var col = ps.colorOverLifetime;
                col.enabled = true;
                Gradient grad = new Gradient();
                grad.SetKeys(
                    new GradientColorKey[] { new GradientColorKey(new Color(1f, 0.85f, 0.55f), 0f), new GradientColorKey(new Color(0.95f, 0.65f, 0.25f), 1f) },
                    new GradientAlphaKey[] { new GradientAlphaKey(0f, 0f), new GradientAlphaKey(0.5f, 0.3f), new GradientAlphaKey(0.4f, 0.7f), new GradientAlphaKey(0f, 1f) }
                );
                col.color = grad;

                var sz = ps.sizeOverLifetime;
                sz.enabled = true;
                AnimationCurve curve = new AnimationCurve();
                curve.AddKey(0f, 0.2f);
                curve.AddKey(0.4f, 1f);
                curve.AddKey(1f, 0f);
                sz.size = new ParticleSystem.MinMaxCurve(1f, curve);

                var renderer = partGO.GetComponent<ParticleSystemRenderer>();
                renderer.sortingOrder = 5;
            }

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
            Debug.Log("<color=green>[Juiciness]</color> Main Menu upgraded with Parallax, Juicy Spring Buttons, and Atmospheric Sunset Particles!");
        }
    }
}
