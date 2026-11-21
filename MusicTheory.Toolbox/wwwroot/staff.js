// Draws ABC text on a staff with the vendored abcjs; the notation itself is produced by C# (StaffAbc).
window.toolboxStaff = {
    render(id, abc) {
        const el = document.getElementById(id);
        if (!el || !window.ABCJS) return;
        const ink = getComputedStyle(document.documentElement).getPropertyValue("--sc-ink").trim() || "#000";
        ABCJS.renderAbc(el, abc, { responsive: "resize", foregroundColor: ink, add_classes: false });
    },
};
