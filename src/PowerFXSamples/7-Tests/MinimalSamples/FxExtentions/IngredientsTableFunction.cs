using Microsoft.PowerFx;
using Microsoft.PowerFx.Types;

namespace MinimalSamples.FxExtentions;

public class IngredientsTableFunction : ReflectionFunction
{
    // needed for the syntax checker to know the structure of the resulting table
    private static readonly TableType TableType = TableType.Empty()
            .Add(new NamedFormulaType("Name", FormulaType.String))
            .Add(new NamedFormulaType("Quantity", FormulaType.Decimal))
            .Add(new NamedFormulaType("Cost", FormulaType.Decimal))
        ;
    private static readonly RecordType RecordType = TableType.ToRecord();
    public IngredientsTableFunction()
        : base("GetIngredients", TableType ,[ FormulaType.String ])
    {
    }

    public FormulaValue Execute(StringValue recipeName)
    {
        // From PowerFX to .NET type 
        string recipeNameStr = recipeName.Value;
        
        // From .NET to PowerFX type 
        List<RecordValue> rows = recipeNameStr.ToLowerInvariant() switch
        {
            "usual-h" =>
            [
                NewIngredientRecord("Pepperoni", 12, 1.23M),
                NewIngredientRecord("Cheese", 20, 0.23M),
            ],
            "best-p" =>
            [
                NewIngredientRecord("Ham", 5, 0.23M),
                NewIngredientRecord("Pinneapple", 20, 1.23M),
            ],
            _ => 
            [
                NewIngredientRecord("Cheese", 10, 0.50M)
            ]
        };
        
        return FormulaValue.NewTable(RecordType, rows);
    }

    private static RecordValue NewIngredientRecord(string name, int quantity, decimal cost)
    {
        return FormulaValue.NewRecordFromFields(
            new NamedValue("Name", FormulaValue.New(name)),
            new NamedValue("Quantity",  FormulaValue.New(quantity)),
            new NamedValue("Cost",  FormulaValue.New(cost))
        );
    }
}