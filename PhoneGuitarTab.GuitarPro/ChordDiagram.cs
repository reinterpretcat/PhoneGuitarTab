using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PhoneGuitarTab.GuitarPro
{
    public class ChordDiagram
    {
        /**
         *Determines if the chord is displayed sharp or flat.
        */
        public bool IsSharp { get; set; }
        /**
         *The root of the Diagram (-1 for the customized chords, 0 when root is C,1 = C#...)
         */
        public ChordNote Root { get; set; }

         /**
        *Determines the chord type as followed:
        * - 0: M
        * - 1: 7
        * - 2: 7M
        * - 3: 6
        * - 4: m
        * - 5: m7
        * - 6: m7M
        * - 7: m6
        * - 8: sus2
        * - 9: sus4
        * - 10: 7sus2
        * - 11: 7sus4
        * - 12: dim
        * - 13: aug
        * - 14: 5
        */
        public ChordType ChordType { get; set; }

        /**
        *Nine, Eleven of Thirteen: byte
        *Determines if the chord goes until the ninth, the eleventh, or the thirteenth.
        */
        public int NineElevenThirteen { get; set; }

        /**
         *Bass: Integer
         *Lowest note of the chord. It gives the chord inversions.
         */
        public ChordNote Bass { get; set; }

        /**
         * Diminished/Augmented Integer
         * Tonality linked with 9/11/13:
         * 0: perfect ("juste")
         * 1: augmented
         * 2: diminished
         */
        public TonalityType TonalityType { get; set; }

        /**
         * add: byte
         * Allows to determine if a 'add' (added note) is present in the chord.
         */
        public int AddedNote { get; set; }

        /**
         * Name: String
         * 20 characters long string containing the chord name.
         */
        public string Name { get; set; }

        /**
         * Fifth: byte
         * Tonality of the fifth:
         * 0: perfect ("juste")
         * 1: augmented
         * 2: diminished
         */
        public TonalityType TonalityFive { get; set; }

        /**
         * Ninth: byte
         * Tonality of the ninth:
         * 0: perfect ("juste")
         * 1: augmented
         * 2: diminished
         * this tonality is valid only if the value "Nine, Eleven or Thirteen" is 11 or 13.
         */
        public TonalityType TonalityNine { get; set; }

        /**
         * Eleventh: byte
         * Tonality of the eleventh:
         * 0: perfect ("juste")
         * 1: augmented
         * 2: diminished
         * this tonality is valid only if the value "Nine, Eleven or Thirteen" is 13.
         */
        public TonalityType TonalityEleven { get; set; }

        /**
         * Base fret: Integer
         * Shows the fret from which the chord is displayed.
         */
        public int BaseFret { get; set; }

        /**
         * Frets: List of 7 integers.
         * Corresponds to the fret number played on each string, from the highest to the lowest.
         * -1 means a string unplayed.
         * 0 means a string played "blank" (ie no fret).
         */
        public int[] Frets { get; set; }

        /**
         * Number of barres: byte
         * Indicates the number of barres there are in the chord. A chord can contain up to 5 barres.
         */
        public int NumBarres { get; set; }

        /**
         * Fret of the barre: List of 5 Bytes
         * Indicates the fret number of each possible barre.
         */
        public int[] FretOfBarres { get; set; }

        /**
         * Barre start: List of 5 Bytes
         * Indicates the first string of the barre, 1 being the highest.
         * The list order matches the chord different barres frets list order.
         */

        public int[] BarreStarts { get; set; }

        /**
         * Barre end: List of 5 Bytes
         * Indicates the first string of the barre, 1 being the lowest.
         * The list order matches the chord different barres frets list order.
         */
        public int[] BarreEnds { get; set; }

        /**
         * Fingering: List of 7 Bytes
         * Describes the fingering used to play the chord.
         * Below is given the fingering from the highest string to the lowest:
         * -2: unknown;
         * -1: X or 0 (no finger);
         * 0: thumb;
         * 1: index;
         * 2: middle finger;
         * 3: annular;
         * 4: little finger.
         */
        public Fingering[] Fingering { get; set; }

        /**
         * ShowDiagFingering: byte
         * if it is 0x01, the chord fingering is displayed.
         * if it is 0x00, the chord fingering is masked.
         */
        public bool ChordFingeringDisplayed { get; set; }

        public ChordDiagram()
        {
            FretOfBarres = new int[5];
            BarreStarts = new int[5];
            BarreEnds = new int[5];
            Fingering = new Fingering[7];
            Frets = new int[7];
        }
    }
}
