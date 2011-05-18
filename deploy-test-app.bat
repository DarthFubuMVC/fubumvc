bottles init -f

bottles init-pak src\FubuMVC.WebForms FubuWebForms -role binaries -noweb -f
bottles add-pak fubuwebforms

bottles init-pak src\FubuMVC.Core FubuMVC -role binaries -noweb -f
bottles assemblies add fubumvc
bottles add-pak fubumvc

bottles init-pak src\FubuTestApplication TestApp -role application -f
bottles add-pak testapp

bottles add-recipe baseline
bottles ref-bottle baseline web FubuMVC
bottles ref-bottle baseline web FubuWebForms
bottles ref-bottle baseline web FubuTestApp

fubu create-deployment FubuTestApp -recipe baseline -host web

bottles set-env-prop virtual-dir=FubuTestApp
bottles set-env-prop app-pool=FubuTestApp
bottles set-env-prop root=FubuTestApp
bottles set-env-prop website-name=FubuTestApp

bottles ref-recipe default baseline


bottles set-profile-prop second virtual-dir=FubuDifferent
bottles set-host-prop baseline web Website.DirectoryBrowsing=Enable

bottles create-all -target debug
