use wasm_bindgen::prelude::*;
use std::f32::consts::PI;

// Guitar string frequencies (standard tuning)
const OPEN_STRING_FREQUENCIES: [f32; 6] = [
    82.41,  // E2
    110.00, // A2
    146.83, // D3
    196.00, // G3
    246.94, // B3
    329.63, // E4
];

#[wasm_bindgen]
pub struct GuitarSynthesizer {
    sample_rate: f32,
    current_time: f32,
    active_notes: Vec<GuitarNote>,
    pluck_strength: f32,
    string_damping: f32,
}

#[wasm_bindgen]
#[derive(Clone)]
pub struct GuitarNote {
    frequency: f32,
    amplitude: f32,
    phase: f32,
    attack_time: f32,
    release_time: f32,
    start_time: f32,
    duration: f32,
    harmonics: Vec<f32>,
    string_index: usize,
}

#[wasm_bindgen]
impl GuitarSynthesizer {
    #[wasm_bindgen(constructor)]
    pub fn new(sample_rate: f32) -> Self {
        GuitarSynthesizer {
            sample_rate,
            current_time: 0.0,
            active_notes: Vec::new(),
            pluck_strength: 0.8,
            string_damping: 0.995,
        }
    }
    
    pub fn play_chord(&mut self, fret_positions: &[i32]) {
        self.active_notes.clear();
        
        for (string_index, &fret) in fret_positions.iter().enumerate() {
            if fret >= 0 {
                let frequency = self.calculate_frequency(string_index, fret);
                let note = GuitarNote {
                    frequency,
                    amplitude: self.pluck_strength,
                    phase: 0.0,
                    attack_time: 0.001,
                    release_time: 2.0,
                    start_time: self.current_time,
                    duration: 3.0,
                    harmonics: self.generate_harmonics(frequency),
                    string_index,
                };
                self.active_notes.push(note);
            }
        }
    }
    
    pub fn play_note(&mut self, string_index: usize, fret: i32) {
        let frequency = self.calculate_frequency(string_index, fret);
        let note = GuitarNote {
            frequency,
            amplitude: self.pluck_strength,
            phase: 0.0,
            attack_time: 0.001,
            release_time: 2.0,
            start_time: self.current_time,
            duration: 3.0,
            harmonics: self.generate_harmonics(frequency),
            string_index,
        };
        
        // Remove any existing note on the same string
        self.active_notes.retain(|n| n.string_index != string_index);
        self.active_notes.push(note);
    }
    
    pub fn stop_all(&mut self) {
        self.active_notes.clear();
    }
    
    pub fn process(&mut self, output_buffer: &mut [f32]) {
        for sample in output_buffer.iter_mut() {
            let mut output = 0.0;
            
            // Process each active note
            let mut i = 0;
            while i < self.active_notes.len() {
                let elapsed = self.current_time - self.active_notes[i].start_time;
                
                if elapsed > self.active_notes[i].duration {
                    self.active_notes.remove(i);
                    continue;
                }
                
                // Calculate envelope (using immutable borrow)
                let attack_time = self.active_notes[i].attack_time;
                let release_time = self.active_notes[i].release_time;
                let duration = self.active_notes[i].duration;
                let envelope = self.calculate_envelope(elapsed, attack_time, release_time, duration);
                
                // Generate sound using Karplus-Strong algorithm simulation
                let mut signal = 0.0;
                
                // Get note data for calculations
                let phase = self.active_notes[i].phase;
                let frequency = self.active_notes[i].frequency;
                let amplitude = self.active_notes[i].amplitude;
                let harmonics = self.active_notes[i].harmonics.clone();
                
                // Fundamental frequency
                signal += (phase * 2.0 * PI).sin() * amplitude;
                
                // Add harmonics for more realistic guitar sound
                for (harmonic_index, &harmonic_amp) in harmonics.iter().enumerate() {
                    let harmonic_phase = phase * (harmonic_index + 2) as f32;
                    signal += (harmonic_phase * 2.0 * PI).sin() * harmonic_amp * amplitude;
                }
                
                // Apply envelope and damping
                signal *= envelope * amplitude;
                output += signal;
                
                // Update phase (now using mutable borrow)
                let note = &mut self.active_notes[i];
                note.phase += note.frequency / self.sample_rate;
                if note.phase > 1.0 {
                    note.phase -= 1.0;
                }
                
                // Apply string damping
                note.amplitude *= self.string_damping;
                
                i += 1;
            }
            
            // Soft clipping to prevent distortion
            *sample = self.soft_clip(output * 0.3);
            
            self.current_time += 1.0 / self.sample_rate;
        }
    }
    
    pub fn set_pluck_strength(&mut self, strength: f32) {
        self.pluck_strength = strength.clamp(0.0, 1.0);
    }
    
    pub fn set_string_damping(&mut self, damping: f32) {
        self.string_damping = damping.clamp(0.9, 0.999);
    }
    
    fn calculate_frequency(&self, string_index: usize, fret: i32) -> f32 {
        if string_index >= 6 || fret < 0 {
            return 0.0;
        }
        
        let base_freq = OPEN_STRING_FREQUENCIES[string_index];
        base_freq * 2.0_f32.powf(fret as f32 / 12.0)
    }
    
    fn generate_harmonics(&self, _fundamental: f32) -> Vec<f32> {
        // Generate harmonic amplitudes for guitar-like timbre
        vec![
            0.5,   // 2nd harmonic
            0.3,   // 3rd harmonic
            0.2,   // 4th harmonic
            0.15,  // 5th harmonic
            0.1,   // 6th harmonic
            0.05,  // 7th harmonic
        ]
    }
    
    fn calculate_envelope(&self, elapsed: f32, attack: f32, release: f32, duration: f32) -> f32 {
        if elapsed < attack {
            // Attack phase
            elapsed / attack
        } else if elapsed < duration - release {
            // Sustain phase
            1.0
        } else {
            // Release phase
            let release_elapsed = elapsed - (duration - release);
            1.0 - (release_elapsed / release).min(1.0)
        }
    }
    
    fn soft_clip(&self, x: f32) -> f32 {
        if x > 1.0 {
            2.0 / 3.0
        } else if x < -1.0 {
            -2.0 / 3.0
        } else {
            x - (x * x * x) / 3.0
        }
    }
}

// AudioWorklet processor implementation
#[wasm_bindgen]
pub struct GuitarAudioProcessor {
    synthesizer: GuitarSynthesizer,
}

#[wasm_bindgen]
impl GuitarAudioProcessor {
    #[wasm_bindgen(constructor)]
    pub fn new(sample_rate: f32) -> Self {
        GuitarAudioProcessor {
            synthesizer: GuitarSynthesizer::new(sample_rate),
        }
    }
    
    pub fn process_audio(&mut self, output: &mut [f32]) {
        self.synthesizer.process(output);
    }
    
    pub fn play_chord(&mut self, fret_positions: &[i32]) {
        self.synthesizer.play_chord(fret_positions);
    }
    
    pub fn play_note(&mut self, string_index: usize, fret: i32) {
        self.synthesizer.play_note(string_index, fret);
    }
    
    pub fn stop_all(&mut self) {
        self.synthesizer.stop_all();
    }
    
    pub fn set_pluck_strength(&mut self, strength: f32) {
        self.synthesizer.set_pluck_strength(strength);
    }
    
    pub fn set_string_damping(&mut self, damping: f32) {
        self.synthesizer.set_string_damping(damping);
    }
}
