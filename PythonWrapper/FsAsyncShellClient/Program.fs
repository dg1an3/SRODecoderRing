open System.Threading.Tasks
open System.IO
open System.Diagnostics

let invokeExternalAsync<'TRequest, 'TResponse> 
        exePath argFormat requestSerialize responseDeserialize (request:'TRequest) =

    let fileIn, fileOut = Path.GetTempFileName(), Path.GetTempFileName()
    (fileIn, requestSerialize request) |> File.WriteAllText

    let proc = 
        (exePath, sprintf argFormat fileIn fileOut)
        |> ProcessStartInfo
        |> Process.Start

    let tcs = TaskCompletionSource<'TResponse>()
    let fileOutToResult _ = 
        fileOut
        |> File.ReadAllText
        |> responseDeserialize
        |> tcs.SetResult
    proc.EnableRaisingEvents <- true
    proc.Exited.Add fileOutToResult
    tcs.Task

open FSharp.Control.Tasks.V2

[<EntryPoint>]
let main argv = 

    let inputLines = 
        ("\n", ["Mary"; "had"; "a"; "little"; "lamb"])
        |> System.String.Join

    let solutionPath = "..\\..\\..\\"

    let reverserTask = task { 
        let consoleReverseExeRelative = solutionPath + "CommandLineReverse\\bin\\Debug\\CommandLineReverse.exe"
        let invokelineReverser = invokeExternalAsync consoleReverseExeRelative "%s %s" id id 
        return! invokelineReverser inputLines }
    printfn "Reversed string by ConsoleReverse = %s" reverserTask.Result

    let invokePythonReverser = task {
        let invokePythonReverser = invokeExternalAsync "python" "..\\..\\..\\PythonReverse\\do_reverse.py %s %s" id id
        return! invokePythonReverser inputLines }
    printfn "Reversed string by PythonReverse = %s" invokePythonReverser.Result

    0 // return an integer exit code
