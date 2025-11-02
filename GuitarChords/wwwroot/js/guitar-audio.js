// Guitar Audio Manager - Main thread JavaScript
window.GuitarAudio = {
    audioContext: null,
    audioWorkletNode: null,
    isInitialized: false,
    initCallbacks: [],
    
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
            
            // Connect to speakers
            this.audioWorkletNode.connect(this.audioContext.destination);
            
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
    }
};