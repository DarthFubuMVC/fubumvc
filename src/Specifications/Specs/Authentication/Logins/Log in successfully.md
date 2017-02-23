# Log in successfully

-> id = 6fc8ae42-e9d1-40c5-bcea-b419df71c8e2
-> lifecycle = Regression
-> max-retries = 0
-> last-updated = 2017-02-23T15:45:01.0147952-06:00
-> tags = 

[Model]
|> UsersAre
    [table]
    |> row UserName=jeremy, Password=jasper

~~~

[LoginScreen]
|> OpenLoginScreen
|> Login user=jeremy, password=jasper
|> IsNotOnTheLoginScreen
~~~
