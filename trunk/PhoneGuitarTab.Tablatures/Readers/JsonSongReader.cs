using System.Collections.Generic;
using System.Linq;
using PhoneGuitarTab.Tablatures.Models;
using Newtonsoft.Json.Linq;

namespace PhoneGuitarTab.Tablatures.Readers
{
    using PhoneGuitarTab.Tablatures.Helpers;
    using PhoneGuitarTab.Tablatures.Models.Effects;

    public class JsonSongReader
    {
        private const int DefaultTemp = 120;
        private Song _song;

        private readonly string _jsonSong;

        private TimeSignature _currentTimeSignature;

        private int _tempo;

        public JsonSongReader(string jsonSong)
        {
            _jsonSong = jsonSong;
            _song = new Song();
        }

        public Song ReadSong()
        {
            var json = JObject.Parse(_jsonSong);

            ParseHeader(json);
            ParseTracks(json);

            _song.MeasureHeaders = _song.Tracks.First().Measures.Select(m => m.Header).ToList();


            return _song;
        }


        private void ParseHeader(JObject json)
        {
            _song.Name = json["name"].SafeValue<string>();
            _song.Artist = json["artist"].SafeValue<string>();
            _tempo = json["tempo"].SafeValue<int>(DefaultTemp);
        }

        private void ParseTracks(JObject json)
        {
            _song.Tracks = new List<Track>();
            foreach (var jTrack in json["tracks"].Values<JObject>())
            {
                _song.Tracks.Add(ParseTrack(jTrack));
            }
        }

        private Track ParseTrack(JObject jTrack)
        {
            Track track = new Track()
                {
                    Name = jTrack["name"].SafeValue<string>(),
                    Offset = jTrack["offset"].SafeValue<int>(),
                    Channel = ParseChannel(jTrack),
                    Strings = ParseGuitarStrings(jTrack["strings"].Value<JArray>()),
                    Measures = ParseMeasures(jTrack["measures"].Value<JArray>())
                };
            return track;
        }

        private Channel ParseChannel(JObject jTrack)
        {
            return new Channel()
                {
                    //ChannelCode = jTrack["midiindex"].SafeValue<short>(),
                    Instrument = jTrack["midiindex"].SafeValue<short>(),
                    Volume = jTrack["volume"].SafeValue<short>(),
                };
        }

        private List<GuitarString> ParseGuitarStrings(JArray jStrings)
        {
            var guitarStrings = new List<GuitarString>();
            int counter = 0;
            foreach (var jString in jStrings)
            {
                guitarStrings.Add(new GuitarString() { Number = counter++, Value = jString.SafeValue<int>() });
            }

            return guitarStrings;
        }

        private List<Measure> ParseMeasures(JArray jMeasures)
        {
            List<Measure> measures = new List<Measure>();
            int counter = 0;
            foreach (JObject jMeasure in jMeasures)
            {
                var measure = ParseMeasure(jMeasure);
                measure.Header.Number = counter++;
                measures.Add(measure);
            }
            return measures;
        }

        private Measure ParseMeasure(JObject jMeasure)
        {
            if (jMeasure["numerator"] != null)
                _currentTimeSignature = new TimeSignature()
                    {
                        Numerator = jMeasure["numerator"].SafeValue<int>(),
                        Denominator = new Duration() { Value = jMeasure["denominator"].SafeValue<int>(), }
                    };
            _tempo = jMeasure["tempo"].SafeValue<int>(_tempo);
            MeasureHeader header = new MeasureHeader()
                {
                    IsRepeatOpen = jMeasure["repeatopen"].SafeValue<bool>(),
                    RepeatClose = jMeasure["repeatcount"].SafeValue<int>(),
                    Tempo = new Tempo() { Value = _tempo, },
                    TimeSignature = _currentTimeSignature
                };
            Measure measure = new Measure(header)
                {
                    Beats = ParseBeats(jMeasure["beats"].Value<JArray>())
                };
            return measure;
        }

        private List<Beat> ParseBeats(JArray jBeats)
        {
            List<Beat> beats = new List<Beat>();
            foreach (JObject jBeat in jBeats)
            {
                beats.Add(ParseBeat(jBeat));
            }
            return beats;
        }

        private Beat ParseBeat(JObject jBeat)
        {
            Beat beat = new Beat()
                {
                    Stroke = new Stroke() { Value = jBeat["stroke"].SafeValue<int>() },
                    Chord = null, //TODO
                    Start = jBeat["start"].SafeValue<long>(), // TODO not sure
                    Text = new Text() { Value = jBeat["text"].SafeValue<string>() },
                };
            beat.Voices = ParseVoices(beat, jBeat["voices"].Value<JArray>()).ToArray();
            return beat;
        }

        private List<Voice> ParseVoices(Beat beat, JArray jVoices)
        {
            List<Voice> voices = new List<Voice>();
            int index = 0;
            foreach (JObject jVoice in jVoices)
            {
                voices.Add(ParseVoice(beat, jVoice, index++));
            }
            return voices;
        }

        private Voice ParseVoice(Beat beat, JObject jVoice, int index)
        {
            Voice voice = new Voice(index)
                {
                    IsEmpty = jVoice["empty"].SafeValue<bool>(),
                    Beat = beat,
                    Duration = ParseDuration(jVoice),
                    Notes = ParseNotes(jVoice["notes"].Value<JArray>()),
                    Direction = jVoice["direction"].SafeValue<int>() // TODO not sure
                };

            return voice;
        }

        private Duration ParseDuration(JObject jVoice)
        {
            int value = 0;
            bool isDotted = jVoice["dotted"].SafeValue<bool>();
            int tuplet = jVoice["tuplet"].SafeValue<int>(1);
            var duration = (sbyte) jVoice["duration"].SafeValue<byte>();
            switch (duration)
            {
                case -2:
                    value = Duration.Whole;
                    break;
                case -1:
                    value = Duration.Half;
                    break;
                case 0:
                    value = Duration.Quarter;
                    break;
                case 1:
                    value = Duration.Eighth;
                    break;
                case 2:
                    value = Duration.Sixteenth;
                    break;
                case 3:
                    value = Duration.ThirtySecond;
                    break;
                case 4:
                    value = Duration.SixtyFourth;
                    break;
            }

            return new Duration()
                {
                    IsDotted = isDotted,
                    Value = value,
                    Division = new DivisionType()
                        {
                            Enters = tuplet
                        }
                };
        }

        private List<Note> ParseNotes(JArray jNotes)
        {
            List<Note> notes = new List<Note>();
            foreach (JObject jNote in jNotes)
            {
                notes.Add(ParseNote(jNote));
            }
            return notes;
        }

        private Note ParseNote(JObject jNote)
        {
            Note note = new Note()
                {
                    Effect = ParseEffect(jNote),
                    Str = jNote["string"].SafeValue<int>(),
                    Value = jNote["fret"].SafeValue<int>(),
                    IsTiedNote = default(bool), //TODO
                    Velocity = default(int) // TODO
                };
            return note;
        }

        private NoteEffect ParseEffect(JObject jNote)
        {
            var flag = jNote["effect"].SafeValue<int>();

            NoteEffect effect = new NoteEffect();

            if (flag == 4)
            {
                effect.Bend = new EffectBend();
                foreach (var jBendPoint in jNote["bend"].Value<JArray>())
                {
                    var x = jBendPoint["X"].SafeValue<int>();
                    var y = jBendPoint["Y"].SafeValue<int>();
                    effect.Bend.AddPoint(x, y);
                }
            }

            if (flag == 8)
            {
                effect.IsHammer = true;
            }

            if (flag == 512)
            {

            }

            if (flag == 2048)
            {
                effect.IsVibrato = true;
            }

            if (flag == 16)
            {
                effect.IsSlide = true;
            }
            
            //if (note.harmonic) {
               // effects.harmonic = {};
               // effects.harmonic.type = note.harmonic.type == 1 ? "" : note.harmonic.type;
               // effect.Harmonic = new EffectHarmonic();
            //}

            if (flag == 32768)
            {
                effect.TremoloPicking = new EffectTremoloPicking(); // TODO duration
            }
            return effect;

        }
    }
}
