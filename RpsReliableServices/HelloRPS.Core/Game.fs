﻿namespace HelloRPS.Core

module Interop =
  open HelloRPS.Core.Models
  open FSharp.Reflection
  open System.Runtime.InteropServices
  let TryParseStateFromString s ([<Out>] state : Move byref) =
    match FSharpType.GetUnionCases typeof<Move>
          |> Array.filter (fun x -> x.Name.Equals(s, System.StringComparison.OrdinalIgnoreCase)) with
    | [| c |] -> state <- FSharpValue.MakeUnion(c, Array.empty) :?> Move; true
    | _ -> false

open HelloRPS.Core.Models
module Game =
  let EmptyState =
      { gameId = ""
        gameState = GameState.NotStarted
        creatorName = ""
        creatorMove = Move.Paper }

  let Outcome playerOneMove playerTwoMove =
      match playerOneMove, playerTwoMove with 
      | Move.Rock, Move.Paper     -> GameResult.PlayerTwoWin
      | Move.Scissors, Move.Rock  -> GameResult.PlayerTwoWin
      | Move.Paper, Move.Scissors -> GameResult.PlayerTwoWin
      | x, y when x = y           -> GameResult.Tie
      | _                         -> GameResult.PlayerOneWin
