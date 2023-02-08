async function loadSoundAssets() {
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

    let result = {
        bang: await loadSound("bang"),
        posebomb: await loadSound("posebomb"),
        sac: await loadSound("sac"),
        pick: await loadSound("pick"),
        player_die: await loadSound("player_die"),
        oioi: await loadSound("oioi"),
        ai: await loadSound("ai"),
        addplayer: await loadSound("addplayer"),
        victory: await loadSound("victory"),
        draw: await loadSound("draw"),
        clock: await loadSound("clock"),
        time_end: await loadSound("time_end"),
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
            const audio = new Audio(this.soundAssets[name].src);
            audio.loop = false;
            audio.play();
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
