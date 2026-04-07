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
        engine.SetFormula("MyOtherCustomer","{ Name: \"Peter\", CustomerId: 2, StartDate: DateValue(\"2021-10-05\")  }", OnFormulaUpdate);
        engine.SetFormula("PizzaCustomer","ParsePizzaCode(MyPizzaCode)", OnFormulaUpdate);
        // engine.SetFormula("PizzaCustomerStartDate","Day(PizzaCustomer.StartDate)", OnFormulaUpdate);
        engine.SetFormula("PizzaCustomerStartDate","Day(MyOtherCustomer.StartDate)", OnFormulaUpdate);
        
        var resultPizzaCustomer = engine.Eval("PizzaCustomer");
        // var resultStartDate = engine.Eval("PizzaCustomerStartDate"); //The value could not be interpreted as a color.
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
            // Assert.IsNotType<ErrorValue>(resultStartDate);
        }
        // _testOutputHelper.WriteLine(resultPizzaCustomer.ToObject()..StartDate);
        // _testOutputHelper.WriteLine(resultStartDate.ToObject().ToString());
        // _testOutputHelper.WriteLine(resultCustomerName.ToObject().ToString());
        // _testOutputHelper.WriteLine(resultCustomerId.ToObject().ToString());
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