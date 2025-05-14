using System;

namespace Laba._2_Animal.Models

{
    public interface IVoiceCapable
    {
        event EventHandler VoiceGiven;

        void GiveVoice();
    }
}