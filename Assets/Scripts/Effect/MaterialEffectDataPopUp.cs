using UnityEditor;
using UnityEngine;

namespace Effect
{
    public class MaterialEffectDataPopup : EditorWindow
    {
        private MaterialEffectData data;

        public static void Open(MaterialEffectData data)
        {
            var window = GetWindow<MaterialEffectDataPopup>("Material Effect Data");
            window.data = data;
            window.Show();
        }

        void OnGUI()
        {
            if (data == null) return;

            data.ParticleAddress = EditorGUILayout.TextField("Particle Address", data.ParticleAddress);
            data.ParticleDuration = EditorGUILayout.FloatField("Particle Duration", data.ParticleDuration);
            data.DecalAddress = EditorGUILayout.TextField("Decal Address", data.DecalAddress);
            data.DecalDuration = EditorGUILayout.FloatField("Decal Duration", data.DecalDuration);
            data.SFX = EditorGUILayout.TextField("SFX", data.SFX);
            data.ExtraParamsRaw = EditorGUILayout.TextField("Extra Params", data.ExtraParamsRaw);

            if (GUILayout.Button("Reset")) { ResetData(); }
            if (GUILayout.Button("Close")) { Close(); }
        }
        void ResetData()
        {
            if (data == null) return;

            data.ParticleAddress = string.Empty;
            data.ParticleDuration = 1.0f;
            data.DecalAddress = string.Empty;
            data.DecalDuration = 1.0f;
            data.SFX = string.Empty;
            data.ExtraParamsRaw = string.Empty;
        }
    }
}