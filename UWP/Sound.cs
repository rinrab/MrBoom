using System;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

namespace MrBoom
{
    public class SoundAssets
    {
        public class Sound
        {
            private SoundEffect sound;

            public Sound(SoundEffect sound)
            {
                this.sound = sound;
            }

            public void Play()
            {
                sound.Play();
            }
        }

        public class Music
        {
            private Song song;

            public Music(Song song)
            {
                this.song = song;
            }

            public void Play()
            {
                MediaPlayer.Play(song);
            }
        }

        public Sound Bang;
        public Sound PoseBomb;
        public Sound Sac;
        public Sound Pick;
        public Sound PlayerDie;
        public Sound Oioi;
        public Sound Ai;
        public Sound Addplayer;
        public Sound Victory;
        public Sound Draw;
        public Sound Clock;
        public Sound TimeEnd;
        public Music[] Musics;

        public static SoundAssets Load(ContentManager content)
        {
            Sound loadSound(string name)
            {
                return new Sound(content.Load<SoundEffect>("sound\\" + name));
            }

            Music loadMusic(string name)
            {
                return new Music(content.Load<Song>("music\\" + name));
            }

            return new SoundAssets()
            {
                Bang = loadSound("bang"),
                PoseBomb = loadSound("posebomb"),
                Sac = loadSound("sac"),
                Pick = loadSound("pick"),
                PlayerDie = loadSound("player_die"),
                Oioi = loadSound("oioi"),
                Ai = loadSound("ai"),
                Addplayer = loadSound("addplayer"),
                Victory = loadSound("victory"),
                Draw = loadSound("draw"),
                Clock = loadSound("clock"),
                TimeEnd = loadSound("time_end"),
                Musics = new Music[]
                {
                    loadMusic("anar11"),
                    loadMusic("chipmunk"),
                    loadMusic("chiptune"),
                    loadMusic("deadfeel"),
                    loadMusic("drop"),
                    loadMusic("external"),
                    loadMusic("matkamie"),
                    loadMusic("unreeeal"),
                }
            };
        }
    }

    [Flags]
    public enum Sound
    {
        Bang = 2 << 0,
        PoseBomb = 2 << 1,
        Sac = 2 << 2,
        Pick = 2 << 3,
        PlayerDie = 2 << 4,
        Oioi = 2 << 5,
        Ai = 2 << 6,
        Addplayer = 2 << 7,
        Victory = 2 << 8,
        Draw = 2 << 9,
        Clock = 2 << 10,
        TimeEnd = 2 << 11,
    }
}
