using System.Threading.Tasks;

namespace PhoneGuitarTab.Tablatures.Writers
{
    using System;
    using System.IO;
    using System.Collections.Generic;
    using PhoneGuitarTab.Tablatures.Models;
    using PhoneGuitarTab.Tablatures.Models.Effects;

    public class FifthGuitarProSongWriter : GuitarProSongWriter
    {
        private static string GP5_FORMAT_EXTENSION = ".gp5";
        private static string GP5_VERSION = "FICHIER GUITAR PRO v5.00";
        private static int GP_BEND_SEMITONE = 25;
        private static int GP_BEND_POSITION = 60;
        private static string[] PAGE_SETUP_LINES =
            {
                "%TITLE%", "%SUBTITLE%", "%ARTIST%", "%ALBUM%", "Words by %WORDS%",
                "Music by %MUSIC%", "Words & Music by %WORDSMUSIC%",
                "Copyright %COPYRIGHT%",
                "All Rights Reserved - International Copyright Secured",
                "Page %N%/%P%", "Moderate",
            };

        public FifthGuitarProSongWriter(BinaryWriter sw)
            : base(sw)
        {
        }

        #region

        public override void WriteSong(Song song)
        {
            if (song.IsEmpty)
                throw new ArgumentException("Empty song");

            MeasureHeader header = song.MeasureHeaders[0];
            WriteStringByte(GP5_VERSION, 30);
            WriteInfo(song);
            WriteLyrics(song);
            WritePageSetup();
            WriteInt(header.Tempo.Value);
            WriteInt(0);
            WriteByte((byte)0);
            WriteChannels(song);
            for (int i = 0; i < 42; i++)
            {
                WriteByte(0xff);
            }
            WriteInt(song.MeasureHeaders.Count);
            WriteInt(song.Tracks.Count);
            WriteMeasureHeaders(song);
            WriteTracks(song);
            SkipBytes(2);
            WriteMeasures(song, header.Tempo);
            Close();
        }

        private void WriteInfo(Song song)
        {
            var comments = ToCommentLines(song.Comments);
            
            WriteStringByteSizeOfInteger(song.Name);
            
            WriteStringByteSizeOfInteger("");
            WriteStringByteSizeOfInteger(song.Artist);
            WriteStringByteSizeOfInteger(song.Album);
            WriteStringByteSizeOfInteger(song.Author);
            WriteStringByteSizeOfInteger("");
            WriteStringByteSizeOfInteger(song.Copyright);
            WriteStringByteSizeOfInteger(song.Writer);
            WriteStringByteSizeOfInteger("");
            WriteInt(comments.Count);
            for (int i = 0; i < comments.Count; i++)
            {
                WriteStringByteSizeOfInteger(comments[i]);
            }
        }

        private void WriteLyrics(Song song)
        {
            Track lyricTrack = null;
            var it = song.Tracks.GetEnumerator();
            while (it.MoveNext())
            {
                var track = it.Current;
                if (!track.Lyrics.IsEmpty)
                {
                    lyricTrack = track;
                    break;
                }
            }
            WriteInt((lyricTrack == null) ? 0 : lyricTrack.Number);
            WriteInt((lyricTrack == null) ? 0 : lyricTrack.Lyrics.From);
            WriteStringInteger((lyricTrack == null) ? "" : lyricTrack.Lyrics.Lyrics);
            for (int i = 0; i < 4; i++)
            {
                WriteInt((lyricTrack == null) ? 0 : 1);
                WriteStringInteger("");
            }
        }

        private void WriteChannels(Song song)
        {
            Channel[] channels = MakeChannels(song);
            for (int i = 0; i < channels.Length; i++)
            {
                WriteInt(channels[i].Instrument);
                WriteByte(ToChannelByte(channels[i].Volume));
                WriteByte(ToChannelByte(channels[i].Balance));
                WriteByte(ToChannelByte(channels[i].Chorus));
                WriteByte(ToChannelByte(channels[i].Reverb));
                WriteByte(ToChannelByte(channels[i].Phaser));
                WriteByte(ToChannelByte(channels[i].Tremolo));
                WriteBytes(new sbyte[] { 0, 0 });
            }
        }

        private void WritePageSetup()
        {
            WriteInt(210); // Page width
            WriteInt(297); // Page height
            WriteInt(10);  // Margin left
            WriteInt(10);  // Margin right
            WriteInt(15);  // Margin top
            WriteInt(10);  // Margin bottom
            WriteInt(100); // Score size percent

            WriteByte((byte)0xff); // View flags
            WriteByte((byte)0x01); // View flags

            for (int i = 0; i < PAGE_SETUP_LINES.Length; i++)
            {
                WriteInt((PAGE_SETUP_LINES[i].Length + 1));
                WriteStringByte(PAGE_SETUP_LINES[i], 0);
            }
        }

        private void WriteMeasureHeaders(Song song)
        {
            TimeSignature timeSignature = new TimeSignature();
            if (song.MeasureHeaders.Count > 0)
            {
                for (int i = 0; i < song.MeasureHeaders.Count; i++)
                {
                    if (i > 0)
                    {
                        SkipBytes(1);
                    }
                    MeasureHeader measure = song.MeasureHeaders[i];
                    WriteMeasureHeader(measure, timeSignature);

                    timeSignature.Numerator = measure.TimeSignature.Numerator;
                    timeSignature.Denominator.Value = measure.TimeSignature.Denominator.Value;
                }
            }
        }

        private void WriteMeasureHeader(MeasureHeader measure, TimeSignature timeSignature)
        {
            sbyte flags = 0;
            if (measure.Number == 0)
            {
                flags |= 0x40;
            }
            if (measure.Number == 0 || !(measure.TimeSignature.Denominator.Value == timeSignature.Denominator.Value && measure.TimeSignature.Numerator == timeSignature.Numerator))
            {
                flags |= 0x01;
                flags |= 0x02;
            }
            if (measure.IsRepeatOpen)
            {
                flags |= 0x04;
            }
            if (measure.RepeatClose > 0)
            {
                flags |= 0x08;
            }
            if (measure.RepeatAlternative > 0)
            {
                flags |= 0x10;
            }
            if (measure.Marker != null)
            {
                flags |= 0x20;
            }

            WriteUnsignedByte(flags);

            if ((flags & 0x01) != 0)
            {
                WriteByte((byte)measure.TimeSignature.Numerator);
            }
            if ((flags & 0x02) != 0)
            {
                WriteByte((byte)measure.TimeSignature.Denominator.Value);
            }
            if ((flags & 0x08) != 0)
            {
                WriteByte((byte)(measure.RepeatClose + 1));
            }
            if ((flags & 0x20) != 0)
            {
                WriteMarker(measure.Marker);
            }
            if ((flags & 0x10) != 0)
            {
                WriteByte((byte)measure.RepeatAlternative);
            }
            if ((flags & 0x40) != 0)
            {
                SkipBytes(2);
            }
            if ((flags & 0x01) != 0)
            {
                WriteBytes(MakeBeamEighthNoteBytes(measure.TimeSignature));
            }
            if ((flags & 0x10) == 0)
            {
                WriteByte((byte)0);
            }
            if (measure.TripletFeel == MeasureHeader.TripletFeelNone)
            {
                WriteByte((byte)0);
            }
            else if (measure.TripletFeel == MeasureHeader.TripletFeelEighth)
            {
                WriteByte((byte)1);
            }
            else if (measure.TripletFeel == MeasureHeader.TripletFeelSixteenth)
            {
                WriteByte((byte)2);
            }
        }

        private void WriteTracks(Song song)
        {
            for (int i = 0; i < song.Tracks.Count; i++)
            {
                var track = song.Tracks[i];
                WriteTrack(track);
            }
        }

        private void WriteTrack(Track track)
        {
            int flags = 0;
            if (track.IsPercussionTrack)
            {
                flags |= 0x01;
            }
            WriteUnsignedByte(flags);
            WriteByte((byte)8);
            WriteStringByte(track.Name, 40);
            WriteInt(track.Strings.Count);
            for (int i = 0; i < 7; i++)
            {
                int value = 0;
                if (track.Strings.Count > i)
                {
                    GuitarString s = track.Strings[i];
                    value = s.Value;
                }
                WriteInt(value);
            }
            WriteInt(1);
            WriteInt(track.Channel.ChannelCode + 1);
            WriteInt(track.Channel.EffectChannel + 1);
            WriteInt(24);
            WriteInt(track.Offset);
            WriteColor(track.Color);

            WriteBytes(new sbyte[] { 67, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 100, 0, 0, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, -1, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 });
        }

        private void WriteMeasures(Song song, Tempo tempo)
        {
            for (int i = 0; i < song.MeasureHeaders.Count; i++)
            {
                MeasureHeader header = song.MeasureHeaders[i];
                for (int j = 0; j < song.Tracks.Count; j++)
                {
                    Track track = song.Tracks[j];
                    Measure measure = track.Measures[i];
                    WriteMeasure(measure, (header.Tempo.Value != tempo.Value));
                    SkipBytes(1);
                }
                header.Tempo = tempo;
            }
        }

        private void WriteMeasure(Measure measure, bool changeTempo)
        {
            for (int v = 0; v < 2; v++)
            {
                var voices = new List<Voice>();
                for (int m = 0; m < measure.Beats.Count; m++)
                {
                    Beat beat = measure.Beats[m];
                    if (v < beat.Voices.Length)
                    {
                        Voice voice = beat.Voices[v];
                        if (!voice.IsEmpty)
                        {
                            voices.Add(voice);
                        }
                    }
                }
                if (voices.Count > 0)
                {
                    WriteInt(voices.Count);
                    for (int i = 0; i < voices.Count; i++)
                    {
                        Voice voice = voices[i];
                        WriteBeat(voice, voice.Beat, measure, (changeTempo && i == 0));
                    }
                }
                else
                {
                    // Fill empty voices.
                    int count = measure.Header.TimeSignature.Numerator;
                    Beat beat = new Beat();
                    if (v < beat.Voices.Length)
                    {
                        Voice voice = beat.Voices[v];
                        voice.Duration.Value = measure.Header.TimeSignature.Denominator.Value;
                        voice.IsEmpty = true;

                        WriteInt(count);
                        for (int i = 0; i < count; i++)
                        {
                            WriteBeat(voice, beat /*voice.Beat*/, measure, (changeTempo && i == 0));
                        }
                    }
                }
            }
        }

        private void WriteBeat(Voice voice, Beat beat, Measure measure, bool changeTempo)
        {
            Duration duration = voice.Duration;
            NoteEffect effect = new NoteEffect();
            for (int i = 0; i < voice.Notes.Count; i++)
            {
                var playedNote = voice.Notes[i];

                if (playedNote.Effect.IsFadeIn)
                {
                    effect.IsFadeIn = true;
                }
                if (playedNote.Effect.TremoloBar != null)
                {
                    effect.TremoloBar = playedNote.Effect.TremoloBar;
                }
                if (playedNote.Effect.IsTapping)
                {
                    effect.IsTapping = true;
                }
                if (playedNote.Effect.IsSlapping)
                {
                    effect.IsSlapping = true;
                }
                if (playedNote.Effect.IsPopping)
                {
                    effect.IsPopping = true;
                }
            }

            int flags = 0;
            if (duration.IsDotted || duration.IsDoubleDotted)
            {
                flags |= 0x01;
            }
            if (voice.Index == 0 && beat.Chord != null)
            {
                flags |= 0x02;
            }
            if (voice.Index == 0 && beat.Text != null)
            {
                flags |= 0x04;
            }
            if (beat.Stroke.Direction != Stroke.StrokeNone)
            {
                flags |= 0x08;
            }
            else if (effect.TremoloBar != null || effect.IsTapping || effect.IsSlapping || effect.IsPopping || effect.IsFadeIn)
            {
                flags |= 0x08;
            }
            if (changeTempo)
            {
                flags |= 0x10;
            }
            if (!duration.Division.Equals(DivisionType.Normal))
            {
                flags |= 0x20;
            }
            if (voice.IsEmpty || voice.IsRestVoice)
            {
                flags |= 0x40;
            }
            WriteUnsignedByte(flags);

            if ((flags & 0x40) != 0)
            {
                WriteUnsignedByte((voice.IsEmpty ? 0x00 : 0x02));

            }
            WriteByte(ParseDuration(duration));
            if ((flags & 0x20) != 0)
            {
                WriteInt(duration.Division.Enters);
            }

            if ((flags & 0x02) != 0)
            {
                WriteChord(beat.Chord);
            }

            if ((flags & 0x04) != 0)
            {
                WriteText(beat.Text);
            }

            if ((flags & 0x08) != 0)
            {
                WriteBeatEffects(beat, effect);
            }

            if ((flags & 0x10) != 0)
            {
                WriteMixChange(measure.Header.Tempo);
            }
            int stringFlags = 0;
            if (!voice.IsRestVoice)
            {
                for (int i = 0; i < voice.Notes.Count; i++)
                {
                    Note playedNote = voice.Notes[i];
                    int str = (7 - playedNote.Str);
                    stringFlags |= (1 << str);
                }
            }
            WriteUnsignedByte(stringFlags);
            for (int i = 6; i >= 0; i--)
            {
                if ((stringFlags & (1 << i)) != 0)
                {
                    for (int n = 0; n < voice.Notes.Count; n++)
                    {
                        var playedNote = voice.Notes[n];
                        if (playedNote.Str == (6 - i + 1))
                        {
                            WriteNote(playedNote);
                            break;
                        }
                    }
                }
            }

            SkipBytes(2);
        }

        private void WriteNote(Note note)
        {
            //int flags = 0x20;
            int flags = (0x20 | 0x10);

            if (note.Effect.IsVibrato ||
                note.Effect.Bend != null ||
                note.Effect.IsSlide ||
                note.Effect.IsHammer ||
                note.Effect.IsPalmMute ||
                note.Effect.IsStaccato ||
                note.Effect.Trill != null ||
                note.Effect.Grace != null ||
                note.Effect.Harmonic != null ||
                note.Effect.TremoloPicking != null)
            {
                flags |= 0x08;
            }
            if (note.Effect.IsGhostNote)
            {
                flags |= 0x04;
            }
            if (note.Effect.IsHeavyAccentuatedNote)
            {
                flags |= 0x02;
            }
            if (note.Effect.IsAccentuatedNote)
            {
                flags |= 0x40;
            }
            WriteUnsignedByte(flags);

            if ((flags & 0x20) != 0)
            {
                int typeHeader = 0x01;
                if (note.IsTiedNote)
                {
                    typeHeader = 0x02;
                }
                else if (note.Effect.IsDeadNote)
                {
                    typeHeader = 0x03;
                }
                WriteUnsignedByte(typeHeader);
            }
            if ((flags & 0x10) != 0)
            {
                WriteByte((byte)(((note.Velocity - Velocities.MinVelocity) / Velocities.VelocityIncrement) + 1));
            }
            if ((flags & 0x20) != 0)
            {
                WriteByte((byte)note.Value);
            }
            SkipBytes(1);
            if ((flags & 0x08) != 0)
            {
                WriteNoteEffects(note.Effect);
            }
        }

        private sbyte ParseDuration(Duration duration)
        {
            sbyte value = 0;
            switch (duration.Value)
            {
                case Duration.Whole:
                    value = -2;
                    break;
                case Duration.Half:
                    value = -1;
                    break;
                case Duration.Quarter:
                    value = 0;
                    break;
                case Duration.Eighth:
                    value = 1;
                    break;
                case Duration.Sixteenth:
                    value = 2;
                    break;
                case Duration.ThirtySecond:
                    value = 3;
                    break;
                case Duration.SixtyFourth:
                    value = 4;
                    break;
            }
            return value;
        }

        private void WriteChord(Chord chord)
        {
            WriteBytes(new sbyte[] { 1, 1, 0, 0, 0, 12, 0, 0, -1, -1, -1, -1, 0, 0, 0, 0, 0 });
            WriteStringByte(chord.Name, 21);
            SkipBytes(4);
            WriteInt(chord.FirstFret);
            for (int i = 0; i < 7; i++)
            {
                WriteInt((i < chord.Strings.Length ? chord.GetFretValue(i) : -1));
            }
            SkipBytes(32);
        }

        private void WriteBeatEffects(Beat beat, NoteEffect effect)
        {
            int flags1 = 0;
            int flags2 = 0;

            if (effect.IsFadeIn)
            {
                flags1 |= 0x10;
            }
            if (effect.IsTapping || effect.IsSlapping || effect.IsPopping)
            {
                flags1 |= 0x20;
            }
            if (effect.TremoloBar != null)
            {
                flags2 |= 0x04;
            }
            if (beat.Stroke.Direction != Stroke.StrokeNone)
            {
                flags1 |= 0x40;
            }
            WriteUnsignedByte(flags1);
            WriteUnsignedByte(flags2);

            if ((flags1 & 0x20) != 0)
            {
                if (effect.IsTapping)
                {
                    WriteUnsignedByte(1);
                }
                else if (effect.IsSlapping)
                {
                    WriteUnsignedByte(2);
                }
                else if (effect.IsPopping)
                {
                    WriteUnsignedByte(3);
                }
            }
            if ((flags2 & 0x04) != 0)
            {
                WriteTremoloBar(effect.TremoloBar);
            }
            if ((flags1 & 0x40) != 0)
            {
                WriteUnsignedByte((beat.Stroke.Direction == Stroke.StrokeUp ? ToStrokeValue(beat.Stroke) : 0));
                WriteUnsignedByte((beat.Stroke.Direction == Stroke.StrokeDown ? ToStrokeValue(beat.Stroke) : 0));
            }
        }

        private void WriteNoteEffects(NoteEffect effect)
        {
            int flags1 = 0;
            int flags2 = 0;
            if (effect.Bend != null)
            {
                flags1 |= 0x01;
            }
            if (effect.IsHammer)
            {
                flags1 |= 0x02;
            }
            if (effect.Grace != null)
            {
                flags1 |= 0x10;
            }
            if (effect.IsStaccato)
            {
                flags2 |= 0x01;
            }
            if (effect.IsPalmMute)
            {
                flags2 |= 0x02;
            }
            if (effect.TremoloPicking != null)
            {
                flags2 |= 0x04;
            }
            if (effect.IsSlide)
            {
                flags2 |= 0x08;
            }
            if (effect.Harmonic != null)
            {
                flags2 |= 0x10;
            }
            if (effect.Trill != null)
            {
                flags2 |= 0x20;
            }
            if (effect.IsVibrato)
            {
                flags2 |= 0x40;
            }
            WriteUnsignedByte(flags1);
            WriteUnsignedByte(flags2);
            if ((flags1 & 0x01) != 0)
            {
                WriteBend(effect.Bend);
            }

            if ((flags1 & 0x10) != 0)
            {
                WriteGrace(effect.Grace);
            }

            if ((flags2 & 0x04) != 0)
            {
                WriteTremoloPicking(effect.TremoloPicking);
            }

            if ((flags2 & 0x08) != 0)
            {
                WriteByte((byte)1);
            }

            if ((flags2 & 0x10) != 0)
            {
                WriteByte((byte)1);
            }

            if ((flags2 & 0x20) != 0)
            {
                WriteTrill(effect.Trill);
            }

        }

        private void WriteBend(EffectBend bend)
        {
            int points = bend.Points.Count;
            WriteByte((byte)1);
            WriteInt(0);
            WriteInt(points);
            for (int i = 0; i < points; i++)
            {
                EffectBend.BendPoint point = (EffectBend.BendPoint)bend.Points[i];
                WriteInt((point.Position * GP_BEND_POSITION / EffectBend.MaxPositionLength));
                WriteInt((point.Value * GP_BEND_SEMITONE / EffectBend.SemitoneLength));
                WriteByte((byte)0);
            }
        }

        private void WriteTremoloBar(EffectTremoloBar tremoloBar)
        {
            int points = tremoloBar.Points.Count;
            WriteByte((byte)1);
            WriteInt(0);
            WriteInt(points);
            for (int i = 0; i < points; i++)
            {
                EffectTremoloBar.TremoloBarPoint point = (EffectTremoloBar.TremoloBarPoint)tremoloBar.Points[i];
                WriteInt((point.Position * GP_BEND_POSITION / EffectBend.MaxPositionLength));
                WriteInt((point.Value * (GP_BEND_SEMITONE * 2)));
                WriteByte((byte)0);
            }
        }

        private void WriteGrace(EffectGrace grace)
        {
            WriteUnsignedByte(grace.Fret);
            WriteUnsignedByte(((grace.Dynamic - Velocities.MinVelocity) / Velocities.VelocityIncrement) + 1);
            if (grace.Transition == EffectGrace.TransitionNone)
            {
                WriteUnsignedByte(0);
            }
            else if (grace.Transition == EffectGrace.TransitionSlide)
            {
                WriteUnsignedByte(1);
            }
            else if (grace.Transition == EffectGrace.TransitionBend)
            {
                WriteUnsignedByte(2);
            }
            else if (grace.Transition == EffectGrace.TransitionHammer)
            {
                WriteUnsignedByte(3);
            }
            WriteUnsignedByte(grace.Duration);
            WriteUnsignedByte((grace.IsDead ? 0x01 : 0) | (grace.IsOnBeat ? 0x02 : 0));
        }

        private void WriteTrill(EffectTrill trill)
        {
            WriteByte((byte)trill.Fret);
            if (trill.Duration.Value == Duration.Sixteenth)
            {
                WriteByte((byte)1);
            }
            else if (trill.Duration.Value == Duration.ThirtySecond)
            {
                WriteByte((byte)2);
            }
            else if (trill.Duration.Value == Duration.SixtyFourth)
            {
                WriteByte((byte)3);
            }
        }

        private void WriteTremoloPicking(EffectTremoloPicking tremoloPicking)
        {
            if (tremoloPicking.Duration.Value == Duration.Eighth)
            {
                WriteByte((byte)1);
            }
            else if (tremoloPicking.Duration.Value == Duration.Sixteenth)
            {
                WriteByte((byte)2);
            }
            else if (tremoloPicking.Duration.Value == Duration.ThirtySecond)
            {
                WriteByte((byte)3);
            }
        }

        private void WriteText(Text text)
        {
            WriteStringByteSizeOfInteger(text.Value);
        }

        private void WriteMixChange(Tempo tempo)
        {
            WriteByte((byte)0xff);
            for (int i = 0; i < 16; i++)
            {
                WriteByte((byte)0xff);
            }
            WriteByte((byte)0xff); //volume
            WriteByte((byte)0xff); //int pan
            WriteByte((byte)0xff); //int chorus
            WriteByte((byte)0xff); //int reverb
            WriteByte((byte)0xff); //int phaser
            WriteByte((byte)0xff); //int tremolo
            WriteStringByteSizeOfInteger(""); //tempo name
            WriteInt((tempo != null) ? tempo.Value : -1); //tempo value
            if ((tempo != null))
            {
                SkipBytes(1);
            }
            WriteByte((byte)1);
            WriteByte((byte)0xff);
        }

        private void WriteMarker(Marker marker)
        {
            WriteStringByteSizeOfInteger(marker.Title);
            WriteColor(marker.Color);
        }

        private void WriteColor(Color color)
        {
            WriteUnsignedByte(color.R);
            WriteUnsignedByte(color.G);
            WriteUnsignedByte(color.B);
            WriteByte((byte)0);
        }

        #endregion

        #region Helpers

        private List<string> ToCommentLines(String comments)
        {
            var lines = new List<string>();

            string line = comments;
            while (line.Length > sbyte.MaxValue)
            {
                string subline = line.Substring(0, sbyte.MaxValue);
                lines.Add(subline);
                line = line.Substring(sbyte.MaxValue);
            }
            lines.Add(line);

            return lines;
        }

        private byte ToChannelByte(short s)
        {
            return (byte)((s + 1) / 8);
        }

        private Channel[] MakeChannels(Song song)
        {
            Channel[] channels = new Channel[64];
            for (int i = 0; i < channels.Length; i++)
            {
                channels[i] = new Channel();
                channels[i].ChannelCode = ((short)i);
                channels[i].EffectChannel = ((short)i);
                channels[i].Instrument = ((short)24);
                channels[i].Volume = ((short)13);
                channels[i].Balance = ((short)8);
                channels[i].Chorus = ((short)0);
                channels[i].Reverb = ((short)0);
                channels[i].Phaser = ((short)0);
                channels[i].Tremolo = ((short)0);
            }

            var it = song.Tracks.GetEnumerator();
            while (it.MoveNext())
            {
                var track = it.Current;
                channels[track.Channel.ChannelCode].Instrument = (track.Channel.Instrument);
                channels[track.Channel.ChannelCode].Volume = (track.Channel.Volume);
                channels[track.Channel.ChannelCode].Balance = (track.Channel.Balance);
                channels[track.Channel.EffectChannel].Instrument = (track.Channel.Instrument);
                channels[track.Channel.EffectChannel].Volume = (track.Channel.Volume);
                channels[track.Channel.EffectChannel].Balance = (track.Channel.Balance);
            }

            return channels;
        }

        private sbyte[] MakeBeamEighthNoteBytes(TimeSignature ts)
        {
            sbyte[] bytes = new sbyte[] { 0, 0, 0, 0 };
            if (ts.Denominator.Value <= Duration.Eighth)
            {
                int eighthsInDenominator = (Duration.Eighth / ts.Denominator.Value);
                int total = (eighthsInDenominator * ts.Numerator);
                int byteValue = (total / 4);
                int missingValue = (total - (4 * byteValue));
                for (int i = 0; i < bytes.Length; i++)
                {
                    bytes[i] = (sbyte)byteValue;
                }
                if (missingValue > 0)
                {
                    bytes[0] += (sbyte)missingValue;
                }
            }
            return bytes;
        }

        private int ToStrokeValue(Stroke stroke)
        {
            if (stroke.Value == Duration.SixtyFourth)
            {
                return 2;
            }
            if (stroke.Value == Duration.ThirtySecond)
            {
                return 3;
            }
            if (stroke.Value == Duration.Sixteenth)
            {
                return 4;
            }
            if (stroke.Value == Duration.Eighth)
            {
                return 5;
            }
            if (stroke.Value == Duration.Quarter)
            {
                return 6;
            }
            return 2;
        }

        #endregion
    }
}
