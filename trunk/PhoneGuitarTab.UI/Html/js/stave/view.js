MusicTab.namespace('MusicTab.Stave.View');

// TODO need refactoring:
// a lot of conditional logic

MusicTab.Stave.View = klass(null, {
    __construct: function (params) {
        this.init(params.selector, params.page, params.track, params.context);
    },

    init: function (selector, page, track, context) {

        this.page = page;
        this.width = context.width;
        this.height = context.height;
        this.scale = context.scale;
        this.stringCount = track.strings.length >= 4 && track.strings.length <= 6 ? track.strings.length : 6; // 4-6 is valid for vexflow

        this.staveHelper = new MusicTab.Stave.Helper({
            height: this.height,
            width: this.width,
            scale: this.scale
        });

        // canvas
        this.canvas = $('<canvas></canvas>').addClass("vex-canvas");
        $(selector).append(this.canvas);
        this.renderer = new Vex.Flow.Renderer(this.canvas[0], Vex.Flow.Renderer.Backends.CANVAS);

        // initilize context
        this.ctx_sel = $(selector).find(".vex-canvas");
        this.ctx = this.renderer.getContext();
        this.ctx.setBackgroundFillStyle(this.ctx_sel.css("background-color"));
        this.ctx.scale(this.scale, this.scale);


        this.message = $(selector).attr("label");

        this.music = new Vex.Flow.Music();
        this.valid = false;
        this.offsetY = this.staveHelper.getLineGap();
        this.offsetX = 20;
        
        if(this.stringCount == 5) {
            this.tuning = new Vex.Flow.Tuning("B/1,E/2,A/2,D/3,G/3");
        } else if (this.stringCount == 4) {
            this.tuning = new Vex.Flow.Tuning("E/2,A/2,D/3,G/3");
        }else{
            this.tuning = new Vex.Flow.Tuning();
        }      
    },

    isValid: function () { return this.valid; },

    getElements: function () {
        return this.elements;
    },

    resize: function (width, height) {
        this.renderer.resize(width, height);
        this.ctx = this.renderer.getContext();
        this.ctx.scale(this.scale, this.scale);
    },

    show: function () {

        this.resize(this.width, this.height);
        this.ctx.clear();
        this.ctx.setFont("Arial", 10, "");

        var hasStandardNotation = true;
        var hasTablature = true;
        var staveClef = this.stringCount < 6 ? "bass" : "treble";
        var staveKey = "C";
        var staveTime = this.page[0][0].number == 1 ? "" : this.page[0][0].time;
        var tempo = "";
        for (var i = 0; i < this.page.length; i++) {
            var line = this.page[i];

            var staves = null;
            // parse measure
            for (var j = 0; j < line.length; j++) {

                var measure = line[j];
                staveTime = (staveTime != measure.time) && measure.time ? measure.time : "";
                tempo = (tempo != measure.tempo) && measure.tempo ? measure.tempo : "";
                
                // gen tabstave
                try {
                    staves = this.genTabStave({
                        measure: measure,
                        notation: hasStandardNotation,
                        tablature: hasTablature,
                        clef: staveClef,
                        keySignature: staveKey,
                        timeSignature: staveTime,
                        tempo: tempo,
                        keyManager: new Vex.Flow.KeyManager(staveKey),
                        firstOnLine: j == 0,
                        lastOnLine: j == (line.length - 1)
                    });
                }catch(err) {
                    console.log(err.message);
                }
                staveTime = measure.time;
                tempo = measure.tempo;
            }

            if (staves) {
                // increment y offset
                this.offsetY += (hasTablature ? staves.tabstave.getHeight() : null) + (hasStandardNotation ? staves.notestave.getHeight() : null)
                    + this.staveHelper.getLineGap();
            }
            this.offsetX = 20;

        }
        
        if (this.message) {
            this.ctx.setFont("Times", 12, "italic");
            this.ctx.fillText(this.message, (this.width / (2 * this.scale)) - 10 * this.scale, this.height / this.scale - 15);
        }

        return this;
    },

    genTabStave: function (params) {
       
        var notestave = null;
        var tabstaveStartX = 40; // line up tab and note staves

        if (params) {
            notation = params.notation;
            tablature = params.tablature;
            clef = params.clef;
            keySignature = params.keySignature;
            timeSignature = params.timeSignature;
        }

        var measureWidth = params.measure.adjustedWidth;
        if (params.lastOnLine && !params.measure.bar.end) {
            measureWidth = this.staveHelper.getLastLineMeasureLength(this.offsetX);
        }

        if (notation) {
            notestave = new Vex.Flow.Stave(this.offsetX, this.offsetY);
            notestave.setMeasure(params.measure.number);
            tabstaveStartX = notestave.getNoteStartX();
        }

        var tabstave = tablature ?
            new Vex.Flow.TabStave(this.offsetX,
                notation ? notestave.getHeight() + this.offsetY : this.offsetY,null,{num_lines : this.stringCount}).setNoteStartX(tabstaveStartX) : null;

        var measureNotes = this.genMeasure(params.measure, params);

        if (params.firstOnLine) {
            notestave.addClef(clef);
            tabstave.addTabGlyph();
           // measureWidth += 50;
        }

        // should redefine time
        if (timeSignature != "") {
            notestave.addTimeSignature(params.timeSignature);

            tabstave.addTimeSignature(params.timeSignature);
            //measureWidth += 30;
        }
        
        if (params.tempo != "") {
            notestave.setTempo({ duration: "q", dots: 0, bpm: params.tempo }, 0);
        }

        if (params.measure.bar.end) {
            notestave.setEndBarType(Vex.Flow.Barline.type.DOUBLE);
            tabstave.setEndBarType(Vex.Flow.Barline.type.DOUBLE);
            measureWidth -= 10; // TODO
        }
        
        if(params.measure.bar.beginRepeat) {
            notestave.setBegBarType(Vex.Flow.Barline.type.REPEAT_BEGIN);
            tabstave.setBegBarType(Vex.Flow.Barline.type.REPEAT_BEGIN);
        }
        
        if (params.measure.bar.endRepeat) {
            notestave.setEndBarType(Vex.Flow.Barline.type.REPEAT_END);
            tabstave.setEndBarType(Vex.Flow.Barline.type.REPEAT_END);
        }

        if (tabstave) {
            tabstave.setWidth(measureWidth);
            tabstave.setContext(this.ctx).draw();
        }

        if (notestave) {
            notestave.setWidth(measureWidth);
            notestave.setContext(this.ctx).draw();
        }

        if (tabstave && notestave) {
            this.formatAndDrawTab(
                 this.ctx,
                 tabstave,
                 notestave,
                 measureNotes.tabnotes,
                 measureNotes.notes, true, params.firstOnLine);
        }
        else {

            if (tabstave) {
                if (measureNotes.tabnotes)
                    Vex.Flow.Formatter.FormatAndDraw(this.ctx, tabstave, measureNotes.tabnotes);
            }
            if (notestave) {
                if (measureNotes.notes)
                    Vex.Flow.Formatter.FormatAndDraw(
                        this.ctx, notestave, measureNotes.notes);
            }
        }
               
        // Draw tuplets
        for (var j = 0; j < measureNotes.tuplets.length; ++j) {
            measureNotes.tuplets[j].setContext(this.ctx).draw();
        }

        // Draw stave ties
        for (var j = 0; j < measureNotes.ties.staveTies.length; ++j) {
            measureNotes.ties.staveTies[j].setContext(this.ctx).draw();
        }
        
        // Draw tab ties
        for (var j = 0; j < measureNotes.ties.tabTies.length; ++j) {
            measureNotes.ties.tabTies[j].setContext(this.ctx).draw();
        }

        this.offsetX += notestave.width;

        return {
            notestave: notestave,
            tabstave: tabstave
        };
    },

    genMeasure: function (measure, params) {
        
        var positions = [];
        var durations = [];
        for (var k = 0; k < measure.beats.length; k++) {
            var position = measure.beats[k];

            // TODO rest note
           // if (position.notes.length == 0) {
           //     continue;
           // }

            positions.push(position);
            durations.push(position.duration);
        }

        var result = this.genNotes(positions, durations, params);
        var value = this.genTuplets(result.notes, result.tabnotes);

        result.beams = value.beams;
        result.tuplets = value.tuplets;
        result.ties = this.genTies(positions, result.notes, result.tabnotes);


        return result;
    },
    
    genNotes: function (positions, durations, params) {

        var hasNotation = true;
        
        var tabnotes = [];
        var notes = [];

        for (var i = 0; i < positions.length; ++i) {
            var position = positions[i].notes;
            var duration = durations[i];

            // isn't rest note
            if (position.length != 0) {
                var tabnote = new Vex.Flow.TabNote(
                    { positions: position, duration: duration });

                //process bend
                this.genBends(position, tabnote);
                this.genVibrato(position, tabnote);
                
                tabnotes.push(tabnote);


                if (hasNotation) {
                    var keys = [];
                    var accidentals = [];

                    for (var j = 0; j < position.length; ++j) {
                        var notefret = position[j];
                        var spec = this.tuning.getNoteForFret(notefret.fret, notefret.str);
                        var specProps = Vex.Flow.keyProperties(spec);

                        var selectedNote = params.keyManager.selectNote(specProps.key);

                        if (selectedNote.change) {
                            if (selectedNote.accidental == null)
                                accidentals.push("n");
                            else accidentals.push(selectedNote.accidental);
                        } else {
                            accidentals.push(null);
                        }

                        var newNote = selectedNote.note;
                        var newOctave = specProps.octave;

                        if (this.music.getNoteParts(selectedNote.note).root == "b" &&
                            this.music.getNoteParts(specProps.key).root == "c") {
                            newOctave--;
                        } else if (this.music.getNoteParts(selectedNote.note).root == "c" &&
                            this.music.getNoteParts(specProps.key).root == "b") {
                            newOctave++;
                        }

                        var newSpec = newNote + "/" + newOctave;
                        keys.push(newSpec);
                    }

                    var note = new Vex.Flow.StaveNote({
                        keys: keys,
                        duration: duration,
                        clef: params.clef
                    });

                    if (duration.endsWith("d")) {
                        note.addDot(0);
                    }

                    for (var j = 0; j < accidentals.length; ++j) {
                        var acc = accidentals[j];

                        if (acc) {
                            note.addAccidental(j, new Vex.Flow.Accidental(accidentals[j]));
                        }
                    }

                    note.m_tuplet = positions[i].tuplet;
                    notes.push(note);
                }
                
                this.genHarmonics(position, note, tabnote);
                this.genArticulations(position, positions[i], note);
                this.genAnnotations(positions[i], note, tabnote);
            }
            
            // rest note
            else {
                var restKey = params.clef == "bass" ? "d/3" : "b/4"; // TODO
                note = new Vex.Flow.StaveNote({
                    keys: [restKey],
                    type: "r",
                    duration: duration,
                    clef: params.clef
                });
                notes.push(note);
                tabnotes.push(new Vex.Flow.TabRestNote({positions:[{fret:0, toString:0}], duration:duration}));
            }
        }
        return {
            notes: notes,
            tabnotes: tabnotes
        };
    },
    
    genBends: function (position, tabnote) {
        if (position[0].effects.bend) {
            // TODO bend type
            var release = false;//position[0].effects.bend.type == 2;
            var text = "";
            switch(position[0].effects.bend.value) {
                case 100:
                    text = "full";
                    break;
                case 50:
                    text = "1/2";
                    break;
                case 25:
                    text = "1/4";
                    break;
            }
            var bend = new Vex.Flow.Bend(text, release);
            tabnote.addModifier(bend, 0);
        }
    },
    
    genVibrato:function (position, tabnote) {
        if (position[0].effects.vibrato) {
            //note.addModifier(new Vex.Flow.Vibrato(), 0);
            tabnote.addModifier(new Vex.Flow.Vibrato(), 0);
        }      
    },
    
    genHarmonics: function (position, note, tabnote) {
        if (position[0].effects.harmonic) {
            var text = position[0].effects.harmonic.type == 0 ? "N.H." : "A.H.";
            tabnote.addModifier(new Vex.Flow.Annotation(text), 0);
            // see stavenote.js
            note.glyph = Vex.Flow.durationToGlyph(note.duration, "h");
        }
    },
    
    genArticulations: function (position, beat, note) {
        if (beat.pickStroke) {
            var sym = beat.pickStroke == 1? "a|":"am";
            note.addArticulation(0, new Vex.Flow.Articulation(sym).setPosition(0));
        }

        if (position[0].effects.tremolo) {
            // TODO pass number based on duration
            note.setStemDirection(-1); //NOTE will be ignored
            note.addArticulation(0, new Vex.Flow.Tremolo(3));
        }

    },

    genAnnotations: function (beat, note, tabnote) {
        // tapping
        if (beat.tapping) {
            tabnote.addModifier(new Vex.Flow.Annotation("T"), 0);
        }
    },
    
    genTuplets: function (notes, tabnotes) {
        var tuplets = [];
        var tabTuplets = [];
        var tupletNotes = [];
        var tupletTabNotes = [];
        var count = notes.length;
        var counter = 0;

        for (var i = 0; i < count; ++i) {
            var note = notes[i];
            if (note.m_tuplet) {
                tupletNotes.push(note);
                tupletTabNotes.push(tabnotes[i]);
                if (note.m_tuplet.times == counter) {
                    tuplets.push(new Vex.Flow.Tuplet(tupletNotes));
                    tabTuplets.push(new Vex.Flow.Tuplet(tupletTabNotes));
                    tupletNotes = [];
                    tupletTabNotes = [];
                    counter = 0;
                    continue;
                }

                counter++;
                continue;
            }
        }
        return {
            tuplets: tuplets,
            tabTuplets: tabTuplets
        };
        
    },
   
    genTies: function (positions, stavenotes, tabnotes) {
        var staveTies = [];
        var tabTies = [];
        for(var i=0; i< positions.length; i++) {
            var info = positions[i];

            var currPos = info.notes[0];
            // TODO chord legato
            if (currPos && currPos.effects.legato) {
                var nextPosIndex = i + 1;
                while (nextPosIndex) {
                    var nextPos = positions[nextPosIndex];
                    // next position is also legato and located at the same string
                    if (nextPos && nextPos.notes[0].effects.legato && currPos.str == nextPos.notes[0].str) {
                        nextPosIndex++;
                    } else {
                        // TODO why I need this
                        // correct index
                        if ((nextPosIndex - i) > 1 && nextPos && currPos.str != nextPos.notes[0].str) {
                            nextPosIndex--;
                        }
                        break;
                    }
                }
                
                staveTies.push(new Vex.Flow.StaveTie({
                    first_note: stavenotes[i],
                    last_note: stavenotes[nextPosIndex]
                }));

                tabTies.push(new Vex.Flow.TabTie({
                    first_note: tabnotes[i],
                    last_note: tabnotes[nextPosIndex]
                }, (nextPosIndex - i) > 1 ? "" : this.getLegatoAnnotation(tabnotes[i], tabnotes[nextPosIndex], 0)));
                i = nextPosIndex;
            }
            
            // slides
            if (currPos && currPos.effects.slide && tabnotes[i + 1]) {
                tabTies.push(new Vex.Flow.TabSlide({
                        first_note: tabnotes[i],
                        last_note: tabnotes[i + 1]
                }));
                staveTies.push(new Vex.Flow.StaveTie({
                    first_note: stavenotes[i],
                    last_note: stavenotes[i+1]
                }));
            }
        }
        return {
            staveTies: staveTies,
            tabTies: tabTies
        };
    },
    
    getLegatoAnnotation: function (first, second, index) {
        if(!second) {
            return "";
        }
        var note1 = first.positions[index];
        var note2 = second.positions[index];
        
        if(note1.str < note2.str ) {
            return "H";
        }
        else if (note1.str > note2.str) {
            return "P";
        }

        return note1.fret > note2.fret ? "P" : "H";
    },

    getDuration: function (note) {
        var duration = 0;
        switch (note.duration) {
            case "8":
                duration = 1 / 8;
                break;
            case "16":
                duration = 1 / 16;
                break;
            case "32":
                duration = 1 / 32;
                break;
            case "64":
                duration = 1 / 64;
                break;
            case "128":
                duration = 1 / 128;
                break;
            case "h":
                duration = 1 / 2;
                break;
            case "w":
                duration = 1;
                break;
            default:
                duration = 1 / 4;
        }
        if (note.dots > 0) {
            return duration + duration / 2;
        }
        return duration;
    },
    
    formatAndDrawTab: function (ctx, tabstave, stave, tabnotes, notes, autobeam, first) {
        var notevoice = new Vex.Flow.Voice(Vex.Flow.TIME4_4).setMode(Vex.Flow.Voice.Mode.SOFT);
        notevoice.addTickables(notes);

        var tabvoice = new Vex.Flow.Voice(Vex.Flow.TIME4_4).
          setMode(Vex.Flow.Voice.Mode.SOFT);
        tabvoice.addTickables(tabnotes);

        var beams = null;

        if (autobeam == true) {
            beams = Vex.Flow.Beam.applyAndGetBeams(notevoice);
        }

        var formatter = new Vex.Flow.Formatter().
          joinVoices([notevoice]).
          joinVoices([tabvoice]).
          formatToStave([notevoice, tabvoice], stave);

        notevoice.draw(ctx, stave);
        tabvoice.draw(ctx, tabstave);
        if (beams != null) {
            for (var i = 0; i < beams.length; ++i) {
                beams[i].setContext(ctx).draw();
            }
        }
        if (first) {
            // Draw a connector between tab and note staves.
            (new Vex.Flow.StaveConnector(stave, tabstave)).setContext(ctx).draw();
        }
    }
});

