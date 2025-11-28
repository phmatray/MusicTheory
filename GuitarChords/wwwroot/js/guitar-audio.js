// Guitar Audio Manager - Main thread JavaScript
window.GuitarAudio = {
    audioContext: null,
    audioWorkletNode: null,
    masterGainNode: null,
    isInitialized: false,
    initCallbacks: [],
    masterVolume: 0.7,

    // Strumming patterns: D=down, U=up, x=muted, .=rest
    strummingPatterns: {
        'basic-4': { name: 'Basic 4/4', pattern: 'D...D...D...D...', beatsPerMeasure: 4 },
        'folk': { name: 'Folk', pattern: 'D.DU.UDU', beatsPerMeasure: 4 },
        'pop-rock': { name: 'Pop Rock', pattern: 'D.DU.UDU', beatsPerMeasure: 4 },
        'country': { name: 'Country', pattern: 'D..D.U.U', beatsPerMeasure: 4 },
        'reggae': { name: 'Reggae', pattern: '.D.U.D.U', beatsPerMeasure: 4 },
        'ballad': { name: 'Ballad 6/8', pattern: 'D..D.U', beatsPerMeasure: 6 },
        'waltz': { name: 'Waltz 3/4', pattern: 'D..D.U', beatsPerMeasure: 3 },
        'blues': { name: 'Blues Shuffle', pattern: 'D.xU.xD.xU.x', beatsPerMeasure: 4 },
        'punk': { name: 'Punk', pattern: 'DDDDDDDD', beatsPerMeasure: 4 },
        'arpeggio': { name: 'Arpeggio', pattern: '123456', beatsPerMeasure: 4 }
    },
    
    // Initialize the audio system
    async initialize() {
        if (this.isInitialized) {
            return;
        }
        
        try {
            // Create audio context
            this.audioContext = new (window.AudioContext || window.webkitAudioContext)();
            
            // Load the audio worklet
            try {
                await this.audioContext.audioWorklet.addModule('/js/guitar-audio-worklet.js');
                console.log('Guitar worklet module loaded successfully');
            } catch (error) {
                console.error('Failed to load guitar worklet:', error);
                throw error;
            }
            
            // Create the audio worklet node
            try {
                this.audioWorkletNode = new AudioWorkletNode(this.audioContext, 'guitar-audio-processor', {
                    numberOfInputs: 0,
                    numberOfOutputs: 1,
                    outputChannelCount: [2]
                });
                console.log('Guitar worklet node created successfully');
            } catch (error) {
                console.error('Failed to create guitar worklet node:', error);
                throw error;
            }
            
            // Create master gain for volume control
            this.masterGainNode = this.audioContext.createGain();
            this.masterGainNode.gain.value = this.masterVolume;
            this.masterGainNode.connect(this.audioContext.destination);

            // Connect worklet through master gain
            this.audioWorkletNode.connect(this.masterGainNode);
            
            // Handle messages from the worklet
            this.audioWorkletNode.port.onmessage = (event) => {
                this.handleWorkletMessage(event.data);
            };
            
            // Wait for worklet to initialize
            return new Promise((resolve, reject) => {
                this.initCallbacks.push({ resolve, reject });
                setTimeout(() => reject(new Error('Worklet initialization timeout')), 5000);
            });
            
        } catch (error) {
            console.error('Failed to initialize audio:', error);
            throw error;
        }
    },
    
    // Handle messages from the audio worklet
    handleWorkletMessage(data) {
        switch (data.type) {
            case 'initialized':
                this.isInitialized = true;
                // Resolve all pending initialization callbacks
                this.initCallbacks.forEach(cb => cb.resolve());
                this.initCallbacks = [];
                console.log('Guitar audio initialized successfully');
                break;
                
            case 'error':
                console.error('Worklet error:', data.error);
                this.initCallbacks.forEach(cb => cb.reject(new Error(data.error)));
                this.initCallbacks = [];
                break;
        }
    },
    
    // Resume audio context (required for user interaction)
    async resume() {
        if (this.audioContext && this.audioContext.state === 'suspended') {
            await this.audioContext.resume();
        }
    },
    
    // Play a chord
    async playChord(fretPositions) {
        await this.ensureInitialized();
        await this.resume();
        
        this.audioWorkletNode.port.postMessage({
            type: 'playChord',
            fretPositions: fretPositions
        });
    },
    
    // Play a single note
    async playNote(stringIndex, fret) {
        await this.ensureInitialized();
        await this.resume();
        
        this.audioWorkletNode.port.postMessage({
            type: 'playNote',
            stringIndex: stringIndex,
            fret: fret
        });
    },
    
    // Stop all sounds
    async stopAll() {
        if (!this.isInitialized) return;
        
        this.audioWorkletNode.port.postMessage({
            type: 'stopAll'
        });
    },
    
    // Set pluck strength (0.0 - 1.0)
    async setPluckStrength(strength) {
        if (!this.isInitialized) return;
        
        this.audioWorkletNode.port.postMessage({
            type: 'setPluckStrength',
            strength: Math.max(0, Math.min(1, strength))
        });
    },
    
    // Set string damping (0.9 - 0.999)
    async setStringDamping(damping) {
        if (!this.isInitialized) return;
        
        this.audioWorkletNode.port.postMessage({
            type: 'setStringDamping',
            damping: Math.max(0.9, Math.min(0.999, damping))
        });
    },
    
    // Ensure the system is initialized
    async ensureInitialized() {
        if (!this.isInitialized) {
            await this.initialize();
        }
    },
    
    // Test tone for debugging
    async playTestTone() {
        console.log('Playing test tone...');
        await this.ensureInitialized();
        await this.resume();
        
        // Create a simple oscillator for testing
        const oscillator = this.audioContext.createOscillator();
        const gainNode = this.audioContext.createGain();
        
        oscillator.connect(gainNode);
        gainNode.connect(this.audioContext.destination);
        
        oscillator.frequency.value = 440; // A4
        gainNode.gain.value = 0.1;
        
        oscillator.start();
        oscillator.stop(this.audioContext.currentTime + 0.5);
        
        console.log('Test tone should be playing');
    },
    
    // Get audio context state
    getState() {
        return {
            initialized: this.isInitialized,
            contextState: this.audioContext ? this.audioContext.state : 'uninitialized'
        };
    },

    // Play metronome click sound
    async playMetronomeClick(isDownbeat = false) {
        await this.ensureInitialized();
        await this.resume();

        const currentTime = this.audioContext.currentTime;

        // Create oscillator for click sound
        const oscillator = this.audioContext.createOscillator();
        const gainNode = this.audioContext.createGain();

        oscillator.connect(gainNode);
        gainNode.connect(this.audioContext.destination);

        // Higher pitch for downbeat (beat 1), lower for other beats
        oscillator.frequency.value = isDownbeat ? 1000 : 800;
        oscillator.type = 'sine';

        // Quick attack, quick decay for click sound
        gainNode.gain.setValueAtTime(0, currentTime);
        gainNode.gain.linearRampToValueAtTime(isDownbeat ? 0.3 : 0.2, currentTime + 0.001);
        gainNode.gain.exponentialRampToValueAtTime(0.001, currentTime + 0.08);

        oscillator.start(currentTime);
        oscillator.stop(currentTime + 0.08);
    },

    // Play strummed chord with timing between strings
    async playChordStrummed(fretPositions, direction = 'down', speed = 30) {
        await this.ensureInitialized();
        await this.resume();

        const stringOrder = direction === 'down'
            ? [0, 1, 2, 3, 4, 5]  // Low to high (standard down strum)
            : [5, 4, 3, 2, 1, 0]; // High to low (up strum)

        for (const stringIndex of stringOrder) {
            if (fretPositions[stringIndex] >= 0) {
                this.audioWorkletNode.port.postMessage({
                    type: 'playNote',
                    stringIndex: stringIndex,
                    fret: fretPositions[stringIndex]
                });
                await new Promise(resolve => setTimeout(resolve, speed));
            }
        }
    },

    // Set master volume (0.0 - 1.0)
    setMasterVolume(volume) {
        this.masterVolume = Math.max(0, Math.min(1, volume));
        if (this.masterGainNode) {
            this.masterGainNode.gain.setValueAtTime(
                this.masterVolume,
                this.audioContext.currentTime
            );
        }
    },

    // Get master volume
    getMasterVolume() {
        return this.masterVolume;
    },

    // Get available strumming patterns
    getStrummingPatterns() {
        return Object.entries(this.strummingPatterns).map(([id, data]) => ({
            id,
            name: data.name,
            pattern: data.pattern,
            beatsPerMeasure: data.beatsPerMeasure
        }));
    },

    // Play a strumming pattern with a chord
    async playStrummingPattern(fretPositions, patternId, bpm = 120, measures = 1) {
        await this.ensureInitialized();
        await this.resume();

        const patternData = this.strummingPatterns[patternId];
        if (!patternData) {
            console.warn('Unknown pattern:', patternId);
            return;
        }

        const pattern = patternData.pattern;
        const beatDuration = 60000 / bpm; // ms per beat
        const subdivisionDuration = beatDuration / 2; // 8th notes

        for (let measure = 0; measure < measures; measure++) {
            for (let i = 0; i < pattern.length; i++) {
                const char = pattern[i];

                switch (char) {
                    case 'D': // Down strum
                        await this.playChordStrummed(fretPositions, 'down', 20);
                        break;
                    case 'U': // Up strum
                        await this.playChordStrummed(fretPositions, 'up', 20);
                        break;
                    case 'x': // Muted strum (percussive)
                        await this.playMutedStrum();
                        break;
                    case '1': case '2': case '3':
                    case '4': case '5': case '6':
                        // Arpeggio: play individual string
                        const stringIndex = parseInt(char) - 1;
                        if (fretPositions[stringIndex] >= 0) {
                            this.audioWorkletNode.port.postMessage({
                                type: 'playNote',
                                stringIndex: stringIndex,
                                fret: fretPositions[stringIndex]
                            });
                        }
                        break;
                    // '.' is a rest, do nothing
                }

                await new Promise(resolve => setTimeout(resolve, subdivisionDuration));
            }
        }
    },

    // Play muted/percussive strum
    async playMutedStrum() {
        if (!this.audioContext) return;

        const currentTime = this.audioContext.currentTime;

        // Create noise for percussive sound
        const bufferSize = this.audioContext.sampleRate * 0.05;
        const buffer = this.audioContext.createBuffer(1, bufferSize, this.audioContext.sampleRate);
        const data = buffer.getChannelData(0);

        for (let i = 0; i < bufferSize; i++) {
            data[i] = (Math.random() * 2 - 1) * 0.3;
        }

        const noise = this.audioContext.createBufferSource();
        noise.buffer = buffer;

        const filter = this.audioContext.createBiquadFilter();
        filter.type = 'bandpass';
        filter.frequency.value = 800;
        filter.Q.value = 1;

        const gainNode = this.audioContext.createGain();
        gainNode.gain.setValueAtTime(0.15, currentTime);
        gainNode.gain.exponentialRampToValueAtTime(0.001, currentTime + 0.05);

        noise.connect(filter);
        filter.connect(gainNode);
        gainNode.connect(this.masterGainNode);

        noise.start(currentTime);
        noise.stop(currentTime + 0.05);
    },

    // Stop any playing pattern
    stopPattern() {
        this._patternStopped = true;
    }
};