using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using River_Raid.Classes;
using System;

namespace River_Raid {
    class FuelPtr : GameObject {
        public Texture2D Fuel_Pointer;
        public Texture2D Fuel_UI;
        public int minFuel, maxFuel;
        private float FuelSpeed = 0.3f;

        bool isExploded = false;
        public bool IsAlive = true;
        public event Action OnFuelEmpty;

        public FuelPtr(Texture2D Fuel_Pointer, Texture2D Fuel_UI, int minFuel, int maxFuel, Vector2 position) {
            this.Fuel_Pointer = Fuel_Pointer;
            this.Fuel_UI = Fuel_UI;
            this.minFuel = minFuel;
            this.maxFuel = maxFuel;
            this.position = position;
        }

        public void UpdateFuelSpend() {
            if (!isExploded) {
                if (position.X > minFuel & IsAlive)
                    position.X -= FuelSpeed;
                else {
                    OnFuelEmpty?.Invoke();
                    isExploded = true;
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
