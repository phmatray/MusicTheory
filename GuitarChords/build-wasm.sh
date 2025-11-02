#!/bin/bash

# Build WASM files
echo "Building WASM files..."
cd ../guitar-audio-synth
export PATH="$HOME/.cargo/bin:$PATH"
wasm-pack build --target web --out-dir pkg

# Create wasm directory in wwwroot
echo "Creating WASM directory..."
mkdir -p ../GuitarChords/wwwroot/wasm

# Copy WASM files
echo "Copying WASM files..."
cp pkg/guitar_audio_synth_bg.wasm ../GuitarChords/wwwroot/wasm/
cp pkg/guitar_audio_synth.js ../GuitarChords/wwwroot/wasm/

echo "WASM build complete!"