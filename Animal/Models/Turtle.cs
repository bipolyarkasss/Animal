using System;

namespace Laba._2_Animal.Models

{
    public class Turtle : Creature
    {
        public Turtle(double maxSpeed, double speedStep) : base(maxSpeed, speedStep)
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

        public override string GetInfo()
        {
            return $"Черепаха: {base.GetInfo()}";
        }
    }
}