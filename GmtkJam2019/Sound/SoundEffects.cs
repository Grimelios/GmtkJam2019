using Engine.Sound;

namespace GmtkJam2019.Sound
{
	// I generally don't like using static god objects, but this is a game jam, so I'm cool with it.
	public static class SoundEffects
	{
		public static readonly CachedSound Speech = new CachedSound("Speech.wav");
	}
}
