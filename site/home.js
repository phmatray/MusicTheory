// MusicTheory home page: the folio, and the circle of fifths that turns under the scroll while the key
// signature engraves itself. The keys, signatures and scales come from #circle-data, which the site
// generator fills with what the library computes; nothing musical is decided here.
(() => {
  const reduce = matchMedia("(prefers-reduced-motion: reduce)").matches;
  const key = document.getElementById("key");
  if (reduce) {
    // Reduced motion: no pinned travel; chapter V shows the same keys as a table.
    key.removeAttribute("data-sc-act");
    document.documentElement.classList.add("reduce");
  }
  ScrollCraft.mount(document.body);

  // ── Folio ────────────────────────────────────────────────────────────────
  const numeral = document.querySelector("[data-folio-numeral]");
  const title = document.querySelector("[data-folio-title]");
  const observer = new IntersectionObserver(entries => {
    for (const e of entries) {
      if (!e.isIntersecting) continue;
      numeral.textContent = e.target.dataset.chapter || " ";
      title.textContent = e.target.dataset.chapterTitle;
    }
  }, { rootMargin: "-45% 0px -50% 0px" });
  document.querySelectorAll("[data-chapter-title]").forEach(s => observer.observe(s));

  if (reduce) return;

  // ── The circle ───────────────────────────────────────────────────────────
  const keys = JSON.parse(document.getElementById("circle-data").textContent);
  const byName = Object.fromEntries(keys.map(k => [k.major, k]));
  // Clockwise from C: the sharps grow to C♯ (which is also D♭), then the flats shrink back to C.
  const tour = ["C", "G", "D", "A", "E", "B", "F♯", "C♯", "A♭", "E♭", "B♭", "F", "C"].map(n => byName[n]);
  const step = 360 / 12;
  const svg = "http://www.w3.org/2000/svg";
  const ring = document.querySelector("[data-circle-ring]");

  const el = (name, attrs, text) => {
    const node = document.createElementNS(svg, name);
    for (const [k, v] of Object.entries(attrs)) node.setAttribute(k, v);
    if (text !== undefined) node.textContent = text;
    return node;
  };

  ring.append(el("circle", { class: "ring", r: 190 }), el("circle", { class: "ring", r: 96 }));
  const spelled = [];
  const labels = []; // counter-rotated each frame so the names stay upright while the ring turns
  // One position per fifth.
  for (let p = 0; p < 12; p++) {
    // Enharmonic keys (B/C♭, F♯/G♭, C♯/D♭) share a position: the ring shows the sharp spelling on the sharp
    // half of the tour and the flat one once the tour has crossed to A♭.
    const here = keys.filter(k => ((k.accidentals % 12) + 12) % 12 === p).sort((a, b) => b.accidentals - a.accidentals);
    const angle = (p * step - 90) * Math.PI / 180;
    const at = r => ({ x: (Math.cos(angle) * r).toFixed(2), y: (Math.sin(angle) * r).toFixed(2) });
    const tick = at(190), inner = at(178);
    ring.append(el("line", { x1: tick.x, y1: tick.y, x2: inner.x, y2: inner.y }));
    const major = at(150), minor = at(118);
    const majorLabel = ring.appendChild(el("text", { class: "circle__major", x: major.x, y: major.y }, here[0].major));
    const minorLabel = ring.appendChild(el("text", { class: "circle__minor", x: minor.x, y: minor.y }, here[0].minor));
    labels.push(majorLabel, minorLabel);
    if (here.length > 1) spelled.push({ majorLabel, minorLabel, sharp: here[0], flat: here[1] });
  }

  // ── The staff ────────────────────────────────────────────────────────────
  // Treble-clef engraving positions, in staff steps above E4 (the bottom line).
  const sharpSteps = { F: 8, C: 5, G: 9, D: 6, A: 3, E: 7, B: 4 };
  const flatSteps = { B: 4, E: 7, A: 3, D: 6, G: 2, C: 5, F: 1 };
  const lines = document.querySelector(".staff__lines");
  const bottom = 92, half = 7;
  for (let s = 0; s <= 8; s += 2) lines.append(el("line", { x1: 0, x2: 320, y1: bottom - s * half, y2: bottom - s * half }));
  const accidentals = document.querySelector("[data-staff-accidentals]");

  const major = document.querySelector("[data-key-major]");
  const minor = document.querySelector("[data-key-minor]");
  const signature = document.querySelector("[data-key-signature]");
  const scale = document.querySelector("[data-key-scale]");

  let shown = null;
  function show(k) {
    if (k === shown) return;
    const previous = shown ? shown.altered.join() : "";
    shown = k;
    major.textContent = k.major;
    minor.textContent = k.minor;
    signature.textContent = k.enharmonic ? `${k.signature}, or ${k.enharmonic} major` : k.signature;
    scale.textContent = k.majorScale;
    accidentals.replaceChildren();
    k.altered.forEach((name, i) => {
      const letter = name[0], sharp = name.endsWith("♯");
      const y = bottom - (sharp ? sharpSteps : flatSteps)[letter] * half;
      const glyph = el("text", { class: "staff__accidental", x: 74 + i * 20, y: y + (sharp ? 5 : 8) }, sharp ? "♯" : "♭");
      // Engrave the accidentals this key adds to the previous one.
      if (!previous.split(",").includes(name)) {
        glyph.setAttribute("data-new", "");
        requestAnimationFrame(() => requestAnimationFrame(() => glyph.removeAttribute("data-new")));
      }
      accidentals.append(glyph);
    });
  }

  let last = -1;
  function frame() {
    const p = parseFloat(key.style.getPropertyValue("--sc-p")) || 0;
    if (p !== last) {
      last = p;
      // A short hold at each end, then one fifth per stop.
      const f = Math.min(Math.max((p - 0.05) / 0.9, 0), 1) * (tour.length - 1);
      const angle = (f * step).toFixed(2);
      ring.setAttribute("transform", `rotate(${-angle})`);
      for (const label of labels) label.setAttribute("transform", `rotate(${angle} ${label.getAttribute("x")} ${label.getAttribute("y")})`);
      const flatSide = Math.round(f) > tour.indexOf(byName["C♯"]);
      for (const s of spelled) {
        const k = flatSide ? s.flat : s.sharp;
        s.majorLabel.textContent = k.major;
        s.minorLabel.textContent = k.minor;
      }
      show(tour[Math.round(f)]);
    }
    requestAnimationFrame(frame);
  }
  show(tour[0]);
  requestAnimationFrame(frame);
})();
