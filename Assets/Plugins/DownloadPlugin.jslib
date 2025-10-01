var DownloadPlugin = {
    DownloadFile: function(array, size, fileNamePtr) {
        var fileName = UTF8ToString(fileNamePtr);
        var bytes = new Uint8Array(size);
        for (var i = 0; i < size; i++) {
            bytes[i] = HEAPU8[array + i];
        }
        var blob = new Blob([bytes], { type: "image/png" });
        var link = document.createElement('a');
        link.href = window.URL.createObjectURL(blob);
        link.download = fileName;
        link.click();
    }
};
mergeInto(LibraryManager.library, DownloadPlugin);