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
        static SoundEffect Fly, Hit, MachineGun, Pickup, Select, Shoot;
        ContentManager Content { get; set; }

        public SoundEffectInstance FlyInstance;

        public AudioManager(ContentManager Content) {
            Content.RootDirectory = "Content/Audio";
            /*
            for (int i = 1; i <= 6; i++) {
                Themes.Add(Content.Load<Song>($"Theme_{i}"));
            }
            */
            Fly = Content.Load<SoundEffect>("Fly");
            Hit = Content.Load<SoundEffect>("Hit");
            MachineGun = Content.Load<SoundEffect>("MachineGun");
            Pickup = Content.Load<SoundEffect>("Pickup");
            Select = Content.Load<SoundEffect>("Select");
            Shoot = Content.Load<SoundEffect>("Shoot");
            Content.RootDirectory = "Content";
            FlyInstance = Fly.CreateInstance();
            FlyInstance.IsLooped = true;
        }

        public void PlaySound(string Type) {
            switch (Type) {
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
            } else if (input.IsKeyDown(Keys.OemCloseBrackets) && SoundEffect.MasterVolume < 1f) {
                SoundEffect.MasterVolume += 0.1f;
            } else if (input.IsKeyDown(Keys.M)) {
                SoundEffect.MasterVolume = 0f;
            }
        }
    }
}
