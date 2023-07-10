// Files to cache
const cacheName = 'MrBoom-v1';
const contentToCache = [
    "index.html",
    "Gfx2x/NEIGE1.PNG",
    "Gfx2x/NEIGE2.PNG",
    "Gfx2x/NEIGE3.PNG",
    "Gfx2x/GAME1.PNG",
    "Gfx2x/GAME2.PNG",
    "Gfx2x/GAME3.PNG",
    "Gfx2x/HELL1.PNG",
    "Gfx2x/HELL2.PNG",
    "Gfx2x/HELL3.PNG",
    "Gfx2x/FOOT.PNG",
    "Gfx2x/SOCCER.PNG",
    "Gfx2x/CRAYON.PNG",
    "Gfx2x/MICRO.PNG",
    "Gfx2x/FEUILLE_OVERLAY.PNG",
    "Gfx2x/FORET.PNG",
    "Gfx2x/NUAGE1.PNG",
    "Gfx2x/NUAGE2.PNG",
    "Gfx2x/SPRITE.PNG",
    "Gfx2x/SPRITE2.PNG",
    "Gfx2x/SPRITE3.PNG",
    "Gfx2x/MED3.PNG",
    "Gfx2x/PAUSE.PNG",
    "Gfx2x/MENU.PNG",
    "Gfx2x/FEUILLE.PNG",
    "Gfx2x/DRAW1.PNG",
    "Gfx2x/DRAW2.PNG",
    "Gfx2x/MED.PNG",
    "Gfx2x/ALPHA.PNG",
    "Gfx2x/GHOST.PNG",
    "Gfx2x/VIC1.PNG",
    "Gfx2x/VIC2.PNG",
    "Gfx2x/VIC3.PNG",
    "Gfx2x/VIC4.PNG",
    "Gfx2x/CRAYON2.PNG",
    "Gfx2x/SOUCOUPE.PNG",
    "js/mainloop.min.js",
    "js/assets.js",
    "js/sound.js",
    "js/main.js",
    "js/map.js",
    "style.css",
    "sound/bang.wav",
    "sound/posebomb.wav",
    "sound/sac.wav",
    "sound/pick.wav",
    "sound/player_die.wav",
    "sound/oioi.wav",
    "sound/ai.wav",
    "sound/addplayer.wav",
    "sound/victory.wav",
    "sound/draw.wav",
    "sound/clock.wav",
    "sound/time_end.wav",
    "music/anar11.mp3",
    "music/chipmunk.mp3",
    "music/chiptune.mp3",
    "music/deadfeel.mp3",
    "music/drop.mp3",
    "music/external.mp3",
    "music/matkamie.mp3",
    "music/unreeeal.mp3",
    "favicon.ico",
    "favicon.png",
    "manifest.json"
];
// Installing Service Worker
self.addEventListener('install', (e) => {
    console.log('[Service Worker] Install');
    e.waitUntil((async () => {
        const cache = await caches.open(cacheName);
        console.log('[Service Worker] Caching all: app shell and content');
        await cache.addAll(contentToCache);
    })());
});

self.addEventListener('activate', event => {
    event.waitUntil(clients.claim());
});

// Fetching content using Service Worker
self.addEventListener('fetch', (e) => {
    // Cache http and https only, skip unsupported chrome-extension:// and file://...
    if (e.request.url.startsWith('http:') || e.request.url.startsWith('https:')) {
        e.respondWith((async () => {
            const cache = await caches.open(cacheName);
            const r = await cache.match(e.request);
            if (r) {
                console.log(`[Service Worker] Serving ${e.request.url} from cache.`);
                return r;
            } else {
                console.warn(`[Service Worker] Serving ${e.request.url} from server.`);
                const response = await fetch(e.request);
                console.log(`[Service Worker] Caching new resource: ${e.request.url}`);

                // cache.put(e.request, response.clone());
                return response;
            }
        })());
    }
});
