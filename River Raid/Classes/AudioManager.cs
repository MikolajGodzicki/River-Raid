using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace River_Raid.Classes {
    public class AudioManager {
        List<Song> Themes = new List<Song>();

        public SoundEffect Explosion, EmptyWeapon, Fly, Hit, MachineGun, Pickup, Select, Shoot;
        ContentManager Content { get; set; }

        float countDuration = 0f;
        float currentTime = 0f;

        public SoundEffectInstance FlyInstance;

        public AudioManager(ContentManager Content) {
            Content.RootDirectory = "Content/Audio";
            
            for (int i = 3; i <= 6; i++) {
                Themes.Add(Content.Load<Song>($"Theme_{i}"));
            }
            
            Explosion = Content.Load<SoundEffect>("Explosion");
            EmptyWeapon = Content.Load<SoundEffect>("EmptyWeapon");
            Fly = Content.Load<SoundEffect>("Fly");
            Hit = Content.Load<SoundEffect>("Hit");
            MachineGun = Content.Load<SoundEffect>("MachineGun");
            Pickup = Content.Load<SoundEffect>("Pickup");
            Select = Content.Load<SoundEffect>("Select");
            Shoot = Content.Load<SoundEffect>("Shoot");
            Content.RootDirectory = "Content";

            FlyInstance = Fly.CreateInstance();
            FlyInstance.Volume = 0.3f;
            FlyInstance.IsLooped = true;

            int random = new Random().Next(Themes.Count);
            countDuration = (Themes[random].Duration.Minutes * 60) + Themes[random].Duration.Seconds;
            MediaPlayer.Play(Themes[random]);
        }

        public void PlaySound(string Type) {
            switch (Type) {
                case "Explosion":
                    Explosion.Play();
                    break;
                case "EmptyWeapon":
                    EmptyWeapon.Play();
                    break;
                case "Hit":
                    Hit.Play();
                    break;
                case "MachineGun":
                    MachineGun.Play();
                    break;
                case "Pickup":
                    Pickup.Play();
                    break;
                case "Select":
                    Select.Play();
                    break;
                case "Shoot":
                    Shoot.Play();
                    break;
                default:
                    break;
            }
        }

        public void Update(KeyboardState input) {
            if (input.IsKeyDown(Keys.OemOpenBrackets) && SoundEffect.MasterVolume > 0.1f) {
                SoundEffect.MasterVolume -= 0.1f;
            } else if (input.IsKeyDown(Keys.OemCloseBrackets) && SoundEffect.MasterVolume < 0.9f) {
                SoundEffect.MasterVolume += 0.1f;
                
            } else if (input.IsKeyDown(Keys.M)) {
                SoundEffect.MasterVolume = 0f;
            }
        }

        public void UpdateTheme(GameTime gameTime) {
            currentTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (currentTime >= countDuration) {
                int random = new Random().Next(Themes.Count);
                countDuration = (Themes[random].Duration.Minutes * 60) + Themes[random].Duration.Seconds;
                MediaPlayer.Play(Themes[random]);
                currentTime = 0;
            }
        }
    }
}
