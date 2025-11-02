// Guitar Audio Worklet Processor - Pure JavaScript implementation
class GuitarAudioProcessor extends AudioWorkletProcessor {
    constructor() {
        super();
        
        console.log('GuitarAudioProcessor constructor called');
        
        // Guitar string frequencies (standard tuning)
        this.openStringFrequencies = [82.41, 110.00, 146.83, 196.00, 246.94, 329.63];
        
        // Active notes
        this.activeNotes = [];
        
        // Audio parameters
        this.pluckStrength = 0.8;
        this.stringDamping = 0.998; // Less damping for longer sustain
        
        // Debug flag
        this.debugCount = 0;
        this.processCallCount = 0;
        
        // Handle messages from main thread
        this.port.onmessage = (event) => {
            this.handleMessage(event.data);
        };
        
        // Notify main thread that we're ready
        this.port.postMessage({ type: 'initialized' });
        console.log('GuitarAudioProcessor initialized');
    }
    
    handleMessage(data) {
        switch (data.type) {
            case 'playChord':
                this.playChord(data.fretPositions);
                break;
                
            case 'playNote':
                this.playNote(data.stringIndex, data.fret);
                break;
                
            case 'stopAll':
                this.activeNotes = [];
                break;
                
            case 'setPluckStrength':
                this.pluckStrength = Math.max(0, Math.min(1, data.strength));
                break;
                
            case 'setStringDamping':
                this.stringDamping = Math.max(0.9, Math.min(0.999, data.damping));
                break;
                
            case 'testTone':
                // Simple test tone at 440Hz
                this.testTone = true;
                this.testPhase = 0;
                console.log('Test tone activated');
                break;
                
            case 'stopTestTone':
                this.testTone = false;
                break;
        }
    }
    
    playChord(fretPositions) {
        console.log('playChord called with:', fretPositions);
        
        // Reset debug counter when playing new chord
        this.debugCounter = 0;
        
        // Clear existing notes
        this.activeNotes = [];
        
        // Add new notes
        for (let i = 0; i < fretPositions.length; i++) {
            if (fretPositions[i] >= 0) {
                const frequency = this.calculateFrequency(i, fretPositions[i]);
                console.log(`Adding note: String ${i}, Fret ${fretPositions[i]}, Freq ${frequency}Hz`);
                this.activeNotes.push({
                    frequency: frequency,
                    phase: 0,
                    amplitude: this.pluckStrength,
                    harmonics: this.generateHarmonics(),
                    stringIndex: i,
                    time: 0
                });
            }
        }
        
        console.log(`Total active notes: ${this.activeNotes.length}`);
    }
    
    playNote(stringIndex, fret) {
        // Remove any existing note on the same string
        this.activeNotes = this.activeNotes.filter(n => n.stringIndex !== stringIndex);
        
        // Add new note
        const frequency = this.calculateFrequency(stringIndex, fret);
        this.activeNotes.push({
            frequency: frequency,
            phase: 0,
            amplitude: this.pluckStrength,
            harmonics: this.generateHarmonics(),
            stringIndex: stringIndex,
            time: 0
        });
    }
    
    calculateFrequency(stringIndex, fret) {
        if (stringIndex < 0 || stringIndex >= 6 || fret < 0) {
            return 0;
        }
        
        const baseFreq = this.openStringFrequencies[stringIndex];
        return baseFreq * Math.pow(2, fret / 12);
    }
    
    generateHarmonics() {
        return [0.5, 0.3, 0.2, 0.15, 0.1, 0.05];
    }
    
    calculateEnvelope(time) {
        const attackTime = 0.002; // Very fast attack
        const decayTime = 0.1;    // Quick decay to sustain level
        const sustainLevel = 0.6;  // Sustain at 60%
        const releaseTime = 3.0;   // Longer release
        
        if (time < attackTime) {
            // Attack phase
            return time / attackTime;
        } else if (time < attackTime + decayTime) {
            // Decay phase
            const decayProgress = (time - attackTime) / decayTime;
            return 1 - (1 - sustainLevel) * decayProgress;
        } else if (time < releaseTime) {
            // Sustain/Release phase
            const releaseProgress = time / releaseTime;
            return sustainLevel * Math.exp(-releaseProgress * 2);
        } else {
            return 0;
        }
    }
    
    process(inputs, outputs, parameters) {
        // Log first few calls
        if (this.processCallCount < 5) {
            console.log(`Process call ${this.processCallCount}, outputs:`, outputs);
            this.processCallCount++;
        }
        
        const output = outputs[0];
        if (!output || output.length === 0) {
            console.log('No output buffer!');
            return true;
        }
        
        // Initialize debug counter
        if (!this.debugCounter) {
            this.debugCounter = 0;
            console.log('Process method started, sampleRate:', globalThis.sampleRate);
        }
        
        const sampleRate = globalThis.sampleRate;
        const dt = 1 / sampleRate;
        
        // Debug log more frequently at start
        this.debugCounter++;
        if (this.activeNotes.length > 0 && (this.debugCounter < 10 || this.debugCounter % 1000 === 0)) {
            console.log(`Frame ${this.debugCounter}: Processing ${this.activeNotes.length} notes, sampleRate: ${sampleRate}`);
        }
        
        // Test tone override for debugging
        if (this.testTone) {
            const freq = 440;
            for (let i = 0; i < output[0].length; i++) {
                const sample = Math.sin(this.testPhase * 2 * Math.PI) * 0.2;
                this.testPhase += freq * dt;
                if (this.testPhase > 1) this.testPhase -= 1;
                
                for (let channel = 0; channel < output.length; channel++) {
                    output[channel][i] = sample;
                }
            }
            return true;
        }
        
        // Debug: check if we're generating any samples
        let maxSample = 0;
        let samplesGenerated = 0;
        
        // Process each sample
        for (let i = 0; i < output[0].length; i++) {
            let sample = 0;
            
            // Process each active note
            for (let j = this.activeNotes.length - 1; j >= 0; j--) {
                const note = this.activeNotes[j];
                
                // Calculate envelope
                const envelope = this.calculateEnvelope(note.time);
                
                // Debug first note on first frame
                if (j === 0 && this.debugCounter === 1 && i === 0) {
                    console.log(`First sample - Note 0: time=${note.time}, env=${envelope}, amp=${note.amplitude}, freq=${note.frequency}`);
                }
                
                // If note has faded out, remove it
                if (envelope <= 0 || note.amplitude < 0.001) {
                    this.activeNotes.splice(j, 1);
                    continue;
                }
                
                // Generate guitar-like sound with harmonics
                let noteOutput = 0;
                
                // Fundamental frequency
                noteOutput += Math.sin(note.phase * 2 * Math.PI);
                
                // Add harmonics for guitar timbre
                if (note.harmonics && note.harmonics.length > 0) {
                    // 2nd harmonic (octave)
                    noteOutput += Math.sin(note.phase * 4 * Math.PI) * 0.5;
                    // 3rd harmonic
                    noteOutput += Math.sin(note.phase * 6 * Math.PI) * 0.3;
                    // 4th harmonic
                    noteOutput += Math.sin(note.phase * 8 * Math.PI) * 0.2;
                }
                
                // Apply envelope and amplitude
                noteOutput *= envelope * note.amplitude;
                sample += noteOutput;
                
                // Update phase
                note.phase += note.frequency * dt;
                if (note.phase > 1) {
                    note.phase -= 1;
                }
                
                // Apply string damping
                note.amplitude *= this.stringDamping;
                
                // Update time
                note.time += dt;
            }
            
            // Mix down multiple notes and apply gain
            if (this.activeNotes.length > 0) {
                sample = sample / Math.sqrt(this.activeNotes.length) * 0.3;
            }
            
            // Soft clipping
            if (sample > 1) {
                sample = 1;
            } else if (sample < -1) {
                sample = -1;
            }
            
            // Track max sample for debugging
            maxSample = Math.max(maxSample, Math.abs(sample));
            if (sample !== 0) samplesGenerated++;
            
            // Write to all output channels
            for (let channel = 0; channel < output.length; channel++) {
                output[channel][i] = sample;
            }
        }
        
        // Debug output
        if (this.activeNotes.length > 0 && this.debugCounter <= 5) {
            console.log(`Frame ${this.debugCounter}: Generated ${samplesGenerated} non-zero samples, max amplitude: ${maxSample.toFixed(6)}`);
        }
        
        return true;
    }
}

registerProcessor('guitar-audio-processor', GuitarAudioProcessor);