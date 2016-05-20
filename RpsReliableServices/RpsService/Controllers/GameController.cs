using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using HelloRPS.Core;
using HelloRPS.Core.Models;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;

namespace RpsService.Controllers
{
    [RoutePrefix("api/game")]
    public class GameController : ApiController
    {
        private readonly IReliableStateManager _stateManager;

        public GameController(IReliableStateManager stateManager)
        {
            _stateManager = stateManager;
        }

        [HttpPost]
        [Route("create")]
        public async Task<IHttpActionResult> Create()
        {
            var games = await GetGames();
            var gameId = Guid.NewGuid().ToString();

            using (var tx = _stateManager.CreateTransaction())
            {
                var result = await games.TryAddAsync(tx, gameId, Game.EmptyState);
                if (!result)
                {
                    return InternalServerError();
                }
                await tx.CommitAsync();
            }

            return Created(Url.Link("Move", new { gameId }), string.Empty);
        }

        [HttpPut]
        [Route("{gameId}", Name = "Move")]
        public async Task<IHttpActionResult> Move(
            string gameId,
            [FromBody] Models.MoveRequest move)
        {
            var games = await GetGames();
            using (var tx = _stateManager.CreateTransaction())
            {
                var gameVal = await games.TryGetValueAsync(tx, gameId);
                if (!gameVal.HasValue)
                {
                    return NotFound();
                }

                Move m = null;
                if (!Interop.TryParseStateFromString(move.Move, out m))
                {
                    return BadRequest("Invalid move");
                }

                var game = gameVal.Value;
                if (game == Game.EmptyState)
                {
                    var newState = new State(gameId, GameState.Started, move.PlayerName, m);
                    await games.TryUpdateAsync(tx, gameId, newState, Game.EmptyState);
                    var response = new HttpResponseMessage(HttpStatusCode.Accepted);
                    response.Headers.Location = new Uri(Url.Link("Status", new { gameId }));

                    await tx.CommitAsync();
                    return ResponseMessage(response);
                }

                if (game.creatorName?.Equals(move.PlayerName, StringComparison.Ordinal) ?? false) return BadRequest("Player has already moved");

                game = new State(gameId, GameState.Ended, game.creatorName, game.creatorMove);

                var outcomeMessage = Outcome(game.creatorMove, game.creatorName, m, move.PlayerName);
                var outcomes = await GetOutcomes();
                await outcomes.AddAsync(tx, gameId, outcomeMessage);
                await tx.CommitAsync();
                return Ok(outcomeMessage);
            }
        }

        private static string Outcome(Move p1Move, string p1Name, Move p2Move, string p2Name)
        {
            var outcome = Game.Outcome(p1Move, p2Move);
            if (outcome.IsPlayerOneWin) return p1Name + " won.";
            else if (outcome.IsPlayerTwoWin) return p2Name + " won.";
            else return "The game ended with a tie.";
        }

        [HttpGet]
        [Route("{gameId}", Name = "Status")]
        public async Task<IHttpActionResult> Status(string gameId)
        {
            var games = await GetGames();
            var outcomes = await GetOutcomes();
            using (var tx = _stateManager.CreateTransaction())
            {
                var gameVal = await games.TryGetValueAsync(tx, gameId);
                if (!gameVal.HasValue)
                {
                    return NotFound();
                }
                var game = gameVal.Value;
                var outcomeVal = await outcomes.TryGetValueAsync(tx, gameId);

                if (!(game.gameState == GameState.Ended || outcomeVal.HasValue))
                {
                    return Ok("Awaiting other player's move.");
                }

                return Ok(outcomeVal.Value);
            }
        }

        private Task<IReliableDictionary<string, State>> GetGames()
        {
            return _stateManager.GetOrAddAsync<IReliableDictionary<string, State>>("Games");
        }

        private Task<IReliableDictionary<string, string>> GetOutcomes()
        {
            return _stateManager.GetOrAddAsync<IReliableDictionary<string, string>>("Outcomes");
        }
    }
}
