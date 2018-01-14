using MastermindInterfaces;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MastermindClient
{
    public class GameClient
    {
        private static Random Random { get; } = new Random();

        private string ServerUrl { get; set; }
        private List<KeyValuePair<Guess, Score>> Guesses { get; set; } = new List<KeyValuePair<Guess, Score>>();

        public GameClient(string serverUrl)
        {
            ServerUrl = serverUrl;
        }

        public void Run()
        {
            RestClient client = new RestClient(ServerUrl);

            Game game = CreateNewGame(client);
            if (game == null)
            {
                Console.WriteLine("Unable to create game");
                return;
            }

            // Tracks which colors and positions are known
            string[] colorsInCorrectPosition = new string[4];

            List<string> colorsInSequence = new List<string>();
            List<string> colorsNotInSequence = new List<string>();

            Guess guess = new Guess { ColorSequence = game.AvailableColors.OrderBy(x => Random.Next()).Take(game.ColorSequenceCount).ToList() };

            while (true)
            {
                Score score = ProcessGuess(client, game, guess);
                if (score == null)
                {
                    Console.WriteLine("Unable to process guess");
                    return;
                }

                if (score.CorrectPlacementCount == game.ColorSequenceCount)
                {
                    Console.WriteLine($"Correct guess!  The sequence is {string.Join(", ", guess.ColorSequence)}");
                    return;
                }

                if ((score.CorrectColorCount + score.CorrectPlacementCount) == game.ColorSequenceCount)
                {
                    colorsInSequence = guess.ColorSequence;
                    colorsNotInSequence = game.AvailableColors.Except(colorsInSequence).ToList();
                }

                // Only run this block if we have a previous guess to compare with
                if (Guesses.Any())
                {
                    KeyValuePair<Guess, Score> pair = Guesses.Last();
                    Guess previousGuess = pair.Key;
                    Score previousScore = pair.Value;

                    int changedIndex = GetChangedIndex(guess.ColorSequence, previousGuess.ColorSequence);
                    if (changedIndex == -1)
                    {
                        throw new Exception("No change in guess since last iteration?");
                    }
                    
                    if (score.CorrectPlacementCount > previousScore.CorrectPlacementCount)
                    {
                        colorsInCorrectPosition[changedIndex] = guess.ColorSequence[changedIndex];

                        // If the change resulted in a new correct placement, but didn't change the correct colors
                        // then we know the previous color is not in the sequence
                        if (score.CorrectColorCount == previousScore.CorrectColorCount)
                        {
                            AddIfNotPresent(colorsNotInSequence, previousGuess.ColorSequence[changedIndex]);
                        }
                    }
                    else if (score.CorrectPlacementCount < previousScore.CorrectPlacementCount)
                    {
                        colorsInCorrectPosition[changedIndex] = previousGuess.ColorSequence[changedIndex];

                        // If the change resulted in a lost correct placement, but didn't change the correct colors
                        // then we know the new color is not in the sequence
                        if (score.CorrectColorCount == previousScore.CorrectColorCount)
                        {
                            AddIfNotPresent(colorsNotInSequence, guess.ColorSequence[changedIndex]);
                        }
                    }

                    if (score.CorrectColorCount > previousScore.CorrectColorCount)
                    {
                        AddIfNotPresent(colorsInSequence, guess.ColorSequence[changedIndex]);

                        // If the change resulted in a new correct color, but didn't change the correct placements
                        // then we know the previous color is not in the sequence
                        if (score.CorrectPlacementCount == previousScore.CorrectPlacementCount)
                        {
                            AddIfNotPresent(colorsNotInSequence, previousGuess.ColorSequence[changedIndex]);
                        }
                    }
                    else if (score.CorrectColorCount < previousScore.CorrectColorCount)
                    {
                        AddIfNotPresent(colorsInSequence, previousGuess.ColorSequence[changedIndex]);

                        // If the change resulted in a lost correct color, but didn't change the correct placements
                        // then we know the new color is not in the sequence
                        if (score.CorrectPlacementCount == previousScore.CorrectPlacementCount)
                        {
                            AddIfNotPresent(colorsNotInSequence, guess.ColorSequence[changedIndex]);
                        }
                    }
                }

                Guesses.Add(new KeyValuePair<Guess, Score>(guess, score));
                
                // Create new guess, starting with the current guess
                Guess newGuess = new Guess { ColorSequence = new List<string>(guess.ColorSequence) };

                // Ensure the known correct position colors are in place
                for (int i = 0; i< colorsInCorrectPosition.Length; i++)
                {
                    if (colorsInCorrectPosition[i] != null)
                    {
                        newGuess.ColorSequence[i] = colorsInCorrectPosition[i];
                    }
                }
            }
        }

        private void AddIfNotPresent(List<string> list, string value)
        {
            if (!list.Contains(value))
            {
                list.Add(value);
            }
        }

        private Game CreateNewGame(RestClient client)
        {
            RestRequest request = new RestRequest("games", Method.POST);
            IRestResponse<Game> response = client.Execute<Game>(request);
            return response.Data;
        }

        private Score ProcessGuess(RestClient client, Game game, Guess guess)
        {
            RestRequest request = new RestRequest($"games/{game.Id}", Method.POST);
            request.AddJsonBody(guess);

            IRestResponse<Score> response = client.Execute<Score>(request);
            return response.Data;
        }

        private int GetChangedIndex(List<string> currentColorSequence, List<string> previousColorSequence)
        {
            for (int i = 0; i < currentColorSequence.Count; i++)
            {
                if (currentColorSequence[i] != previousColorSequence[i])
                {
                    return i;
                }
            }

            return -1;
        }
    }
}
