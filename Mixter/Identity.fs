﻿module Mixter.Domain.Identity

open System

type UserId = UserId of string

type SessionId = SessionId of string
    with static member generate = SessionId (Guid.NewGuid().ToString())
        
type Event = 
    | UserRegistered of UserRegisteredEvent
    | UserConnected of UserConnectedEvent
    | UserDisconnected of UserDisconnectedEvent
and UserRegisteredEvent = { UserId: UserId }
and UserConnectedEvent = { SessionId: SessionId; UserId: UserId; ConnectedAt: DateTime}
and UserDisconnectedEvent = { SessionId: SessionId; UserId: UserId }

type DecisionProjection = {
    UserId: UserId;
    SessionId: SessionId option
}
    with static member initial = { UserId = UserId ""; SessionId = None }

let register userId =
    [ UserRegistered { UserId = userId } ]

let logIn sessionId getCurrentTime decisionProjection =
    [ UserConnected { SessionId = sessionId; UserId = decisionProjection.UserId; ConnectedAt = getCurrentTime () } ]

let logOut decisionProjection =
    match decisionProjection.SessionId with
        | Some sessionId -> [ UserDisconnected { SessionId = sessionId; UserId = decisionProjection.UserId } ]
        | None -> []

let applyOne decisionProjection event =
    match event with
    | UserRegistered e -> { decisionProjection with UserId = e.UserId }
    | UserConnected e -> { decisionProjection with SessionId = Some e.SessionId }
    | UserDisconnected _ -> { decisionProjection with SessionId = None }

let apply decisionProjection =
    Seq.fold applyOne decisionProjection

module Read =
    type RepositoryChange<'a> = 
        | None
        | Add of 'a
        | Remove of 'a

    type Session = { SessionId: SessionId; UserId: UserId }
        with static member empty = { SessionId = SessionId ""; UserId = UserId "" }

    type getSessionById = SessionId -> Session option

    let project (getSessionById:getSessionById) event = 
        match event with
        | UserConnected e -> Add { SessionId = e.SessionId; UserId = e.UserId }
        | UserDisconnected e -> 
            let session = getSessionById e.SessionId
            match session with
            | Some session -> Remove session
            | option.None -> None