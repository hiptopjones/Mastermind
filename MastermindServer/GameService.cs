using MastermindInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MastermindServer
{
    public class GameService
    {
        private const int MaxGuessCount = 12;
        private const int DefaultColorSequenceCount = 4;

        private static List<string> Colors { get; } = new List<string> { "red", "orange", "yellow", "green", "blue", "indigo", "violet", "pink" };
        private static Random Random { get; } = new Random();

        private int NextGameId { get; set; }
        private Dictionary<int, Game> Games { get; set; } = new Dictionary<int, Game>();

        public Game CreateNewGame()
        {
            Game game = new Game()
            {
                Id = NextGameId++,
                ColorSequenceCount = DefaultColorSequenceCount,
                RemainingGuesses = MaxGuessCount,
                AvailableColors = Colors,
                AllowDuplicates = false,
            };

            game.ColorSequence = Colors.OrderBy(x => Random.Next()).Take(game.ColorSequenceCount).ToList();

            Games[game.Id] = game;
            return game;
        }

        public Game GetGameById(int gameId)
        {
            Game game;
            Games.TryGetValue(gameId, out game);

            return game;
        }

        public Score ProcessGuess(int gameId, Guess guess)
        {
            Score score = null;

            Game game = GetGameById(gameId);
            if (game == null)
            {
                throw new Exception($"No active game found for ID: {gameId}");
            }

            if (game.IsSolved)
            {
                throw new Exception($"Color sequence was already solved");
            }

            if (guess.ColorSequence.Count != game.ColorSequence.Count)
            {
                throw new Exception($"Provided color sequence count ({guess.ColorSequence.Count}) does not match game color sequence count  ({game.ColorSequence.Count})");
            }

            if (game.RemainingGuesses == 0)
            {
                throw new Exception($"Reached the maximum number of guesses ({MaxGuessCount})");
            }

            game.RemainingGuesses--;

            score = new Score();

            for (int i = 0; i < guess.ColorSequence.Count; i++)
            {
                if (game.ColorSequence[i] == guess.ColorSequence[i])
                {
                    score.CorrectPlacementCount++;
                }
                else if (game.ColorSequence.Contains(guess.ColorSequence[i]))
                {
                    score.CorrectColorCount++;
                }
            }

            if (score.CorrectPlacementCount == game.ColorSequenceCount)
            {
                game.IsSolved = true;
            }

            return score;
        }
    }
}
