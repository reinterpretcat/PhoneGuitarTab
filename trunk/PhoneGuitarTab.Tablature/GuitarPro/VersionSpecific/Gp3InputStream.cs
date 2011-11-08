using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using PhoneGuitarTab.Tablature;

namespace PhoneGuitarTab.Tablature.GuitarPro.VersionSpecific
{
    internal class Gp3InputStream: GpInputStream
    {
        private const byte MidiChannelCount = 64;

        internal Gp3InputStream(Stream fileStream)
            : base(fileStream)
        {
            
        }

        internal static GpFile Create(Stream stream, Version version)
        {
            Gp3InputStream inputStream = new Gp3InputStream(stream)
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

        #region read gp3 format

        protected AddInfo ReadAddInfo()
        {
            AddInfo info = new AddInfo()
            {
                Tempo = ReadInt(),
               /* Key = ReadKey()*/
               /* Octave = ReadInt()*/
            };
            ReadInt();
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
                Skip(25);
                chord.Name = ReadString(35);
                chord.BaseFret = ReadInt();
                for (int i = 0; i < 6; i++)
                {
                    int fret = ReadInt();
                    if (i < 6)
                    {
                        chord.Frets[i] = fret;
                    }
                }
                Skip(36);
            }

            return chord;
        }

        protected EffectsOnBeat ReadEffectsOnBeat()
        {
            EffectsOnBeat effs = new EffectsOnBeat();
            var header = new byte[1];
            header[0] = ReadByte();
            effs.Header = header;
            //TODO:
            /*
             * 	effect.setVibrato(((flags & 0x01) != 0) || ((flags & 0x02) != 0));
		        effect.setFadeIn(((flags & 0x10) != 0));
             * */
            if ((header[0] & 0x20) != 0)
            {
                int type = ReadByte();
                if (type == 0)
                {
                    //effs.TremoloBar = ReadBend();
                    ReadInt();
                }
                else
                {
                    effs.Tapping = (type == 1);
                    effs.Slapping = (type == 2);
                    effs.Popping = (type == 3);
                    ReadInt();
                }
              
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


            if ((header[0] & 0x04) != 0)
            {
                /*TGEffectHarmonic harmonic = getFactory().newEffectHarmonic();
                harmonic.setType(TGEffectHarmonic.TYPE_NATURAL);
                effect.setHarmonic(harmonic);*/
            }
            if ((header[0] & 0x08) != 0)
            {
                /*TGEffectHarmonic harmonic = getFactory().newEffectHarmonic();
                harmonic.setType(TGEffectHarmonic.TYPE_ARTIFICIAL);
                harmonic.setData(0);
                effect.setHarmonic(harmonic);*/
            }

            /*// Rasgueado
            if ((header[1] & 0x01) != 0)
                effs.HasRasgueado = true;

            if ((header[1] & 0x02) != 0)
                effs.PickStroke = ReadEnum<PickStroke>();*/

            return effs;
        }

        protected EffectsOnNote ReadEffectsOnNote()
        {
            EffectsOnNote effects = new EffectsOnNote();
            int b;

            byte header = ReadByte();



            effects.Slide = (((header & 0x04) != 0)) ? Slide.FromAbove : Slide.NoSlide;
            effects.HammerOnPullOff = (header & 0x02) != 0;
            if ((header & 0x01) != 0)
                effects.Bend = ReadBend();
            if ((header & 0x10) != 0)
                effects.GraceNote = ReadGraceNote();

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
                    /*Lyrics = ReadLyrics(),*/
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
                //measure.TonalityMeasure = ReadKey();
            }


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
         
            MixTableChange mtc = new MixTableChange();

            ReadByte(); //instrument
            int volume = (SByte)ReadByte();
            int pan = (SByte)ReadByte();
            int chorus = (SByte)ReadByte();
            int reverb = (SByte)ReadByte();
            int phaser = (SByte)ReadByte();
            int tremolo = (SByte)ReadByte();
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
                //mtc.Tempo.NewValue = tempoValue;
                ReadByte();
            }

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
                note.FingeringLeftHand = ReadEnum<Fingering>();
                note.FingeringRightHand = ReadEnum<Fingering>();
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

        #endregion

    }
}
