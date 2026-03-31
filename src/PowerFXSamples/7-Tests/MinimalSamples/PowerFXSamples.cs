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
    public void PrimitiveCustomFunctionTest()
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

    private void OnFormulaUpdate(string arg1, FormulaValue arg2)
    {
    }
}