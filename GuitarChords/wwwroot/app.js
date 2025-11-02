// Function to download a file from base64 data
window.downloadFile = (filename, base64Data, mimeType) => {
    const byteCharacters = atob(base64Data);
    const byteNumbers = new Array(byteCharacters.length);
    
    for (let i = 0; i < byteCharacters.length; i++) {
        byteNumbers[i] = byteCharacters.charCodeAt(i);
    }
    
    const byteArray = new Uint8Array(byteNumbers);
    const blob = new Blob([byteArray], { type: mimeType });
    
    const link = document.createElement('a');
    link.href = window.URL.createObjectURL(blob);
    link.download = filename;
    
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    
    window.URL.revokeObjectURL(link.href);
};

// Keyboard shortcuts handling
let keyboardShortcutHandler = null;

window.registerKeyboardShortcuts = (dotNetRef) => {
    keyboardShortcutHandler = (e) => {
        // Check if user is typing in an input field
        if (e.target.tagName === 'INPUT' || e.target.tagName === 'TEXTAREA') {
            return;
        }
        
        let shortcut = '';
        
        // Build shortcut string
        if (e.ctrlKey || e.metaKey) shortcut += 'Ctrl+';
        if (e.altKey) shortcut += 'Alt+';
        if (e.shiftKey) shortcut += 'Shift+';
        
        // Handle special keys
        switch(e.key) {
            case ' ':
                shortcut += 'Space';
                break;
            case 'Enter':
                shortcut += 'Enter';
                break;
            case 'Escape':
                shortcut += 'Escape';
                break;
            case 'ArrowUp':
                shortcut += 'Up';
                break;
            case 'ArrowDown':
                shortcut += 'Down';
                break;
            case 'ArrowLeft':
                shortcut += 'Left';
                break;
            case 'ArrowRight':
                shortcut += 'Right';
                break;
            default:
                if (e.key.length === 1) {
                    shortcut += e.key.toUpperCase();
                }
                break;
        }
        
        // Define shortcuts
        const shortcuts = [
            'Ctrl+K', // Search
            'Ctrl+P', // Practice mode
            'Ctrl+C', // Compare chords
            'Ctrl+E', // Export
            'Space',  // Play/Pause
            'Escape', // Close dialog
            'Left',   // Previous chord
            'Right',  // Next chord
            'G',      // Grid view
            'L',      // List view
            'F',      // Toggle filters
            'P',      // Toggle progression
            '?'       // Help
        ];
        
        if (shortcuts.includes(shortcut)) {
            e.preventDefault();
            dotNetRef.invokeMethodAsync('HandleKeyPress', shortcut);
        }
    };
    
    document.addEventListener('keydown', keyboardShortcutHandler);
};

window.unregisterKeyboardShortcuts = () => {
    if (keyboardShortcutHandler) {
        document.removeEventListener('keydown', keyboardShortcutHandler);
        keyboardShortcutHandler = null;
    }
};