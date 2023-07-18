using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using System.Threading;
using System.Threading.Tasks;

namespace MrBoom
{
    public class SoundAssets
    {
        public class Sound
        {
            private SoundEffect sound;

            public Sound(SoundEffect song)
            {
                this.sound = song;
            }

            public void Play()
            {
                sound.Play();
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

        public static SoundAssets Load(ContentManager content)
        {
            Sound loadSound(string name)
            {
                return new Sound(content.Load<SoundEffect>("sound\\" + name));
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
            };
        }
    }
}
