# Default Conneg Rules

-> id = 003e93ae-a8dc-44c7-9a83-b9c11e1efa0a
-> lifecycle = Regression
-> max-retries = 0
-> last-updated = 2017-02-23T15:45:01.0787952-06:00
-> tags = 

[Conneg]
|> DefaultConnegRules
    [table]
    |> DefaultConnegRules-row QueryString=NULL, format=Json, Accepts=*/*, ContentType=application/json, ResponseCode=200
    |> DefaultConnegRules-row QueryString=NULL, format=Json, Accepts=text/json, ContentType=text/json, ResponseCode=200
    |> DefaultConnegRules-row QueryString=NULL, format=Xml, Accepts=*/*, ContentType=application/json, ResponseCode=200
    |> DefaultConnegRules-row QueryString=NULL, format=Xml, Accepts=application/xml, ContentType=application/xml, ResponseCode=200
    |> DefaultConnegRules-row QueryString=NULL, format=Xml, Accepts=text/xml, ContentType=text/xml, ResponseCode=200
    |> DefaultConnegRules-row QueryString=NULL, format=Form Post, Accepts=*/*, ContentType=application/json, ResponseCode=200
    |> DefaultConnegRules-row QueryString=NULL, format=Json, Accepts=random/format, ContentType=text/plain, ResponseCode=406

~~~
