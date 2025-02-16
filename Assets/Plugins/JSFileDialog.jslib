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

      document.addEventListener('click', function () {

        var fileuploader = document.getElementById('fileuploader');
        if (!fileuploader) {
          fileuploader = document.createElement('input');
          fileuploader.setAttribute('style', 'display:none;');
          fileuploader.setAttribute('type', 'file');
          // Only allow json files to be opened. For more info, see
          // https://developer.mozilla.org/en-US/docs/Web/HTML/Element/input/file#accept.
          fileuploader.setAttribute('accept', '.json');
          fileuploader.setAttribute('id', 'fileuploader');
          fileuploader.setAttribute('class', '');
          document.getElementsByTagName('body')[0].appendChild(fileuploader);

          fileuploader.onchange = function (e) {
            var files = e.target.files;
            for (var i = 0, f; f = files[i]; i++) {
              // window.alert(URL.createObjectURL(f));
              // Note that the first argument of SendMessage must be the name of
              // the gameobject the message will be sent to.
              SendMessage('FileDialogManager', 'FileDialogResult', URL.createObjectURL(f));
            }
          };
        }
        if (fileuploader.getAttribute('class') == 'focused') {
          fileuploader.setAttribute('class', '');
          fileuploader.click();
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
      var fileuploader = document.getElementById('fileuploader');
      if (fileuploader) {
        fileuploader.setAttribute('class', 'focused');
      }
    }
  }
);

// This one writes to the disk. Derived from
// https://forum.unity.com/threads/webgl-read-write.336171/#post-2678039.
mergeInto(
  LibraryManager.library,
  {
    WriteToDisk_JS: function (rawFileName, rawMsg) {
      var msg = Pointer_stringify(rawMsg);
      var fn = Pointer_stringify(rawFileName);

      var data = new Blob([msg], { type: 'text/plain' });
      var link = document.createElement('a');
      link.setAttribute('download', fn);
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

// Save as file dialog.
mergeInto(
  LibraryManager.library,
  {
    /// rawFileName includes the extension!
    DownloadFileDialog_JS: function (rawFileName, rawContents) {
      const fileName = Pointer_stringify(rawFileName);
      const contents = Pointer_stringify(rawContents);
      const data = new Blob([contents], { type: 'text/plain' });

      // There is another function I can use here, which is showSaveFilePicker.
      // However, this function is experimental right now and only works on
      // HTTPS connections on Chromium browsers, so I can't test it. Here's a
      // website showing it in action though: https://save-canvas-as.glitch.me/.
      var filesaver = document.getElementById('filesaver');

      if (!filesaver) {
        filesaver = document.createElement('a');
        filesaver.id = 'filesaver';
      }

      filesaver.href = URL.createObjectURL(data);
      filesaver.download = fileName;
      filesaver.click();
    }
  }
)