window.noticeStore = {
    get: (k) => localStorage.getItem(k),
    set: (k, v) => localStorage.setItem(k, v),
    del: (k) => localStorage.removeItem(k)
};
