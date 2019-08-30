module Main

[<EntryPoint>]
let main argv = 

    (fun model _ -> 
        printf "%A\n" model)
    |> Elmish.Program.mkProgram Model.init Update.update
    |> Elmish.Program.run

    0