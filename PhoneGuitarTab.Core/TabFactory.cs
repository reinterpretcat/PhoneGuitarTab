using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using PhoneGuitarTab.Core.Tab;
using PhoneGuitarTab.GuitarPro;
using Beat = PhoneGuitarTab.Core.Tab.Beat;
using Bend = PhoneGuitarTab.Core.Tab.Bend;
using Measure = PhoneGuitarTab.Core.Tab.Measure;
using Note = PhoneGuitarTab.Core.Tab.Note;
using Slide = PhoneGuitarTab.Core.Tab.Slide;
using Track = PhoneGuitarTab.Core.Tab.Track;


namespace PhoneGuitarTab.Core
{
    /// <summary>
    /// Used for creating TabFile from different formats
    /// </summary>
    public class TabFactory
    {

        public static TabFile CreateFromGp(Stream stream)
        {
            TabFile file = new TabFile();

            var gpFile = PhoneGuitarTab.GuitarPro.GpFactory.CreateFile(stream);
            int tracksCount = gpFile.Body.Tracks.Length;
            int measureTrackPairsCount = gpFile.Body.MeasureTrackPairs.Length;
            int measureCount = gpFile.Body.Measures.Length;
            //tracks init
            for (int i = 0; i < tracksCount; i++)
            {
                //TODO: init track header
                Track track = new Track
                                  {
                                      Index = i,
                                      Name = gpFile.Body.Tracks[i].Name,
                                      IsDrum = gpFile.Body.Tracks[i].IsDrumsTrack,
                                      StringNumber = gpFile.Body.Tracks[i].StringNumber
                                  };
                for (int j = 0; j < measureCount; j++)
                {
                   // if ((j + i) % tracksCount != 0)
                   //     continue;//other track measure
                    //TODO init measure
                    Measure measure = new Measure();
                    measure.NumeratorSignature = gpFile.Body.Measures[j].NumeratorSignature;
                    measure.DenominatorSignature = gpFile.Body.Measures[j].DenominatorSignature;
                    measure.StringsNumber = gpFile.Body.Tracks[i].StringNumber; //TODO: ambiguous usage
                    foreach (var gpBeat in gpFile.Body.MeasureTrackPairs[j*tracksCount+i].Beats)
                    {
                        //TODO init beat
                        Beat beat = new Beat();
                        beat.Duration = (Core.Tab.Duration) gpBeat.Duration;
                        beat.Tuplet = gpBeat.NTuplet;
                        beat.IsDotted = gpBeat.DottedNotes;
                        //TODO this is for 6-strings only
                        var stringCount = gpBeat.Strings.Length;
                        beat.Notes = new List<Note>();
                        for (int s = 0; s < stringCount;s++ )
                            beat.Notes.Add(new Note() { Fret = "" });
                          
                        int noteIndex = 0;
                        for (int stringIndex = stringCount -1; stringIndex >= 0; stringIndex--)
                        {
                            if (!gpBeat.Strings[stringIndex])
                                continue;
                            
                            var effects = gpBeat.Notes[noteIndex].Effects ?? new EffectsOnNote();
                            beat.Notes[stringCount - stringIndex - 1] = new Note()
                                                                            {
                                                                                Fret =
                                                                                    gpBeat.Notes[noteIndex].FretNumber.
                                                                                    ToString(),
                                                                                IsLegato = effects.HammerOnPullOff,
                                                                                Bend = effects.Bend==null?null:new Bend(),
                                                                                Slide = effects.Slide == 
                                                                                PhoneGuitarTab.GuitarPro.Slide.NoSlide ? null : new Slide()
                                                                            };
                            noteIndex++;
                        }
                        //TODO: Remove feature
                        if (stringCount >6)
                            beat.Notes.RemoveAt(6);
                        measure.Beats.Add(beat);
                    }
                    track.Measures.Add(measure);
                }
                file.Tracks.Add(track);
            }
            return file;
        }
    }
}
