module Model

open System
open Microsoft.FSharp.Quotations

type IecCouchShiftModel = 
    { Tx:decimal; Ty:decimal; Tz:decimal; 
        Rx:decimal; Ry:decimal; Rz:decimal; }

type IecCouchShiftLockExpression =
    IecCouchShiftModel -> Expr<decimal> list

let noLocks : IecCouchShiftLockExpression = 
    function
    | _ -> []

let lockAllShifts : IecCouchShiftLockExpression = 
    function
    | couchShift ->
        [ <@couchShift.Tx@>; 
            <@couchShift.Ty@>; 
            <@couchShift.Tz@>;
            <@couchShift.Rx@>; 
            <@couchShift.Ry@>; 
            <@couchShift.Rz@> ]

let initCouchShift = 
    { Tx=0.0m;
        Ty = 0.0m;
        Tz = 0.0m;
        Rx = 0.0m;
        Ry = 0.0m;
        Rz = 0.0m; }

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

type GeometryEstimates =
    PatientPosition * TransformationOrder -> decimal

let uniformGeometryEstimates : GeometryEstimates =
    let numberOfPositions = 
        Enum.GetValues(typeof<PatientPosition>).Length
    let numberOfOrders = 
        Enum.GetValues(typeof<TransformationOrder>).Length
    function
    | (_:PatientPosition, _:TransformationOrder) ->
        1.0m / decimal (numberOfPositions * numberOfOrders)

type GeometryEstimateMaximizationParameters =
    { SelectPatientPosition : PatientPosition option;
        SelectTransformationOrder : TransformationOrder option; }

let noMaximization = 
    { SelectPatientPosition = None;
        SelectTransformationOrder = None }

let maximizePosition position = 
    { SelectPatientPosition = Some position;
        SelectTransformationOrder = None }

type Model = 
    { SroMatrix : decimal[];
        IecCouchShift : IecCouchShiftModel;
        LockExpression : IecCouchShiftLockExpression;
        GeometryEstimates : GeometryEstimates;
        MaximizeForGeometry : GeometryEstimateMaximizationParameters; }

