using System;

namespace Laba._2_Animal.Models

{
    public class Dog : Creature, IVoiceCapable
    {
        public event EventHandler VoiceGiven;

        public Dog(double maxSpeed, double speedStep) : base(maxSpeed, speedStep)
        {
        }

        public override void Move()
        {
            CurrentSpeed += SpeedStep;
        }

        public override void Stand()
        {
            CurrentSpeed = Math.Max(0, CurrentSpeed - SpeedStep);
        }

        public void GiveVoice()
        {
            VoiceGiven?.Invoke(this, EventArgs.Empty);
        }

        public override string GetInfo()
        {
            return $"Собака: {base.GetInfo()}";
        }
    }
}