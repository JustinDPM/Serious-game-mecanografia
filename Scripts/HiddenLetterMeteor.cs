using Godot;
using System;

public partial class HiddenLetterMeteor : Meteor
{
	[Export] public float HideChance = 0.35f;

	private string hiddenWord = "";

	public override void _Ready()
	{
		GenerateHiddenWord();

		DisplayWord = hiddenWord;

		base._Ready();
	}

	private void GenerateHiddenWord()
	{
		hiddenWord = "";

		RandomNumberGenerator rng = new RandomNumberGenerator();
		rng.Randomize();

		for (int i = 0; i < Word.Length; i++)
		{
			char c = Word[i];

			if (c == ' ')
			{
				hiddenWord += " ";
				continue;
			}

			bool hide = rng.Randf() < HideChance;

			hiddenWord += hide
				? "_"
				: c.ToString();
		}

		bool allHidden = true;

		foreach (char c in hiddenWord)
		{
			if (c != '_' && c != ' ')
			{
				allHidden = false;
				break;
			}
		}

		if (allHidden && Word.Length > 0)
		{
			int randomIndex = rng.RandiRange(0, Word.Length - 1);

			hiddenWord =
				hiddenWord.Remove(randomIndex, 1)
				.Insert(randomIndex, Word[randomIndex].ToString());
		}
	}

	public override void UpdateDisplay(string input)
	{
		if (label == null)
			return;

		string result = "";

		for (int i = 0; i < Word.Length; i++)
		{
			char realChar = Word[i];
			char visibleChar = hiddenWord[i];

			if (i < input.Length && input[i] == realChar)
			{
				result +=
					"[color=green]" +
					realChar +
					"[/color]";
			}
			else if (i < input.Length)
			{
				result +=
					"[color=red]" +
					realChar +
					"[/color]";
			}
			else
			{
				result += visibleChar;
			}
		}

		label.Text = result;
	}
}
