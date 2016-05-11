﻿module Mixter.Infrastructure.Identity.Read

open Mixter.Domain.Identity
open Mixter.Domain.Identity.Read

open System.Collections.Generic

type MemorySessionsStore() =
    let store = new Dictionary<SessionId, Session>()

    member this.GetSession sessionId = 
        if store.ContainsKey(sessionId) 
        then Some store.[sessionId]
        else option.None

    member this.ApplyChange change = 
        match change with 
        | Add session -> store.Add (session.SessionId, session)
        | Remove sessionId -> store.Remove sessionId |> ignore
        | None -> ()