using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LudoApp
{
    class SoundPlayer
    {
        public SoundPlayer() { }

        public void playSound(string soundName)
        {
            if (soundName == "send")
                try
                {
                    System.Media.SoundPlayer myPlayer = new System.Media.SoundPlayer();
                    myPlayer.SoundLocation = "../../Sound/sent.wav";
                    myPlayer.Play();
                }
                catch { }
            if (soundName == "received")
                try
                {
                    System.Media.SoundPlayer myPlayer = new System.Media.SoundPlayer();
                    myPlayer.SoundLocation = "../../Sound/received.wav";
                    myPlayer.Play();
                }
                catch { }
            if (soundName == "win")
                try
                {
                    System.Media.SoundPlayer myPlayer = new System.Media.SoundPlayer();
                    myPlayer.SoundLocation = "../../Sound/win.wav";
                    myPlayer.Play();
                }
                catch { }
            if (soundName == "move")
                try
                {
                    System.Media.SoundPlayer myPlayer = new System.Media.SoundPlayer();
                    myPlayer.SoundLocation = "../../Sound/move.wav";
                    myPlayer.Play();
                }
                catch { }
            if (soundName == "die")
                try
                {
                    System.Media.SoundPlayer myPlayer = new System.Media.SoundPlayer();
                    myPlayer.SoundLocation = "../../Sound/roll.wav";
                    myPlayer.Play();
                }
                catch { }
        }
    }
}
