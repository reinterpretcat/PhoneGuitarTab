using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using PhoneGuitarTab.Tablature;

namespace PhoneGuitarTab.Tablature.GuitarPro.VersionSpecific
{
    internal class Gp4InputStream: GpInputStream
    {
        private const byte MidiChannelCount = 64;

        internal Gp4InputStream(Stream fileStream)
            : base(fileStream)
        {
            
        }

        internal static GpFile Create(Stream stream, Version version)
        {
            Gp4InputStream inputStream = new Gp4InputStream(stream)
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

        #region read gp4 format

        protected AddInfo ReadAddInfo()
        {
            AddInfo info = new AddInfo()
            {
                Tempo = ReadInt(),
                Key = (Key)(SByte) ReadInt(),
                Octave = ReadByte()
            };
            for (int i = 0; i < MidiChannelCount; i++)
                info.MidiChannels[i] = ReadMidiChannel();

            info.MeasuresNumber = ReadInt();
            info.TracksNumber = ReadInt();

            return info;
        }

        protected Beat ReadBeat()
        {
            Beat beat = new Beat();
            var header = ReadByte();
            beat.Header = header;

            // Beat status
            if ((header & 0x40) != 0)
            {
                int beatStatus = ReadByte();
                beat.EmptyBit = beatStatus == 0x00;
                beat.RestBit = beatStatus == 0x02;
            }

            // Dotted notes
            beat.DottedNotes = ((header & 0x01) != 0);

            // Beat duration
            beat.Duration = ReadDuration();

            // N-Tuplet)
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

            // Finds out which strings are played
            byte stringsPlayed = ReadByte();
            byte numberOfStrings = 0;

            for (int i = 0; i < 7; i++)
            {
                if ((stringsPlayed & (1 << i)) != 0)
                {
                    numberOfStrings++;
                    beat.Strings[i] = true;
                }
            }

            // Gets the corresponding notes
            beat.Notes = new Note[numberOfStrings];
            for (int i = 0; i < numberOfStrings; i++)
                beat.Notes[i] = ReadNote();

            return beat;
        }

        protected Bend ReadBend()
        {
            Bend bend = new Bend();
            bend.Type = ReadEnum<BendType>();
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
            if (measuresCount > 0)
            {
                body.Measures = new Measure[measuresCount];
                body.Measures[0] = ReadMeasure();
                for (int i = 1; i < measuresCount; i++)
                    body.Measures[i] = ReadMeasure(body.Measures[i - 1]);
            }

            //processing tracks
            int tracksCount = header.AdditionalInfo.TracksNumber;
            body.Tracks = new Track[tracksCount];

            //stream.ReadByte();
            for (int i = 0; i < tracksCount; i++)
                body.Tracks[i] = ReadTrack(i);

            //processing measure track pairs
            List<MeasureTrackPair> pairs = new List<MeasureTrackPair>();
            for (int i = 0; i < header.AdditionalInfo.MeasuresNumber; i++)
                for (int j = 0; j < header.AdditionalInfo.TracksNumber; j++)
                    pairs.Add(ReadMeasureTrackPair());

            body.MeasureTrackPairs = pairs.ToArray();

            return body;
        }

        protected ChordDiagram ReadChordDiagram()
        {
            ChordDiagram chord = new ChordDiagram();
            byte header = ReadByte();
            if ((header & 0x01) == 0)
            {
                chord.Name = ReadStringInteger();
                //chord.setName(readStringByteSizeOfInteger());
                chord.BaseFret = ReadInt();
                //chord.setFirstFret(readInt());
                if (chord.BaseFret != 0)
                {
                    for (int i = 0; i < 6; i++)
                    {
                        int fret = ReadInt();
                        if (i < 6)
                        {
                            chord.Frets[i] = fret;
                        }
                    }
                }
            }
            else
            {
                stream.Seek(17, SeekOrigin.Current);
                chord.Name = ReadString(21);
                stream.Seek(4, SeekOrigin.Current);
                chord.BaseFret = ReadInt();
                for (int i = 0; i < 7; i++)
                {
                    int fret = ReadInt();
                    if (i < 6)
                    {
                        chord.Frets[i] = fret;
                    }
                }
                stream.Seek(32, SeekOrigin.Current);
            }

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
                    default:
                        // return effs;
                        throw new BodyFileStructureException(
                            String.Format("{0}:{1}", SR.UnexpectedEffectValue, effect));
                }
            }

            // Tremolo bar effect
            if ((header[1] & 0x04) != 0)
            {
                //effs.TremoloBar = ReadBend();
                /*
                 * skip(5);
		int points = readInt();
		for (int i = 0; i < points; i++) {
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
                 * */
                //Skip(18);

                ReadTremoloBar();
            }


            if ((header[0] & 0x40) != 0)
            {
                // Upstroke
                int durationValue = ReadByte();
                if (durationValue != 0)
                    effs.UpStroke = (Duration)Enum.ToObject(typeof(Duration), 6 - durationValue);
                // Downstroke
                durationValue = ReadByte();
                if (durationValue != 0)
                    effs.DownStroke = (Duration)Enum.ToObject(typeof(Duration), 6 - durationValue);
            }

            // Rasgueado
            if ((header[1] & 0x01) != 0)
                effs.HasRasgueado = true;

            if ((header[1] & 0x02) != 0)
                effs.PickStroke = ReadEnum<PickStroke>();

            return effs;
        }

        protected void ReadTremoloBar()
        {
            /*
             skip(5);
		int points = readInt();
		for (int i = 0; i < points; i++) {
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
             */
            Skip(5);
            int points = ReadInt();
            for (int i = 0; i < points; i++)
                Skip(9);
        }

        protected EffectsOnNote ReadEffectsOnNote()
        {
            EffectsOnNote effects = new EffectsOnNote();

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
                effects.Slide = ReadEnum<Slide>();

            // Harmonic note present..this causes the Effects on Beat to be set
            if ((header2 & 0x10) != 0)
                effects.Harmonic = ReadEnum<Harmonic>();

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
            GraceNote graceNote = new GraceNote();
            int i;
            byte[] b = new byte[4];


            for (i = 0; i < 4; i++)
                b[i] = ReadByte();

            graceNote.Fret = b[0];
            graceNote.Dynamic = (Dynamic)Enum.ToObject(typeof(Dynamic), b[1]);
            graceNote.Transition = (GraceNoteTransition)Enum.ToObject(typeof(GraceNoteTransition), b[2]);

            graceNote.Duration = (Duration)Enum.ToObject(typeof(Duration), 3 - b[3]);

            return graceNote;
        }

        protected Header ReadHeader(Version version)
        {
            try
            {
                return new Header()
                {
                    Version = version,
                    //Version = GetVersionFromString(),
                    Tablature = ReadTablature(),
                    Lyrics = ReadLyrics(),
                    AdditionalInfo = ReadAddInfo()
                };
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
                measure.EndRepeat = ReadByte();

            // Number of alternate endings
            if ((measure.Header & 0x10) != 0)
                measure.NumberAltEnding = ReadByte();

            // Marker
            if ((measure.Header & 0x20) != 0)
                measure.PresenceMarker = ReadMarker();


            // Tonality
            if ((measure.Header & 0x40) != 0)
            {
                ReadByte();
                ReadByte();
            }
                //measure.TonalityMeasure = ReadKey();

            // Presence of a double bar
            measure.PresenceDoubleBar = ((measure.Header & 0x80) != 0);


            return measure;
        }

        protected MeasureTrackPair ReadMeasureTrackPair()
        {
            MeasureTrackPair pair = new MeasureTrackPair();
            var beatsNumber = ReadInt();
            pair.Beats = (new Beat[beatsNumber]).ToList();
            for (int i = 0; i < beatsNumber; i++)
                pair.Beats[i] = ReadBeat();

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
            int[] pos = new int[8];
            int i;
            int n;
            int aux;

            MixTableChange mtc = new MixTableChange();

            // For easier processing, creates an array
            MixTableElement[] elements = new MixTableElement[8];

            for (i = 0; i < 8; i++)
                elements[i] = new MixTableElement();

            n = 0;
            for (i = 0; i < 7; i++)
            {
                aux = ReadByte();
                if ((i != 0) && (aux != 255))
                {
                    pos[n] = i;
                    n++;
                }
                elements[i].NewValue = aux;
            }
            // The tempo field is different (needs an integer)
            aux = ReadInt();
            if (aux != -1)
            {
                pos[n] = i;
                n++;
            }

            elements[7].NewValue = aux;

            // Skip the instrument field

            for (i = 0; i < n; i++)
            {
                aux = ReadByte();
                if (elements[pos[i]].NewValue != 255)
                {
                    elements[pos[i]].ChangeDuration = aux;
                }
            }

            int applyToAllTracks = ReadByte();

            // The instrument and the tempo are not affected
            for (i = 0; i < 6; i++)
            {
                if ((applyToAllTracks & (1 << i)) != 0)
                {
                    elements[i + 1].ApplyToAllTracks = true;
                }
            }
            // The tempo always applies to all the tracks.
            elements[7].ApplyToAllTracks = true;

            // Sets all the values
            mtc.Instrument = elements[0];
            mtc.Volume = elements[1];
            mtc.Balance = elements[2];
            mtc.Chorus = elements[3];
            mtc.Reverb = elements[4];
            mtc.Phaser = elements[5];
            mtc.Tremolo = elements[6];
            mtc.Tempo = elements[7];

            return mtc;
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

            // Note duration
            if ((header & 0x01) != 0)
            {
                try
                {
                    // TODO: understand why we can have here a Duration whose value
                    // is SIX
                    // In GP, it seems to be some kind of special value (a triplet
                    // appears...)
                    note.Duration = ReadEnum<Duration>();
                }
                catch (Exception e)
                {
                }
                note.NTuplet = ReadByte();
            }

            // Note dynamic
            if ((header & 0x10) != 0)
                note.Dynamic = ReadEnum<Dynamic>();

            // Fret number
            if ((header & 0x20) != 0)
                note.FretNumber = ReadByte();


            // Fingering
            if ((header & 0x80) != 0)
            {
                /*note.FingeringLeftHand = ReadEnum<Fingering>();
                note.FingeringRightHand = ReadEnum<Fingering>();*/
                Skip(2);
            }

            // Effects on the note
            if ((header & 0x08) != 0)
                note.Effects = ReadEffectsOnNote();


            return note;

        }

        protected Tablature ReadTablature()
        {
            return new Tablature()
            {
                Title = ReadHeaderEntry(),
                Subtitle = ReadHeaderEntry(),
                Interpret = ReadHeaderEntry(),
                Album = ReadHeaderEntry(),
                Author = ReadHeaderEntry(),
                Copyright = ReadHeaderEntry(),
                Tab = ReadHeaderEntry(),
                Instructional = ReadHeaderEntry(),
                Notice = ReadStringArrayWithLength(),
                IsTripletFeel = ReadByte() > 0
            };

        }

        protected Track ReadTrack(int number)
        {
            Track track = new Track();


            track.Header = ReadByte();
            //TODO: move this code into header's setter
            track.IsDrumsTrack = ((track.Header & 0x01) != 0);
            track.Is12StringedGuitarTrack = ((track.Header & 0x02) != 0);
            track.IsBanjoTrack = ((track.Header & 0x04) != 0);

            stream.ReadByte();
            /*stream.ReadByte();
            stream.ReadByte();*/
            track.Name = ReadString(40).TrimEnd('\0');
            track.StringNumber = ReadInt();
            for (int i = 0; i < 7; i++)
                track.StringTuning[i] = ReadInt();

            track.Port = ReadInt();
            track.Channel = ReadInt();
            track.ChannelEffects = ReadInt();
            track.FretsNumber = ReadInt();
            track.CapoHeight = ReadInt();
            track.Color = ReadColor();
            return track;
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
