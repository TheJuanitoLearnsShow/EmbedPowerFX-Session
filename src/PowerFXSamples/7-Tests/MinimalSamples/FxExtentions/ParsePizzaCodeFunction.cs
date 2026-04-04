using Microsoft.PowerFx;
using Microsoft.PowerFx.Types;
using PublicHoliday;

namespace MinimalSamples.FxExtentions;

public class ParsePizzaCodeFunction : ReflectionFunction
{
    public ParsePizzaCodeFunction()
        : base("ParsePizzaCode", FormulaType.UntypedObject ,[ FormulaType.String ])
    // FormulaType.Deferred is not supported by the PowerFX interpreter
    {
    }

    public FormulaValue Execute(StringValue pizzaCode)
    {
        // From PowerFX to .NET type 
        var pizzaCodeStr = pizzaCode.Value;
        
        // regular .NET logic
        var codeParts = pizzaCodeStr.Split("-");
        var startDt = DateTime.ParseExact(codeParts[0], "yyyyMMdd", null);
        var customerName = codeParts[1];
        var customerId = int.Parse(codeParts[2]);
        
        // From .NET to PowerFX type 
        var record = FormulaValue.NewRecordFromFields(
            new NamedValue("StartDate", FormulaValue.New(startDt)),
            new NamedValue("CustomerName",  FormulaValue.New(customerName)),
            new NamedValue("CustomerId",  FormulaValue.New(customerId))
        );
        return record;
    }
}