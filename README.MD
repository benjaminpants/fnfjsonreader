# FNF Json Reader for C#

A tool designed for making the reading and editing of FNF Song Data easy. Completely functional, but still in development, so functions might change or move around.

Source coding coming when I figure out how to properly split the solution and seperate it from FNF JSON Tester and FNF Song Modifier

## Features
- Deserialize and Serialize FNF Song Data
- Compatibility with base FNF and Kade Engine
- Various helper functions

Also its pretty cool and has a lot of summaries for functions.
***
## Info/How To Use
You need [Newtonsoft JSON][nt] for FNFJSON to work. You can install it via Nuget or add it as a DLL reference by hand.

FNFJSON is the main library, you can obviously import it with
```cs
using FNFJSON;
using System.Collections.Generic;
```
To import the extensions, you can also do:
```cs
using FNFJSON;
using FNFJSON.Extensions;
using System.Collections.Generic;
```
The extensions library is very small at the moment but adds a few extra functions that make no sense to be in base.

To import a Song:
```cs
Song.LoadFromFile("C:\\song.json");
```

To export/save a Song(automatically overwrites):
```cs
Song.SaveToFile("C:\\song.json",SongClassGoesHere);
```

FNFJSON has "high-level" classes so you don't have to deal with the raw data. For instance, instead of having to deal with editing the raw note data stored in the game, you can call
```cs
Section.ConvertSectionToNotes();
```
to convert the raw note data stored in a section object to an array of Note objects.
And you can use:
```cs
Section.SaveNotes(List<Note> data);
```
To save the list of notes back into raw data.

If you are using the FNFJSON extensions, you can even convert a note back into raw data.
```cs
Note.ToRawData(bool musthitsection);
```

[nt]: <https://www.newtonsoft.com/json>
