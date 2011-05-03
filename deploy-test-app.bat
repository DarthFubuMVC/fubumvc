bottles init -f

bottles init-pak src\FubuMVC.WebForms FubuWebForms -role binaries -noweb -f
bottles add-pak fubuwebforms

bottles init-pak src\FubuMVC.Core FubuMVC -role binaries -noweb -f
bottles assemblies add fubumvc
bottles add-pak fubumvc

bottles init-pak src\FubuTestApplication TestApp -role application -f
bottles add-pak testapp

bottles add-recipe baseline
bottles ref baseline web FubuMVC
bottles ref baseline web FubuWebForms

bottles create-all -target debug
