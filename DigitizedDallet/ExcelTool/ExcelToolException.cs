namespace DigitizedDallet.ExcelTool;

public class ExcelToolException : Exception
{
    public ExcelToolException(string message, Exception? innerEx = null)
        : base(message, innerEx)
    {

    }
}