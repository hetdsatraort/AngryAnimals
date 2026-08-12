using Godot;
using System;

public partial class ScoreManager : Node
{
	// Called when the node enters the scene tree for the first time.
	private const string SCORES_PATH = "user://angryanimals.res";

	public static ScoreManager Instance { get; private set; }

	public LevelScores LevelScores { get; private set; } = new();

	public static int LevelSelected { get; set; } = 1;
	public override void _Ready()
	{
		Instance = this;
		LoadScores();
	}

	public static void SetScoreForCurrentLevel(int score)
	{
		Instance.LevelScores.SetBestScore(LevelSelected, score);
		Instance.SaveScores();
	}

	public static int GetScoreForCurrentLevel()
	{
		return Instance.LevelScores.GetBestScore(LevelSelected);
	}

	private void LoadScores()
	{
		LevelScores = new LevelScores();
		if(ResourceLoader.Exists(SCORES_PATH))
		{
			var data = ResourceLoader.Load<LevelScores>(SCORES_PATH);
			if(data != null)
			{
				LevelScores = data;
			}
		}
	}

	private void SaveScores()
	{
		Error err = ResourceSaver.Save(LevelScores, SCORES_PATH);
		if(err != Error.Ok)
		{
			GD.PrintErr($"Failed to save scores: {err}");
		}
	}
}
