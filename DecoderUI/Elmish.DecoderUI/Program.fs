module Main

[<EntryPoint>]
let main _ = 

    let initSroMatrix = [| 0.0m; 0.0m; 0.0m |]

    (fun model _ -> 
        printf "%A\n" model)
    |> Elmish.Program.mkProgram Update.init Update.update
    |> Elmish.Program.runWith initSroMatrix

    0