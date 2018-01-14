using MastermindInterfaces;
using Nancy;
using Nancy.ModelBinding;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MastermindServer
{
    public class GameModule : NancyModule
    {
        public GameModule(GameService gameService)
        {
            Get["/"] = _ =>
            {
                return "Mastermind Game Server v1.0";
            };

            Get["/games/{id}"] = parameters =>
            {
                Game game = gameService.GetGameById(parameters.id);
                if (game == null)
                {
                    return HttpStatusCode.NotFound;
                }

                return Response.AsJson(game);
            };

            Post["/games"] = _ =>
            {
                Game game = gameService.CreateNewGame();

                Console.WriteLine($"Created game {game.Id}: {string.Join(", ", game.ColorSequence)}");
                return Response.AsRedirect($"/games/{game.Id}");
            };

            Post["/games/{id}"] = parameters =>
            {
                Guess guess = this.Bind<Guess>();
                if (guess == null)
                {
                    return HttpStatusCode.NotFound;
                }

                try
                {
                    Score score = gameService.ProcessGuess(parameters.id, guess);

                    Console.WriteLine($"New guess for game {parameters.id}: {string.Join(", ", guess.ColorSequence)}");
                    Console.WriteLine($"    color: {score.CorrectColorCount} placement: {score.CorrectPlacementCount}");

                    return Response.AsJson(score);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return HttpStatusCode.NotAcceptable;
                }
            };
        }
    }
}
