namespace API.ML.CustomAtrributes
{

    [AttributeUsage(AttributeTargets.Property)]
    public class GetDetail : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class GetAll : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class GetDataPagingInclude : Attribute
    {
    }
}
