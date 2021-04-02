using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;

namespace FNFJSON
{

    /// <summary>
    /// Note Direction/Color
    /// </summary>
    public enum NoteType : int
    {
        Left = 0,
        Up = 2,
        Down = 1,
        Right = 3
    }

    /// <summary>
    /// Do not use this class for anything at all unless you want to serialize the songs yourself for some reason.
    /// </summary>
    public class SongHolder
    {
        /// <summary>
        /// The actual song contained within in the song holder.
        /// </summary>
        public Song song; //this is so fucking dumb, why do you force me to do this ninjamuffin
        public SongHolder()
        {
        }

        public SongHolder(Song son)
        {
            song = son;
        }

    }

    /// <summary>
    /// The main Song class, contains information like the song name, the bpm, the characters, etc.
    /// Note data is stored in the Sections list as a list of Section classes.
    /// </summary>
    public class Song
    {
        [JsonProperty("song")]
        public string Name = "Song";
        public int bpm = 100;
        /// <summary>
        /// A seemingly unused variable.
        /// </summary>
        [JsonProperty("sections")]
        public int SectionAmount;
        public bool needsVoices = true;
        /// <summary>
        /// Player 1's Character(Usually BF or some variation)
        /// </summary>
        [JsonProperty("player1")]
        public string Player1Char = "bf";
        /// <summary>
        /// Player 2's Character(Usually the foe BF is fighting, EX: dad, mom
        /// </summary>
        [JsonProperty("player2")]
        public string Player2Char = "dad";
        /// <summary>
        /// Another unused variable
        /// </summary>
        public int[] sectionLengths = new int[1]; //wtf is this
        /// <summary>
        /// Determines whether or not the song's high score will be saved.
        /// </summary>
        public bool validScore = true;
        /// <summary>
        /// Speed of the notes, not the speed of the song.
        /// </summary>
        public float speed = 1f;
        /// <summary>
        /// The "sections" for the song.
        /// </summary>
        [JsonProperty("notes")]
        public List<Section> Sections = new List<Section>(); //this was named stupidly, so i changed it


        /// <summary>
        /// Create a completely blank song that uses default data.
        /// </summary>
        public Song()
        {
        }

        /// <summary>
        /// Create a song from a list of notes, which is automatically split into sections for you. Currently bugged.
        /// </summary>
        public Song(string name, int bpmm, List<Note> Notes)
        {
            Name = name;
            bpm = bpmm;
            int secid = 0;
            Section cursec = new Section(); //start off with a nil current section.
            List<Note> notestosave = new List<Note>();
            int dadnotes = 0;
            int bfnotes = 0;
            for (int i=0; i < Notes.Count; i++) //iterate through every note
            {
                int nexindex = CalculateSectionStart(secid + 1); //calculate the start of the next section
                if (Notes[i].GottaHit)
                {
                    bfnotes++;
                }
                else
                {
                    dadnotes++;
                }
                if (Notes[i].StrumTime >= nexindex || (i == (Notes.Count - 1) && Sections.Count == 0)) //if the starting time of the note is greater then the start of the next section put it into the next section
                {
                    notestosave.Add(Notes[i]);
                    secid++; //increase the sec id
                    cursec.mustHitSection = (bfnotes >= dadnotes); //try to figure out who the section is for, and assign it as such
                    bfnotes = 0;
                    dadnotes = 0;
                    cursec.SaveNotes(notestosave); //save the notes after the section gets the person assigned so the notedata gets handled properly
                    notestosave = new List<Note>();
                    Sections.Add(cursec);
                    cursec = new Section();
                }
                notestosave.Add(Notes[i]);
            }
            Console.WriteLine(Sections.Count);
        }

        /// <summary>
        /// Calculates the start of the section with the provided index. Used primary by <see cref="FNFJSON.Song(System.String,System.Int32,List{FNFJSON.Note})"/> to calculate when to split into a seperate section.
        /// </summary>
        public int CalculateSectionStart(int index) //converted to c# directly from the games code and should output the exact same stuff.
        {
            int curBPM = this.bpm;
            float curPos = 0f;
            for (int i = 0; i < (index + 1); i++)
            {
                if (i < Sections.Count)
                {
                    if (Sections[i].changeBPM)
                    {
                        curBPM = Sections[i].bpm;
                    }
                }
                curPos += 4 * (1000 * 60 / curBPM);
            }
            return (int)curPos;
        }


        /// <summary>
        /// Displays the song in a nice, simple "{SongName} - {BPM} BPM" format.
        /// </summary>
        public override string ToString()
        {
            return this.Name + " - " + this.bpm + " BPM";
        }


        /// <summary>
        /// Loads a song from the path provided.
        /// </summary>
        public static Song LoadFromFile(string path)
        {
            string data = File.ReadAllText(path);
            return ReadFromJson(data);
        }


        /// <summary>
        /// Saves the song to a file, overriding if it already exists.
        /// </summary>
        public static bool SaveToFile(string path, Song son)
        {
            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path); //delete
                }
                File.WriteAllText(path, JsonConvert.SerializeObject(new SongHolder(son)));
            }
            catch
            {
                return false;   
            }
            return true;
        }


        /// <summary>
        /// Reads a song from a json file
        /// </summary>
        public static Song ReadFromJson(string data)
        {
            return JsonConvert.DeserializeObject<SongHolder>(data).song;
        }
    }


    /// <summary>
    /// Sections are usually 16 note pieces of a song that contain the note data. You can edit the raw note data but its recommended to edit use the provided functions to get higher level classes.
    /// </summary>
    public class Section
    {
        public List<float[]> sectionNotes;
        /// <summary>
        /// If true, Player 1(bf) is playing, otherwise, Player 2(dad/foe) is playing.
        /// </summary>
        public bool mustHitSection = false;
        /// <summary>
        /// Completely unused variable.
        /// </summary>
        public int typeOfSection;
        /// <summary>
        /// BPM, only takes effect if changeBPM is true.
        /// </summary>
        public int bpm = 100;
        /// <summary>
        /// If set to true it will change the BPM to the BPM provided in the bpm variable for this section.
        /// </summary>
        public bool changeBPM;
        /// <summary>
        /// Whether or not the foe will use their alternate animation when playing their notes. Used in Week 5 for Mom and Dad.
        /// </summary>
        public bool altAnim;
        /// <summary>
        /// The section length, unsure if this works.
        /// </summary>
        public int lengthInSteps = 16;


        /// <summary>
        /// Creates a completely blank section
        /// </summary>
        public Section()
        {

        }


        /// <summary>
        /// Creates a section based off a list of notes.
        /// </summary>
        public Section(bool musthit, List<Note> notes)
        {
            mustHitSection = musthit;
            SaveNotes(notes);
        }

        public override string ToString()
        {
            return "Section: " + lengthInSteps + ":" + sectionNotes.Count;
        }


        /// <summary>
        /// Converts the raw note data into a note array for easy modification/viewing.
        /// </summary>
        public Note[] ConvertSectionToNotes()
        {
            List<Note> notelist = new List<Note>();
            foreach (float[] data in sectionNotes)
            {
                notelist.Add(new Note(data, mustHitSection));
            }
            return notelist.ToArray();
        }



        /// <summary>
        /// Saves the notes to the section, overriding the already existing data.
        /// </summary>
        public void SaveNotes(List<Note> data)
        {
            List<float[]> notelist = new List<float[]>();
            foreach (Note n in data)
            {
                notelist.Add(new float[3]
                {
                    n.StrumTime,
                    (float)((float)n.NoteData + ((n.GottaHit == mustHitSection) ? 0 : 4)),
                    n.SustainLength
                });
            }
            sectionNotes = notelist; //save
        }

    }

    /// <summary>
    /// Notes contain various data about themselves, like when they play, how long the note will be held for, or who will play the note
    /// </summary>
    public class Note
    {
        /// <summary>
        /// How far a note is in the song in miliseconds.(Not relative to the start of a section)
        /// </summary>
        public int StrumTime = 0;
        /// <summary>
        /// The NoteType of the Note, can be changed to change which direction the note is in.
        /// </summary>
        public NoteType NoteData = NoteType.Up;
        /// <summary>
        /// If true, Player 1(bf) will play this note, otherwise, Player 2(dad/foe) will play this note.
        /// </summary>
        public bool GottaHit = true;
        /// <summary>
        /// How long the note will be held/sustained for. In miliseconds.
        /// </summary>
        public float SustainLength;
        public bool isSustainNote { //this is usually set by hand in game but that's dumb
           get
           {
               return SustainLength > 0f;
           }
        }
        public Note()
        {

        }

        /// <summary>
        /// Says whether or not you hold the note and the note's direction
        /// </summary>
        public override string ToString()
        {
            return isSustainNote ? "Single-hit " : "Sustain " + NoteData.ToString();
        }

        /// <summary>
        /// Clone an already existing note. For convience.
        /// </summary>
        public Note(Note note)
        {
            StrumTime = note.StrumTime;
            NoteData = note.NoteData;
            GottaHit = note.GottaHit;
            SustainLength = note.SustainLength;
        }

        /// <summary>
        /// Creates a note class by hand.
        /// </summary>
        public Note(int strumtime, NoteType direction, bool gottahit, float sustain = 0f)
        {
            StrumTime = strumtime;
            NoteData = direction;
            GottaHit = gottahit;
            SustainLength = sustain;
        }


        /// <summary>
        /// Create a note from raw note data.
        /// </summary>
        public Note(float[] notedata, bool musthit = true)
        {
            StrumTime = (int)notedata[0];
            NoteData = (NoteType)(((int)notedata[1]) % 4);
            GottaHit = musthit;
            if (notedata[1] > 3)
            {
                GottaHit = !GottaHit;
            }
            SustainLength = notedata[2];
        }
    }
}
