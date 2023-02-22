using ObjectBinderDemo.Models;

namespace ObjectBinderDemo.Common
{
    public static class DataSource
    {
        public static List<ParentModel> DataList = new List<ParentModel>
        {
           new ChildrenModel1
           {
               Id = 1,
               Name = "a",
               Address= "b",
           },
           new ChildrenModel2
           { 
               Id = 2,
               Name = "c",
               Company= "d"
           }
        };
    }
}
