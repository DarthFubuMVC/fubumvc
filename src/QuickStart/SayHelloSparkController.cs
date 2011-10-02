namespace QuickStart
{
    public class SayHelloSparkController
    {
        public SparkHelloViewModel get_spark_my_name_is_Name(SparkNameModel input)
        {
            return new SparkHelloViewModel
                   {
                       Name = input.Name
                   };
        }

        public SparkNoMasterHelloViewModel get_nomaster_my_name_is_Name(SparkNoMasterNameModel input)
        {
            return new SparkNoMasterHelloViewModel
                   {
                       Name = input.Name
                   };
        }
    }

    public class SparkNoMasterNameModel
    {
        public string Name { get; set; }
    }

    public class SparkNoMasterHelloViewModel
    {
        public string Name { get; set; }
    }

    public class SparkNameModel
    {
        public string Name { get; set; }
    }

    public class SparkHelloViewModel
    {
        public string Name { get; set; }
    }
}
