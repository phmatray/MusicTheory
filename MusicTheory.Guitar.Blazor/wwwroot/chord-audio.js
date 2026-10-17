// Light chord playback for MusicTheory.Guitar voicings: plucked strings (Karplus-Strong) rendered into
// AudioBuffers and strummed low to high. No dependencies, no WASM. Takes MIDI numbers, so it is tuning-
// and capo-agnostic and plays a piano voicing just as well (GuitarVoicing.ToMidi / ChordSymbol.Voice).

let ctx = null;
let current = null; // { out: GainNode, sources: AudioBufferSourceNode[] }
const buffers = new Map();

function context() {
  ctx ??= new AudioContext();
  return ctx;
}

// One plucked string: a noise burst through a damped delay line one period long.
function pluck(c, midi, seconds) {
  const key = `${midi}:${seconds}`;
  let buffer = buffers.get(key);
  if (buffer) return buffer;

  const rate = c.sampleRate;
  const length = Math.max(1, Math.round(seconds * rate));
  const period = Math.max(2, Math.round(rate / (440 * Math.pow(2, (midi - 69) / 12))));
  const damping = 0.996 - Math.max(0, midi - 40) * 0.0001; // higher strings die a little faster
  const fade = Math.min(length, Math.round(0.05 * rate));

  buffer = c.createBuffer(1, length, rate);
  const out = buffer.getChannelData(0);
  for (let i = 0; i < length; i++) {
    out[i] = i < period
      ? Math.random() * 2 - 1
      : damping * 0.5 * (out[i - period] + out[i - period + 1]);
  }
  for (let i = 0; i < fade; i++) out[length - 1 - i] *= i / fade; // no click at the end

  buffers.set(key, buffer);
  return buffer;
}

/**
 * Plays MIDI notes as a strum, low to high. Call it from a user gesture (click): it resumes a
 * suspended AudioContext. A new call stops the previous chord.
 * @param {number[]} midiNotes e.g. [48, 52, 55, 60, 64] for an open C
 * @param {{strumMs?: number, durationMs?: number, volume?: number}} [options]
 */
export async function playVoicing(midiNotes, { strumMs = 25, durationMs = 1800, volume = 0.6 } = {}) {
  const c = context();
  if (c.state === 'suspended') await c.resume();
  stop();

  const out = c.createGain();
  out.gain.value = volume / Math.sqrt(Math.max(1, midiNotes.length));
  out.connect(c.destination);

  const start = c.currentTime + 0.01;
  const seconds = durationMs / 1000;
  const sources = midiNotes.map((midi, i) => {
    const source = c.createBufferSource();
    source.buffer = pluck(c, midi, seconds);
    source.connect(out);
    source.start(start + (i * strumMs) / 1000);
    return source;
  });
  current = { out, sources };
}

/** Silences the current chord within ~30 ms. */
export function stop() {
  if (!current) return;
  const { out, sources } = current;
  current = null;
  const t = ctx.currentTime;
  out.gain.cancelScheduledValues(t);
  out.gain.setValueAtTime(out.gain.value, t);
  out.gain.linearRampToValueAtTime(0, t + 0.03);
  for (const source of sources) source.stop(t + 0.035);
  setTimeout(() => out.disconnect(), 100);
}
