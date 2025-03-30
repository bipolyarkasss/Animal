using System;

namespace Laba._2_Animal.Models

{
    public class Panther : Creature, IVoiceCapable, ITreeClimber
    {
        private bool _isOnTree;

        public event EventHandler VoiceGiven;

        public bool IsOnTree => _isOnTree;

        public Panther(double maxSpeed, double speedStep) : base(maxSpeed, speedStep)
        {
            _isOnTree = false;
        }

        public override void Move()
        {
            if (IsOnTree)
            {
                return;
            }

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

        public void ClimbTree()
        {
            if (!IsOnTree)
            {
                _isOnTree = true;
                CurrentSpeed = 0;
            }
        }

        public void GetDownFromTree()
        {
            if (IsOnTree)
            {
                _isOnTree = false;
            }
        }

        public override string GetInfo()
        {
            string baseInfo = base.GetInfo();
            string treeStatus = IsOnTree ? "на дереве" : "на земле";
            return $"Пантера: {baseInfo}, {treeStatus}";
        }
    }
}