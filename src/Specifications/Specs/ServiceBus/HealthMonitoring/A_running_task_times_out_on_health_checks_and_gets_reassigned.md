# A running task times out on health checks and gets reassigned

-> id = 79ae3e18-38f3-4a8e-9242-eb914dc03fe2
-> lifecycle = Acceptance
-> max-retries = 0
-> last-updated = 2017-02-23T15:45:01.1787952-06:00
-> tags = 

[Monitoring]
|> Context
    [MonitoringSetup]
    |> TheNodesAre
        [table]
        |id   |incoming                       |
        |Node1|lq.tcp://localhost:1000/control|
        |Node2|lq.tcp://localhost:2000/control|
        |Node3|lq.tcp://localhost:3000/control|
        |Node4|lq.tcp://localhost:4000/control|

    |> HealthMonitoringIsDisabled
    |> TheTasksAre
        [table]
        |task   |node |nodes                     |
        |foo://1|Node4|Node1, Node2, Node3, Node4|
        |foo://2|Node4|Node2, Node1, Node3, Node4|
        |foo://3|NONE |Node3, Node1, Node2, Node4|
        |foo://4|Node4|Node4, Node1, Node2, Node3|



The initial state should look like this

|> TheTaskAssignmentsShouldBe
    [rows]
    |Task   |Node |
    |foo://1|Node4|
    |foo://2|Node4|
    |foo://4|Node4|


Now, these two jobs will become unresponsive on Node4

|> TaskStateIs
    [table]
    |Task   |Node |State                               |
    |foo://1|Node4|Times out on startup or health check|
    |foo://2|Node4|Times out on startup or health check|

|> AfterTheHealthChecksRunOnNode node=Node1
|> ThePersistedAssignmentsShouldBe
    [rows]
    |Task   |Node |
    |foo://1|Node1|
    |foo://2|Node2|
    |foo://3|Node3|
    |foo://4|Node4|

|> TheTaskAssignmentsShouldBe
    [rows]
    |Task   |Node |
    |foo://1|Node1|
    |foo://2|Node2|
    |foo://3|Node3|
    |foo://4|Node4|

~~~
