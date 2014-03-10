using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;

namespace BraveChess.Engines
{
    public class AudioEngine : GameComponent
    {
        private Dictionary<string, Song> _songs = new Dictionary<string, Song>();
        private Dictionary<string, SoundEffect> _effects = new Dictionary<string, SoundEffect>();

        public Dictionary<string, Song> LoadedSongs { get { return _songs; } }
        public Dictionary<string, SoundEffect> LoadedEffects { get { return _effects; } }

        public AudioEngine(Game game)
            : base(game)
        {
            game.Components.Add(this);
        }

        public override void Initialize()
        {
            base.Initialize();
        }//End of Override

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }//End of Override

        public void LoadSong(string name)
        {
            if (Game.Content != null && !_songs.ContainsKey(name))
            {
                var song = Game.Content.Load<Song>("Audio\\Songs\\" + name);
                _songs.Add(name, song);
            }
        }//End of Method

        public void RemoveSongs(string name)
        {
            if (_songs.ContainsKey(name))
                _songs.Remove(name);
        }//End of Method

        public void LoadEffect(string name)
        {
            if (Game.Content != null && !_effects.ContainsKey(name))
            {
                var effect = Game.Content.Load<SoundEffect>("Audio\\Effects\\" + name);
                _effects.Add(name, effect);
            }
        }//End of Method

        public void RemoveEffect(string name)
        {
            if (_effects.ContainsKey(name))
                _effects.Remove(name);
        }//End of Method

        public void PlaySong(string name)
        {
            if (_songs.ContainsKey(name))
            {
                MediaPlayer.Stop();
                MediaPlayer.Play(_songs[name]);
            }
        }//End of method

        public void PlayEffect(string name)
        {
            if (_effects.ContainsKey(name))
            {
                _effects[name].Play();
            }
        }//End of method

        public void StopSong(string name)
        {
            MediaPlayer.Stop();
        }


    }//End of Class
}
