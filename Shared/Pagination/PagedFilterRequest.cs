namespace Shared.Pagination;

public class PagedFilterRequest
{
    public string Column { get; private set; } = string.Empty;
    public OperatorType Operator { get; private set; }
    public object Value { get; private set; } = null!;
    public DataType DataType { get; private set; }

    private PagedFilterRequest() { }

    public static PagedFilterRequest Create(string column, OperatorType operatorType, object value, DataType dataType)
    {
        return new PagedFilterRequest
        {
            Column = column,
            Operator = operatorType,
            Value = value,
            DataType = dataType
        };
    }

}
