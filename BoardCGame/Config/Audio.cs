using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Audio;
using OpenTK.Audio.OpenAL;
using System.Media;
using System.IO;


namespace BoardCGame.Config
{
    public class Audio
    {
        private SoundPlayer _currentAudioPlayer;


        public static int BufferData(short[] data, ALFormat format, int sampleRate)
        {
            int handle = AL.GenBuffer();
            AL.BufferData(handle, format, data, data.Length * sizeof(short), sampleRate);
            return handle;
        }

        public void PlayData(short[] data, ALFormat format, int sampleRate)
        {
            int buffer = BufferData(data, format, sampleRate);
            int source = AL.GenSource();
            AL.SourceQueueBuffer(source, buffer);
            AL.SourcePlay(source);

            // wait for source to finish playing

            AL.SourceStop(source);
            AL.DeleteSource(source);
            AL.DeleteBuffer(buffer);
        }

        public void PlayIntro()
        {
            this.Play(Path.Combine("Sound/", "introsound.wav"));
        }

        public void Play(string resource)
        {
            _currentAudioPlayer = new SoundPlayer(resource);
            _currentAudioPlayer.Play();
        }

        public void PlayLooping(string resource)
        {
            _currentAudioPlayer = new SoundPlayer(resource);
            _currentAudioPlayer.PlayLooping();
        }

        public void Stop()
        {
            _currentAudioPlayer.Stop();
        }

        public void PlayRollDice()
        {
            PlayLooping(Path.Combine("Sound/", "rolling.wav"));
        }

        public void PlayStopDice()
        {
            Play(Path.Combine("Sound/", "rollingstop.wav"));
        }

        public void PlayAmbient()
        {
            SoundPlayer player = new SoundPlayer(Path.Combine("Sound/", "ambientsound.wav"));
            player.PlayLooping();
        }
    }
}
