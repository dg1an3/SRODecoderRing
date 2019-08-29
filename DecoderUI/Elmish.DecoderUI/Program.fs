open Elmish
open Elmish

type Model = 
    { SroMatrix : double[];
        Offset : double[]; 
        PatientPosition : string;
        RotationOrder : string }

let init () = 
    { SroMatrix = [||];
        Offset = [||]; 
        PatientPosition = "";
        RotationOrder = ""; }, Cmd.none

type Message = 
| SetSroMatrix of double[]
| SetPatientPosition of string
| SetRotationOrder of string

let update msg model = 
    match msg with
    | SetSroMatrix matrix -> 
        { model with SroMatrix = matrix }, 
            Cmd.none
    | SetRotationOrder order -> 
        // update offset
        let offset = model.Offset
        { model with RotationOrder = order; Offset = offset }, 
            Cmd.none
    | SetPatientPosition position -> 
        // update offset if matrix is set
        let offset = model.Offset
        { model with PatientPosition = position; Offset = offset },
            Cmd.none

[<EntryPoint>]
let main argv = 
    (fun model _ -> 
        printf "%A\n" model)
    |> Elmish.Program.mkProgram init update
    |> Elmish.Program.run

    0