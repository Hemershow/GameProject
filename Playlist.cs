using System;
using System.Collections.Generic;
using System.Media;

public class Playlist
{
    public DateTime latestChange { get; set; }
    public int currentMusic { get; set; }
    public bool playing { get; set; } = false;
    public List<Music> sounds { get; set; } = new List<Music>() {
        new Music("./sounds/tututu.wav", 138000),
        new Music("./sounds/dububububu.wav", 159000),
        new Music("./sounds/dudum.wav", 200000),
    };
    public void Update()
    {
        if (!playing)
        {
            var i = Game.Current.rnd.Next(0, sounds.Count);
            sounds[i].Start();
            latestChange = DateTime.Now;
            playing = true;
            currentMusic = i;
        }
        else 
        {
            if ((DateTime.Now - latestChange).TotalMilliseconds >= sounds[currentMusic].lenght)
            {
                var i = Game.Current.rnd.Next(0, sounds.Count);
                while (i == currentMusic)
                {
                    i = Game.Current.rnd.Next(0, sounds.Count);
                }
                
                sounds[i].Start();
                latestChange = DateTime.Now;
                currentMusic = i;
            }
        }
    }
}

public class Music 
{
    public string path { get; set; }
    public int lenght { get; set; }
    public SoundPlayer mixer { get; set; }
    public Music(string path, int lenght)
    {
        this.lenght = lenght;
        this.path = path;
        this.mixer = new SoundPlayer(path);
    } 
    public void Start() => mixer.Play();
    public void Stop() => mixer.Stop();
}