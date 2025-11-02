// Simple audio test without AudioWorklet
window.SimpleAudioTest = {
    audioContext: null,
    
    async playSimpleOscillator() {
        try {
            // Create or reuse audio context
            if (!this.audioContext) {
                this.audioContext = new (window.AudioContext || window.webkitAudioContext)();
            }
            
            // Resume if suspended
            if (this.audioContext.state === 'suspended') {
                await this.audioContext.resume();
            }
            
            console.log('Audio context state:', this.audioContext.state);
            
            // Create oscillator
            const oscillator = this.audioContext.createOscillator();
            const gainNode = this.audioContext.createGain();
            
            oscillator.connect(gainNode);
            gainNode.connect(this.audioContext.destination);
            
            oscillator.frequency.value = 440;
            gainNode.gain.value = 0.2;
            
            oscillator.start();
            oscillator.stop(this.audioContext.currentTime + 1);
            
            console.log('Simple oscillator should be playing');
            return true;
        } catch (error) {
            console.error('Error in simple oscillator:', error);
            return false;
        }
    },
    
    async testAudioWorklet() {
        try {
            if (!this.audioContext) {
                this.audioContext = new (window.AudioContext || window.webkitAudioContext)();
            }
            
            // Create a minimal worklet that just generates white noise
            const workletCode = `
                class TestProcessor extends AudioWorkletProcessor {
                    process(inputs, outputs, parameters) {
                        const output = outputs[0];
                        console.log('TestProcessor process called');
                        
                        for (let channel = 0; channel < output.length; channel++) {
                            for (let i = 0; i < output[channel].length; i++) {
                                output[channel][i] = (Math.random() * 2 - 1) * 0.1;
                            }
                        }
                        
                        return true;
                    }
                }
                
                registerProcessor('test-processor', TestProcessor);
            `;
            
            const blob = new Blob([workletCode], { type: 'application/javascript' });
            const workletUrl = URL.createObjectURL(blob);
            
            await this.audioContext.audioWorklet.addModule(workletUrl);
            
            const workletNode = new AudioWorkletNode(this.audioContext, 'test-processor');
            workletNode.connect(this.audioContext.destination);
            
            console.log('Test worklet created and connected');
            
            // Stop after 1 second
            setTimeout(() => {
                workletNode.disconnect();
                URL.revokeObjectURL(workletUrl);
                console.log('Test worklet disconnected');
            }, 1000);
            
            return true;
        } catch (error) {
            console.error('Error in worklet test:', error);
            return false;
        }
    }
};