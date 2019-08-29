module VisualStudioSolution =

  open System
  open System.IO

  type t
    = Root of folder
  and folder
    = Folder of 
        path:  string * 
        uid:   Guid   *
        items: folder list
    | File   of 
        path:string

  let (++) x y = x + @"\" + y

  let cd    = __SOURCE_DIRECTORY__
  let mkdir = Guid.Parse("2150E333-8FDC-42A3-9474-1A3956D46DE8")

  let foldername : string -> string =
    fun path ->
      path.[path.LastIndexOf(@"\") + 1 ..]

  let relativePath : string -> string -> string =
    fun prefix path ->
      path.[path.IndexOf(prefix) ..]

  let rec folders : string -> folder = 
    fun path ->
      Folder (
        path,
        Guid.NewGuid(),
        Directory.GetDirectories(path, "*", SearchOption.TopDirectoryOnly)
        |> Array.map folders
        |> Array.append (files path)
        |> Array.toList
      )

  and files : string -> folder array = 
    fun path ->
      Directory.GetFiles(path, "*", SearchOption.TopDirectoryOnly)
      |> Array.map File

  let getFolderStructure : string -> t = 
    fun path ->
      folders path |> Root

  let rec createProjects : t -> (folder * string * Guid) seq =
    fun (Root folder) ->
      match folder with
        | File   _ -> Seq.empty
        | Folder _ ->
          seq {
            yield! createProjectsAux folder
          }
  and createProjectsAux : folder -> (folder * string * Guid) seq = function
      | File    ______________            -> Seq.empty
      | Folder (path, guid, fs) as folder ->
        seq {
          yield (folder, path, guid)
          yield! 
            fs
            |> Seq.map createProjectsAux
            |> Seq.concat
        }
  let rec createSolutionItems : folder -> string seq = function
    | File _ -> Seq.empty
    | Folder (items = fs) ->
      fs
      |> Seq.map (function | File path -> Some path | Folder _ -> None)
      |> Seq.choose id

  let rec nestedProjects : t -> (Guid * Guid) seq =
    fun (Root folder) ->
      match folder with
        | File    ______________________  -> Seq.empty
        | Folder (uid = guid; items = fs) -> 
          seq {
            yield!
              fs
              |> Seq.map (nestedProjectsAux guid)
              |> Seq.concat
          }
  and nestedProjectsAux : Guid -> folder -> (Guid * Guid) seq =
    fun parentid -> function
      | File    ______________________  -> Seq.empty
      | Folder (uid = guid; items = fs) ->
        seq {
          yield parentid, guid
          yield! 
            fs
            |> Seq.map (nestedProjectsAux guid)
            |> Seq.concat
        }

open VisualStudioSolution

fsi.CommandLineArgs
|> Array.skip 1 // Skip the name of the F# script file
|> Array.iter (
  fun folder -> 
    let prefix        = folder
    let root          = cd ++ folder
    let structure     = getFolderStructure root
    let solutionItems folder = 
      let xs = createSolutionItems folder
      if xs <> Seq.empty then
        "\r\n\tProjectSection(SolutionItems) = preProject\r\n" +
        (
          createSolutionItems folder
          |> Seq.map(
            fun path ->
              let rpath = relativePath prefix path
              sprintf "\t\t%s = %s\r\n"
                rpath rpath
            )
          |> Seq.fold((+)) ""
        ) +
        "\tEndProjectSection"
      else
        ""
    let createSection =
      createProjects structure
      |> Seq.map(
        fun (folder,path,guid) -> 
          sprintf 
            "Project(\"{%s}\") = \"%s\", \"%s\", \"{%s}\"%s\r\nEndProject"
              (mkdir.ToString().ToUpper())
              (foldername path)
              (foldername path)
              (guid.ToString().ToUpper())
              (solutionItems folder)
      )
    let nestedSection = 
      nestedProjects structure
      |> Seq.map(
        fun (parentguid,childguid) -> 
          sprintf "\t\t{%s} = {%s}"
            (childguid.ToString().ToUpper())
            (parentguid.ToString().ToUpper())
      )
    
    createSection
    |> Seq.iter (printfn "%s")
    nestedSection
    |> Seq.iter (printfn "%s")
)
