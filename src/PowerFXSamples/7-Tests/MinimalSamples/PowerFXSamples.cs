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
                IsMatch(
                    EmailTxt,
                   "^[0-9]$"
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
        _testOutputHelper.WriteLine($"KineticEnergy: {ValueFormatter.ToDisplayOutput(greetingResult)}");
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
        
        var result = engine.Eval("HasDeadlinePassed");
        _testOutputHelper.WriteLine(result.ToObject().ToString());
    }

    [Fact]
    public void CustomFunction_Record_Test()
    {
        var engineConfig = new PowerFxConfig();
        engineConfig.AddFunction(new ParsePizzaCodeFunction());
        
        var engine = new RecalcEngine(engineConfig);
        
        engine.UpdateVariable("MyPizzaCode", "20020104-JOY-777");
        // engine.SetFormula("MyOtherCustomer","{ Name: \"Peter\", CustomerId: 2, StartDate: DateValue(\"2021-10-05\")  }", OnFormulaUpdate);
        engine.SetFormula("PizzaCustomer","ParsePizzaCode(MyPizzaCode)", OnFormulaUpdate);
        // engine.SetFormula("PizzaCustomerStartDate","Day(PizzaCustomer.StartDate)", OnFormulaUpdate);
        // engine.SetFormula("PizzaCustomerStartDate","Day(MyOtherCustomer.StartDate)", OnFormulaUpdate);
        
        var resultPizzaCustomer = engine.Eval("PizzaCustomer");
        var resultStartDay = engine.Eval("Day(PizzaCustomer.StartDate)"); //The value could not be interpreted as a color.
        
        
        var propertiesDict = (IDictionary<string, object>)resultPizzaCustomer.ToObject();
        _testOutputHelper.WriteLine(propertiesDict["StartDate"].ToString());
        _testOutputHelper.WriteLine(propertiesDict["CustomerName"].ToString());
        _testOutputHelper.WriteLine(propertiesDict["CustomerId"].ToString());
        
        _testOutputHelper.WriteLine(resultStartDay.ToObject().ToString());

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
        
        var resultTable = engine.Eval("PizzaIngredients");
        var resultTotalCost = engine.Eval("TotalCost");
        
        _testOutputHelper.WriteLine(ValueFormatter.ToDisplayOutput(resultTotalCost));
        _testOutputHelper.WriteLine(ValueFormatter.ToDisplayOutput(resultTable));
    }
    private void OnFormulaUpdate(string arg1, FormulaValue arg2)
    {
    }
}