async function loadSoundAssets() {
    const poolSize = 4;

    async function loadSound(name) {
        let audio = new Audio("sound/" + name + ".wav");
        audio.loop = false;

        let result;
        console.debug("audio: " + name, audio.readyState);

        if (audio.readyState != 4) {
            result = new Promise((resolve, reject) => {
                audio.addEventListener("canplaythrough", () => {
                    console.debug("audio: " + name, audio.readyState);
                    resolve(audio);
                });

                audio.addEventListener("error", () => {
                    console.debug("audio: " + name, audio.readyState);
                    reject();
                })
            });
        } else {
            result = audio;
        }

        audio.load();

        return result;
    }

    async function makeSoundPool(name) {
        let promises = [];
        for (let i = 0; i < poolSize; i++) {
            promises.push(loadSound(name));
        }

        return {
            pool: await Promise.all(promises),
            play: function() {
                let audio = this.pool.pop();

                audio.loop = false;
                audio.onended = (event) => {
                    console.debug("audio ended");
                    this.pool.push(audio);
                    audio.onended = undefined;
                };

                audio.play();
            }
        };
    }

    let result = {
        bang: await makeSoundPool("bang"),
        posebomb: await makeSoundPool("posebomb"),
        sac: await makeSoundPool("sac"),
        pick: await makeSoundPool("pick"),
        player_die: await makeSoundPool("player_die"),
        oioi: await makeSoundPool("oioi"),
        ai: await makeSoundPool("ai"),
        addplayer: await makeSoundPool("addplayer"),
        victory: await makeSoundPool("victory"),
        draw: await makeSoundPool("draw"),
        clock: await makeSoundPool("clock"),
        time_end: await makeSoundPool("time_end"),
    };

    return result;
}

class SoundManager {
    constructor() {
    }

    async init() {
        this.soundAssets = await loadSoundAssets();
    }

    playSound(name) {
        if (this.soundAssets && this.soundAssets[name]) {
            this.soundAssets[name].play();
        }
    }
}

class MusicManager {
    playlist;
    audio;
    constructor(playlist) {
        this.playlist = playlist;
    }

    start(song) {
        this.stop();
        this.next(song);
    }

    stop() {
        if (this.audio) {
            this.audio.pause();
            this.audio = null;
        }
    }

    next(song) {
        if (!args.includes("-z")) {
            this.stop();

            if (!song) {
                song = Math.floor(Math.random() * this.playlist.length);
            }

            this.audio = new Audio(this.playlist[song]);
            this.audio.volume = 0.7;
            this.audio.loop = true;
            this.audio.play();
        }
    }
}
