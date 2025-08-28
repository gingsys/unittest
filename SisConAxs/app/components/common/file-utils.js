export const FileUtils = new class {

    downloadBlob(blob, name = 'file.txt') {
        // Convert your blob into a Blob URL (a special url that points to an object in the browser's memory)
        const blobUrl = URL.createObjectURL(blob);

        // Create a link element
        const link = document.createElement("a");

        // Set link's href to point to the Blob URL
        link.href = blobUrl;
        link.download = name;
        link.hidden = true;

        // Append link to the body
        document.body.appendChild(link);
      
        // Dispatch click event on the link
        // This is necessary as link.click() does not work on the latest firefox
        link.dispatchEvent(
            new MouseEvent('click', { 
                bubbles: true,
                cancelable: true,
                view: window
            })
        );

        // Remove link from body
        document.body.removeChild(link);
        URL.revokeObjectURL(blobUrl);
    }

    downloadFile(file) {
        file.arrayBuffer().then((arrayBuffer) => {
            const blob = new Blob([new Uint8Array(arrayBuffer)], {type: file.type });
            this.downloadBlob(blob, file.name);
        });
    }
}