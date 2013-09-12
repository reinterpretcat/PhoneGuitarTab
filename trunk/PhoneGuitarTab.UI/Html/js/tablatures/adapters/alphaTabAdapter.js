
MusicTab.namespace('MusicTab.Tablatures');

MusicTab.Tablatures.AlphaTabAdapter = klass(MusicTab.Tablatures.TabAdapter, {
    __construct: function (tab, helper, success) {
        this.helper = helper;
        this.init(tab, success);
    },

    init: function (tab, success) {
        var tracks = this.getTracks(tab);

        success({
            header:{
                artist: tab.artist,
                album: tab.album,
                title: tab.title,
                music: tab.music,
                words: tab.words,
                notice: tab.notice,
                tempoName: tab.tempoName
            },
            tracks: tracks
        });
    },
    
    getTracks: function(body) {
        var tracksCount = body.tracks.length;
        // var measureTrackPairsCount = body.measureTrackPairs.length;
        var measureCount = body.measureHeaders.length;
        var tracks = [];
        for (var i = 0; i < tracksCount; i++) {

            var track = new MusicTab.Tablatures.Track();

            if (body.tracks[i].isPercussionTrack) {
                continue;
            }
            // track headers
            track.index = i;
            track.name = body.tracks[i].name;
            track.strings = body.tracks[i].strings; // todo tonality
            track.instrument = this.getInstrument(body.tracks[i].channel._instrument);
            for (var j = 0; j < measureCount; j++) {

                var numerator = body.measureHeaders[j].timeSignature.numerator;
                var denominator = body.measureHeaders[j].timeSignature.denominator.value;
                var beats = this.getBeats(body.tracks[i].measures[j].beats);
                
                track.measures.push({
                    number: j + 1,
                    time: numerator + "/" + denominator,
                    beats: beats,
                    tempo: body.measureHeaders[j].tempo.value,
                    bar: {
                        begin: j == 0,
                        end: j == measureCount - 1,
                        beginRepeat: body.measureHeaders[j].isRepeatOpen,
                        endRepeat: body.measureHeaders[j].repeatClose
                    },
                    width: this.helper.getMeasureWidth(beats)
                    
                });
            }
            tracks.push(track);
        }
        return tracks;

    },
    
    getBeats: function(beats) {
        var tBeats = [];
        var me = this;
        beats.forEach(function(beat) {
            var tBeat = me.getBeat(beat);
            tBeats.push(tBeat);
        });
        return tBeats;
    },

    getBeat: function (beat) {
        
        var notes = [];
        for (var i = 0; i < beat.voices[0].notes.length;i++) {
            var note = {
                fret: beat.voices[0].notes[i].value,
                str: beat.voices[0].notes[i].string,
                effects: {
                    legato: beat.voices[0].notes[i].effect.hammer,
                    slide: beat.voices[0].notes[i].effect.slide,
                    bend: beat.voices[0].notes[i].effect.bend,
                    vibrato: beat.voices[0].notes[i].effect.vibrato,
                    harmonic: beat.voices[0].notes[i].effect.harmonic,
                    trill: beat.voices[0].notes[i].effect.trill,
                    tremolo: beat.voices[0].notes[i].effect.tremoloPicking
                }
            };
            notes.push(note);
        }
        
        return {
            duration: this.getDurationString(beat.voices[0].duration),
            pickStroke: beat.effect.pickStroke,
            tuplet: this.getTuplet(beat),
            tapping: beat.effect.tapping,
            notes: notes,
        };
    },
    
    getTuplet: function (beat) {
        if (beat.voices[0].duration.tuplet.enters == 1) {
            return null;
        }
        return {
            enters: beat.voices[0].duration.tuplet.enters,
            times: beat.voices[0].duration.tuplet.times
        };
    },
});

MusicTab.Tablatures.AlphaTabAdapter.transform = function (tab, helper, success) {
    new MusicTab.Tablatures.AlphaTabAdapter(tab, helper, success);
};