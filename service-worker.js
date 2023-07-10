// Files to cache
const cacheName = 'MrBoom-v1';
const contentToCache = [
    "index.html"
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
    e.respondWith((async () => {
        const cache = await caches.open(cacheName);
        const r = await caches.match(e.request);
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
});
