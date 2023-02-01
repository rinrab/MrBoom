async function loadSoundAssets() {
    async function loadSound(name) {
        let audio = document.getElementById("sound-" + name);
        audio.loop = false;

        if (audio.readyState != 4) {
            return new Promise((resolve, reject) => {
                audio.addEventListener("canplaythrough", () => {
                    resolve(audio);
                });

                audio.addEventListener("error", () => {
                    reject();
                })
            });
        } else {
            return audio;
        }
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
    };

    return result;
}

class SoundManager {
    constructor(soundAssets) {
        this.soundAssets = soundAssets;
    }

    playSound(name) {
        if (this.soundAssets[name]) {
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
