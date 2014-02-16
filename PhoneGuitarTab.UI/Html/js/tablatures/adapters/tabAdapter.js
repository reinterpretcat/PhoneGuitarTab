MusicTab.namespace('MusicTab.Tablatures');

MusicTab.Tablatures.TabAdapter = klass(null, {
    __construct: function () {
    },
	
	getDurationString: function(duration) {
        var result = "";
        switch (duration.value) {
            case 8:
                result = "8";
                break;
            case 16:
                result = "16";
                break;
            case 32:
                result = "32";
                break;
            case 64:
                result = "64";
                break;
            case 128:
                result = "128";
                break;
            case 2:
                result = "h";
                break;
            case 1:
                result = "w";
                break;
            default:
                result = "q";
        }
        if (duration.isDotted) {
            result += "d";
        }
        return result;
    },
	
	getInstrument: function(number) {
        var instrument = "";
        switch (number) {
            case 0:
                instrument = "acoustic grand piano";
                break;
            case 1:
                instrument = "bright acoustic piano";
                break;
            case 2:
                instrument = "electric grand piano";
                break;
            case 24:
                instrument = "acoustic guitar nylon";
                break;
            case 25:
                instrument = "acoustic guitar steel";
                break;
            case 26:
                instrument = "electric guitar jazz";
                break;
            case 27:
                instrument = "electric guitar clean";
                break;
            case 28:
                instrument = "electric guitar muted";
                break;
            case 29:
                instrument = "overdriven guitar";
                break;
            case 30:
                instrument = "distortion guitar";
                break;
            case 31:
                instrument = "guitar harmonics";
                break;
            case 32:
                instrument = "acoustic bass";
                break;
            case 33:
                instrument = "electric bass finger";
                break;
            case 34:
                instrument = "electric bass pick";
                break;
            case 35:
                instrument = "fretless bass";
                break;
            case 48:
                instrument = "stringensemble 1";
                break;
            case 49:
                instrument = "stringensemble 2";
                break;
            case 50:
                instrument = "synth strings 1";
                break;
            case 51:
                instrument = "synth strings 2";
                break;
        }

        return instrument;
    }
	
});