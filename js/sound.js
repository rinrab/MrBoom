async function loadSoundAssets() {
    const poolSize = 4;

    async function loadSound(name) {
        let audio = new Audio("sound/" + name + ".wav");
        audio.loop = false;

        let result;

        if (audio.readyState != 4) {
            console.debug("audio (" + name + ") is not ready. Loading...");
            result = new Promise((resolve, reject) => {
                audio.oncanplaythrough = () => {
                    console.debug("audio (" + name + ") loaded.");
                    resolve(audio);
                    audio.oncanplaythrough = null;
                };

                audio.onerror = () => {
                    console.warn("audio (" + name + ") error.");
                    reject();
                    audio.onerror = null;
                };
            });
        } else {
            console.debug("audio (" + name + ") is ready.");
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
            play: function () {
                let audio = this.pool.pop();

                if (audio) {
                    audio.loop = false;
                    audio.onended = (event) => {
                        console.debug("audio ended");
                        this.pool.push(audio);
                        audio.onended = undefined;
                    };

                    audio.play();
                } else {
                    console.warn("Out of audio object for '" + name + "'");
                }
            }
        };
    }

    sounds = [
        "bang",
        "posebomb",
        "sac",
        "pick",
        "player_die",
        "oioi",
        "ai",
        "addplayer",
        "victory",
        "draw",
        "clock",
        "time_end"
    ]

    let result = {};
    for (let name of sounds) {
        result[name] = await makeSoundPool(name);
    }

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
