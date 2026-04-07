using System.Globalization;
using Microsoft.PowerFx;
using Microsoft.PowerFx.Types;
using PublicHoliday;

namespace MinimalSamples.FxExtentions;

public class AddBusinessDaysFunction : ReflectionFunction
{
    private USAPublicHoliday _holidaysEngine = new();
    public AddBusinessDaysFunction()
        : base("AddBusinessDays", FormulaType.DateTime ,[ FormulaType.DateTime, FormulaType.Number ])
    {
    }

    public FormulaValue Execute(DateTimeValue fromDate, NumberValue days)
    {
        // get value from powerfx to .net
        var input = fromDate.GetConvertedValue(TimeZoneInfo.Local);
        var numDaysToAdd = Convert.ToInt32(days.Value);
        
        // .net logic
        var result = _holidaysEngine.BusinessDaysAdd(input, numDaysToAdd); 
        
        // return value to powerfx
        return FormulaValue.New(result);
    }
}