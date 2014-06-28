using HtmlTags;

namespace FubuMVC.Core.Diagnostics
{
    public class AutoReloadingTag : HtmlTag
    {
        public AutoReloadingTag() : this("/_loaded", 1000)
        {
        }

        public AutoReloadingTag(string url, int interval)
            : base("script")
        {
            if (!FubuMode.InDevelopment())
            {
                Render(false);
            }

            Attr("language", "javascript");

            Encoded(false);

            var js = @"
      function FubuAppWatcher(url, interval){
        var self = this;

        self.lastValue = null;
        
        self.check = function(){
          xhr.open('GET', url, true);
          xhr.send();
        };
        
        var xhr = new XMLHttpRequest();
        xhr.onreadystatechange = function(){
          if (xhr.readyState==4 && xhr.status==200){
            if (self.lastValue == null){
              self.lastValue = xhr.responseText;
            }
            else if (self.lastValue != xhr.responseText){
              location.reload(true);
              return;
            }
            
            window.setTimeout(self.check, interval);
          }
        };
        
        
        window.setTimeout(self.check, interval);
        return self;
      }

      var watcher = new FubuAppWatcher('{URL}', {INTERVAL});
      watcher.start();
";

            js = js.Replace("{URL}", url).Replace("{INTERVAL}", interval.ToString());


            Text(js);
        }
    }
}