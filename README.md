# arden-audio-system
Sound manager with pooling, volumes, playlists, sound materials, and more for Unity

## Features

### Pooling
Pool your audio sources - allows attaching to a transform, a point, or neither. This feature can be used without any of the other features, specifically the ArdenAudioClip.

### MixerGroups
Make AudioMixerGroups a bit more friendly by using a enum. 

### Custom Audio Asset
The built in audio clip is useful, but is made even more useful by having each clip also create an ArdenAudioClip asset. This asset has some nifty values, such as volume (now you can tweak the volume of individual clips!), pitch (random pitch as well), and a string for subtitles, if you want to hook that into your subtitle engine.

###  Sound Materials
Now you can attach components to objects that will interface with other materials, making sounds! I personally use this for footsteps! Also supports terrain, where individual sound materials are used per terrain texture.

### Mixers, Playlists, Etc
Also includes some helpful audio tools, such as mixers (play different sound effect layers with different settings such as loop, random play, and so on - especially useful for ambient tracks), and playlists (play a list of sounds, with shuffle and smart shuffle included - perfect for music)

### Music Player
A nice little tool that will play a list of songs, much like a playlist - it however allows for dynamic addition / removal of songs, and will automatically play a new song if need be. Will also allow silence to play, depending on your settings. (Never underestimate the power of silence!)

### Filters
Partial implementation of making filters more usable (specifically fixing the issue of not allowing multiple filter components at once). Currently doesn't work as intended.

### TODO
* Automated ArdenAudioClip asset creation 

* Fade In / Out for playlists / mixers / music player

* Make Filters actually useful

* Possibly use DSPGraph whenever that comes out
