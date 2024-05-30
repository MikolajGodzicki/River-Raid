using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;

namespace River_Raid.Core {
    public class AudioManager {
        List<Song> Themes = new List<Song>();

        private Dictionary<string, SoundEffect> SoundEffects;

        ContentManager content { get; set; }

        float countDuration = 0f;
        float currentTime = 0f;

        public SoundEffectInstance FlyInstance;

        public AudioManager(ContentManager Content) {
            content = Content;

            content.RootDirectory = "Content/Audio";

            LoadSongs();

            LoadSoundEffects();

            content.RootDirectory = "Content";

            SetupFlyInstance();

            PlayRandomTheme();
        }

        private void PlayRandomTheme() {
            int random = new Random().Next(Themes.Count);
            countDuration = Themes[random].Duration.Minutes * 60 + Themes[random].Duration.Seconds;
            MediaPlayer.Play(Themes[random]);
        }

        private void SetupFlyInstance() {
            FlyInstance = SoundEffects["Fly"].CreateInstance();
            FlyInstance.Volume = 0.3f;
            FlyInstance.IsLooped = true;
        }

        private void LoadSongs() {

            for (int i = 3; i <= 6; i++) {
                Themes.Add(LoadAudioContent<Song>($"Theme_{i}"));
            }
        }

        private void LoadSoundEffects() {
            SoundEffects = new Dictionary<string, SoundEffect>() {
                {"Explosion", LoadAudioContent<SoundEffect>("Explosion") },
                {"EmptyWeapon", LoadAudioContent<SoundEffect>("EmptyWeapon") },
                {"Fly", LoadAudioContent<SoundEffect>("Fly") },
                {"Hit", LoadAudioContent<SoundEffect>("Hit") },
                {"MachineGun", LoadAudioContent<SoundEffect>("MachineGun") },
                {"Pickup", LoadAudioContent<SoundEffect>("Pickup") },
                {"Select", LoadAudioContent<SoundEffect>("Select") },
                {"Shoot", LoadAudioContent<SoundEffect>("Shoot") },
            };
        }

        private T LoadAudioContent<T>(string audio) => content.Load<T>(audio);

        public void PlaySound(string Type) {
            if (!SoundEffects.ContainsKey(Type)) {
                return;
            }

            SoundEffects[Type].Play();
        }

        public void Update(KeyboardState input) {
            if (input.IsKeyDown(Keys.OemOpenBrackets) && SoundEffect.MasterVolume > 0.1f) {
                SetSoundEffectMasterVolume(val: -0.1f);
            } else if (input.IsKeyDown(Keys.OemCloseBrackets) && SoundEffect.MasterVolume < 0.9f) {
                SetSoundEffectMasterVolume(val: 0.1f);
            } else if (input.IsKeyDown(Keys.M)) {
                SetSoundEffectMasterVolume(true);
            } else if (input.IsKeyDown(Keys.J)) {
                if (MediaPlayer.State == MediaState.Paused)
                    MediaPlayer.Resume();
                else
                    MediaPlayer.Pause();
            }
        }

        private void SetSoundEffectMasterVolume(bool mute = false, float val = 0f) {
            if (mute) {
                SoundEffect.MasterVolume = 0;
                return;
            }

            SoundEffect.MasterVolume += val;
        }

        public void UpdateTheme(GameTime gameTime) {
            currentTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (currentTime >= countDuration) {
                int random = new Random().Next(Themes.Count);
                countDuration = Themes[random].Duration.Minutes * 60 + Themes[random].Duration.Seconds;
                MediaPlayer.Play(Themes[random]);
                currentTime = 0;
            }
        }
    }
}
