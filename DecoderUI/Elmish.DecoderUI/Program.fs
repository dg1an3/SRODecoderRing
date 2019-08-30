open Microsoft.FSharp.Quotations
open Elmish

type PatientPosition =
| HeadFirstSupine
| HeadFirstDecubitusLeft
| HeadFirstProne
| HeadFirstDecubitusRight
| FootFirstSupine
| FootFirstDecubitusLeft
| FootFirstProne
| FootFirstDecubitusRight

type TransformationOrder =
| TRxRyRz
| TRzRyRx
| RxRyRzT
| RzRyRxT

type IecCouchShiftModel = 
    { Tx:decimal; Ty:decimal; Tz:decimal; 
        Rx:decimal; Ry:decimal; Rz:decimal; }

let initCouchShift = 
    { Tx=0.0m; Ty = 0.0m; Tz = 0.0m;
        Rx=0.0m; Ry = 0.0m; Rz = 0.0m; }

type Model = 
    { SroMatrix : decimal[];
        IecCouchShift : IecCouchShiftModel;
        GeometryEstimate : PatientPosition * TransformationOrder -> decimal;
        LockExpression : IecCouchShiftModel -> Expr<bool>;
        MaximizeExpression : PatientPosition * TransformationOrder -> Expr<bool>; }

let init () = 
    { SroMatrix = [||];
        IecCouchShift = initCouchShift;
        LockExpression = (fun offset -> <@ offset.Tx = 0.0m && offset.Ty = 0.0m @>);
        GeometryEstimate = (fun (patpos,xord) -> 0.0m);
        MaximizeExpression = (fun (patpos,xord) -> <@patpos = HeadFirstSupine@>); }, 
        Cmd.none

type Message = 
| LoadSroMatrix of decimal[]
| UpdateOffset of (IecCouchShiftModel -> IecCouchShiftModel)
| Lock of (IecCouchShiftModel -> Expr<bool>)
| Unlock of (IecCouchShiftModel -> Expr<bool>)
| Maximize of (PatientPosition * TransformationOrder -> Expr<bool>)

let update msg model = 
    match msg with
    | LoadSroMatrix matrix -> 
        { model with SroMatrix = matrix }, 
            Cmd.none
    | UpdateOffset deltaFunc -> 
        let newShift = (deltaFunc model.IecCouchShift)
        { model with IecCouchShift = newShift },
            Cmd.none
    | Lock expr -> 
        // update offset
        let shift = model.IecCouchShift
        { model with IecCouchShift = shift; LockExpression = expr },
            Cmd.none
    | Unlock expr -> 
        // update offset if matrix is set
        let shift = model.IecCouchShift
        { model with IecCouchShift = shift; LockExpression = expr },
            Cmd.none
    | Maximize expr -> 
        { model with MaximizeExpression = expr },
            Cmd.none

[<EntryPoint>]
let main argv = 

    (fun model _ -> 
        printf "%A\n" model)
    |> Elmish.Program.mkProgram init update
    |> Elmish.Program.run

    0