using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;

namespace FNFJSON.Extensions
{
    public static class SongExtensions
    {
        /// <summary>
        /// Scales an entire song, 0.5 is double speed and 2 is half speed.
        /// </summary>
        public static void ScaleSong(this Song s, float scale, bool changebpm = true)
        {
            if (changebpm)
            {
                s.bpm = (int)(s.bpm / scale);
            }
            foreach (Section se in s.Sections)
            {
                List<Note> ns = new List<Note>();
                se.bpm = (int)(se.bpm / scale);
                foreach (Note n in se.ConvertSectionToNotes())
                {
                    n.StrumTime = (int)Math.Round((n.StrumTime * scale));
                    n.SustainLength = n.SustainLength * scale;
                    ns.Add(n);
                }
                se.SaveNotes(ns);
            }
        }



    }

    public static class NoteExtensions
    {
        /// <summary>
        /// Converts a note back into its raw data, mustHitSection needs to be the same as the mustHitSection bool in the section you plan adding this note to or it will cause the wrong side to play the note!
        /// </summary>
        public static float[] ToRawData(this Note n, bool mustHitSection)
        {
            return new float[3]
                {
                    n.StrumTime,
                    (float)((float)n.NoteData + ((n.GottaHit == mustHitSection) ? 0 : 4)),
                    n.SustainLength
                };
        }
    }



    public static class SectionExtensions //this is unused
    {
        
    }

}

namespace FNFJSON
{
    public static class EnumExtensions
    {

        /// <summary>
        /// Flips notes like in B-Sides. (Left becomes Right, Up becomes Down, etc.)
        /// </summary>
        public static NoteType Flip(this NoteType n)
        {
            switch (n)
            {
                case NoteType.Down:
                    return NoteType.Up;
                case NoteType.Up:
                    return NoteType.Down;
                case NoteType.Right:
                    return NoteType.Left;
                case NoteType.Left:
                    return NoteType.Right;
            }
            return NoteType.Left;
        }


        /// <summary>
        /// Inverts the notes, Left notes become Up notes, Right notes become down notes, and vice versa.
        /// </summary>
        public static NoteType Invert(this NoteType n)
        {
            switch (n)
            {
                case NoteType.Down:
                    return NoteType.Left;
                case NoteType.Up:
                    return NoteType.Right;
                case NoteType.Right:
                    return NoteType.Up;
                case NoteType.Left:
                    return NoteType.Down;
            }
            return NoteType.Left;
        }

    }
}
