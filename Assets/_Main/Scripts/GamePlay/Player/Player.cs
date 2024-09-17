using Fiber.Managers;
using Fiber.Utilities;

namespace GamePlay.Player
{
	/// <summary>
	/// /// Handle Player Specific Information Assigning,  ...
	/// </summary>
	public class Player : Singleton<Player>
	{
		public bool CanInput { get; set; } = false;

		private void OnEnable()
		{
			LevelManager.OnLevelLoad += OnLevelLoaded;
			LevelManager.OnLevelStart += OnStart;
			LevelManager.OnLevelWin += OnWin;
			LevelManager.OnLevelWinWithMoveCount += OnLevelWinWithMoveCount;
			LevelManager.OnLevelLose += OnLose;
		}

		private void OnDisable()
		{
			LevelManager.OnLevelLoad -= OnLevelLoaded;
			LevelManager.OnLevelStart -= OnStart;
			LevelManager.OnLevelWin -= OnWin;
			LevelManager.OnLevelWinWithMoveCount -= OnLevelWinWithMoveCount;
			LevelManager.OnLevelLose -= OnLose;
		}

		private void OnLevelLoaded()
		{
		}

		// OnStart is called when click "tap to play button"
		private void OnStart()
		{
			CanInput = true;
		}

		// OnWin is called when game is completed as succeed
		private void OnWin()
		{
			CanInput = false;
		}

		private void OnLevelWinWithMoveCount(int moveCount)
		{
			CanInput = false;
			OnWin();
		}

		// OnLose is called when game is completed as failed
		private void OnLose()
		{
			CanInput = false;
		}
	}
}