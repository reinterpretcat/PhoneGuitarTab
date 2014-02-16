
MusicTab.namespace('MusicTab.Tablatures');

MusicTab.Tablatures.UgTabAdapter = klass(MusicTab.Tablatures.TabAdapter, {
    __construct: function (title, tab, helper, success) {
        this.helper = helper;
        this.init(title, tab, success);
    },

    init: function (title, tab, success) {
        var tracks = this.getTracks(tab);

        success({
            header: {
                artist: tab.artist || "",
                album: tab.album || "",
                title: tab.name || title,
                music: tab.music || "",
                words: tab.words || "",
                notice: tab.notice || "",
                tempoName: tab.tempoName || ""
            },
            tracks: tracks
        });
    },

    getTracks: function (body) {
        var tracksCount = body.tracks.length;
        var measureCount = body.tracks[0].measures.length;

        var tracks = [];
        for (var i = 0; i < tracksCount; i++) {
            var track = new MusicTab.Tablatures.Track();

            if (body.tracks[i].percussion) {
                continue;
            }
            // track headers
            track.index = i;
            track.name = body.tracks[i].name;
            track.strings = body.tracks[i].strings; // todo tonality
            track.instrument = this.getInstrument(body.tracks[i].midiindex);

            var numerator;
            var denominator;
            var tempo;
            for (var j = 0; j < measureCount; j++) {
                numerator = body.tracks[i].measures[j].numerator || numerator;
                denominator = body.tracks[i].measures[j].denominator || denominator;
                tempo = body.tracks[i].measures[j].tempo || tempo;

                var beats = this.getBeats(body.tracks[i].measures[j].beats);

                track.measures.push({
                    number: j + 1,
                    time: numerator + "/" + denominator,
                    beats: beats,
                    tempo: tempo,
                    bar: {
                        begin: j == 0,
                        end: j == measureCount - 1,
                        beginRepeat: body.tracks[i].measures[j].repeatopen,
                        endRepeat: body.tracks[i].measures[j].repeatcount
                    },
                    width: this.helper.getMeasureWidth(beats)

                });
            }
            tracks.push(track);
        }
        return tracks;
    },


    getBeats: function (beats) {
        var tBeats = [];
        var me = this;
        beats.forEach(function (beat) {
            var tBeat = me.getBeat(beat);
            tBeats.push(tBeat);
        });
        return tBeats;
    },

    getBeat: function (beat) {

        var notes = [];
        if (beat.voices[0].notes) {
            for (var i = 0; i < beat.voices[0].notes.length; i++) {
                var effects = this.getEffects(beat.voices[0].notes[i].effect);
                var note = {
                    fret: beat.voices[0].notes[i].fret,
                    str: beat.voices[0].notes[i].string,
                    effects: this.getEffects(beat.voices[0].notes[i])
                };
                notes.push(note);
            }
        }

        return {
            duration: this.getDurationString(beat.voices[0].duration, beat.voices[0].dotted),
            tuplet: this.getTuplet(beat),
            /* pickStroke: beat.effect.pickStroke,
             tapping: beat.effect.tapping,*/
            notes: notes,
        };
    },

    getEffects: function (note) {
        var flag = note.effect;

        var effects = {};

        if (flag == 4) {
            effects.bend = {
                value: ""
            };
        }

        if (flag == 8) {
            effects.legato = {
                hammer: true
            };
        }

        if (flag == 512) {
            /*effects.legato = {
                hammer: false
            };*/
        }

        if (flag == 2048) {
            effects.vibrato = true;
        }

        if (flag == 16) {
            effects.slide = true;
        }

        if (note.harmonic) {
            effects.harmonic = {};
            effects.harmonic.type = note.harmonic.type == 1 ? "" : note.harmonic.type;
        }

        if (flag == 32768) {
            effects.tremolo = true;
        }

        return effects;
    },

    getTuplet: function (beat) {
        if (!beat.voices[0].tuplet) {
            return null;
        }
        return {
            times: beat.voices[0].tuplet-1
        };
    },

    getDurationString: function (duration, dotted) {
        var result = "";
        switch (duration) {
            case 1:
                result = "8";
                break;
            case 2:
                result = "16";
                break;
            case 3:
                result = "32";
                break;
            case 4:
                result = "64";
                break;
            case 5:
                result = "128";
                break;
            case 255:
                result = "h";
                break;
            case 254:
                result = "w";
                break;
            default:
                result = "q";
        }
        if (dotted) {
            result += "d";
        }
        return result;
    }

});

MusicTab.Tablatures.UgTabAdapter.transform = function (title, tab, helper, success) {
    new MusicTab.Tablatures.UgTabAdapter(title, tab, helper, success);
};