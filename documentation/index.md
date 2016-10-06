<!--Title:FubuMVC-->

<[img:/content/images/logo.jpg;]>

FubuMVC is a .Net framework for enabling teams to efficiently build server side systems. FubuMVC has support for:

* <[linkto:documentation/http;title=HTTP services]>. Despite the poor choice of name, FubuMVC *is not* a traditional Model 2 MVC framework
* <[linkto:documentation/servicebus;title=Distributed messaging systems]>
* <[linkto:documentation/jobs;title=Scheduled or polling job execution]>

FubuMVC's guiding philosophy from day one was to minimize the amount of ceremonial code cruft and coupling to the framework that most
other frameworks of that time (and now) forced down developer's throats. We strove to utilize "convention over configuration" as much 
as possible to further simplify the code. At the same time, FubuMVC put a high emphasis on designing its internals with composition to 
enable the most effective extensibility and modularity solution in the .Net space. Finally, FubuMVC was built with the idea that users
would want to use Test Driven Development and other forms of automated test support.

## Roadmap

1. Official 3.0 Release in December 2016. We're really just waiting for the latest bits to run in production for awhile longer without any issues before 
   making that official.
1. A stopgap 4.0 release in early 2017. The only change is that the entire runtime pipeline will be "async by default" and some other performance related
   improvements. This work is already done in this branch, but won't be used until 2017.
1. "[Jasper]()" will be an all new framework built on the CoreCLR and ASP.Net Core that attempts to retain the best parts of FubuMVC while providing a much more
   performant runtime model, usability improvements, and far better interoperability with the mainstream ASP.Net Core tools.

## History

FubuMVC was conceived as an addon to the nascent ASP.Net MVC framework during a large web application project in 2008-9 that [Chad Myers and Jeremy Miller presented at KaizenConf](http://kaizenconf.pbworks.com/w/page/8900249/Using%20and%20Abusing%20ASPNET%20MVC%20for%20Fun%20and%20Profit). 
The project team building and using it theorized that they could be more productive, both in coding and in testing, if they had patterns for building HTTP endpoints that required
far less coupling to the underlying framework. After becoming increasingly frustrated with our efforts to customize ASP.Net MVC,
FubuMVC ("for us, by us") [was rebooted as a separate web framework](http://codebetter.com/jeremymiller/2009/12/16/an-update-on-the-fubumvc-reboot) 
and went into production for the first time in 2010.

The service bus part of FubuMVC, originally called "FubuTransportation" was built as an addon in 2013 to utilize FubuMVC's modular runtime
pipeline for distributed messaging systems. The basic FubuMVC "behaviors" pipeline was later adopted by NServiceBus.

For a brief time FubuMVC was a vibrant OSS project with an engaged community and a [sprawling ecosystem](https://jeremydmiller.com/2012/11/16/a-high-level-look-at-the-fubumvc-ecosystem/).
The core team made a "big" 1.0 reveal at Codemash 2012 to an almost empty room -- and that was really just about it for FubuMVC as an OSS project. It limped on for another
year before the core team basically decided to [throw in the towel](https://jeremydmiller.com/2014/04/03/im-throwing-in-the-towel-in-fubumvc/).

FubuMVC is still heavily used internally at at least two shops, so in the summer of 2015 we embarked on the current FubuMVC 3.0 architecture as a
stopgap to improve FubuMVC until such time that we could replace it in our ecosystem. The 3.0 work consolidated all of the elements of
FubuMVC that were still used into a single code repository and collapsed what had been several separate extensions and the service bus into
the main `FubuMVC.Core` Nuget.


