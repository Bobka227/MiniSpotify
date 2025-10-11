let queue = [];
let queueKey = null;
let queueIndex = -1;
let currentTrackId = null; 

function fmtTime(sec) {
    if (!isFinite(sec)) return "0:00";
    sec = Math.max(0, Math.floor(sec));
    const m = Math.floor(sec / 60);
    const s = sec % 60;
    return m + ":" + String(s).padStart(2, "0");
}

function playFromData(d) {
    const audio = document.getElementById('audio');
    audio.src = d.src;
    audio.play().catch(() => { /* ignore autoplay block */ });

    document.getElementById('np-title').textContent = d.title || '—';
    document.getElementById('np-sub').textContent = d.artist || '—';
    const img = document.getElementById('np-cover');
    if (img) img.src = (d.cover && d.cover.length) ? d.cover : '/img/cover_placeholder.png';

    currentTrackId = d.id ?? null;

    const btnPlay = document.getElementById('btn-play');
    if (btnPlay) btnPlay.innerHTML = '<i class="bi bi-pause-fill fs-2"></i>';
}

window.playTrack = function (src, title, artist, cover, id) {
    queue = [{ id, src, title, artist, cover }];
    queueKey = 'single';
    queueIndex = 0;
    playFromData(queue[0]);
};

window.playFromButton = function (btn) {
    const key = btn.dataset.queue;
    const buttons = Array.from(document.querySelectorAll(`button.play-btn[data-queue="${key}"]`));

    queue = buttons.map(b => ({
        id: parseInt(b.dataset.id || '0'),
        src: b.dataset.src,
        title: b.dataset.title,
        artist: b.dataset.artist,
        cover: b.dataset.cover
    }));

    queueIndex = Math.max(0, buttons.indexOf(btn));
    queueKey = key;
    playFromData(queue[queueIndex]);
};

function playNext(step) {
    if (!queue.length || queueIndex < 0) return false;
    let idx = queueIndex + step;
    if (idx < 0 || idx >= queue.length) return false;
    queueIndex = idx;
    playFromData(queue[queueIndex]);
    return true;
}

document.addEventListener('DOMContentLoaded', () => {
    const audio = document.getElementById('audio');
    const btnPrev = document.getElementById('btn-prev');
    const btnPlay = document.getElementById('btn-play');
    const btnNext = document.getElementById('btn-next');
    const seek = document.getElementById('seek');
    const timeLbl = document.getElementById('time');
    const volume = document.getElementById('volume');
    const addBtn = document.getElementById('np-open'); 

    if (!audio) return;

    if (volume) {
        const v = parseFloat(volume.value);
        if (!isNaN(v)) audio.volume = v;
        volume.addEventListener('input', () => {
            const vv = parseFloat(volume.value);
            audio.volume = isNaN(vv) ? 1 : vv;
        });
    }

    if (btnPlay) {
        btnPlay.addEventListener('click', () => {
            if (audio.paused) audio.play().catch(() => { });
            else audio.pause();
        });
        audio.addEventListener('play', () => btnPlay.innerHTML = '<i class="bi bi-pause-fill fs-2"></i>');
        audio.addEventListener('pause', () => btnPlay.innerHTML = '<i class="bi bi-play-fill fs-2"></i>');
    }

    function seekBy(seconds) {
        if (!isFinite(audio.duration)) return;
        const t = Math.max(0, Math.min(audio.currentTime + seconds, audio.duration));
        audio.currentTime = t;
    }

    if (btnPrev) btnPrev.addEventListener('click', () => { if (!playNext(-1)) seekBy(-10); });
    if (btnNext) btnNext.addEventListener('click', () => { if (!playNext(+1)) seekBy(+10); });

    let seeking = false;

    audio.addEventListener('timeupdate', () => {
        if (!seek || seeking) return;
        const cur = audio.currentTime;
        const dur = audio.duration || 0;
        const pct = dur > 0 ? (cur / dur) * 100 : 0;
        seek.value = String(pct);
        if (timeLbl) timeLbl.textContent = `${fmtTime(cur)} / ${fmtTime(dur)}`;
    });

    if (seek) {
        seek.addEventListener('input', () => {
            seeking = true;
            const dur = audio.duration || 0;
            const pct = parseFloat(seek.value) || 0;
            const cur = (pct / 100) * dur;
            if (timeLbl) timeLbl.textContent = `${fmtTime(cur)} / ${fmtTime(dur)}`;
        });
        seek.addEventListener('change', () => {
            const dur = audio.duration || 0;
            const pct = parseFloat(seek.value) || 0;
            const cur = (pct / 100) * dur;
            audio.currentTime = cur;
            seeking = false;
        });
    }

    audio.addEventListener('ended', () => {
        if (!playNext(+1)) {
            if (seek) seek.value = "0";
            if (timeLbl) timeLbl.textContent = "0:00 / " + fmtTime(audio.duration || 0);
            if (btnPlay) btnPlay.innerHTML = '<i class="bi bi-play-fill fs-2"></i>';
        }
    });

    audio.addEventListener('loadedmetadata', () => {
        if (timeLbl) timeLbl.textContent = `0:00 / ${fmtTime(audio.duration || 0)}`;
        if (seek) seek.value = "0";
    });

    if (addBtn) {
        addBtn.addEventListener('click', () => {
            if (!currentTrackId) { alert('Nothing is playing'); return; }
            const hidden = document.getElementById('atp-trackId'); 
            if (hidden) hidden.value = currentTrackId;

            const modalEl = document.getElementById('addToPlaylistModal');
            if (modalEl && window.bootstrap && bootstrap.Modal) {
                const modal = new bootstrap.Modal(modalEl);
                modal.show();
            }
        });
    }
});
