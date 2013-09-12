
MusicTab.namespace('MusicTab.Utils');

MusicTab.Utils.FileReader = klass(null, {
    __construct: function () {
		this.init();
    },

    init: function () {
	},
	
    read: function (file, success, error) {
        try {
            var data = this._b64toBlob(file);
            success(data);
        }catch (err) {
            error(err);
        }
    },
    
    _b64toBlob: function (str) {
        var decoded = atob(str);
        var i, il = decoded.length;
        var array = [];

        for (i = 0; i < il; ++i) {
            array.push(decoded.charCodeAt(i));
        }
        
        return array;
    },
   
    /*_b64toBlob: function (b64Data, contentType, sliceSize) {
        contentType = contentType || '';
        sliceSize = sliceSize || 1024;

        function charCodeFromCharacter(c) {
            return c.charCodeAt(0);
        }

        var byteCharacters = atob(b64Data);
        
        var byteArrays = [];

        for (var offset = 0; offset < byteCharacters.length; offset += sliceSize) {
            var slice = byteCharacters.slice(offset, offset + sliceSize);
            var byteNumbers = Array.prototype.map.call(slice, charCodeFromCharacter);
            var byteArray = new Uint8Array(byteNumbers);

            byteArrays.push(byteArray);
        }

        var blob = new Blob(byteArrays, { type: contentType });
        return blob;
    }*/

    /*read: function(file, success, error){
		// chrome & FF
		if( typeof FileReader === 'function'){
			this._readAllBytes(file, success, error);
		} 
		// IE
		else if(typeof ActiveXObject=== 'function'){
			this._readAllBytesIE(file, success, error);

		}
	},
	
	_readAllBytes: function(path, success, error){
		var reader = new FileReader();
		reader.onloadend = function(evt) {
		  if (evt.target.readyState == FileReader.DONE) { 
		  // TODO avoid conversion
			var result = evt.target.result;
			var array = [];
			for (n = 0; n < result.length; ++n) {
				array.push(result.charCodeAt(n));
			}
			success(array);
		  } 
		  else 
			error();
		};
		var blob = path.slice(0, path.size);
		reader.readAsBinaryString(blob);
	},
	
	 _readAllBytesIE: function(path, success, error) {
        var fso = new ActiveXObject("Scripting.FileSystemObject"),
            ts = fso.OpenTextFile(path, 1), array = [];
        while (!ts.AtEndOfStream)
            array.push(ts.Read(1).charCodeAt(0));

        ts.Close();
		success(array);
    }
	*/
	/*getBinaryFromXHR: function(responseText, xhr) {
        var result = "";
        for (var i = 0; i < responseText.length; i++) {
            var code = responseText.charCodeAt(i) & 0xff;
            result += String.fromCharCode(code);
        }
        return result;
    },*/
	
});