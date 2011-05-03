using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using PhoneGuitarTab.GuitarPro.Infrastructure;

namespace PhoneGuitarTab.GuitarPro.Fourth
{
    internal class Gp5InputStream: GpInputStream
    {
        private const byte MidiChannelCount = 64;

        internal Gp5InputStream(Stream fileStream):base(fileStream)
        {
            
        }

        internal static GpFile Create(Stream stream, Version version)
        {
            Gp5InputStream inputStream = new Gp5InputStream(stream)
                                             {
                                                 Version = version
                                             };

            var header = inputStream.ReadHeader(version);
            var body = inputStream.ReadBody(header);

            return new GpFile()
            {
                Header = header,
                Body = body
            };
        }

        #region read gp5 format

        protected AddInfo ReadAddInfo()
        {
            AddInfo info = new AddInfo();
            info.Tempo = ReadInt();
            if (Version.Minor != "00")
                stream.Seek(1, SeekOrigin.Current);
            /*info.Key =*/ ReadInt();
            /*info.Octave =*/ ReadByte();

            for (int i = 0; i < MidiChannelCount; i++)
                info.MidiChannels[i] = ReadMidiChannel();

            stream.Seek(42, SeekOrigin.Current);

            info.MeasuresNumber = ReadInt();
            info.TracksNumber = ReadInt();

            return info;
        }

        protected Beat ReadBeat(int voiceIndex, Track track)
        {
            Beat beat = new Beat();
            //Voice voice = new Voice() { Index = voiceIndex };
            var header = ReadByte();
            beat.Header = header;

            // Beat status
            if ((header & 0x40) != 0)
            {
                int beatStatus = ReadByte();
                //beat.EmptyBit = beatStatus == 0x00;
                // voice.Empty = (beatStatus & 0x02) == 0;
                beat.EmptyBit = (beatStatus & 0x02) == 0;
                beat.RestBit = beatStatus == 0x02;
            }

            // Dotted notes
            beat.DottedNotes = ((header & 0x01) != 0);

            // Beat duration
            //TODO different!
            beat.Duration = ReadDuration();

            // N-Tuplet
            if ((header & 0x20) != 0)
                beat.NTuplet = ReadInt();

            // Chord diagram
            if ((header & 0x02) != 0)
                beat.ChordDiagram = ReadChordDiagram();

            // Text
            if ((header & 0x04) != 0)
                beat.Text = ReadHeaderEntry();


            // Effects on the beat
            if ((header & 0x08) != 0)
                beat.Effects = ReadEffectsOnBeat();


            // Mix table change
            if ((header & 0x10) != 0)
                beat.MixTableChange = ReadMixTableChange();

            /*byte stringFlags = ReadByte();
            beat.Notes = new Note[7];
            for (int i = 6; i >= 0; i--)
            {
                if ((stringFlags & (1 << i)) != 0 && (6 - i) < 6) //TODO strings count
                {
                    beat.Notes[0] = ReadNote();
                    //voice.Notes.Add(note);
                }
            }*/

            byte stringsPlayed = ReadByte();
            /* byte numberOfStrings = 0;
             byte stringCount = 6;
             for (int i = 0; i < 7; i++)
             {
                 if (((stringsPlayed & (1 << i)) != 0)&&( i < stringCount))
                 {
                     numberOfStrings++;
                     beat.Strings[i] = true;
                 }
             }

             // Gets the corresponding notes
             beat.Notes = new Note[numberOfStrings];
             for (int i = 0; i < numberOfStrings; i++)
                 beat.Notes[i] = ReadNote();*/

            int stringCount = track.StringNumber;
            List<Note> notes = new List<Note>();
            for (int i = 6; i >= 0; i--)
                if (((stringsPlayed & (1 << i)) != 0) && ((6 - i) < stringCount))
                {
                    beat.Strings[i] = true;
                    notes.Add(ReadNote());
                }

            beat.Notes = notes.ToArray();

            Skip(1);

            int read = ReadByte();
            //if (read == 8 || read == 10)
            if ((read & 0x08) != 0)
                Skip(1);

            return beat;

        }

     
        protected Bend ReadBend()
        {
            Bend bend = new Bend();
            //bend.Type = ReadEnum<BendType>();
            ReadByte();
            bend.Value = ReadInt();

            int pointsNumber = ReadInt();
            bend.Points = new BendPoint[pointsNumber];
            for (int i = 0; i < pointsNumber; i++)
                bend.Points[i] = ReadBendPoint();

            return bend;
        }

        protected BendPoint ReadBendPoint()
        {
            return new BendPoint()
            {
                Position = ReadInt(),
                Value = ReadInt(),
                Vibrato = ReadEnum<Vibrato>()
            };
        }

        protected Body ReadBody(Header header)
        {
            Body body = new Body();
            //processing measures
            int measuresCount = header.AdditionalInfo.MeasuresNumber;
            body.Measures = new Measure[measuresCount];
            body.Measures[0] = ReadMeasure();
            for (int i = 1; i < measuresCount; i++)
            {
                stream.Seek(1, SeekOrigin.Current);
                body.Measures[i] = ReadMeasure(body.Measures[i - 1]);
            }

           

            //processing tracks
            int tracksCount = header.AdditionalInfo.TracksNumber;
            body.Tracks = new Track[tracksCount];

            stream.ReadByte();
            for (int i = 0; i < tracksCount; i++)
                body.Tracks[i] = ReadTrack(i+1);
            Skip((Version.Minor == "00" ? 2 : 1));
            //processing measure track pairs
            List<MeasureTrackPair> pairs = new List<MeasureTrackPair>();
            for (int i = 0; i < header.AdditionalInfo.MeasuresNumber; i++)
                for (int j = 0; j < header.AdditionalInfo.TracksNumber; j++)
                {
                    pairs.Add(ReadMeasureTrackPair(body.Tracks[j]));
                    Skip(1);
                }

            body.MeasureTrackPairs = pairs.ToArray();

            return body;
        }

        protected ChordDiagram ReadChordDiagram()
        {
            ChordDiagram chord = new ChordDiagram();
            Skip(17);
            chord.Name = ReadString(22);
            Skip(4);
            chord.BaseFret = ReadInt();

            for (int i = 0; i < 7; i++)
            {
                int fret = ReadInt();
                if (i < 6)
                {
                    chord.Frets[i] = fret;
                }
            }
            Skip(32);
            return chord;
        }

        protected EffectsOnBeat ReadEffectsOnBeat()
        {
            EffectsOnBeat effs = new EffectsOnBeat();
            var header = new byte[2];
            header[0] = ReadByte();
            header[1] = ReadByte();
            effs.Header = header;

            if ((header[0] & 0x20) != 0)
            {
                byte effect = ReadByte();
                switch (effect)
                {
                    case 0:
                        break; // no effect
                    case 1:
                        effs.Tapping = true;
                        break;
                    case 2:
                        effs.Slapping = true;
                        break;
                    case 3:
                        effs.Popping = true;
                        break;
                    /*default:
                        // return effs;
                        throw new BodyFileStructureException(
                            String.Format("{0}:{1}", SR.UnexpectedEffectValue, effect));*/
                }
            }

            // Tremolo bar effect
            if ((header[1] & 0x04) != 0)
            {

                //effs.TremoloBar = ReadBend();
                /*skip(5);
		TGEffectTremoloBar tremoloBar = getFactory().newEffectTremoloBar();
		int numPoints = readInt();
		for (int i = 0; i < numPoints; i++) {
			int position = readInt();
			int value = readInt();
			readByte();
			
			int pointPosition = Math.round(position * TGEffectTremoloBar.MAX_POSITION_LENGTH / GP_BEND_POSITION);
			int pointValue = Math.round(value / (GP_BEND_SEMITONE * 2f));
			tremoloBar.addPoint(pointPosition,pointValue);
		}
		if(!tremoloBar.getPoints().isEmpty()){
			effect.setTremoloBar(tremoloBar);
		}
	}*/
                ReadTremoloBar();
            }


            if ((header[0] & 0x40) != 0)
            {
                //TODO
               /* // Upstroke
                int durationValue = ReadByte();
                if (durationValue != 0)
                    effs.UpStroke = (Duration) Enum.ToObject(typeof (Duration), (byte) (6 - durationValue));
                // Downstroke
                durationValue = ReadByte();
                if (durationValue != 0)
                    effs.DownStroke = (Duration)Enum.ToObject(typeof(Duration), (byte)(6 - durationValue));*/

                int strokeUp = ReadByte();
                int strokeDown = ReadByte();
                if (strokeUp > 0)
                {
                    /*beat.getStroke().setDirection(TGStroke.STROKE_UP);
                    beat.getStroke().setValue(toStrokeValue(strokeUp));*/
                }
                else if (strokeDown > 0)
                {
                    /*beat.getStroke().setDirection(TGStroke.STROKE_DOWN);
                    beat.getStroke().setValue(toStrokeValue(strokeDown));*/
                } 


            }

            // Rasgueado
            if ((header[1] & 0x01) != 0)
                effs.HasRasgueado = true;

            if ((header[1] & 0x02) != 0)
            {
                /*effs.PickStroke = ReadEnum<PickStroke>();*/
                ReadByte();
            }

            return effs;
        }

        protected void ReadTremoloBar()
        {
            /*skip(5);
            TGEffectTremoloBar tremoloBar = getFactory().newEffectTremoloBar();
            int numPoints = readInt();
            for (int i = 0; i < numPoints; i++)
            {
                int position = readInt();
                int value = readInt();
                readByte();

                int pointPosition = Math.round(position * TGEffectTremoloBar.MAX_POSITION_LENGTH / GP_BEND_POSITION);
                int pointValue = Math.round(value / (GP_BEND_SEMITONE * 2f));
                tremoloBar.addPoint(pointPosition, pointValue);
            }
            if (!tremoloBar.getPoints().isEmpty())
            {
                effect.setTremoloBar(tremoloBar);
            }*/
            Skip(5);
            int points = ReadInt();
            for (int i = 0; i < points; i++)
                Skip(9);

        }

        protected EffectsOnNote ReadEffectsOnNote()
        {
            EffectsOnNote effects = new EffectsOnNote();
            int b;

            byte header1 = ReadByte();
            byte header2 = ReadByte();

            // Bend present
            if ((header1 & 0x01) != 0)
                effects.Bend = ReadBend();

            // Interpretation of the FIRST header
            // Grace note present)
            if ((header1 & 0x10) != 0)
                effects.GraceNote = ReadGraceNote();

            // Tremolo picking present
            if ((header2 & 0x04) != 0)
                effects.TremoloPicking = ReadEnum<Duration>();

            // Slide from the current note present
            if ((header2 & 0x08) != 0)
            {
                //var slide = ReadEnum<Slide>();
                effects.Slide = ReadEnum<Slide>();
            }

            // Harmonic note present..this causes the Effects on Beat to be set
            if ((header2 & 0x10) != 0)
            {
                /*effects.Harmonic = ReadEnum<Harmonic>();
                if (effects.Harmonic == Harmonic.Artificial)
                    Skip(3);
                else if (effects.Harmonic == Harmonic.Tapped)
                    Skip(1);*/
                var type = ReadByte();
                if (type == 1)
                {
                    effects.Harmonic = Harmonic.Natural;

                }
                else if (type == 2)
                {
                    Skip(3);
                    effects.Harmonic = Harmonic.Artificial;
                }
                else if (type == 3)
                {
                    Skip(1);
                    effects.Harmonic = Harmonic.Tapped;
                }
                else if (type == 4)
                {
                    effects.Harmonic = Harmonic.Pitch;
                }
                else if (type == 5)
                {
                    effects.Harmonic = Harmonic.Semi;
                }

            }

            // Trill present
            if ((header2 & 0x20) != 0)
                effects.Trill = ReadTrill();

            // Let-ring present
            effects.LetRing = (header1 & 0x08) != 0;

            // hammer-on or a pull-off from the current note present
            effects.HammerOnPullOff = (header1 & 0x02) != 0;

            // Interpretation of the SECOND header
            // Left-hand vibrato present..this causes the Effects on Beat to be set
            effects.LeftHandVibrato = (header2 & 0x40) != 0;

            // Palm Mute present
            effects.PalmMute = (header2 & 0x02) != 0;

            // Note played staccato
            effects.Staccato = (header2 & 0x01) != 0;

            return effects;

        }

        protected GraceNote ReadGraceNote()
        {
            //TODO:
            //grace.setDead( (flags & 0x01) != 0 );
            //grace.setOnBeat( (flags & 0x02) != 0 );

            GraceNote graceNote = new GraceNote();
            graceNote.Fret = ReadByte();
            //TODO
            var dynamic = (Dynamic)Enum.ToObject(typeof(Dynamic), ReadByte());
            graceNote.Dynamic = Dynamic.Invalid;
            //TODO
            var transition = (GraceNoteTransition)Enum.ToObject(typeof(GraceNoteTransition), ReadByte());
            graceNote.Transition = GraceNoteTransition.None;
            graceNote.Duration = (Duration) Enum.ToObject(typeof (Duration), (byte) (3 - ReadByte()));

            int flags = ReadByte();

            return graceNote;
        }

        protected Header ReadHeader(Version version)
        {
            try
            {
                var header = new Header();
               // header.Version = GetVersionFromString();
                header.Version = version;
                header.Tablature = ReadTablature();
                header.Lyrics = ReadLyrics();
                //page setup
                stream.Seek(header.Version.Minor != "00" ? 49 : 30, SeekOrigin.Current);
                for (int i = 0; i < 11; i++)
                {
                    stream.Seek(4, SeekOrigin.Current);
                    var len = ReadByte();
                    var gg = ReadString(len);

                }
                
                header.AdditionalInfo = ReadAddInfo();

                return header;

            }
            catch (Exception ex)
            {
                throw new HeaderFileStructureException(SR.InvalidFileHeader, ex);
            }
        }

        protected Lyrics ReadLyrics()
        {
            Lyrics lyrics = new Lyrics();
            lyrics.TrackNumber = ReadInt();
            for (int i = 0; i < 5; i++)
            {
                lyrics.MeasureNumber[i] = ReadInt();
                lyrics.Lines[i] = ReadStringInteger();
            }
            return lyrics;
        }

        protected Marker ReadMarker()
        {
            return new Marker()
            {
                Name = ReadHeaderEntry(),
                Color = ReadColor()
            };
        }

        protected Measure ReadMeasure()
        {
            return ReadMeasure(null);
        }

        protected Measure ReadMeasure(Measure previous)
        {
            Measure measure = Measure.GetMeasureFromPrevious(previous);
            measure.Header = (byte)stream.ReadByte();

            //TODO: move this code into header's setter
            // Numerator
            if ((measure.Header & 0x01) != 0)
                measure.NumeratorSignature = ReadByte();

            // Denominator
            if ((measure.Header & 0x02) != 0)
                measure.DenominatorSignature = ReadByte();


            // Beginning of repeat
            measure.BeginRepeat = ((measure.Header & 0x04) != 0);

            // End of repeat
            if ((measure.Header & 0x08) != 0)
                measure.EndRepeat = (byte) ((ReadByte() & 0xff) - 1);

            // Marker
            if ((measure.Header & 0x20) != 0)
                measure.PresenceMarker = ReadMarker();

            // Number of alternate endings
            if ((measure.Header & 0x10) != 0)
                measure.NumberAltEnding = ReadByte();


            // Tonality
            if ((measure.Header & 0x40) != 0)
            {
                //measure.TonalityMeasure = ReadKey();
                ReadByte();
                ReadByte();
            }

            if ((measure.Header & 0x01) != 0)
                Skip(4);

            if ((measure.Header & 0x10) == 0)
                Skip(1);

            int tripletFeel = ReadByte();
            
            //TODO Implement tripletFeel
            /*if (tripletFeel == 1)
            {
                header.setTripletFeel(TGMeasureHeader.TRIPLET_FEEL_EIGHTH);
            }
            else if (tripletFeel == 2)
            {
                header.setTripletFeel(TGMeasureHeader.TRIPLET_FEEL_SIXTEENTH);
            }
            else
            {
                header.setTripletFeel(TGMeasureHeader.TRIPLET_FEEL_NONE);
            }*/

            // Presence of a double bar
            //measure.PresenceDoubleBar = ((measure.Header & 0x80) != 0);


            return measure;
        }

        protected MeasureTrackPair ReadMeasureTrackPair(Track track)
        {
            MeasureTrackPair pair = new MeasureTrackPair();
            pair.Beats = new List<Beat>();
            for (int voice = 0; voice < 2; voice++)
            {
                var beatsNumber = ReadInt();

                for (int i = 0; i < beatsNumber; i++)
                {
                    var beat = ReadBeat(voice, track);
                    //if (beat.Notes.Count() > 0)
                    if(!beat.EmptyBit)
                        pair.Beats.Add(beat);
                }
            }
            //pair.Beats = pair.Beats.Where(b => !b.RestBit).ToList();
            //NOTE add code is here
           /* List emptyBeats = new List<Beat>();
            for (int i = 0; i < pair.Beats.Count(); i++)
            {
                TGBeat beat = measure.getBeat(i);
                boolean empty = true;
                for (int v = 0; v < beat.countVoices(); v++)
                {
                    if (!beat.getVoice(v).isEmpty())
                    {
                        empty = false;
                    }
                }
                if (empty)
                {
                    emptyBeats.add(beat);
                }
            }
            Iterator it = emptyBeats.iterator();
            while (it.hasNext())
            {
                TGBeat beat = (TGBeat)it.next();
                measure.removeBeat(beat);
            }
            )*/
            return pair;
        }

        protected MidiChannel ReadMidiChannel()
        {
            return new MidiChannel()
            {
                Instrument = ReadInt(),
                Volume = ReadByte(),
                Balance = ReadByte(),
                Chorus = ReadByte(),
                Reverb = ReadByte(),
                Phaser = ReadByte(),
                Tremolo = ReadByte(),
                Blank1 = ReadByte(),
                Blank2 = ReadByte()
            };
        }

        protected MixTableChange ReadMixTableChange()
        {
            MixTableChange mtc = new MixTableChange();
            byte instrument = ReadByte(); //instrument

            Skip(16);
            int volume = (SByte)ReadByte();
            int pan = (SByte)ReadByte();
            int chorus = (SByte)ReadByte();
            int reverb = (SByte)ReadByte();
            int phaser = (SByte)ReadByte();
            int tremolo = (SByte)ReadByte();
            string tempoName = ReadHeaderEntry(); //tempoName
            int tempoValue = ReadInt();
            if (volume >= 0)
            {
                ReadByte();
            }
            if (pan >= 0)
            {
                ReadByte();
            }
            if (chorus >= 0)
            {
                ReadByte();
            }
            if (reverb >= 0)
            {
                ReadByte();
            }
            if (phaser >= 0)
            {
                ReadByte();
            }
            if (tremolo >= 0)
            {
                ReadByte();
            }
            if (tempoValue >= 0)
            {
                Skip(1);
                if (Version.Minor != "00")
                    Skip(1);
            }
            ReadByte();
            Skip(1);
            if (Version.Minor != "00")
            {
                ReadHeaderEntry();
                ReadHeaderEntry();
            }

            return new MixTableChange();
        }

        /*public MixTableElement MixTableElementRead()
        {
            return null;
        }*/

        protected Note ReadNote()
        {
            Note note = new Note();
            var header = ReadByte();
            note.Header = header;
            // Note status
            note.IsAccentuated = ((header & 0x40) != 0);
            note.IsDotted = ((header & 0x02) != 0);
            note.IsGhostNote = ((header & 0x04) != 0);

            // Note type
            if ((header & 0x20) != 0)
            {
                byte noteType = ReadByte();
                note.IsTieNote = noteType == 0x02;
                note.IsDeadNote = noteType == 0x03;
            }

            // Note dynamic
            if ((header & 0x10) != 0)
            {
                var dynamic = ReadEnum<Dynamic>();
                //TODO set dynamic
                note.Dynamic = Dynamic.Invalid;
            }

            // Fret number
            if ((header & 0x20) != 0)
                note.FretNumber = ReadByte();

            // Fingering
            if ((header & 0x80) != 0)
            {
                note.FingeringLeftHand = ReadEnum<Fingering>();
                note.FingeringRightHand = ReadEnum<Fingering>();
            }

            // Note duration
            if ((header & 0x01) != 0)
                Skip(8);
                      
            Skip(1);
            // Effects on the note
            if ((header & 0x08) != 0)
                note.Effects = ReadEffectsOnNote();


            return note;

        }

        protected Tablature ReadTablature()
        {
            var tablature = new Tablature();

            tablature.Title = ReadHeaderEntry();
            tablature.Subtitle = ReadHeaderEntry();
            tablature.Interpret = ReadHeaderEntry();
            tablature.Album = ReadHeaderEntry();
            tablature.Author = ReadHeaderEntry();
            tablature.Copyright = ReadHeaderEntry();
            tablature.Tab = ReadHeaderEntry();
            tablature.Instructional = ReadHeaderEntry();
            var gg = ReadHeaderEntry();
            tablature.Notice = ReadStringArrayWithLength();
            //IsTripletFeel = ReadByte() > 0

            return tablature;

        }

        protected Track ReadTrack(int number)
        {
            Track track = new Track();
            
            track.Header = ReadByte();
            if (number == 1 || Version.Minor == "00")
                Skip(1);
            //TODO: move this code into header's setter
            track.IsDrumsTrack = ((track.Header & 0x01) != 0);
            track.Is12StringedGuitarTrack = ((track.Header & 0x02) != 0);
            track.IsBanjoTrack = ((track.Header & 0x04) != 0);

            //NOTE ???
            if (number != 1)
                stream.ReadByte();

            track.Name = ReadString(40).TrimEnd('\0');
            track.StringNumber = ReadInt();
            for (int i = 0; i < 7; i++)
                track.StringTuning[i] = ReadInt();

            track.Port = ReadInt();
            track.Channel = ReadChannel();
            ReadInt();
            ReadInt();
            track.Color = ReadColor();
            Skip((Version.Minor != "00") ? 49 : 44);
            if (Version.Minor != "00")
            {
                ReadHeaderEntry();
                ReadHeaderEntry();
            }

            return track;
        }

        protected int ReadChannel()
        {
            int index = (ReadInt() - 1);
            int effectChannel = (ReadInt() - 1);
            if (index >= 0 && index < MidiChannelCount)
            {
                /*((TGChannel) channels.get(index)).copy(channel);
                if (channel.getInstrument() < 0) {
                    channel.setInstrument((short)0);
                }
                if(!channel.isPercussionChannel()){
                    channel.setEffectChannel((short)effectChannel);
                }*/
            }
            return effectChannel;
        }

        protected Trill ReadTrill()
        {
            return new Trill()
            {
                Fret = ReadByte(),
                Period = ReadEnum<Duration>()
            };
        }

        #endregion

    }
}
