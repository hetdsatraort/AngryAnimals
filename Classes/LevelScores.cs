using Godot;
using System;

public partial class LevelScores : Resource
{
    private const int DefaultScore = 999;

    [Export] private Godot.Collections.Dictionary<int, int> _levelScores = new();
    
    public int GetBestScore(int level)
    {
        GD.Print($"The level has a previous best score? {_levelScores.ContainsKey(level)}");
        var bestScore = _levelScores.ContainsKey(level) ? _levelScores[level] : DefaultScore;
        return bestScore;
    }

    public void SetBestScore(int level, int score)
    {
        if (!_levelScores.ContainsKey(level) || score < _levelScores[level])
        {
            _levelScores[level] = score;
        }
    }
}
