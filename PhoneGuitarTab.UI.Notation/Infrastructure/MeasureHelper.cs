using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Linq;
using PhoneGuitarTab.Core.Tab;
using PhoneGuitarTab.UI.Notation.ViewModel;

namespace PhoneGuitarTab.UI.Notation.Infrastructure
{
    internal class MeasureHelper
    {

        public static int GetMeasureWidth(Core.Tab.Measure measure)
        {
            //performance
            int acc = 0;
            foreach (var beat in measure.Beats)
                acc += beat.Width = GetBeatWidth(beat);
            return acc + 20;
            //slow performance variant
            //return measure.Beats.Aggregate((double)0, (acc, b) => acc += b.Width = GetBeatWidth(b));
        }

        public static Ellipse GetDottedNote()
        {
            Ellipse ellipse = new Ellipse();
            ellipse.Width = 5;
            ellipse.Height = 5;
            ellipse.StrokeThickness = 4;
            ellipse.Fill = Constants.MeasureBrush;
            ellipse.Stroke = Constants.MeasureBrush;
            return ellipse;
        }

        public static Path GetBend(Point p1)
        {
            Path bend = new Path()
            {
                Stroke = Constants.EffectsBrush,
                StrokeThickness = 1
            };

            BezierSegment bezier = new BezierSegment()
            {
                Point1 = p1,
                Point2 = new Point(p1.X + 7, p1.Y - 2), //NOTE depends on the current sizes
                Point3 = new Point(p1.X + 7, p1.Y - 10)
            };

            Point pp1 = new Point(p1.X + 7, p1.Y - 16);
            Point pp2 = bezier.Point3;

            //Arrow
            GeometryGroup lineGroup = new GeometryGroup();

            double theta = Math.Atan2((pp2.Y - pp1.Y), (pp2.X - pp1.X)) * 180 / Math.PI;

            PathGeometry pathGeometry = new PathGeometry();
            PathFigure pathFigure = new PathFigure();
            pathFigure.StartPoint = pp1;

            Point lpoint = new Point(pp1.X + 2, pp1.Y + 6);
            Point rpoint = new Point(pp1.X - 2, pp1.Y + 6);
            LineSegment seg1 = new LineSegment();
            seg1.Point = lpoint;
            pathFigure.Segments.Add(seg1);

            LineSegment seg2 = new LineSegment();
            seg2.Point = rpoint;
            pathFigure.Segments.Add(seg2);

            LineSegment seg3 = new LineSegment();
            seg3.Point = pp1;
            pathFigure.Segments.Add(seg3);

            pathGeometry.Figures.Add(pathFigure);
            RotateTransform transform = new RotateTransform();
            transform.Angle = theta - 90;
            transform.CenterX = pp1.X;
            transform.CenterY = pp1.Y;
            pathGeometry.Transform = transform;
            lineGroup.Children.Add(pathGeometry);
            //end arrow

            PathFigure figure = new PathFigure() { StartPoint = p1 };
            figure.Segments.Add(bezier);

            PathGeometry geometry = new PathGeometry();
            geometry.Figures.Add(figure);

            lineGroup.Children.Add(geometry);

            bend.Data = lineGroup;//geometry;
            bend.HorizontalAlignment = HorizontalAlignment.Center;
            return bend;

        }

        public static Line GetSlide(Point p1)
        {
           return new Line()
                {

                    X1 = p1.X+Constants.NoteFontSize/2,
                    Y1 = p1.Y+Constants.NoteFontSize-5,
                    X2 = p1.X + Constants.NoteFontSize / 2+5,
                    Y2 = p1.Y+5,
                    Stroke = Constants.MeasureBrush
                };
        }

        public static Path GetLegato(Point p1)
        {
            Path legato = new Path()
            {
                Stroke = Constants.EffectsBrush,
                StrokeThickness = 1
            };

            BezierSegment bezier = new BezierSegment()
            {
                Point1 = p1,
                Point2 = new Point(p1.X + 9, p1.Y-12), //NOTE depends on the current sizes
                Point3 = new Point(p1.X + 22, p1.Y)
            };
            PathFigure figure = new PathFigure() { StartPoint = p1};
            figure.Segments.Add(bezier);
            PathGeometry geometry = new PathGeometry();
            geometry.Figures.Add(figure);

            legato.Data = geometry;
            legato.HorizontalAlignment = HorizontalAlignment.Center;
            return legato;
        }

        public static WriteableBitmap GetMeasureImage(Core.Tab.Measure measure)
        {
            if (measure == null)
                return null;
            //TODO Fix it!
            WriteableBitmap bitmap = new WriteableBitmap
                (((int)MeasureHelper.GetMeasureWidth(measure)), (6 * Constants.NoteHeight) + Constants.StemHeight + 10);

            //render measure border
            bitmap.Render(new Line()
            {
                X1 = 0,
                Y1 = 12,
                X2 = 0,
                Y2 = 30 + (measure.StringsNumber - 1) * Constants.NoteHeight - 18, //NOTE some magic numbers
                Stroke = Constants.MeasureBrush,
                StrokeThickness = 3
            }, null);

            #region Measure variables
            //notes
            int horizontalOffset = 0;

            //beams
            double beamAcc = 0;
            double beamSize = 1f / measure.DenominatorSignature;
            int tupletsCounter = 0;
            Beat startBeat = null;
            int startBeatOffset = 0;
            int beatsCount = measure.Beats.Count;
            #endregion

            for (int i = 0; i < beatsCount; i++)
            {
                var currentBeat = measure.Beats[i];

                #region Processing notes
                int verticalOffset = -4;
                int centerX = (int)currentBeat.Width / 2;
                var middleBeat = horizontalOffset + centerX;
                int lastNoteVerticalOffset = 0;
                foreach (var note in currentBeat.Notes)
                {

                    //render fret
                    if (!String.IsNullOrEmpty(note.Fret))
                    {
                        TextBlock txt1 = new TextBlock();
                        txt1.Text = note.Fret;
                        //slide
                        if (note.Slide != null)
                            txt1.Text += "/";

                        txt1.FontSize = Constants.NoteFontSize;
                        txt1.Foreground = Constants.MeasureBrush;
                        bitmap.Render(txt1, new TranslateTransform()
                        {
                            X = middleBeat,
                            Y = verticalOffset
                        });
                        lastNoteVerticalOffset = verticalOffset;
                    }

                    //render legato
                    if (note.IsLegato)
                        bitmap.Render(MeasureHelper.GetLegato(
                            new Point(middleBeat + Constants.NoteSizeOffset, verticalOffset)),
                                      null);
                    if (note.Bend != null)
                        bitmap.Render(MeasureHelper.GetBend(
                            new Point(middleBeat + Constants.NoteSizeOffset+4, verticalOffset+4)),
                                      null);
                    //NOTE see above
                    //if (note.Slide != null)
                    //    bitmap.Render(MeasureHelper.GetSlide(
                    //        new Point(middleBeat + Constants.NoteSizeOffset + 4, verticalOffset + 4)),
                    //                  null);
                  

                    verticalOffset += Constants.NoteHeight;
                }
                #endregion

                #region Processing steams
                bitmap.Render(new Line()
                {

                    X1 = middleBeat + Constants.NoteSizeOffset,
                    Y1 = lastNoteVerticalOffset == 0 ? verticalOffset : lastNoteVerticalOffset + Constants.NoteHeight,
                    X2 = middleBeat + Constants.NoteSizeOffset,
                    Y2 = verticalOffset + Constants.StemHeight,
                    Stroke = Constants.MeasureBrush
                }, null);
                #endregion

                horizontalOffset += (int)currentBeat.Width;

                #region Processing beams
                //set beam marks

                var currentDuration = MeasureHelper.GetDurationValue(currentBeat.Duration);
                if (currentBeat.IsDotted)
                    currentDuration += (currentDuration / 2);

                #region START processing tuplets
                if (currentBeat.Tuplet != 0)
                {
                    tupletsCounter++;
                    if (tupletsCounter == 1)
                        currentBeat.Beam = BeamType.Begin;

                    else if (tupletsCounter < currentBeat.Tuplet)
                        currentBeat.Beam = BeamType.Continue;

                    if (tupletsCounter == currentBeat.Tuplet)
                    {
                        tupletsCounter = 0;
                        currentBeat.Beam = BeamType.End;
                    }
                    goto beam;
                }
                #endregion END tuplets processing

                //greater than 1/8
                if (currentDuration > 0.125)
                {
                    beamAcc = 0;
                    currentBeat.Beam = BeamType.Single;
                    goto beam;
                }
                //last beat processing
                if ((i + 1) >= beatsCount)
                {
                    if (i == 0)
                        currentBeat.Beam = BeamType.Single;
                    else
                        //test previous beat
                        currentBeat.Beam = MeasureHelper.GetCloseBeam(measure.Beats[i - 1]);
                    goto beam;
                }

                var nextBeat = measure.Beats[i + 1];
                var nextDuration = MeasureHelper.GetDurationValue(nextBeat.Duration);
                //next > current
                if (nextDuration > currentDuration)
                {
                    if (i != 0)
                        currentBeat.Beam = MeasureHelper.GetCloseBeam(measure.Beats[i - 1]);
                    else
                        currentBeat.Beam = BeamType.Single;
                    beamAcc = 0;
                    goto beam;
                }

                //next >= current
                //1. acc+next > beamSize (e.g. 1/4)
                //NOTE consider additional condition to beam notes
                if ((beamAcc + nextDuration) >= beamSize)
                {
                    if (i != 0)
                        currentBeat.Beam = MeasureHelper.GetCloseBeam(measure.Beats[i - 1]);
                    else
                        currentBeat.Beam = BeamType.Single;
                    beamAcc = 0;
                    goto beam;
                }
                //acc=0
                if ((beamAcc == 0) && ((beamAcc + nextDuration) <= beamSize))
                {
                    currentBeat.Beam = BeamType.Begin;
                    beamAcc = currentDuration;
                    goto beam;
                }
                //acc + next < beamSize
                if ((beamAcc + nextDuration) < beamSize)
                {
                    currentBeat.Beam = BeamType.Continue;
                    beamAcc += currentDuration;
                    goto beam;
                }

            beam:
                switch (currentBeat.Beam)
                {
                    case BeamType.Single:
                        //draw single
                        var singleLinesCount = MeasureHelper.GetBeamLineCount(currentBeat.Duration);
                        for (int beamsLines = 0; beamsLines < singleLinesCount; beamsLines++)
                            bitmap.Render(new Line()
                            {
                                X1 = middleBeat + Constants.NoteSizeOffset,
                                Y1 = verticalOffset + Constants.StemHeight - beamsLines * 6,
                                X2 = middleBeat + Constants.NoteSizeOffset + 6,
                                Y2 = verticalOffset + Constants.StemHeight - beamsLines * 6,
                                Stroke = Constants.MeasureBrush,
                                StrokeThickness = 3
                            }, null);
                        //draw dotted with magic
                        if (currentBeat.IsDotted)
                            bitmap.Render(MeasureHelper.GetDottedNote(),
                                          new TranslateTransform()
                                          {
                                              X = middleBeat + Constants.NoteFontSize / 2,
                                              Y = verticalOffset + Constants.StemHeight - (singleLinesCount - 1) * 6 - 12
                                          });

                        break;
                    case BeamType.Begin:
                        startBeat = currentBeat;
                        startBeatOffset = middleBeat + Constants.NoteSizeOffset;
                        break;
                    case BeamType.Continue:
                        continue;
                    case BeamType.End:
                        if (startBeat == null)
                            throw new InvalidOperationException("null start beat");
                        var linesCount = MeasureHelper.GetBeamLineCount(startBeat.Duration);
                        for (int beamsLines = 0; beamsLines < linesCount; beamsLines++)
                        {
                            bitmap.Render(new Line()
                            {
                                X1 = startBeatOffset,
                                Y1 = verticalOffset + Constants.StemHeight - beamsLines * 6,
                                X2 = middleBeat + Constants.NoteSizeOffset,
                                Y2 = verticalOffset + Constants.StemHeight - beamsLines * 6,
                                Stroke = Constants.MeasureBrush,
                                StrokeThickness = 3
                            }, null);
                        }

                        if (startBeat.Tuplet != 0)
                            bitmap.Render(new TextBlock() { Text = startBeat.Tuplet.ToString() },
                                      new TranslateTransform()
                                      {
                                          X = (startBeatOffset + middleBeat + 6) / 2,
                                          Y = verticalOffset + Constants.StemHeight,
                                      }
                                );
                        break;

                }
                #endregion
            }

            bitmap.Invalidate();
            return bitmap;
        }


        public static int GetBeatWidth(Core.Tab.Beat beat)
        {
            double scale = 1.5;
            switch (beat.Duration)
            {
                case Core.Tab.Duration.Whole:
                    return (int)(45 * scale);
                case Core.Tab.Duration.Half:
                    return (int)(37 * scale);
                case Core.Tab.Duration.Quarter:
                    return (int)(30 * scale);
                case Core.Tab.Duration.Eighth:
                    return (int)(25 * scale);
                case Core.Tab.Duration.Sixteenth:
                    return (int)(23 * scale);
                case Core.Tab.Duration.ThirtySecond:
                    return (int)(20 * scale);
                default:
                    return (int)(20 * scale);
            }
        }

        public static int GetBeamLineCount(Core.Tab.Duration duration)
        {
            switch (duration)
            {
                case Core.Tab.Duration.Eighth:
                    return 1;
                case Core.Tab.Duration.Sixteenth:
                    return 2;
                case Core.Tab.Duration.ThirtySecond:
                    return 3;
                case Core.Tab.Duration.SixtyFourth:
                    return 4;
                case Core.Tab.Duration.HundredTwentyEighth:
                    return 5;
                default:
                    return 0;
            }
        }

        internal static BeamType GetCloseBeam(Beat previous)
        {
            switch (previous.Beam)
            {
                case BeamType.Begin:
                case BeamType.Continue:
                    return BeamType.End;
                default:
                    return BeamType.Single;
            }
        }

        internal static double GetDurationValue(Core.Tab.Duration duration)
        {
            switch(duration)
            {
                case Core.Tab.Duration.Quarter:
                    return 0.25;
                case Core.Tab.Duration.Eighth:
                    return 0.125;
                case Core.Tab.Duration.Sixteenth:
                    return 0.0625;
                case Core.Tab.Duration.ThirtySecond:
                    return 0.03125;
                case Core.Tab.Duration.SixtyFourth:
                    return 0.015625;
                case Core.Tab.Duration.HundredTwentyEighth:
                    return 0.0078125;
                default:
                    return 0;
            }
        }
    }
}
