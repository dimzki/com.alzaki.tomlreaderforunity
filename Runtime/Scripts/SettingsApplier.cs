using UnityEngine;

namespace Alzaki.TomlReader
{
    public class SettingsApplier : MonoBehaviour
    {
        private void OnEnable()
        {
            TomlSettingsManager.OnSettingsReloaded += Apply;
        }

        private void OnDisable()
        {
            TomlSettingsManager.OnSettingsReloaded -= Apply;
        }

        private void Start()
        {
            Apply(TomlSettingsManager.Current);
        }

        private void Apply(GameSettings settings)
        {
            ApplyGraphics(settings.Graphics);
            ApplyAudio(settings.Audio);

            Debug.Log("Applied updated settings!");
        }

        private void ApplyGraphics(GraphicsSettings graphics)
        {
            Screen.SetResolution(graphics.Width, graphics.Height, graphics.Fullscreen);
            QualitySettings.vSyncCount = graphics.VSync ? 1 : 0;
        }

        private void ApplyAudio(AudioSettings audio)
        {
            AudioListener.volume = audio.MasterVolume;

            // TODO: Apply music/sfx volumes to AudioMixer when ready
        }
    }
}
