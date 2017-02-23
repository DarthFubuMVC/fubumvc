# Mixed assigned and unassigned tasks

-> id = 664bd284-97ac-4702-a6ed-d691bfe9e923
-> lifecycle = Regression
-> max-retries = 0
-> last-updated = 2016-06-21T00:00:00.0000000
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
        |foo://4|NONE |Node4, Node1, Node2, Node3|


|> AfterTheHealthChecksRunOnNode node=Node1
|> ThePersistedAssignmentsShouldBe
    [rows]
    |Task   |Node |
    |foo://1|Node4|
    |foo://2|Node4|
    |foo://3|Node3|
    |foo://4|Node4|

|> TheTaskAssignmentsShouldBe
    [rows]
    |Task   |Node |
    |foo://1|Node4|
    |foo://2|Node4|
    |foo://3|Node3|
    |foo://4|Node4|

~~~
