// This Javascript Library interacts with the browser and the C# code from
// Unity. It is required as WebGL does not allow you to open files without the
// user clicking on something first. I only have limited experience with
// JavaScript, so I won't pretend that I know what this is doing. Also, this
// code is taken from https://stackoverflow.com/a/58489273.
mergeInto(
  LibraryManager.library,
  {
    AddClickListenerForFileDialog: function () {
      // window.alert('Add click listener');

      document.addEventListener('click', function (accept) {

        var file_uploader = document.getElementById('file_uploader');
        if (!file_uploader) {
          file_uploader = document.createElement('input');
          file_uploader.setAttribute('style', 'display:none;');
          file_uploader.setAttribute('type', 'file');
          // Only allow json files to be opened. For more info, see
          // https://developer.mozilla.org/en-US/docs/Web/HTML/Element/input/file#accept.
          file_uploader.setAttribute('accept', accept);
          file_uploader.setAttribute('id', 'file_uploader');
          file_uploader.setAttribute('class', '');
          document.getElementsByTagName('body')[0].appendChild(file_uploader);

          file_uploader.onchange = function (e) {
            var files = e.target.files;
            for (var i = 0, f; f = files[i]; i++) {
              // window.alert(URL.createObjectURL(f));
              // Note that the first argument of SendMessage must be the name of
              // the gameobject the message will be sent to.
              SendMessage('FileDialogManager', 'FileDialogResult', URL.createObjectURL(f));
            }
          };
        }
        if (file_uploader.getAttribute('class') == 'focused') {
          file_uploader.setAttribute('class', '');
          file_uploader.click();
        }
      });
    }
  }
);

// Also need this. Call this from OnPointerDown (in the Unity C# script).
mergeInto(
  LibraryManager.library,
  {
    FocusFileUploader: function () {
      var file_uploader = document.getElementById('file_uploader');
      if (file_uploader) {
        file_uploader.setAttribute('class', 'focused');
      }
    }
  }
);

// This one writes text to the disk. Derived from
// https://forum.unity.com/threads/webgl-read-write.336171/#post-2678039.
mergeInto(
  LibraryManager.library,
  {
    WriteTextToDisk_JS: function (rawFileName, rawContents) {
      const contents = Pointer_stringify(rawContents);
      const fileName = Pointer_stringify(rawFileName);
      const data = new Blob([contents], { type: 'text/plain' });

      var link = document.createElement('a');
      link.setAttribute('download', fileName);
      link.setAttribute('style', 'display:none;');
      link.setAttribute('id', 'TextDownloader');

      if (window.webkitURL != null) {
        link.href = window.webkitURL.createObjectURL(data);
      }
      else {
        link.href = window.URL.createObjectURL(data);
        link.onclick = function () {
          var child = document.getElementById('TextDownloader');
          child.parentNode.removeChild(child);
        };
        link.setAttribute('style', 'display:none;');
        document.body.appendChild(link);
      }
      link.click();
    }
  }
);

// This one writes a byte stream to the disk. Derived from
// https://discussions.unity.com/t/webgl-saving-a-file-in-the-browser/684252/6.
mergeInto(
  LibraryManager.library,
  {
    WriteBytesToDisk_JS: function (rawFileName, rawContents) {
      function fixBinary(bin) {
        var length = bin.length;
        var buf = new ArrayBuffer(length);
        var arr = new Uint8Array(buf);
        for (var i = 0; i < length; i++) {
          arr = bin.charCodeAt(i);
        }
        return buf;
      }

      const fileName = Pointer_stringify(rawFileName);
      const contents = fixBinary(atob(Pointer_stringify(rawContents)));
      const data = new Blob([contents], { type: 'application/octet-stream' });

      var link = document.createElement('a');
      link.setAttribute('download', fileName);
      link.setAttribute('style', 'display:none;');
      link.setAttribute('id', 'BytesDownloader');

      if (window.webkitURL != null) {
        link.href = window.webkitURL.createObjectURL(data);
      }
      else {
        link.href = window.URL.createObjectURL(data);
        link.onclick = function () {
          var child = document.getElementById('BytesDownloader');
          child.parentNode.removeChild(child);
        };
        link.setAttribute('style', 'display:none;');
        document.body.appendChild(link);
      }
      link.click();
    }
  }
);

// Save as file dialog.
mergeInto(
  LibraryManager.library,
  {
    /// rawFileName includes the extension!
    DownloadTextDialog_JS: function (rawFileName, rawContents) {
      const fileName = Pointer_stringify(rawFileName);
      const contents = Pointer_stringify(rawContents);
      const data = new Blob([contents], { type: 'text/plain' });

      // There is another function I can use here, which is showSaveFilePicker.
      // However, this function is experimental right now and only works on
      // HTTPS connections on Chromium browsers, so I can't test it. Here's a
      // website showing it in action though: https://save-canvas-as.glitch.me/.
      var fileSaver = document.getElementById('file_saver');

      if (!fileSaver) {
        fileSaver = document.createElement('a');
        fileSaver.id = 'file_saver';
      }

      fileSaver.href = URL.createObjectURL(data);
      fileSaver.download = fileName;
      fileSaver.click();
    }
  }
);

// Save as file dialog.
mergeInto(
  LibraryManager.library,
  {
    /// rawFileName includes the extension!
    DownloadBytesDialog_JS: function (rawFileName, rawContents) {
      
      function fixBinary(bin) {
        var length = bin.length;
        var buf = new ArrayBuffer(length);
        var arr = new Uint8Array(buf);
        for (var i = 0; i < length; i++) {
          arr = bin.charCodeAt(i);
        }
        return buf;
      }

      const fileName = Pointer_stringify(rawFileName);
      const contents = fixBinary(atob(Pointer_stringify(rawContents)));
      const data = new Blob([contents], { type: 'application/octet-stream' });

      // There is another function I can use here, which is showSaveFilePicker.
      // However, this function is experimental right now and only works on
      // HTTPS connections on Chromium browsers, so I can't test it. Here's a
      // website showing it in action though: https://save-canvas-as.glitch.me/.
      var fileSaver = document.getElementById('file_saver');

      if (!fileSaver) {
        fileSaver = document.createElement('a');
        fileSaver.id = 'file_saver';
      }

      fileSaver.href = URL.createObjectURL(data);
      fileSaver.download = fileName;
      fileSaver.click();
    }
  }
);