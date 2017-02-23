# A running task goes down and gets reassigned

-> id = a4d3e4a6-78f9-4d98-9fe8-b58f89f6fdd4
-> lifecycle = Acceptance
-> max-retries = 0
-> last-updated = 2017-02-23T15:45:01.1737952-06:00
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


Now, Node4 drops offline and its owned tasks should be reassigned

|> TaskStateIs
    [table]
    |Task   |Node |State                                      |
    |foo://1|Node4|Throws exception on startup or health check|
    |foo://2|Node4|Throws exception on startup or health check|

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
