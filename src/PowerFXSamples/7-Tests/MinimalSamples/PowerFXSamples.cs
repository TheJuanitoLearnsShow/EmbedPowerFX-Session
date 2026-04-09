using Microsoft.PowerFx;
using Microsoft.PowerFx.Types;
using MinimalSamples.FxExtentions;
using Xunit.Abstractions;

namespace MinimalSamples;

public class PowerFXSamples
{
    private readonly ITestOutputHelper _testOutputHelper;

    public PowerFXSamples(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void Starter_Test()
    {
        var engine = new RecalcEngine();
// notice the escaped quotes as we are passing a PowerFX string literal. Our second parameter needs to be 
//  a valid PowerFX expression.
        engine.SetFormula("MyName", "\"Juanito\"", OnFormulaUpdate); 
    
        // engine.SetFormula("MyName", "\"Juanito\"", OnFormulaUpdate); // throws error because MyName is already defined (System.InvalidOperationException: MyName is already defined)
   
        engine.SetFormula("Greeting", "\"Hello, \" & MyName & \"!\"", OnFormulaUpdate);
        engine.SetFormula("EmailTxt", "\"jj@gamil.com\"", OnFormulaUpdate); 
        engine.SetFormula("IsEmail", 
            """
            If(
                Find(
                   "@",
                   EmailTxt
                ),
                true,
                false
            )
            """, OnFormulaUpdate); 
        var greetingResult = engine.Eval("Greeting");
        var isEmailResult = engine.Eval("IsEmail");
        _testOutputHelper.WriteLine(greetingResult.ToObject().ToString());
        _testOutputHelper.WriteLine(isEmailResult.ToObject().ToString());
    
        // 2nd Example
        engine.UpdateVariable("Velocity", 20);
        engine.SetFormula("Mass", "2", OnFormulaUpdate);
        engine.SetFormula("KineticEnergy", "(1/2) * Mass * Velocity * Velocity", OnFormulaUpdate);
        var energyResult = engine.GetValue("KineticEnergy");
        _testOutputHelper.WriteLine($"KineticEnergy: {ValueFormatter.ToDisplayOutput(energyResult)}");
    }
    [Fact]
    public void CustomFunction_Primitive_Test()
    {
        var engineConfig = new PowerFxConfig();
        engineConfig.AddFunction(new AddBusinessDaysFunction());
        
        var engine = new RecalcEngine(engineConfig);
        
        var sampleDateInput = new DateTime(2026, 1, 1);
        engine.UpdateVariable("ContactDate", sampleDateInput);
        engine.SetFormula("OnboardDeadline","AddBusinessDays(ContactDate, 10)", OnFormulaUpdate);
        engine.SetFormula("HasDeadlinePassed","If ( Today() > OnboardDeadline , true, false)", OnFormulaUpdate);
        
        _testOutputHelper.WriteLine("HasDeadlinePassed:");
        var result = engine.Eval("HasDeadlinePassed"); // true
        _testOutputHelper.WriteLine(result.ToObject().ToString());
    }

    [Fact]
    public void CustomFunction_Record_Test()
    {
        var engineConfig = new PowerFxConfig();
        engineConfig.AddFunction(new ParsePizzaCodeFunction());
        
        var engine = new RecalcEngine(engineConfig);
        
        engine.UpdateVariable("MyPizzaCode", "20020104-JOY-777");
        engine.SetFormula("PizzaCustomer","ParsePizzaCode(MyPizzaCode)", OnFormulaUpdate);
        engine.SetFormula("Greeting","\"Hi \" & PizzaCustomer.CustomerName", OnFormulaUpdate);
        
        var resultPizzaCustomer = engine.Eval("PizzaCustomer");
        var greeting = engine.Eval("Greeting");
        var resultStartDay = engine.Eval("Day(PizzaCustomer.StartDate)"); //The value could not be interpreted as a color.
        
        
        var propertiesDict = (IDictionary<string, object>)resultPizzaCustomer.ToObject();
        _testOutputHelper.WriteLine("StartDate:");
        _testOutputHelper.WriteLine(propertiesDict["StartDate"].ToString());
        _testOutputHelper.WriteLine("CustomerName:");
        _testOutputHelper.WriteLine(propertiesDict["CustomerName"].ToString());
        _testOutputHelper.WriteLine("CustomerId:");
        _testOutputHelper.WriteLine(propertiesDict["CustomerId"].ToString());
        
        _testOutputHelper.WriteLine("resultStartDay:");
        _testOutputHelper.WriteLine(resultStartDay.ToObject().ToString());
        _testOutputHelper.WriteLine("greeting:");
        _testOutputHelper.WriteLine(greeting.ToObject().ToString());

        if (resultStartDay is ErrorValue errorValue)
        {
            var errorMessages = string.Join(" | ", errorValue.Errors.Select(e => e.Message));
            _testOutputHelper.WriteLine($"Error: {errorMessages}");
        }
    }
    
    [Fact]
    public void CustomFunction_Table_Test()
    {
        var engineConfig = new PowerFxConfig();
        engineConfig.AddFunction(new IngredientsTableFunction());
        
        var engine = new RecalcEngine(engineConfig);
        
        engine.UpdateVariable("PizzaSelection", "usual-h");
        engine.SetFormula("PizzaIngredients","GetIngredients(PizzaSelection)", OnFormulaUpdate);
        engine.SetFormula("TotalCost","Sum(PizzaIngredients As r, r.Cost )", OnFormulaUpdate);
        
        var resultTotalCost = engine.Eval("TotalCost");
        var resultTable = engine.Eval("PizzaIngredients");
        
        _testOutputHelper.WriteLine("TotalCost:");
        _testOutputHelper.WriteLine(ValueFormatter.ToDisplayOutput(resultTotalCost));
        _testOutputHelper.WriteLine("PizzaIngredients:");
        _testOutputHelper.WriteLine(ValueFormatter.ToDisplayOutput(resultTable));
    }
    
    [Fact]
    public async Task CustomFunction_Async_Test()
    {
        var engineConfig = new PowerFxConfig();
        engineConfig.AddFunction(new AsyncProductTagLineSearch());
        
        var engine = new RecalcEngine(engineConfig);
        
        engine.UpdateVariable("PizzaSelection", "Ham");
        engine.SetFormula("TagLine","GetProductTagLine(PizzaSelection)", OnFormulaUpdate);
        
        var resultTagLine = await engine.EvalAsync("TagLine", CancellationToken.None);
        
        _testOutputHelper.WriteLine("TagLine:");
        _testOutputHelper.WriteLine(ValueFormatter.ToDisplayOutput(resultTagLine));
    }
    private void OnFormulaUpdate(string arg1, FormulaValue arg2)
    {
    }
}