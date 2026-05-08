window.widgetDrag = {
    _dragging: null,
    _dotnetRef: null,
    _rafId: null,
    _currentLeft: 0,
    _currentTop: 0,
    _targetLeft: 0,
    _targetTop: 0,
    _lastPushKey: null,

    GRID: 180,
    GAP: 12,
    LERP: 0.07,

    init(dotnetRef) {
        this._dotnetRef = dotnetRef;
        document.addEventListener('mousemove', (e) => this._onMove(e));
        document.addEventListener('mouseup',   () => this._onUp());
    },

    snap(v) {
        return Math.round(Math.max(0, v) / this.GRID) * this.GRID;
    },

    _maxLeft(el) {
        const canvas = document.getElementById('widget-canvas');
        if (!canvas) return 9999;
        return Math.max(0, canvas.clientWidth - el.offsetWidth - this.GAP);
    },

    _occupiedCells(excludeEl) {
        const map = new Map();
        document.querySelectorAll('[id^="widget-"]').forEach(w => {
            if (w === excludeEl || w.style.display === 'none') return;
            const x = this.snap(parseFloat(w.style.left) || 0);
            const y = this.snap(parseFloat(w.style.top)  || 0);
            map.set(`${x},${y}`, w);
        });
        return map;
    },

    _findEmpty(occupied, avoidKey) {
        for (let row = 0; row < 20; row++) {
            for (let col = 0; col < 20; col++) {
                const x = col * this.GRID;
                const y = row * this.GRID;
                const key = `${x},${y}`;
                if (key !== avoidKey && !occupied.has(key)) return { x, y };
            }
        }
        return { x: 0, y: 0 };
    },

    _pushColliding(snapLeft, snapTop, draggingEl) {
        const key = `${snapLeft},${snapTop}`;
        if (key === this._lastPushKey) return;
        const occupied = this._occupiedCells(draggingEl);
        const victim = occupied.get(key);
        if (!victim) return;
        this._lastPushKey = key;

        const withDragging = new Map(occupied);
        withDragging.set(key, draggingEl);
        const empty = this._findEmpty(withDragging, key);

        victim.style.transition = 'left 0.4s cubic-bezier(0.34,1.3,0.64,1), top 0.4s cubic-bezier(0.34,1.3,0.64,1)';
        victim.style.left = empty.x + 'px';
        victim.style.top  = empty.y + 'px';
        setTimeout(() => { victim.style.transition = ''; }, 450);

        const widgetId = victim.id.replace('widget-', '');
        if (this._dotnetRef)
            this._dotnetRef.invokeMethodAsync('OnWidgetMoved', widgetId, empty.x, empty.y);
    },

    _animate() {
        if (!this._dragging) return;
        const { el } = this._dragging;

        this._currentLeft += (this._targetLeft - this._currentLeft) * this.LERP;
        this._currentTop  += (this._targetTop  - this._currentTop)  * this.LERP;

        const clampedLeft = Math.min(this._currentLeft, this._maxLeft(el));
        el.style.left = clampedLeft + 'px';
        el.style.top  = this._currentTop + 'px';

        const snapLeft = this.snap(clampedLeft);
        const snapTop  = this.snap(this._currentTop);
        this._pushColliding(snapLeft, snapTop, el);

        this._rafId = requestAnimationFrame(() => this._animate());
    },

    _onMove(e) {
        if (!this._dragging) return;
        const { startX, startY, startLeft, startTop } = this._dragging;
        const rawLeft = startLeft + (e.clientX - startX);
        const rawTop  = startTop  + (e.clientY - startY);
        this._targetLeft = this.snap(rawLeft);
        this._targetTop  = this.snap(rawTop);
        const key = `${this._targetLeft},${this._targetTop}`;
        if (key !== this._lastPushKey) this._lastPushKey = null;
    },

    async _onUp() {
        if (!this._dragging) return;
        cancelAnimationFrame(this._rafId);
        const { widgetId, el } = this._dragging;
        this._dragging = null;
        this._lastPushKey = null;

        const left = this.snap(Math.min(parseFloat(el.style.left) || 0, this._maxLeft(el)));
        const top  = this.snap(parseFloat(el.style.top) || 0);
        el.style.left = left + 'px';
        el.style.top  = top  + 'px';

        if (this._dotnetRef)
            await this._dotnetRef.invokeMethodAsync('OnWidgetMoved', widgetId, left, top);
    },

    startDrag(clientX, clientY, widgetId) {
        const el = document.getElementById('widget-' + widgetId);
        if (!el) return;
        const startLeft = parseFloat(el.style.left) || 0;
        const startTop  = parseFloat(el.style.top)  || 0;
        this._currentLeft = startLeft;
        this._currentTop  = startTop;
        this._targetLeft  = startLeft;
        this._targetTop   = startTop;
        this._lastPushKey = null;
        this._dragging = { widgetId, el, startX: clientX, startY: clientY, startLeft, startTop };
        cancelAnimationFrame(this._rafId);
        this._rafId = requestAnimationFrame(() => this._animate());
    },

    dispose() {
        cancelAnimationFrame(this._rafId);
        this._dotnetRef = null;
        this._dragging = null;
    }
};
