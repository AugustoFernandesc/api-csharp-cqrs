namespace Shared.Pagination;

public class PagedFilterRequest
{
    public string Column { get; set; } = string.Empty;
    public OperatorType Operator { get; set; }
    public string? Value { get; set; }
    public DataType DataType { get; set; }

    // Construtor e setters publicos permitem que o model binder monte Filters vindos da query string.
    public PagedFilterRequest() { }

    public static PagedFilterRequest Create(string column, OperatorType operatorType, string? value, DataType dataType)
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
