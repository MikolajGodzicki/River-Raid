using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using River_Raid.Classes;
using River_Ride___MG;
using System;

namespace River_Raid {
    class FuelPtr : GameObject {
        public Texture2D Fuel_Pointer;
        public Texture2D Fuel_UI;
        public int minFuel, maxFuel;

        public bool IsAlive = true;
        public event Action OnFuelEmpty;

        public FuelPtr(Texture2D Fuel_Pointer, Texture2D Fuel_UI, int minFuel, int maxFuel, Vector2 position) {
            this.Fuel_Pointer = Fuel_Pointer;
            this.Fuel_UI = Fuel_UI;
            this.minFuel = minFuel;
            this.maxFuel = maxFuel;
            this.position = position;
        }

        public void UpdateFuelSpend(Player player) {
            if (player.IsAlive) {
                if (position.X > minFuel & player.IsAlive)
                    position.X -= Main.FuelSpeed;
                else {
                    OnFuelEmpty?.Invoke();
                }
            }
        }

        public void AddFuel(float amount) {
            float tempAmount = amount;
            if (position.X + amount > maxFuel)
                tempAmount -= (position.X + amount) - maxFuel;
            position.X += tempAmount;
        }
    }
}
