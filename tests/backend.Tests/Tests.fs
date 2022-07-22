module Tests

open System
open System.IO
open System.Net
open System.Net.Http
open Xunit
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.TestHost
open Microsoft.Extensions.DependencyInjection
open Microsoft.AspNetCore.Mvc.Testing
open Microsoft.VisualStudio.TestPlatform.TestHost
open Newtonsoft.Json
open backend.Barbecue.Service
open backend.Barbecue.Validator

// ---------------------------------
// Helper functions (extend as you need)
// ---------------------------------
let runTestApi () =
    (new WebApplicationFactory<Program>()).Server

let createHost () =
    WebHostBuilder()
        .UseContentRoot(Directory.GetCurrentDirectory())
        .Configure(Action<IApplicationBuilder>(backend.Main.configureApp))
        .ConfigureServices(Action<IServiceCollection> backend.Main.configureServices)

let runTask task =
    task |> Async.AwaitTask |> Async.RunSynchronously

let httpGet (path: string) (client: HttpClient) = path |> client.GetAsync |> runTask

let httpDelete (path: string) (client: HttpClient) = path |> client.DeleteAsync |> runTask

let httpPost (path: string) (payload: obj) (client: HttpClient) =
    let json = JsonConvert.SerializeObject payload

    use content = new StringContent(json, Text.Encoding.UTF8, "application/json")

    client.PostAsync(path, content) |> runTask

let httpPut (path: string) (payload: obj) (client: HttpClient) =
    let json = JsonConvert.SerializeObject payload

    use content = new StringContent(json, Text.Encoding.UTF8, "application/json")

    client.PutAsync(path, content) |> runTask

let isStatus (code: HttpStatusCode) (response: HttpResponseMessage) =
    Assert.Equal(code, response.StatusCode)
    response

let ensureSuccess (response: HttpResponseMessage) =
    if not response.IsSuccessStatusCode then
        response.Content.ReadAsStringAsync()
        |> runTask
        |> failwithf "%A"
    else
        response

let readText (response: HttpResponseMessage) =
    response.Content.ReadAsStringAsync() |> runTask

let shouldEqual expected actual = Assert.Equal(expected, actual)

let shouldContain (expected: string) (actual: string) = Assert.True(actual.Contains expected)

let barbecuePayload =
    { name = "Churrasco"
      description = "Churrasco"
      date = DateTime.Now
      participants =
        [ { name = "Victor"
            contribution = 20 } ] }

let participantPayload id =
    { name = "Victor teste"
      contribution = 10
      barbecue_id = id }


let createBarbecueFactory payload = createBarbecue payload

let addParticipantFactory payload = addParticipantToBarbecue payload
// ---------------------------------
// Tests
// ---------------------------------

[<Fact>]
let ``POST /api/barbecue should create a new barbecue`` () =
    use server = new TestServer(createHost ())
    use client = server.CreateClient()

    let payload =
        {| name = "Churrasco"
           description = "Churrasco"
           date = DateTime.Now
           participants = [ {| name = "Victor"; contribution = 50 |} ] |}

    client
    |> httpPost "/api/barbecue" payload
    |> ensureSuccess
    |> readText
    |> shouldContain "BARBECUE_CREATED_SUCCESSFULLY"

[<Fact>]
let ``PUT /api/barbecue/participant should add a participant to a existing barbecue`` () =
    use server = new TestServer(createHost ())
    use client = server.CreateClient()

    let barbecue = createBarbecueFactory barbecuePayload |> runTask

    match barbecue with
    | Ok result ->
        let payload = participantPayload result.id

        client
        |> httpPut "/api/barbecue/participant" payload
        |> ensureSuccess
        |> readText
        |> shouldContain "PARTICIPANT_ADDED_SUCCESSFULLY"

    | Error error ->
        Console.WriteLine error
        failwith "error"


[<Fact>]
let ``DELETE /api/barbecue/participant/$UUID should delete a barbecue`` () =
    use server = new TestServer(createHost ())
    use client = server.CreateClient()

    let barbecue = createBarbecueFactory barbecuePayload |> runTask

    match barbecue with
    | Ok result ->
        let payload = participantPayload result.id
        let participant = addParticipantFactory payload |> runTask

        match participant with
        | Ok participantResult ->
            client
            |> httpDelete $"/api/barbecue/participant/{participantResult.id}"
            |> isStatus HttpStatusCode.NoContent
        | Error err ->
            Console.WriteLine err
            failwith "err"

    | Error err ->
        Console.WriteLine err
        failwith "err"


[<Fact>]
let ``GET /api/barbecue/$UUID should get details from a barbecue`` () =
    use server = new TestServer(createHost ())
    use client = server.CreateClient()

    let barbecue = createBarbecueFactory barbecuePayload |> runTask

    match barbecue with
    | Ok result ->
        let response =
            client
            |> httpGet $"/api/barbecue/{result.id}"
            |> isStatus HttpStatusCode.OK
            |> readText

        response |> shouldContain (result.id.ToString())

        response
        |> shouldContain (barbecuePayload.ToString())
    | Error err ->
        Console.WriteLine err
        failwith "err"

// [<Fact>]
// let ``GET /api/barbecue should list all barbecues`` () =
//     use server = new TestServer(createHost ())
//     use client = server.CreateClient()

//     client
//     |> httpGet "/route/which/does/not/exist"
//     |> isStatus HttpStatusCode.NotFound
//     |> readText
//     |> shouldEqual "Not Found"

[<Fact>]
let ``Route which doesn't exist returns 404 Page not found`` () =
    use server = new TestServer(createHost ())
    use client = server.CreateClient()

    client
    |> httpGet "/route/which/does/not/exist"
    |> isStatus HttpStatusCode.NotFound
    |> readText
    |> shouldEqual "Not Found"
